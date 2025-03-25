# API Examples

### Create Sale (POST /sales)

```json
{
  "saleNumber": 1001,
  "saleDate": "2025-03-25T14:00:00Z",
  "customerId": "aaaaaaa1-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
  "customerName": "John Doe",
  "branch": "North Branch",
  "items": [
    {
      "productId": "11111111-1111-1111-1111-111111111111",
      "productName": "Product A",
      "quantity": 5,
      "unitPrice": 20
    }
  ]
}
```

### Update Sale (PUT /sales/{id})

Same structure as POST, with additional `"id"` in the body.

### Delete Sale

No body required. Just DELETE to `/sales/{id}`.

### Response structure

```json
{
  "success": true,
  "message": "Sale created successfully",
  "data": {
    "id": "...",
    "saleNumber": 1001,
    ...
  }
}
```