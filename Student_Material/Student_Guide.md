# 📚 Student Guide — Session 16: Routing
## ITI Summer Training | Web Development Using .NET | Morning Group

---

## 🎯 Session Goal

**What this session is:** The day the URL stops being something the framework
hands you and becomes something you design. One line of `Program.cs` decided every
address in your application yesterday. Today that line becomes five, and you write
all of them.

**By the end:** You can read a route pattern out loud and say exactly which URLs it
accepts and which it refuses; you can add your own routes so `/students/3` means
what you want it to mean; you can attach constraints so a bad URL is rejected
before any of your code runs; you can write a constraint of your own that the
framework calls to make its decision; and you can say when a value belongs in the
path and when it belongs in the query string.

**Bridge from Session 15:** Yesterday you registered `StudentPortalContext` in the
DI container, a `HomeController` received it without ever calling `new`, and four
students rendered in a browser. You also typed a middleware that prints
`[START] Request path : ...` for every request. Today that middleware becomes a
measuring instrument, and the single line at the bottom of `Program.cs` you barely
looked at — `app.MapControllerRoute(pattern: "{controller=Home}/{action=Index}/{id?}")` —
becomes the whole session.

---

## 🗺️ ~3h25 LECTURE ROADMAP — The URL Is a Design Decision

| TIME | BLOCK | LOAD | KEY |
|------|-------|------|-----|
| 0:00–0:15 | 🌅 Warm-Up | 🟢 | Yesterday's app, and the one line nobody read |
| 0:15–0:50 | 🎯 Block 1 — The Route Table | 🟡 | How a URL becomes a method call |
| 0:50–1:25 | 🎯 Block 2 — Custom Routes | 🔴 | Designing your own URLs, and why order matters |
| 1:25–1:35 | ☕ Break 1 + Puzzle 1 | 🟢 | Route order |
| 1:35–2:10 | 🎯 Block 3 — Route Constraints | 🔴 | Refusing bad URLs at the door |
| 2:10–2:40 | 🎯 Block 4 — Custom Constraints | 🔴 | The framework calls YOUR code |
| 2:40–2:50 | ☕ Break 2 + Puzzle 2 | 🟢 | Constraint behaviour |
| 2:50–3:15 | 🎯 Block 5 — Attribute Routing & the Query String | 🟡 | Two ways a URL carries information |
| 3:15–3:25 | 🏁 Wrap-Up + Lab Handoff | 🟢 | The API you designed |

Three of those blocks include a 🖐️ **guided practice** round — you typing on your own
machine, with Hamdy circulating: tracing six URLs by hand (Block 1), adding a
constraint yourself (Block 3), and giving an action a second address (Block 5).

🟢 Light 🟡 Medium 🔴 Heavy

---

## 🌅 WARM-UP

### Where we left off

Yesterday's application is on screen right now, unchanged. Press F5 and you get:

```
[START] Request path : /
[END] Request path : /
```

…and a page listing the students — the same four rows you have been looking at
since Session 13:

| FullName | YearOfStudy | Gpa |
|---|---|---|
| Kareem Fouad | 4 | 3.20 |
| Nada Samir | 1 | 4.00 |
| Omar Hesham | 3 | 2.80 |
| Yara Adel | 2 | 3.50 |

That is Session 15's whole achievement in two lines of console output and one HTML
table. Every URL you build today shows some slice of exactly those four rows — so if
a page ever looks wrong, you already know what right looks like.

Now look at the browser's address bar and ask a question nobody asked yesterday:

> **Who decided that `/` means `HomeController.Index()`?**

Not the controller — the controller has no idea what URL it was reached by. Not the
middleware — it printed the path and passed it on. Not `AddDbContext`. Something
between the browser and the controller read the text `/`, decided it referred to a
specific method in a specific class, and called it.

That something is **routing**. It has been running all week. Today we look at it.

### Quick recall — three questions before we start

**1. In `Program.cs`, what does `builder.Build()` divide?**

<details>
<summary>Answer</summary>

Everything above it registers **what the app can do** (services in the DI
container). Everything below it defines **how a request is handled** (the
middleware pipeline). Nothing can be registered after `Build()`; nothing can be
added to the pipeline before it. Today you will add one thing on each side of that
line, and Block 4 will explain why they cannot both live in the same place.
</details>

**2. Your middleware prints `[START]` before `await next.Invoke()` and `[END]`
after it. If a request never reaches a controller at all, what do you see?**

<details>
<summary>Answer</summary>

You still see **both lines**. `[START]` prints on the way in, `[END]` prints on the
way out, and the way out happens whether the request reached a controller, hit a
static file, or was answered with a 404 by the framework. That property is what
makes the middleware useful today: it proves a request *arrived*, independently of
whether anything agreed to *handle* it.
</details>

**3. What is the exact pattern string in the one `MapControllerRoute` call at the
bottom of yesterday's `Program.cs`?**

<details>
<summary>Answer</summary>

`"{controller=Home}/{action=Index}/{id?}"`

Three segments. Two of them have an `=` in them. One has a `?`. By the end of Block
1 every one of those characters will mean something specific to you.
</details>

---

## 🎯 BLOCK 1 — The Route Table

### ❌ The Problem: The URL is describing our source code

Yesterday's application answers these addresses:

