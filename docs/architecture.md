# Arquitetura do Sistema - Sistema de Gerenciamento de Estoque

## Visão Geral

O Sistema de Gerenciamento de Estoque é uma aplicação full-stack que utiliza uma arquitetura em camadas com separação clara de responsabilidades. O sistema é composto por três aplicações principais: API Backend (.NET), Frontend Operacional (Angular) e Admin Panel (Vue.js).

## Arquitetura Backend (.NET)

### Padrões Arquiteturais

#### 1. Repository Pattern
- **Objetivo**: Abstrair o acesso a dados e desacoplar a lógica de negócio da camada de persistência
- **Implementação**: Interfaces e implementações em `src/Estoque.Api/Repositories/`
- **Benefícios**:
  - Facilita testes unitários com mocks
  - Permite troca de implementação de banco de dados
  - Centraliza lógica de acesso a dados

```csharp
// Interface
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<PagedResponse<Product>> GetAllAsync(ProductQueryRequest query);
    Task<Product> CreateAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);
}

// Implementação
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;
    
    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}
```

#### 2. Dependency Injection
- **Objetivo**: Inverter dependências e facilitar testabilidade
- **Implementação**: Registro no `Program.cs`
- **Benefícios**:
  - Baixo acoplamento entre componentes
  - Facilita testes unitários
  - Flexibilidade para trocar implementações

```csharp
// Registro de dependências
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
```

#### 3. Middleware Pattern
- **Objetivo**: Tratar requisições de forma centralizada
- **Implementação**: `GlobalExceptionHandlerMiddleware`
- **Benefícios**:
  - Tratamento consistente de erros
  - Logs centralizados
  - Separação de preocupações

### Estrutura de Camadas

```
src/Estoque.Api/
├── Controllers/          # Camada de apresentação (Endpoints da API)
├── Services/            # Camada de negócio (Regras de negócio)
├── Repositories/        # Camada de dados (Acesso a dados)
│   ├── Interfaces/      # Contratos dos repositórios
│   └── Implementations/ # Implementações dos repositórios
├── Domain/              # Camada de domínio (Entidades)
├── Contracts/           # Camada de contratos (DTOs)
├── Data/                # Camada de persistência (DbContext)
├── Middleware/          # Middlewares customizados
├── Validators/          # Validadores (FluentValidation)
├── Exceptions/          # Exceções customizadas
└── Interfaces/          # Interfaces de serviços
```

### Fluxo de Dados

1. **Requisição** → Controller
2. **Controller** → Service (opcional) ou Repository
3. **Repository** → DbContext
4. **Resposta** → Controller → Cliente

### Validação

- **Biblioteca**: FluentValidation
- **Implementação**: Validadores em `src/Estoque.Api/Validators/`
- **Benefícios**:
  - Validações complexas e reutilizáveis
  - Mensagens de erro customizadas
  - Separação de lógica de validação

```csharp
public class ProductUpsertRequestValidator : AbstractValidator<ProductUpsertRequest>
{
    public ProductUpsertRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(160).WithMessage("Nome deve ter no máximo 160 caracteres.");
    }
}
```

### Tratamento de Exceções

- **Exceções de Domínio**: `src/Estoque.Api/Exceptions/DomainException.cs`
- **Middleware**: `GlobalExceptionHandlerMiddleware`
- **Benefícios**:
  - Tratamento consistente de erros
  - Mensagens amigáveis para o usuário
  - Logs detalhados para debugging

```csharp
public class ProductNotFoundException : DomainException
{
    public ProductNotFoundException(int id) 
        : base($"Produto com ID {id} não encontrado.") { }
}
```

## Arquitetura Frontend (Angular)

### Padrões Arquiteturais

#### 1. Service Pattern
- **Objetivo**: Centralizar lógica de acesso a API
- **Implementação**: Services em `src/estoque-web/src/app/services/`
- **Benefícios**:
  - Reutilização de código
  - Manutenção centralizada
  - Facilita testes

```typescript
@Injectable({
  providedIn: 'root'
})
export class ProductsService {
  private apiUrl = '/api/products';

  constructor(private http: HttpClient) {}

  getAll(query: ProductQuery = {}): Observable<PagedResponse<Product>> {
    let params = new HttpParams();
    if (query.search) params = params.set('search', query.search);
    return this.http.get<PagedResponse<Product>>(this.apiUrl, { params });
  }
}
```

#### 2. Guard Pattern
- **Objetivo**: Controlar acesso a rotas
- **Implementação**: Guards em `src/estoque-web/src/app/guards/`
- **Benefícios**:
  - Segurança centralizada
  - Reutilização de lógica de autenticação
  - Redirecionamento automático

```typescript
export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    return true;
  }

  router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
  return false;
};
```

#### 3. Interceptor Pattern
- **Objetivo**: Interceptar requisições HTTP
- **Implementação**: Interceptors em `src/estoque-web/src/app/interceptors/`
- **Benefícios**:
  - Adição automática de token
  - Tratamento global de erros
  - Logs de requisições

```typescript
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();

  if (token) {
    const clonedRequest = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
    return next(clonedRequest);
  }

  return next(req);
};
```

### Estrutura de Módulos

```
src/estoque-web/src/app/
├── components/          # Componentes de UI
├── services/           # Services de API
├── guards/             # Guards de rota
├── interceptors/       # Interceptors HTTP
├── models/             # Models/Interfaces TypeScript
└── app.routes.ts       # Configuração de rotas
```

## Arquitetura Admin Panel (Vue.js)

### Padrões Arquiteturais

