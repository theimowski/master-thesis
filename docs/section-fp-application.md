APPLICATION OF FUNCTIONAL PROGRAMMING
======================================

Many associate functional programming with academic environment only.
People tend to think of functional programming as an experimental area which is not used in practice in software industry.
This might be because functional programming relies on mathematical foundations, thus is surely more popular among scholars.
Despite the fact that functional paradigm has indeed academic background, it is being widely adopted in business cases.
This observation turns out to be specially true at the time of writing, as more and more companies show interest in using functional programming.
This section goes through the most popular functional languages, describing how they got created and what are their main premises.
Real-world applications that achieved success in the industry are pointed out together with substantiation of why functional programming proved helpful.
The success stories presented in this section are largely cited from a number of websites.
After that, three areas where the paradigm really shines and leaves other approaches behind, are enlisted.

Popular languages
-----------------

Six functional programming languages which seem to be the most popular nowadays in software industry, are briefly described.
This list is equivalent to the one present at Functional Works website {{{functionalworks}}}, which contains languages that the biggest (at the time of this writing) functional programming recruitment agency around the globe is seeking employees for.
Description for each language sticks to the following structure:

* **short history** is first presented, when a particular language was built and under what circumstances,
* **most important features** and properties of the language are described next, including static typing and purity,
* **names of companies** are finally enlisted that use the language in production or for their internal tools.

The oldest functional languages like Miranda, LISP or Scheme are deliberately not included in the list, as they are not really widely used in business applications.
It is however important to remember that those languages became inspiration for more modern functional languages that found their way to the list.

### Haskell

First version of Haskell was developed in 1990, but it awaited newer releases, of which Haskell 2010 is the latest one.
It was named after a mathematician, Haskell Curry, famous for work in combinatory logic, as well as the "currying" concept which is ubiquitous in functional programming.
Glasgow Haskell Compiler (GHC) is the best known compiler of the language.
Written almost entirely in Haskell, GHC is *"a freely available, robust and portable compiler for Haskell that generates good quality code"* {{{jones1993glasgow}}}.

Haskell is the most prominent functional languages among all with regards to function purity.
The language does not allow any type of side effects to be performed, meaning that **every** function must be pure.
Another intriguing property of the language is that it does not evaluate any expressions eagerly.
Every expression in Haskell is deferred until it has to be evaluated, and never before.
This property, commonly known as "lazy evaluation" was described in previous section.
Haskell is s statically typed language, with its type system being one of the most strict in this category.
Following are some of the type constructs baked into Haskell type system:

* **Type Classes**, which *"provide a uniform solution to overloading, including providing operations for equality, arithmetic, and string conversion"* {{{hall1996type}}},
* **Data Types**, which represent discrete set of constructors for a specific type of data,
* **Type Inference** - a mechanism used by the type system to infer type of a function or expression to make them statically typed, without the need of manual annotations.

Strictness in context of static typing means that in order to compile a program, more prerequisites must be met.
While it makes harder for a software engineer to compile code in strongly statically typed language, a priceless benefit of program correctness is usually gained.

Because of its nature, Haskell still struggles to be visible in enterprise architectures, although big players do use it for their tools (often as part of their research department) {{{haskellindustry}}}:

* **Facebook** internally makes use of *HaXL* - a tool that simplifies access to remote data, such as databases or web-based services,
* **Microsoft** uses Haskell for its production serialization system named *Bond*, which proves useful in high scale services,
* Haskell was also adopted by a plenty of financial institutions including:
    * Bank of America Merril Lynch,
    * Barclays Capital Quantitative Analytics Group,
    * Credit Suisse Global Modeling and Analytics Group,
    * Deutsche Bank Equity Proprietary Trading.


### OCaml

First OCaml version was released in 1996 as an extension to the Caml (Categorical Abstract Machine Language) programming language, which in turn comes from the family of ML (Metalanguage) dialects.
The development of the language is led by a company called *Inria*.

Coming from the statically typed languages family, OCaml ships with a rich type system offering type inference, as well as various data types, such as records or sums {{{ocamlwebsite}}}.
Similar to Haskell, OCaml enables interoperability with languages like C, thanks to the foreign function interface feature.
In addition to that, OCaml provides ability to compile efficient machine code, which boosts its attractiveness.
The language is a valuable example of hybrid, multi-paradigm movement, combining approaches from both declarative (functional) and imperative (object-oriented) worlds.
With its distinctive syntax, OCaml became an inspiration for a few modern languages like F#, Scala or Rust.

