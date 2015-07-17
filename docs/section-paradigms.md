PROGRAMING PARADIGMS
====================

There is a lot of programming languages in use nowadays.
TIOBE Index web site, which examines popularity of programming languages, keeps track of more than 150 programming languages {{{tiobeindex}}}, while the total number of all *notable* programming languages exceeds a thousand {{{wikilistpl}}}.

The most common classification of programming languages relies on programming paradigms. 
Paradigm determines some kind of abstract pattern that is followed by a family of languages.
The concept was first known as Structured Programming {{{dijkstra1970notes}}}.
In 1978, Floyd received Turing Award for his paper, where he described the notion of paradigm and how it can influence language designers {{{floyd1979paradigms}}}.
Those early works on paradigms of programming prove that the concept has been studied since the beginning of software industry.

In this section focus will be laid on surveying most popular programming paradigms.
It must be emphasized that a single programming language usually spans over a few paradigms, benefiting from one and another.
The paradigms are not thus mutually exclusive.

Imperative
----------

Imperative word looked up in a dictionary means "giving a command".
That definition summarizes what really imperative programming is all about.
When writing software in imperative paradigm, one gives commands to the computer and tells it what to do.
It is based on explicit modification of program state in order to achieve a desired goal.
Main actions performed in imperative paradigm are updating statements, jumps and intermediate input / output, which spoil referential transparency by introducing the possibility of "side effects" or transfers of control during expression evaluations {{{tennent1976denotational}}}.

#### History

First programming languages were machine languages, which offered Application Programming Interface (API) consisting of registers manipulation, jump instructions, basic arithmetic and logic operators.
Machine languages are the precursors of imperative programming, despite the fact that the concept of paradigms was not yet born back then.
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

#### Example

Listing {{csparadigmimperative}} demonstrates how flow of a strictly imperative program usually looks like.
Language used in listing {{csparadigmimperative}} is C#, which allows to write in such manner.
Code was taken from a tweet {{{imperativevsfunctional}}} and adjusted for C# language syntax.

```xxx
{C]{Example of imperative approach in CSharp}{csparadigmimperative}
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

Problem, which listing {{csparadigmimperative}} is trying to solve can be defined following:

> Given a log file, extract 10 first error entries (lines that start with \[ERROR\] prefix).

It is evident that the implementation consists of a step-by-step instructions.
Code focuses on **how** to achieve the goal rather than **what** is the goal.
In contract, listing {{csparadigmfunctional}} demonstrates how the very same program could be written without providing a detailed recipe.

Another interesting thing to note in listing {{csparadigmimperative}} is separation of concerns (emphasized in the referenced tweet {{{imperativevsfunctional}}}):

* **Reading from a file** interlaces in lines 3,5,6 (null check),13
* **Collecting error lines** in lines 1,10
* **Counting errors** in lines 2,6,11
* **Filtering results** in line 8

Declarative
-----------

Declarative paradigm is an opposite to imperative with regards to how code is written.
Program written in declarative style focuses on the **what** rather than **how**.
This means that it does not specify exact low level instructions to be executed in a given order, but rather synthesizes a set of other components.
It does so by declaring how the components relate to themselves, hence the name of the paradigm.
As per Padawitz {{{padawitz2006deductive}}}, the term declarative is a combination of functional (applicative) and relational (logic) programming.

#### History



Procedural
----------

Object-Oriented
---------------

Functional
----------

#### History

#### Example

Placeholder text

```xxx
{C]{Example of functional approach in CSharp}{csparadigmfunctional}
var errors = 
    File.ReadLines("log")
        .Where(line => line.StartsWith("[ERROR]"))
        .Take(10)
        .ToList();```

Impact of paradigm choice
-------------------------