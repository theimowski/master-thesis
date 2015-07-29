PROGRAMING PARADIGMS
====================

There is a lot of programming languages in use nowadays.
TIOBE Index web site, which examines popularity of programming languages, keeps track of more than 150 programming languages {{{tiobeindex}}}, while the total number of all *notable* programming languages exceeds a thousand {{{wikilistpl}}}.

The most common classification of programming languages relies on programming paradigms. 
Paradigm determines some kind of abstract pattern that is followed by a family of languages.
One of the first attempts to describe methods of organizing code was made by Dijkstra, who gave it a name of Structured Programming {{{dijkstra1970notes}}}.
In 1978, Floyd received Turing Award for his paper, where he described the notion of a **paradigm** and how it can influence language designers {{{floyd1979paradigms}}}.
Those early works prove that the concept and its impact on programming has been studied since the beginning of software industry.

In this section most popular programming paradigms are surveyed.
The thesis focuses on a clear separation between imperative and declarative paradigms, and describes their derivatives.
Since there exist plenty of different taxonomies for programming paradigms, the presented categorization should not be treated as the only appropriate one.
It must be emphasized that most programming languages usually span over more than one paradigm, which makes them labeled with multi-paradigm notion.
The paradigms are not thus mutually exclusive in context of a specific language.

Imperative
----------

Imperative word looked up in a dictionary means "giving a command".
That definition summarizes what really imperative programming is all about.
When writing software in imperative manner, the programmer gives commands to a computer and tells it what to do step by step.
Imperative is based on explicit modification of program state in order to achieve a desired goal.

### History

First programming languages were machine languages, which offered Application Programming Interface (API) consisting of registers manipulation, jump instructions, basic arithmetic and logic operators.
Because of their nature, which was based on giving low level commands to the processor, machine languages became the precursors of imperative programming.
Building complex systems with machine code usually resulted in enormous code bases, which turned out to be painful to maintain.

That is why in the late 50s and in the beginning of 60s one level higher languages were introduced, such as FORTRAN, COBOL and BASIC, all of which can be considered imperative.
They aimed to reduce the cost of code maintainability in comparison to machine languages by abstracting set of machine code instructions with more human-readable statements.

In 70s, languages like Pascal or C were created.
While Pascal served mainly for educational purposes, C language quickly became a mainstream programming language and at the time of the writing still remains one of the most popular in ranks {{{tiobeindex}}}.
With its distinctive syntax, C originated a whole family of languages, commonly known as "C-based syntax" languages.
Such constructs as `for` and `while` loops with their syntax proposed by C, are the most basic building blocks for an imperative program in majority of contemporary languages.

Together with the 80s came a growing interest in object-oriented approach, which also has it roots in imperative approach.
From the imperative category, Smalltalk and C++ were invented, the latter being "extension" to the C language with object concept among its new features.

The imperative approach evolved since then, with languages like Python, Visual Basic, PHP, Java, Ruby developed in the 90s and C# after the Millennium.
Despite some of these languages aimed to target multiple paradigms, the concept of building program's flow step by step, with the help of commands, remained ubiquitous.

### Object-Oriented

Majority of imperative programming languages that were developed till the 80s have procedural nature.
This means that they focus on defining reusable procedures which invoke certain actions.
Procedures are stateless, and as a result they cannot carry any kind of data with them.
Object-Oriented approach was introduced in order to combine data with behavior inside an object, in order to enable relations between object, its internal state and applicable operations on that object.
Object-Oriented follows imperative approach with regards to how behavior of an object is defined.
The paradigm outlines a few principles for design of a program.

**Objects** are the basic building blocks of the program.
In some Object-Oriented languages, every data structure (including primitive types) is an object.
Instance of an object is usually shaped by the concept of class, which defines available members - fields and methods.
Fields persist state of an object that methods can operate on.

