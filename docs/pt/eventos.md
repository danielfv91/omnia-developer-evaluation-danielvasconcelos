
# Publicação de Eventos no Projeto

## Objetivo

A publicação de eventos permite que diferentes partes do sistema reajam a ações importantes sem acoplamento direto. Isso segue os princípios da Clean Architecture e DDD, especialmente separação de responsabilidades e escalabilidade.

## Localização dos Eventos

Todos os eventos estão localizados na seguinte estrutura de pastas:

```
src/
└── Application/
    └── Events/
        ├── Interfaces/
        │   └── IEventPublisher.cs
        └── Sales/
            ├── ISaleEvent.cs
            ├── SaleCreatedEvent.cs
            ├── SaleModifiedEvent.cs
            ├── SaleCancelledEvent.cs
            └── ItemCancelledEvent.cs
        └── EventPublisher.cs
```

## Interfaces Envolvidas

### `IEventPublisher`

```csharp
public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class;
}
```

## Implementações dos Eventos

Todos os eventos implementam `ISaleEvent`, que inclui campos comuns como:

```csharp
public interface ISaleEvent
{
    Guid SaleId { get; }
    DateTime Timestamp { get; }
}
```

### Tipos de Eventos

- **SaleCreatedEvent** – Publicado ao criar uma venda.
- **SaleModifiedEvent** – Publicado ao atualizar uma venda.
- **SaleCancelledEvent** – Publicado ao excluir uma venda.
- **ItemCancelledEvent** – Publicado ao substituir itens em uma venda atualizada.

## Publisher de Eventos

Implementado em:

```
Application/Events/EventPublisher.cs
```

Faz log dos eventos com Serilog usando um `SourceContext` customizado:

```csharp
private static readonly ILogger _logger = Log.ForContext("SourceContext", "EventPublisher");
_logger.Information("Event published: {@Event}", @event);
```

## Separação de Logs com Serilog

Configurado no `program.cs` para gravar todos os eventos em:

```
logs/event-log-{Date}.txt
```

## Onde os Eventos São Disparados

- `CreateSaleHandler`: publica `SaleCreatedEvent`
- `UpdateSaleHandler`: publica `SaleModifiedEvent` e `ItemCancelledEvent`
- `DeleteSaleHandler`: publica `SaleCancelledEvent`

## Testes

A cobertura de testes está em:

```
tests/Unit/Application/Sales/EventPublishingTests.cs
```

Garante que os eventos são corretamente publicados pelas operações.

## Como Estender

Para adicionar novos eventos:

1. Criar o modelo do evento na pasta `Events/`.
2. Publicar com `IEventPublisher.PublishAsync(...)`.
3. Incluir teste correspondente.

## Benefícios

- Baixo acoplamento
- Alta observabilidade (via Serilog)
- Legibilidade e manutenção facilitadas
- Suporte para integrações futuras (ex: outbox, mensageria)
