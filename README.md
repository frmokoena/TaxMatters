# Tax Matters

## Introduction

The solution offers an application for performing tax calculations. It utilizes ASP.NET Core Web API and ASP.NET Core Web App (Razor Pages). The API exposes various endpoints that can be found in the Swagger UI (i.e. `https:/localhost:5443/swagger`). For example, a request to calculate taxes would look like:

    POST /services/TaxCalculations/calculate
    {
      "postalCode": "0042",
      "annualIncome": 500000
    }

## Table Of Contents

- [Tax Matters](#Tax-matters)
  - [Introduction](#introduction)
  - [Table Of Contents](#table-of-contents)

- [Usage](#usage)
  - [Prerequisites for building the project](#prerequisites-for-building-the-project)
  - [Building the solution](#building-the-solution)
  - [Running the application](#running-the-application)
- [Architecture and Design Decisions](#architecture-and-design-decisions)
  - [The API Application](#the-api-application-taxmattersapi)
    - [The API Security](#the-api-security)  
  - [The Core API Application](#the-core-api-application-taxmattersapicore)
  - [The Client Project](#the-client-project-taxmattersclient)
  - [The Domain Project](#the-domain-project-taxmattersdomain)
  - [The Infrastructure Project](#the-infrastructure-project)
  - [The Web Application](#the-web-application-taxmattersweb)  
  - [The Core Web Application](#the-core-web-application-taxmatterswebcore)
  - [The Test Projects](#the-test-projects)
  - [Concurrency Conflicts](#concurrency-conflicts)
  - [Audit Trail](#audit-trail)
- [Patterns Used](#patterns-used)

# Usage

## Prerequisites for building the project:
* The [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) with the `ASP.NET` and web development workload.

## Building the solution

The solution contains the release assets in the file `assets.zip`. The assets can be unzipped and this section can be skipped.

To build and run the project, open a command prompt to the root of the solution, and perform the following: 

1. Run tests

       dotnet test


2. Publish the API project to a folder:

    ```
    cd ./src/Tax.Matters.API
    dotnet publish -o ../../assets/api
    cd ..
    ```

3. Publish the Web project to a folder:

    ```
    cd ./src/Tax.Matters.Web 
    dotnet publish -o ../../assets/web

## Running the application

1. To run the application, open a command prompt (each for api and the web, respectively) to the `assets` folder created from the previous step (should be at the root of the solution), or extracted from the `assets.zip`, and run the executable. The API initializes the database ([LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)) with the initial test data during startup.

   - From the API Command Prompt:     

        ```
        cd ./api/
        dotnet Tax.Matters.API.dll --urls "https://localhost:5443"        
        ```

   - From the Web app Command Prompt:

        ```
        cd ./web
        dotnet Tax.Matters.Web.dll --urls "https://localhost:50443"
        ```

  2. The `api` listen to `https` port `5443` and the web listen to `https`port `50443`.

  3. Open your favourite browser and visit `https://localhost:50443`

  `Note:`

  > To test the api only, e.g. via Postman, visit the [security section](#the-api-security) of the api.

# Architecture and Design Decisions

The development follows a clean architecture. The core applications are loosely coupled and any client application (e.g. Razor, Angular, Vue, etc.) can be integrated into the architecture with minimal effort. In this way, the core applications can be scaled independently of the clients. The solution projects are briefly described below.

## The API Application (`Tax.Matters.API`)

This is the Api project. It provides the Api endpoints for the clients. The Api is protected by an Api key to prevent unauthorised use. This can easily be changed to `OAuth` as it is a pluggable middleware.

### The API Security

Authorised access is required for all endpoints.

To deter unauthorised access, all unauthorised access will receive the following response (this can be changed to any other preferred response, including the 401 response):

    {
        "title": "Not Found",
        "description": "Endpoint not found",
        "statusCode": 404
    }

The Api also uses the policy-based authorization. `i.e.` Only the `web` client is allowed access to tax management endpoints:

    /services/TaxManagement/*`


The api can be tested with the 2 clients provided in the `appsettings.json`.

    "AuthorizedClients": {
      "Clients": [
        {
          "name": "Web",
          "key": "d5a548b2-50df-40d9-88d3-e366d833892f"
        },
        {
          "name": "thirdparty",
          "key": "a110ab4f-1cf1-478e-b78b-4a09b50b3e73"
        }
      ]
    }

 The authentication is passed into the authentication header field:

     Authorization: Basic <credentials>

where `<credential>` is `base64` of `name:key`

## The Core API Application (`Tax.Matters.API.Core`)

This project hosts the business logic of the Api. This allows us to have thin controllers. Too much logic in a controller violates the single responsibility principle (SRP) of the `SOLID` principles.

## The Client Project (`Tax.Matters.Client`)

The services for the integration are provided here. The web frontend project uses the services for integration with the API. The services are client-independent. The API can also reuse the services for integration with third-party services. This contributes to compliance with the **DRY** principle: Maintain in one place and use everywhere.

## The Domain Project (`Tax.Matters.Domain`)

This houses the domain entities and models.

## The Infrastructure Project

This houses the infrastructure and repositories for the data access.

## The Web Application (`Tax.Matters.Web`)

This is the entry point for the web front-end application. This is the ASP.NET Core Razor Pages web application, which is responsible for the UI logic. The app configurations are contained in the `appsettings.json` file.

## The Core Web Application (`Tax.Matters.Web.Core`)

The project hosts the business logic of the web application, which is mainly the integration calls to the API. The actual business rules of the application are implemented in the API. This allows us to switch to another front-end framework such as Angular or Vue without affecting the business logic. The core web abstraction allows our pages to focus on the display logic.

## The Test Projects

Each project has a corresponding test project.

## Concurrency Conflicts

The system implements optimistic concurrency via concurrency token, `Version`. i.e. The data modification fail on save if the data has changed since it was queried.

In the code below, the `[Timestamp]` attribute maps a property to a SQL Server `rowversion` column. Since `rowversion` automatically changes when the row is updated, this provides a very useful concurrency token with minimum-effort that protects the entire row.

    public class IncomeTax
    {
      public string Id { get; set; }
      public ICollection<ProgressiveIncomeTax> ProgressiveTaxTable { get; set; } = [];

      [Timestamp]
      public byte[] Version { get; set; }
    }

## Audit Trail

The system maintains a complete log of all operations that alter the state of auditable entities (i.e., `Add`, `Modify`, `Delete`).

# Patterns Used

The solution architecture adopts the CQRS (Command and Query Responsibility Segregation) pattern, along with the mediator and repository patterns.

_**CQRS pattern**_: Separates write commands from read queries. 

_**Mediator pattern**_:  Ensures that components are loosely coupled by preventing objects from explicitly referencing each other.

_**Repository pattern**_: Design pattern that keeps persistence concerns separate from the domain model of the system.