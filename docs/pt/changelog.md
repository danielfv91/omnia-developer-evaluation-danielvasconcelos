# Changelog

## v1.3.0 — Testes automatizados completos + Eventos + Logs + Boas práticas

### Visão Geral
Esta versão representa a entrega final e consolidada do projeto, com foco em qualidade de código, testes automatizados, logs estruturados e documentação abrangente.

### Funcionalidades Implementadas

- CRUD completo de vendas
- Aplicação das regras de negócio:
  - 4–9 itens: 10% de desconto
  - 10–20 itens: 20% de desconto
  - Mais de 20 itens: não permitido
- Respostas padronizadas da API (ApiResponse)
- Validações com FluentValidation e mensagens em inglês

### Diferenciais Entregues

- Eventos de domínio:
  - SaleCreatedEvent
  - SaleModifiedEvent
  - SaleCancelledEvent
  - ItemCancelledEvent
- Publisher desacoplado via `IEventPublisher`
- Log estruturado para eventos (event-log-*.txt)

### Testes Automatizados

- Testes unitários com 100% de cobertura das regras de negócio
- Testes de validação com FluentValidation
- Testes funcionais com chamadas reais HTTP + Testcontainers
- Testes de integração com MediatR, FluentValidation e banco real PostgreSQL

### Boas Práticas e Arquitetura

- Aplicação de SOLID, DRY e YAGNI
- Remoção de duplicidade de lógica nos Handlers
- Criação do `ISaleItemBuilder` com única responsabilidade
- Separação clara entre camada WebApi e Application
- Middleware global para tratamento de exceções

### Documentação

- README atualizado em português e inglês
- Documentação criada:
  - Arquitetura do Projeto
  - Estratégia de Testes
  - Logs e Eventos
  - Clean Code e Boas Práticas

---

## Histórico de Versões

### v1.2.1 — Refatoração de Testes e Organização por Features

- Refatoração completa dos testes da feature Sales
- Criação do SaleFakerBuilder com Bogus
- Organização dos testes por feature
- Manutenção da compatibilidade da API pública

### v1.2.0 — Validação automática com FluentValidation + Documentação de arquitetura

- Pipeline de validação com FluentValidation via MediatR
- Redução de código nos controllers
- Documentação técnica da arquitetura (pt e en)

### v1.1.1 — Correção de autenticação e atualização da documentação

- Corrigido mapeamento de autenticação JWT
- README atualizado com instruções de segurança

### v1.1.0 — Entrega final com eventos de domínio e documentação expandida

- Implementação de eventos e testes automatizados
- Documentação técnica bilíngue completa

### v1.0.0 — Entrega da parte obrigatória do projeto de avaliação (CRUD de Vendas)

- CRUD completo de vendas
- Regras de negócio com descontos
- Testes unitários com cobertura total
- Estrutura de projeto DDD