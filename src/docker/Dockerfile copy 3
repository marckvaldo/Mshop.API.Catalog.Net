# Etapa de build (compilação com o SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Definir diretório de trabalho para a aplicação
WORKDIR /src

# Copiar os arquivos da solução para o container
COPY ../ . 

# Restaurar as dependências
RUN dotnet restore Mshop.API.Catalog/Mshop.API.Catalog.csproj

# Publicar o projeto
RUN dotnet publish Mshop.API.Catalog/Mshop.API.Catalog.csproj -c Release -o /app/Mshop.API.Catalog

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Criar um diretório para a aplicação
WORKDIR /app

# Criar um usuário não-root e definir permissões
RUN useradd -m -s /bin/bash appuser
RUN chown -R appuser:appuser /app

# Copiar os arquivos publicados do container de build
COPY --from=build /app/Mshop.API.Catalog /app

# Mudar para o usuário não-root
USER appuser

# Expor porta padrão para a API
#EXPOSE 80

# Configurar o ponto de entrada
ENTRYPOINT ["dotnet", "Mshop.API.Catalog.dll"]
