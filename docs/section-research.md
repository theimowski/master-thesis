RESEARCH
========

Previous section described how Functional Programming managed to be successfully applied in many projects.
In this section focus will be laid on creating software that uses functional techniques.
For that purpose, process of developing such application will be shown.
In the course of this section, multiple comparisons will be made between Object-Oriented and Functional approaches.
All examples will be shown in F# programming language.

Domain choice
-------------

Functional Programming is already present in a significant amount of areas. 
There are however fields, where it is still not widely adopted. One of such fields is **Web Development**. 
Here, Object-Oriented programming seems to be a strong leader with regards to number of existing frameworks, libraries and tools as well as popularity among software engineers. 
In order to verify whether Functional Programming could potentially be applied to software from arbitrary domain, decision has been made to discover how it would fit this relatively foreign area - Web Development.

Another reason why Web Development domain has been chosen was a will to contribute to the F# community.
As the F# language is getting more and more interest, the language community is doing its best to encourage developers to give F# a try.
On one of mailing groups there appeared a suggestion to prepare a tutorial on how to create Web applications with F#.
This turned out to be a great candidate for research part of this thesis.

Functional Web
--------------

Even though Web Development is usually associated with using imperative techniques, it's not completely uncommon to follow functional principles while creating Internet applications.
Majority of web applications are built on top of the HTTP protocol.
From a software engineer point of view, the HTTP protocol boils down to requests and responses.
One could even think of a Web application as a general function of type `HTTPRequest -> HTTPResponse`.

### Async

When dealing with Internet applications in practice, it turns out that aspect of asynchrony plays a crucial role.
Operations that require reading / writing from the input / output are extremely ineffective if performed synchronously.
Synchronous execution of tasks that require input / output communication results in blocking threads.
In return, when a thread is blocked, next incoming request will have to engage more threads.
As a result, having a significant number of blocked threads results in high memory consumption and ineffective usage of resources.

Writing software in asynchronous fashion does not come for free.
Callback-passing style is one of the most popular techniques for writing asynchronous code.
While the technique may be a sufficient solution for smaller problems, when applied to large and complex systems, code very easily gets hard to maintain.

Among various approaches to asynchronous programming, there is one that abstracts away the concept of asynchrony from the actual flow of program.
It is called **future**, also known as **promise** or **asynchronous workflow** in different programming languages.
This approach bypasses callbacks in a clever way, resulting in code that is easier to read and reason about.
Futures in conjunction with services and filters present a powerful programming model for building safe, modular, and efficient server software {{{ServerFunction}}}.
All those concepts originate from the functional paradigm and that is why utilizing functions can be helpful for developing client-server architecture.
In F# there is a type called `Async`, which represents **asynchronous workflow**.

### Parametric polymorphism

It is worth noting that `Async` in F# is a generic type.
This means that any arbitrary type can be applied to `Async`.
The concept is also known as parametric polymorphism and is very important in functional programming.
First description of parametric polymorphism was made by Strachey {{{strachey2000fundamental}}}:

Let `f` be a function of type `a -> b` and `l` be a list of type `a list` (`l` has only elements of type `a`).
A function `map` can be constructed that applies `f` on each element of `l` returning a `b list`.
We can say that `map` function is polymorphic of parametric type `(a -> b, a list) -> b list`.

Music Store Tutorial
--------------------

The example application is built on top of the Suave.IO {{{suave}}} framework, which allows to write functional server software in a composable fashion.
Here, composable means that very granular functions / components can be easily joined to create more robust functions.
The resulting functions can be then again glued together.
Following the pattern, one can get eventually a fully working software, which in practice turns out to be a function.

### WebPart

The most important building block in Suave.IO is **WebPart**.
It is a basic unit of composition in this framework - when combining two smaller WebParts together, a composite WebPart gets created.
WebPart is a **type alias** for the following: 

```fsharp
HttpContext -> Async<HttpContext option>```

The above notation describes a function from `HttpContext` to `Async<HttpContext option>`.
`HttpContext` is a type that contains all relevant information regarding HTTP request, HTTP response, server environment and user state.
Type parameter for `Async` is `HttpContext option` in that case.

### Option

Conclusions
-----------