PROGRAMING PARADIGMS
====================

There is a lot of programming languages in use nowadays.
TIOBE Index web site, which examines popularity of programming languages, keeps track of more than 150 programming languages {{{tiobeindex}}}, while the total number of all notable programming languages exceeds a thousand {{{wikilistpl}}}.

The most common classification of programming languages relies on programming paradigms. 
Paradigm determines some kind of abstract pattern that is followed by a family of languages.
One of the first attempts to describe methods of organizing code was made by Dijkstra, who gave it a name of Structured Programming {{{dijkstra1970notes}}}.
In 1978, Floyd received Turing Award for his paper, where he described the notion of a paradigm and how it can influence language designers {{{floyd1979paradigms}}}.
Those early works prove that the concept and its impact on programming has been studied since the beginning of software industry.

In this section most popular programming paradigms are surveyed.
The thesis focuses on a clear separation between imperative and declarative paradigms, and describes their derivatives.
Since there exist plenty of different taxonomies for programming paradigms, the presented categorization should not be treated as the only appropriate one.
It must be emphasized that most programming languages usually span over more than one paradigm, which makes them labeled with multi-paradigm notion.
The paradigms are not thus mutually exclusive in context of a specific language.

Imperative
----------

Imperative word looked up in a dictionary means *"giving an authoritative command"* {{{oxforddic}}}.
That definition summarizes what really imperative programming is all about.
When writing software in imperative manner, the programmer gives commands to a computer and tells it what to do, step by step.
Imperative relies on explicit modification of state in a program with use of statements, in order to achieve a desired goal.
The term does also relate to linguistics, where it denotes *"mood of a verb to express a command or exhortation"* {{{oxforddic}}}.

### History

First programming languages were machine languages, which offered Application Programming Interface (API) consisting of register manipulations, jump instructions, basic arithmetics and logic operators.
Machine languages API was therefore based on giving lowest level commands to the processor.
Because of their nature, machine languages became the precursors of imperative programming.
Building complex systems with machine code usually resulted in enormous code-bases, which turned out to be painful to maintain.

That is why in the late 50s and in the beginning of 60s languages of higher level were introduced.
Among them there was FORTRAN, COBOL and BASIC, all of which can be considered imperative.
They aimed to reduce (comparing to the machine languages) cost of code maintainability, by abstracting certain sets of machine code instructions with statements and constructs better readable by a human.

In 70s, languages like Pascal and C were created.
While Pascal served mainly for educational purposes, C language quickly became a mainstream programming language and at the time of the writing still remains one of the most popular in ranks {{{tiobeindex}}}.
With its distinctive syntax, C originated a whole family of languages, commonly known as "C-based syntax" languages.
Such constructs as `for` and `while` loops with their syntax proposed by C, are the most basic building blocks for an imperative program in majority of contemporary languages.

Together with the 80s came a growing interest in object-oriented approach, which also has it roots in imperative approach.
From the imperative category, Smalltalk and C++ were invented, the latter being "extension" to the C language with concept of objects among its new features.

The imperative approach evolved since then, with languages like Python, Visual Basic, PHP, Java or Ruby developed in the 90s and C# after the Millennium.
Despite some of these languages aimed to target multiple paradigms, the concept of building program's flow with help of mutating statements and other imperative constructs remained ubiquitous.

### Object-oriented

Majority of imperative programming languages that were developed till the 80s have procedural nature.
This means that they focus on defining reusable procedures which invoke certain actions.
Procedures are stateless, and as a result they cannot carry any kind of data with them.
OOP was introduced in order to combine data with behavior inside an object, in order to relate object to its internal state and applicable operations.
OOP follows imperative approach with regards to how the behavior inside an object (in form of methods or members) is implemented.
Following are listed a few design principles that the paradigm outlines.

**Objects** are the basic building blocks of a program in OOP world.
In some languages, every data structure including primitive types is an object (it is sometimes common to say that "Everything is an object").
Instance of an object is usually shaped by the concept of a class, which defines available members - fields for storing data and methods for defining behavior.

**Encapsulation** is an interesting principle of OOP software, that often gets misunderstood.
It aims to hide information, that is to make the internals of an object invisible to the outside scope, but to provide a public interface that does not leak unnecessary state for other components interacting with that object.
Encapsulation is fulfilled when the call site neither has to make any assumptions about nor be aware of existence of the internals of called object.
Another consequence of proper encapsulation is the effect that a referenced object should never go into invalid state which makes it unusable.
The misconception occurs because developers tend to treat encapsulation as a way of sharing internal state through trivial methods, known as getters and setters in Java and properties in C#.
Getters and setters allow to query for or alter (respectively) data that often should be available only inside the referenced object itself.
Exposing internals of an object (private fields) to the outside world with the help of those methods not only refrains proper encapsulation, but also denies it by making the call-site aware of referenced object's implementation details.

