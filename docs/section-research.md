RESEARCH
========

Previous section described how Functional Programming managed to be successfully applied in many projects.
In this section focus will be laid on creating software that uses functional techniques.
For that purpose, process of developing such application will be shown.
In the course of this section, multiple comparisons will be made between Object-Oriented and Functional approaches.
F# programming language will be used for implementing the application.

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
The detailed tutorial has been published and is available on-line {{{suavemusicstoretutorial}}}.

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
The same syntactic sugar can also be used for sequences (`'a seq`) or lists (`'a list`).

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
Below is a snippet which shows how routing for the Music Store can be implemented in Suave:

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
It tries to apply each WebPart from the list in order until it finds one that returns `Some`.
If none element returns `Some`, the `choose` function itself will also return `None`.

To detect if the incoming request URL path matches specific route, `path` function can be used.
Type of `path` function is `string -> WebPart`.
The function returns `Some` if URL path matches the `string` parameter.

In lines 9-11, there is a `>>=` operator (commonly known as "bind" operator in functional jargon).
It applies the right-hand side operand only if the left-hand side operand evaluates to `Some`.
This means, that for example `(OK "Home")` will be applied if `path "/"` returns `Some`.

Query parameters in URL are often used to pass arguments to an HTTP call.
Defined in lines 1-5, `browse` WebPart enables to extract name of a genre from URL.
For `/store/browse?genre=Disco` URL `browse` will recognize "Disco" value for "genre" argument.
Under the hood, to reach the query string, `request` function (of type `HttpRequest -> WebPart`) is used.
If the query string contains "genre" key (line 4), `OK` response is returned with the genre name in response body.
If the query string does not contain "genre" key (line 5), `BAD_REQUEST` response (HTTP status code 400) is returned, together with adequate message.
The patterns used to determine if "genre" key is present, `Choice1Of2` and `Choice2Of2`, are the only two possible values of type `Choice<'T1,'T2>`. 
The `Choice` type is generic with two type parameters.
It is often used to model computations that may succeed or fail.
In this example, the `Choice` drags along the genre name in case of success, and a failure message otherwise.

Another popular way of passing arguments to an HTTP call is encoding them into the URL itself.
To handle this scenario, Suave comes with a powerful feature called **Typed Routes**.
Typed Routes provide a type-safe solution to parsing URL arguments.
Example of using Typed Routes is shown in line 12.
The `pathScan` function takes a raw string as its first argument, however the compiler treats this value in a special way.
Because the string contains a literal `%d`, compiler expects an integer value in place of that literal.
As a result, the second argument to the `pathScan` function, must be of type `int -> WebPart`.
Thanks to such behavior, accidental type mismatch can be prevented.
That is yet another example of the power of strong static typing in F#.

It is worth noting, that `sprintf` function (lines 4 and 12) makes use of the same compiler feature.
In line 4, the `genre` value is of type `string`, so the string literal for `sprintf` contains `%s`.
On the other hand, in line 12, the `id` value is of type `int` - here the string literal contains `%d`.

#### Pattern matching

Lines 3-5 present construct which is called pattern matching.
The construct is part of syntax of several programming languages, both imperative and declarative.
For developers familiar with C# language, pattern matching could be explained as a switch statement in conjunction with an assignment to a symbol in each branch.
However, there is more than that to pattern matching.
In F# pattern matching issues a warning in compile-time, if not all possible branches of execution are defined.
As an example, if line 5 from the above snippet was missing, the compiler would warn about possible unmatched case (`Choice2Of2`).

#### Routing in ASP.NET MVC

Routing in ASP.NET MVC framework is handled with what is known as "Convention over Configuration".
The term here means, that instead of declaring handlers in code, some convention is adopted.
ASP.NET MVC convention for routing works by prefixing type name of a Controller with the corresponding name of the route.
As an example, `HomeController` will match requests to "/Home" resource.

Passing arguments to Controllers in ASP.NET MVC is based on the "Model Binding" concept.
The concept usually relies on attribute annotations or reflection, however it does not deliver such type-safety as Suave does.

While debate continues on whether "Convention over Configuration" is convenient to use, WebPart composition in Suave together with benefits of statically typed arguments seem to be a competitive alternative to ASP.NET MVC with regards to the Routing concept.

### HTML rendering

When developing Web applications that aim to be consumed by browsers, rendering HTML views is an important aspect to consider.
Nowadays Internet applications happen to be rich on the client side.
HTML markup with attached Cascade StyleSheets and JavaScripts are becoming more and more complex.
Non-trivial logic and rules for displaying HTML pages lead to formation of multiple template engines, both those which render on client and server side.
The problem of HTML rendering is complicated, and can be approached with various different solution.
The thesis will only touch upon how it is possible to solve HTML rendering aspect in Suave framework.
It will not however go into much details of HTML rendering or try to compare to different available possibilities.

#### Suave HTML DSL

For the Music Store rendering engine, a simple to use and built into Suave **DSL** has been chosen. Domain Specific Language has plenty of definitions, one of which is following {{{van2000domain}}}:

>> A domain-specific language (DSL) is a programming language or executable specification language that offers, through appropriate notations and abstractions, expressive power focused on, and usually restricted to, a particular problem domain.

Based on the above definition, HTML DSL available in Suave could be categorized as a set of functions focused on building HTML markup. 
HTML markup can be a valid XML markup, under the condition that all element tags are closed.
In fact, the HTML DSL in Suave relies on creating XML tree and formatting it to plain text.

XML tree rules are simple (set of XML node types has been reduced to the most common): 

* Element node can have 0 or more children
* Attribute node contains key and value but cannot have children
* Text node contains only value and cannot have children

Below follows a code snippet that shows how a basic HTML page for Music Store has been defined:

```fsharp
let index = 
    html [
        head [
            title "Suave Music Store"
        ]

        body [
            divId "header" [
                h1 (aHref "/" (text "F# Suave Music Store"))
            ]

            divId "footer" [
                text "built with "
                aHref "http://fsharp.org" (text "F#")
                text " and "
                aHref "http://suave.io" (text "Suave.IO")
            ]
        ]
    ]```

Result of the above snippet is the following HTML markup:

```html
<html>
    <head>
        <title>Suave Music Store</title>
    </head>
    <body>
        <div id="header">
            <h1>
                <a href="/">F# Suave Music Store</a>
            </h1>
        </div>
        <div id="footer">built with <a href="http://fsharp.org">F#</a> and <a href="http://suave.io">Suave.IO</a></div>
    </body>
</html>```

The DSL usage looks similar to how the actual markup is defined.
Thanks to that it should be relatively easy, for an HTML developer who is not familiar with F#, to get started with designing views in such fashion.

As the example showed static content, no real benefits were gained from using a DSL instead of plain HTML.
Such benefits arise when there is some kind of data model to be displayed in a view.
In Music Store, this can be demonstrated by rendering page for list of genres:

```fsharp
let store genres = [
    h2 "Browse Genres"
    p [
        text (sprintf "Select from %d genres:" (List.length genres))
    ]
    ul [
        for g in genres -> 
            li (aHref (Path.Store.browse 
                       |> Path.withParam (Path.Store.browseKey, g)) (text g))
    ]
]```

The `store` function in snippet returns a list of nodes, which can then be passed as an argument to the `index` function defined earlier and displayed in a container between header and footer.
First node (line 2) is a standalone `h2` element with "Browse Genres" sign.

The second node (lines 3-5) is a paragraph that displays number of all genres in the Store.
F# core library function, `List.length` has been used to count the number of elements in `genres`.
Thanks to the usage of this function, F# compiler is capable of inferring the type of `genres` value to be `'a list` (it does not specify the type parameter for the `list` though).

The last node returned (lines 6-10) is `ul` which stands for "unordered list".
Unordered list contains `li` "list items" elements, each of which has a hyper-link to the corresponding genre.
Line 7 demonstrates "list comprehension" syntax, which binds each element of the `genres` list to `g` value and uses it to yield list items.
With help of `Path` module, lines 8-9 generate a hyper-link with URL argument, similar to the one presented in Routing section (`/store/browse?genre=Disco`). 

Because `text` function, which is invoked at the end of line 9 expect a `string` as its argument, `g` value is inferred to be of type `string`.
Powerful type-inference mechanism in F# is thus able to determine that the type of `genres` argument is `string list`.
Thanks to the type-inference, no explicit type annotations are required, and the code is more concise.

Two great benefits from using such a DSL for rendering HTML can be enlisted:

* Type system can prevent common bugs in compile-time
* Every language syntax construct can be used in the DSL, making it easier to express complex logic

### Data Access

Data Access is yet another important aspect of software development in any domain, including Web.
There is a wide variety of options, when it comes to choose how to persist data.
Common classification of the options consists of two main classes: 

* Relational databases
* No-SQL databases: graph databases, key-value stores, document databases, etc.

There is even one approach that can be associated with functional programming, which is **Event Sourcing**.
It relies on persisting **immutable** events in a store.
Immutability is one of the major concept of functional programming itself, that is why Event Sourcing feels like a good fit for functional paradigm.
In order to obtain certain state in Event Sourcing, **fold** operation is performed on the stored events.
Folding comes also from the functional background.
In fact, every functional programming has `fold` function in its suite:

```fsharp
List.fold : ('State -> 'T -> 'State) -> 'State -> 'T list -> 'State```

`List.fold` calculates `'State` from the initial `'State` and list of `'T` elements.
It takes 3 arguments (enlisted in reverse order):

