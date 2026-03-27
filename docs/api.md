# API REST - Endpoints

Base URL local: `http://localhost:5177/api`

## Autenticacao
- `POST /auth/login`
  - Body:
    ```json
    { "email": "admin@estoque.com", "password": "Admin@123" }
    ```
  - Response:
    ```json
    { "accessToken": "jwt...", "userName": "Administrador", "role": "Admin" }
    ```

## Categorias
- `GET /categories` (auth)
- `POST /categories` (auth + role `Admin`)

## Fornecedores
- `GET /suppliers` (auth)
- `POST /suppliers` (auth + role `Admin`)

## Produtos
- `GET /products` (auth)
  - Query params opcionais:
    - `search` (filtra por nome ou SKU)
    - `page` (padrao: 1)
    - `pageSize` (padrao: 20, max: 100)
  - Response:
    ```json
    {
      "items": [{ "id": 1, "name": "Notebook Corporativo 14", "sku": "NB-14-CP" }],
      "totalItems": 2,
      "page": 1,
      "pageSize": 20
    }
    ```
- `POST /products` (auth + role `Admin`)

## Transacoes de estoque
- `GET /stock-transactions` (auth)
- `POST /stock-transactions` (auth)
  - Body:
    ```json
    { "productId": 1, "type": 1, "quantity": 10, "reason": "Reposicao" }
    ```
  - `type`: `1=Entrada`, `2=Saida`, `3=Ajuste`

## Relatorios
- `GET /reports/dashboard` (auth)
  - Retorna KPIs e fluxo diario de entradas/saidas.

## Notificacoes em tempo real (SSE)
- `GET /notifications/stream`
  - Stream de eventos `stock` consumidos da fila RabbitMQ.

## Regras de negocio
- Saida nao permite estoque negativo.
- Ajuste redefine estoque atual para o valor informado em `quantity`.
- Cada movimentacao gera evento na fila `stock-events`.
