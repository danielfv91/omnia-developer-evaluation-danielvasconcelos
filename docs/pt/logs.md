# Logs da Aplicação

Este documento descreve como a aplicação implementa o registro de logs utilizando **Serilog**, incluindo logs estruturados, roteamento por contexto e arquivos separados para eventos e exceções.

## Configuração Geral

A configuração de logging está centralizada em `Ambev.DeveloperEvaluation.Common.Logging.LoggingExtension`.

Utiliza-se o pacote **Serilog**, com enriquecimento de contexto, suporte a exceções detalhadas e arquivos separados para logs por categoria.

A configuração é registrada em `Program.cs` com:

```csharp
builder.AddDefaultLogging();
app.UseDefaultLogging();
```

## Arquivos de Log

São gerados automaticamente na pasta `logs/` com os seguintes arquivos:

- `app-log-<data>.txt`: log geral da aplicação.
- `event-log-<data>.txt`: log específico de eventos de domínio (como criação e modificação de vendas).
- `exception-log-<data>.txt`: log de exceções não tratadas capturadas por middleware global.

## Eventos

Para publicar logs de eventos em `event-log-*.txt`, usamos o `EventPublisher` com `SourceContext`:

```csharp
private static readonly ILogger _logger = Log.ForContext("SourceContext", "EventPublisher");
_logger.Information("Event published: {@Event}", @event);
```

## Exceções

Para registrar exceções, usamos o `ExceptionMiddleware`, também com `SourceContext`:

```csharp
private static readonly ILogger _logger = Log.ForContext("SourceContext", "ExceptionMiddleware");
_logger.Error(ex, "Unhandled exception");
```

Esse middleware garante que falhas inesperadas sejam logadas e uma resposta genérica seja enviada ao cliente.

## Extensibilidade

A configuração permite adicionar logs contextuais personalizados via o método `AddContextualLogs`, que adiciona novos `WriteTo.Logger` com base no `SourceContext`.

Exemplo:

```csharp
.AddContextualLogs(new Dictionary<string, string>
{
    ["EventPublisher"] = "event",
    ["ExceptionMiddleware"] = "exception"
});
```

## Conclusão

A solução implementada cobre:

- Logs estruturados e contextuais
- Arquivos separados para eventos e exceções
- Registro de requisições com enriquecimento
- Middleware global de tratamento de erros

Essa abordagem garante rastreabilidade, organização e possibilidade de auditoria e análise futura.
