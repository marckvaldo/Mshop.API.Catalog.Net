# Etapa de build (compilação com o SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Definir diretório de trabalho para a aplicação
WORKDIR /src

# Copiar os arquivos da solução para o container
COPY ../ . 

# Restaurar as dependências
RUN dotnet restore Mshop.API.Catalog/Mshop.API.Catalog.csproj
RUN dotnet restore Mshop.GraphQL.API/Mshop.API.GraphQL.csproj
RUN dotnet restore MShop.gRPC.Catalog/Mshop.gRPC.Catalog.csproj

# Publicar o projeto
RUN dotnet publish Mshop.API.Catalog/Mshop.API.Catalog.csproj -c Release -o /app/APICatalog
RUN dotnet publish Mshop.GraphQL.API/Mshop.API.GraphQL.csproj -c Release -o /app/APIGraphQL
RUN dotnet publish MShop.gRPC.Catalog/Mshop.gRPC.Catalog.csproj -c Release -o /app/APIgRPC

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Criar um diretório para a aplicação
WORKDIR /app

# Instalar Supervisord
RUN apt-get update && apt-get install -y supervisor && rm -rf /var/lib/apt/lists/*

# Criar um usuário não-root e definir permissões
RUN useradd -m -s /bin/bash appuser
RUN chown -R appuser:appuser /app
RUN mkdir -p /var/log && chown -R appuser:appuser /var/log

# Copiar os arquivos publicados do container de build
COPY --from=build /app /app

# Copiar configuração do supervisord
COPY docker/supervisord.conf /etc/supervisor/conf.d/supervisord.conf

# Mudar para o usuário não-root
USER appuser

CMD ["supervisord", "-c", "/etc/supervisor/conf.d/supervisord.conf"]

