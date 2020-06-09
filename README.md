# Overview of Solution

This solution is intended to solve the requirements of the AGL developer test as per http://agl-developer-test.azurewebsites.net/ but in summary is a simple example of a website consuming a web service, manipulating the data and displaying it.  The idea is to showcase some best practices and to follow common design principles like separation of concerns.

The focus of the solution is on the backend with just a simple frontend to show the output.

The solution is split into three projects:
* Pets.UI - Front end with basic MVC template
* Pets.Application - Contains back end PeopleService for retrieving a list of people and their pets from an external API.
* Pets.Test - Contains tests for the solution.  Only contains tests for the UI read service because the service in the application layer that retrieves data has an external dependency and doesn't actually have any business logic so there isn't much point testing it.

The technology chosen is as follows:
* Pets.UI project targets .NET Core 3.1 and uses ASP.NET Core 3.1 (TODO terminology might need some work here... target vs framework vs metapackage vs project template)
* Pets.Application targets .NET Standard 2.0 for maximum compatability with other projects
* Pets.Test targets .NET Core 3.1 as it is testing functionality in the Pets.UI project which targets .NET Core 3.1

The CI/CD and hosting is as follows:
* Azure DevOps - Used for CI/CD - every commit to master triggers the CI (build, run unit tests and package) and CD (deploy to Azure)
* Azure App Service - https://petswebsite.azurewebsites.net/ (Resource group in Azure, containing an App Service Plan and App Service) 

Details of considerations while developing each layer:

## Pets.UI project

* First .NET Core 3.1 was chosen because this is a Greenfields solution and .NET Core offers many benefits over the .NET Framework (if this was an existing solution being extended the cost/benefit of upgrading it say from .NET Framework to .NET Core would need careful consideration).  Some key benefits of .NET Core over .NET Framework are cross-platform (could deploy to a Unix server to save money), high performance, first class container support, flexibility to deploy with or without the runtime. https://docs.microsoft.com/en-us/dotnet/standard/choosing-core-framework-server
* Version 3.1 of .NET Core was chosen because it is this current version AND it is a Long Term Support (LTS) release. For any new projects I'd generally go with the Current version and then upgrade it to the next LTS release that comes out.  Ideally you'd always be keeping your apps up to date but it depends on priorities of the business. https://dotnet.microsoft.com/platform/support/policy/dotnet-core https://github.com/dotnet/core/blob/master/os-lifecycle-policy.md
* The project structure was to follow the design principle of Separation of Concerns... hence the UI and data retrieval were separated into their own projects https://deviq.com/separation-of-concerns/ ... and in general the S in the SOLID design principles i.e. Single Responsibility Principle - a class should have only one reason to change aka a class should just have one job, making maintenance easier https://www.dotnettricks.com/learn/designpatterns/solid-design-principles-explained-using-csharp