```
/                     →  HomeController.Index()
/Home/Index           →  HomeController.Index()
/Home/Privacy         →  HomeController.Privacy()
```

That works. But read the second and third addresses as a stranger would. They do
not describe **what the page is about** — they describe **what our classes are
called**. `/Home/Privacy` tells the world that somewhere inside this application
there is a class named `Home` with a method named `Privacy`.

Three real consequences:

1. **Rename the controller, break every link.** If `HomeController` becomes
   `DashboardController` tomorrow, every bookmark, every printed URL, every link
   in an email anyone ever sent about this site is dead. Your internal naming
   decision leaked into a public contract.
2. **The URL cannot describe the thing.** A student page ends up at
   `/Students/Details/3`. The word `Details` is there because we named a method
   that; it means nothing to a human.
3. **You have no way to refuse a bad address.** `/Students/Details/abc` is a URL
   the framework will happily accept, hand to your method as the number zero, and
   let you discover the problem in a database query.

### ✅ The Solution: The route table

ASP.NET Core keeps an ordered list of **route patterns**. On every request it walks
that list from the top, and the first pattern that matches the URL wins. Matching a
pattern produces a set of **route values** — a small dictionary — and MVC uses two
particular keys from that dictionary, `controller` and `action`, to decide which
method to call.

That is the entire mechanism. There is no magic anywhere in it.

```
┌──────────────────────────────────────────────────────────────────────┐
│  HOW A URL BECOMES A METHOD CALL                                      │
│                                                                        │
│   Browser  ──"/Home/Privacy"──►  [ your [START] middleware ]           │
│                                          │                             │
│                                          ▼                             │
│                                   [ UseRouting ]                       │
│                                          │                             │
│                          walks the route table, top to bottom          │
│                                          │                             │
│                  ┌───────────────────────┴──────────────────────┐      │
│                  │  pattern: {controller=Home}/{action=Index}/  │      │
│                  │           {id?}                              │      │
│                  └───────────────────────┬──────────────────────┘      │
│                                          │  MATCH                      │
│                                          ▼                             │
│                    route values: controller = "Home"                   │
│                                  action     = "Privacy"                │
│                                          │                             │
│                                          ▼                             │
│                       HomeController.Privacy()  is called              │
└──────────────────────────────────────────────────────────────────────┘
```

### 🔧 Reading the pattern, character by character

```
"{controller=Home}/{action=Index}/{id?}"
```

| Piece | Name | What it means |
|---|---|---|
| `{...}` | Parameter segment | Captures whatever text is in this position into a route value |
| `controller` | Reserved name | MVC reads this value to pick the class |
| `action` | Reserved name | MVC reads this value to pick the method |
| `=Home` | Default | If this segment is absent from the URL, use `Home` |
| `=Index` | Default | If this segment is absent from the URL, use `Index` |
| `id` | Ordinary name | Captured and passed to a method parameter also called `id` |
| `?` | Optional | This segment may be missing entirely and the pattern still matches |
| `/` | Literal | A separator; the URL must have one here too |

Now the three addresses make sense:

| URL | Segments present | controller | action | id |
|---|---|---|---|---|
| `/` | none | `Home` (default) | `Index` (default) | — |
| `/Home/Privacy` | two | `Home` | `Privacy` | — |
| `/Students/Details/3` | three | `Students` | `Details` | `3` |

One pattern, three very different-looking URLs. Defaults and optional markers are
what let a single line cover all of them — and also what make that single line so
easy to stop noticing.

### 🍰 Analogy — The hotel switchboard

A guest phones the hotel and says a room number. The switchboard operator does not
know the guest, does not know what the call is about, and does not answer any
questions. She has one job: turn the words she heard into a specific extension, and
connect the call.

The **route table is the switchboard's list**. Each line says "words that look like
*this* go to extension *that*". The operator reads her list from the top and uses
the first line that fits.

Two things follow immediately, and both matter today:

- If two lines could fit, **the one higher on the list wins** — the operator never
  reads further.
- If no line fits, the call does not reach a wrong room; it **does not connect at
  all**. That is a 404.

### 🔬 Proving it — the demo you will watch

Comment out the single `MapControllerRoute` line and re-run. Predict first: what
happens to `/`, and what happens to `/Home/Privacy`?

Every URL returns **404**. Not an error page from a controller — nothing was
reached at all. The `[START]` and `[END]` lines still print, because the request
absolutely arrived at the server; it just never found a line on the switchboard's
list. That is the cleanest possible proof that routing, not luck, was holding the
site up.

**📌 Students May Ask — "Is `UseRouting()` the same thing as the route table?"**

No. `UseRouting()` is the middleware that *performs* the lookup — it is a step in
the hallway, in the position you place it. `MapControllerRoute(...)` calls are what
*fill in* the table that lookup reads. That is why the `app.Use...` line you wrote
yesterday, registered before `UseRouting()`, still prints for URLs that no route
matches: it runs earlier in the hallway than the door it never gets through.

---

### 🧩 MINI-PUZZLE — Block 1 Check

Given only the default pattern `{controller=Home}/{action=Index}/{id?}`, which URL
below does **NOT** match it?

**A)** `/`
**B)** `/Home`
**C)** `/Home/Privacy/7/extra`
**D)** `/Students/Details/3`

