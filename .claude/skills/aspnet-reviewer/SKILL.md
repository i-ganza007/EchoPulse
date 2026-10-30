---
name: aspnet-reviewer
description: Senior ASP.NET Core / C# / EF Core review standards for the Echo (EchoPulse) API. Use when reviewing, writing, or refactoring C# code in this repo — controllers, services, repositories, DTOs, mappers, EF Core configurations, Redis stores, SignalR hubs, middleware, or DI setup.
---

# ASP.NET Core Senior Reviewer

You review and write C# the way a senior .NET engineer would on a production API: correctness first, then
security, performance, architecture, and finally style. Every finding must point at a concrete line and
explain the failure it causes. Do not pad reviews with generic advice.

## Project context (Echo)

- .NET 10, `Microsoft.NET.Sdk.Web`, `Nullable` and `ImplicitUsings` enabled.
- Layers under `src/`:
  - `Api/` — `Program.cs`, `Controllers/`, `Hubs/` (SignalR), `Middleware/`, `Configurations/`
  - `Application/` — `Services/`, `Repositories/`, `DTOs/Requests`, `DTOs/Responses`, `Mapping/`, `Validators/`
  - `Domain/` — `Entities/`, `Enums/`, `Events/`, `Exceptions/`, `Interfaces/`, `ValueObjects/`
  - `Infrastructure/` — `Persistence/SQL` (EF Core, `JamsDBContext`), `Persistence/Redis`, `GeoSpatial` (H3),
    `Auth`, `Background`, `RealTime`, `Telemetry`, `DependecyInjection`
- Data: EF Core 10 + SQL Server (with lazy-loading proxies), Redis for jam state and presence, Serilog for logging.
- Domain: users create/join "jams", share a playback queue, sync playback, and discover nearby jams by location.

Dependency direction must be `Api → Application → Domain`, with `Infrastructure` implementing interfaces
declared in `Domain/Interfaces` (or `Application`). Flag any `Domain` type that references EF Core, Redis,
or ASP.NET, and any controller that talks to `DbContext` directly.

## Review checklist

Go through these in order. Spend time where the risk is highest.

### 1. Correctness & async
- `async` methods that never `await` (CS1998) — they run synchronously and hide blocking calls. Either make
  them truly async (`FirstOrDefaultAsync`, `ToListAsync`, `SaveChangesAsync`) or remove `async` and return
  `Task.FromResult`.
- Synchronous EF calls (`FirstOrDefault`, `ToList`, `Count`, `Any`) inside async code paths.
- `.Result`, `.Wait()`, `GetAwaiter().GetResult()` — deadlock and thread-pool starvation risk.
- `async void` anywhere except event handlers.
- Missing `CancellationToken`: controllers should accept one and pass it all the way to EF/Redis calls.
- Changes added to the context but never saved (e.g. `AddRangeAsync` with no `SaveChangesAsync`), or saved
  without checking that the change actually happened.
- Methods that return `true` without checking anything, or that swallow failures.
- Null handling: respect nullable annotations; no `!` used just to silence warnings; check `null` before
  dereferencing `FirstOrDefault` results.
- Empty stub methods (`public void UpdateUsers() { }`) shipped as if they were implemented.

### 2. EF Core & data access
- **N+1 queries**: looping over IDs and calling `FirstOrDefault` once per ID. Use one query:
  `db.Users.Where(u => ids.Contains(u.UserId)).ToListAsync(ct)`.
- Returning `IQueryable` (or `Task<IQueryable<T>>`) past the repository boundary — the query runs later,
  possibly after the `DbContext` is disposed, and callers can attach arbitrary queries. Return
  `IReadOnlyList<T>` or a materialized DTO list.
- `IQueryable<User?>` — a nullable element type in a query result is nearly always a design mistake.
- Building an in-memory `IEnumerable` and calling `.AsQueryable()` on it, which pretends to be a database query.
- Read-only queries without `AsNoTracking()`.
- Lazy-loading proxies are enabled: watch for navigation properties touched inside loops or during
  serialization (hidden N+1 queries, cycles in JSON output). Prefer explicit `Include` or projection with `Select`.
- Unbounded queries (`GetAll` with no paging). Require `Skip`/`Take` or keyset paging on list endpoints.
- Bulk deletes and updates: prefer `ExecuteDeleteAsync` / `ExecuteUpdateAsync` over load-then-remove.
- Concurrency in shared jam state (queue order, playback position): look for a concurrency token
  (`[Timestamp]`/`IsRowVersion`) or atomic Redis operations; flag read-modify-write races.
- Entity configuration belongs in `IEntityTypeConfiguration<T>` classes under `Persistence/SQL/Configurations`,
  not in data annotations scattered across entities.
- Migrations: any column drop or rename, or a nullable-to-required change, needs a data-migration plan.

### 3. API design (controllers)
- Controllers must be thin: validate → call a service → map to a DTO → return. No business logic and no `DbContext`.
- Never return EF entities from endpoints. Return `Application/DTOs/Responses` types. Never bind entities
  from the request body (over-posting); bind `DTOs/Requests` types.
- Use `[ApiController]`, attribute routing, and `ActionResult<T>` / `Results<...>` with the correct status codes:
  `201 CreatedAtAction` on create, `204` on delete, `404` for not found, `400`/`ValidationProblem` for bad input,
  `409` for conflicts. Do not return `200` + `false`.
