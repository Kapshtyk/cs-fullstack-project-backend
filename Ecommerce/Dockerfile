FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY Ecommerce.sln .
COPY Ecommerce.Controllers/Ecommerce.Controllers.csproj Ecommerce.Controllers/
COPY Ecommerce.Infrastructure/Ecommerce.Infrastructure.csproj Ecommerce.Infrastructure/
COPY Ecommerce.Domain/Ecommerce.Domain.csproj Ecommerce.Domain/
COPY Ecommerce.Services/Ecommerce.Services.csproj Ecommerce.Services/
COPY Ecommerce.Tests/Ecommerce.Tests.csproj Ecommerce.Tests/

RUN dotnet restore

COPY . .

WORKDIR /app/Ecommerce.Infrastructure
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Ecommerce.Infrastructure.dll"]