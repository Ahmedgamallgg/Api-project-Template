# Core.ServicesAbstractions (Application Contracts)

Purpose
- Defines *interfaces* for the application layer (e.g., IServiceManager, ICasheService).
- Allows Web/Presentation to depend on contracts without knowing implementations.

What belongs here
- Service interfaces and DTO contracts that app services expose.
- Marker interfaces or cross-service contracts used by Presentation.

What does NOT belong here
- Implementations, data access, or framework specifics.

Dependencies
- Depends on Shared types as needed (DTO).
- No dependency on Infrastructure.* or Web.

Outcome
- Stable seam for DI and testing: Presentation and other callers code to interfaces; Infrastructure and Core.Services plug in implementations.
