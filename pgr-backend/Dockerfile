# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# --- CORREÇÃO AQUI ---
# Como o .csproj está dentro da pasta 'pgr', precisamos especificar o caminho
COPY ["pgr/pgr.csproj", "pgr/"]
RUN dotnet restore "pgr/pgr.csproj"

# Copia todo o resto dos arquivos
COPY . .

# --- CORREÇÃO AQUI ---
# Entramos na pasta pgr para rodar o build
WORKDIR "/src/pgr"
RUN dotnet build "pgr.csproj" -c Release -o /app/build

# Estágio de Publicação
FROM build AS publish
RUN dotnet publish "pgr.csproj" -c Release -o /app/publish

# Estágio Final (Execução)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Essa linha que você colocou é ótima para o Render, mantive ela:
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "pgr.dll"]