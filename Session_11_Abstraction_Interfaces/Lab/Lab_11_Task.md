# 🧪 Lab 11 — Abstraction + Interfaces + OOP Capstone
## ITI Summer Training | Web Development Using .NET | Morning Group

---

## 🔑 Your Personal Lab ID

Use the **same Personal Lab ID** Hamdy assigned you in Session 10 — it is not reassigned today.
If you've genuinely forgotten it, ask before starting; do not guess. Today threads it into TWO
of `Admin`'s values (Part E), continuing Session 10's `AccessLevel` and adding a brand-new one
for today's Interfaces content — both explained below, both required, both must trace back to
YOUR number specifically.


## WARNING : DO NOT ASK On THE GROUP OR ON THE LIVE SESSION .... YOUR TASK WILL BE REJECTED ❌
## ASK In the Private chat ✅
 
---

## 🎯 What You're Building

Today you finish Part 2 of the syllabus by extending yesterday's `Person`/`Student`/`Instructor`
hierarchy with real Abstraction (an abstract `Person` class, one virtual and one abstract
method) and Interfaces (`IPrintable`, implemented across the whole `Person` family AND by
`Course`, which isn't a `Person` at all). You'll also extend Session 10's `Admin` class so it
satisfies both, add a second interface of your own design to prove multiple-interface
implementation, and finish by wiring a working capstone menu that exercises Encapsulation,
Polymorphism, Inheritance, and today's Abstraction/Interfaces together in one running app —
exactly like the lecture's capstone, but built by you, with your own personalized values.

---

## ✅ What You Need Before You Start

- Yesterday's `Session_10_Inheritance/Application/StudentPortalConsole_Complete/Program.cs`,
  confirmed building and running.
- Your copy of today's `Session_11_Abstraction_Interfaces/Application/StudentPortalConsole/`
  TODO project, open and ready to edit.
- Your Personal Lab ID from Session 10 (see above).

---

## Part A — Setup (confirm, don't build)

Open today's TODO project. Confirm it currently contains `Person`, `Student`, `Instructor`, and
`Course` copied forward from Session 10/09, unchanged so far — nothing today's Part B onward
asks you to build should already exist yet. If your copy is missing any of these, copy them
forward from Session 10's Complete Solution before continuing.

---

## Part B — Predict-the-Output Drills (write your answer BEFORE running anything)

For each snippet below, write down what you believe happens — compiles and prints something
specific, or fails to compile with a specific reason — before running any code.

**B1.**
```
abstract class Shape
{
    public abstract double GetArea();
}

Shape s = new Shape();
```

**B2.**
```
abstract class Shape
{
    public abstract double GetArea();
}

class Square : Shape
{
    public double Side;
}
```

**B3.**
```
class Animal
{
    public virtual string MakeSound() { return "..."; }
}

class Cat : Animal
{
    public override string MakeSound() { return "Meow"; }
}

Animal a = new Cat();
Console.WriteLine(a.MakeSound());
```

---

## Part C — Make `Person` Abstract

Following today's lecture pattern exactly: mark `Person` itself `abstract`. Add a `virtual`
`PrintBasicInfo()` if it isn't already (carry forward Session 10's version, marking it
`virtual`). Add a new `abstract` method, `GetRoleDescription()`, returning text, with no body.
Override both in `Student` and `Instructor` — `PrintBasicInfo()` should call `base.
PrintBasicInfo()` first, then print role-specific detail; `GetRoleDescription()` should return
`"Student"` / `"Instructor"` respectively.

---

## Part D — Define and Implement `IPrintable`

Define `IPrintable` with one method, `PrintDetails()`, no parameters, no return value. Implement
it on `Student` and `Instructor` (each calling their own `PrintBasicInfo()`), AND on `Course` —
which must NOT derive from `Person` at all. `Course`'s `PrintDetails()` should call its existing
roster-printing method.

---

## Part E — Extend `Admin` (Fingerprinted — Your Own Numbers Required)

Bring `Admin : Person` forward from your Session 10 Lab. Today it must ALSO:

