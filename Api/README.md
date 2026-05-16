# AgendamentoPro - API de Agendamentos Superior

Este projeto foi desenvolvido com o objetivo de consolidar práticas avançadas de arquitetura de software e design de sistemas voltados para alta escalabilidade, resiliência e manutenibilidade em ecossistemas .NET modernos.

O sistema resolve o problema de agendamentos distribuídos utilizando conceitos de design guiado por domínio e desacoplamento de serviços.

## 🏛️ Arquitetura e Design Pattern

O projeto adota os princípios de **Clean Architecture** combinados com **DDD (Domain-Driven Design)**, garantindo o isolamento completo das regras de negócio em relação a frameworks externos, bancos de dados ou provedores de nuvem.



### Camadas do Sistema:
* **Domain:** O coração do sistema. Contém as entidades de negócio, agregados, objetos de valor (Value Objects), exceções de domínio e as interfaces (contratos) dos repositórios. Totalmente livre de dependências externas.
* **Application:** Implementa os casos de uso (Use Cases) da aplicação. Responsável por orquestrar o fluxo de dados, utilizar os DTOs para comunicação e aplicar as regras de negócio através dos serviços de domínio.
* **Infrastructure:** Contém as implementações técnicas de acesso a dados (Entity Framework Core), configurações de persistência, integrações com brokers de mensageria e serviços externos.
* **Api (Presentation):** Porta de entrada do sistema. Uma API HTTP REST otimizada que expõe os endpoints, configura a injeção de dependência global e gerencia os ambientes de execução.

---

## 🛠️ Tecnologias e Ecossistema

* **Runtime:** .NET 8 (LTS)
* **ORM:** Entity Framework Core 8.0.0
* **Bancos de Dados Suportados (Multi-Environment):**
  * **Development (Local):** SQLite (Persistência leve em arquivo para agilidade no desenvolvimento local).
  * **Staging / Production:** SQL Server / Azure SQL (Configuração robusta para ambientes de nuvem).
* **Documentação:** Swagger / OpenAPI

---

## ☁️ Estratégia Cloud Native & DevOps

A solução foi projetada para portabilidade imediata para a nuvem através de:
1. **Multi-banco dinâmico:** Isolamento por variáveis de ambiente (`ASPNETCORE_ENVIRONMENT`), permitindo chaveamento transparente entre SQLite e SQL Server sem alteração de código.
2. **Mensageria (Roadmap):** Estrutura preparada para desacoplamento assíncrono utilizando **RabbitMQ** com padrões de resiliência (Retry e Dead Letter Exchange - DLQ).

---

## 🚀 Como Executar o Projeto Localmente

1. Certifique-se de ter o **SDK do .NET 8** instalado.
2. Clone o repositório.
3. Navegue até a raiz do projeto e execute a restauração de dependências e inicialização:

```bash
dotnet restore
dotnet run --project Api