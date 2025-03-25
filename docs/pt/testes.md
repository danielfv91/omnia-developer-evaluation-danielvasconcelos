# Estratégia de Testes

O projeto inclui testes unitários para toda a lógica de negócio, garantindo comportamento correto e segurança em alterações futuras.

## Ferramentas utilizadas

- xUnit
- NSubstitute
- Bogus (geração de massa de dados)
- AutoMapper com configuração simplificada para testes

## Cobertura de testes

| Handler                | Cobertura                              |
|------------------------|----------------------------------------|
| CreateSaleHandler      | Lógica de negócio + regras de desconto |
| UpdateSaleHandler      | Lógica de negócio + regras de desconto |
| DeleteSaleHandler      | Comportamento de exclusão              |
| Eventos publicados     | SaleCreated, SaleModified, Cancelled   |

Os testes estão no projeto `tests/Ambev.DeveloperEvaluation.Unit`.