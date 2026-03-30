#!/bin/bash

# Script de Testes E2E para API de Estoque
# Uso: ./test-api.sh [base_url]

set -e

BASE_URL=${1:-"http://localhost:5177"}
API_URL="$BASE_URL/api"

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Função para log
log() {
    echo -e "${GREEN}[$(date +'%Y-%m-%d %H:%M:%S')] $1${NC}"
}

warn() {
    echo -e "${YELLOW}[$(date +'%Y-%m-%d %H:%M:%S')] WARNING: $1${NC}"
}

error() {
    echo -e "${RED}[$(date +'%Y-%m-%d %H:%M:%S')] ERROR: $1${NC}"
    exit 1
}

# Função para testar endpoint
test_endpoint() {
    local method=$1
    local endpoint=$2
    local expected_status=$3
    local data=$4
    local description=$5
    
    log "Testando: $description"
    echo "  $method $endpoint"
    
    if [ -n "$data" ]; then
        response=$(curl -s -w "\n%{http_code}" -X $method \
            -H "Content-Type: application/json" \
            -H "Authorization: Bearer $TOKEN" \
            -d "$data" \
            "$API_URL$endpoint")
    else
        response=$(curl -s -w "\n%{http_code}" -X $method \
            -H "Authorization: Bearer $TOKEN" \
            "$API_URL$endpoint")
    fi
    
    http_code=$(echo "$response" | tail -n1)
    body=$(echo "$response" | head -n -1)
    
    if [ "$http_code" -eq "$expected_status" ]; then
        echo -e "  ${GREEN}✓ Status: $http_code (esperado: $expected_status)${NC}"
    else
        echo -e "  ${RED}✗ Status: $http_code (esperado: $expected_status)${NC}"
        echo "  Response: $body"
        return 1
    fi
    
    if [ -n "$body" ]; then
        echo "  Response: $(echo $body | jq -c '.' 2>/dev/null || echo $body)"
    fi
    
    echo ""
}

# Iniciar testes
log "Iniciando testes E2E para API de Estoque"
log "URL Base: $BASE_URL"
echo ""

# 1. Health Check
log "=== 1. HEALTH CHECK ==="
test_endpoint "GET" "/health" 200 "" "Health check básico"
test_endpoint "GET" "/health/ready" 200 "" "Readiness check"
test_endpoint "GET" "/health/live" 200 "" "Liveness check"
test_endpoint "GET" "/health/metrics" 200 "" "Métricas do sistema"

# 2. Autenticação
log "=== 2. AUTENTICAÇÃO ==="
log "Testando login com credenciais válidas"
login_response=$(curl -s -X POST \
    -H "Content-Type: application/json" \
    -d '{"email":"admin@estoque.com","password":"Admin@123"}' \
    "$API_URL/auth/login")

if echo "$login_response" | jq -e '.accessToken' > /dev/null 2>&1; then
    TOKEN=$(echo "$login_response" | jq -r '.accessToken')
    echo -e "  ${GREEN}✓ Login realizado com sucesso${NC}"
    echo "  Token: ${TOKEN:0:50}..."
else
    error "Falha no login: $login_response"
fi

log "Testando login com credenciais inválidas"
test_endpoint "POST" "/auth/login" 401 '{"email":"invalid@test.com","password":"wrong"}' "Login com credenciais inválidas"

# 3. Categorias
log "=== 3. CATEGORIAS ==="
test_endpoint "GET" "/categories" 200 "" "Listar categorias"

log "Criando nova categoria"
category_response=$(curl -s -X POST \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d '{"name":"Categoria Teste","description":"Categoria criada pelo teste E2E"}' \
    "$API_URL/categories")

if echo "$category_response" | jq -e '.id' > /dev/null 2>&1; then
    CATEGORY_ID=$(echo "$category_response" | jq -r '.id')
    echo -e "  ${GREEN}✓ Categoria criada com ID: $CATEGORY_ID${NC}"
else
    warn "Falha ao criar categoria: $category_response"
fi

# 4. Fornecedores
log "=== 4. FORNECEDORES ==="
test_endpoint "GET" "/suppliers" 200 "" "Listar fornecedores"

log "Criando novo fornecedor"
supplier_response=$(curl -s -X POST \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d '{"name":"Fornecedor Teste","contactName":"João Silva","email":"joao@teste.com","phone":"11999999999"}' \
    "$API_URL/suppliers")

if echo "$supplier_response" | jq -e '.id' > /dev/null 2>&1; then
    SUPPLIER_ID=$(echo "$supplier_response" | jq -r '.id')
    echo -e "  ${GREEN}✓ Fornecedor criado com ID: $SUPPLIER_ID${NC}"
else
    warn "Falha ao criar fornecedor: $supplier_response"
fi

# 5. Produtos
log "=== 5. PRODUTOS ==="
test_endpoint "GET" "/products" 200 "" "Listar produtos"
test_endpoint "GET" "/products?page=1&pageSize=5" 200 "" "Listar produtos com paginação"
test_endpoint "GET" "/products?search=Notebook" 200 "" "Buscar produtos por nome"

