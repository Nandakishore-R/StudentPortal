# Use the official ASP.NET Core runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["StudentPortal.Web/StudentPortal.Web.csproj", "StudentPortal.Web/"]
RUN dotnet restore "StudentPortal.Web/StudentPortal.Web.csproj"
COPY . .
WORKDIR "/src/StudentPortal.Web"
RUN dotnet publish "StudentPortal.Web.csproj" -c Release -o /app/publish

# Create the runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "StudentPortal.Web.dll"]
