# ─── Estágio 1: Build ────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia os arquivos de projeto e restaura dependências
COPY ["OrderManagement.API/OrderManagement.API.csproj", "OrderManagement.API/"]
COPY ["OrderManagement.Application/OrderManagement.Application.csproj", "OrderManagement.Application/"]
COPY ["OrderManagement.Domain/OrderManagement.Domain.csproj", "OrderManagement.Domain/"]
COPY ["OrderManagement.Infrastructure/OrderManagement.Infrastructure.csproj", "OrderManagement.Infrastructure/"]

RUN dotnet restore "OrderManagement.API/OrderManagement.API.csproj"

# Copia todo o código e faz o build
COPY . .
WORKDIR "/src/OrderManagement.API"
RUN dotnet build "OrderManagement.API.csproj" -c Release -o /app/build

# ─── Estágio 2: Publish ──────────────────────────────────────
FROM build AS publish
RUN dotnet publish "OrderManagement.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ─── Estágio 3: Runtime ──────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Cria pasta de logs
RUN mkdir -p /app/logs

COPY --from=publish /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "OrderManagement.API.dll"]