<details>
<summary>Answer</summary>

**C — `/Home/Privacy/7/extra`.**

- **A** matches: zero segments, so both defaults apply. ✅
- **B** matches: one segment fills `controller`, `action` falls back to its default
  `Index`. ✅ (Many people expect a one-segment URL to fail — it does not, because
  `action` has a default.)
- **C** does not match: the pattern has exactly three segments and no catch-all.
  A fourth segment has nowhere to go, so the pattern is rejected. ❌
- **D** matches: three segments, three slots, all filled. ✅ (Whether
  `StudentsController.Details` *exists* is a separate question asked after routing;
  the pattern itself matches fine.)

**Rule:** a pattern matches on **shape first** — segment count and literals — and
only then are defaults and optional markers applied.
</details>

---

## 🎯 BLOCK 2 — Custom Routes

### ❌ The Problem: `/Students/Details/3` is not an address, it is a confession

We want the student page to live at:

```
/students/3
```

Short, readable, says what it is about, and reveals nothing about our class names.
With only the default pattern in the table, that URL is read as: controller
`students`, action `3`. There is no method called `3`, so the request 404s.

We cannot fix this by renaming things. We fix it by **adding a line to the
switchboard's list**.

### ✅ The Solution: your own `MapControllerRoute` entries

A route entry has three parts:

```
┌──────────────────────────────────────────────────────────────────────┐
│  ANATOMY OF A CUSTOM ROUTE                                            │
│                                                                        │
│    app.MapControllerRoute(                                             │
│        name:     "studentDetails",     ← a label, for YOU. Not a URL. │
│        pattern:  "students/{id}",      ← what the URL must look like  │
│        defaults: new { controller = "Students",   ← where it goes,    │
│                        action     = "Details" }); ← privately         │
│                                                                        │
│    URL:       /students/3                                              │
│               └──────┘ └┘                                              │
│                literal  captured as id = "3"                           │
│                                                                        │
│    Route values produced:  controller = "Students"   (from defaults)   │
│                            action     = "Details"    (from defaults)   │
│                            id         = "3"          (from the URL)    │
└──────────────────────────────────────────────────────────────────────┘
```

The important shift: **`controller` and `action` are no longer in the pattern.**
They moved into the `defaults` object, where the public URL cannot see them. The
address `/students/3` now says what the page is about. Rename `StudentsController`
tomorrow and you change one string in `Program.cs` — every bookmark still works.

### 🔧 How the captured value reaches your method

Routing puts `id = "3"` into the route values. MVC then looks at the target
method's parameter list, finds a parameter whose **name** matches a route value
key, converts the text, and passes it in:

```
route value  id = "3"        ──►   public IActionResult Details(int id)
                                                            └┘
                                              same name, so it gets the value
```

⚠️ **Matched by name, not by position.** Rename the method parameter to `studentId`
while the pattern still says `{id}` and nothing errors — `studentId` silently
becomes `0`, and you get a page saying no such student exists. This is one of the
most common beginner bugs in MVC, and it never produces a compiler error.

### 🍰 Analogy — Street addresses vs. "third door on the left"

"Third door on the left, past the photocopier" works right up until someone moves
the photocopier. It describes the *building's current interior*, so it breaks
whenever the interior changes.

"14 Tahrir Street" describes the *place*. The owners can rebuild the whole inside
and the address is still correct.

`/Students/Details/3` is *third door on the left*. `/students/3` is *14 Tahrir
Street*. Custom routes are how you give your pages real addresses.

### 🔧 Order matters — and it breaks in the worst possible way

The route table is walked **top to bottom**, first match wins. So this is fine:

```
students                    →  Students / Index
students/{id}               →  Students / Details
{controller=Home}/{action=Index}/{id?}
```

…and this is a disaster:

```
{controller=Home}/{action=Index}/{id?}    ← moved to the top
students                    →  Students / Index
students/{id}               →  Students / Details
```

Predict before you run it. With the default route on top:

| URL | What happens | Why |
|---|---|---|
| `/students` | ✅ still works | Default pattern reads it as controller `students`, action defaults to `Index` — which happens to be exactly right |
| `/students/3` | ❌ 404 | Default pattern reads it as controller `students`, action `3`. No method named `3` |

That is the nastiest kind of bug: **it half works.** The page you happened to test
first is fine, and a different page is broken for a reason that has nothing to do
with either page's code. The rule that prevents it is simple and absolute:

> 📌 **Specific routes go above general ones. The catch-all goes last.**

**📌 Students May Ask — "Does the `name:` argument affect the URL?"**

No. It is a label used for generating links back out (`Url.Link("studentDetails",
…)`) and for reading the table in a debugger. You could name a route
`"banana"` and every URL would behave identically. It must be **unique**, though —
two routes with the same name throw at startup.

---

### 🧩 MINI-PUZZLE — Block 2 Check

The table contains, in this order:

```
1.  "students"                              → Students / Index
2.  "students/{id}"                         → Students / Details
3.  "{controller=Home}/{action=Index}/{id?}"
```

Which route answers `/students/search`?

**A)** Route 2 — `id` is captured as the text `"search"`
**B)** Route 1 — `students` is a literal match and the rest is ignored
**C)** Route 3 — the first two only accept numbers
**D)** None — `/students/search` is a 404