### Startup.cs
* The Dependency Injection design pattern (which implements the D in the SOLID design principles i.e. Dependency Inversion Principle) is used to ensure the code is loosely coupled and hence is more testable and maintainable - the code depends on abstractions rather than details. .NET Core supports DI out of the box via the Startup class ConfigureServices method. https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-3.1 https://dotnettutorials.net/lesson/dependency-injection-design-pattern-csharp/ . If we weren't using .NET Core or if we wanted a DI Container framework that perhaps gave more control, we could consider alternatives like Autofac, Castle Windsor, Ninject or Unity https://www.claudiobernasconi.ch/2019/01/24/the-ultimate-list-of-net-dependency-injection-frameworks/. The dependencies injected include:
  * "Magic strings" (the end point URLs) are in appsettings.json which is made available via the Options pattern to provide strongly typed access to groups of related settings. By isolating the configuration settings into their own classes the app can following the I in SOLID (Interface Segration Principle - so that our classes are only depending on the configuration settings that they use) and Separation of Concerns principle (settings for different parts of the app aren't dependent or coupled to one another) - https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-3.1
  * IHttpClientFactory https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-3.1 is used to provides a central location for naming and configuring logical HttpClient instances.  Its use could be updated a number of ways depending on future HTTP client request requirements - for now I've followed the basic usage (KISS principle).
  * PeopleService is injected so the CatsReadService can get the data it needs to create the view model for the view without being tightly coupled to how that data is retrieved.  
  * CatsReadService is injected so the controller can get the view model without being tightly coupled to the business logic on how that data is pulled together and structured.
  * Both the PeopleService and CatsReadService have a lifetime configured as Singleton i.e. every request uses the same instance.  The bonus is that .NET Core takes care of the object's lifetime, there is not extra code required to implement the Singleton design pattern https://www.dofactory.com/net/singleton-design-pattern

### HomeController.cs
* HomeController is using Action injection with FromServices rather than Constructor injection because nothing else in the controller needs the service - if there were other actions later that depended on the service it could be changed to Constructor injection https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/dependency-injection?view=aspnetcore-3.1 
* The action method is async because the service it is depending on has a network dependency - so by making it async the request thread is not stalled while it waits for the service to return the list of cats (helps prevent request queuing and thread pool growth when the website goes viral and everyone wants to find out the most popular pets names by gender!)  https://docs.microsoft.com/en-us/aspnet/mvc/overview/performance/using-asynchronous-methods-in-aspnet-mvc-4#ChoosingSyncVasync

### CatsReadService.cs
* GetCatOwnersViewModel retrieves the data for and constructs the view model for the HomeController Index action.  It keeps the logic out of the controller and doesn't reside in the Pets.Application project because it has view related concerns.
* For readability the LINQ query is split into two queries.
* While gender order was not specified in the requirements everything should have a defined and reproduceable order, so the assumption I've gone with is to list genders alphabetically.

## Pets.Application project

### PeopleService.cs
* TODO Probably just leave the interface in the application layer and the implementation in new Infrastructure layer since there is no actual business logic - just a straight retrieval of people.

## Pets.Test project

* NUnit used as testing framework - solid framework that's been around. Could have also considered XUnit among others.
* Moq was used for mocking of the PeopleService during testing
* The PeopleService had no logic and so isn't being tested, just the CatsReadService.
* The main thing to test is that the requirements are being met i.e. how the data is being filtered, grouped and ordered, and how cases like no data are handled etc.
* TODO For readability could have considered a library like Shouldly or Fluent Assertions to get more natural sounding assertion statements.
* TODO Add some links to unit testing best practice

## Further considerations:
* Integration tests for PeopleService perhaps.
* Polly to give more robustness to HTTP requests (timeout handling, retries etc).
* etc.

## Azure DevOps
* There are plenty of options for dev ops and CI/CD e.g. Azure DevOps, TeamCity, Jenkins, GitHub Actions etc. but I've chosen Azure DevOps to keep as much in the Microsoft stack as possible, making for easier integrations between source code (GitHub), dev ops (Azure DevOps) and deployment (Azure App Service). https://docs.microsoft.com/en-us/azure/devops/learn/what-is-devops
* Regardless of tools used, setting up CI/CD even for a small project helps ensures code quality by automatically running a build and tests for every checkin and saves time and money by automating the deployments (and rollbacks if required) whether they be manually or automatically triggered. https://nevercode.io/blog/what-is-continuous-integration-and-how-to-benefit-from-it/
* The Azure DevOps pipelines for build and release setup for this project have been built with standard pipeline tasks https://docs.microsoft.com/en-au/azure/devops/pipelines/ecosystems/dotnet-core?view=azure-devops. 
* The build pipeline is triggered by any commit into master branch of the GitHub repository. The process has been simplified - for any production system I'd following something like GitFlow or GitHub Flow (depending on complexity of system and versioning requirements etc) https://nvie.com/posts/a-successful-git-branching-model/ https://guides.github.com/introduction/flow/. The main thing is that any change should have a pull request ideally associated with it to maintain code quality.
* The automated tests are run in the CI via the Visual Studio Tests task "VSTest@2".  I had to make two changes to get this working correctly 1) set it to target the .NET Core 3.1 framework via otherConsoleOptions (it is meant to pick up the target framework of the test project but for some reason this wasn't working) and get it to explicity target the test project DLL (the default DLLs were too broad and it was trying to run tests on non-test DLLs).   
* The release pipeline is triggered by any successful build pipeline completing (it uses the zip file build artificat from the build pipeline) and deploys to a website hosted in Azure as an App Service.  

## Azure App Service
* Azure Service Plan - I've used a Windows host for the Azure Service Plan hosting the App Service but could have chosen Linux to probably save some money without any major issues given we're using .NET Core.
* Azure App Seervice - https://petswebsite.azurewebsites.net/ (Resource group in Azure, containing an App Service Plan and App Service) 
