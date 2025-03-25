# Exemplos da API

### Criar venda (POST /sales)

```json
{
  "saleNumber": 1001,
  "saleDate": "2025-03-25T14:00:00Z",
  "customerId": "aaaaaaa1-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
  "customerName": "João da Silva",
  "branch": "Filial Norte",
  "items": [
    {
      "productId": "11111111-1111-1111-1111-111111111111",
      "productName": "Produto A",
      "quantity": 5,
      "unitPrice": 20
    }
  ]
}
```

### Atualizar venda (PUT /sales/{id})

Mesmo corpo do POST, mas com o campo `"id"` incluso.

### Excluir venda (DELETE /sales/{id})

Não requer corpo. Apenas envie o DELETE para o endpoint com o ID.

### Exemplo de resposta

```json
{
  "success": true,
  "message": "Venda criada com sucesso",
  "data": {
    "id": "...",
    "saleNumber": 1001,
    ...
  }
}
```