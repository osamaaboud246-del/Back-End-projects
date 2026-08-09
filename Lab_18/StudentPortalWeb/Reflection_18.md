# Reflection 18 — Osama Aboud (Lab ID: 07)

1. Values:
- Lab ID = 7
- CHIP_YEAR = (7 mod 4) + 1 = 3 + 1 = 4
- CHIP_LABEL = Since CHIP_YEAR is 4, it is "Final".

2. In Session 15, the database connection string was hardcoded directly inside `OnConfiguring` in the `DbContext`.
   We fixed it by moving the connection string to `appsettings.json` and injecting it via Dependency Injection in `Program.cs`, 
   defining it once globally.

3. The shared pattern is registration and invocation: you write a custom class inheriting from a specific base class or interface,
   you register or map it in the configuration (`Program.cs` or `_ViewImports`),
   and the framework automatically invokes its logic (Match/IsValid/Process) during the request lifecycle before generating the final output.

4. `<gpa-badge>` contains the RULE (the logic of which value determines "first" vs "pass"), while `year-chip` primarily contains the LABEL text.
   I would be more nervous about duplicating `<gpa-badge>`'s logic in views
   because business rules (like GPA classifications) change often and must be universally consistent,
   whereas a visual label is just a UI string.