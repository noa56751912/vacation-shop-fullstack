# Dreams Escapes: full-stack vacation store with an AI assistant

An online store for vacation packages with three parts: an Angular storefront and admin area, an ASP.NET Core API, and a Python AI service that powers a travel chat assistant, semantic search and a "Dream Vacation" generator.

## Parts

| Folder | What it is |
|---|---|
| `client/` | Angular 21 single-page app: product browsing and filters, cart and checkout, orders, an admin area (dashboard, products, categories, orders), the chat assistant and the Dream Vacation page |
| `server/` | ASP.NET Core Web API in layers (API, Services, Repositories, Entities, DTOs) with Entity Framework Core, AutoMapper, NLog, and xUnit unit and integration tests |
| `ai-service/` | FastAPI micro-service. Called only by the .NET backend, never directly by the browser. Details in [ai-service/README.md](ai-service/README.md) |

## AI features

- **Chat assistant:** answers questions about destinations and store policies using retrieval over the product catalog and the policy documents (`POST /chat`)
- **Semantic search:** embedding-based search over the catalog (`POST /index`, `POST /search`)
- **Dream Vacation generator:** a free-text, voice or image description of a trip is turned into a structured vacation profile and matched against packages in a vector database (`/dream-vacation/analyze/text|audio|image`, `/dream-vacation/search`)
- Built with OpenAI (`gpt-4o`, `text-embedding-3-small`), Qdrant Cloud and Pydantic structured output

## Backend highlights

- Three-layer architecture wired with dependency injection
- Fully async data access
- Global error-handling middleware and structured logging
- Unit and integration tests for the Category, Product, Order and User repositories

## Run it

Each part runs in its own terminal. The client talks to the API through a
dev proxy, and the API talks to the AI service on port 8001.

```bash
# 1. Database + API (needs the .NET 9 SDK and SQL Server; LocalDB works)
cd server
dotnet tool install --global dotnet-ef          # once
dotnet ef database update --project Repositories --startup-project WebApiShop
dotnet run --project WebApiShop --launch-profile http   # http://localhost:5000

# 2. AI service (optional: without OPENAI_API_KEY it answers 503 and the store still works)
cd ai-service
python -m venv .venv && source .venv/bin/activate     # Windows: .venv\Scripts\activate
pip install -r requirements.txt
cp .env.example .env                                  # then fill in the keys
python -m uvicorn chat_service:app --port 8001

# 3. Client
cd client
npm install
npm start                                             # http://localhost:4200
```

The first `database update` creates the schema and seeds 4 categories and 20
destinations with photos and seasonal prices. The connection string is in
`server/WebApiShop/appsettings.json` (`ConnectionStrings:ApiShop`).

Tests: `cd server && dotnet test` (they create and delete their own LocalDB
database; set `TEST_DB_SERVER` to use another SQL Server).

## Configuration

Secrets (JWT signing key, SMTP credentials, database connection string, API keys) belong in environment variables or the .NET user-secrets store, not in `appsettings.json`.

## Tech

Angular, TypeScript, C#, ASP.NET Core, Entity Framework Core, xUnit, Python, FastAPI, OpenAI API, Qdrant, SQL Server.
