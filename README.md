# Sistema de Gerenciamento de Estoque

Projeto full stack com backend em `.NET 10` e frontend em `Angular 19`, focado em controle de estoque corporativo com UI moderna e fluxo operacional.

## Stack
- Backend: ASP.NET Core Web API, EF Core, SQLite, JWT, RabbitMQ
- Frontend: Angular 19, SCSS, Bootstrap, Chart.js
- Admin Panel: Vue.js 3, Vite, Pinia, Tailwind CSS
- Mensageria: RabbitMQ (`stock-events`)

## Estrutura
- `src/Estoque.Api`: API RESTful e regras de negocio
- `src/estoque-web`: aplicacao Angular (frontend operacional)
- `src/admin-panel`: painel administrativo Vue.js (gestão de cadastros)
- `docs`: documentacao tecnica e guia de uso

## Como executar
1. Suba RabbitMQ:
   - `docker compose up -d`
2. API:
   - `dotnet restore src/Estoque.Api/Estoque.Api.csproj`
   - `dotnet run --project src/Estoque.Api/Estoque.Api.csproj`
3. Frontend Angular:
   - `cd src/estoque-web`
   - `npm install`
   - `npm start`
4. Admin Panel Vue.js:
   - `cd src/admin-panel`
   - `npm install`
   - `npm run dev`
5. Acesse:
   - Frontend Angular: `http://localhost:4200`
   - Admin Panel Vue.js: `http://localhost:5177`
   - API OpenAPI: `http://localhost:5177/openapi/v1.json`
   - RabbitMQ UI: `http://localhost:15672` (`guest/guest`)

## Testes automatizados
- Executar todos os testes:
  - `dotnet test EstoqueComercio.slnx`
- Cobertura atual:
  - Teste de integracao para login e endpoint paginado de produtos
  - Teste unitario da regra de estoque insuficiente

## Credenciais de exemplo
- Admin: `admin@estoque.com` / `Admin@123`
- Funcionario: `funcionario@estoque.com` / `Funcionario@123`

## Documentacao
- Banco de dados: `docs/database.md`
- API: `docs/api.md`
- Guia de usuario: `docs/user-guide.md`
