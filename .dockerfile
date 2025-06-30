FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MyProject.csproj", "."]
RUN dotnet restore "MyProject.csproj"
COPY . .
RUN dotnet build "MyProject.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "MyProject.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyProject.dll"]