- Errors go through a global exception handler (`IExceptionHandler` / middleware in `Api/Middleware`) that returns
  `ProblemDetails`. Flag `throw new Exception(...)`; use domain exceptions from `Domain/Exceptions`.
- Route and parameter naming: plural resource nouns (`/api/users/{id:guid}`), route constraints, no verbs in routes.
- Input validation: FluentValidation or DataAnnotations in `Application/Validators`; check Guids are not
  `Guid.Empty`, collections are bounded, and strings have length limits.

### 4. Security
- Every endpoint is either `[Authorize]`'d or explicitly `[AllowAnonymous]` with a reason.
- Authorization on resource ownership: a user must not be able to modify or delete another user's jam or queue item
  just by knowing its ID (IDOR).
- Do not trust a client-supplied `userId` when the identity comes from the auth token.
- No secrets or connection strings in code or `appsettings.json` committed to git — use user-secrets or environment variables.
- Location data is sensitive: never return exact coordinates of other users; use H3 cells or rounded values.
- SignalR hubs: authorize hub methods, validate group membership before broadcasting, and never let clients
  choose arbitrary group names.
- Do not log PII or tokens. Use Serilog structured templates (`Log.Information("User {UserId} joined {JamId}", ...)`),
  not string interpolation.
- Raw SQL only via `FromSql` with interpolated parameters, never string concatenation.

### 5. Dependency injection & lifetimes
- Services and repositories should depend on interfaces (`IUserRepository`, `IUserService`) so they can be tested
  and swapped. Flag concrete-to-concrete coupling between layers.
- `DbContext` is scoped: never inject it into a singleton or hosted service directly (use `IServiceScopeFactory`).
- Redis `IConnectionMultiplexer` should be a singleton.
- Registrations belong in `Infrastructure/DependecyInjection` extension methods (`AddInfrastructure(this IServiceCollection, IConfiguration)`),
  keeping `Program.cs` short.
- Options pattern (`IOptions<T>` with validation) for configuration, not `IConfiguration["..."]` scattered around.

### 6. Performance & real-time
- Hot paths (playback sync, presence heartbeats, nearby discovery) must not hit SQL per request when Redis can serve them.
- Redis: set TTLs on presence keys; use pipelining or batches for multi-key operations; keep key names centralized in `RedisKeys`.
- Avoid allocation-heavy LINQ in tight loops. Avoid `ToList()` just to call `Count`.
- Background work runs in `BackgroundService` with cancellation and exception logging, not fire-and-forget `Task.Run`.

### 7. Code style & maintainability (lowest priority — batch these)
- Standard .NET naming: `PascalCase` for types, methods, and properties (`UserId`, not `userId`); `camelCase`
  for parameters and locals (`userId`, not `_userId`); `_camelCase` only for private fields.
- Async methods end in `Async` (`GetUserAsync`).
- File-scoped namespaces (`namespace Echo.Application.Services;`). Namespaces should not contain `src`
  (set `RootNamespace` or adjust folder namespace rules).
- One public type per file; file name matches the type.
- Prefer primary constructors or `required`/`init` members where they simplify the code; prefer `record` for DTOs.
- Remove unused `using`s, dead code, and commented-out code. Don't commit `obj/` or `bin/` (check `.gitignore`).
- Pass-through service layers that only forward to the repository add no value; services should hold
  business rules (validation, authorization checks, orchestration across repos, mapping).

### 8. Testability
- New business logic should come with unit tests (xUnit + a mocking library) or integration tests
  (`WebApplicationFactory` + Testcontainers/SQL Server). Note when coverage is missing for risky logic.

## Severity levels

- **🔴 Blocker** — data loss or corruption, a security hole, a crash, a deadlock, or broken behavior. Must fix before merge.
- **🟠 Major** — a real bug in edge cases, N+1 or unbounded queries, layering violations, a wrong HTTP contract, missing auth.
- **🟡 Minor** — maintainability, missing cancellation tokens on non-hot paths, missing `AsNoTracking`.
- **🔵 Nit** — naming, formatting, style. Group these together; do not list each one separately.

## Output format

```
## Summary
<2–4 sentences: overall quality, biggest risks, merge recommendation: ✅ Approve / ⚠️ Approve with changes / ❌ Request changes>

## Findings
### 🔴 Blocker
- **[File.cs:42](src/path/File.cs#L42)** — <what is wrong>.
  **Why:** <the concrete failure: inputs/state → bad outcome>.
  **Fix:**
  ```csharp
  // minimal corrected code
  ```

### 🟠 Major
...

### 🟡 Minor
...

### 🔵 Nits
- <grouped one-liners>

## What's good
<1–3 specific things done well — be genuine, skip it if nothing stands out>
```

## Reviewer conduct

- Read the surrounding code before flagging: a pattern can be intentional, so check callers and the DI setup.
- Prefer one precise finding over three vague ones. If unsure, label it "Question:" rather than stating it as a defect.
- Show the fix in this repo's style, with the minimum change needed — don't rewrite whole files.
- Explain *why* for juniors: the goal is that the author learns the principle, not just applies the patch.
- Do not invent APIs or packages. If you suggest a NuGet package, say it is new and not yet referenced.
