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
  - [The Core API Application](#the-core-api-application)  
  - [The Infrastructure Project](#the-infrastructure-project)
  - [The Core Web Application](#the-core-api-application)  
  - [The Web Project](#the-web-project)
  - [The Test Projects](#the-test-projects)
- [Patterns Used](#patterns-used)
  - [Domain Events](#domain-events)
  - [Related Projects](#related-projects)

# Usage

## Prerequisites for building the project:
* The [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) with the `ASP.NET` and web development workload.

## Building the solution

The solution contains the release assets in the file `assets.zip`. The assets can be unzipped and step `1` below can be skipped.

To build and run the project, open a command prompt to the root of the solution, and perform the following: 

1. Publish the API project to a folder:

    ```
    cd ./src/Tax.Matters.API
    dotnet publish -o ../../assets/api
    cd ..
    ```

2. Publish the Web project to a folder:

    ```
    cd ./src/Tax.Matters.Web 
    dotnet publish -o ../../assets/web

## Running the application

1. To run the application, open a command prompt (each for api and the web, respectively) to the `assets` folder created from the previous step (should be at the root of the solution), or extracted from the `assets.zip`, and run the executable:

   - API Command Prompt:

        ```
        cd ./api/
        dotnet Tax.Matters.API.dll --urls "https://localhost:5443"        
        ```

   - Web Command Prompt:

        ```
        cd ./web
        dotnet Tax.Matters.Web.dll --urls "https://localhost:50443"
        ```

  2. The `api` listen to `https` port `5443` and the web listen to `https`port `50443`.

  3. Open your favourite browser and visit `https://localhost:50443`

# Architecture and Design Decisions

