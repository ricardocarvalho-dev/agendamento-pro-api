
# Guia de Deploy Blazor Front-End para Azure

Este guia detalha os passos para realizar o deploy de um projeto Blazor WebAssembly para o Azure, utilizando Azure Static Web Apps ou Azure App Service, e como configurar a URL da API.

## Opção 1: Azure Static Web Apps (Recomendado para Blazor WebAssembly)

O Azure Static Web Apps é ideal para aplicações Blazor WebAssembly, pois oferece hospedagem de conteúdo estático, integração com APIs (Azure Functions) e CI/CD integrado.

### Pré-requisitos:
- Conta Azure ativa.
- Azure CLI instalado e configurado (`az login`).
- Repositório Git (GitHub, Azure DevOps) com o código-fonte do projeto Blazor.

### Passos para Deploy:

1.  **Crie um novo recurso Static Web App no Azure:**

    ```bash
    az staticwebapp create \
        --name <nome-do-seu-aplicativo> \
        --resource-group <seu-grupo-de-recursos> \
        --source <local-do-repositorio> \
        --location <regiao> \
        --output-location "wwwroot" \
        --app-location "AgendamentoPro.FrontEnd/AgendamentoPro.FrontEnd" \
        --api-location "" \
        --branch <sua-branch-principal>
    ```

    - `--name`: Nome único para o seu Static Web App.
    - `--resource-group`: Grupo de recursos existente.
    - `--source`: URL do seu repositório Git (ex: `https://github.com/seu-usuario/seu-repo`).
    - `--output-location`: O diretório de saída da build do Blazor WebAssembly (geralmente `wwwroot`).
    - `--app-location`: O caminho para a pasta raiz do seu projeto Blazor dentro do repositório (ex: `AgendamentoPro.FrontEnd/AgendamentoPro.FrontEnd`).
    - `--api-location`: Deixe vazio se a API estiver separada (como neste caso).
    - `--branch`: A branch principal do seu repositório (ex: `main` ou `master`).

2.  **Configurar a URL da API (Variáveis de Ambiente):**

    Para que o Front-End Blazor se comunique com a API, você precisa configurar a URL da API como uma variável de ambiente no Azure Static Web Apps.

    No portal do Azure, navegue até o seu Static Web App, vá em "Configuration" (Configuração) e adicione uma nova configuração de aplicativo:

    - **Name:** `ApiBaseUrl` (ou o nome que você usa no `Program.cs` para configurar o `HttpClient`)
    - **Value:** `https://agendamentopro-api-ricardo-staging-hzhzhfddb7djbmd6.centralus-01.azurewebsites.net` (ou a URL da sua API de produção/staging)

    No seu `Program.cs` do Blazor, você pode acessar essa variável de ambiente da seguinte forma:

    ```csharp
    var builder = WebAssemblyHostBuilder.CreateDefault(args);
    // ...
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress;
    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
    // ...
    ```

    **Observação:** Para ambientes de desenvolvimento local, você pode configurar `ApiBaseUrl` no `appsettings.json` ou `appsettings.Development.json`.

3.  **Configuração de CI/CD (GitHub Actions/Azure DevOps):**

    O Azure Static Web Apps automaticamente configura um workflow de CI/CD no seu repositório Git. Qualquer push para a branch configurada (`--branch`) irá disparar uma nova build e deploy.

## Opção 2: Azure App Service (Para Blazor Server ou Blazor WebAssembly com necessidades específicas)

Se você optou por Blazor Server ou tem requisitos mais complexos que o Static Web Apps não atende, pode usar o Azure App Service.

### Pré-requisitos:
- Conta Azure ativa.
- Azure CLI instalado e configurado (`az login`).

### Passos para Deploy:

1.  **Publique seu projeto Blazor:**

    No terminal, navegue até a pasta do seu projeto Blazor (ex: `AgendamentoPro.FrontEnd/AgendamentoPro.FrontEnd`) e execute:

    ```bash
    dotnet publish -c Release -o ./publish
    ```

    Isso criará os arquivos de deploy na pasta `publish`.

2.  **Crie um novo App Service no Azure:**

    ```bash
    az webapp up \
        --resource-group <seu-grupo-de-recursos> \
        --name <nome-do-seu-aplicativo> \
        --location <regiao> \
        --runtime "DOTNET|8.0" \
        --sku F1 \
        --os Windows \
        --path ./publish
    ```

    - `--name`: Nome único para o seu App Service.
    - `--resource-group`: Grupo de recursos existente.
    - `--location`: Região do Azure.
    - `--runtime`: Tempo de execução do .NET (ajuste conforme a versão do seu projeto).
    - `--sku`: Plano de preços (F1 é o plano gratuito para testes).
    - `--os`: Sistema operacional (Windows ou Linux).
    - `--path`: Caminho para a pasta de publicação (`./publish`).

3.  **Configurar a URL da API (Variáveis de Ambiente):**

    No portal do Azure, navegue até o seu App Service, vá em "Configuration" (Configuração) -> "Application settings" (Configurações do aplicativo) e adicione uma nova configuração:

    - **Name:** `ApiBaseUrl`
    - **Value:** `https://agendamentopro-api-ricardo-staging-hzhzhfddb7djbmd6.centralus-01.azurewebsites.net`

    No seu `Program.cs` do Blazor, o acesso é o mesmo da Opção 1.

## Considerações Finais

- **HTTPS:** Certifique-se de que sua API esteja usando HTTPS para evitar problemas de segurança e mixed content no navegador.
- **CORS:** Configure o CORS na sua API para permitir requisições do domínio do seu Front-End Blazor.
- **Ambientes:** Utilize diferentes configurações de ambiente (desenvolvimento, staging, produção) para as URLs da API.
