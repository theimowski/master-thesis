APPLICATION OF FUNCTIONAL PROGRAMMING
========

Section {{FUNCTIONAL PROGRAMMING IN INDUSTRY}} described how FP managed to be successfully applied in many projects.
In this section focus is laid on creating software that uses functional techniques.
For that purpose, process of developing such application is shown.
In the course of this section, multiple comparisons are made between functional and OOP approaches.
F# programming language is used for implementing the sample application.

Domain choice
-------------

FP is already present in a significant amount of areas. 
There are however fields, where it is still not widely adopted, and one of such fields is **web development**. 
Here, OOP seems to be a strong leader with regards to number of existing frameworks, libraries and tools as well as popularity among software engineers. 
While some FP languages together with corresponding libraries / frameworks are starting to settle down in the area, like Clojure with Compojure, Scala with Play or Erlang with Webmachine {{{github}}}, F# does not yet go out of line in this regard. 
At the moment of writing, there are three notable web libraries / frameworks written completely in F# {{{fsharpwebsite}}}: WebSharper, Suave.IO and Freya, but none of them is prominent in respect to popularity.
In order to verify whether FP and F# language in particular could potentially be applied to software from arbitrary domain, decision was made to discover how it would fit into this relatively foreign area of web development.

Another reason for which web development domain was chosen, was a will to contribute to the F# community.
In order to attract more and more attention to F# language, the language community leaded by F# Software Foundation {{{fsharpwebsite}}} is doing its best to encourage developers to give F# a try.
On one of mailing groups there appeared a suggestion to prepare a tutorial on how to create web applications with F#.
The tutorial would guide step by step on how to build an E-commerce website called "Music Store".
In the "Music Store" user could browse music albums by genres, add his favorite to cart and buy.
Apart from that, the application would show a plenty of other common aspects of web development, such as:

* Data access,
* Create, Update, Delete operations,
* Authorization,
* Managing user's state in cookies,
* HTML rendering.

Implementing such application and preparing a tutorial turned out to be a great candidate for research part of this thesis.
The detailed tutorial has been published and is available on-line {{{suavemusicstoretutorial}}}.
In addition to that the tutorial is included in the list of tutorials for web development on F# Software Foundation website {{{fsharpwebsite}}}.
Credit also goes to the ASP.NET MVC team, which prepared the original tutorial {{{aspnetmvctutorial}}} that largely influenced the variation.
ASP.NET MVC is a C#-based, heavy-weight framework for creating web applications in .NET ecosystem.
MVC acronym stands for Model-View-Controller pattern, which aims to separate components for layout rendering, data objects behavior and handling user's interaction.
Because both F# and C# reside in the .NET platform, and because ASP.NET MVC is considered an OOP framework, this section continuously relates to how a similar functionality could be achieved in the counterpart.
Thanks to the initial work, it was easier to compare certain aspects and draw conclusions.

Functional Web
--------------

Even though web development is usually associated with using imperative techniques only, it's not completely uncommon to follow functional principles while creating Internet applications.
Majority of web applications are built on top of the Hypertext Transformation Protocol (HTTP) protocol.
From a software engineer point of view, the HTTP protocol boils down to requests and responses and accompanied details, such as paths, status codes, headers or content body.
The HTTP protocol is stateless, meaning that details about requests are not retained by the protocol and thus there is no ad hoc relation between two requests.
Because of the stateless nature of the underlying protocol, HTTP response should only depend on the issued request, and therefore a functional programmer could even think of a web application as a general function of type `HTTPRequest -> HTTPResponse`.

### Asynchrony

When dealing with Internet applications in practice, it turns out that aspect of asynchrony plays a crucial role.
IO operations are extremely ineffective if performed synchronously, as synchronous execution of such tasks results in thread blocking.
When threads are blocked, incoming requests that follow have to engage more threads, because the blocked ones are in use.
As a result, having a significant number of blocked threads results in high memory consumption and ineffective usage of resources.

Writing software in asynchronous fashion does not come for free.
Callback-passing style is one of the most popular techniques for writing asynchronous code.
While the technique may be a sufficient solution for smaller problems, when applied to large and complex systems, code very easily gets hard to maintain.

Among various approaches to asynchronous programming, there is one that abstracts away the concept of asynchrony from the actual flow of program.
In F# it is called **asynchronous workflow** and is represented with generic `Async` type.
The term is also known as **future**, **promise** or **delay** in other programming languages.
This approach bypasses callbacks in a clever way, resulting in code that is easier to read and reason about.
Thanks to abstracting time-consuming calls within the `Async` objects, they can be easily combined together.
As a result, multiple actions can be implemented without the need of callbacks, but still with the availability of utilizing asynchrony.
Asynchronous workflow represents a computation that is supposed to evaluate to a value of the type parameter of `Async`.
Value inside the `Async` type can happen to be available at once, but usually the deferred execution of `Async` leads to evaluating the inner value.
From the developer's perspective, extracting the internal state of `Async` object is transparent, as it is always necessary to eventually invoke a blocking call to "unwrap" the value from `Async`.
The general concept of "wrapping" a value with a cross-cutting structure originates from the functional paradigm.
Asynchronous workflows or futures and can prove helpful when developing client-server architecture {{{eriksen2013your}}}:

>> *"Futures in conjunction with services and filters present a powerful programming model for building safe, modular, and efficient server software."*

Music Store Tutorial
--------------------

The example application was built on top of the Suave.IO (Suave) {{{suave}}} framework, which allows to write functional server software in a composable fashion.
Here, composable means that very granular functions / components can be easily joined to create more robust functions.
The resulting functions can be then again glued together to create even bigger functions.
Following this pattern, one can get eventually end up with a complex function (built from composing smaller ones) which can serve as a complete software.

### WebPart

The most important building block in Suave is **WebPart**.
It is a basic unit of composition in the framework - when combining two smaller WebParts together, another WebPart gets created.
Technically, WebPart is a **type alias** for a function (functions as first-class citizens can be represented by types) presented in listing {{fswebpartalias}}. 

The notation used in listing {{fswebpartalias}} describes a function from `HttpContext` to `Async<HttpContext option>`.
The `HttpContext` is a type that contains all relevant information regarding HTTP request, HTTP response, server environment and user state.
The return type is `Async` with type parameter of `HttpContext option`.
`Option` is also a generic type - here the type parameter for `Option` is `HttpContext`.
F# syntactic sugar has been used for `Option` in type declaration: `HttpContext option` is equivalent to `Option<HttpContext>`. 
The same syntactic sugar can also be used for sequences (`'a seq`) or lists (`'a list`).

```xxx
{FSharp]{Type alias for WebPart}{fswebpartalias}
type WebPart = HttpContext -> Async<HttpContext option>```

