INTRODUCTION AND AIM OF THE THESIS
==================================

Computer science has very strong academic background.
Plenty of studies concern computer science subareas such as hardware engineering, computing technology, algorithms, information technology and **software engineering**.
The last is undoubtedly one of the most remunerative realm of computer science.
One can however get the impression that at some point, software industry detached from academic stream to live its own life.
While scholars focused on proving how **mathematics** influenced the theory of computation, dogmatists strived to show that these two do not have much in common.
The latter managed to convince the society that imperative programming is the key to solving all kinds of computational challenges.
Object-oriented programming (OOP), which originated from the imperative movement and became extremely popular amid various enterprises, is currently the dominant paradigm in the enormous software industry.
On the other hand, functional programming (FP), which was born from the declarative branch of programming paradigms as a major subject of studies on programming in universities, did not get as much notice in software business around the world.
Luckily, recent trends show that FP is gaining more and more interest among software developers as well as real-world, business use cases.

The aim of this thesis is to highlight that FP does not only find its usage in academic researches, but is also applicable to various business or enterprise scenarios.
This is done by describing how FP appeared at the stage and what it has to offer as opposed to its counterpart approaches.
In addition to that, the thesis specifies programming areas where FP is most notable, as well as what does a process of creating an E-commerce application with functional paradigm in mind look like.
Throughout the course of this thesis benefits gained from applying FP are boosted, often by citing various works - both coming in form of academic publications as well as successful real-life stories from software industry.
Focus is also laid on pointing out caveats of imperative paradigm and OOP in particular, together with examples of how FP addresses or cleverly omits such kind of problems.

Section {{PROGRAMING PARADIGMS}} considers programming paradigms.
It first goes back to beginnings of software engineering history to discuss imperative programming origins and its evolution.
Then it identifies how OOP was invented, together with premises and design principles that establish the philosophy.
In passing, a few drawbacks are presented in order to emphasize what issues occur with OOP approach.
Next, declarative paradigm is introduced in parallel with its derivatives such as logic and constraint programming.
After that, the section concentrates on FP, by showing how it relates to declarative paradigm and presenting basic foundations and properties of FP.
Finally, a connection between programming paradigms and code quality is demonstrated with help of the community's studies as well as detailed comparison of solving an authentic problem with imperative and functional ways.

Section {{FUNCTIONAL PROGRAMMING IN INDUSTRY}} examines business cases where FP has already been utilized.
To start with, the section goes through some of the most popular FP languages that are being listed in job offerings around the globe.
Each of the language is briefly described and contains examples of companies and projects that incorporate the language.
In the further course of section {{FUNCTIONAL PROGRAMMING IN INDUSTRY}} a few areas are presented that seem most prominent with regards to application of FP.
Every area comes with a motivation for using functional approach by pointing out the main advantages that the paradigm entails.

Section {{APPLICATION OF FUNCTIONAL PROGRAMMING}} converts from being theoretic to being very practical.
It does so by demonstrating multiple code excerpts from a complete web-based application.
Firstly, choice of Internet applications domain as the base for example program is justified.
Secondly, the domain is explored from the point of view of FP, together with emphasizing how concept of asynchrony can be dealt with functional approach.
Lastly, a significant part of the thesis is devoted to exhibit step-by-step guide with code listings on how to tackle cross-cutting concerns in web development.
Language chosen for the demo application is FSharp (F#), a functional-first programming language that emerged from Microsoft Research initiative.

Topic of the thesis is relatively new, however there already exist some works that consider FP in context of software industry and business applications in particular.
For example, Pastircak {{{pastircak2014application}}} describes application of FP and F# language to enterprise systems, underlining the interoperability feature that comes with F#.
Other papers depict specific use cases of FP, such as building distributed and efficient back-end servers {{{eriksen2013your}}}, or applying FP to the financial sector software {{{berthold2012functional}}}.
Petricek at al. {{{fsharp2014deep}}} cumulate multiple examples of FP and F# usage among variety of domains.
This thesis, after going through the concept of FP paradigm and its premises, shows practical usage of FP on the example of building a web application in F#.