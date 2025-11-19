# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["pgr.csproj", "./"]
RUN dotnet restore "pgr.csproj"
COPY . .
RUN dotnet build "pgr.csproj" -c Release -o /app/build

# Estágio de Publicação
FROM build AS publish
RUN dotnet publish "pgr.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio Final (Execução)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .
# A linha abaixo é crucial para o Render saber qual porta usar
ENV ASPNETCORE_URLS=http://+:8080 
ENTRYPOINT ["dotnet", "pgr.dll"]