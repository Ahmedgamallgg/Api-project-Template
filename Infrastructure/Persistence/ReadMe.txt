# Infrastructure.Persistence (Data Access – EF Core)

Purpose
- Implements repository/specification/unit-of-work abstractions using EF Core.
- Owns the DbContext, migrations, and data initialization (DbInitializer).

Contents
- Data/AppDbContext.cs – EF Core context and configuration.
- Repositories/: GenericRepository, UnitOfWork, SpecificationsEvaluator.
- InfrastructureServicesRegistration – DI wiring for persistence (DbContext, repositories, UoW).
- DbInitializer – seeding/bootstrap utilities.

What belongs here
- All EF Core specifics (OnModelCreating, migrations, tracking).
- Concrete repository implementations that return Domain entities.
- Translating Specification objects into EF queries (SpecificationsEvaluator).

What does NOT belong here
- Controller code, HTTP abstractions, or business workflows.
- Returning EF types to upper layers (keep EF internal).

Dependencies
- Depends on Core.Domain contracts/entities; implements them.
- No dependency on Api.Web. Presentation can *use* this via DI only.

Outcome
- A contained persistence layer you can swap/migrate (SQL Server → PostgreSQL) with minimal ripple effect.