**Inheritance** allows a class to derive behavior as well as state from another class.
The derived members can then be reused to implement more specific sub-class methods.
Thanks to the mechanism, some implementation can be shared between multiple classes of objects and thus duplication of code can be avoided.
On the other hand, languages usually allow for shadowing of derived members, that is overriding definition of a member, which can result in spoiled abstraction.
Inheritance also leads to forming class hierarchies which easily get big and complex.
It turns out that the same effect that is achieved thanks to inheritance can also be achieved with help of composition, and indeed the latter is a recommendation given by these frequently cited words {{{gamma1994design}}}:

>> *"Favor object composition over class inheritance."*

**Polymorphism** is a concept that in OOP approach relies mainly on inheritance.
"Poly" standing for "many" and "morphism", which can be thought of "form", describe the ability to treat objects of different classes in unified way ("morphism" has also its formal definition in category theory, however it is not covered by the thesis).
In OOP software this is usually achieved by deriving behavior (and data) from a base class, which enables to refer to an object of specific type with more general, base class type.
The ability of polymorphism is therefore enforced by specific class hierarchies.
The polymorphism concept relying on class hierarchies is known as ad hoc polymorphism {{{strachey2000fundamental}}}.
Section {{FUNCTIONAL PROGRAMMING IN INDUSTRY}} mentions also parametric polymorphism which is to some extent available in a few OOP languages.

**Abstraction** is closely related to polymorphism, and allows to treat referenced objects as if they were of the most possible general type, while in runtime they appear to be an instance of a very specific class.
Abstraction enables better testability of code, as well as late binding, which can be used for the purpose of inversion of control (IOC).
As stated above, when not cautious, abstraction can be easily spoiled in OOP by overridden behavior or leaking internals of an object.

Many other principles and design patterns apply to OOP.
With the SOLID acronym, Martin {{{martin2003agile}}} presents set of five principles to keep software maintainable.
Multiple design patterns described in the famous Design Patterns book {{{gamma1994design}}} demonstrate common approaches that address cross cutting concerns in OOP software.
While it is certainly a good thing that such patterns are established for software engineers to follow, majority of them tend to be quite cumbersome, hence utilizing them cause code complexity to grow rapidly.
Many of the standardized OOP patterns seem to solve problems that the programming paradigm introduced itself in the first place.

### Referential opacity

Imperative and derivative paradigms suffer from referential opacity - an opposite of referential transparency property.
Tennent in his work explains {{{tennent1976denotational}}}: 

>> *"Main actions performed in imperative paradigm are updating statements, jumps and intermediate input / output, which spoil referential transparency by introducing the possibility of "side effects" or transfers of control during expression evaluations."*

Referential transparency has already been defined a few times {{{sondergaard1990referential, strachey2000fundamental}}}, where it concerns more complex topics such as non-determinism or definiteness.
In context of this thesis and basic paradigms' characteristics it is enough to say that referential transparency describes a property of programming language, which enables to substitute certain expression with another one that evaluates to the same value, without impact on the program's behavior.
The downsides of imperative program actions mentioned by Tennent {{{tennent1976denotational}}} with regards to referential transparency can be explained as following:

* **side effects** - if an expression evaluates to a certain value, but it has side effects, such as input / output (IO) operations, then it cannot be replaced with another expression without changing behavior of the program, even if the latter expression evaluates to the same value;
* **transfers of control** - if an expression consists of more than one exit points, and the exit point of expression is determined by condition branching (jump statements), then the value of such expression is ambiguous because control can be transferred from arbitrary point of expression to the outside scope.

In programming practice, these drawbacks lead to difficulties with understanding program's flow and behavior, which in turn imply spending more time on code debugging.

Declarative
-----------

Declarative paradigm is different than imperative, with regards to how programs are built.
Program written in declarative style focuses on **what** is the desired goal rather than **how** this goal has to be achieved {{{petricek2009real}}}.
This means that code does not give a recipe for how low level instructions are to be executed in what order, but rather synthesizes a set of constructs that are capable of being glued (combined) together.
Term declarative refers to the code, which is written by **declaring** how such constructs relate to themselves, but it also has its roots in linguistics {{{nilsson1990logic}}}:

>> *"A declarative sentence is a complete expression of natural language which is either true or false, as opposed to imperative (...) sentences."*

In his work, Padawitz {{{padawitz2006deductive}}} describes the declarative programming term as *"a combination of functional (or applicative) and relational (or logic) programming"*.
As it turns out, both functional and logic programming can be thought of sub-paradigms of declarative programming paradigm.
Before functional programming is discussed in details, other examples of specific and based on declarative approach sub-paradigms (including mentioned logic programming) are listed together with brief description.

### Logic programming

Logic programming builds on top of logic formulas, which describe relations between objects in an isolated world.
In order to be processable, logic formulas have to obey a specific formalized syntax.
Language constructs associated with the syntax are referred to as **predicate logic**, and include {{{nilsson1990logic}}}:

* **variables** and **constants**, which stand for individuals (objects) in the isolated world,
* **functors**, for representing composites (such as family) of individuals; they denote functions over objects in the domain and build up **compound terms**,
* **predicate symbols**, that describe relations between the objects,
* **logical connectives**, among which there is $\wedge$ (conjunction), $\lor$ (disjunction), $\to$ (implication), $\lnot$ (negation) or $\leftrightarrow$ (logical equivalence),
* **quantifiers**, like universal $\forall$ (for all) or existential $\exists$ (exists),
* **auxiliary** symbols, such as parentheses or comma.

