# Estratégia de Testes

O projeto adota uma estratégia completa de testes automatizados, com foco em garantir a estabilidade, a corretude da lógica de negócio e a segurança para evolução contínua do código.

## Ferramentas utilizadas

- **xUnit** — framework principal de testes
- **FluentAssertions** — assertivas legíveis e robustas
- **NSubstitute** — biblioteca de mocks para substituições em testes
- **Bogus** — geração de dados realistas
- **AutoMapper** — mapeamento configurado para testes

## Organização dos testes

Os testes estão localizados no projeto `tests/Ambev.DeveloperEvaluation.Unit`, organizados por tipo:

```
tests/
├── Application/
│   └── Sales/
│       ├── Handlers/         → Testes dos Handlers do MediatR
│       ├── Validators/       → Testes de validadores (FluentValidation)
│       ├── Events/           → Testes dos Eventos
│   └── TestData/             → Builders e massa de dados centralizada
```

## Cobertura de testes

| Handler                | Cobertura                                           |
|------------------------|-----------------------------------------------------|
| CreateSaleHandler      | Criação de venda, regras de desconto                |
| UpdateSaleHandler      | Atualização, desconto e eventos cancelados          |
| DeleteSaleHandler      | Exclusão e eventos                                  |
| GetSaleHandler         | Retorno correto e not found                         |
| GetSalesHandler        | Paginação, filtros e ordenação                      |
| Eventos publicados     | SaleCreated, SaleModified, Cancelled, ItemCancelled |

## Estratégia com TestData

A geração de dados de teste está centralizada em três arquivos principais:

- `SaleFakerBuilder` → Gera comandos válidos/realistas com Bogus
- `SalesHandlersTestData` → Define cenários com quantidade, descontos e valores esperados
- `SalesValidatorTestData` → Gera comandos inválidos/limítrofes para validadores

Essa abordagem reduz duplicações e melhora a manutenção dos testes.
