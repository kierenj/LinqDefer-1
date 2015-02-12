# LinqDefer
LinqDefer is a .NET library which extends LINQ query handling for data access, allowing for expressions not otherwise supported by the provider.  For example, it allows you to use methods and expressions which would not otherwise be supported by Entity Framework:

    context.People
        .OrderBy(p => p.LastName)
        .Take(10)
        .Select(p => string.Format("{0}, {1}", p.Lastname, p.Firstname)
        .ThenDoDeferred();
        
Simply put, it is a solution to the exception, "**LINQ to Entities does not recognise the method 'foo'**".

LinqDefer is available as a NuGet package: can be installed using the following in the Package Manager Console in Visual Studio:

    Install-Package LinqDefer

Features
--
* MIT licensed
* Unit tests with 100% code coverage on key classes
* Designed to work with *Entity Framework*, but not specifically - integrates with LINQ
* Designed to work with *AutoMapper* and the `Project().To<>()` extension
* Configurable expression analysis

Building
--
A Visual Studio solution file is included, but you can also build from the commandline using:

    ./build Debug|Release 1.0.0.0

How it works
--
Each LINQ query has an underlying `Expression`.  Typically, this expression is translated to another language or into another domain by a LINQ provider - for example, Entity Framework.

LinqDefer works by intercepting expressions, modifying and simplifying them and passing the result to the underlying provider.  As results are received from the provider, the original expression is used to rebuild the expected results.  Analysis of expressions - to see what parts can be passed to the underlying provider, and which ones should be defered and executed afterwards - is delegated to a implementation of `IExpressionAnalyser`.

Currently, the only implemented expression analyser is `DataAccessOnlyAnalyser`.  This will pass simple data-access expressions, such as the examples below, to the underlying provider.  All other parts of the expression are evaluated after the results are received.

* Direct access of an object: `p => p`
* Accessing a member of the object: `p => p.Name`
* Calling a method of the object: `p => p.Name.ToUpper()` - note, extension methods are not passed down, but are deferred
* Accessing an array index: `p => p[0]`

These checks are performed recusively, for example, `Select(p => p.Owner.Name.ToUpper()[0])` would be passed in its entirety to the underlying provider.

In other words, the following:

    // throws exception: provider doesn't understand "string.Format()"
    source
        .Select(p => string.Format("{0}, {1}", p.LastName.ToUpper(), p.FirstName))
    
..Is transformed automatically into this:

    source
        .Select(p => new { f0 = p.LastName.ToUpper(), f1 = p.FirstName }) // retrieve required data
        .ToList()                                                         // materialise: runs the query
        .Select(t => string.Format("{0}, {1}", t.f0, t.f1))               // run any deferred expressions

This extends to all kinds of expressions: new objects, arrays, method calls, unary and binary operators.

This means there is no need to query the database to produce an intermediate object, then construct another object immediately afterwards.

Sample app
--
A sample console application using Entity Framework is included.  It logs the SQL generated by Entity Framework to the console, making it possible to see the mechanism in action.
