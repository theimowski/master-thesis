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

The oldest functional languages like LISP or Scheme are deliberately not included in the list, as they are not really widely used.
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

Strictness in context of static typing means that in order to compile a program, a number of prerequisites must be met.
While it makes harder for a software engineer to compile code in strongly statically typed language, a priceless benefit of program correctness is usually gained.

Usage

### OCaml

### F#

### Scala

### Clojure

### Erlang

Profitable areas
----------------

Successful examples
-------------------