<details>
<summary>Answer</summary>

**A — Route 2, with `id = "search"`.**

- **B** is wrong: a pattern must consume the **whole** path. Route 1 is one segment
  and the URL has two, so it does not match.
- **C** is wrong, and it is the trap: **nothing in these patterns says "numbers
  only".** `{id}` captures any non-empty text. That assumption is exactly what
  Block 3 exists to fix.
- **D** is wrong: route 2 matches happily. What follows is worse than a 404 —
  `Details(int id)` receives `0`, queries the database for student zero, finds
  nothing, and reports "not found" as though the data were missing rather than the
  URL being nonsense.

**Rule:** an unconstrained `{parameter}` matches **any** text, including text you
never intended.
</details>

---

## ☕ BREAK PUZZLE 1

You add a route for a page listing every student in an academic year:

```csharp
app.MapControllerRoute(
    name: "studentsByYear",
    pattern: "students/year/{year}",
    defaults: new { controller = "Students", action = "ByYear" });
```

You place it **below** this existing route:

```csharp
app.MapControllerRoute(
    name: "studentDetails",
    pattern: "students/{id}",
    defaults: new { controller = "Students", action = "Details" });
```

You browse to `/students/year/2`. What happens?

**A)** 404 — `students/{id}` matched first and `Details` rejected the value `year`
**B)** The by-year page renders correctly
**C)** The details page renders, showing the student whose id is 2
**D)** Startup throws — two routes both begin with the literal `students`

<details>
<summary>Answer</summary>

**B — the by-year page renders correctly.**

This one is deliberately awkward, because "order matters" is fresh in your mind and
the obvious move is to punish the lower route. But look at the **shapes**:

- `students/{id}` is **two** segments.
- `/students/year/2` is **three** segments.

They cannot collide. Segment count is checked before anything else, so
`students/{id}` is eliminated instantly and routing carries on to the next entry.

**Why each wrong answer is wrong:**

- **A** assumes `students/{id}` matched. It never could — wrong number of segments.
- **C** assumes routing "gets close enough" and reuses the last segment. Routing
  never partially matches; a pattern either consumes the entire path or it is
  discarded.
- **D** assumes routes must have unique prefixes. They do not. Only route **names**
  must be unique, and these two differ.

**Key insight:** "order matters" is real, but it only bites when two patterns can
match the **same shape** of URL. Different segment counts, or different literal
segments in the same position, cannot conflict at all.

**Bridge:** so `{id}` accepts the text `year`, accepts `abc`, accepts anything.
After the break we stop hoping the URL is sensible and start *requiring* it.
</details>

☕ **Break — 10 minutes.**

---

## 🎯 BLOCK 3 — Route Constraints

### ❌ The Problem: the failure happens in the wrong place

`/students/abc` today:

```
[START] Request path : /students/abc
   ↓ route students/{id} matches      (id = "abc")
   ↓ MVC tries to convert "abc" to int, fails, uses 0
   ↓ Details(0) runs
   ↓ a real database query executes: SELECT ... WHERE Id = 0
   ↓ no row
   ↓ NotFound()  →  404
[END] Request path : /students/abc
```

The user sees a 404, which looks right. Everything about *how* they got it is
wrong:

- A **database query ran** for a request that was never valid.
- The 404 says "**this student does not exist**", when the truth is "**that is not
  a student id**". Different problems, and only one of them is worth investigating.
- The action had to defend itself against input the URL should never have carried.

### ✅ The Solution: constraints — a matching rule attached to a parameter

A **route constraint** narrows what a parameter segment will accept. It is written
inside the same braces, after a colon:

```
"students/{id:int}"
              └──┘
        this segment must be a whole number
```

Now `/students/abc` never matches this route at all. Routing moves on, finds
nothing else that fits, and returns 404 **before any controller is constructed,
before the DbContext for this request exists, before a single line of your code
runs.**

### 🔧 The built-in constraints you will actually use

| Constraint | Pattern example | Accepts | Rejects |
|---|---|---|---|
| `int` | `{id:int}` | `3`, `-7`, `0042` | `abc`, `3.5`, empty |
| `alpha` | `{code:alpha}` | `abc`, `XYZ` | `a1`, `a-b` |
| `min(n)` | `{id:min(1)}` | `1`, `2`, `900` | `0`, `-4` |
| `max(n)` | `{page:max(50)}` | `50`, `1` | `51` |
| `range(a,b)` | `{year:range(1,4)}` | `1`, `2`, `3`, `4` | `0`, `5`, `77` |
| `length(n)` | `{code:length(4)}` | `2026` | `202`, `20261` |
| `minlength(n)` / `maxlength(n)` | `{q:minlength(3)}` | `abc` | `ab` |
| `regex(...)` | `{sku:regex(^[A-Z]{{2}}\\d+$)}` | `AB12` | `ab12` |
| `guid`, `bool`, `datetime`, `decimal`, `double`, `long` | `{id:guid}` | the matching type | anything else |

Constraints **chain with colons**, and chaining always reads as AND:

```
"students/year/{year:int:range(1,4)}"
                     └─┘ └────────┘
              whole number   AND   between 1 and 4 inclusive
```

