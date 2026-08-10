1. Lab ID & Derived Values:
- Lab ID = 29
- MIN_GPA_EDIT = 2.0 + (29 mod 5) * 0.3 = 2.0 + (4 * 0.3) = 3.2
- MAX_YEAR_EDIT = (29 mod 3) + 2 = 2 + 2 = 4

2. Three places bad input can be rejected:
- Client-side validation (HTML5 / JavaScript)
- Server-side validation (ModelState in the Controller)
- Database constraints (SQL Server)
* My Part D change lives in Server-side validation (and Client-side via Tag Helpers).

3. Post/Redirect/Get pattern prevents the browser from re-submitting the POST request. After a successful save,
the server responds with a 302 Redirect instruction. The browser then makes a brand new GET request to the confirmation page.
Pressing F5 only refreshes that last GET request.

4. 
- Same: Both [Required]/[MaxLength] and [Range] are written as C# attributes applied to model properties.
- Different: [Required] and [MaxLength] change the actual database schema directly (e.g., column nullability and size limits in SQL),
  while [Range] only enforces validation rules in .NET memory and does not alter the database table structure.
