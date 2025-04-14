# Uso de DDD no Projeto

Este documento descreve como o projeto adota o modelo de Domínio Rico com base nos princípios do Domain-Driven Design (DDD).

## 1. Entidades como centro do domínio

As entidades (`Sale`, `SaleItem`) contêm lógica de negócio, como:
- Cálculo de desconto e total
- Validação de quantidade
- Adição e cancelamento de itens

Exemplo:

```csharp
if (quantity > 20)
    throw new BusinessException("Cannot sell more than 20 identical items per product.");
```

## 2. Eventos de Domínio

Eventos como `SaleCreatedDomainEvent`, `SaleModifiedDomainEvent`, `SaleCancelledDomainEvent` e `ItemCancelledDomainEvent` são disparados diretamente pelas entidades. Eles são armazenados internamente em uma coleção (`DomainEvents`) e publicados posteriormente.

## 3. Publicação de eventos desacoplada

A publicação é realizada por um `DomainEventsDispatcher` na camada de Application, utilizando a interface `IEventPublisher`, permitindo flexibilidade de implementação (log, fila, etc).

## 4. Handlers orientados ao domínio

Os `Handlers` apenas coordenam a operação, delegando a lógica para as entidades. Exemplo:

```csharp
sale.Update(...); // A lógica de atualização está na entidade
```

## 5. Modelo de persistência desacoplado

O repositório (`ISaleRepository`) trabalha com a entidade de domínio, sem alterar seu comportamento. A atualização mantém o estado da entidade como autoritativo.
