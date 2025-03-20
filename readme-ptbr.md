
# Projeto Avaliação para Desenvolvedor Ambev

Este guia irá ajudá-lo a configurar rapidamente o projeto em seu ambiente local.

---

## Pré-requisitos

Antes de começar, verifique se você possui as seguintes ferramentas instaladas:

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/pt-br/downloads/) ou [Visual Studio Code](https://code.visualstudio.com/)
- [PostgreSQL](https://www.postgresql.org/download/) (recomendado PgAdmin incluso na instalação)
- [Git](https://git-scm.com/downloads)

---

## Configuração Inicial

### 1. Clonar o repositório

```bash
git clone https://github.com/seu-usuario/seu-repositorio.git
cd seu-repositorio
```

### 2. Configurar o banco de dados PostgreSQL

- Abra o PgAdmin e crie um banco de dados com o nome `DeveloperEvaluation`.

### 3. Atualizar a Connection String

No arquivo `appsettings.json` localizado em `src/Ambev.DeveloperEvaluation.WebApi/appsettings.json`, atualize a connection string com suas credenciais do PostgreSQL:

```json
"ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;Database=DeveloperEvaluation;User Id=postgres;Password=sua_senha;"
}
```

### 4. Aplicar Migrations do EF Core

No **Package Manager Console** do Visual Studio, execute:

```powershell
Update-Database -Project Ambev.DeveloperEvaluation.ORM -StartupProject Ambev.DeveloperEvaluation.WebApi
```

Isso criará automaticamente as tabelas necessárias no seu banco PostgreSQL.

---

## Executando a Aplicação

- No Visual Studio, defina o projeto `Ambev.DeveloperEvaluation.WebApi` como projeto de inicialização e pressione **F5**.

- A aplicação será iniciada usando HTTPS (`https://localhost:<porta>`), abrindo automaticamente a interface do Swagger para interação com a API.

### Problema com o Certificado HTTPS

Se ao executar a aplicação você encontrar o erro **"Sua conexão não é particular"** (`ERR_CERT_INVALID`), execute os comandos abaixo no Prompt (como administrador):

```bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

Reinicie o Visual Studio e execute novamente.

---

## Executando os Testes

Você pode executar os testes diretamente pelo Visual Studio ou através do terminal:

```bash
dotnet test
```

---

## Estrutura do projeto

```
src/
├── Ambev.DeveloperEvaluation.Application
├── Ambev.DeveloperEvaluation.Common
├── Ambev.DeveloperEvaluation.Crosscutting
├── Ambev.DeveloperEvaluation.Domain
├── Ambev.DeveloperEvaluation.Infrastructure
├── Ambev.DeveloperEvaluation.IoC
├── Ambev.DeveloperEvaluation.ORM
└── Ambev.DeveloperEvaluation.WebApi

tests/
├── Ambev.DeveloperEvaluation.Unit
├── Ambev.DeveloperEvaluation.Integration
└── Ambev.DeveloperEvaluation.Functional
```

---

## Tecnologias Utilizadas

- .NET 8
- Entity Framework Core
- PostgreSQL
- xUnit (testes)
- MediatR, AutoMapper, Rebus
- Git (controle de versão)

---

**Agora você está pronto para começar!**
