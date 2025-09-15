# Core.Services (Application Layer – Implementations)

Purpose
- Implements use-cases/application workflows by orchestrating domain entities and repository abstractions.
- Encodes business processes, validation, and transactional boundaries (via IUnitOfWork).

Contents
- Services.csproj, ApplicationServicesRegistration (DI extension).
- ServiceManager: typed facade/aggregator for service interfaces.
- Specifications/: base specification helpers for querying through repositories (without leaking EF types).
- CacheService.cs (application-facing cache orchestration via abstractions).

What belongs here
- Use-case services that coordinate repositories, caches, and domain logic.
- Mapping and validation that are independent of web or persistence frameworks.
- Transactions (via IUnitOfWork) when needed.

What does NOT belong here
- Controller code, filters, or middleware.
- EF Core or concrete infrastructure code.
- Any direct network/IO provider code (should be abstracted).

Dependencies
- Depends on Core.Domain (for entities/contracts) and Core.ServicesAbstractions (for service interfaces).
- No dependency on Infrastructure.*.

Outcome
- A clean application layer you can test with in-memory repositories and stubs, independent of the web/persistence tech.
