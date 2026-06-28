# =========================================================
# Dockerfile - finansee-api (.NET 10)
# Build multi-stage: compila em uma imagem com o SDK completo
# e roda em uma imagem final só com o runtime (bem mais leve).
# =========================================================

# ---------- STAGE 1: build ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# --- CASO A: projeto único (.csproj direto na raiz) ---
# Copia só o .csproj primeiro pra aproveitar cache do Docker
# (se o código mudar mas as dependências não, não reinstala tudo de novo)
COPY *.csproj ./
RUN dotnet restore

# Copia o resto do código e publica
COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

# ---------- STAGE 2: runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Porta padrão que o Kestrel escuta dentro do container desde o .NET 8
EXPOSE 8080

# Copia somente o resultado da publicação (sem SDK, sem código fonte)
COPY --from=build /app/publish .

# Troque "finansee-api.dll" pelo nome real do seu projeto/assembly
# (geralmente é <NomeDoProjeto>.dll, igual ao .csproj)
ENTRYPOINT ["dotnet", "finansee-api.dll"]