OCaml found its adoption in a few prospering companies, such as {{{ocamlwebsite}}}:

* **Jane Street** - a proprietary trading firm that uses OCaml as its primary development platform {{{minsky2008caml}}},
* **LexiFi** - company that used OCaml to develop the Modeling Language for Finance (MLFi), which is the first formal language that accurately describes the most sophisticated capital market, credit, and investment products,
* **Facebook** - which does not need any introduction, uses OCaml for the following:
    * *Hack* - a programming language with its compiler written in OCaml, as an extension to PHP with static typing,
    * *pfff* - a set of tools for static code analysis, which allows Facebook to keep track of its enormous code-bases.

### F#

FSharp (F#) is a language that was born in Microsoft Research department with its first version released in 2005 together with Microsoft Visual Studio 2005.
With great help of the language designer Don Syme, the F# language evolved and newer versions got delivered, with the most recent 4.0 version released in parallel to the time of writing.
Don Syme, who worked for Microsoft Research was also one of the people responsible for introducing generics into .NET ecosystem.
In fact, one of the reasons why .NET generics feature was incorporated into the platform, was that F# language required generics for its design {{{donsymeinterview}}}.
Apart from Microsoft taking care of the language development, there exists a non-profit F# Software Foundation {{{fsharpwebsite}}} that gathers community around the language and makes its best to popularize F# among software engineers.
F# design was influenced by a set of other languages, most notably:

* **OCaml**, with regards to main syntax constructs, 
* **Python**, by adapting the whitespace sensitivity property, 
* **Haskell**, from which it accommodated many functional features,
* **Erlang**, in respect to the message passing model and asynchrony support.

F# is statically typed and its type system is considered to be very strict (almost as Haskell).
Just as OCaml, F# is a functional-first language, meaning that while it encourages to write code in functional style, it is not purely functional and allows also for both imperative and object-oriented constructs.
Being built on top of .NET, it enables interoperability with all languages from this platform.
Code written in F# is succinct and readable, making the language an attractive candidate for fast prototyping and reducing time-to-market factor {{{fsharp2014deep}}}.

Following are some examples of fruitful employment of F# in the industry {{{fsharpwebsite}}}:

* **Kaggle** - company that deals with Data Science at first used F# only for core analysis algorithms only to discover that the language can replace C# in majority of components,
* **Jet.com** - service which specializes in on-line shopping field, utilizes F# in most of its back-end processing libraries,
* **Tachyus** - U.S. based firm which works in energy sector, employs F# as a base programming language for their software, 
* as was the case with Haskell, F# also proves useful in a number of financial institutions, such as:
    * Credit Suisse,
    * Counterparty Risk Handelsbanken,
    * other financial services firms, which do not reveal their identity.

### Scala

Scala language was built for (competitive from Microsoft's point of view) Java Virtual Machine (JVM) platform in 2004.
Initially it also targeted the .NET platform, however this distribution stopped being supported some time later.
The creator of Scala language, Martin Odersky, had solid backgrounds in Java world as he was involved into Java Generics feature (just like Don Syme for .NET generics) as well as Java compiler.
A few years after Scala was released, it turned out to be very prominent, and in result a number of financial investments have been made to promote the language.
Together with his collaborators, Martin Odersky started a company called "Typesafe Inc." in 2011 in order to offer support and guidance for using Scala for enterprise cases.

Just as Haskell, OCaml and F#, Scala is a statically typed language.
Again, the type system ships with type inference mechanism which minimizes need of type annotating members and functions, making the code shorter.
Scala provides a feature called "Traits", which enables to mix multiple interfaces and their behavior within a single class.
The Traits feature can be compared to concept of Typeclasses in Haskell, however the foundations differ.
With its full interoperability with JVM and Java, Scala is a multi-paradigm language that allows to write code in functional as well as object-oriented manner.
Inspired by other languages from functional family, Scala comes with well-known to functional community features, such as pattern matching or higher-order functions.
Language designers made their best to fit Scala to systems that require distribution and concurrency by adapting actor-based processing model and making it easier to write asynchronous code with Futures.

Partly thanks to the money invested, but also because of the language power itself, Scala found its way to be incorporated into an impressive amount of significant businesses around the world {{{typesafestories}}}:

* **Twitter** completely re-architected its services to use Scala {{{eriksen2013your}}} and achieved an enormous indicator of hundreds of thousands tweets per second,
* **LinkedIn** converted from plain Java web frameworks to using Scala and Play framework, which turned out to speed up code and test cycles as well as made the LinkedIn platform more scalable,
* **Walmart** Canada by rebuilding its web and mobile stacks with combination of Scala language, Play framework and Akka library for actor-based processing, noted a serious improvement in web traffic conversions and mobile orders for their Canada's largest on-line retailing business,
* **Airbnb**, one of the largest lodge renting services, based their internal tool for scheduling data pipelines on Scala language, making the tool easier to manage and debug than with previously utilized Cron,
* **The Guardian** upon transition from standard print-based to digital-first newspaper organization, chose stack based on Scala language (similar to Walmart Canada) which proved to be a solid and scalable foundation for its new services.

### Clojure

Clojure was created by Rich Hickey in 2007.
Its major motivation was to design a language that has Lisp-like syntax, embraces existing virtual machine platform (JVM) and allows to write code in functional style with emphasis on immutability and concurrency support.
Clojure was made a fully open-source software so that it could be easier adopted by the community.

Unlike previously enlisted languages, Clojure is dynamically typed.
The code is compiled on-the-fly to corresponding JVM byte-code, but is not type-checked before running, which does not guarantee type safety and can result in this class of run-time errors, which would not be otherwise possible in statically typed language.
Dynamic typing in Clojure made it easier to incorporate macros feature into the language.
Macro system in Clojure follows philosophy named "code-as-data", which enables to generate and manipulate arbitrary code dynamically.
This feature is a great advantage for Clojure over most of statically typed functional languages, which lack of such functionality.
Similarly to F# or Scala, Clojure is functional-first, meaning it provides a set of functional constructs, but it also allows to write impure, imperative code which does not hold referential transparency property.
Clojure also comes with a great deal of models for concurrent processing, each of which is built on top of the standard Java concurrency libraries {{{clojurewebsite}}}:

* **STM** (Software transactional memory system) which can modify and share state between multiple threads in a synchronous and coordinated manner (STM is also presented in the further course of thesis, in context of concurrency support for Haskell language),
* **Agents** model, that separates independent communicating parties and allows for message passing in asynchronous fashion,
* **Atoms** system, being similar to agents, but with the difference in communication mode which is synchronous,
* **Dynamic var**, which isolates modifiable state within the communicating threads.

With its distinctive nature, Clojure settled down in code-bases in a number of companies, including {{{clojurestories}}}:

* **Puppet Labs**, that used Clojure for building *Trapperkeeper* - an open source framework for hosting services which are supposed to run for a long period,
* **Beanstalk**, which after switching to Clojure for its continuous integration application gained a multiply of 20 boost in performance for the caching component,
* **ThoughtWorks**, a consulting company, which gave Clojure a try to rewrite their CMS solution, found out that the language helped them to deliver the software before deadline and under designated budget,
* **Sonian**, that deals with archiving emails in cloud, employs Clojure for its core back-end components and claims that thanks to such solution is able to ship new features very quickly,
* **MailOnline**, which is the world's biggest newspaper website, decided to rebuild old service on top of Clojure and did not regret that choice.

### Erlang

First version of Erlang was released back in 1986. 
Erlang evolution led to release of stable version 18.0 at the time of the writing.
Aim of creating Erlang language was to enhance telephony software industry.
Origin of the language's name is ambiguous: some claim that it is a tribute to danish mathematician Agner Krarup Erlang, while other say that it is an abbreviation of "Ericsson language" {{{armstrong2007history}}}.
Indeed, Ericsson is not only the company that first incorporated Erlang, but also the one that started the development of the language at its Ericsson Computer Science Laboratory.
Erlang was born in Ericsson labs thanks to Joe Armstrong and Robert Virding together with help of Mike Williams {{{armstrong2007history}}}.
At first, the language usage was restricted to Ericsson only, however it became open sourced some time later. 

The most important aspect of Erlang is concurrency, for which the language has a very sophisticated runtime.
It enables to span multiple processes with little cost (a process is related to runtime, not operating system).
As processes do not share memory, the main philosophy of Erlang is based on passing messages between them.
Message passing technique relies on process isolation and asynchronous communication.
All processes are autonomic, thus there is no single point of failure in systems built on top of Erlang.
The message passing approach introduced in Erlang influenced plenty of other languages that appeared afterwards.
Erlang language is dynamically typed, which entails all the related drawbacks of dynamic languages, but it does also imply some advantages, one of which is ability of providing hot code replacement.
Hot code replacement is a very powerful feature, that allows to deploy new versions of software to production, without need to stop the existing ones.
While Erlang was designed with functional approach in mind and it mainly consists of functional constructs, it is not pure in respect to implementation of a single process which can follow imperative techniques.

Besides Ericsson, Erlang contributed to plenty of success stories for world-class companies' production systems {{{cesarini2009erlang}}}:

* **Amazon** makes use of Erlang for its database component SimpleDB, that is a building block of Amazon Elastic Compute Cloud (EC2),
* **Yahoo!** employs Erlang for its product called Delicious, a bookmarking service that deals with millions of users and hundreds of millions of bookmarks,
* **Facebook** powers the back-end services of its chat service with Erlang, delivering messages to people all around the world,
* **T-Mobile** uses the language to support their Short Message Service (SMS), as well as authentication services,
* **Motorola** utilizes Erlang in public-safety sector for analyzing and processing telephone calls.

Dominated areas
---------------

Functional programming is getting more and more popular in the software industry.
Not only the languages that were designed with functional approach in mind are being used, but also those languages that originate from a different paradigm branch incorporate functional techniques.
Good examples of such languages are C# and Java, which initially supported only object-oriented and imperative style of programming, but new versions of these two introduce concepts well known to functional programmers, such as higher-order functions, lambda expressions or type polymorphism.

While there are a lot of fields in software industry where functional programming is employed to only a specific sub-domain, like processing or analytics components, there are areas where functional paradigm **dominates** and leaves other behind.
This domination manifests with impact of institutions that achieved success with the paradigm on other players that have not yet tried different approach than imperative.
Such areas, where functional seems to take the lead are described in this section, together with a rationale on why functional programming is more promising in specific case.
As a disclaimer, it is important to note, that the domination does not necessarily mean more job openings in the market (as non-functional paradigms are in general still more popular), but rather that there is a visible trend of growing employments in these areas.

### Financial industry

Financial sector faces many challenges related to computing nowadays.
Institutions compete with each other with regards to real-time processing speed of their systems.
Stochastic simulations of mathematical models used in financial software can potentially strive to infinite amount of compute cycles.
Those models in turn happen to be very complicated and require tough logic implementation.
In addition to that, volumes of data to be processed reach unbelievable sizes, and the trend seems to grow {{{berthold2012functional}}}.
Those challenges squeeze huge amounts of money invested in financial software, but as this industry does not suffer from poverty, funds are still allocated to improve the overall quality and performance of systems.
Functional programming turns out to address most of these problems and that is why it settled down for good in the financial industry.

Several works describe role of functional programming in software development for financial sector and explain why the paradigm proved helpful {{{minsky2008caml,berthold2012functional,fsharp2014deep}}}.
Below are excerpts from these works that point out main reasons why financial industry benefits from application of functional programming:

* **Maintainability** - thanks to the code being more succinct and readable, it is much easier for a developer to reason about a specific piece (especially since financial models can be really complex) and thus it makes the system more maintainable {{{minsky2008caml,fsharp2014deep}}}, moreover such properties as immutability or referential transparency cause a less number of software defects;
* **Performance** - compiled code turns out to be really fast when certain optimization techniques are used {{{minsky2008caml}}}, plus the interoperability with libraries built from different languages (with help of foreign function interface) allows to natively execute chunks that are performance critical for the business;
* **Generalization** - higher order functions and currying abstract away the behavior from data, making it possible to implement simulation algorithms applicable to various pricing derivatives {{{fsharp2014deep}}}, type polymorphism done this way leads to less amount of code compared to what it would require with only object-oriented techniques used;
* **Parallelization** - immutable data structures that are used in functional programming encourage to process data in parallel, as there is a lower risk of any kind of concurrency issue, resulting in vertical processing scalability and allowing to consume larger volumes of data for financial analysis {{{berthold2012functional}}};
* **Hiring better engineers** - it is noticed that job openings related to functional languages like OCaml {{{minsky2008caml}}} attract more talented engineers (financial institutions can afford higher salaries implied), even though there is a minority of developers that are familiar with functional programming languages (such phenomenon is known as "the python paradox" {{{pythonparadox}}});

### Data science

Another area where functional programming seems to start taking lead is data science.
Data science is a very broad field which concerns multiple computer science topics, such as artificial intelligence, machine learning or data mining.
Because these topics use extensively various numerical methods, functional programming feels like a natural candidate when it comes to choosing a proper paradigm for solving particular problem.

In his work {{{hughes1989functional}}}, Hughes describes how functional properties make it convenient to implement an artificial intelligence heuristic.
The example heuristic in the work is an alpha-beta method of determining best move for computer opponent in a logic game that can be represented by a game state tree.
Thanks to using higher-order functions, the game tree can be defined in a recursive manner, where the recursive higher-order function aims to provide valid moves from a given node in the tree.
As a game tree in logic game can be potentially infinite, the functional property of lazy evaluation enables to define the whole tree without actually evaluating unnecessary nodes of the tree.

Machine learning is yet another perfectly suitable field for functional paradigm, what is shown in Brandewinder's book {{{brandewinder2015ml}}}.
The book presents examples in F# language of addressing concerns feasible for machine learning such as spam filter, optical character recognition, regression models and logical games.
Interesting aspect of F# in context of machine learning is the interoperability (using F# Type Providers feature) with R language, which is famous for its comprehensive suite of ready functions and tools for data processing.

Proof for growing interest in using functional techniques for data mining are books like Haskell Data Analysis Cookbook {{{shukla2014haskell}}}, where the author illustrates in details a variety of concepts including advanced data analysis and processing as well as input and output operations for consuming the data from and to outside world.
The latter concept despite standing for hard to express in pure functional approach is clearly explained in the book, which confirms that even purely functional languages can be treated as general-purpose programming languages.

### Concurrent systems

The survey would not be complete without mentioning concurrent systems, which benefit a lot from functional programming.
Although they are not really a separate domain like financial industry or data science, concurrent systems made it to this list, because of the great impact of functional paradigm on concurrency aspect.
That said, software from arbitrary domain, be it social network or shopping industry, which relies on processing parallelization might potentially gain from employing functional programming.

An obvious advantage that applies to parallelization is that data structures are immutable.
The immutability is enforced in pure languages, whereas in functional-first languages it is the default behavior that can be overridden, i.e data structures can be made mutable (usually for performance reasons).
Vast majority of concurrency issues appear in object-oriented software due to the fact that objects are stateful, thus accessing an object in multi-threaded environment may lead to unexpected results when certain precautions are not taken.
In functional paradigm on the other hand, such danger goes away when immutable data structures are being used, releasing the engineer from non-trivial task of context synchronization and preventing whole class of defects.

One approach to write concurrent programs is described in article by Jones {{{jones2007beautiful}}}, where the author outlines benefits of Software Transactional Memory (STM) mechanism embedded into Haskell.
The STM prevents issues related to *locks* as well as *condition variables*, therefore allowing the code to be more modular (composable).
Thanks to the fact that STM bases on the Haskell type system, the compiler disallows illegal (from the concurrency point of view) read / write operations in code blocks that are meant to be executed *atomically*.
It is worth noting, that the mechanism can also be potentially employed into imperative programming languages, however sophisticated language features and ease of modularity, present in functional languages, allow STM to be fully efficient.

Another concurrent programming model associated with functional paradigm is, already described together with Erlang, message passing.
The model is ubiquitous in context of Erlang, as it is emphasized in {{{cesarini2009erlang}}} that message passing is the only way for communicating between processes in this language.
Practice has shown that message passing concurrency model is extremely reliable and profitable, as systems relying on this technique achieve one of the highest system up-time indicators, as well as allow to process impressive amount of messages per unit of time.

Above approaches are just examples of how functional paradigm influenced development of concurrent algorithms.
Success stories demonstrated in previous section prove that employing functional programming in distributed and concurrent systems made them more scalable, stable and efficient.
This is the main reason, for which more and more companies show interest in incorporating this philosophy into their business applications.