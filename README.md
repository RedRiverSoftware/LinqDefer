# LinqDefer
LinqDefer is a .NET library which extends LINQ query handling for data access, allowing for expressions not otherwise supported by the provider.  For example, it allows you to use methods and expressions which would not otherwise be supported by Entity Framework:

    context.People
        .OrderBy(p => p.LastName)
        .Take(10)
        .Select(p => string.Format("{0}, {1}", p.Lastname, p.Firstname)
        .ThenDoDeferred();
        
Simply put, it is a solution to the exception, "**LINQ to Entities does not recognise the method 'foo'**".

LinqDefer is available as a NuGet package: can be installed using the following in the Package Manager Console in Visual Studio:

``Install-Package LinqDefer``

Features
--
* MIT licensed
* Unit tests with 100% code coverage on key classes
* Designed to work with *Entity Framework*, but not specifically - integrates with LINQ
* Designed to work with *AutoMapper* and the `Project().To<>()` extension

Building
--
A Visual Studio solution file is included, but you can also build from the commandline using:

    ./build Debug|Release 1.0.0.0