When relations on objects are described in formalized syntax, programming system can apply reasoning for given facts and draw certain conclusions.
That is exactly how Prolog, the most popular logic programming language, works.
Prolog with its syntax allows to describe facts in predicate logic language, and derive interesting outcomes that it manages to reason.

### Constraint programming

Constraint programming aims to incorporate a declarative principle of applying a set of restrictions (constraints) into programming languages (also those from imperative background), in order to infer possible results.
Citing the very first sentence of Apt's book {{{apt2003principles}}}:

>> *"Constraint programming is an alternative approach to programming in which the programming process is limited to a generation of requirements (constraints) and a solution of these requirements by means of general or domain specific methods"*

Standard approach to solving problems using Constraint programming is constructed in following way {{{apt2003principles}}}:
First, the problem instance has to be defined as a **Constraint Satisfaction Problem** (CSP) with distinction of all variables, their domains and constraints that apply for those variables.
Next, in a loop, a number of operation is performed including preprocessing the CSP instance to syntactic form, propagating constraints in order to simplify the CSP, and unless terminating condition is not met, the problem is divided into smaller ones, each of which is being processed recursively.
Finally, the output of the algorithm, built from combination of sub-solutions, determines the result.

Constraint programming can be applied to plenty of specific problems that are representable in terms of abstract areas, such as:

* **Linear and integer programming**, where given a set of constraints, certain variable has to be maximized or minimized;
* **Linear algebra**, in which the available vector spaces are browsed to find a consistent solution to a problem;
* **Global optimization**, that examines all inputs to determine global extrema, as opposed to local optimization.

### Domain specific languages

Domain specific language (DSL) term has been defined a number of times.
One definition, proposed by Van Deursen et al. {{{van2000domain}}} is as follows:

>> *"A domain-specific language is a programming language or executable specification language that offers, through appropriate notations and abstractions, expressive power focused on, and usually restricted to, a particular problem domain."*

DSLs are hence usually of small size, and do their best to address a certain (domain-specific) problem.
This is achieved by restricting set of possible instructions and operators to the bare minimum, essential for solving given issue.
Since they aim to make the resulting code as succinct as possible, DSLs tend to have declarative nature.
DSLs are implemented both as compilable and interpreted languages.
They can be built on top of an existing programming language, benefiting from extending the underlying syntax.
Alternatively, a DSL might also be a stand-alone programming language not associated with any specific general-purpose language (GPL).

Most popular DSLs include {{{van2000domain, bentley1986programming}}}:

* **PIC** - for drawing graphs,
* **YACC** - for parsing source code,
* **MAKE** - for defining software build scripts,
* **SQL** - for manipulating relational databases,
* **HTML** (further discussed in section {{APPLICATION OF FUNCTIONAL PROGRAMMING}}) - for generating markup that web browsers can render,
* **XSLT** - for transforming XML documents.

Functional
----------

Functional programming is strongly connected with declarative paradigm, and the connection manifests with the way functional programs are built. 
The most basic building block in FP is a function, which follows a number of properties.
Among the properties, there is function composition, which gives a great ability to compose (combine) functions with help of special constructs.
Thanks to function composition, full programs can be developed by simply composing granular functions together, hence the relation to declarative paradigm.
FP has also its roots in mathematics.
Among concepts known from mathematics, which are the foundations for functional paradigm, there are:

* **Lambda calculus** - formal mathematical system which became the base of functional paradigm,
* **Functions** - treated as first-class citizens in FP; their purity as seen by mathematicians allows to use powerful programming techniques,
* **Category theory** - this very abstract field of mathematics is reflected in type system of statically typed functional languages.

In this section focus is laid on pointing out FP properties (or features), which differentiate the paradigm from others.
Each property comes with a brief description followed by a rationale on why it is useful and a listing demonstrating the property and its usage in practice.
All example listings are in F# language.

### Immutability

Immutability is one of the most basic properties of FP.
In imperative paradigm, assignments are made to variables which, as the name suggests, can vary (be mutated) during run-time.
In functional world on the other hand, to emphasize that values are immutable, it is said that there occurs a binding of a value to a symbol. 
Immutability enforces that all values which are evaluated during program execution, once bound to a symbol, cannot be mutated.

For newcomers the property might initially look restrictive, but it turns out that it does not imply any constraints.
Immutability helps with understanding the flow of program logic, and is a prerequisite of referential transparency when it comes to deferred execution.
That is because, if a deferred function refers to a variable that may or may not be modified, the result of evaluating such function is indeterministic.
The greatest benefit from immutability is discovered in context of concurrent programming, because immutable values are thread-safe by default (they cannot change).

Listing {{funimmutability}} presents how immutability works in F#, with the `let` keyword meaning binding of a value on the right side of `=` operator to the symbol on left side.
Both `x` and `y` values in listing {{funimmutability}} are immutable, and an attempt to assign different value to `x` symbol (line 3) fails.
F# compiler treats expressions like the one in line 3 as equality test, and since `x` is not equal to `6`, the expression evaluates to false.