#### 1. Store Pattern (Pinia)
- **Objetivo**: Gerenciar estado global da aplicação
- **Implementação**: Stores em `src/admin-panel/src/stores/`
- **Benefícios**:
  - Estado reativo e centralizado
  - DevTools para debugging
  - Persistência de estado

```javascript
export const useProductsStore = defineStore('products', () => {
  const products = ref([])
  const loading = ref(false)

  async function fetchProducts(search = '', page = 1) {
    loading.value = true
    const response = await fetch(`/api/products?search=${search}&page=${page}`)
    const data = await response.json()
    products.value = data.items
    loading.value = false
  }

  return { products, loading, fetchProducts }
})
```

#### 2. Composable Pattern
- **Objetivo**: Reutilizar lógica entre componentes
- **Implementação**: Composables em `src/admin-panel/src/composables/`
- **Benefícios**:
  - Código reutilizável
  - Separação de preocupações
  - Facilita testes

```javascript
export function useApi() {
  const loading = ref(false)
  const error = ref(null)

  const request = async (url, options = {}) => {
    loading.value = true
    const token = localStorage.getItem('token')
    
    const response = await fetch(url, {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        ...(token && { 'Authorization': `Bearer ${token}` })
      }
    })

    loading.value = false
    return response.json()
  }

  return { loading, error, request }
}
```

### Estrutura de Componentes

```
src/admin-panel/src/
├── components/          # Componentes Vue
├── stores/             # Stores Pinia
├── composables/        # Composables Vue
├── router/             # Configuração de rotas
└── assets/             # Assets estáticos
```

## Infraestrutura

### Containerização

- **Docker**: Containers para cada serviço
- **Docker Compose**: Orquestração de serviços
- **Nginx**: Proxy reverso e load balancing

### Configuração de Ambiente

- **Variáveis de ambiente**: `.env.example` como template
- **Configurações separadas**: Development, Production
- **Secrets**: JWT keys e credenciais em variáveis de ambiente

### Monitoramento

- **Health Checks**: Endpoints de saúde para cada serviço
- **Métricas**: OpenTelemetry para métricas de performance
- **Logs**: Structured Logging para observabilidade

## Segurança

### Autenticação

- **JWT**: Tokens para autenticação stateless
- **Refresh Tokens**: Renovação automática de tokens
- **Logout**: Invalidação de tokens

### Autorização

- **Role-based**: Controle de acesso por roles (Admin, Funcionário)
- **Guards**: Proteção de rotas no frontend
- **Middleware**: Validação de permissões no backend

### Proteção de Dados

- **CORS**: Configuração de origens permitidas
- **Rate Limiting**: Limitação de requisições
- **Headers de Segurança**: Headers HTTP de segurança

## Performance

### Cache

- **Memory Cache**: Cache em memória para consultas frequentes
- **Cache Keys**: Estratégia de invalidação de cache
- **TTL**: Time-to-live configurável

### Otimização

- **Lazy Loading**: Carregamento sob demanda de módulos
- **Compression**: Compressão Gzip no Nginx
- **CDN**: Cache de arquivos estáticos

## Testes

### Estratégia

- **Unit Tests**: Testes unitários para services e repositories
- **Integration Tests**: Testes de integração para controllers
- **E2E Tests**: Testes end-to-end para APIs

### Cobertura

- **Backend**: xUnit para .NET
- **Frontend**: Jasmine/Karma para Angular
- **Admin Panel**: Vitest para Vue.js

## Deploy

### Estratégia

- **Blue-Green**: Deploy com zero downtime
- **Rolling Update**: Atualização gradual
- **Rollback**: Reversão automática em caso de erro

### CI/CD

- **Build**: Build automático em commits
- **Test**: Execução de testes automatizados
- **Deploy**: Deploy automático em aprovação

## Decisões Arquiteturais

### 1. Separação de Frontends

**Decisão**: Manter Angular e Vue.js separados

**Justificativa**:
- Angular para operações complexas (CRUD completo)
- Vue.js para administração rápida (painel administrativo)
- Equipes podem trabalhar independentemente
- Deploy independente de cada frontend

### 2. Repository Pattern

**Decisão**: Implementar Repository Pattern no backend

**Justificativa**:
- Abstrai acesso a dados
- Facilita testes unitários
- Permite troca de banco de dados
- Centraliza lógica de queries

### 3. JWT para Autenticação

**Decisão**: Usar JWT para autenticação

**Justificativa**:
- Stateless (não mantém sessão)
- Escalável para múltiplas instâncias
- Suporte a múltiplos clientes
- Fácil integração com SPAs

### 4. RabbitMQ para Mensageria

**Decisão**: Usar RabbitMQ para eventos assíncronos

**Justificativa**:
- Desacopla componentes
- Garante entrega de mensagens
- Suporte a filas e exchanges
- Escalável horizontalmente

## Melhorias Futuras

### Curto Prazo

1. **Testes**: Aumentar cobertura de testes para 80%
2. **Documentação**: Completar documentação de API
3. **Logs**: Implementar structured logging completo

### Médio Prazo

1. **CQRS**: Implementar Command Query Responsibility Segregation
2. **Event Sourcing**: Implementar event sourcing para auditoria
3. **Microservices**: Evoluir para arquitetura de microservices

### Longo Prazo

1. **Kubernetes**: Migração para Kubernetes
2. **Service Mesh**: Implementar service mesh com Istio
3. **Observabilidade**: Implementar stack completa de observabilidade

---

**Última Atualização**: 31 de Março de 2026
**Versão**: 1.0.0
**Autor**: Equipe de Desenvolvimento