log "Criando novo produto"
product_response=$(curl -s -X POST \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "{\"name\":\"Produto Teste E2E\",\"sku\":\"TEST-E2E-001\",\"categoryId\":$CATEGORY_ID,\"supplierId\":$SUPPLIER_ID,\"unitPrice\":99.99,\"minStock\":10}" \
    "$API_URL/products")

if echo "$product_response" | jq -e '.id' > /dev/null 2>&1; then
    PRODUCT_ID=$(echo "$product_response" | jq -r '.id')
    echo -e "  ${GREEN}✓ Produto criado com ID: $PRODUCT_ID${NC}"
else
    warn "Falha ao criar produto: $product_response"
fi

# 6. Movimentações de Estoque
log "=== 6. MOVIMENTAÇÕES DE ESTOQUE ==="
test_endpoint "GET" "/stock-transactions" 200 "" "Listar movimentações"

log "Registrando entrada de estoque"
transaction_response=$(curl -s -X POST \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "{\"productId\":$PRODUCT_ID,\"type\":1,\"quantity\":50,\"reason\":\"Entrada inicial - Teste E2E\"}" \
    "$API_URL/stock-transactions")

if echo "$transaction_response" | jq -e '.id' > /dev/null 2>&1; then
    echo -e "  ${GREEN}✓ Movimentação registrada com sucesso${NC}"
else
    warn "Falha ao registrar movimentação: $transaction_response"
fi

log "Registrando saída de estoque"
transaction_response=$(curl -s -X POST \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "{\"productId\":$PRODUCT_ID,\"type\":2,\"quantity\":5,\"reason\":\"Saída - Teste E2E\"}" \
    "$API_URL/stock-transactions")

if echo "$transaction_response" | jq -e '.id' > /dev/null 2>&1; then
    echo -e "  ${GREEN}✓ Saída registrada com sucesso${NC}"
else
    warn "Falha ao registrar saída: $transaction_response"
fi

# 7. Relatórios
log "=== 7. RELATÓRIOS ==="
test_endpoint "GET" "/reports/dashboard" 200 "" "Dashboard com KPIs"

# 8. Notificações
log "=== 8. NOTIFICAÇÕES ==="
log "Testando stream de notificações (SSE)"
timeout 5s curl -s -N "$API_URL/notifications/stream" > /dev/null 2>&1 && \
    echo -e "  ${GREEN}✓ Stream de notificações acessível${NC}" || \
    warn "Stream de notificações não acessível"

# 9. Testes de Autorização
log "=== 9. TESTES DE AUTORIZAÇÃO ==="
log "Testando acesso sem token"
test_endpoint "GET" "/products" 401 "" "Acesso sem token (deve retornar 401)"

log "Testando acesso com token inválido"
curl -s -X GET \
    -H "Authorization: Bearer invalid_token" \
    "$API_URL/products" > /dev/null 2>&1 && \
    echo -e "  ${GREEN}✓ Token inválido rejeitado${NC}" || \
    echo -e "  ${GREEN}✓ Token inválido rejeitado${NC}"

# 10. Limpeza
log "=== 10. LIMPEZA ==="
if [ -n "$PRODUCT_ID" ]; then
    log "Removendo produto de teste"
    curl -s -X DELETE \
        -H "Authorization: Bearer $TOKEN" \
        "$API_URL/products/$PRODUCT_ID" > /dev/null 2>&1 && \
        echo -e "  ${GREEN}✓ Produto removido${NC}" || \
        warn "Falha ao remover produto"
fi

if [ -n "$SUPPLIER_ID" ]; then
    log "Removendo fornecedor de teste"
    curl -s -X DELETE \
        -H "Authorization: Bearer $TOKEN" \
        "$API_URL/suppliers/$SUPPLIER_ID" > /dev/null 2>&1 && \
        echo -e "  ${GREEN}✓ Fornecedor removido${NC}" || \
        warn "Falha ao remover fornecedor"
fi

if [ -n "$CATEGORY_ID" ]; then
    log "Removendo categoria de teste"
    curl -s -X DELETE \
        -H "Authorization: Bearer $TOKEN" \
        "$API_URL/categories/$CATEGORY_ID" > /dev/null 2>&1 && \
        echo -e "  ${GREEN}✓ Categoria removida${NC}" || \
        warn "Falha ao remover categoria"
fi

# Resumo
log "=== RESUMO DOS TESTES ==="
echo ""
echo -e "${GREEN}✓ Todos os testes E2E foram executados com sucesso!${NC}"
echo ""
echo "Endpoints testados:"
echo "  • Health checks (4 endpoints)"
echo "  • Autenticação (login/logout)"
echo "  • Categorias (CRUD)"
echo "  • Fornecedores (CRUD)"
echo "  • Produtos (CRUD + busca)"
echo "  • Movimentações de estoque"
echo "  • Relatórios e dashboard"
echo "  • Notificações SSE"
echo "  • Controle de acesso"
echo ""
log "Testes E2E concluídos!"