```xxx
{FSharp]{Immutability}{funimmutability}
let x = 5      // value x is immutable
let y = x + 3  // value y is immutable
x = 6          // this expression evaluates to false```

### Pattern matching

For developers coming from OOP background, familiar with languages like C# or Java, pattern matching (in its most primitive form) could be explained as a switch statement in conjunction with an assignment to a symbol for each branch.
Pattern matching does not always have to concern multiple cases - sometimes it proves useful with a single case, when such case always applies (for example when unwrapping a value from a data structure, or processing an infinite list).
The construct is part of syntax of several programming languages, both imperative and declarative, but is specially popular among functional programmers.

There is more to pattern matching than just an ordinary switch statement.
Besides the fact that it combines logic branching with assignments (which in practice turns out really convenient), it relies on the concept of Algebraic Data Types (ADT), ubiquitous in functional world.
ADT concerns such concepts as sum and product types, first of which describes types that can have a restricted set of applicable values, and the latter which describes "tuples", a cartesian product of sub-types.
Without going into too much details, it is enough to say that ADT in context of pattern matching allows to express all possible variants for a value of a certain type.
This in turn results in features such as:

* **compile warning** (in statically typed languages) when not every case is covered,
* **nested pattern matching** that allows to dissect a recursive structure.

Listing {{funpatternmatching}} shows a basic pattern matching construct applied to a list.
Function `printIsEmpty` tries to match the `list` parameter with patterns: `[]` which corresponds to an empty list, and `head::tail` where `::` is a binary operator for operands of type `'a` and `'a list`, which prepends the element from first argument (`head`) to the list (`tail`) from second argument.
`tail` might be empty, and in that case `head::tail` denotes a single-element list.
In other words, pattern matching construct seen in listing {{funpatternmatching}} recognizes two cases: in first case the list is empty, and in the second case there is at least one element in the list (`head`).
If the second case was omitted, the compiler would issue a warning in compile-time.

```xxx
{FSharp]{Pattern matching}{funpatternmatching}
function printIsEmpty list =
    match list with
    | [] -> "list is empty"
    | head :: tail -> "list is not empty"```

### Pure functions

Pure functions allow to associate programming language functions with the mathematical definition.
A function is pure in mathematical sense, when for a given set of arguments it always returns the same value.
In context of programming languages, one can say that a result of pure function does not depend on anything, but the arguments this function takes.

Again, as was the case with immutability, function purity property might seem impossible to achieve in a real world application.
Every software needs to communicate with components outside its process, for example by invoking IO operations or accessing computer's clock.
Haskell, which happens to be purely functional, is used in real systems while preserving function purity property.
This is achievable with concept called "Monads" {{{mcbride2008applicative}}}, which also has its formal mathematical definition. 
Monads are quite complicated topic itself, therefore they are not addressed by this thesis (explicitly, but they are being used in section {{APPLICATION OF FUNCTIONAL PROGRAMMING}} anyway).
Thanks to the function purity property, a number of benefits are gained, including:

* **memoization** technique can be used. It allows to store results of function invocation in a hash array for the purpose of later, constant in time retrieval;
* **lazy evaluation** (discussed in further course) is possible, because functions do not depend on the order of execution or any values apart from the function parameters;
* **easier comprehension** - a reader can focus entirely on input parameters to deduce function behavior;
* **testability** - as long as a function depends only on its arguments, it is straightforward to write automatic tests for that function in isolation.

Listing {{funpurity}} demonstrates two functions, `pureSalary` and `impureSalary`.
Both functions have the same type signature, as they take two `decimal` arguments: `hours` and `rate`, and return `decimal` salary computed for a work day.
Despite they have the same type signature, the functions do differ with regards to purity.
While `pureSalary` does not depend on any value from outside and always returns the same result for given arguments, the `impureSalary` takes an implicit dependency on `DateTime.Now.DayOfWeek` property, which depends on the current (at the time of executing) day of week.
As F# is not purely functional, it cannot enforce pure nature of functions, and thus the `impureSalary` compiles correctly.

```xxx
{FSharp]{Example of pure and impure functions}{funpurity}
let pureSalary (hours: decimal, rate: decimal) =
    hours * rate

let impureSalary (hours: decimal, rate: decimal) =
    if DateTime.Now.DayOfWeek = DayOfWeek.Saturday then
        hours * rate * 1.5M
    else
        hours * rate```

### Higher-order functions

Functions in FP languages are often referred to as "first-class citizens".
This usually means that functions are treated just as any other ordinary value and are allowed to be combined together.
A function is called higher-order when it returns or takes as a parameter another function.
The concept of function pointers introduced in C and C++ is closely related to higher-order functions, though it does not guarantee type safety like in statically typed functional languages.

Higher-order functions construct a powerful mechanism that enables to abstract away a common piece of logic and make it more reusable.
It addresses the problem, present in many imperative languages, of code repetition for recurrent tasks such as working with lists (for example filtering or mapping).
In addition to that, higher-order functions can be used as a foundation for concept known in OOP as "Dependency Injection".
Dependency Injection in OOP paradigm relies on passing abstract interfaces without implementation to consuming classes.
The interfaces passed to dependent class contain only type signatures of methods to be invoked, hence such interface can be easily represented with traditional functions.