* `'T list` - a list of `'T` elements
* `'State` - initial state
* `('State -> 'T -> 'State)` - "folder" function which is applied on each `'T` element of the list, and based on the value of the element and the `'State` it produces new `'State.

While the Event Sourcing would be a good candidate for persistence mechanism in application written in functional language, it was not used for the Music Store.
That's because despite recent peak in interest in the No-SQL movement, a relational database still seems to be most popular for most of the software engineers.
For that reason MS SQL Server has been chosen as a persistence mechanism for Music Store application.

#### F# Type Providers

During the computer era, a number of data serialization formats have evolved, some of which became standard in the industry.
XML, Json, CSV are just the beginning of the long list.
Majority of software, no matter the domain, processes some kind of data from different sources.
It is therefore a very common task in a development process to parse structured input into an in-memory object model.
This task requires proper type hierarchy and parsing logic to be implemented.
In addition to the fact that such task is time consuming, it may also be error-prone when the format changes.

To meet the challenge, F# comes with another powerful feature, called **Type Providers**.
Type Providers automatically generate (provide) a set of types and parsing logic for a given schema.
For example, given a literal XML string, F# type provider will generate in compile-time a separate type for each corresponding element from the XML.
With the types in place, another literal XML string that fulfills the schema, can be loaded and parsed into the object model with one line of code.

Type Provider libraries for the most popular standards are ready to use.
In addition to that, the mechanism is extensible which means that Type Providers can be created for arbitrary data source.

The feature is quite unique - apart from Idris, no other programming language has Type Providers
(In fact, Type Providers in Idris are even more rich in functionality than those in F# {{{christiansen2013dependent}}}).

#### SQL Provider

Among various Type Provider libraries there is one called **SQL Provider**. 
As the name suggests, it provides types for a relational database schema.
This means that no Object-Relational Mapping libraries or hand-rolled models are needed to implement Data Access Layer.
To use SQL Provider in Music Store, it is only necessary to lead a SQL connection string for the Type Provider:

```fsharp
type Sql = 
    SqlDataProvider< 
        "Server=(LocalDb)\\v11.0;Database=SuaveMusicStore;Trusted_Connection=True", 
        DatabaseVendor=Common.DatabaseProviderTypes.MSSQLSERVER >```

This code executes proper queries against the given connection and generates corresponding in the background.
In Music Store, types for both database tables and views will be used.
Following is a snippet for defining type aliases for the generated (provided) types:

```fsharp
// database context
type DbContext      = Sql.dataContext

// database tables
type Album          = DbContext.``[dbo].[Albums]Entity``
type Artist         = DbContext.``[dbo].[Artists]Entity``
type Genre          = DbContext.``[dbo].[Genres]Entity``
type User           = DbContext.``[dbo].[Users]Entity``
type Cart           = DbContext.``[dbo].[Carts]Entity``

// database views
type AlbumDetails   = DbContext.``[dbo].[AlbumDetails]Entity``
type CartDetails    = DbContext.``[dbo].[CartDetails]Entity``
type BestSeller     = DbContext.``[dbo].[BestSellers]Entity```

Type aliases such as `Album`, `Artist`, `Genre` are defined for corresponding database tables (lines 5-9).
The last 3 type aliases (lines 12-14) represent database views which will be used in Music Store.

Following is a snippet demonstrating how certain queries can be constructed with SQL Provider:

```fsharp
let firstOrNone s = s |> Seq.tryFind (fun _ -> true)

let getGenres (ctx : DbContext) : Genre list = 
    ctx.``[dbo].[Genres]`` |> Seq.toList

let getAlbumsForGenre genreName (ctx : DbContext) : Album list = 
    query { 
        for album in ctx.``[dbo].[Albums]`` do
            join genre in ctx.``[dbo].[Genres]`` on (album.GenreId = genre.GenreId)
            where (genre.Name = genreName)
            select album
    }
    |> Seq.toList

let getAlbumDetails id (ctx : DbContext) : AlbumDetails option = 
    query { 
        for album in ctx.``[dbo].[AlbumDetails]`` do
            where (album.AlbumId = id)
            select album
    } 
    |> firstOrNone```

Function defined in line 1 `firstOrNone` is of type `'t seq -> 't option`.
It returns `Some` with the value of first element if any in the given sequence.
If the sequence is empty, `firstOrNone` returns `None`.

`getGenres` (line 3) is a simple query that fetches all `Genre`s from the database and returns them as a `Genre list` type.

Lines 7-12 and 16-20 show usage of `query` expression.
The concept is known as LINQ (Language Integrated Query).
The (approximate) intended semantics of the LINQ-SQL libraries is that the execution of the meta-programs should be the same as if the programs were converted to equivalent programs over in-memory lists, where the database table is treated as an in-memory enumerable data structure {{{syme2006leveraging}}}.
Indeed, the semantics of above snippet looks very similar to a standard SQL query.

`getAlbumsForGenre` function returns a list of all `Album`s that are associated with the given `Genre`.
The `Genre` is identified here by its name (`string` type).
The last function, `getAlbumDetails` tries to find an `Album` with given `id` in context of the `AlbumDetails` view - the database view performs a join with two other tables to obtain the details for the album.
There may be no album with a given `id`, hence the function's return type is `AlbumDetails option`

#### Summary

Data Access Layer implementation is easily achievable in F#.
As far as the persistence mechanism remains the same, details of the implementation do not differ drastically comparing to the Object-Oriented or imperative world.
Type Providers in F#, together with its type-safe nature allow for convenient data access logic.
In conjunction with Intellisense feature of Integrated Development Environment (IDE) and Language Integrated Query, writing Data Access code in F# is blazingly fast and little error-prone.

Conclusions
-----------