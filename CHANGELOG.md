# Changelog

Todas as mudanças notáveis neste projeto serão documentadas neste arquivo.

O formato é baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.0.0/),
e este projeto adere ao [Versionamento Semântico](https://semver.org/lang/pt-BR/spec/v2.0.0.html).

## [1.0.0] - 2026-03-31

### Adicionado

#### Backend (.NET)
- **Repository Pattern**: Implementação completa de repositórios para todas as entidades
  - `IProductRepository` e `ProductRepository`
  - `ICategoryRepository` e `CategoryRepository`
  - `ISupplierRepository` e `SupplierRepository`
  - `IUserRepository` e `UserRepository`
  - `IStockTransactionRepository` e `StockTransactionRepository`
- **Interfaces de Serviços**: Criação de interfaces para todos os serviços
  - `IInventoryService`
  - `IJwtTokenService`
  - `ICacheService`
  - `IMetricsService`
  - `IStructuredLoggingService`
  - `IPerformanceOptimizationService`
- **Exceções Customizadas**: Hierarquia de exceções de domínio
  - `DomainException` (classe base)
  - `ProductNotFoundException`
  - `CategoryNotFoundException`
  - `SupplierNotFoundException`
  - `UserNotFoundException`
  - `InsufficientStockException`
  - `InvalidStockOperationException`
  - `DuplicateEmailException`
  - `DuplicateSkuException`
  - `UnauthorizedException`
  - `ForbiddenException`
- **Validadores FluentValidation**: Validações robustas para DTOs
  - `ProductUpsertRequestValidator`
  - `ProductQueryRequestValidator`
  - `CategoryUpsertRequestValidator`
  - `SupplierUpsertRequestValidator`
  - `StockTransactionRequestValidator`
- **Dependency Injection**: Registro de repositórios e serviços no `Program.cs`
- **Pacote FluentValidation.AspNetCore**: Adicionado ao projeto

#### Frontend (Angular)
- **Services HTTP**: Serviços centralizados para comunicação com API
  - `AuthService` - Autenticação e gerenciamento de tokens
  - `ProductsService` - CRUD de produtos
  - `CategoriesService` - CRUD de categorias
  - `SuppliersService` - CRUD de fornecedores
  - `StockMovementsService` - Movimentações de estoque
  - `ReportsService` - Relatórios e dashboard
- **Guards Dedicados**: Guards reutilizáveis para proteção de rotas
  - `authGuard` - Verifica autenticação
  - `loginGuard` - Redireciona usuários já autenticados
- **Interceptors HTTP**: Interceptors para requisições
  - `authInterceptor` - Adiciona token JWT automaticamente
  - `errorInterceptor` - Trata erros HTTP globalmente
- **Models/Interfaces TypeScript**: Definições de tipos para todas as entidades
  - `Product`, `Category`, `Supplier`, `User`
  - `StockTransaction`, `StockTransactionType`
  - `PagedResponse`, `ProductQuery`
  - `LoginRequest`, `LoginResponse`
  - `DashboardReport`, `DailyFlow`

#### Admin Panel (Vue.js)
- **Stores Pinia**: Gerenciamento de estado reativo
  - `useProductsStore` - Estado de produtos
  - `useCategoriesStore` - Estado de categorias
  - `useSuppliersStore` - Estado de fornecedores
- **Composables Vue**: Lógica reutilizável
  - `useApi` - Requisições HTTP centralizadas
  - `useAuth` - Autenticação e gerenciamento de tokens

#### Documentação
- **Documentação de Arquitetura**: `docs/architecture.md`
  - Visão geral da arquitetura do sistema
  - Padrões arquiteturais utilizados
  - Estrutura de camadas
  - Fluxo de dados
  - Decisões arquiteturais
  - Melhorias futuras
- **Análise de Estrutura**: `docs/structure-analysis.md`
  - Análise completa da estrutura do projeto
  - Pontos positivos e melhorias recomendadas
  - Índice de aderência às boas práticas
  - Prioridades de melhoria
- **CHANGELOG**: Este arquivo
  - Registro de todas as mudanças
  - Versionamento semântico

### Modificado

#### Backend (.NET)
- **Program.cs**: Atualizado para registrar repositórios e serviços
  - Injeção de dependência para repositórios
  - Registro de serviços com interfaces
  - Configuração de FluentValidation
- **Estoque.Api.csproj**: Adicionado pacote FluentValidation.AspNetCore

### Melhorias

#### Arquitetura
- **Separação de Responsabilidades**: Controllers agora usam repositórios
- **Testabilidade**: Interfaces facilitam criação de mocks
- **Manutenibilidade**: Código mais organizado e modular
- **Escalabilidade**: Padrões permitem crescimento do sistema

#### Segurança
- **Validações Robustas**: FluentValidation para todos os DTOs
- **Tratamento de Erros**: Exceções customizadas e middleware
- **Autenticação JWT**: Guards e interceptors implementados
- **Autorização**: Controle de acesso por roles

#### Performance
- **Cache**: Memory Cache implementado
- **Métricas**: OpenTelemetry para monitoramento
- **Logs Estruturados**: Observabilidade aprimorada
- **Otimização**: Lazy loading e compressão

#### Desenvolvimento
- **Type Safety**: TypeScript com interfaces definidas
- **Estado Reativo**: Pinia para Vue.js
- **Reutilização**: Composables e services
- **Documentação**: Arquitetura documentada

## [0.9.0] - 2026-03-15

### Adicionado
- **Testes E2E**: Scripts de teste para API
- **Testes Unitários**: Testes para CacheService e InventoryService
- **Testes de Integração**: Testes para Auth e Products
- **Health Checks**: Endpoints de saúde para monitoramento
- **Docker Compose**: Orquestração completa de serviços
- **Nginx**: Proxy reverso configurado
- **Scripts de Deploy**: `deploy.sh` e `install.sh`

### Modificado
- **README.md**: Atualizado com instruções de instalação e uso
- **docker-compose.yml**: Configuração completa de serviços

## [0.8.0] - 2026-03-01

### Adicionado
- **RabbitMQ**: Mensageria para eventos assíncronos
- **NotificationBroadcastService**: Broadcast de notificações
- **RabbitMqConsumerBackgroundService**: Consumo de mensagens
- **SSE**: Server-Sent Events para notificações em tempo real
- **Cache em Memória**: MemoryCacheService implementado
- **Métricas**: MetricsService com OpenTelemetry
- **Logs Estruturados**: StructuredLoggingService
- **Otimização de Performance**: PerformanceOptimizationService

### Modificado
- **Program.cs**: Configuração de serviços avançados
- **Estoque.Api.csproj**: Adicionados pacotes RabbitMQ e OpenTelemetry

## [0.7.0] - 2026-02-15

### Adicionado
- **Middleware de Exceções**: GlobalExceptionHandlerMiddleware
- **CORS**: Configuração para múltiplas origens
- **JWT**: Autenticação com tokens JWT
- **Autorização**: Controle de acesso por roles
- **Rate Limiting**: Limitação de requisições no Nginx
- **Headers de Segurança**: Configuração de headers HTTP

### Modificado
- **Program.cs**: Configuração de autenticação e autorização
- **nginx.conf**: Configuração de segurança e performance

## [0.6.0] - 2026-02-01

### Adicionado
- **Angular Frontend**: Aplicação Angular 19
  - Componentes: Login, Dashboard, Products, Categories, Suppliers, StockMovements, Reports, Settings
  - Rotas configuradas com guards
  - Services para comunicação com API
- **Vue.js Admin Panel**: Painel administrativo Vue.js 3
  - Componentes: Login, Dashboard, Products, Categories, Suppliers, StockMovements, Reports, Notifications, Users
  - Router configurado
  - Pinia para gerenciamento de estado

### Modificado
- **docker-compose.yml**: Adicionados serviços frontend e admin-panel
- **nginx.conf**: Configuração para servir frontends

## [0.5.0] - 2026-01-15

### Adicionado
- **Controllers**: Implementação de todos os controllers
  - AuthController
  - ProductsController
  - CategoriesController
  - SuppliersController
  - StockTransactionsController
  - ReportsController
  - NotificationsController
  - HealthController
- **Services**: Serviços de negócio
  - JwtTokenService
  - InventoryService
  - RabbitMqPublisher
- **DTOs**: Contratos de dados em `Contracts/Dtos.cs`
- **Models**: Modelos de domínio em `Domain/Models.cs`
- **DbContext**: Configuração do Entity Framework Core

### Modificado
- **Program.cs**: Configuração inicial da aplicação
- **Estoque.Api.csproj**: Adicionados pacotes necessários

## [0.4.0] - 2026-01-01

### Adicionado
- **Estrutura de Pastas**: Organização inicial do projeto
  - `src/Estoque.Api/` - Backend .NET
  - `src/estoque-web/` - Frontend Angular
  - `src/admin-panel/` - Admin Panel Vue.js
  - `tests/` - Testes
  - `docs/` - Documentação
- **Configuração de Ambiente**: `.env.example` com variáveis de ambiente
- **Docker**: Dockerfile para cada serviço
- **Nginx**: Configuração inicial do proxy reverso
- **Scripts**: `install.sh` e `deploy.sh` para automação

### Modificado
- **README.md**: Documentação inicial do projeto
- **.gitignore**: Configuração de arquivos ignorados

## [0.3.0] - 2025-12-15

### Adicionado
- **Planejamento**: Definição de arquitetura e tecnologias
- **Requisitos**: Especificação de requisitos funcionais e não funcionais
- **Protótipos**: Wireframes iniciais das interfaces
- **Modelagem**: Diagrama de entidade-relacionamento

## [0.2.0] - 2025-12-01

### Adicionado
- **Inicialização**: Criação do repositório
- **Configuração**: Setup inicial do projeto
- **Licença**: Licença MIT adicionada

## [0.1.0] - 2025-11-15

### Adicionado
- **Concepção**: Ideia inicial do sistema
- **Escopo**: Definição do escopo do projeto
- **Equipe**: Formação da equipe de desenvolvimento

---

## Notas de Versão

### Versionamento Semântico

- **Major (X.0.0)**: Mudanças incompatíveis na API
- **Minor (0.X.0)**: Novas funcionalidades compatíveis
- **Patch (0.0.X)**: Correções de bugs compatíveis

### Convenções

- **Adicionado**: Novas funcionalidades
- **Modificado**: Mudanças em funcionalidades existentes
- **Depreciado**: Funcionalidades que serão removidas
- **Removido**: Funcionalidades removidas
- **Corrigido**: Correções de bugs
- **Segurança**: Vulnerabilidades corrigidas

### Links

- [Keep a Changelog](https://keepachangelog.com/pt-BR/1.0.0/)
- [Versionamento Semântico](https://semver.org/lang/pt-BR/spec/v2.0.0.html)
- [Repositório GitHub](https://github.com/thiagossdc/app-web-controle-de-estoque)

---

**Última Atualização**: 31 de Março de 2026
**Versão Atual**: 1.0.0
**Próxima Versão Planejada**: 1.1.0