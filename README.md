### Url Shortener

Your own URL Shortener :  Url Shortener built using API (ASP.NET Core Minima,), PostgreSQL and Auth0 for
authentication and UI (Webassembly Blazor UI)


-----------------

1. [API](https://github.com/iAmBipinPaul/UrlShortener/tree/main/src/API/UrlShortener.Server) : Built on ASP.NET core minimal API (.NET 8)
2. [UI](https://github.com/iAmBipinPaul/UrlShortener/tree/main/src/UI/UrlShortener.UI.Blazor) : Blazor Webassembly

## Getting started

----------------------

### Prerequisites

* [.NET ](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
* [Visual Studio 2022 ](https://visualstudio.microsoft.com/)
  or [VS Code](https://code.visualstudio.com/) or [Rider](https://www.jetbrains.com/rider/)
  (VS 2022)

### Setting up project locally

#### API

1. Configuration

```json 
 "ConnectionStrings": { 
   "DbContext":"Add  connection string here"
   },
  "DefaultUrlForRedirect": "your own default Url for redirect",
  "Auth0_Authority": "your own auth0 authority", // eg  https://tenant1.jp.auth0.com/
  "Auth0_Audience": "your own auth0 audience" // eg portal.github.in
```
_blog for using Auth0 in Webassembly Blazor.([link](https://auth0.com/blog/securing-blazor-webassembly-apps/))_
2. In VS studio or Rider select `UrlShortener.Server` project as startup project and run
3. In VS code can navigate to `src/API/UrlShortener.Server` and then type  ```dotnet run``` and hit enter

#### UI

1. Configuration

```json 
   "RemoteHostUrl": "your API URL",
   "Auth0": {
    "Authority": "your own auth0 authority", // eg  https://tenant1.jp.auth0.com/
    "ClientId": "your own auth0 client Id",
    "Audience": "your own auth0 audience" // eg portal.github.in
  }
```

2. In VS studio or Rider select `UrlShortener.UI.Blazor` project as startup project and run
3. In VS code can navigate to `src/UI/UrlShortener.UI.Blazor` and then type  ```dotnet run``` and hit enter

(In Visual studio you can select multiple project as startup project by right clicking on Solution file and set start up
project)


## Contributing

To improve and optimize create pull request or open issue 
