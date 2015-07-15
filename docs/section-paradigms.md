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
It is based on implicit modification of program state in order to achieve the desired goal.
Main actions performed in imperative paradigm are updating statements, jumps and intermediate input / output, which spoil referential transparency by introducing the possibility of "side effects" or transfers of control during expression evaluations {{{tennent1976denotational}}}.

#### History

First programming languages were machine languages, which offered Application Programming Interface (API) consisting of registers manipulation, jump instructions, basic arithmetic and logic operators.
The machine languages followed thus imperative paradigm, which was the only known approach back then.
Building complex systems with machine code usually resulted in enormous code bases, which turned out to be painful to maintain.
That is why higher level languages were introduced, such as FORTRAN, ALGOL, COBOL and BASIC, all of which can also be considered imperative.
They aimed to reduce the cost of code maintainability in comparison to machine languages by abstracting set of machine code instructions with more human-readable statements.

In 1970s, languages like Pascal or C were created.

#### Example

Declarative
-----------

Procedural
----------

Object-Oriented
---------------

Functional
----------

Impact of paradigm choice
-------------------------