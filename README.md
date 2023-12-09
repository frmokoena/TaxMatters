# Tax Matters

## Introduction

This application is written in C#/.NET 8.0. It follows a clean architecture. The core applications are loosely coupled and any client application (e.g. Razor, Angular, Vue, etc.) can be integrated into the architecture with minimal effort. In this way, the core applications can be scaled independently of the clients.

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
  - [The Infrastructure Project](##the-infrastructure-project)
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

The solution contains the release assets in the file `assets.zip`. The assets can be unzipped and step `1` below can be skipped.

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

1. To run the application, open a command prompt (each for api and the web, respectively) to the `assets` folder created from the previous step (should be at the root of the solution), or extracted from the `assets.zip`, and run the executable:

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

## The API Application (`Tax.Matters.API`)

This is the Api project. It provides the Api endpoints for the clients. The Api is protected by an Api key to prevent unauthorized use. This can easily be changed to `Oauth` as it is a pluggable middleware.

### The API Security

Authorized access is required for all the endpoints.

To deter unauthorized access, all unauthorized accesses receive the following response (This can be changed to anything else, including the `401` response):

    {
        "title": "Not Found",
        "description": "Endpoint not found",
        "statusCode": 404
    }

The Api also uses the policy-based authorization. `i.e.` Only the `web` client is allowed access to tax management endpoints:

    /services/taxmanagement/*`


The api can be tested with the 2 clients provided in the `appsettings`.

    "AuthorizedClients": {
      "Clients": [
        {
          "name": "web"
          "key": ...
        },
        {
          "name": "integration",
          "key": ...
        }
      ]
    }

 The authentication is passed into the authentication header field:

     Authorization: Basic <credentials>

where `<credential>` is `base64` of `name:apiKey`


## The Core API Application (`Tax.Matters.API.Core`)

This project harbours the business logic of the api. Our controllers are thin. Too much logic in a controller violates the single responsibility principle (SRP) of our `SOLID` principles. The abstraction of the business logic allows our controllers to have only 3 lines.

## The Client Project (`Tax.Matters.Client`)

Services for integration are provided here. The web project uses the services for integration with the API. The services are client-independent. The API can also reuse the services for integration with third-party services. _**The DRY principle**_ - Maintain in one place and deploy everywhere.

## The Domain Project (Tax.Matters.Domain)

This houses the domain entities and models.

## The Infrastructure Project

This houses the infrastructure for the data access.

## The Web Application (`Tax.Matters.Web`)

This is the web frontend. This is .NET Core Razor Pages web app.

## The Core Web Application (`Tax.Matters.Web.Core`)

Just like the core api project, this project harbours the business logic of the web application. Too much logic in page violates our single responsibility principle. The abstraction of the business logic allows our pages focus on display logic.

## The Test Projects

Each project has corresponding test project with the calculate command receiving the most coverage.

## Concurrency Conflicts

The system implements optimistic concurrency via concurrency token, `Version`. i.e. The data modification fail on save if the data has changed since it was queried.

## Audit Trail

There system provides full `Audit Trail` in `AuditLog` entity that logs all the actions that modify entity state (i.e Add, Modify, Delete) on the `auditable` entities.

# Patterns Used

The project follows the CQRS (Command and Query Responsibility Segregation), mediator and repository patterns.

_**CQRS pattern**_: Segregates the operations that handle write requests (commands) from operations that handle read requests (queries). 

_**Mediator pattern**_:  Ensures that components are loosely coupled by preventing objects from explicitly referencing each other.

_**Repository pattern**_: Domain-driven design pattern used to keep persistence concerns outside the domain model of the system.