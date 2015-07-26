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

That is why in the late 50s and in the beginning of 60s one level higher languages were introduced, such as FORTRAN, ALGOL, COBOL and BASIC, all of which can be considered imperative.
They aimed to reduce the cost of code maintainability in comparison to machine languages by abstracting set of machine code instructions with more human-readable statements.

In 70s, languages like Pascal or C were created.
While Pascal served mainly for educational purposes, C language quickly became a mainstream programming language and at the time of the writing still remains one of the most popular in ranks {{{tiobeindex}}}.
With its distinctive syntax, C originated a whole family of languages, commonly known as "C-based syntax" languages.
Such constructs as `for` and `while` loops with their syntax proposed by C, are the most basic building blocks for an imperative program in majority of contemporary languages.

Together with the 80s came a growing interest in object-oriented approach, which also has it roots in imperative approach.
From the imperative category, Smalltalk and C++ were invented, the latter being "extension" to the C language with object concept among its new features.

The imperative approach evolved since then, with languages like Python, Visual Basic, PHP, Java, Ruby developed in the 90s and C# after the Millennium.
Despite some of these languages aimed to target multiple paradigms, the concept of building program's flow step by step, with the help of commands, remained ubiquitous.

### Example

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
* **Collecting error lines**:
    * list of errors is initialized in line 1,
    * every matching error entry is appended to that list (line 10),
* **Counting errors**:
    * counter is initialized in line 2,
    * it is then compared to the maximum number of lines in line 6,
    * finally it has to be incremented every time an error is added to the list (line 11),
* **Filtering lines**:
    * only lines starting with "\[ERROR\]" are taken into account (line 8).

Example of imperative approach presented in listing {{csparadigmimperative}} shows that different concerns intersect between different lines, which leads to tight coupling of code.
As result of tight coupling, particular changes in such implementation might have **unwanted impact** on the rest of algorithm (for example, if instead of first 10 lines, one would have to read 10 last lines of file, the whole algorithm would have to be redesigned).

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

Declarative paradigm is an opposite to imperative with regards to how code is written.
Program written in declarative style focuses on the **what** rather than **how**.
This means that it does not specify exact low level instructions to be executed in a given order, but rather synthesizes a set of other components.
It does so by declaring how the components relate to themselves, hence the name of the paradigm.
As per Padawitz {{{padawitz2006deductive}}}, the term declarative is a combination of functional (applicative) and relational (logic) programming.

### History

### Example

Placeholder text

```xxx
{CSharp]{Example of functional approach in CSharp}{csparadigmfunctional}
var errors = 
    File.ReadLines("log")
        .Where(line => line.StartsWith("[ERROR]"))
        .Take(10)
        .ToList();```

### Functional

Impact of paradigm choice
-------------------------