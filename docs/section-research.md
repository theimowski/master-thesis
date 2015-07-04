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
The tutorial would guide step by step on how to build an E-commerce website called "Music Store".
In the "Music Store" user could browse music albums by genres, add his favorite to cart and buy.
Apart from that, the application would show a plenty of other common aspects of Web Development, such as:

* Data access
* Create, Update, Delete operations 
* Authorization
* Managing user's state in cookies
* HTML rendering

Implementing such application and preparing tutorial turned out to be a great candidate for research part of this thesis.

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
It can be said that `map` function is polymorphic of parametric type `(a -> b, a list) -> b list`.

Music Store Tutorial
--------------------

The example application is built on top of the Suave.IO {{{suave}}} framework, which allows to write functional server software in a composable fashion.
Here, composable means that very granular functions / components can be easily joined to create more robust functions.
The resulting functions can be then again glued together.
Following the pattern, one can get eventually a fully working software, which in practice turns out to be a function.

### WebPart

The most important building block in Suave.IO is **WebPart**.
It is a basic unit of composition in this framework - when combining two smaller WebParts together, another WebPart gets created.
WebPart is a **type alias** for the following: 

```fsharp
HttpContext -> Async<HttpContext option>```

The above notation describes a function from `HttpContext` to `Async<HttpContext option>`.
`HttpContext` is a type that contains all relevant information regarding HTTP request, HTTP response, server environment and user state.
The return type for WebPart function is `Async` with type parameter of `HttpContext option`.
`Option` is another generic type - here the type parameter is `HttpContext`.
F# syntactic sugar has been used for `Option` in type declaration: `HttpContext option` is equivalent to `Option<HttpContext>`.

The `Option` type (also known as `Maybe` in different functional languages) is a better alternative to the infamous `null` concept, which is ubiquitous in Object-Oriented world.
In C# for example, every reference type can have a legal value of `null`.
This is the cause of what is known as "The Billion Dollar Mistake" - Null References.
Null References are exceptions thrown at runtime because of referencing a symbol which was not assigned any real value (was `null`).
In F#, one cannot explicitly bind `null` to any symbol or pass `null` to a function invocation.
The compiler prevents from doing that by issuing a compile-time error.
Thanks to that, it is hardly possible to get a Null Reference exception in F# code (when no interoperability or reflection is used).

`Option` type is commonly in F# used to model a property that may or may not have a value.
If a property has value, then it is `Some`:

```fsharp
let x: int option = Some 28 (* there is value 28 *)```

Otherwise, it is `None`:

```fsharp
let x: int option = None (* there is no value *)```

In context of WebPart, `Option` determines whether a result should be applied.
Usually if a WebPart can return `None`, this WebPart is composed with another which always returns `Some`.

To summarize the WebPart type, it can be defined as a function that for a given `HttpContext` may or may not apply a specific, updated `HttpContext`.
In addition to that, the return value is surrounded with asynchronous computation (`Async`) making the WebPart function asynchronous-friendly.

The simplest possible WebPart can be defined following:

```fsharp
let webPart = OK "Hello World!"```

The `OK` WebPart always "succeeds" (returns `Some`) and writes to the HTTP response:

* 200 OK status code
* "Hello World!" response body content

Such WebPart can now be used to start an HTTP server using default configuration:

```fsharp
startWebServer defaultConfig webPart```

From the above snippets it is evident that Suave.IO allows to build Web applications in a very succinct way and does not require too much ceremony.

### Routing

Routing is a basic concept of Web Development.
It allows to delegate request handling to a specific component based on the request URL path.
Below is a snippet which shows how routing for the Music Store can be defined in Suave:

```fsharp
let browse =
    request (fun r ->
        match r.queryParam "genre" with
        | Choice1Of2 genre -> OK (sprintf "Genre: %s" genre)
        | Choice2Of2 msg -> BAD_REQUEST msg)

let webPart = 
    choose [
        path "/" >>= (OK "Home")
        path "/store" >>= (OK "Store")
        path "/store/browse" >>= browse
        pathScan "/store/details/%d" (fun id -> OK (sprintf "Details: %d" id))
    ]```

Lines 8-13 show how 4 different WebParts are composed together with `choose` function.
`choose` is of type `WebPart list -> WebPart`.
It tries to apply in order each WebPart from the list until it finds one that returns `Some`.
If none element returns `Some`, the `choose` function itself returns `None`.

To detect if the incoming request URL path matches specific route, `path` function can be used.
Type of `path` is `string -> WebPart` and the function returns `Some` if URL path matches the `string` parameter.

In lines 9-11 `>>=` operator (commonly known as "bind" operator in functional jargon) applies the right-hand side operand only if the left-hand side operand evaluates to `Some`.

Query parameters in URL are often used to pass arguments to an HTTP call.
Defined in lines 1-5, `browse` WebPart enables to extract name of a genre from URL like this: `/store/browse?genre=Disco`.
Under the hood it uses `request` function of type `HttpRequest -> WebPart` to reach the query string.
If the query string contains "genre" key (line 4), `OK` response is returned with the genre name in response body.
If the query string does not contain "genre" key (line 5), `BAD_REQUEST` response (HTTP status code 400) is returned, together with adequate message.

Another popular way of passing arguments to an HTTP call is encoding them into the URL itself.
To handle this scenario, Suave comes with a great feature called "Typed Routes".

TODO: typed routes + sprintf
TODO: Pattern matching
TODO: How Routing would be handled in MVC C#

Conclusions
-----------