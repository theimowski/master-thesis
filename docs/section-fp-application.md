APPLICATION OF FUNCTIONAL PROGRAMMING
======================================

For many, functional programming is associated with academic environment only.
People tend to think of functional programming paradigm as an experimental area which is not used in practice.
This might be because functional programming has its roots in mathematics.
Indeed there exist concepts from mathematics which are the components of functional paradigm, such as:

* **Lambda calculus** - formal mathematical system which became the base of functional paradigm,
* **Functions** - treated as first-class citizens in functional programming; their purity as seen by mathematicians allow to use powerful constructs,
* **Category theory** - this very abstract field of mathematics is reflected in type system of a few functional languages.

Despite the fact that the paradigm comes from academic background, it is being widely adopted in business use cases, and what is more this observation at the time of writing turns out to be specially true.
This section first goes through the most popular functional languages, describing how they got created and what are their main premises.
Next, areas are enumerated where the paradigm really shines, leaving other approaches behind.
Finally, real-world applications that achieved success in the business are pointed out together with substantiation of why functional programming proved helpful.

Popular languages
-----------------

Six functional programming languages which seem to be the most popular nowadays in software industry, are briefly described.
Description for each language sticks to the following order:

* first, short history is presented, when a particular language was built and under what circumstances,
* next, most important features and properties of the language are described, including static typing and purity,
* finally, names of companies are enlisted that use the language in production or for their internal tools.

The oldest functional languages like LISP or Scheme are deliberately not included in the list, as they are not really widely used in business applications.
It is however important to remember that those languages became inspiration for more modern functional languages that found their way to the list.

### Haskell

First version of Haskell was developed in 1990, but it awaited newer releases, of which Haskell 2010 is the latest one.
It was named after a mathematician, Haskell Curry, famous for work in combinatory logic, as well as "currying" concept which is ubiquitous in functional programming.
Glasgow Haskell Compiler (GHC) is the best known compiler of the language.
Written almost entirely in Haskell, GHC is a freely available, robust and portable compiler for Haskell that generates good quality code {{{jones1993glasgow}}}.

Haskell is the most prominent functional languages among all with regards to purity.
The language does not allow any type of side effects to be performed, meaning that **every** function must be pure.
Another intriguing property of the language is that it does not evaluate any expressions eagerly.
Every expression in Haskell is deferred until it has to be evaluated, and never before.
This property is commonly known as "lazy evaluation" and allows for interesting constructs, such as infinite lists.
Haskell is s statically typed language, with its type system being one of the most strict in this category.
It consists of type constructs such as:

* **Type Classes** - provide a uniform solution to overloading, including providing operations for equality, arithmetic, and string conversion {{{hall1996type}}},
* **Data Types** - represent discrete set of constructors for a specific type of data,
* **Type Inference** - mechanism used by the type system to infer type of a function or expression to make them statically typed, without the need of manual annotations.

Strictness in context of static typing means that in order to compile a program, more prerequisites must be met.
While it makes harder for a software engineer to compile code in strongly statically typed language, a priceless benefit of program correctness is usually gained.

Because of its nature, Haskell still struggles to be visible in enterprise architectures, although major companies do use it for their tools (often as part of their research department){{{haskellindustry}}}:

* **Facebook** internally makes use of *HaXL* that simplifies access to remote data, such as databases or web-based services,
* **Microsoft** uses Haskell for its production serialization system, *Bond* in high scale services,
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

* **Jane Street** - a proprietary trading firm that uses OCaml as its primary development platform,
* **LexiFi** - company that used OCaml to develop the Modeling Language for Finance (MLFi), which is the first formal language that accurately describes the most sophisticated capital market, credit, and investment products,
* **Facebook** - which does not need any introduction, uses OCaml for the following:
    * *Hack* programming language with its compiler written in OCaml, as an extension to PHP with static typing,
    * *pfff* - a set of tools for static code analysis, which allows Facebook to keep track of its enormous code-bases.

### F#

