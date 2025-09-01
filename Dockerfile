FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy project file and restore dependencies first (better Docker layer caching)
COPY src/BMPTec.ChuBank.Api/BMPTec.ChuBank.Api.csproj src/BMPTec.ChuBank.Api/
RUN dotnet restore src/BMPTec.ChuBank.Api/BMPTec.ChuBank.Api.csproj --verbosity normal

# Copy source code and build
COPY src/ src/
WORKDIR /src/src/BMPTec.ChuBank.Api
RUN dotnet clean && dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Set environment variable for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "BMPTec.ChuBank.Api.dll"]