The `Option` type (also known as `Maybe` in different functional languages) is a better alternative to the infamous `null` concept, which is ubiquitous in OOP world.
In C# for example, every reference type can have a legal value of `null`.
This is the cause of what is known as "The Billion Dollar Mistake" - null references.
Null references are exceptions thrown at runtime because of referencing a symbol which was not assigned any real value (was `null`).
In F#, one cannot explicitly bind `null` to any symbol or pass `null` to a function invocation.
The compiler prevents from doing that by issuing a compile-time error.
Thanks to this feature, it is hardly possible to get a null reference exception in F# code (unless heavily relying on interoperability or reflection).
`Option` type is commonly used in F# to model a property that may or may not have a value.
If a property has value, then it is `Some`, and otherwise it is `None`, as shown in listing {{fsoption}}.
In context of WebPart, `Option` determines whether a result should be applied.
Usually if a WebPart can return `None`, this WebPart is composed with another which eventually returns `Some`.

```xxx
{FSharp]{FSharp Option type}{fsoption}
let x: int option = Some 28 (* there is value 28 *)
let y: int option = None (* there is no value *)```

The simplest possible WebPart can be defined as in line 1 of listing {{fswebparthello}}.
The `OK` WebPart always "succeeds" (returns `Some`) and writes to the HTTP response 200 OK status code, as well as "Hello World!" response body content.
Such WebPart can be used to start an HTTP server using a default configuration (line 2 of listing {{fswebparthello}}).
From the listing {{fswebparthello}}, it is evident that Suave allows to build Web applications in a very succinct way and does not require too much ceremony.

```xxx
{FSharp]{WebPart - hello world example}{fswebparthello}
let webPart = OK "Hello World!"
startWebServer defaultConfig webPart```

To summarize the WebPart type, it can be defined as a function that for a given `HttpContext` may or may not apply a specific, updated `HttpContext`.
In addition to that, the return value is surrounded with asynchronous computation (`Async`) making the WebPart function asynchrony-friendly.

### Routing

Routing is a basic concept of web development.
It allows to delegate request handling to a specific component based on the request URL path.
Listing {{fssuaverouting}} shows how routing for the Music Store was implemented in Suave.

```xxx
{FSharp]{Routing in Suave}{fssuaverouting}
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
If none element returns `Some`, the `choose` function itself also returns `None`.
To detect if the incoming request URL path matches specific route, `path` function of type `string -> WebPart` can be used (line 9, 10, 11).
The function returns `Some` if URL path matches the `string` parameter.
Result of `path` function is applied to "bind" operator, denoted by `>>=`.
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
Similarly in line 12, the `id` value is of type `int`, hence the string literal here contains `%d`.

Routing in ASP.NET MVC framework is handled with what is known as "Convention over Configuration".
The term here means, that instead of declaring handlers in code, some kind of convention is adopted.
ASP.NET MVC convention for routing works by prefixing type name of a Controller with the corresponding name of the route.
As an example, `HomeController` will match requests to "/Home" resource.
Passing arguments to Controllers in ASP.NET MVC is based on the "Model Binding" concept.
The concept usually relies on attribute annotations or reflection.
It does does not provide such type-safety as Suave does, which means that variety of type mismatch errors could be thrown at runtime.

While debate continues on whether "Convention over Configuration" is convenient to use, WebPart composition in Suave together with benefits of Typed Routes seem to be a competitive alternative to ASP.NET MVC with regards to the Routing concept.

### HTML rendering

When developing Web applications that aim to be consumed by browsers, rendering HTML views is an important aspect to consider.
Nowadays Internet applications happen to be rich on the client side.
HTML markup with attached cascade style sheets (CSS) and Javascripts are becoming more and more complex.
Non-trivial logic and rules for displaying HTML pages led to formation of multiple template engines, both those which render on client side and those that do the processing on server side.
The problem of HTML rendering is complicated, and can be approached in many different ways.
This thesis will only touch upon how HTML rendering aspect was solved with Suave framework for the Music Store application.
It will not however go into much details of HTML rendering topic itself nor try to compare chosen approach with different available possibilities.

For the Music Store rendering engine, a simple to use and built into Suave HTML DSL was chosen.
The HTML DSL available in Suave could be categorized as a set of functions focused on building HTML markup. 
HTML markup can be a valid XML markup, under the condition that all element tags are closed (actually not all valid XML documents are proper HTML documents, due to the self-closing tag concept, which is forbidden in HTML markup).
Indeed the HTML DSL in Suave relies on creating XML tree and formatting it to plain text.
Listing {{fssuaveindexpage}} shows how a basic HTML page for Music Store was defined.
In result of evaluating code from listing {{fssuaveindexpage}}, HTML markup presented in listing {{htmlsuaveindexpage}} was returned.
The DSL usage looks similar to how the actual markup is defined.
Thanks to this fact it should be relatively easy for an HTML developer who is not familiar with F# syntax, to get started with designing views with this DSL.

```xxx
{FSharp]{Rendering index page with Suave HTML DSL}{fssuaveindexpage}
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

```xxx
{HTML]{HTML markup for index page}{htmlsuaveindexpage}
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

As the example showed static content, no real benefits were gained from using a DSL as opposed to plain HTML.
Such benefits arise when there is need to display some kind of data model in a view.
In context of Music Store, this can be demonstrated by rendering page for list of genres, as shown in listing {{fssuavestoreview}}.

```xxx
{FSharp]{Rendering page with a given data parameter}{fssuavestoreview}
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

The `store` function in listing {{fssuavestoreview}} returns a list of nodes, which can then be passed as an argument to the `index` function defined earlier and displayed in a container between header and footer.
First node (line 2) is a standalone `h2` element with "Browse Genres" sign.
The second node (lines 3-5) is a paragraph that displays number of all genres in the Store.
F# core library function, `List.length` has been used to count the number of elements in `genres`.
Thanks to the usage of this function, F# compiler is capable of inferring the type of `genres` value to be `'a list` (it does not specify the type parameter for the `list` though).
The last node returned (lines 6-10) is `ul` which stands for "unordered list".
Unordered list contains `li` "list items" elements, each of which has a hyper-link to the corresponding genre.
Line 7 demonstrates "list comprehension" syntax, which binds each element of the `genres` list to `g` value and uses it to yield list items.
With help of `Path` module, lines 8-9 generate a hyper-link with URL argument, similar to the one presented in {{Routing}} (`/store/browse?genre=Disco`). 
Because `text` function, which is invoked at the end of line 9 expect a `string` as its argument, `g` value is inferred to be of type `string`.
Powerful type-inference mechanism in F# is thus able to determine that the type of `genres` argument is `string list`.
Thanks to the type-inference, no explicit type annotations are required, and the code is more concise.

While a disadvantage of this approach is that the productivity suffers during development because any change to the view has to be recompiled, 
two great benefits from using such a DSL for rendering HTML can be enlisted:

* Type system can catch common type mismatch issues in compile-time,
* Every language syntax construct can be used in conjunction with the DSL (as seen with the list comprehension), making it easier to express complex logic.

### Data Access

Data Access is yet another important aspect of software development in any domain, including web development.
There is a wide variety of options, when it comes to choose how to persist data.
One common classification of these options boils down to two alternatives: 

