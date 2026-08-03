# Lab 15 Answers — Osama Aboud (Lab ID: 07)

## Part A — Scaffold
* Port Number: https://localhost:7123
* Source File: Properties/launchSettings.json

## Part B — Predictions
* B.1 Prediction: Will fail at runtime when builder.Build() is executed.
  - Actual: Throws InvalidOperationException (cannot modify ServiceCollection after application build).
* B.2 Prediction: Failure occurs when a user visits the controller page at request time.
  - Actual: Fails at request time when visiting the page with InvalidOperationException.
* B.3 Prediction: 1 time (only for HTML page, static files handled before).
  - Actual: 1 time.

## Part C — Verification
* Number of students rendered: 4 (Matches SSMS `SELECT COUNT(*) FROM Students`).

## Part D — Deliberate Failures
* D.2 Exception Type: `System.InvalidOperationException`
  - Message: "Unable to resolve service for type 'StudentPortalWeb.Models.StudentPortalContext' while attempting to activate 'StudentPortalWeb.Controllers.HomeController'."
  - Timing: Fails at request time when visiting the page. Matches prediction B.2.
* D.5 Singleton Context Behaviour: App fails at startup with InvalidOperationException regarding scoped service consumption from singleton.
* D.6 Answer: Silent success in D.4/D.5 would be bad news. Multi-threaded requests would share a single DbContext instance, leading to state corruption and concurrency crashes as soon as multiple users access the app simultaneously.

## Part E — Lifetime Experiment (Lab ID 07: Scoped)
* Registration Lifetime: Scoped (`AddScoped<IOsamaStampService, OsamaStampService>()`)

| Load Iteration | Stamp A | Stamp B |
|---|---|---|
| First load | 3f9c1e02 | 3f9c1e02 |
| Second load | 8a1b4c77 | 8a1b4c77 |

* Answers:
  1. Stamp A and B matched within a single load because `Scoped` lifetime reuses the same instance throughout a single HTTP request context.
  2. Stamps changed between loads because each new HTTP request creates a new Scope and constructs a new instance.
  3. Neighbour (Lab ID 06 - Transient): Their Stamp A and Stamp B differed even within the same single page load because Transient creates a new instance for every parameter injection.

## Part F — Pipeline Observation
* F.4 `[START]` lines: 4 calls (`/`, `/css/site.css`, `/js/site.js`, `/favicon.ico`). One page load causes multiple HTTP requests to fetch assets.
* F.5 Visiting `/audit-07`: Console logs `[AUDIT] Osama Aboud saw a request for /audit-07` followed by 404. Middleware ran because it is registered early in the pipeline before routing.
* F.7 Moving middleware after `UseStaticFiles()`: Static files (`/css/site.css`, `/js/site.js`, `/favicon.ico`) disappeared from the log because `UseStaticFiles()` handles them and short-circuits the pipeline. Matches prediction B.3.

## Part G — Reflection
* G.1: Removing `OnConfiguring` decoupled the context from a hardcoded connection string, allowing Dependency Injection to inject different configurations dynamically.
* G.2: Failing at startup is better because it alerts developers immediately during deployment rather than crashing randomly in front of a real user.
* G.3: Session 13: Deferred execution in LINQ queries. Session 14: AsNoTracking tracking state misinterpretation during SaveChanges.
* G.4: Having two projects own migrations would cause conflicting migration histories in `__EFMigrationsHistory`, generating duplicate table creation scripts and failing database updates.