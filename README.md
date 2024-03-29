# Archived

This project is archived and has been replaced with [Colibri Lisp](https://github.com/paulirwin/colibri).

# Lillisp

[![.NET](https://github.com/paulirwin/lillisp/actions/workflows/dotnet.yml/badge.svg)](https://github.com/paulirwin/lillisp/actions/workflows/dotnet.yml)

This repo contains the Lillisp core runtime (which includes the standard library) and the Lillisp REPL.

## What is Lillisp?

Lillisp (prounounced "lill-isp", like "lillies") is a Lisp-based language that is written in C# (with some library functions written in Lillisp itself) and runs on [.NET](https://dotnet.microsoft.com/). 

Currently, Lillisp can call some .NET code, but anything defined in it is not yet easily callable *from* .NET code. Lillisp can be used as a REPL, or you can specify a file to interpret. Compilation is on the roadmap but not yet supported.

Lillisp is a Scheme-based Lisp, and ultimately aims to be as [R7RS-small](https://small.r7rs.org/) compliant as possible. Being a Scheme, Lillisp is a [Lisp-1](https://andersmurphy.com/2019/03/08/lisp-1-vs-lisp-2.html), meaning functions and variables/parameters cannot share the same name, and functions do not need to be quoted to be passed as values.

Lillisp also draws inspiration from Clojure, and uses its syntax in part, such as with .NET interop.

Lillisp started as a C# implementation of Peter Norvig's lis.py from the blog post [(How to Write a (Lisp) Interpreter (in Python))](https://norvig.com/lispy.html). Many thanks to Peter for the excellent tutorial that inspired this project.

## Screenshot

![image](https://user-images.githubusercontent.com/1874103/137605342-15623f3f-9ca0-429c-b655-e02176ba6b9a.png)

(Using the open-source [Windows Terminal](https://github.com/microsoft/terminal), [Powershell 7](https://github.com/PowerShell/PowerShell), and [Cascadia Code](https://github.com/microsoft/cascadia-code) font, running on .NET 6.)

## Using the REPL

Build and run the Lillisp project in Visual Studio 2022 or later, or via the `dotnet` CLI.

Input commands and hit enter. Note that multi-line input is not yet supported.

Certain REPL commands may be entered, without parentheses. These include:

| Command | Description |
| --- | --- |
| `clear` or `cls` | Clears the console buffer/window and starts a new input line at the top of the buffer. |
| `exit` or `quit` or <kbd>Ctrl</kbd>+<kbd>C</kbd> | Exits the Lillisp REPL. |
| `reset` | Resets the current Lillisp runtime environment, starting fresh. All data in memory will be lost. |

## Lillisp CLI

Running the Lillisp project executable will launch the REPL if no command-line arguments are provided.

The following command-line arguments are supported:

| Argument | Description |
| --- | --- |
| `--file <path>` | Run the Lillisp interpreter on the specified file. |
| `--version` | Display the Lillisp version. |
| `--help` | Display help information on the available command-line arguments. |

## The Lillisp Language

Full docs coming at some point in the future. But it's basically a normal Lisp with mostly Scheme syntax. Check out LillispRuntime.cs and Library/core.lisp for built-in library methods.

An incomplete list of features currently supported:
* Data types: list, pair, vector, bytevector, number, boolean, character, string, symbol, nil, procedure
* Number types: complex (rectangular `-3+2i` notation), real, rational (i.e. `3/8`), integer
* Defining variables with `define` (aliased as `def`)
* Mutating variables with `set!`
* Most common math operations (`+`, `-`, `*`, `/`, `abs`, `log`, `sqrt`, etc. - others available in `System.Math`)
* Boolean expressions (`<`, `>`, `<=`, `==`, `and`, `or`, etc)
* List operations (`list`, `car`, `cdr`, `cons`, etc)
* Quoting expressions (i.e. `'(1 2 3)` or `(quote (1 2 3))`)
* Quasiquoting, unquoting, and splicing (i.e. ``(eval `(+ ,@(range 0 10)))``)
* Higher-order list functions (`apply`, `map`)
* Conditional logic (`if`, `cond`, `when`)
* Sequential logic with `begin`
* Lambda expressions with `lambda`
* Tail recursion (except for `and` and `or`, or any invoked .NET code)
* Shorthand for defining a lambda variable (aka a named function) with `defun` (or `define` with a list as the first parameter)
* Block-scoping variables with `let`, `let*`, etc
* Rational number operations (`rationalize`, `numerator`/`denominator`, `simplify`)
* Exceptions (`with-exception-handler`, `raise`, `error`, `raise-continuable`, etc.)
* Record types with `define-record-type`
* Almost all of the Scheme base library string-, vector-, port-, and bytevector-related functions
* Almost all of the Scheme `char`, `complex`, `CxR`, `file`, `inexact`, `lazy`, `process-context`, `read`, `time`, and `write` library functions

Notable features not yet implemented from Scheme R7RS include:
* Tail context for `and` and `or`
* Macros
* Libraries (as in, i.e. `import`)
* Many base library methods, and other libraries

Basically, give your existing Lisp code a try, and if a given feature doesn't work, file an issue.

## .NET Interop

Lillisp uses Clojure-inspired syntax for interop with .NET types. Any .NET object can be stored to a Lillisp variable. (Note: all values are boxed to `System.Object`.)

.NET Interop is *extremely experimental* and *very fragile*.

### Static Methods

You can call static methods on types with the `/` character in place of i.e. a `.` character in C#. Examples:

```lisp
Lillisp> (String/IsNullOrWhiteSpace "foo")
-> False
Lillisp> (String/IsNullOrWhiteSpace " \t ")
-> True
Lillisp> (Int32/Parse "123")
-> 123
Lillisp> (Guid/NewGuid)
-> 7bf62a3c-bcd1-4e38-aceb-50f13c4113d5
```

### Static Members

Just like static methods, you can access static members like fields and properties the same way.

```lisp
Lillisp> Int32/MaxValue
-> 2147483647
Lillisp> (def intmax Int32/MaxValue)
-> intmax
Lillisp> intmax
-> 2147483647
```

### Creating Objects

.NET objects can be created with the `new` function. The first parameter is the .NET type name, followed by any constructor parameters.

```lisp
Lillisp> (new Object)
-> System.Object
Lillisp> (def rnd (new Random))
-> rnd
Lillisp> rnd
-> System.Random
```

### Instance Methods

Just like Clojure, you can call instance methods on an object with the name of the method proceeded by a `.`, then the instance to call it on, followed by any parameters.

```lisp
Lillisp> (def rnd (new Random))
-> rnd
Lillisp> (.Next rnd)
-> 1055373556
Lillisp> (.Next rnd)
-> 938800480
Lillisp> (.Next rnd 100)
-> 73
Lillisp> (.Next rnd 100)
-> 55
```

### Instance Members

Likewise, instance members (fields and properties) can be accessed the same way.

```lisp
Lillisp> (def u (new Uri "https://www.github.com/paulirwin/lillisp"))
-> u
Lillisp> (.Scheme u)
-> "https"
Lillisp> (.Host u)
-> "www.github.com"
Lillisp> (.PathAndQuery u)
-> "/paulirwin/lillisp"
```

### Importing Namespaces

Currently, only the .NET 6 Base Class Library is available. Any namespaces can be "imported" (like the `using` statement in C#) into the current environment with the `use` keyword and a quoted symbol of the namespace name. Examples:

```
Lillisp> (new StringBuilder)
ERROR: Unable to resolve symbol StringBuilder
Lillisp> (use 'System.Text)
-> ()
Lillisp> (def x (new StringBuilder))
-> x
Lillisp> x
-> ""
Lillisp> (.Append x #\*)
-> "*"
Lillisp> (.Append x "foo")
-> "*foo"
Lillisp> (.AppendLine x "bar!")
-> "*foobar!\r\n"
```

### Generic Types

Generic types can be used as if invoking the type as a function. In other languages, generic types are called "parameterized types", so Lillisp takes this literally, as if parameters to a function. Think of an "open" generic type as a function that takes type parameters and returns a "closed" generic type. For example, `List<String>` becomes `(List String)`.

```lisp
Lillisp> (use 'System.Collections.Generic)
-> ()
Lillisp> (List String)
-> System.Collections.Generic.List`1[System.String]
Lillisp> (Dictionary Int32 Guid)
-> System.Collections.Generic.Dictionary`2[System.Int32,System.Guid]
Lillisp> (def x (new (List String)))
-> x
Lillisp> (.Add x "foo")
-> null
Lillisp> (.Add x "bar")
-> null
Lillisp> x
-> ("foo" "bar")
```

Note that generic methods are not yet supported.

### .NET Types

All variables are boxed to `System.Object`.

You can get the type of a variable with either `(.GetType var)` or `(typeof var)`. Notice how `typeof` operates more like JavaScript than C#. 
There is no need to use a keyword to get a .NET `System.Type` reference: just use the type name directly. This enables convenient use of the `new` function. 
This also potentially enables interesting runtime reflection scenarios, such as using `new` with a variable of the type to instantiate, without a bunch of `Activator.CreateInstance` boilerplate.

You can also cast using the `cast` function. Examples:

```lisp
Lillisp> (.GetType "foo")
-> System.String
Lillisp> (typeof "foo")
-> System.String
Lillisp> Int32
-> System.Int32
Lillisp> (.GetType Int32)
-> System.RuntimeType
Lillisp> (def t Uri)
-> t
Lillisp> (new t "https://www.google.com")
-> https://www.google.com/
Lillisp> (typeof 7)
-> System.Double
Lillisp> (typeof (cast 7 Int32))
-> System.Int32
```

Common Lillisp to .NET type mappings:
| Lillisp type | .NET type |
| --- | --- |
| list/pair | `Lillisp.Core.Pair` |
| vector | `Lillisp.Core.Vector` (wraps a `System.Collections.Generic.List<System.Object?>`) |
| bytevector | `Lillisp.Core.Bytevector` (wraps a `System.Collections.Generic.List<System.Byte>`) |
| boolean (i.e. `true` or `#t`) | `System.Boolean` |
| integer numbers (i.e. `7`) | `System.Int32` |
| real numbers (i.e. `42.03` or `1.1e-10`) | `System.Double` |
| rational numbers (i.e. `3/8`) | [`Rationals.Rational`](https://github.com/tompazourek/Rationals) |
| complex numbers (i.e. `-4+7i`) | `System.Numerics.Complex` |
| character | `System.Char` |
| constant string | `System.String` |
| mutable string (i.e. with `(make-string)`) | `System.Text.StringBuilder` |

## Creating Types

C#-like "record" types can be defined with the `defrecord` function. 
Note that these are true .NET class types, and are not to be confused with Scheme records.

The `defrecord` form is: `(defrecord TypeName *properties)`

`*properties` is one or more property definitions. A property definition is either of the form `Name` (for an `object`-typed property) or `(Name Type)` where `Type` is a .NET type name.

Example: `(defrecord Customer (Id Int32) (Name String))`

Each property is generated on the new record type with a private backing field, and a constructor parameter in the order specified. 
Properties without a type specified will be of type `System.Object`.

In addition, a `ToString` implementation is generated, along with equality members. Two records with the same values will not be `eq?` (aka reference equals)
but will be `eqv?` (aka value-wise equivalent).

Records are IL-emitted at runtime, when the type is defined. This means they are real .NET types, and so you can create a `(List Customer)` that is strongly-generic-typed to the newly-created `Customer` type.

```lisp
Lillisp> (defrecord Customer (Id Int32) (Name String) (Balance Double))
-> Customer
Lillisp> (new Customer 123 "Foo Bar" 2021.11)
-> Customer { Id = 123, Name = Foo Bar, Balance = 2021.11 }
Lillisp> (def c (new Customer 234 "Fizz Buzz" 123.45))
-> c
Lillisp> (.Id c)
-> 234
Lillisp> (.Name c)
-> "Fizz Buzz"
Lillisp> (def c2 (new Customer 234 "Fizz Buzz" 123.45))
-> c2
Lillisp> (eq? c c2)
-> False
Lillisp> (eqv? c c2)
-> True
```

You can also create .NET enum tyes with `defenum`. The first argument is the enum type name, followed by its values. Just like records, this is a real .NET enum type, dynamically IL-generated at runtime. 

Example:

```lisp
Lillisp> (defenum Fruit Apple Banana Cranberry Date Elderberry)
-> Fruit
Lillisp> Fruit/Apple
-> Apple
Lillisp> (def myfruit Fruit/Apple)
-> myfruit
Lillisp> (str myfruit)
-> "Apple"
Lillisp> (eqv? myfruit Fruit/Apple)
-> True
Lillisp> (eqv? myfruit Fruit/Banana)
-> False
Lillisp> (Enum/GetValues Fruit)
-> (Apple Banana Cranberry Date Elderberry)
Lillisp> (cast 4 Fruit)
-> Elderberry
Lillisp> (cast Fruit/Cranberry Int32)
-> 2
```

## LispINQ

Work has started on adding a LINQ-like syntax to Lillisp, called LispINQ. Currently `from`, `where`, `orderby`, `thenby`, and `select` are supported to some degree.

This is probably best expressed with an example:

```lisp
Lillisp> (defrecord Customer (Id Int32) (Name String))
-> Customer
Lillisp> (use 'System.Linq)
-> ()
Lillisp> (use 'System.Collections.Generic)
-> ()
Lillisp> (def mylist (new (List Customer)))
-> mylist
Lillisp> (.Add mylist (new Customer 123 "Foo Bar"))
-> null
Lillisp> (.Add mylist (new Customer 234 "Fizz Buzz"))
-> null
Lillisp> (.Add mylist (new Customer 345 "Buzz Lightyear"))
-> null
Lillisp> (.ToArray (from i in mylist where (.StartsWith (.Name i) "F") orderby (.Id i) desc select (.Id i)))
-> (234 123)
Lillisp> (.ToArray (from i in mylist where (eqv? (% (.Id i) 2) 1) orderby (.Id i) desc select (.Id i)))
-> (345 123)
```

Note that the example above also makes use of dynamic dispatch of extension methods, with the `.ToArray` call. 
And as you can see in the last line, you can mix and match Lillisp expressions with .NET interop in LispINQ expressions.

Unlike C#, to add an additional `orderby` clause, you'll add a `thenby` for any subsequent ordering expressions.
e.g.: `(from i in mylist orderby (.Name i) thenby (.Id i) desc select i)`. Also note that LispINQ uses `desc` to indicate descending, while C# uses `descending`.
