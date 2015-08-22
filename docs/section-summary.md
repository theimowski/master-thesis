SUMMARY
=======

Section {{APPLICATION OF FUNCTIONAL PROGRAMMING}} focused on building a real-world application in FP language.
As the domain of the problem, web development was chosen to prove that functional paradigm does not have to be restricted to a specific set of fields.
F# language in conjunction with Suave web framework made it possible to implement server concise in size, but with load of features.
With the help of a number of functional techniques, an e-commerce Music Store website was created.
The guide through the code covered a significant amount of cross-cutting concerns of web development.
Every single concept extended the WebPart idea, which allowed to match all the building blocks into one consistent whole, in a natural for FP manner.

Throughout the research, comparisons were made with ASP.NET MVC framework, which comes from OOP world.
One interesting finding was that ASP.NET MVC, despite being much larger and offering much more features than Suave, does not guarantee such type-safety as the latter.
F# as well as any statically typed functional language in general, prevents a huge amount of issues yet in compile time.
The language combined with Suave, which ships with features like typed routes, made the concept of type safety in demonstrated application even more visible.
Such behavior could cause frustration for a new-comer to functional world, but is an enormous time saver for more experienced programmer, who is familiar with functional techniques and compiler requirements.
Following are some of the main benefits that FP may bring for building and maintaining business applications:

* **immutability** - data does not have any behavior which can mutate its internal state - it is hence thread-safe by definition, allowing to scale out easily,
* **less code** - functional code is more concise, thus is more likely to contain less defects,
* **strongly static typing** (applicable for statically typed functional languages) - type systems of F# or Haskell are very strict, therefore can detect more issues in compile time,
* **composition** - fine-grained functions can be composed into larger building blocks without losing precious properties of pure functions,
* **no side effects** - FP is all about transforming inputs to outputs {{{leanfunprog}}} which eliminates bizarre order dependencies from code and results in better reasoning about the code.

Functional programming is considered the right approach but only when applied to a restricted, related to academic background, set of domains.
Majority of areas are still seen as being dominated by OOP.
The thesis aimed to prove that FP can in fact have much broader spectrum of use cases, including enterprise-specific software.
Hopefully the thesis confirmed that it is not only possible, but even profitable to build real-world, business applications using FP.