* Relational databases,
* No-SQL databases: graph databases, key-value stores, document databases, etc.

Among plethora of No-SQL solutions, there is one approach that can be especially associated with FP named **Event Sourcing**.
It relies on persisting **immutable** events (records) in a store.
Immutability is one of the major concept of FP itself, that is why Event Sourcing feels like a good fit for functional paradigm.
In order to obtain certain state in Event Sourcing, **fold** operation is performed on the stored events.
Folding comes also from the functional background {{{hughes1989functional}}}.
In fact, every FP defines `fold` in its core library function suite.
Type signature for the one in F# is presented in listing {{fsfoldsignature}}.
`List.fold` calculates result `'State` from the initial `'State` and list of `'T` elements.
It takes 3 arguments (enlisted in reverse order):

* `'T list` - a list of `'T` elements,
* `'State` - initial state,
* `('State -> 'T -> 'State)` - "folder" function which is applied on each `'T` element of the list, and based on the value of the element and the intermediate `'State` it produces new `'State.

```xxx
{FSharp]{Fold function signature in FSharp}{fsfoldsignature}
List.fold : ('State -> 'T -> 'State) -> 'State -> 'T list -> 'State```

While the Event Sourcing could be a good candidate for persistence mechanism in application written in functional language, it was not used for the Music Store.
That is because despite recent peak in interest in the No-SQL movement, a relational database still seems to be most popular for majority of software engineers.
In order for the tutorial not to distract attention with different concepts, MS SQL Server was chosen as a persistence mechanism for Music Store application.

During the computer era, a number of data serialization formats have evolved, some of which became standard in the industry.
XML, Json, CSV are just the beginning of the long list.
Majority of software, no matter from which domain, processes some kind of data from different sources.
It is therefore a very common task in a development process to parse structured input into an in-memory object model.
This task requires proper type hierarchy and parsing logic to be implemented.
In addition to the fact that such task can be time consuming, it may also be error-prone when the format changes.
To meet the challenge, F# comes with another powerful feature, called **Type Providers**.
Type Providers automatically generate (provide) a set of types and parsing logic for a given schema.
For example, given a literal XML string, F# type provider will generate in compile-time a separate type for each corresponding element from the XML.
With the types in place, another literal XML string that fulfills the schema, can be loaded and parsed into the object model within a single line of code.

Type Provider libraries for the most popular standards are ready to use.
The mechanism is extensible which means that Type Providers can be created for arbitrary data source.
The feature is quite unique - apart from Idris, no other programming language has Type Providers (in fact, Type Providers in Idris are even more rich in functionality than those in F# {{{christiansen2013dependent}}}).
Among various Type Provider libraries there is one called **SQL Provider**. 
As the name suggests, it provides types for a relational database schema.
This means that no object-relational mapping libraries or hand-rolled models are needed to implement data access layer.
To use SQL Provider in Music Store, it was only necessary to deliver a SQL connection string to the Type Provider, as shown in listing {{fssqlproviderdef}}.
Code in listing {{fssqlproviderdef}} executes proper queries against the given connection and generates necessary types in the background.

```xxx
{FSharp]{Providing types for SQL connection in FSharp}{fssqlproviderdef}
type Sql = 
    SqlDataProvider< 
        "Server=(LocalDb)\\v11.0;Database=SuaveMusicStore;Trusted_Connection=True", 
        DatabaseVendor=Common.DatabaseProviderTypes.MSSQLSERVER >```

In Music Store, types for both database tables and views were used.
Listing {{fssqlprovideraliases}} presents definition of type aliases for the generated (provided) types.
Type aliases such as `Album`, `Artist`, `Genre` are defined for corresponding database tables (lines 5-9).
The last 3 type aliases (lines 12-14) represent database views which proved useful in Music Store.

```xxx
{FSharp]{Defining type aliases for SQL Provider}{fssqlprovideraliases}
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

Database queries can be constructed with SQL Provider like shown in listing {{fssqlproviderqueries}}.
Function defined in line 1 `firstOrNone` is of type `'t seq -> 't option`.
It returns `Some` with the value of first element if any in the given sequence.
If the sequence is empty, `firstOrNone` returns `None`.
`getGenres` (line 3) is a simple query that fetches all `Genre`s from the database and returns them as a `Genre list` type.
Lines 7-12 and 16-20 show usage of `query` expression.
The concept is known as LINQ (Language Integrated Query) and can be explained with following Syme's citation {{{syme2006leveraging}}}:

>> *"The (approximate) intended semantics of the LINQ-SQL libraries is that the execution of the meta-programs should be the same as if the programs were converted to equivalent programs over in-memory lists, where the database table is treated as an in-memory enumerable data structure."*

Indeed, the semantics of query expressions in listing {{fssqlproviderqueries}} look very similar to a standard SQL query.
`getAlbumsForGenre` function returns a list of all `Album`s that are associated with the given `Genre`.
The `Genre` is identified here by its name (`string` type).
The last function, `getAlbumDetails` tries to find an `Album` with given `id` in context of the `AlbumDetails` view - the database view performs a join with two other tables to obtain the details for the album.
There may be no album with a given `id`, hence the function's return type is `AlbumDetails option`

```xxx
{FSharp]{Constructing queries with SQL Provider}{fssqlproviderqueries}
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

Data Access Layer implementation is easily achievable in F#.
As far as the persistence mechanism remains the same, details of the implementation do not differ drastically comparing to the OOP or imperative world.
Type Providers in F#, together with its type-safe nature allow for convenient data access logic.
In conjunction with Intellisense feature of Integrated Development Environment (IDE) and Language Integrated Query, writing Data Access code in F# is highly productive and little error-prone.

### Create, Update and Delete operations

From a programmer's point of view, majority of features in any software can be marked as either a **query** or a **command**.
Such generalization refers to concept known as Command and Query Separation.
The term defines queries as something that does not change state, but only fetch certain portion of data from a system.
On the other hand, commands are actions that have side effects because of modifying the internal state of the system {{{meyer1988object}}}.
In {{Data Access}} queries were implemented to fetch basic sets of data regarding music albums or genres.
In this section commands that alter state of Music Store are shown.
As a part of administration management module, following features were implemented in Music Store application:

* **Creating** a new album from scratch by assigning a title, price, artist and genre (the last two being restricted to discrete subset of possible values),
* **Editing** an existing album by modifying any of the properties,
* **Deleting** an album from the Music Store, and therefore making it impossible for users to buy.

```xxx
{FSharp]{Music Store - creating album}{fsmusiccreatealbum}
let createAlbum =
    let ctx = Db.getContext()
    choose [
        GET >>= warbler (fun _ -> 
            let genres = 
                Db.getGenres ctx 
                |> List.map (fun g -> decimal g.GenreId, g.Name)
            let artists = 
                Db.getArtists ctx
                |> List.map (fun g -> decimal g.ArtistId, g.Name)
            html (View.createAlbum genres artists))

        POST >>= bindToForm Form.album (fun form ->
            Db.createAlbum (int form.ArtistId, int form.GenreId, form.Price, form.Title) ctx
            Redirection.FOUND Path.Admin.manage)
    ]```

For the sake of new album feature, code presented in listing {{fsmusiccreatealbum}} was written.
The `createAlbum`, together with its own route was composed into the main WebPart of the Music Store application.
Two HTTP methods were defined as acceptable within `createAlbum`: GET and POST (lines 4 and 13).
The GET method means retrieve whatever information (in the form of an entity) is identified by the Request-URI, while POST method is used to indicate that the server should accept the entity enclosed in the request body {{{fielding1999hypertext}}}.
In context of `createAlbum` WebPart, GET method was used to retrieve HTML form for creating new album.
POST method was further used to send the album in request body to the origin server for the purpose of saving in the database.

Because the right-hand side of `>>=` operator is evaluated eagerly, GET WebPart was wrapped with a `warbler` function in line 4.
This was done to delay the logic execution inside WebPart (in that case fetching list of `genres` and `albums` from the database) until the request method is recognized as GET.
The `warbler` turned out to be mandatory in this case, otherwise unnecessary calls would be performed against the database if the request method had been POST.
In the body of WebPart applied in the `warbler`, two queries were made to the database to fetch all `genres` and `albums` in the Music Store.
With the help of `|>` (pipe operator) those two lists were then mapped to lists of a pair (tuple) of `decimal` and `string` (`decimal * string`).
The resulting lists of tuples became arguments for the `View.createAlbum` function to populate the drop-down inputs in produced HTML form.

For the POST case, after the `>>=` operator, `bindToForm` function was used.
`bindToForm` tries to parse the request body into a given model, in this case `Form.album`.
If the parsing failed (the request could be malformed), `bindToForm` took care of responding with 400 Bad Request status code, describing why it was unable to parse the entity (`bindToFrom` is described in details in {{Forms}}).
On the other hand, if the entity could be parsed as `Form.album`, the anonymous function (second argument of `bindToForm`) was applied.
The anonymous function takes the form model as its parameter, invokes `Db.createAlbum` action with proper arguments, and finally redirects to the main administration management page.
Redirection was achieved by calling the `Redirection.FOUND` function, which writes 302 Found status code to the response and "Location" header with an URL.
Browsers treat 302 status code as a signal to issue another request to the URL defined by the "Location" header of the response.

```xxx
{FSharp]{Music Store - editing album}{fsmusiceditalbum}
let editAlbum id =
    let ctx = Db.getContext()
    match Db.getAlbum id ctx with
    | Some album ->
        choose [
            GET >>= warbler (fun _ ->
                let genres = 
                    Db.getGenres ctx 
                    |> List.map (fun g -> decimal g.GenreId, g.Name)
                let artists = 
                    Db.getArtists ctx
                    |> List.map (fun g -> decimal g.ArtistId, g.Name)
                html (View.editAlbum album genres artists))

            POST >>= bindToForm Form.album (fun form ->
                Db.updateAlbum album (int form.ArtistId, int form.GenreId, form.Price, form.Title) ctx
                Redirection.FOUND Path.Admin.manage)
        ]
    | None -> 
        never```

```xxx
{FSharp]{Composing typed route in Suave - edit album}{fssuavetypedroute}
pathScan "/admin/edit/%d" editAlbum```

Next feature implemented in the administration management module was editing an existing album.
WebPart for this functionality was placed in listing {{fsmusiceditalbum}}.
Because there is no guarantee that an album with a given `id` exists, `Db.getAlbum` (line 3) in listing {{fsmusiceditalbum}} had to be invoked, and two cases handled.
The `Db.getAlbum` was designed to return `Album option`, that's why a pattern matching could be employed in the example.
In case the `id` was correct and album was found (`Some album`), `choose` WebPart (line 5) applied.
Otherwise, if `Db.getAlbum` turned out to return `None` (album was not found) then `never` (line 20) WebPart was used.
WebPart `never` always "fails" (returns `None`), causing the composed typed route to return `None` as well and marking the WebPart as not applicable for the request.
Besides `editAlbum` took an additional `id` parameter of type `int` to identify a proper album, similar logic was followed as in listing {{fsmusiccreatealbum}} (in case the album was found):

* distinction between two allowed HTTP methods GET and POST was made,
* in case of GET, edit album form was created,
* in case of POST, update action was performed on the database and redirection to main management page was issued.

It is worth noting that because `editAlbum` signature was inferred to be `int -> WebPart`, it composed extremely easily and gracefully with a typed route WebPart, as presented in listing {{fssuavetypedroute}}.

```xxx
{FSharp]{Music Store - deleting album}{fsmusicdeletealbum}
let deleteAlbum id =
    let ctx = Db.getContext()
    match Db.getAlbum id ctx with
    | Some album ->
        choose [ 
            GET >>= warbler (fun _ -> 
                html (View.deleteAlbum album.Title))
            POST >>= warbler (fun _ -> 
                Db.deleteAlbum album ctx
                Redirection.FOUND Path.Admin.manage)
        ]
    | None ->
        never```

```xxx
{FSharp]{Composing typed route in Suave - delete album}{fssuavetypedroute2}
pathScan "/admin/delete/%d" deleteAlbum```

Last functionality added to the administration management module was deleting an album.
After an album is deleted, it is no longer available in the Music Store for users to buy.
Implementation of this action required code as in listing {{fsmusicdeletealbum}}.
The `deleteAlbum` function consisted of similar steps as `editAlbum`.
First, verification happened that checked whether an album with given `id` existed.
Secondly, pattern matching was applied to the result of invocation of `Db.getAlbum` function.
In case the album did not exist, `never` implied discarding this WebPart.
If however the album was found, then depending on the HTTP method (GET or POST) corresponding result would apply.
For GET requests, the application returned an HTML page with a confirmation screen whether to delete the album.
Sending a POST request confirmed the deletion.
The user could also navigate away from the confirmation page, or move back in browsing history, which would not affect the album in question.
Logic for POST WebPart invoked proper action on `Db` module, and returned redirection WebPart afterwards (the same as in `editAlbum`).
Both POST and GET WebParts had to be surrounded with `warbler`s, because eager evaluation would cause unwanted effects.
Again, as was the case with editing album, the `deleteAlbum` composed nicely with the typed route (listing {{fssuavetypedroute2}}).

The 3 actions described in this section are sometimes associated with "CRUD" acronym (Create - Update - Delete).
With their uniform logic, they tend to be used for the purpose of demonstrating a library or tool capabilities.
Above listings proved that one can cope with this common programming challenge in functional-first language such as F#.
Typed routes feature from Suave confirmed to be highly composable, and thus the WebParts that were built could be matched together in a transparent manner.

### Session

Another interesting concern with regards to Internet applications development is State Management Mechanism, also known as Session in software engineers' dialect.
Despite the HTTP protocol being stateless, majority of web applications still keep track of the requests coming from the same initiator.
To do that, they have to attach specific meta-data in request and responses such as cookies or other headers, arguments in URL paths, contents of the body.
In addition to that, those application have to persist the state on the server side for later retrieval.

In Music Store, session was used to track state of cart identifier and details of logged on user (if present).
The application allowed anonymous users to add albums to their cart (without authentication).
For that purpose, a unique identifier was assigned in a cookie and corresponding database table row.
Thanks to that, users could add albums to cart without being logged on to the Store.
Authentication was however required, when the user wanted to checkout with his albums in cart.
If user was authenticated, session contained information about the name and role of the user.
The first proved helpful for displaying a greeting at the very top of the page, and the second was used for authorization.
From the database point of view both anonymous cart identifier and user's name were used for "cartId" database table row.
In order to model possible session states, a few types were declared as per listing {{fsmusicsessiontype}}.

An F# Record type was declared in first line of listing {{fsmusicsessiontype}}.
Records behave similarly to standard C# classes, however there is a number of advantages for records, such as:

* they are immutable by default, 
* constructor enforces all fields to be initialized, which prevents from having any record field undefined,
* they have structural equality by default, meaning that one does not have to override equality members as in C#,
* they can appear in pattern matching constructs,
* they provide a handy shortcut for copying (cloning) objects called copy expression, which allows to create new instance of record with some of the fields modified.

Another F# type construct, called discriminated union was used in line 6.
They allow for declaring a type with a set of applicable constructors and are specially designed to work with pattern matching.
Following is a definition from Microsoft Developer Network (MSDN) on discriminated unions {{{msdn}}}:
 
>> *"Discriminated unions provide support for values that can be one of a number of named cases, possibly each with different values and types."*

```xxx
{FSharp]{Modelling session in Music Store}{fsmusicsessiontype}
type UserLoggedOnSession = {
    Username : string
    Role : string
}

type Session = 
    | NoSession
    | CartIdOnly of string
    | UserLoggedOn of UserLoggedOnSession```

The `UserLoggedOnSession` record type has two properties: `Username` and `Role`, both of `string` type.
This type was part of the discriminated union case (line 9).
`Session` discriminated union type (line 6) consisted of three possible cases:

* `NoSession` (line 7) - indicated that no session is attached to context request,
* `CartIdOnly` (line 8) - reflected that user is adding albums to his cart without being authenticated,
* `UserLoggedOn` (line 9) - determined authenticated requests with details of the user (inside `UserLoggedOnSession` type).

Main reason for creating this type in Music Store was to achieve a unified way of determining user state.
For the purpose of composing `Session` type with WebPart, a companion function `session` (lower case) was defined in listing {{fsmusicsessionfun}}.
At its starting point (line 2), session invoked `statefulForSession` which is a WebPart from Suave library that initiates user's state to work with.
The initialization was then bound to the `context` function (line 3) to allow browsing user's state.
For browsing the state, `HttpContext.state` proved useful (line 4).
Its return type is `SessionStore option` where `SessionStore` accumulates a reader `get` and a writer `set` functions, for fetching and modifying state respectively.
A pattern matching on a triple (tuple with three values) of user's state keys matched following cases (lines 7-10): 

* if a "cartid" key was present, but there was no "username" and no "role" keys, `CartIdOnly` would be returned,
* if both "username" and "role" keys appeared in the session store, `UserLoggedOn` value applied,
* as a fall-back, `NoSession` was chosen when none of those keys were present.

Result of pattern matching block was applied (or colloquially "piped") to function `f` (line 11), which came as an argument to `session` and had a type of `Session -> WebPart`.
Having such function in toolbox allowed to define new WebParts that depended on user's state in a convenient manner, with one example shown in listing {{fsmusicaddtocart}}.

```xxx
{FSharp]{Determining session state in Music Store}{fsmusicsessionfun}
let session f = 
    statefulForSession
    >>= context (fun x -> 
        match x |> HttpContext.state with
        | None -> f NoSession
        | Some state ->
            match state.get "cartid", state.get "username", state.get "role" with
            | Some cartId, None, None -> CartIdOnly cartId
            | _, Some username, Some role -> UserLoggedOn {Username = username; Role = role}
            | _ -> NoSession
            |> f )```

```xxx
{FSharp]{Session in Music Store - adding to cart}{fsmusicaddtocart}
let addToCart albumId =
    let ctx = Db.getContext()
    session (function
            | NoSession -> 
                let cartId = Guid.NewGuid().ToString("N")
                Db.addToCart cartId albumId ctx
                sessionStore (fun store ->
                    store.set "cartid" cartId)
            | UserLoggedOn { Username = cartId } | CartIdOnly cartId ->
                Db.addToCart cartId albumId ctx
                succeed)
        >>= Redirection.FOUND Path.Cart.overview```

```xxx
{FSharp]{Syntactic sugar for pattern matching in FSharp}{fspatternmatchsugar}
// standard syntax
let f x =
    match x with
    | ... ->

// shortcut "syntactic sugar" syntax
let f = function 
    | ... -> ```

As the `Session` type utilizes discriminated unions, pattern matching could be used again for handling different cases (listing {{fsmusicaddtocart}} lines 3-11).
Whenever `addToCart` was invoked without any session (`NoSession`), a new Globally Unique Identifier (GUID) was generated (line 5).
Using the new GUID value, proper database action was then invoked and the GUID got saved in session store.
For the above `addToCart` function, the same piece of code was invoked when user was logged on to Music Store and when he was not but had already an anonymous cart identifier assigned to his session.
In order to express such logic, a double-matching pattern was used (line 9).
If that was the case, cartId was sufficient to perform the `Db.addToCart` action, and there was no need to generate new GUID.
A syntactic sugar has been used for pattern matching construct, which comes handy when declaring a one-argument function that immediately pattern matches on this argument.
Listing {{fspatternmatchsugar}} shows how the syntactic sugar can be used to make the code more concise.

Another example on `Session` type usage is presented in listing {{fsmusicviewcart}}.
In listing {{fsmusicviewcart}}, `cart` WebPart was declared.
Its purpose was to display contents of the current user's cart.
In case he did not have any session attached (and no albums in cart), the user was shown a `View.emptyCart` page.
That page had to encourage the user to do the shopping.
On the other hand, if user had already some albums in his cart, contents of the cart were displayed on `View.cart` page (line 6).

```xxx
{FSharp]{Session in Music Store - viewing the cart}{fsmusicviewcart}
let cart = 
    session (function
    | NoSession -> View.emptyCart |> html
    | UserLoggedOn { Username = cartId } | CartIdOnly cartId ->
        let ctx = Db.getContext()
        Db.getCartsDetails cartId ctx |> View.cart |> html)```


Session handling concept in ASP.NET MVC is relatively similar to how it was achieved in Music Store with F# and Suave.
A `Session` property in every Controller exists to serve session values indexed by keys of type `string`.

State management mechanism in Music Store was implemented in akin manner when comparing to how it is usually dealt with in imperative, OOP frameworks.
Thanks to discriminated unions, records and the great feature of pattern matching in F#, it was convenient to model all possible states and handle each of them separately.

### Authentication and Authorization

Access to some contents of a WebSite is not always public these days anymore.
Companies that make money on selling information across the web have certain limitations with regards to who and under what conditions can browse specific parts of their site.
Web applications that use concept of accounts to tie data with users, must know the identity of a person that is performing certain actions.
There are also web sites that tend to make profit on persisting in database and selling details about a significant amount of users and their emails.
Such needs as in above examples led to forming the concepts of authentication and authorization.
Both of them can be built on top of the state management mechanism by using cookies.
Since authentication, authorization and session are rather an orthogonal concept to session, they are described separately.

Authentication is a process, in which a user of a system (be it a human or machine program) presents his credential to the requested authority.
In web browsing scenario it is often preceded by the web site (playing authority role) blocking access (for user) to a desired resource.
The HTTP protocol defines a specific status code for that purpose.
401 Unauthorized status code in HTTP response means that request requires user authentication {{{fielding1999hypertext}}}.
It is a bit unfortunate that the HTTP standard defines the status code with "Unauthorized" keyword, which relates to authorization and is a different concept.

In Music Store, form authentication approach was applied.
Form authentication relies on passing users credentials in request body, with Content-Type header representing either URL or multi-part form data encoding.
This mechanism should always be accompanied with Secure Socket Layer (SSL) encryption, meaning that HTTPS scheme should be used, because form authentication does not provide ad hoc encryption and the request body is often sent in plain text.
For the sake of simplicity, SSL is not covered in Music Store application.
Listing {{fsmusiclogon}} shows how basic form authentication was implemented in Music Store.

```xxx
{FSharp]{Authentication in Music Store}{fsmusiclogon}
let returnPathOrHome = 
    request (fun x -> 
        let path = 
            match (x.queryParam "returnPath") with
            | Choice1Of2 path -> path
            | _ -> Path.home
        Redirection.FOUND path)

let logon =
    choose [
        GET >>= (View.logon "" |> html)
        POST >>= bindToForm Form.logon (fun form ->
            let ctx = Db.getContext()
            let (Password password) = form.Password
            match Db.validateUser(form.Username, passHash password) ctx with
            | Some user ->
                    Auth.authenticated Cookie.CookieLife.Session false 
                    >>= returnPathOrHome
            | _ ->
                View.logon "Username or password is invalid." |> html
        )
    ]```

The main handler for authentication `logon` (line 9) benefited from `choose` to distinguish between GET and POST methods.
For GET requests (line 11), `logon` did respond with an HTML page with form to log in to the Music Store.
Interesting fact is that `warbler` function was not crucial in this case.
That is because the result of `View.logon` was treated as a fixed HTML markup, as it always took empty string as parameter.
Once the WebPart on the right-hand side of `>>=` in line 11 was evaluated, it did not need to be recomputed anymore.
There is also another invocation of `View.logon` in line 20, which is discussed in the further course.

POST requests to `logon` were treated as an authentication trial.
As was the case with creating and editing albums, `bindToForm` took care (by returning Bad Request status code) of possible malformed requests that did not originate from the login page.
When the data sent with POST matched `Form.logon` model, a database query (line 15) was triggered to validate passed credentials.
Written in-line, `passHash` was defined as a helper function that returned a hash from the password with help of 32-bit-word Secure Hash Algorithm (SHA-256).
In case, the given credentials did not match (either such user's name did not exist or the password was incorrect), `View.logon` page was displayed again this time with a validation error message in red color to indicate the failure (line 20). 
If however both user's name and password were correct, authentication process was initiated.

Suave framework ships with helper functions such as `Auth.authenticated` (line 17) that made working with authentication more convenient.
Because it relies on cookies, first argument of `Auth.authenticated` describes lifetime of the cookie.
Here `CookieLife.Session` was chosen, which means that the cookie is valid until browser session is open.
Second argument of `boolean` type determines whether the cookie should be marked as secure - as SSL was not employed in the application, `false` value was passed. 
In result of applying the `Auth.authenticated` WebPart, the framework wrote `Set-Cookie` header with properly encrypted authentication cookie to the HTTP response. 
Upon receiving response with `Set-Cookie` header present, a browser saves the cookie value for visited URL in order to issue each following request with this cookie.

In line 18, another WebPart composition occurred.
Having set the cookie, program flow was bound to `returnPathOrHome`.
WebPart `returnPathOrHome` (line 1) had a look inside the incoming request to find out whether parameter called "returnPath" existed in URL query.
If that was the case (line 5), then value of this parameter would determine to what location redirection should happen.
Otherwise (line 6), the redirection would be made to the main page, `Path.home`.

```xxx
{FSharp]{Helper functions for authentication}{fsmusicisauthenticate}
let redirectWithReturnPath redirection =
    request (fun x ->
        let path = x.url.AbsolutePath
        Redirection.FOUND (redirection |> Path.withParam ("returnPath", path)))

let loggedOn f_success =
    Auth.authenticate
        Cookie.CookieLife.Session
        false
        (fun () -> Choice2Of2(redirectWithReturnPath "/account/logon"))
        (fun _ -> Choice2Of2 (BAD_REQUEST "Descryption error!"))
        f_success```

Functions from listing {{fsmusicisauthenticate}} were necessary to verify if incoming request is authenticated.
The first one, `redirectWithReturnPath` (line 1) did set the "returnPath" URL query parameter, used by `returnPathOrHome` WebPart which was defined earlier.
It basically took a `redirection` URL as its argument, appended current absolute path as query parameter to `redirection`, and issued a redirection to the result URL.
WebPart `loggedOn`, which was defined in line 6 can be thought of a "guard" of the `f_success` WebPart.
`loggedOn` challenged the request with another built into Suave helper `Auth.authenticate`.
The `Auth.authenticate` took 5 arguments:

* `CookieLife.Session` life expiry - the same as was used for setting authentication cookie,
* `false` which reflected that secure protocol was not employed,
* function which determined what should happen in case authentication cookie was missing,
* another function which would be invoked if the cookie was present, but decryption of the cookie value failed,
* finally the `f_success` WebPart that was applied when request came with valid authentication cookie.

In line 10, `redirectWithReturnPath` was applied in case of missing cookie.
This meant that when someone tried to get to a resource without being authenticated, he would be redirected to the login page.
Line 11, on the other hand, was used when the cookie could not be decrypted with secrete server key.
That could potentially mean a malicious request, that is why 400 Bad Request status code was chosen as a response.
One of the actions that required being logged on to the Music Store was checking out the cart with albums.
To apply `loggedOn` validation on a `checkout` WebPart, those two were composed like in lisiting {{fsmusiccomposeauthentication}}.

```xxx
{FSharp]{Composing authentication with other WebParts}{fsmusiccomposeauthentication}
path "/cart/checkout" >>= loggedOn checkout```

Unlike authentication, authorization does not rely on providing credential, but rather validating already present credential against some kind of a "challenge".
Most popular challenge these days seems to be the concept of roles.
A role describes the category of a user.
As an example, in Music Store there were 2 simple roles defined:

* "user" - a standard user, someone who can browse through and buy music albums,
* "admin" - a privileged user of the system, that is allowed to manage albums.

New users that would register to the Music Store, got automatically the "user" role assigned.
There was only one predefined user with "admin" role and the same name.
Function from listing {{fsmusicadminauth}} was implemented to allow only authorized users to a specific handler.

```xxx
{FSharp]{Authorize access for administrator in Music Store}{fsmusicadminauth}
let admin f_success =
    loggedOn (session (function
        | UserLoggedOn { Role = "admin" } -> f_success
        | UserLoggedOn _ -> FORBIDDEN "Only for admin"
        | _ -> UNAUTHORIZED "Not logged in" ))```

As was the case with `loggedOn`, `admin` function played a role of a guard over the `f_success` WebPart and allowed only "admin" roles in.
In fact, it invoked the `loggedOn` (in line 2) to ensure that request was authenticated, because otherwise there would be no one to authorize.
`session` function was utilized to determine the state of the user.
Among three possible values, one of them was `UserLoggedOn` with `Name` and `Role` properties inside.
The fact that record types can be subject of pattern matching was exploited to match on `Role` property.
Thanks to that, `f_success` got applied only when `Role` matched "admin" (line 3).
If user was authenticated but his role was not "admin" (line 4), a 403 Forbidden status code was written to the HTTP response.
Line 5 would return 401 Unauthorized status code in case the request was not authenticated.
The last case served only as a safety net preventing compiler warnings, as the `loggedOn` guard would not allow authenticated requests to pass through.

Once again, the HTTP status code names for 401 and 403 may sound confusing in context of this section.
**401 Unauthorized** code in practice means that request is not **authenticated**, while **403 Forbidden** stands for being **authenticated**, but **unauthorized** to browse selected resource.

```xxx
{FSharp]{Composing authorization with other WebParts}{fsmusiccomposingauthorization}
path "/admin/manage" >>= admin manage
path "/admin/create" >>= admin createAlbum
pathScan "/admin/edit/%d" (fun id -> admin (editAlbum id))
pathScan "/admin/delete/%d" (fun id -> admin (deleteAlbum id))```

Composing `admin` function with other WebParts was achieved thanks to code like in listing {{fsmusiccomposingauthorization}}.
While for parameter-less WebParts like `manage` or `createAlbum`, it was only necessary to wrap them with `admin`, those WebParts that took an argument (`editAlbum` and `deleteAlbum`) had to be contained inside a lambda (anonymous) function.

Authentication and Authorization in ASP.NET MVC are usually handled with code annotations in form of attributes.
Those attributes are attached to specific controllers / actions and allow to express the concept in declarative way.
As an example, to mark that every action in a specific controller should be authorized and only "admin" role can access these actions, the controller would be decorated with `[Authorize(Roles="Administrators")]` attribute.
While it is a convenient way of dealing with those cross-cutting concerns, such approach can also have its disadvantages.
Using annotations is related to making the software engineer unaware of what is really happening under the hood.
In contrast to that, an obligation to connect the authentication and authorization concepts with rest of the application's logic, results in easier reasoning about the implementation as well as discovering potential software defects.

It is getting more evident that aspects of web development follow similar pattern in Music Store application.
To utilize both authentication and authorization, another building blocks were created.
All those building blocks happened to reside near WebPart type, which made them reusable and suitable for composition.
Implementation of the concepts in functional paradigm significantly differs from how they tend to be defined in OOP.
Declarative approach used in frameworks like ASP.NET MVC makes it extremely easy to employ authentication and authorization, but has a downside of being less aware of mechanism that drives the program flow.
Solution that Suave takes advantage of requires the developer to get familiar with the underlying protocol and adapt existing functions into his code.
Thanks to that, one gets better understanding on how different components are meant to work in integration.

### Forms

HTML form is the most popular way of communication between browser-based user interface and the server.
Values provided in form fields are aggregated and pasted into the request body when a form is submitted.
Implementation of form communication consists of a number of steps:

* rendering proper HTML markup with named inputs,
* client-side validation of form fields values,
* formatting the request body upon submitting (usually this is step is handled by browsers),
* parsing the request body on the server side,
* server-side validation of form fields values,
* actual handler for request.

Most rich and heavy-weight frameworks for Internet applications (including ASP.NET MVC) already include in their repertoire modules and helpers for handling above scenario end-to-end.
Since Suave is very light-weight, it does not (at the time of writing) come out with ready to use functions for end-to-end form communication between client and server.
Engaging part of the research was thus ability to create utility modules for working with forms in Suave.
Even more inviting experience was that the written modules were accepted as part of the official Suave package (in its "Experimental" distribution), since Suave is an open-source software hosted at GitHub web site {{{suave}}}.

The prepared functionality aimed to target all of the steps enlisted above.
It was designed to do so in a declarative way, by providing strongly typed access to values of form fields, an example of which is shown in listing {{fsformfields}}.
Form fields were to be enclosed in a record type.
Supported types for the fields were: 

* `string` - for representing text fields,
* `Password` - for representing passwords,
* `decimal` - for representing numbers (decimal was chosen deliberately to handle both integers as well as fractions),
* `System.Net.Mail.MailAddress` - built-in .NET type for representing email addresses.

In addition to that, the module supported concept of required / optional fields.
By default field was treated as required.
If a field was to be marked as optional, it had to be of `option` type.
Thanks to such solution, all values were type-safe (i.e. no null references would occur while reading a required field).
`Register` type in listing {{fsformfields}} was declared for the sake of registering a new user.
It reflected a form with following fields:

* `Username` - required field of type `string` being a unique name of the user of Music Store,
* `Email` - required field of type `MailAddress` for email address of the user,
* `Password` - required field of type `Password` for providing user's password,
* `ConfirmPassword` - the same as above, but used to prevent user from mistyping the passwords,
* `YearOfBirth` - optional field of type `decimal option` that could be used for recommending appropriate albums.

```xxx
{FSharp]{Declaration of fields for registration form}{fsformfields}
type Register = {
    Username : string
    Email : MailAddress
    Password : Password
    ConfirmPassword : Password
    YearOfBirth : decimal option }```

```xxx
{FSharp]{Validating field values in registration form}{fsformvalid}
let pattern = @"^\w{6,20}$"

let passwordsMatch =
    (fun f -> f.Password = f.ConfirmPassword), "Passwords must match"

let register : Form<Register> =
    Form ([ TextProp ((fun f -> <@ f.Username @>), [ maxLength 30 ] )
            PasswordProp ((fun f -> <@ f.Password @>), [ passwordRegex pattern ] )
            PasswordProp ((fun f -> <@ f.ConfirmPassword @>), [ passwordRegex pattern ] )
            ],[ passwordsMatch ])```


Next thing that the module supported was declaring certain validation of the fields.
For the `Register` form, validation rules were presented in listing {{fsformvalid}}.
Actual declaration of the `Register` form occurred in line 6.
It consisted of single union case `Form` with two lists as its arguments: 

* first list (lines 7-9) contained validation rules that could be used both on client and server side,
* second list (line 10) with a single element `passwordsMatch` accommodated additional validations that could be performed only on server side.

The server side validation functions had to be of type `('t -> bool) * string`.
This meant a tuple of predicate function and `string` message that would be used in case of violation of that rule.
Argument of the predicate function was a record enclosing form fields, which in case of `Register` allowed to query for and compare `Password` with `ConfirmPassword` fields.
Such server-side only validation rules could be used for logic that could not be easily expressed without any JavaScript code on the client side.
Indeed, in the above example it was impossible to compare two fields on client side without having to write additional script.

Validation rules from the first list could be used both for client and server side, as they involved just a single field.
Thanks to new `input` element attributes in HTML5 standard, such as `maxlength` or `pattern`, client-side validation could be achieved.
The rules shown in above snippet were either `TextProp` or `PasswordProp` (property) meaning that they concerned text field or password fields respectively.
First argument of the property was a function that made use of F# feature called **quotations**.
Properties enclosed within the `<@ ... @>` operators were interpreted by the compiler as quotations {{{syme2006leveraging}}}: 

>> *"F# quotations allow the capture of type-checked expressions as structured terms (...) that can then be interpreted, analyzed and compiled to alternative languages."*

For the form module utility, F# quotations allowed to extract name of a field to be used in HTML rendering, as well as parsing the request body on server side.
Thanks to that, no annotations had to be used for the properties of `Register` record type.
Second argument of the `TextProp` and `PasswordProp` were lists of duplex (client and server side) validations rules.
Type definition of duplex rules was similar to server-side-only rules, except they required also a tuple of string (key and value) for to depict proper attribute of HTML `input` element:

* `maxLength 30` determined a string for text input, no longer than 30 characters,
* `passwordRegex pattern` specified a regular expression to be matched on password inputs.

Regular expression `pattern` defined in first line, matched a string of alphanumeric characters (or underscore) which length was at least 6 and at most 20.
This regular expression designated possible passwords for newly coming users.

Music Store application consisted of a few forms, and all of them followed similar layout.
In order to unify way of structuring forms' layout and rendering the forms in HTML markup, specific types were defined as shown in listing {{fsformlayouttypes}}.
`Xml` property of `Field<'a>` type was a function from `Form<'a>` to `Suave.Html.Xml` type, which represented object model for HTML markup.
Rest of record type properties are rather self-explanatory.
With help of the `FormLayout` type, the `Register` form as defined earlier could be then used to render a corresponding HTML markup, which was shown in listing {{fsformrenderhtml}}.

```xxx
{FSharp]{Declaration of form layout types}{fsformlayouttypes}
type Field<'a> = {
    Label : string
    Xml : Form<'a> -> Suave.Html.Xml
}

type Fieldset<'a> = {
    Legend : string
    Fields : Field<'a> list
}

type FormLayout<'a> = {
    Fieldsets : Fieldset<'a> list
    SubmitText : string
    Form : Form<'a>
}```

```xxx
{FSharp]{Rendering HTML markup from form layout}{fsformrenderhtml}
renderForm
    { Form = Form.register
      Fieldsets = 
          [ { Legend = "Create a New Account"
              Fields = 
                  [ { Label = "User name (max 30 characters)"
                      Xml = input (fun f -> <@ f.Username @>) [] }
                    { Label = "Email address"
                      Xml = input (fun f -> <@ f.Email @>) [] }
                    { Label = "Year of birth"
                      Xml = input (fun f -> <@ f.YearOfBirth @>) [] }
                    { Label = "Password (between 6 and 20 characters)"
                      Xml = input (fun f -> <@ f.Password @>) [] }
                    { Label = "Confirm password"
                      Xml = input (fun f -> <@ f.ConfirmPassword @>) [] } ] } ]
      SubmitText = "Register" }```

Function `renderForm` was responsible for unified HTML markup of all forms in Music Store.
Partial application mechanism proved to be useful with regard to defining `Xml` property (lines 7,9,11,13,15).
`input` function took following arguments (in order):

* `'a -> Expr<'b>` - function for pointing out a proper field from the record type,
* `(string * string) list` - additional attributes to be used for HTML `input` element (not used in listing {{fsformrenderhtml}}),
* `Form<'a>` - definition of the form of type `'a`.

Because the `input` function was being invoked with only 2 parameters, the last `Form<'a>` argument got curried.
As a result of partial application on `input` function, the return type was `Form<'a> -> Suave.Html.Xml`, which turned out to match exactly the expected type for `Xml` property.

Having served proper HTML markup, an actual handler for registering users could be defined.
Registration WebPart was presented in listing {{fsmusicregister}}.
Handler for POST requests to "/user/register" route was defined in line 4.
It employed the `bindToForm` function, which was part of the created form utility module.
Intent of `bindToForm` was to parse the request body, encoded in standard form's way, into a corresponding type determined by the first argument of `bindToForm`.
In this case the argument was `Form.register` which meant that the parser looked for field names such as `Username`, `Email` or `Password`.
After successful extraction of values for those fields, the parser would try to parse the values to proper types.
At the end of the day, if request body was not malformed, instance of `Register` type would be created and passed (`form` in line 4) into function which was second argument of `bindToForm`.
The function's type was `'a -> WebPart` where `'a` was the type of form.
In case of parsing failures, `bindToForm` would return 400 Bad Request status code with an informative message on which part could not be processed.

```xxx
{FSharp]{Implementation of register WebPart in Music Store}{fsmusicregister}
let register =
    choose [
        GET >>= (View.register "" |> html)
        POST >>= bindToForm Form.register (fun form ->
            let ctx = Db.getContext()
            match Db.getUser form.Username ctx with
            | Some existing -> 
                View.register "Sorry this username is already taken. Try another one." |> html
            | None ->
                let (Password password) = form.Password
                let email = form.Email.Address
                let user = Db.newUser (form.Username, passHash password, email) ctx
                authenticateUser user
        )
    ]```

End-to-end form data handling with validation in ASP.NET MVC framework works in declarative fashion as well.
It usually does so with the help of appropriate annotations on members of a class.
For the client-side validation rules it makes use of quite complex JavaScript code.
In addition to that, it utilizes concept of "scaffolding", which automatically generates corresponding forms for creating and editing an entity.
While quite extensive and heavy-weight, the mechanism of creating forms with ASP.NET MVC framework meets the challenge.

Suave framework with its concise nature did not ship with a ready-to-use top-to-bottom form handling features.
Thanks to Suave being highly composable, it was opportune to create a form utility module that could fit into the WebPart pipeline.
With the help of a few features in F# language, the module managed to be reusable in declarative and succinct way.
Also, it turned out to be a fascinating experience to be able to contribute to the Suave framework by sharing implementation of that module.