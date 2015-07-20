PROGRAMING PARADIGMS
====================

There is a lot of programming languages in use nowadays.
TIOBE Index web site, which examines popularity of programming languages, keeps track of more than 150 programming languages {{{tiobeindex}}}, while the total number of all *notable* programming languages exceeds a thousand {{{wikilistpl}}}.

The most common classification of programming languages relies on programming paradigms. 
Paradigm determines some kind of abstract pattern that is followed by a family of languages.
One of the first attempts to describe methods of organizing code was made by Dijkstra, who gave it a name of Structured Programming {{{dijkstra1970notes}}}.
In 1978, Floyd received Turing Award for his paper, where he described the notion of a **paradigm** and how it can influence language designers {{{floyd1979paradigms}}}.
Those early works prove that the concept and its impact on programming has been studied since the beginning of software industry.

In this section most popular programming paradigms will be surveyed.
The thesis focuses on a clear separation between imperative and declarative and their derivatives.
Since there exist plenty of different taxonomies for programming paradigms, the presented categorization should not be treated as the only appropriate one.
It must be emphasized that most programming languages usually span over more than one paradigm, which makes them multi-paradigm.
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

That is why in the late 50s and in the beginning of 60s higher level languages were introduced, such as FORTRAN, ALGOL, COBOL and BASIC, all of which can be considered imperative.
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
It focuses on the detailed algorithm flow, that is the engineer has to specify **how** to achieve the goal.
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
As result of tight coupling, particular changes in such implementation might have **unwanted impact** on the rest of algorithm (for example, if instead of first 10 lines, one has to read 10 last lines of file, the whole code snippet has to be redesigned).

### Object-Oriented

Majority of imperative programming languages that were developed till the 80s have procedural nature.
This means that they focus on defining reusable procedures which invoke certain actions.
Procedures are stateless, and as a result they cannot carry any kind of data with them.
Object-Oriented approach was introduced in order to combine data with behavior inside an object.

### Referential opacity

As per Tennent {{{tennent1976denotational}}}, main actions performed in imperative paradigm are updating statements, jumps and intermediate input / output, which spoil referential transparency by introducing the possibility of "side effects" or transfers of control during expression evaluations.
Lack of referential transparency leads to an opposite property, namely referential opacity.

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