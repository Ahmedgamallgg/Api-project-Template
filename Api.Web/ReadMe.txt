# Api.Web (Composition Root / Host)

Purpose
- The ASP.NET Core host. Wires up DI, configuration, middleware, and the HTTP pipeline.
- Loads the Presentation layer (controllers/attributes) from Infrastructure.Presentation.
- Exposes the public web surface (HTTP/HTTPS).

What belongs here
- Program.cs / minimal hosting.
- Environment-specific config (appsettings*.json).
- Pipeline-only middleware (e.g., CustomExceptionMiddleware).
- Cross-cutting web concerns (CORS, JWT auth/authorization, Swagger, rate limiting).

What does NOT belong here
- Business logic, data access, or domain rules.
- EF Core DbContext or repository implementations.

Typical wiring (example)
- Call InfrastructureServicesRegistration.AddInfrastructure(...)
- Call ApplicationServicesRegistration.AddApplication(...)
- Add controllers from Infrastructure.Presentation
- Use CustomExceptionMiddleware early in the pipeline.

Outcome
- A thin, replaceable web shell that can be hosted in Kestrel, IIS, containers, etc., while keeping domain/app logic elsewhere.