### 🍰 Analogy — The bouncer and the manager

A nightclub has a bouncer at the door and a manager inside.

Without a bouncer, everyone walks in. The manager then has to spot the person who
should not be there, escort them back out, and apologise — after they have already
been served a drink and taken up a table. Work was done for someone who was never
allowed to be there.

The **bouncer is the constraint**. He does not know or care what happens inside; he
checks one thing at the door, and the people he turns away consume none of the
club's resources at all.

Notice the bouncer does not *arrest* anyone either. He simply does not let them
through this door — and that distinction is the most misunderstood thing about
constraints:

> 📌 **A constraint is a matching rule, not a validation error.** Failing it does
> not raise anything. The route is skipped, and routing continues down the table to
> the next entry. The 404 only appears if *nothing else* matches either.

### 🔬 Proving it — the console tells you which failure you got

Both of these show a 404 page in the browser. They are completely different events,
and yesterday's middleware is what makes the difference visible:

| URL | Console shows | What really happened |
|---|---|---|
| `/students/abc` | `[START]`, `[END]`, no EF query, no action | The **route** refused it |
| `/students/9999` | `[START]`, an EF `SELECT`, `[END]` | The route **accepted** it; the row does not exist |

Same status code, opposite causes. Being able to tell them apart from the log
alone is an interview-grade skill, and you now have the instrument for it — the one
you built yesterday.

**📌 Students May Ask — "So constraints validate my input? Do I still need checks in
the action?"**

Yes, you still need them. A constraint only guarantees the value has the right
**shape**. `students/{id:int}` guarantees `id` is a whole number — it says nothing
about whether student 9999 exists, whether the caller is allowed to see them, or
whether the number is negative. Constraints filter **URLs**; guard clauses in the
action protect **behaviour**. Both, always.

**📌 Students May Ask — "Why does `{id:int}` accept a negative number?"**

Because `int` means "parses as `System.Int32`", nothing more. If you want positive
only, say so: `{id:int:min(1)}`.

---

### 🧩 MINI-PUZZLE — Block 3 Check

The table is:

```
1.  "students/{id:int}"       → Students / Details
2.  "students/{name}"         → Students / ByName
3.  "{controller=Home}/{action=Index}/{id?}"
```

You browse to `/students/nada`. What happens?

**A)** 404 — route 1 failed its constraint, so routing stops
**B)** An exception — `"nada"` cannot be converted to `int`
**C)** Route 1 runs `Details` with `id = 0`
**D)** Route 2 runs `ByName` with `name = "nada"`

<details>
<summary>Answer</summary>

**D — route 2 runs `ByName` with `name = "nada"`.**

- **A** is the popular wrong answer, and it is the whole point of this puzzle. A
  failed constraint does **not** stop routing. It removes route 1 from
  consideration and routing carries on to route 2, which matches.
- **B** is wrong: no conversion is ever attempted. The constraint is checked
  *instead of* converting, not after it. Nothing throws.
- **C** is what would happen **without** the `:int` — and it is exactly the bug the
  constraint exists to prevent.

**Rule:** a constraint eliminates **one route**, never the request. Fall-through is
a feature: it is what lets two routes share a shape and be told apart by type.
</details>

---

## 🎯 BLOCK 4 — Custom Constraints

### ❌ The Problem: your rule is not on Microsoft's list

The registrar wants:

```
/students/honours/first       students with GPA 3.5 and above
/students/honours/second      GPA 3.0 up to (not including) 3.5
/students/honours/pass        below GPA 3.0
```

Three words. Not four, not any word — exactly those three, because those are the
only bands the university awards.

Look down the built-in constraint list for one that means "must be one of these
three specific words". There isn't one. `alpha` accepts `banana`. `length(5)`
accepts `first` and also `there`. `regex(^(first|second|pass)$)` would work, and it
is a legitimate answer — but a regular expression buried in a route pattern is
something the next person has to decode, and it cannot be unit-tested, reused, or
given a name that explains itself.

### ✅ The Solution: write the constraint yourself

`int`, `range` and `alpha` are not special language features. They are **classes**
that implement one interface, registered in a dictionary under a short nickname.
You can add a row to that dictionary.

```
┌──────────────────────────────────────────────────────────────────────┐
│  WHAT A CUSTOM CONSTRAINT IS                                          │
│                                                                        │
│   1.  A class implementing IRouteConstraint                            │
│           └── one method:  bool Match(...)                             │
│                                                                        │
│   2.  Registered into the ConstraintMap under a nickname               │
│           "honourband"  ──►  typeof(HonourBandConstraint)              │
│                                                                        │
│   3.  Used in a pattern exactly like a built-in one                    │
│           "students/honours/{band:honourband}"                         │
│                                    └────────┘                          │
│                                                                        │
│   The routing system cannot tell your constraint apart from            │
│   Microsoft's. They went into the same dictionary, the same way.       │
└──────────────────────────────────────────────────────────────────────┘
```

### 🔧 The three moving parts

**One — the class.** `IRouteConstraint` has exactly one method:

```csharp
bool Match(
    HttpContext? httpContext,      // the request, if there is one
    IRouter? route,                // legacy, effectively unused
    string routeKey,               // WHICH parameter you are judging: "band"
    RouteValueDictionary values,   // all route values matched so far
    RouteDirection routeDirection) // matching a URL, or generating one?
```

