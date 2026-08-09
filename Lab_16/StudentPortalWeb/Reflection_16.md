# Reflection 16 — Osama Aboud (Lab ID: 7)

## Part F Answers

1. Lab ID & Derived Values:
   - Lab ID: 7
   - MAX_YEAR: (7 mod 4) + 1 = 3 + 1 = 4
   - MIN_GPA: 2.5 + (7 mod 3) * 0.5 = 2.5 + 1 * 0.5 = 3.0
   - INTAKE_CODE: itiB (7 mod 3 = 1 -> B)

2. Request Flow for `/students/top/5`:
   - The request hits the application pipeline and logs`[START] Request path : /students/top/5`.
   - The routing engine evaluates the route table top-to-bottom.
   - It matches the pattern `students/top/{count:int:range(1,4)}`, but the constraint `range(1,4)` fails because `5` exceeds MAX_YEAR (4).
   - Routing rejects this candidate route and falls back to subsequent routes including `default`, none of which match.
   - Routing short-circuits with a 404 Status Code, logging `[END] Request path : /students/top/5`.
   - What does NOT happen: `StudentsController.Top` action is never executed, and no Entity Framework / SQL Server query is executed.

3. Custom vs Built-in Constraint Comparison:
   - Same: Both implement the `IRouteConstraint` interface and execute inside `ConstraintMap` during the routing matching phase prior to controller action execution.
   - Different: Built-in `int` constraint is provided natively by the framework in `Microsoft.AspNetCore.Routing.Constraints`, whereas `IntakeCodeConstraint` is custom C# code written to validate application-specific logic (`itiB`).

4. Address Guarantee:
   - It is a guarantee, because applying an explicit `[Route("about/osama")]` attribute to an action overrides conventional routing, ensuring the action is strictly reachable only via that designated attribute path and no longer accepts `/Students/About`.