RESEARCH
========

Previous section described how Functional Programming managed to be successfully applied in many projects.
In this section focus will be laid on creating software that uses functional techniques.
For that purpose, process of developing such application will be shown.
In the course of this section, multiple comparisons will be made between Object-Oriented and Functional approaches.

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

When dealing with Internet applications in practice, it turns out that aspect of asynchrony plays a crucial role.
Operations that require reading / writing from the input / output are extremely ineffective if performed synchronously.
Synchronous execution of tasks that require input / output communication results in blocking threads.
In return, when a thread is blocked, next incoming request will have to engage more threads.
As a result, having a significant number of blocked threads results in high memory consumption and ineffective usage of resources.

Writing software in asynchronous fashion is possible, but does not come for free.
Callback-passing style is one of the most popular techniques of dealing with asynchronous code.
It is a sufficient solution for small problems, however when applied to large and complex systems, code may very easily get hard to maintain.
...
Abstractions like future (also known as promise or asynchronous workflow), services and filters combine to present a powerful programming model for building safe, modular, and efficient server software {{{ServerFunction}}}.
Such abstractions originate from the functional approach and that is why utilizing functions can feel natural when developing client-server architecture.

Music Store Tutorial
--------------------

Conclusions
-----------