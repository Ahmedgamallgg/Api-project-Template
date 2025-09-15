# Infrastructure.Presentation (Web Delivery – Controllers & Attributes)

Purpose
- Houses ASP.NET presentation concerns as a reusable library: controllers, filters/attributes, model binders.
- Keeps Api.Web thin: the host references this library and maps its controllers.

Contents
- Controllers/: APIController.cs and other endpoints.
- Attributes/: cross-cutting web attributes (e.g., RedisCacheAttribute).
- Presentation.csproj – targets ASP.NET Core.

What belongs here
- Controller actions that call Core.ServicesAbstractions.
- Filters, attributes, model validators tied to ASP.NET.
- HTTP-only DTOs and response shapes (if not shared).

What does NOT belong here
- Business logic or data access (call services instead).
- EF Core, repositories, or DbContext.

Dependencies
- Depends on Core.ServicesAbstractions and Shared for DTOs/options.
- Should not depend on Infrastructure.Persistence (keep web decoupled from data tech).

Outcome
- A clean web delivery layer you can version and reuse across different hosts (Api.Web, integration tests, etc.).