Return `true` and this route may match. Return `false` and routing skips it — same
fall-through behaviour as every built-in constraint, because it *is* the same
mechanism.

Note what you are handed: `routeKey`, not the value. You look the value up
yourself, using `values.TryGetValue(routeKey, out var value)`. That is what lets one
constraint class serve several different parameters in several different patterns.

The body is three steps — a guard, a conversion, and a decision:

```csharp
if (!values.TryGetValue(routeKey, out var value) || value is null)
{
    return false;
}

var band = Convert.ToString(value, CultureInfo.InvariantCulture);

return AllowedBands.Contains(band, StringComparer.OrdinalIgnoreCase);
```

`TryGetValue`, never the indexer — routing calls `Match` speculatively, so a missing
key is a normal answer, not an exception. `InvariantCulture`, never the current
culture — a URL is not written in anyone's local language, and `CurrentCulture` here
is a bug that only appears on someone else's machine. `OrdinalIgnoreCase` because
`First` with a capital F is not a mistake worth a 404.

**Two — registration.** This is a service registration, so it lives **above**
`builder.Build()`:

```csharp
builder.Services.AddRouting(options =>
{
    options.ConstraintMap.Add("honourband", typeof(HonourBandConstraint));
});
```

You hand over the **type**, not an instance. The framework constructs it when it
needs one — the same "you don't call `new`, the framework does" idea from Session
15, showing up in a completely different corner of the framework.

**Three — use it.** Identical syntax to a built-in:

```csharp
app.MapControllerRoute(
    name: "studentHonours",
    pattern: "students/honours/{band:honourband}",
    defaults: new { controller = "Students", action = "Honours" });
```

### 🍰 Analogy — Teaching the bouncer one more rule

Block 3's bouncer came with a fixed rulebook: check ID, check the dress code, check
the guest list. Useful, generic, written by someone who has never seen your venue.

Tonight is a private event with three named guest categories. You cannot express
that in his rulebook — so you write one more page, in his format, and clip it in.
From that moment he applies your page **exactly** the way he applies the printed
ones. He does not treat it as an exception or a special case; he does not even know
which pages were printed and which were yours.

That is `ConstraintMap.Add`. You are not extending routing with a special case. You
are filling in one more row of a table that was always designed to be filled in.

### ⚠️ Two things that will bite you

**The nickname must match, exactly.** Register `"honourband"` and write
`{band:HonourBand}` and the app throws at **startup** — before any request — with a
message naming the constraint it could not resolve. That is the good outcome: a
loud, immediate, unambiguous failure. Read the message; it tells you the answer.

**Never touch the database inside `Match`.** It is tempting: "only match if that
band actually has students". Don't. `Match` runs on **every candidate route of
every request**, including requests that never reach a controller — static files,
favicons, mistyped URLs. A query in there is a database round-trip on traffic that
does nothing. Constraints decide on the **shape** of a URL, never on the state of
your data.

**📌 Students May Ask — "Is this the same as `[Authorize]`? Both block requests."**

No, and confusing them is a genuine security mistake. A constraint decides which
route **matches**; a failure falls through to another route, and if one matches, the
request proceeds. `[Authorize]` decides whether a matched action may **run**, and
produces a real 401/403. Never use a constraint as a security boundary — a
different route matching is a perfectly normal outcome of failing one.

---

### 🧩 MINI-PUZZLE — Block 4 Check

`HonourBandConstraint.Match` currently ends with `return false;` for every input.
The route `students/honours/{band:honourband}` is registered above the default
route. You browse to `/students/honours/first`. What do you see?

**A)** A startup exception — a constraint that never matches is invalid
**B)** 404, and the console shows `[START]` and `[END]` with no action running
**C)** The honours page, empty — the action ran but the constraint filtered the rows
**D)** The Session 15 home page — routing fell back to `/`

<details>
<summary>Answer</summary>

**B — 404, with `[START]` and `[END]` printing and nothing in between.**

- **A** is wrong: returning `false` is a completely legal answer for a constraint.
  The framework has no opinion about how often you say no.
- **C** confuses the two layers. The constraint runs **before** any controller is
  chosen; it filters **routes**, not rows. The action never ran, so nothing could
  have been filtered.
- **D** is wrong: falling through does not mean falling back to `/`. Routing tries
  the remaining patterns against **this** URL. `{controller}/{action}/{id?}` needs
  at most three segments and this URL has three — `students` / `honours` / `first` —
  so it does try, looks for `StudentsController.honours`, does not find it, and
  404s. Close, but for a reason worth being precise about.

**Rule:** a constraint that always returns `false` makes its route unreachable —
and gives you the exact console signature of "the route refused it" from Block 3.
</details>

---

## ☕ BREAK PUZZLE 2

The route table is:

```
1.  "students/{id:int}"                        → Students / Details
2.  "students/honours/{band:honourband}"       → Students / Honours
3.  "{controller=Home}/{action=Index}/{id?}"
```

`HonourBandConstraint` accepts only `first`, `second`, `pass` — case-insensitively.

You browse to `/students/honours/THIRD`. Which statement is true?

