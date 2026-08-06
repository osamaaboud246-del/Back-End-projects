# 🧪 Lab 16 — Designing Your Own URLs
## ITI Summer Training | Web Development Using .NET | Morning Group
### Session 16 — Routing | 3 hours, in the room, graded out of 100

---

## 🔑 Before anything: your Personal Lab ID

Hamdy calls out a number next to your name on the roster. **Write it down.** Every
concrete value in this lab is derived from it, so no two correct submissions in this
room can be the same file.

Work out your three values now, on paper, before you open Visual Studio:

| Value | Formula | Yours |
|---|---|---|
| **MAX_YEAR** | `(Lab ID mod 4) + 1` | ______ |
| **MIN_GPA** | `2.5 + (Lab ID mod 3) × 0.5` | ______ |
| **INTAKE_CODE** | `iti` + `A` if `Lab ID mod 3` is 0, `B` if 1, `C` if 2 | ______ |

> **Worked example — Lab ID 7.** `7 mod 4 = 3`, so MAX_YEAR = **4**.
> `7 mod 3 = 1`, so MIN_GPA = `2.5 + 0.5` = **3.0**, and INTAKE_CODE = **itiB**.
> Every code sample in this task uses Lab ID 7's values. **Yours will be different.**
> Copying the samples verbatim is an automatic zero on the affected part.

⚠️ Hamdy will walk around and ask you, out loud, **why your numbers are what they
are**. Have the arithmetic on your paper.

---

## 🎯 What You're Building

You are extending **today's** route table — the same `StudentPortalWeb` project from
the lecture, not a new one. By the end you will have added four more addresses to
the ITI StudentPortal, one of them guarded by a route constraint you wrote yourself,
and one of them carrying your own name.

Nothing in this lab needs a concept that was not taught this morning. If you find
yourself needing something you have not seen, you have misread the task — re-read it
before asking.

---

## ✅ What You Need Before You Start

- Visual Studio, with
  `Session_16_Routing/Application/StudentPortalWeb/StudentPortalWeb.slnx` open.
- The project **running** — press F5 once and confirm the home page loads before you
  change anything.
- Your Student Guide open at Blocks 2 through 5.
- Your Lab ID and your three derived values, on paper.
- SQL Server running. You are not changing any schema today — no migrations, from any
  project.

---

## Part A — Verify and orient *(10 min, 5 points)*

**Tests:** Block 1 — reading the route table.

1. Run the app. Confirm `/`, `/students`, `/students/3`, `/students/year/2` and
   `/students/honours/first` all work.
2. In a comment at the very top of `Program.cs`, write your **Lab ID** and your
   **three derived values**, in this exact shape:

```csharp
// LAB 16 — Lab ID: 7 | MAX_YEAR = 4 | MIN_GPA = 3.0 | INTAKE_CODE = itiB
```

3. In the same comment, add one sentence answering: **why does the `default` route
   sit at the bottom of the table and not the top?**

---

## Part B — A second address for a page that already exists *(30 min, 15 points)*

**Tests:** Block 2 — custom conventional routes, and route order.

The registrar's office refers to the student list as "the roster" and wants a short
URL for it.

1. Add a conventional route so that **`/roster`** reaches
   `StudentsController.Index` — the same action `/students` already reaches.
2. It must be a **single literal segment**, with no parameters.
3. `/students` must **still work** afterwards. Both addresses reach the same page.
4. Place it correctly relative to the `default` route.

✅ **Check:** `/roster` and `/students` both render the roster.
✅ **Check:** `/Home/Privacy` still works. If it does not, your route is in the wrong
place.

📌 In a comment above your route, answer in one sentence: **is it acceptable for two
different URLs to reach the same action?** (There is a defensible answer either way.
State yours.)

---

## Part C — A route with a personalised constraint *(45 min, 25 points)*

**Tests:** Block 3 — built-in constraints, chaining, and inclusive ranges.

The dean wants a "top students" page showing the highest-GPA students, but refuses to
let anyone request an unlimited number of rows.

1. Add a new action to `StudentsController`:

```csharp
public async Task<IActionResult> Top(int count)
```

   It loads students **ordered by GPA, highest first**, takes the first `count` of
   them, and passes the list to a view. Reuse the existing
   `Views/Students/Index.cshtml` by returning `View("Index", students)`.

2. Add a conventional route so this action answers:

```
/students/top/{count}
```

3. Constrain `count` so that it must be **a whole number** AND fall in the
   **inclusive range 1 to MAX_YEAR** — your MAX_YEAR, not the example's. Chain both
   constraints on the one parameter.

✅ **Check (Lab ID 7, MAX_YEAR = 4):** `/students/top/4` renders. `/students/top/5`
is a **404**. `/students/top/abc` is a **404**.
✅ **Check:** when the URL 404s, your console shows `[START]` and `[END]` with
**nothing between them** — no EF query. If a query ran, your constraint is not doing
its job.

📌 In a comment, answer: **is your MAX_YEAR itself accepted, or rejected?** Say why.

---

## Part D — A constraint you write yourself *(55 min, 30 points)*

**Tests:** Block 4 — `IRouteConstraint`, `ConstraintMap`, and the difference between
matching and validating.

Each intake at ITI has a code. Yours is **INTAKE_CODE** from the table above. The
portal must accept a URL naming an intake — but only a **real** one.

1. Create `Constraints/IntakeCodeConstraint.cs`, a public class implementing
   `IRouteConstraint`.
2. It must accept **only your INTAKE_CODE**, compared **case-insensitively**, and
   reject every other string.
