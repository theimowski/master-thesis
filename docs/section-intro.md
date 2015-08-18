INTRODUCTION AND AIM OF THE THESIS
==================================

Computer science has very strong academic background.
Plenty of studies concern computer science subareas such as hardware engineering, computing technology, algorithms, information technology and **software engineering**.
The last is undoubtedly one of the most remunerative realm of computer science.
One can however get the impression that at some point, software industry detached from academic stream to live its own life.
While scholars focused on proving how **mathematics** influenced the theory of computation, dogmatists strived to show that these two do not have much in common.
The latter managed to convince the society that imperative programming is the key to solving all kinds of computational challenges.
Object-oriented programming (OOP), which originated from the imperative movement is the dominant paradigm in the enormous software industry.
On the other hand functional programming (FP), born from the declarative branch of programming paradigms as a major subject of studies on programming in universities, did not get much notice in software business around the world.
Luckily, recent trends show that FP is gaining more and more interest among software developers as well as real-world, business use cases.

The aim of this thesis is to describe how FP appeared at the stage, what it has to offer as opposed to its counterpart approaches, in which programming areas it is visible and what does a process of creating application with functional paradigm in mind look like.
Throughout the course of the thesis benefits gained from applying FP are boosted, often by citing various works - both coming in form of academic publications as well as successful real-life stories from software industry.
Focus is also laid on pointing out caveats of imperative paradigm and OOP in particular, together with examples of how FP addresses or omits these problems.

Section {{PROGRAMING PARADIGMS}} considers programming paradigms.
It first discusses imperative programming origins and its history.
Then it identifies how OOP was invented, together with its premises and design principles that establish the philosophy.
A few drawbacks are presented in order to emphasize what issues occur within this approach.
Next, declarative paradigm is introduced simultaneously with such derivatives as logic and constraint programming.
The section concentrates on FP, by showing how it relates to declarative paradigm and presenting a few basic foundations of FP.
Finally a connection between programming paradigms and code quality is demonstrated with help of community's impressions as well as an authentic example.

Section {{FUNCTIONAL PROGRAMMING IN INDUSTRY}} examines business cases where FP has already been utilized.
To start with, it goes through the list of most popular FP languages that are included in job offerings around the globe.
Each of the language is briefly described and examples of companies and projects that incorporate the language are enlisted.
In the further course of section {{FUNCTIONAL PROGRAMMING IN INDUSTRY}} a few areas are presented that seem most prominent with regards to application of FP.
Every area comes with a motivation by pointing out the main advantages for using functional approach.

Section {{APPLICATION OF FUNCTIONAL PROGRAMMING}} converts from being theoretic to being very practical by demonstrating multiple code excerpts from a complete web-based application.
Firstly, choice of Internet applications domain as the infrastructure for example program is justified.
Secondly, the domain is explored from the point of view of FP, together with emphasizing how concept of asynchrony can be dealt with functional approach.
Lastly, a significant part of the thesis is devoted to exhibit step-by-step guide with code listings on how to tackle cross-cutting concerns in web development.

Similar study on the topic {{{pastircak2014application}}} describes application of FP to enterprise systems.