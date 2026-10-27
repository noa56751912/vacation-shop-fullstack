using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Entities;
using AutoMapper;
using DTOs;
using WebApiShop.Extensions;

namespace WebApiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersServices _ordersServices;
        public OrdersController(IOrdersServices ordersServices, IMapper mapper)
        {
            _ordersServices = ordersServices;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDTO>> Get(int id)
        {
            OrderDTO? order = await _ordersServices.GetOrderById(id);
            if (order == null)
                return NotFound();
            if (!User.CanAccess(order.UserId))
                return Forbid();
            return Ok(order);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<OrderDTO>>> GetOrdersByUserId(int userId)
        {
            if (!User.CanAccess(userId))
                return Forbid();
            var orders = await _ordersServices.GetOrdersByUserId(userId);
            return Ok(orders);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<OrderDTO>>> GetAll()
        {
            var orders = await _ordersServices.GetAllOrders();
            return Ok(orders);
        }

        [HttpGet("dashboard-stats")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DashboardStatsDTO>> GetDashboardStats()
        {
            var stats = await _ordersServices.GetDashboardStats();
            return Ok(stats);
        }

        [HttpPost]
        public async Task<ActionResult<OrderDTO>> Post([FromBody] OrderDTO newOrder)
        {
            Console.WriteLine($"=== ORDER CREATION REQUEST ===");

            try
            {
                // The order always belongs to whoever is signed in, whatever the body says
                newOrder = newOrder with { UserId = User.GetUserId() };

                newOrder = await _ordersServices.AddOrder(newOrder);

                if (newOrder == null)
                    return BadRequest("Failed to create order");

                return CreatedAtAction(nameof(Get), new { id = newOrder.OrderId }, newOrder);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR creating order: {ex.Message}");
                return StatusCode(500, new { error = ex.Message, innerError = ex.InnerException?.Message });
            }
        }
    }
}
