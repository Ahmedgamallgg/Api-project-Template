# Core.Domain (Enterprise/Domain Layer)

Purpose
- The heart of the system: entities, value objects, domain rules, and domain contracts that describe *what* the system does.
- Technology-agnostic. No EF Core, no ASP.NET types.

Contents
- Models/: Domain entities (e.g., ApplicationUser, BaseEntity).
- Contracts/: Abstractions that describe persistence and querying (IGenericRepository, IUnitOfWork, ISpecifications) and environment-agnostic services.
- Exceptions/: Domain/application-level exceptions (BadRequestException, NotFoundException, UnauthorizedException).

What belongs here
- Pure C# domain types and invariants and POCO Classes.
- Repository/UnitOfWork/specification *interfaces* (not implementations).
- Exception types shared across layers.

What does NOT belong here
- EF Core DbContext, LINQ-to-Entities specifics, SQL, HTTP, caching providers, or any tech detail.

Dependencies
- Should reference nothing outside Core and BCL. This layer must not depend on Infrastructure or Web.

Outcome
- Stable, testable, framework-agnostic domain core that all other layers orbit around.
