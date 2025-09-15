# Shared (Cross-Cutting Types)

Purpose
- Framework-agnostic, reusable primitives and options shared across layers.
- Keep these simple and dependency-light.

Contents
- ErrorModels/: ErrorDetails, ValidationErrorResponse – standardized problem shapes.
- Options/: JWTOptions – strongly typed configuration objects.
- PaginatedResponse – pagination envelope generics for list endpoints.

What belongs here
- POCOs used by multiple layers (DTOs/options/response envelopes).
- Small helpers that have no framework coupling.

What does NOT belong here
- EF Core or ASP.NET Core types.
- Business logic or repository code.

Dependencies
- Should target a low runtime with minimal dependencies.

Outcome
- Consistent models/options that reduce duplication and align responses across services.
