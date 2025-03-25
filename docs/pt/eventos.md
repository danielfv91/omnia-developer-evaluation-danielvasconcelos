# Eventos de Domínio

Este projeto inclui a implementação de eventos de domínio como demonstração de extensibilidade e separação de responsabilidades.

## Eventos implementados

- `SaleCreatedEvent`
- `SaleModifiedEvent`
- `SaleCancelledEvent`
- `ItemCancelledEvent` (estrutura criada, ainda não utilizada)

## Publisher

Foi criado um `ConsoleEventPublisher` para simular a publicação de eventos, escrevendo no console em formato JSON.

## Onde os eventos são disparados

| Evento              | Gatilho                            |
|---------------------|------------------------------------|
| SaleCreatedEvent    | Após POST /sales com sucesso       |
| SaleModifiedEvent   | Após PUT /sales/{id} com sucesso   |
| SaleCancelledEvent  | Após DELETE /sales/{id} com sucesso|

## Por que isso é importante

Demonstra como a aplicação pode evoluir para uma arquitetura reativa ou baseada em eventos no futuro.