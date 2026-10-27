using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Repositories;

namespace TestProject
{
    /// <summary>
    /// Runs the real API in memory against its own throwaway database, so
    /// requests go through routing, authentication and authorization.
    /// </summary>
    public class ApiFactory : WebApplicationFactory<Program>
    {
        private readonly string _connectionString =
            $"Data Source={Environment.GetEnvironmentVariable("TEST_DB_SERVER") ?? @"(localdb)\MSSQLLocalDB"};" +
            $"Initial Catalog=ApiShopAuthTest_{Guid.NewGuid():N};Integrated Security=True;TrustServerCertificate=True";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
            builder.UseSetting("ConnectionStrings:ApiShop", _connectionString);
            // The background jobs call the AI service and update orders; tests need neither
            builder.ConfigureTestServices(services => services.RemoveAll<IHostedService>());
        }

        /// <summary>A context on the test database that works even after the app has stopped.</summary>
        public ApiShopContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApiShopContext>().UseSqlServer(_connectionString).Options);

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                using var db = CreateDbContext();
                db.Database.EnsureDeleted();
            }
        }
    }

    public class AuthorizationTests : IClassFixture<ApiFactory>
    {
        private const string Password = "Vacation#2026-Strong";
        private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);
        private readonly ApiFactory _factory;

        public AuthorizationTests(ApiFactory factory)
        {
            _factory = factory;
            _factory.CreateDbContext().Database.EnsureCreated();
        }

        private record SignedIn(int Id, bool IsAdmin, string Token);

        /// <summary>Signs up a new user through the API and signs them in.</summary>
        private async Task<SignedIn> SignUp(bool admin = false, bool askForAdmin = false)
        {
            var client = _factory.CreateClient();
            string email = $"user{Guid.NewGuid():N}@example.test";
            var signUp = await client.PostAsJsonAsync("/api/Users",
                new { email, firstName = "Test", lastName = "User", password = Password, isAdmin = askForAdmin });
            Assert.Equal(HttpStatusCode.Created, signUp.StatusCode);

            if (admin)
            {
                using var db = _factory.CreateDbContext();
                var user = await db.Users.SingleAsync(u => u.Email == email);
                user.IsAdmin = true;
                await db.SaveChangesAsync();
            }

            var login = await client.PostAsJsonAsync("/api/Users/Login", new { email, password = Password });
            login.EnsureSuccessStatusCode();
            return (await login.Content.ReadFromJsonAsync<SignedIn>(Json))!;
        }

        private HttpClient ClientFor(SignedIn? user)
        {
            var client = _factory.CreateClient();
            if (user != null)
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
            return client;
        }

        private static object NewCategory() => new { categoryId = 0, categoryName = $"Cat {Guid.NewGuid():N}"[..20] };

        [Fact]
        public async Task Guest_CanBrowseProducts()
        {
            var response = await ClientFor(null).GetAsync("/api/Products?position=1&skip=10");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Guest_CannotAddCategory()
        {
            var response = await ClientFor(null).PostAsJsonAsync("/api/Categories", NewCategory());
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task User_CannotAddCategory()
        {
            var user = await SignUp();
            var response = await ClientFor(user).PostAsJsonAsync("/api/Categories", NewCategory());
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Admin_CanAddCategory()
        {
            var admin = await SignUp(admin: true);
            var response = await ClientFor(admin).PostAsJsonAsync("/api/Categories", NewCategory());
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task User_SeesOnlyTheirOwnOrders()
        {
            var alice = await SignUp();
            var bob = await SignUp();

            var own = await ClientFor(alice).GetAsync($"/api/Orders/user/{alice.Id}");
            var others = await ClientFor(alice).GetAsync($"/api/Orders/user/{bob.Id}");

            Assert.Equal(HttpStatusCode.OK, own.StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, others.StatusCode);
        }

        [Fact]
        public async Task User_CannotEditAnotherUser()
        {
            var alice = await SignUp();
            var bob = await SignUp();

            var response = await ClientFor(alice).PutAsJsonAsync($"/api/Users/{bob.Id}",
                new { id = bob.Id, firstName = "Hacked", lastName = "User", email = "x@example.test", password = "", isAdmin = false });

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task User_CannotMakeThemselvesAdmin()
        {
            var user = await SignUp();
            await ClientFor(user).PutAsJsonAsync($"/api/Users/{user.Id}",
                new { id = user.Id, firstName = "Test", lastName = "User", email = $"self{Guid.NewGuid():N}@example.test", password = "", isAdmin = true });

            using var db = _factory.CreateDbContext();
            Assert.False((await db.Users.FindAsync(user.Id))!.IsAdmin);
        }

        [Fact]
        public async Task Guest_CannotSignUpAsAdmin()
        {
            var user = await SignUp(askForAdmin: true);
            Assert.False(user.IsAdmin);
        }
    }
}
