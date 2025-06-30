# Ganti base image dengan SDK .NET 9.0
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MyProject.csproj", "."]
RUN dotnet restore "MyProject.csproj"
COPY . .
RUN dotnet build "MyProject.csproj" -c Release -o /app/build

# Gunakan runtime .NET 9.0
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "MyProject.dll"]