FSharp (F#) is a language that was born in Microsoft Research department with its first version released in 2005 together with Microsoft Visual Studio 2005.
With great help of the language designer Don Syme, the F# language evolved and newer versions got delivered, with the most recent 4.0 version released in parallel to the time of writing.
Don Syme, who worked for Microsoft Research was also one of the people responsible for introducing generics into .NET ecosystem.
In fact, one of the reasons why .NET generics feature was incorporated into the platform, was that F# language required generics for its design {{{donsymeinterview}}}.
Apart from Microsoft taking care of the language development, there exists a non-profit F# Software Foundation {{{fsharpwebsite}}} that gathers community around the language and makes its best to popularize F# among software engineers.
F# design was influenced by a set of other languages, most notably:

* **OCaml**, with regards to main syntactic constructs, 
* **Python**, by adapting the whitespace sensitivity property, 
* **Haskell**, from which it accommodated many functional features,
* **Erlang**, in respect to the message passing model and asynchrony.

F# is statically typed and its type system is considered to be very strict (almost as Haskell).
Just as OCaml, F# is a functional-first language, meaning that while it encourages to write code in functional style, it is not purely functional and allows also for both imperative and object-oriented constructs.
Being built on top of .NET, it enables interoperability with all languages from this platform.
Code written in F# is succinct and readable, making the language an attractive candidate for fast prototyping and reducing time-to-market factor.

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
Scala was considered very prominent, and in result a number of financial investments have been made to promote the language.
Together with his collaborators, Martin Odersky started a company called "Typesafe Inc." in 2011 in order to offer support and guidance for using Scala for enterprise cases.

Just as Haskell, OCaml and F#, Scala is a statically typed language.
Again, the type system ships with type inference mechanism which minimizes need of type annotating members and functions, making the code much shorter.
Scala provides a feature called "Traits", which enables to mix multiple interfaces and their behavior within a single class.
The Traits feature can be compared to concept of Typeclasses in Haskell, however the foundations differ.
With its full interoperability with JVM and Java, Scala is a multi-paradigm language that allows to write code in functional as well as object-oriented manner.
Inspired by other languages from functional family, Scala provides with well-known, associated with functional paradigm features, such as Pattern Matching or Higher-order functions.
Language designers made their best to fit Scala to systems that require distribution and concurrency by adapting actor-based processing model and making it easier to work write asynchronous code with Futures.

Partly thanks to the money invested, but also because of the language power itself, Scala found its way to be incorporated into an impressive amount of significant businesses around the world {{{typesafestories}}}:

* **Twitter** completely re-architected its services to use Scala {{{eriksen2013your}}} and achieve an enormous indicator of hundreds of thousands tweets per second,
* **LinkedIn** converted from plain Java web frameworks to using Scala and Play framework, which turned out to speed up code and test cycles as well as made the LinkedIn platform more scalable,
* **Walmart** Canada by rebuilding its web and mobile stacks with combination of Scala language, Play framework and Akka library for actor-based processing, noted a serious improvement in web traffic conversions and mobile orders for their Canada's largest on-line retailing business,
* **Airbnb**, one of the largest lodge renting services, based their internal tool for scheduling data pipelines on Scala language, making the tool easier to manage and debug than with previously utilized Cron,
* **The Guardian** upon transition from standard print-based to digital-first newspaper organization, chose stack based on Scala language (similar to Walmart Canada) which proved to be a solid and scalable foundation for its new services.

### Clojure

Clojure was created by Rich Hickey in 2007.
Its major motivation was to design a language that has Lisp-like syntax, embraces existing virtual machine platform (JVM) and allows to write code in functional style with emphasis on immutability and concurrency support.
Clojure was made a fully open-source software so that it could be easier adopted by the community.

Unlike previously enlisted languages, Clojure is dynamically typed.
The code is compiled on-the-fly to corresponding JVM byte-code, but is not type-checked before running, which does not guarantee type safety and can result in this class of run-time errors, which would not be possible in statically typed language.
Dynamic typing in Clojure made it easier to incorporate macros feature into the language.
Macro system in Clojure follows philosophy named "code-as-data", which enables to generate and manipulate arbitrary code dynamically.
This feature is a great advantage for Clojure over most of statically typed functional languages, where it is not present.
Similarly to F# or Scala, Clojure is functional-first, meaning it provides a set of functional constructs, but it also allows to write impure, imperative code which does not hold referential transparency property.
Clojure also comes with a great deal of models for concurrent processing, each of which is built on top of the standard Java concurrency libraries {{{clojurewebsite}}}:

* **STM** (Software transactional memory system) which can modify and share state between multiple threads in a synchronous and coordinated manner,
* **Agents** model, that separates independent communicating parties and allows for message passing in asynchronous fashion,
* **Atoms** system, being similar to agents, but with the difference in synchronous communication,
* **Dynamic var**, which isolates modifiable state within the communicating threads.

With its distinctive nature, Clojure settled down in code-bases in a number of companies, including {{{clojurestories}}}:

* **Puppet Labs**, that used Clojure for building *Trapperkeeper* - an open source framework for hosting services which are supposed to run for a long period,
* **Beanstalk**, whose product is a continuous integration application, after switching to Clojure gained a multiply of 20 boost in performance for their caching component,
* **ThoughtWorks**, a consulting company, which gave Clojure a try to rewrite their CMS solution, found out that the language helped them to deliver the software before deadline and under designated budget,
* **Sonian**, that deals with archiving emails in cloud, employs Clojure for its core back-end components and claims that thanks to such solution is able to ship new features very quickly,
* **MailOnline**, which is the world's biggest newspaper website, decided to rebuild old service on top of Clojure and did no regret that choice.

### Erlang

First version of Erlang was released back in 1986. 
Erlang evolution led to release of stable version 18.0 at the time of the writing.
Aim of creating Erlang language was to enhance telephony software industry.
Origin of the language's name is ambiguous: some claim that it is a tribute to danish mathematician Agner Krarup Erlang, while other say that it is an abbreviation of "Ericsson language".
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
While Erlang was designed with functional approach in mind and it mainly consists of functional constructs, it is not pure with respect to implementation of a single process which can follow imperative techniques.

Besides Ericsson, Erlang contributed to plenty of success stories for world-class companies' production systems {{{cesarini2009erlang}}}:

* **Amazon** makes use of Erlang for its database component SimpleDB, that is a building block of Amazon Elastic Compute Cloud (EC2),
* **Yahoo!** employs Erlang for its product called Delicious, a bookmarking service that deals with millions of users and hundreds of millions of bookmarks,
* **Facebook** powers the back-end services of its chat service with Erlang, delivering messages to people all around the world,
* **T-Mobile** uses the language to support their Short Message Service (SMS), as well as authentication services,
* **Motorola** utilizes Erlang in public-safety sector for analyzing and processing telephone calls.

Profitable areas
----------------

* machine learning,
* financial institutions,
* insurance companies,
* power companies,
* scripting.

Successful examples
-------------------