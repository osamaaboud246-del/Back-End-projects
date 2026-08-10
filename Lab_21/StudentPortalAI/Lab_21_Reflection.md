# Lab 21 Reflection — Osama Aboud (Lab ID: 29)

## B1. Write-Access Guardrail
If my InstructorLookupTool were given write access to reassign instructors to courses,
the specific guardrail I would add is a strict Role-Based Access Control (RBAC) 
verification before execution. I would mandate that the agent verifies the 
user session holds "Admin" privileges before generating or executing any 
update query on the database. This is crucial for this specific tool to 
prevent regular students from arbitrarily changing their instructors through the chat interface.

## B2. Course Wrap-Up: 5 Real Artifacts
1. The ITI_StudentPortal relational database I designed and populated from Session 2's ER diagram.
2. The ASP.NET Core MVC application with complete CRUD operations and route constraints from Sessions 16-17.
3. The custom Tag Helpers (like `<gpa-badge>` and `<year-chip>`) and Razor partial views built in Session 18.
4. The Razor Pages module built in Session 20 to handle "Add Student" form submissions securely.
5. The complete StudentPortalAI RAG pipeline and routing Agent built today to query real database records using natural language.
