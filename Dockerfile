FROM mcr.microsoft.com/dotnet/sdk:7.0.302-jammy-amd64 AS buildimg
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY . .
WORKDIR /app/src/API/UrlShortener.Server
RUN dotnet publish -c Release -o /app/output

FROM mcr.microsoft.com/dotnet/aspnet:7.0.5-jammy-amd64
WORKDIR /app/output
COPY --from=buildimg /app/output .
ENTRYPOINT ["dotnet","UrlShortener.Server.dll"]
LABEL org.opencontainers.image.source=https://github.com/AditiKraft/UrlShortener
LABEL org.opencontainers.image.description="Your own URL Shortener: Url Shortener is built uisng API (ASP.NET Core with fastend point), CosmosDb SQL and Auth0 for authentication and UI (Webassembly Blazor UI)"
LABEL org.opencontainers.image.licenses=MIT