1. Override `GetRoleDescription()`, returning `"Admin"`.
2. Override `PrintBasicInfo()` (or keep your Session 10 version, adjusted to call `base.
   PrintBasicInfo()` first if it doesn't already).
3. Implement `IPrintable`'s `PrintDetails()`.
4. Implement a SECOND, brand-new interface you define yourself, named `IRankable`, with one
   method: `GetRankScore()`, returning a whole number. `Admin`'s implementation must return
   exactly **`(your Lab ID mod 4) + 1`** — e.g. Lab ID 7 → `(7 mod 4) + 1 = 4`; Lab ID 9 →
   `(9 mod 4) + 1 = 2`. Write this literal computed number as a comment next to the `return`
   line so it's easy to verify at a glance.
5. Keep `AccessLevel`'s valid range from Session 10 (`1` to `(your Lab ID mod 3) + 2`) —
   unchanged, still required, still yours specifically.
6. `Admin`'s class declaration must now read
   `class Admin : Person, IPrintable, IRankable` — proving one class implementing TWO separate
   interfaces at once, on top of its one base class.
7. Create (or keep, if carried from Session 10) exactly one `Admin` object using **your own real
   name** as its full name.

---

## Part F — Capstone Menu Wiring

Extend `Main`'s menu with two new options, matching today's lecture pattern:

1. **Print everyone** — build ONE `List<Person>` containing every `Student`, `Instructor`, AND
   your `Admin` object, then loop over it calling the virtual/override `PrintBasicInfo()` method
   and the abstract/override `GetRoleDescription()` method on each — zero casts, zero
   type-checking `if`/`else`.
2. **Print everything printable** — build ONE `List<IPrintable>` containing every `Student`,
   `Instructor`, `Admin`, AND every `Course`, then loop over it calling `PrintDetails()` on each.

Also add a third new option: **show rank scores** — loop over a list containing every object
that implements `IRankable` (in today's build, that's just your `Admin`, but write the loop
generically against `List<IRankable>` rather than hard-coding just `Admin`) and print each
one's `GetRankScore()`.

---

## Part G — Wrap-Up Reflection

Answer in a few sentences, written into a comment block at the bottom of your `Program.cs`:

1. State your Lab ID, your `Admin`'s resulting `AccessLevel` upper bound, and your `Admin`'s
   `GetRankScore()` result — with the arithmetic shown, not just the final numbers.
2. Explain, in your own words, why `Course` implementing `IPrintable` is valid even though
   `Course` is not, and never will be, a `Person`.
3. Explain what specifically would break, or need to be duplicated, if you had tried to give
   `Admin` its `GetRankScore()` capability by making it inherit from a SECOND base class instead
   of implementing `IRankable` — name the actual C# rule this runs into.

---

## 📋 Grading Rubric (sums to 100)

| Part | Points | What's Graded |
|---|---|---|
| B — Predict-the-output | 10 | All 3 correct, written BEFORE running |
| C — Abstract Person | 20 | `abstract class`, virtual + abstract method, both overridden correctly in Student/Instructor |
| D — IPrintable | 20 | Correct interface definition + implementation on Student, Instructor, AND Course |
| E — Admin extension | 25 | GetRoleDescription/PrintBasicInfo/PrintDetails/IRankable all correct, AccessLevel and GetRankScore match the trainee's OWN Lab ID formula, self-named object present |
| F — Capstone wiring | 15 | Both new menu options work correctly, zero casts in the Person loop |
| G — Reflection | 10 | All 3 points answered, correctly citing the trainee's own numbers |

---

## ⏰ Time Budget (sums to 180 min / 3h)

| Part | Suggested Minutes |
|---|---|
| A — Setup | 10 |
| B — Predict-the-output | 15 |
| C — Abstract Person | 35 |
| D — IPrintable | 35 |
| E — Admin extension | 35 |
| F — Capstone wiring | 35 |
| G — Reflection | 15 |

---

## 🙋 If You Get Stuck

Before asking: re-read today's Student Guide Block that matches the part you're stuck on — every
part traces to one specific block. Check whether your compiler error names a specific error
code (`CS0144`, `CS0534`, `CS0535`) — all three appear in today's lecture and mean something
specific. If still stuck after that, flag Hamdy — bring the exact error message, not just "it's
not working."