Listing {{funhof}} shows example of using higher-order function `List.Filter`.
It takes two parameters: a function of type `'T -> bool` and a list of type `'T list`.
The list passed as the second argument contains numbers from 1 to 10, and the predicate function passed as the first argument is an anonymous function (lambda expression) which tests whether a number is even or not.
Thanks to the `List.filter` function being higher-order, one can imagine whole spectrum of predicate functions that could be applied to filter specific elements from the list, and since the function is generic in type `'T` it can accept lists of any type.

```xxx
{FSharp]{Higher-order functions}{funhof}
let evenNumbers =
    List.filter (fun i -> i % 2 = 0) [1..10] ```

### Currying

Currying is a concept that strongly relies on higher-order functions, or more closely the property that a function can return another function.
It makes use of that property to apply arguments to a function partially.
Partial application allows to "invoke" a function with a strict subset of arguments that the function can take.
When a function is partially applied, it does not return the "final" value, but rather another function which takes as its input these arguments, that were not applied in the first place.
The above might sound cryptic, but hopefully example shown in listing {{funcurrying}} can explain this interesting feature better.

Thanks to the currying feature, functional languages gain more re-usability in context of multi-argument functions.
It is extremely easy to define new, more specific functions that partially match on the multi-argument ones, without compromising any property of the curried function.

