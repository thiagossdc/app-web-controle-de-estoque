# Estrutura do Banco de Dados (SQLite)

Arquivo: `src/Estoque.Api/estoque.db` (gerado automaticamente no primeiro run da API).

## Tabelas principais
- `Users`
  - `Id` (PK)
  - `Name`
  - `Email` (UNIQUE)
  - `PasswordHash`
  - `Role` (`Admin` ou `Funcionario`)
- `Categories`
  - `Id` (PK)
  - `Name`
  - `Description`
- `Suppliers`
  - `Id` (PK)
  - `Name`
  - `ContactName`
  - `Email`
  - `Phone`
- `Products`
  - `Id` (PK)
  - `Name`
  - `Sku` (UNIQUE)
  - `CategoryId` (FK)
  - `SupplierId` (FK)
  - `UnitPrice`
  - `CurrentStock`
  - `MinStock`
- `StockTransactions`
  - `Id` (PK)
  - `ProductId` (FK)
  - `UserId` (FK)
  - `Type` (`Entrada`, `Saida`, `Ajuste`)
  - `Quantity`
  - `Reason`
  - `CreatedAtUtc`

## Seed inicial
- 2 usuarios (admin e funcionario)
- 2 categorias
- 2 fornecedores
- 2 produtos