3. It must use a **guard clause** for a missing or null value, exactly as the lecture's
   constraint did — no dictionary indexer.
4. It must **not** touch the database. State why in a comment.
5. Register it in `Program.cs` under the nickname **`intakecode`**, above
   `builder.Build()`.
6. Add an action `Intake(string code)` to `StudentsController` that loads all students
   and returns `View("Index", students)`, and a route so it answers:

```
/students/intake/{code}
```

   …constrained with your `intakecode` nickname.

✅ **Check (Lab ID 7, INTAKE_CODE = itiB):** `/students/intake/itiB` renders.
`/students/intake/ITIB` renders — same page.
✅ **Check:** `/students/intake/itiA` is a **404**. `/students/intake/banana` is a
**404**.
✅ **Check:** no EF query appears in the console for either 404.

⚠️ A constraint that returns `true` for everything will look perfectly correct on the
happy path. Hamdy will type a **bad** value into your running app. Test that yourself
first.

---

## Part E — Your own address *(30 min, 15 points)*

**Tests:** Block 5 — attribute routing, and route data vs. the query string.

1. Add an action to `StudentsController` called `About`.
2. Give it its own address with a `[Route]` attribute, at exactly:

```
about/<your-first-name-in-lowercase>
```

   For a trainee called Mariam, that is `[Route("about/mariam")]`. **Use your real
   first name.**
3. `About` takes one parameter, `minGpa`, marked with the attribute that states it
   comes from the **query string**. It loads students whose GPA is **greater than or
   equal to** `minGpa`, ordered by name, and returns `View("Index", students)`.
4. If no `minGpa` is supplied in the query string, default to **your MIN_GPA**.

✅ **Check (Lab ID 7, first name Mariam, MIN_GPA = 3.0):**
`/about/mariam` renders the students with GPA ≥ 3.0.
`/about/mariam?minGpa=3.9` renders fewer students.
✅ **Check:** `/Students/About` is a **404**. Explain why in a comment — one sentence.

📌 In the same comment, answer: **why does `minGpa` belong in the query string rather
than in the path?** Use the test from Block 5.

---

## Part F — Wrap-Up Reflection *(10 min, 10 points)*

Create `Reflection_16.md` next to your project and answer all four. Answer them about
**your own** values — an answer using the example's numbers scores zero.

1. Write out your Lab ID and all three derived values, with the arithmetic.
2. Your Part C route rejects a `count` one above your MAX_YEAR. Describe, in order,
   what the framework does with that request — from the `[START]` line to the 404.
   Name what does **not** happen.
3. Your Part D constraint and the built-in `int` constraint are both used in your
   route table. Name one thing that is genuinely the same about how the framework
   treats them, and one thing that is different about how you wrote them.
4. Your Part E action is unreachable at `/Students/About`. In one sentence: is that a
   limitation or a guarantee, and why?

---

## 📋 Grading Rubric

| Part | What earns the marks | Points |
|---|---|---|
| **A** | Lab ID + three derived values in the header comment (2); route-order answer correct (3) | **5** |
| **B** | `/roster` route present and working (7); `/students` and `/Home/Privacy` still work — correct placement (5); comment answered (3) | **15** |
| **C** | `Top` action correct — ordered by GPA descending, takes `count` (8); route present (5); **both** constraints chained correctly (8); MAX_YEAR is the trainee's own (2); comment answered (2) | **25** |
| **D** | Constraint class implements `IRouteConstraint` correctly (8); accepts only their own INTAKE_CODE (6); case-insensitive (3); guard clause with `TryGetValue`, no indexer (4); registered in `ConstraintMap` under `intakecode` (4); route uses it (3); no database access, with reason stated (2) | **30** |
| **E** | `[Route]` attribute with their own first name (5); `[FromQuery]` on the parameter (3); default falls back to their MIN_GPA (3); the two comment answers (4) | **15** |
| **F** | Four reflection answers, using their own values (10) | **10** |
| | **TOTAL** | **100** |

**Automatic deductions:**
- Any route placed **below** the `default` route that should be above it: **−5**, even
  if the page happens to work.
- A constraint that accepts a value it should reject: **−10**. It is not a constraint
  if it never says no.
- Values copied from the worked example instead of derived from your own Lab ID:
  **zero on that part.**

---

## ⏰ Time Budget

| Part | Minutes |
|---|---|
| A — Verify and orient | 10 |
| B — `/roster` | 30 |
| C — Personalised constraint on a route | 45 |
| D — Your own `IRouteConstraint` | 55 |
| E — Your own address | 30 |
| F — Reflection | 10 |
| **Total** | **180** |

If you are not finished with Part C by the **one-hour** mark, tell Hamdy rather than
pushing on — Part D is worth more and needs the time.

---

## 🙋 If You Get Stuck

**Check these four things, in this order, before raising your hand:**

1. **Did you stop the app before re-running?** Shift+F5, then F5. A stale run is the
   number one cause of "my change did nothing".
2. **Is your route above the `default` route?** If some of your URLs work and others
   404, this is almost always why.
3. **Does the nickname in your pattern match `ConstraintMap.Add` exactly?** If the app
   throws at startup, read the exception message — it names the constraint it could
   not find.
4. **Does your action's parameter name match the `{name}` in the pattern?** They are
   matched by name. A mismatch produces no error at all — just a zero or a null.

Still stuck? Raise your hand, and tell Hamdy **which of those four you have already
checked**. "It doesn't work" is not a question anyone can answer.