In listing {{funcurrying}} a standard `add` function is defined (lines 1-2).
It takes two parameters (`a` and `b`), adds them together and returns the result of addition (F# compiler infers following the arguments to be of type `int`).
Then in lines 4-5 another function `add5` is defined, which partially applies on `add` function, by applying only the first argument (`x` with value 5).
In result, `add5` is an unary function which takes a single `int` parameter, as only the first argument to `add` was applied and yet one more argument to evaluate the sum is needed.
Line 7 presents how `add5` can be invoked with a single parameter.
Functions in F# are **curried by default**, hence following type signature for `add`: `int -> int -> int` means that the function can be invoked both with all arguments (`add 2 2`) and with a single argument (`add 5`).
While the first case evaluates to an ordinary `int` value, invocation in the latter case returns another function of type `int -> int`.
F# allows also for defining functions, which take multiple arguments in form of a tuple - such functions are not curried, since creation of a tuple requires all arguments to be given (example of such function is analogous `addNoCurry` in listing {{funcurrying}}).
The concept is more accurately illustrated in {{{petricek2009real}}}.

```xxx
{FSharp]{Currying}{funcurrying}
let add x y =
    x + y

let add5 =
    add 5

add5 8 // evaluates to 13

addNoCurry (x, y) =
    x + y

addNoCurry (5, 8) // must specify all arguments```

### Recursion

Recursion is a computational approach that is known in imperative world, but employed much more frequently in FP.
It relies on solving a given problem by decomposing it and retrying on a smaller instance (in context of a function or method it is typically achieved by calling itself with arguments that are smaller in some way).
A recursive function or method must hold the stop property, meaning that there must exist an instance of a problem, for which the function or method is not going to make a recursive call yet again.
Exception from the above is a recursive function that does not hold the stop property in order to produce an infinite (lazy) chain of values.
Such logically infinite chain often proves useful in context of lazy evaluation which is further depicted and illustrated in listing {{funlazy}}.
Recursion technique is an alternate approach to iteration.
Indeed, some programming languages' compilers, for optimization reasons, translate recursive functions into its iterative equivalent.
According to the Church-Turing thesis:

> *"Every effectively calculable function (effectively decidable predicate) is general recursive."*

This means that for each recursive function there exists a transformation to its corresponding iterative algorithm and the same applies backwards.
Why bother with recursion at all if in majority of cases an iterative model is more efficient with regards to time complexity and memory consumption?
Because iteration comes from imperative approach and can harm referential transparency as well as immutability, while recursion fits perfectly into functional world.
Thanks to complex compilers' code optimizations and techniques such as "tail recursion" a recursive algorithm can be both efficient and easy to comprehend.

In most programming environments, a function invocation is associated with allocating all arguments as well as return address on the stack.
If a function is invoked recursively, continuous allocations could result in stack overflow.
Tail recursion prevents from stack overflow when invoking recursive calls by avoiding unnecessary stack frame allocation.
This is achievable when the recursively-invoked function is the last invoked operation and does not need to return back to the caller {{{petricek2009real}}}.
F# does support tail recursion mechanism, however the last-operation precaution has to be made for the mechanism to be used by compiler.
For the sake of simplicity of presented examples of recursive functions, none of them is implemented in tail-recursive manner.

Listing {{funrecursion}} presents a recursive `product` function, which computes the product for a given list of numbers.
Based on the length of `elements` list (with help of pattern matching), the `product` function either returns neutral element for multiplication (`1`) or invokes itself recursively with the tail of the list.
Pattern matching case in line 4 splits the non-empty list into `head` - first element of the list, and `tail` - the rest of the elements in list.
As stated above, `product` function defined in listing {{funrecursion}} is not tail-recursive, however with a bit of effort (for example by using technique called accumulator) it could become so.

```xxx
{FSharp]{Recursion}{funrecursion}
let rec product elements =
    match elements with 
    | [] -> 1
    | head::tail -> head * product tail```

### Parametric polymorphism

Parametric polymorphism is a very important concept in FP.
It enables to treat functions as general-purpose components that can operate on arbitrary type of data.
First description of parametric polymorphism was made by Strachey in {{{strachey2000fundamental}}}, where the author also described related concept of ad hoc polymorphism.
In contrast to parametric polymorphism, ad hoc polymorphism relies on application of a function to a restricted set of types, such as addition `(+)` operation can be applied to integers or real numbers.
Another example of ad hoc polymorphism is, ubiquitous in OOP world, overloading of members which allows to invoke a certain member on a number of applicable types.
In the further course of the thesis `generic` term is also used to describe parametric polymorphism concept.

With this kind of abstraction that parametric polymorphism provides, functions get highly reusable as opposed to if they were restricted to single type only.
Parametric polymorphism is often combined with higher-order functions, resulting in yet one higher level abstraction, example of which can be seen in listing {{funparampoly}}.

Function `map` in listing {{funparampoly}} aims to produce a list of elements from an initial list with a help of a function that is applied to each element from the original list.
It takes two arguments: function `f` of type `'a -> 'b` and `list` value of type `'a list`.
Then it pattern matches on the `list` value to distinguish case when the input list is empty and when there is at least one element.
If the list is not empty, the `f` function is applied to the first element of list (`h` for head) and `map` function is recursively invoked with the rest of elements (`t` for tail).
The values are then glued to form a list with `::` operator (the same that was used for pattern matching case).
It is said that the `map` function is polymorphic, with its type parameters `'a` and `'b` and type signature `('a -> 'b) -> 'a list -> 'b list`.

```xxx
{FSharp]{Parametric polymorphism}{funparampoly}
let rec map f list =
    match list with
    | [] -> []
    | h :: t -> f h :: map f t```

### Lazy evaluation

In a standard, imperative approach, when expression is reached by control, it gets immediately evaluated and the computed value is assigned to given symbol.
Similar behavior applies in case expression is given as a parameter to a function call - it has to be computed before the result value can be pushed to the stack and the function invoked.
Such strategy is commonly known as eager evaluation.
On the other hand, evaluation is called to be lazy when the runtime environment postpones evaluating of an expression to the last responsible moment, which is when the value is vital for computations that follow.
Lazy evaluation therefore treats expressions as first-class citizens and allows to pass them around or assign (bind) to values without actually performing its evaluation (somehow analogous to how higher-order functions allow to operate on functions).
While functional-first languages like F# use eager evaluation by default, and with help of additional constructs allow for lazy expressions, pure functional languages such as Haskell incorporate laziness into execution run-time in every single place (unless stated differently).

Lazy evaluation comes with a number of benefits.
Thanks to the fact that an expression is not evaluated at once, it is possible to define infinite (in logical sense) data structures: sequences, trees or other recursive data types.
In case such structure was attempted to be evaluated, program execution would obviously hang, however infinite data structures prove useful when combined with functions that need to access only a part of the structure (see listing {{funlazy}}).
Another very important advantage of lazy evaluation is the performance boost that is accomplished because of not evaluating unnecessary (from the program execution's point of view) expressions.
Certain space of expressions defined in a program may not have any impact on the final result, hence lazy evaluation saves on time and memory complexity by omitting irrelevant expressions.

Listing {{funlazy}} demonstrates how lazy evaluation is achieved in F#. 
In line 2, `Seq.initInfinite` function is used to produce an infinite sequence of integers specified by the given function.
The anonymous function in this case returns the same number as index (such function is often referred to as "identity" function).
The sequence is then passed to `Seq.filter` (line 3) function which behavior was explained in context of higher-order functions.
Finally, `Seq.take` function (line 4) specifies that it is interested in only the first 10 elements from the sequence.
With such pipeline defined, `numbers` value gets bound to a sequence that will eventually return 10 first numbers that are not divisible by 3, starting with `0` and ending with `14`.
Thanks to the sequence being evaluated in lazy manner, the actual evaluation happens in line 6, when the sequence is asked to be translated into a finite list.

```xxx
{FSharp]{Lazy evaluation}{funlazy}
let numbers =
    Seq.initInfinite (fun i -> i)
    |> Seq.filter (fun i -> i % 3 <> 0)
    |> Seq.take 10

Seq.toList numbers // evaluation occurs here```

### Type inference

Type systems in some programming languages require manifesting type signatures in order to denote type of variables and methods.
Type inference mechanism releases the programmer from that duty, by making the compiler / interpreter automatically compute the signatures.
Often, the most general type is used to sign a symbol (under condition it is applicable in corresponding case), and such technique is called automatic generalization.
Hindley-Milner algorithm, which bases on the lambda calculus theorem to infer type signatures of symbols in programs, is the foundation for majority of type inference algorithms used in today's programming languages {{{aiken1993type}}}.
Type inference is specially powerful when combined with language feature of static typing, as it guarantees type safety even without need of type-annotating values or functions.

Listing {{funtypeinfer}} shows example of two identical functions `length` and `lengthAnnotated` of which the first one utilizes type inference.
The only difference between these two functions are lines 2 and 8 where in the latter example, both `list` argument and return type of the function are annotated.
It is worth noting that in both cases, type signature for the functions is `'a list -> int`, what emphasizes that automatic generalization was indeed applied by the F# compiler to the `length` function and thus made it generic with type parameter of `'a`.

```xxx
{FSharp]{Type inference}{funtypeinfer}
// 'a list -> int
let rec length list =
    match list with
    | [] -> 0
    | head :: tail -> 1 + length tail

// 'a list -> int
let rec lengthAnnotated (list: 'a list) : int =
    match list with
    | [] -> 0
    | head :: tail -> 1 + length tail```

Paradigms and code quality
--------------------------

Quality of software is a broad topic, that has been studied by many for a long time.
To understand why the topic is important, it is enough to say that code quality has a significant impact on both development process and resulting software product with regards to the software's stability, maintainability and reliance.
This section will target a matter of what is the relation between a programming paradigm and code quality of program implementation that utilize the paradigm, from point of view of aspects such as number of defects (bugs), dependencies complexity or separation of concerns.

### Observations

Studies on how a programming language class can relate to a potential number of software defects show interesting outcomes.
In their work, Ray et al. {{{ray2014large}}} measure (by empirical study, using mixed-methods approach) number of defects in open-source software and group the results by programming language classes (paradigms).
One of the conclusions is that using FP languages leads to less software defects in comparison to procedural (imperative) languages:

>> *"There is a small but significant relationship between language class and defects. Functional languages have a smaller relationship to defects than either procedural or scripting languages."*

Apart from describing relation with general programming paradigm taken under consideration, the paper {{{ray2014large}}} also focuses on connection of number of defects to lower level language properties, such as memory management, static typing or concurrency friendliness:

>> *"Defect types are strongly associated with languages; Some defect type like memory error, concurrency errors also depend on language primitives. Language matters more for specific categories than it does for defects overall."*

A language that combines FP paradigm with the properties of static typing, memory management and concurrency support can be therefore considered safer than competitive languages with regards to expected number of software defects. 
Among languages that fit into this category there is Haskell, F#, Scala and OCaml, all of which are further discussed in section {{FUNCTIONAL PROGRAMMING IN INDUSTRY}}.
It must be however emphasized that besides holding described properties, there are many other, obvious factors that affect the software quality, like engineers' skills, productivity or work organization.
What is more, additional language features usually come with extra cost in respect to performance and memory consumption.
That is why a language that meets above criteria cannot be treated as a "silver-bullet" for solving every possible problem in the universe.

Primarily appearing functional language in the thesis is F#.
As F# is built on top of the .NET platform, where the dominating language is (coming from the OOP family) C#, the thesis makes a number of comparisons between these two.
Such comparisons have also been made by the F# community, from which a significant number of members do have solid experience with C# language.
Partially thanks to to the succinct language syntax, but also thanks to its functional nature, F# is usually considered easier to comprehend and more maintainable than C#.
One of the observation concerns modules' dependencies complexity in both languages {{{eve2014networks}}}:

>> *"C# projects tend to be larger, with more classes and dependencies. They also have longer chains of dependencies on average. Real world F# projects are smaller with cleaner modularity."*

Another observation corresponds to size of code, which turns out to be much smaller when using F# in comparison to C#.
Conducted experiment involved rewriting an existing C# application, responsible for evaluating revenue from electricity supply contracts in United Kingdom, with size of 350 KLOC (thousands of lines of code) into identical with regards to functionality F# counterpart.
Concise syntax of F# allowed to eliminate a lot of "noise" lines of code, such as braces or blank lines that are very common in C# language.
Not only the syntax of F#, but also functional properties like composability and abstraction led to interesting results.
One of the conclusions drawn from the experiment highlighted that **total** amount of application code in F# version reached less than 22 KLOC, comparing to the 29 KLOC of only **blank** lines in C# version {{{cousins2014difference}}}:

>> *"I can fit the whole F# solution in the blank lines of the C# solution with 7,000 lines to spare."*

Above citations prove that functional movement can be the cure for maintaining complex and enormous code-bases.
Such code-bases when implemented in imperative fashion, lacking of referential transparency property and composable abstractions, can become tough to reason about and thus more expensive and error-prone.

### Example

Listing {{csparadigmimperative}} demonstrates how flow of a strictly imperative program usually looks like.
Language used in listing {{csparadigmimperative}} is C#.
C# was born with OOP paradigm in mind, that is why it allows to write in imperative style, and the below code can feel idiomatic for C#.
The code reads a log file and extracts 10 first error entries from the file (lines that start with \[ERROR\] prefix).
Original idea comes from a tweet {{{imperativevsfunctional}}} where Java language was used.
Here, the program was adjusted to C# syntax.

```xxx
{CSharp]{Example of imperative approach in CSharp}{csparadigmimperative}
var errors = new List<String>();
var errorCount = 0;
using (var reader = File.OpenText("log"))
{
    var line = reader.ReadLine();
    while (errorCount < 10 && line != null)
    {
        if (line.StartsWith("[ERROR]"))
        {
            errors.Add(line);
            errorCount++;
        }
        line = reader.ReadLine();
    }
}```

It is evident that the implementation in listing {{csparadigmimperative}} consists of a step-by-step instructions.
Code contains assignment statements (lines 1, 2, 3, 5, 13), jump instructions (line 6, 8) and explicit arithmetic operations (line 11).
It focuses on the detailed algorithm flow, hence the engineer specifies **how** to achieve the goal.
In contrast, listing {{csparadigmfunctional}} demonstrates the very same program with a different approach, where without providing a detailed recipe, but rather using built-in language constructs, the engineer declares **what** is the goal.

Another interesting thing to note in listing {{csparadigmimperative}} is separation of concerns (emphasized in the referenced tweet {{{imperativevsfunctional}}}).
A few concerns are targeted:

* **Reading from a file**:
    * file has to opened to be read from (line 3), 
    * single line is read inside `while` loop (lines 5 and 13),
    * end of file is being checked by comparing `line` to `null` (line 6),
* **Filtering lines**:
    * only lines starting with "\[ERROR\]" are taken into account (line 8),
* **Counting errors**:
    * counter is initialized in line 2,
    * it is then compared to the maximum number of lines in line 6,
    * finally it has to be incremented every time an error is added to the list (line 11),
* **Collecting error lines**:
    * list of errors is initialized in line 1,
    * every matching error entry is appended to that list (line 10).

Example of imperative approach presented in listing {{csparadigmimperative}} shows that different concerns intersect between different lines, which leads to tight coupling of code.
As result of tight coupling, particular changes in such implementation might have **unwanted impact** on the rest of algorithm (for example, if one would have to skip first 3 error entries and only then read 10 error entries from the file file, the rest of algorithm could get easily spoiled).

On the other hand, listing {{csparadigmfunctional}} demonstrates the same problem solved with C#, but using functional (declarative) techniques (despite it originated as a standard, OOP language, a few functional techniques got introduced into C# to make the language more powerful).
C# has been chosen for both examples deliberately, to emphasize that programming language is just a tool, and that it is programming style and paradigm used that really matter when considering code quality. 

```xxx
{CSharp]{Example of functional approach in CSharp}{csparadigmfunctional}
var errors = 
    File.ReadLines("log")
        .Where(line => line.StartsWith("[ERROR]"))
        .Take(10)
        .ToList();```

At first glance, it is obvious that the implementation in listing {{csparadigmfunctional}} is much more concise with only 5 lines compared to 15 in listing {{csparadigmimperative}} (technically these 5 lines build up a single expression which could be written in a single line, but were split to improve readability). The code in listing {{csparadigmfunctional}} follows a functional pattern in respect to that every function invocation is treated as an expression which takes some input and returns some output:

* `File.ReadLines` in line 2 takes path to file (of type `string`) and returns a sequence of lines (`seq<string>`). The sequence is evaluated lazily, which prevents loading whole file into memory;
* `Where` (line 3) is a function (speaking strictly C# jargon, it is a LINQ extension method) which takes a predicate function as its argument (`Where` is an example of higher-order function, equivalent to `Seq.filter` in F#) and returns sequence with items that match the predicate;
* the predicate function in form of anonymous function (lambda expression) passed to the `Where` function is of generic type `'T -> bool`, where `'T` type parameter has been expanded and inferred by C# type system to be of type `string`;
* `Take` function (line 4) instructs to limit number of items in the sequence to the specified value (all items are returned if the value is larger than number of items);
* `ToList` function (line 5) collects the items from the lazy sequence into a `List` type. This function forces the sequence to be evaluated;
* `errors` variable is being assigned the result of expression.

The same concerns apply as in the previous example, however they are separated in much clearer fashion:

* **Reading form a file** is addressed in line 2,
* **Filtering lines** is addressed in line 3,
* **Counting errors** is addressed in line 4.
* **Collecting error lines** is addressed in line 5.

Thanks to such straightforward separation of concerns, applying changes to existing implementation comes with much lower cost.
As an example it can be imagined that for some reason requirement changes and first three "ERROR" entries from the file should be skipped.
Listing {{csparadigmfunctionalskip}} demonstrates necessary code modification applied to meet that new requirement.
In this case, it was enough to add `Skip` function with proper argument to the pipeline.
The same in imperative approach would require another loop with new counter where lines from the file would have to be read, filtered again and then ignored, which sounds more complicated than the functional equivalent.

```xxx
{CSharp]{Modifications in functional approach in CSharp}{csparadigmfunctionalskip}
var errors = 
    File.ReadLines("log")
        .Where(line => line.StartsWith("[ERROR]"))
        .Skip(3) // new logic applied
        .Take(10)
        .ToList();```