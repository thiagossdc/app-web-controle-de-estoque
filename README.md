# Sistema de Gerenciamento de Estoque

Projeto full stack com backend em `.NET 10` e frontend em `Angular 19`, focado em controle de estoque corporativo com UI moderna e fluxo operacional.

## Stack
- Backend: ASP.NET Core Web API, EF Core, SQLite, JWT, RabbitMQ
- Frontend: Angular 19, SCSS, Bootstrap, Chart.js
- Admin Panel: Vue.js 3, Vite, Pinia, Tailwind CSS
- Mensageria: RabbitMQ (`stock-events`)
- Cache: Memory Cache
- Métricas: OpenTelemetry
- Logs: Structured Logging
- Containerização: Docker, Docker Compose
- Proxy Reverso: Nginx

## Estrutura
- `src/Estoque.Api`: API RESTful e regras de negocio
- `src/estoque-web`: aplicacao Angular (frontend operacional)
- `src/admin-panel`: painel administrativo Vue.js (gestão de cadastros)
- `docs`: documentacao tecnica e guia de uso
- `tests`: testes E2E e unitários
- `nginx.conf`: configuração do proxy reverso
- `docker-compose.yml`: orquestração completa dos serviços

## Como executar

### Instalação Automática (Recomendado)
```bash
# Executar script de instalação
./install.sh
```

### Instalação Manual
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
   - Admin Panel Vue.js: `http://localhost:3000`
   - API OpenAPI: `http://localhost:5177/openapi/v1.json`
   - RabbitMQ UI: `http://localhost:15672` (`guest/guest`)

## Testes automatizados
- Executar todos os testes:
  - `dotnet test EstoqueComercio.slnx`
- Testes E2E:
  - `./tests/e2e/test-api.sh`
- Cobertura atual:
  - Teste de integracao para login e endpoint paginado de produtos
  - Teste unitario da regra de estoque insuficiente
  - Testes E2E para todos os endpoints principais

## Credenciais de exemplo
- Admin: `admin@estoque.com` / `Admin@123`
- Funcionario: `funcionario@estoque.com` / `Funcionario@123`

## Documentacao
- Banco de dados: `docs/database.md`
- API: `docs/api.md`
- Guia de usuario: `docs/user-guide.md`
- Deploy: `docs/deploy.md`

## Melhorias Implementadas

### Segurança
- ✅ JWT Key movida para variável de ambiente
- ✅ CORS configurado para múltiplas origens
- ✅ Tratamento global de erros
- ✅ Validações robustas de formulário
- ✅ Rate limiting no Nginx
- ✅ Headers de segurança no Nginx

### Performance
- ✅ Cache em memória para consultas frequentes
- ✅ Métricas de performance com OpenTelemetry
- ✅ Logs estruturados para observabilidade
- ✅ Compressão Gzip no Nginx
- ✅ Cache de arquivos estáticos

### Arquitetura
- ✅ Docker Compose completo com todos os serviços
- ✅ Proxy reverso com Nginx
- ✅ Health checks para todos os serviços
- ✅ Scripts de deploy automatizado
- ✅ Testes E2E abrangentes

### Frontend
- ✅ Rotas implementadas no Angular
- ✅ Dashboard administrativo completo
- ✅ Validações de formulário robustas
- ✅ Componentes reutilizáveis
- ✅ Gráficos interativos com Chart.js

### Backend
- ✅ Middleware de tratamento de erros
- ✅ Serviço de cache
- ✅ Serviço de métricas
- ✅ Serviço de logs estruturados
- ✅ Health checks detalhados

## Comandos Úteis
```bash
# Deploy completo
./deploy.sh

# Testes E2E
./tests/e2e/test-api.sh

# Ver logs
docker-compose logs -f

# Parar containers
docker-compose down

# Reiniciar serviços
docker-compose restart

# Status dos containers
docker-compose ps
```

## URLs de Acesso
- **Frontend Angular**: http://localhost:4200
- **Admin Panel Vue.js**: http://localhost:3001
- **API .NET**: http://localhost:5177
- **RabbitMQ Management**: http://localhost:15672 (guest/guest)
- **Health Check**: http://localhost:5177/api/health
- **Métricas**: http://localhost:5177/api/health/metrics