**A)** `Match` is never called, because route 1 matched first
**B)** `Match` is called and returns `true` — the comparison ignores case, so
`THIRD` is accepted
**C)** `Match` is called twice: once for route 1's `{id:int}` and once for route 2
**D)** `Match` is called once, returns `false`, and the request ends as a 404

<details>
<summary>Answer</summary>

**D — `Match` is called once, returns `false`, and the request 404s.**

Walk the table:

- **Route 1** is two segments; the URL has three. Eliminated on shape, before any
  constraint is consulted. So **A** is wrong (route 1 did not match) and **C** is
  wrong (`{id:int}` is the built-in integer constraint, not your `Match`; your class
  is only attached to `{band:honourband}`).
- **Route 2** matches on shape, so routing asks your constraint about `band`.
  `"THIRD"` is not in the allowed list, so `Match` returns `false` — case-insensitive
  means `THIRD` would be accepted if the *word* were right, and `third` is not one
  of the three words at all. **B** confuses ignoring case with ignoring content.
- **Route 3** then tries: three segments, so it looks for `StudentsController` with
  an action named `honours`. There is no such action. Nothing matches. **404.**

**Key insight:** every rejection today is a rejection of a **route**, not of the
request. The 404 is what is left over when the whole table has been walked and
nothing claimed the URL.

**Bridge:** every route so far has been declared in `Program.cs`, far away from the
method it points at. After the break: what happens when an action carries its own
address instead — and where information belongs when it is not part of the address
at all.
</details>

☕ **Break — 10 minutes.**

---

## 🎯 BLOCK 5 — Attribute Routing & the Query String

### ❌ The Problem: the address is a two-file scavenger hunt

To answer "what URL reaches `Honours`?" you have to open `Program.cs`, find the
route whose `defaults` mention `Honours`, and read its pattern. The address and the
method are in different files, and nothing in the compiler connects them. Delete the
route and the method silently becomes unreachable — no error, no warning.

That is tolerable for four routes. It is miserable for forty, and worse for an API
where nearly every action has its own distinct address.

### ✅ The Solution: attribute routing — the address written on the action

```csharp
[Route("students/search")]
public async Task<IActionResult> Search([FromQuery] string name)
```

The route pattern now sits directly above the method it routes to. One place to
look, and it moves with the method when the method moves.

⚠️ **The rule that surprises everyone:** once an action carries its own route
attribute, it **stops answering conventional routes entirely**. `/Students/Search`
returns 404 after adding `[Route("students/search")]`. That is not a bug to work
around — it is the guarantee that makes attribute routing worth using: the address
is exactly what the attribute says, and nothing in `Program.cs` can quietly give
that method a second address behind your back.

### 🔧 Conventional vs. attribute — when to use which

| | Conventional (`Program.cs`) | Attribute (`[Route]`) |
|---|---|---|
| Where it lives | One table, one file | On each action |
| Best for | Whole sections sharing a shape | Actions with individual addresses |
| Reading one URL | Search the table | Look above the method |
| Seeing the whole API | Read one file, top to bottom | Read every controller |
| Enforcing consistency | Strong — the pattern applies to everything | Weak — every action decides for itself |
| Typical real project | Server-rendered MVC sites | Web APIs |

They coexist in the same application, which is exactly what today's project ends up
proving. This is not a "which is better" question, and answering it that way in an
interview is a mistake:

> 🎯 **Interview answer:** "Conventional routing puts the URL design in one place,
> which is what you want when whole sections of a site share a shape. Attribute
> routing puts each address next to the method that serves it, which is what you
> want when addresses are individual — which is why almost every Web API uses it.
> Most real applications use both, and an action carrying its own attribute stops
> answering the conventional routes."

### 🔧 Route data vs. the query string

Two ways a URL carries information, and they are not interchangeable:

```
┌──────────────────────────────────────────────────────────────────────┐
│   /students/3?highlight=gpa                                           │
│    └────────┘ └───────────┘                                           │
│     the PATH    the QUERY STRING                                      │
│                                                                        │
│   PATH  — WHICH resource this is.                                      │
│           Part of the address. Routing matches on it.                  │
│           Change it and you are looking at a different thing.          │
│           /students/3  and  /students/4  are two different pages.      │
│                                                                        │
│   QUERY — HOW you want it. Filtering, sorting, paging, searching.      │
│           NOT part of the address. Routing ignores it completely.      │
│           Change it and you are looking at the same thing, differently.│
│           /students?sort=gpa  is still the students page.              │
└──────────────────────────────────────────────────────────────────────┘
```

The practical test, and the one to say out loud in an interview:

> 📌 **If two values identify two different things, that belongs in the path. If two
> values are two views of the same thing, that belongs in the query string.**

A search term is not an identity — `?name=nada` and `?name=omar` are two views of
"the students page". So it belongs in the query string, and putting it in the path
would be wrong even though it would work.

`[FromQuery]` states that explicitly:

```csharp
public async Task<IActionResult> Search([FromQuery] string name)
```

Model binding would have found `name` in the query string anyway — the attribute
buys **nothing** at runtime. It buys everything at reading time: the signature now
tells the truth about where its data comes from, without opening `Program.cs`.

### 🔬 The payoff — the route table IS your public API

Read the finished table top to bottom:

```
name: "studentsList"     students                              →  the roster
name: "studentDetails"   students/{id:int}                     →  one student
name: "studentsByYear"   students/year/{year:int:range(1,4)}   →  one academic year
name: "studentHonours"   students/honours/{band:honourband}    →  one honours band
name: "default"          {controller=Home}/{action=Index}/{id?}   ← catch-all, last
```

Five lines. That is not configuration — that is a **published contract**, designed
on purpose, readable by someone who has never seen your C#.

**Now the demonstration to watch closely.** Comment out **all five** routes and
re-run. Predict first: how much of the site still works?

Everything 404s — `/`, `/students`, `/students/3`, `/Home/Privacy` — **except one
address**:

```
https://localhost:7019/students/search?name=a     ✅ still works
```

The only URL left alive in the entire application is the one whose action carries
its own address. Every other page in the site depended on a table in `Program.cs`
that no longer has anything in it.

> 📌 **That is the session in one screen:** URLs are not a property of your
> controllers. They are a thing you declare — in one table, or on each action — and
> when you stop declaring them, the controllers are still perfectly good code that
> nobody on earth can reach.

Then uncomment the five lines and the site comes back.

**📌 Students May Ask — "Can one action have both a `[Route]` and a conventional
route?"**

Not for the same action. Once an action has any route attribute, conventional
routes skip it entirely. You *can* stack multiple attributes on one action to give
it several addresses (`[Route("students/search")]` and `[Route("find-student")]`
together), and both will work — but the conventional table no longer applies to it
either way.

---

### 🧩 MINI-PUZZLE — Block 5 Check

`Search` is decorated with `[Route("students/search")]`. The conventional table
still contains the default route. Which of these returns the search page?

**A)** `/Students/Search?name=nada`
**B)** `/students/search/nada`
**C)** `/students/search?name=nada`
**D)** Both A and C — the attribute adds an address, it does not remove one

<details>
<summary>Answer</summary>

**C — only `/students/search?name=nada`.**

- **A** was the address before the attribute was added. Adding a route attribute
  makes the action invisible to conventional routing, so this now 404s. This is the
  single most-missed fact about attribute routing.
- **B** puts the search term in the **path**. The attribute's pattern has no
  parameter segment, so a third segment has nowhere to go and the pattern does not
  match. (It is also the wrong design: a search term is not an identity.)
- **D** is the intuitive answer and the wrong one — attributes **replace** the
  conventional address for that action, they do not add to it.

**Rule:** an action that carries its own address answers at that address and
nowhere else.
</details>

---

## 📊 SESSION SUMMARY

| Concept | What It Does | Why It Matters |
|---|---|---|
| **The route table** | An ordered list of patterns; first match wins | The URL becomes a method call here, and nowhere else |
| **Pattern anatomy** — `{x}`, `=default`, `?` | Captures, falls back, allows absence | One line covered `/`, `/Home`, and `/Home/Privacy/3` all week |
| **Custom routes** | Move `controller`/`action` out of the URL into `defaults` | The address describes the page, not your class names |
| **Route order** | Specific above general, catch-all last | Getting it wrong breaks *some* pages — the worst kind of bug |
| **Built-in constraints** — `int`, `range`, `alpha` | Narrow what a segment accepts | Bad URLs are refused before a controller or DbContext exists |
| **Constraint ≠ validation** | A failed constraint skips the route and falls through | Explains why `/students/nada` can reach a *different* action |
| **Custom constraint** (`IRouteConstraint` + `ConstraintMap`) | Your rule, applied exactly like Microsoft's | Routing was designed to be extended — you filled in a row |
| **Attribute routing** (`[Route]`) | The address lives on the action | One place to look; the action stops answering conventional routes |
| **Path vs. query string** | Path = *which* thing; query = *how* you want it | The design test to say out loud in an interview |

---

## 🧪 Today's Lab

Three hours, in the room, graded. You will extend today's route table with routes
of your own — including a **custom constraint you write from scratch** whose valid
values are derived from your **Personal Lab ID**, so no two correct submissions in
this room can be identical.

You will also add one attribute-routed action whose address contains **your own
first name**, and the Wrap-Up Reflection asks you about **your own** derived values,
not a generic example.

Full task, rubric and time budget: **`Lab/Lab_16_Task.md`**.

Bring your Lab ID. Hamdy calls it out at the start of lab time.

---

## 🔜 Next Session

```
Session 16 (today)
├── The route table turns a URL into a controller and an action
├── Custom routes let you design the address instead of exposing class names
├── Constraints refuse bad URLs before any of your code runs
├── A custom IRouteConstraint plugs your own rule into the same mechanism
└── Attribute routing puts the address on the action; the query string
    carries how, not which
    │
    └── Session 17: Controllers
        ├── Today every route ended at an action we borrowed without
        │   explaining. Tomorrow that action is the subject.
        ├── IActionResult — today we returned View() and NotFound() and
        │   moved on. What else can an action return, and why is the
        │   return type an interface at all?
        ├── Model binding, properly — today `id` arrived from the route
        │   and `name` from the query string, and we waved at how. Tomorrow:
        │   the full set of sources, and what happens when they disagree.
        └── Validation — the guard clauses we keep writing by hand, done
            the way the framework intends.

    The switchboard is built. Now we meet the person who answers the phone.
```
