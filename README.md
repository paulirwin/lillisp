# Lillisp

This repo contains the Lillisp core runtime (which includes the standard library) and the Lillisp REPL.

## What is Lillisp?

Lillisp (prounounced "lill-isp", like "lilly") is a prototype Lisp-based language that is written in C# (with some library functions written in Lillisp itself) and runs on [.NET](https://dotnet.microsoft.com/). 

Currently, Lillisp can call some .NET code, but is not yet callable *from* .NET code. Also, Lillisp is currently only interpreted as a REPL. The short term plan is to allow for interpretation of Lillisp files too, and the long term plan is to support compilation of Lillisp code into a .NET assembly.

Lillisp is a Scheme-based Lisp, and ultimately aims to be as R7RS compliant as possible. Being a Scheme, Lillisp is a Lisp-1, meaning functions and variables/parameters cannot share the same name.

Lillisp also draws inspiration from Clojure, and uses its syntax in part, such as with .NET interop.

Lillisp started as a C# implementation of Peter Norvig's lis.py from the blog post [(How to Write a (Lisp) Interpreter (in Python))](https://norvig.com/lispy.html). Many thanks to Peter for the excellent tutorial that inspired this project.

## Using the REPL

Build and run the Lillisp.Repl project in Visual Studio 2019 or later, or via the `dotnet` CLI.

Input commands and hit enter. Note that multi-line input is not yet supported.

Certain REPL commands may be entered, without parentheses. These include:

| Command | Description |
| --- | --- |
| `clear` or `cls` | Clears the console buffer/window and starts a new input line at the top of the buffer. |
| `exit` or `quit` or <kbd>Ctrl</kbd>+<kbd>C</kbd> | Exits the Lillisp REPL. |
| `reset` | Resets the current Lillisp runtime environment, starting fresh. All data in memory will be lost. |

## The Lillisp Language

Full docs coming at some point in the future. But it's basically a normal Lisp with mostly Scheme syntax. Check out LillispRuntime.cs and Library/core.lisp for built-in library methods.

An incomplete list of features currently supported:
* Data types: list, pair (partial support), vector, number, boolean, character, string, symbol, nil, procedure
* Defining variables with `define` (aliased as `def`)
* Mutating variables with `set!`
* Most common math operations (`+`, `-`, `*`, `/`, `abs`, `log`, `sqrt`, etc. - others available in `System.Math`)
* Boolean expressions (`<`, `>`, `<=`, `==`, `and`, `or`, etc)
* List operations (`list`, `car`, `cdr`, `cons`, etc)
* Quoting expressions (i.e. `'(1 2 3)` or `(quote (1 2 3))`)
* Higher-order list functions (`apply`, `map`)
* Conditional logic (`if`, `cond`, `when`)
* Sequential logic with `begin`
* Lambda expressions with `lambda`
* Shorthand for defining a lambda variable (aka a named function) with `defun` (or `define` with a list as the second parameter)
* Block-scoping variables with `let`
* Almost all of the Scheme base library string- and vector-related functions
* Almost all of the Scheme `char`, `CxR`, and `lazy` library functions

Notable features not yet implemented from Scheme R7RS include:
* Proper tail recursion
* Quasiquoting and unquoting
* Macros
* Dotted pairs
* Bytevectors
* Exceptions
* Pipes
* Libraries (as in, i.e. `import`)
* Ports and I/O
* System interface
* Many base library methods, and other libraries

Basically, give your existing Lisp code a try, and if a given feature doesn't work, file an issue.

## .NET Interop

Lillisp uses Clojure-like syntax for interop with .NET types. Any .NET object can be stored to a Lillisp variable. (Note: all values are boxed to `System.Object`.)

.NET Interop is *extremely experimental* and *very fragile*.

### Static Methods

You can call static methods on types with the `/` character in place of i.e. a `.` character in C#. Examples:

```lisp
Lillisp> (String/IsNullOrWhiteSpace "foo")
-> False
Lillisp> (String/IsNullOrWhiteSpace " \t ")
-> True
```

Note: overload resolution does not currently work for static methods.

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
Lillisp> (.Next rnd (cast 100 Int32))
-> 73
Lillisp> (.Next rnd (cast 100 Int32))
-> 55
```

Note that the `cast` above is currently required since all numbers in Lillisp are of type `System.Double`, and there is no overload of `System.Random.Next` that takes a `System.Double`.

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

Currently, only the .NET 5 Base Class Library is available. Any namespaces can be "imported" (like the `using` statement in C#) into the current environment with the `use` keyword and a quoted symbol of the namespace name. Examples:

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