**Encapsulation** is an interesting principle of Object-Oriented software, that often gets misunderstood.
It aims to hide information, that is to make the internals of an object invisible to the outside scope, but to provide a public interface that does not leak unnecessary data for other components interacting with that object.
Encapsulation is fulfilled when the call site neither has to make any assumptions about nor be aware of existence of the internals of called object.
The misconception occurs because developers treat encapsulation as a way of sharing internal state through trivial methods (known as getters and setters in Java and properties in C#).

**Inheritance** allows a class to derive behavior as well as state from another class.
The derived members can then be reused to implement more specific sub-class methods.
Thanks to the mechanism, some implementation can be shared between multiple classes of objects and duplication of code avoided.
Often languages allow for shadowing of derived members, that is overriding definition of a member, which can result in spoiled abstraction.
Inheritance also leads to forming of class hierarchies which easily get big and complex.

**Polymorphism** is a concept known from functional programming, that in Object-Oriented approach relies on inheritance.
"Poly" standing for "many" and "morphism" which can be thought of "form" describe the ability to treat objects of different classes in unified way.
In Object-Oriented paradigm however this ability is forced by belonging to a specific class hierarchy.

**Abstraction** is closely related to polymorphism, and allows to treat referenced objects as if they were of the most possible general type, while in runtime they appear to be an instance of a very specific class.
Abstraction enables better testability of code, and late binding which can be used for inversion of control (IOC).
As stated above, when not cautious, abstraction can be easily spoiled in Object-Oriented programming by overriding behavior or leaking internals of an object.

Many other principles and design patterns apply to Object-Oriented programming.
With the SOLID acronym, Bob C. Martin {{{martin2003agile}}} presents set of five principles that ensure software to be maintainable.
Multiple design patterns described in the famous Design Patterns book {{{gamma1994design}}} demonstrate common approaches that address cross cutting concerns in Object-Oriented program.
While it is certainly a good thing that such patterns are established for software engineers to follow, majority of them tend to be quite cumbersome and utilizing them cause complexity of software to grow exponentially.
Many of the standardized patterns seem to **solve problems that the Object-Oriented programming caused** itself.

### Referential opacity

Both imperative and object-oriented paradigms suffer from referential opacity. 
Referential opacity is an opposite of referential transparency property.
Tennent in his work explains {{{tennent1976denotational}}}: 

>> *"Main actions performed in imperative paradigm are updating statements, jumps and intermediate input / output, which spoil referential transparency by introducing the possibility of "side effects" or transfers of control during expression evaluations."*

Referential transparency has already been defined a few times {{{sondergaard1990referential, strachey2000fundamental}}}, where it concerns more complex topics such as non-determinism or definiteness.
In context of this thesis and basic paradigms' characteristics it is enough to say that referential transparency describes a property of programming language, which enables to substitute certain expression with another of the same value, without impact on the program's behavior.
The downsides of imperative program actions mentioned by Tennent {{{tennent1976denotational}}} with regards to referential transparency can be explained as following:

* **side effects** - if an expression evaluates to a certain value, but it has side effects (such as input / output operations), then it cannot be replaced with another expression without changing behavior of the program, even if the latter expression evaluates to the same value,
* **transfers of control** - if an expression consists of more than one exit points, and the exit point of expression is determined by condition branching (jump statements), then the value of such expression is ambiguous because control can be transferred from arbitrary point of expression to the outside scope.

In programming practice, these drawbacks lead to difficulties with understanding program's flow and behavior, which in turn imply spending more time on code debugging.

Declarative
-----------

Declarative paradigm is an alternative approach (different than imperative) with regards to how logic is specified.
Program written in declarative style focuses on **what** goal has to be achieved rather than **how** this goal is to be achieved {{{petricek2009real}}}.
This means that code does not specify how detailed, low level instructions are to be executed in order, but rather synthesizes a set of constructs that are capable of being combined together.
It does so by **declaring** how these constructs relate to themselves, hence the name of the paradigm.
Padawitz {{{padawitz2006deductive}}} describes the term declarative as a combination of functional (or applicative) and relational (or logic) programming.

### History?

Functional
----------

Functional programming is strongly connected with declarative paradigm, and has its roots in mathematics.
Among concepts from mathematics which are the foundations for functional paradigm, there are:

* **Lambda calculus** - formal mathematical system which became the base of functional paradigm,
* **Functions** - treated as first-class citizens in functional programming; their purity as seen by mathematicians allows to use powerful programming techniques,
* **Category theory** - this very abstract field of mathematics is reflected in type system of statically typed functional languages.

In this section focus is laid on pointing out functional programming properties (or features), which differentiate the paradigm from other.
Each property comes with a brief description followed by a listing demonstrating the property and its usage in practice.
All example listings are in F# language.

### Immutability

Immutability is one of the most basic properties of functional programming.
In imperative paradigm, assignments are made to variables which, as the name suggests, can vary (be mutated) during run-time.
In functional world on the other hand, it is said that there is a binding of a value to a symbol. 
Immutability enforces that all values which are evaluated during program execution, once bound to a symbol, cannot be mutated.

For newcomers the property might initially look restrictive, but it turns out that it does not imply any constraints.
Immutability helps with understanding the flow of program logic, and is a prerequisite of referential transparency when it comes to deferred execution, because if a deferred function refers to a variable that may or may not be modified, the result of evaluating such function is indeterministic.
The greatest benefit from immutability is discovered in context of concurrent programming, because immutable values are thread-safe by default (they cannot change).

Listing {{funimmutability}} presents how immutability in F#, with the `let` keyword meaning binding of a value on the right side of `=` operator to the symbol on left side.
Both `x` and `y` values in listing {{funimmutability}} are immutable, and attempt to assign different value to `x` symbol (line 3) fails.
F# compiler treats the expression in line 3 as equality test, and since `x` is not equal to `6`, the expression evaluates to false.

```xxx
{FSharp]{Immutability}{funimmutability}
let x = 5      // value x is immutable
let y = x + 3  // value y is immutable
x = 6          // this expression evaluates to false```

### Purity

Purity property allows to associate programming language's functions with mathematical functions.
A function is pure in mathematical sense, when for a given set of arguments it always returns the same value.
In context of programming languages, one can say that a pure function does not depend on anything but the arguments it takes.

Again, as was the case with immutability, the purity properties might seem impossible to achieve in a real world application.
Every software needs to communicate with components outside its process, for example by invoking IO operations or accessing computer's clock.
Haskell which happens to be purely functional is used in real systems and in the meanwhile preserves purity property.
This is achievable with technique called "Monad" {{{mcbride2008applicative}}}, which also has its formal mathematical definition. 
Monads however are quite complicated topic itself, therefore are not explicitly (they are used in the research part anyway) addressed by this thesis.

Listing {{funpurity}} demonstrates two functions, `pureSalary` and `impureSalary`.
Both functions have the same type signature (`decimal -> decimal -> decimal`), which means that they take two `decimal` arguments: `hours` and `rate`, and return `decimal` salary computed for a work day.
Despite they have the same type signature, the functions do differ with regards to purity.
While `pureSalary` does not depend on any value from outside and always returns the same result for given arguments, the `impureSalary` takes an implicit dependency on `DateTime.Now.DayOfWeek` property, which depends on the current (at the time of executing) day of week.
As F# is not purely functional, it cannot enforce pure nature of functions, and thus the `impureSalary` compiles correctly.

```xxx
{FSharp]{Purity}{funpurity}
let pureSalary (hours: decimal) (rate: decimal) =
    hours * rate

let impureSalary (hours: decimal) (rate: decimal) =
    if DateTime.Now.DayOfWeek = DayOfWeek.Saturday then
        hours * rate * 1.5M
    else
        hours * rate```

### Higher-order functions

Functions in functional programming languages are often referred to as "first-class citizens".
This usually means that functions are treated just as any other ordinary value and are allowed to be combined together.
A function is called higher-order when it takes as a parameter or returns another function.
The concept of function pointers introduced in C and C++ is closely related to higher-order functions, though it does not guarantee type safety like in statically typed functional languages.

Higher-order functions is a powerful feature that enables to abstract away a common piece of logic and make it more reusable.
It addresses the problem, present in many imperative languages, of code repetition for recurrent tasks such as working with collections (for example filtering or mapping) or null-checking.
In addition to that, higher-order functions can be used as a foundation for concept known in Object-Oriented programming as "Dependency Injection".
Dependency Injection in Object-Oriented paradigm relies on passing abstract interfaces without implementation to consuming classes.
The interfaces passed to dependent class contain only type signatures of methods to be invoked, hence such interface can be easily represented with traditional functions.

Listing {{funhof}} shows example of using higher-order function `Array.Filter`.
It takes two parameters: a function of type `'T -> bool` and an array of type `'T array`.
The array passed as the second argument contains numbers from 1 to 10, and the predicate function passed as the first argument is an anonymous function (lambda expression) which tests whether a number is even or not.
Thanks to the `Array.filter` function being higher-order, one can imagine whole spectrum of predicate functions that could be applied to filter specific elements from the array, and since the function is generic in type `'T` it can accept arrays of any type.

```xxx
{FSharp]{Higher-order functions}{funhof}
let evenNumbers =
    Array.filter (fun i -> i % 2 = 0) [|1..10|] ```

### Currying

Currying is a concept that strongly relies on higher-order functions, or more closely the property that a function can return another function.
It makes use of that property to apply arguments to a function partially.
Partial application allows to "invoke" a function with only a strict subset of arguments that the function can take.
When a function is partially applied, it does not return the "final" value, but rather another function which takes as its input these arguments, that were not applied to the original one.
The above might sound cryptic, but hopefully example shown in listing {{funcurrying}} can explain this interesting feature better.

Thanks to the currying feature, functional languages gain more re-usability in context of multi-argument functions.
It is extremely easy to define new, more granular functions that partially match on the multi-argument ones, without losing any initial property.

In listing {{funcurrying}} a standard `add` function is defined (lines 1-2).
It takes two parameters (`a` and `b`), adds them together and returns the result of addition (F# compiler infers following type signature: `int -> int -> int`).
Then in lines 4-5 another function `add5` is defined, which partially applies on `add` function, by applying only the first argument (`x` with value 5).
In result, `add5` gets inferred by the compiler to be of type `int -> int`, because it applied only the first argument to `add` and now needs yet another argument to evaluate the sum.
Line 7 presents how `add5` can be invoked with a single parameter.

```xxx
{FSharp]{Currying}{funcurrying}
let add x y =
    x + y

let add5 =
    add 5

add5 8 // evaluates to 13```

### Recursion

Recursion is a computational approach that is known in imperative world, but employed much more frequently in functional programming.
It relies on solving a given problem by decomposing it and retrying on a smaller instance (in context of a function or method it is typically achieved by calling itself with arguments that are smaller in some way).
A recursive function or method must hold the stop property, meaning that there must exist an instance of a problem, for which the function or method is not going to make a recursive call yet again.
Recursion technique is an alternate approach to iteration.
In fact, some programming languages' compilers, for optimization reasons, translate recursive functions into its iterative equivalent.

According to the Church-Turing thesis:

> *"Every effectively calculable function (effectively decidable predicate) is general recursive."*

This means that for each recursive function there exists a transformation to its corresponding iterative algorithm and the same applies backwards.
Why bother with recursion at all if in majority of cases an iterative model is more efficient with regards to time complexity and memory consumption?
Because iteration comes from imperative approach and can harm referential transparency as well as immutability, while recursion fits perfectly for functional world.
Thanks to complex compilers' code optimizations and techniques such as "Tail recursive calls" (which prevents from stack overflows by avoiding stack frame allocation for recursive calls) a recursive algorithm can be both efficient and easier to comprehend.

Listing {{funrecursion}} presents a recursive `product` function, which computes the product for a given list of numbers.
Based on the length of `elements` list (with help of pattern matching), the `product` function either returns neutral element for multiplication (1) or invokes itself recursively with the tail of the list.
Pattern matching case in line 4 splits the non-empty list into head (`h`) - first element of the list, and tail (`t`) - the rest of the elements in list (tail can potentially be empty).
It is worth noting that the `product` function as defined in listing {{funrecursion}} is not tail-recursive, however with a bit of effort (for example by using technique called accumulator) it could become so.

```xxx
{FSharp]{Recursion}{funrecursion}
let rec product elements =
    match elements with 
    | [] -> 1
    | (h::t) -> h * product t```

### Lazy evaluation

```xxx
{FSharp]{Lazy evaluation}{funlazy}
let numbers =
    Seq.initInfinite (fun i -> i)
    |> Seq.filter (fun i -> i % 3 <> 0)
    |> Seq.take 10

Seq.toList numbers // evaluation occurs here```


Comparison by example
---------------------

Listing {{csparadigmimperative}} demonstrates how flow of a strictly imperative program usually looks like.
Language used in listing {{csparadigmimperative}} is C#, which allows to write in such manner.
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
In contract, listing {{csparadigmfunctional}} demonstrates the very same program with a different approach, where without providing a detailed recipe, but rather using built-in language constructs, the engineer declares **what** is the goal.

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
As result of tight coupling, particular changes in such implementation might have **unwanted impact** on the rest of algorithm (for example, if instead of first 10 lines, one would have to read 10 last lines of file, the whole algorithm would have to be redesigned).

On the other hand, listing {{csparadigmfunctional}} demonstrates the same problem solved with C#, but using functional (declarative) techniques.

```xxx
{CSharp]{Example of functional approach in CSharp}{csparadigmfunctional}
var errors = 
    File.ReadLines("log")
        .Where(line => line.StartsWith("[ERROR]"))
        .Take(10)
        .ToList();```

At first glance, it is obvious that the implementation in listing {{csparadigmfunctional}} is much more concise with only 5 lines compared to 15 in listing {{csparadigmimperative}} (technically these 5 lines build a single expression which could be written in a single line, but were split to improve readability).

The code in listing {{csparadigmfunctional}} follows a functional pattern with respect to that every function invocation is treated as an expression which takes some input and returns some output:

* `File.ReadLines` in line 2 takes path to file (of type `string`) and returns a sequence of lines (`seq<string`). The sequence is evaluated lazily, which prevents loading whole file into memory;
* `Where` (line 3) is a function (speaking strictly C# jargon, it is a LINQ extension method) which takes a predicate function as its argument (`Where` is an example of higher-order function) and returns sequence with items that match the predicate;
* the predicate function in form of anonymous function (lambda expression) passed to the `Where` function is of generic type `'T -> bool`, where `'T` type parameter has been expanded and inferred by C# type system to be of type `string`;
* `Take` function (line 4) instructs to limit number of items in the sequence to the specified value (all items are returned if the value is larger than number of items);
* `ToList` function (line 5) collects the items from the lazy sequence into a `List` type. This function forces the sequence to be evaluated;
* the result of expression is assigned to variable `errors`.

The same concerns apply as in the previous example, however they are separated in much clearer fashion:

* **Reading form a file** is addressed in line 2,
* **Filtering lines** is addressed in line 3,
* **Counting errors** is addressed in line 4.
* **Collecting error lines** is addressed in line 5.

Thanks to such straightforward separation of concerns, applying changes to existing implementation comes much easier.
Listing {{csparadigmfunctionalreverse}} demonstrates code modification made to collect 10 last instead of 10 first lines from the file.
In this case, it was enough to add `Reverse` function to the pipeline that reverses the order of lines that are processed.

```xxx
{CSharp]{Modifications in functional approach in CSharp}{csparadigmfunctionalreverse}
var errors = 
    File.ReadLines("log")
        .Where(line => line.StartsWith("[ERROR]"))
        .Reverse() // new logic applied
        .Take(10)
        .ToList();```