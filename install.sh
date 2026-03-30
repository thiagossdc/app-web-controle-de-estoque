#!/bin/bash

# Script de Instalação do Sistema de Estoque
# Uso: ./install.sh

set -e

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
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

info() {
    echo -e "${BLUE}[$(date +'%Y-%m-%d %H:%M:%S')] INFO: $1${NC}"
}

# Banner
echo -e "${BLUE}"
echo "╔══════════════════════════════════════════════════════════════╗"
echo "║                                                              ║"
echo "║           Sistema de Gerenciamento de Estoque               ║"
echo "║                                                              ║"
echo "║                    Script de Instalação                      ║"
echo "║                                                              ║"
echo "╚══════════════════════════════════════════════════════════════╝"
echo -e "${NC}"

# Verificar se está no diretório correto
if [ ! -f "README.md" ]; then
    error "Execute este script no diretório raiz do projeto"
fi

# Verificar dependências
log "Verificando dependências..."

check_dependency() {
    if ! command -v $1 &> /dev/null; then
        error "$1 não está instalado. Por favor, instale $1 primeiro."
    else
        log "$1 encontrado: $(command -v $1)"
    fi
}

check_dependency "docker"
check_dependency "docker-compose"
check_dependency "node"
check_dependency "npm"
check_dependency "dotnet"

# Verificar versões
log "Verificando versões..."
node_version=$(node --version)
npm_version=$(npm --version)
dotnet_version=$(dotnet --version)

info "Node.js: $node_version"
info "npm: $npm_version"
info ".NET: $dotnet_version"

# Criar arquivo .env se não existir
if [ ! -f .env ]; then
    log "Criando arquivo .env..."
    cp .env.example .env
    
    # Gerar JWT Key aleatória
    jwt_key=$(openssl rand -base64 32)
    sed -i.bak "s/SUA_CHAVE_SECRETA_AQUI_MUITO_LONGA_E_SEGURA/$jwt_key/" .env
    rm -f .env.bak
    
    log "Arquivo .env criado com JWT Key gerada automaticamente"
else
    warn "Arquivo .env já existe. Mantendo configurações existentes."
fi

# Instalar dependências do frontend Angular
log "Instalando dependências do frontend Angular..."
cd src/estoque-web
npm install
cd ../..

# Instalar dependências do admin panel Vue.js
log "Instalando dependências do admin panel Vue.js..."
cd src/admin-panel
npm install
cd ../..

# Restaurar pacotes .NET
log "Restaurando pacotes .NET..."
dotnet restore src/Estoque.Api/Estoque.Api.csproj

# Build da aplicação .NET
log "Compilando aplicação .NET..."
dotnet build src/Estoque.Api/Estoque.Api.csproj --configuration Release --no-restore

# Build do frontend Angular
log "Compilando frontend Angular..."
cd src/estoque-web
npm run build
cd ../..

# Build do admin panel Vue.js
log "Compilando admin panel Vue.js..."
cd src/admin-panel
npm run build
cd ../..

# Criar diretórios necessários
log "Criando diretórios necessários..."
mkdir -p logs
mkdir -p ssl
mkdir -p data

# Configurar permissões
log "Configurando permissões..."
chmod +x deploy.sh
chmod +x tests/e2e/test-api.sh

# Verificar se Docker está rodando
log "Verificando se Docker está rodando..."
if ! docker info &> /dev/null; then
    error "Docker não está rodando. Por favor, inicie o Docker primeiro."
fi

# Iniciar containers
log "Iniciando containers Docker..."
docker-compose up -d

# Aguardar containers iniciarem
log "Aguardando containers iniciarem..."
sleep 30

# Verificar saúde dos containers
log "Verificando saúde dos containers..."
if docker-compose ps | grep -q "Exit"; then
    error "Alguns containers não estão rodando. Verifique os logs com: docker-compose logs"
fi

# Executar testes E2E
log "Executando testes E2E..."
if [ -f "tests/e2e/test-api.sh" ]; then
    ./tests/e2e/test-api.sh http://localhost:5177
else
    warn "Script de testes E2E não encontrado"
fi

# Mostrar informações finais
log "Instalação concluída com sucesso!"
echo ""
echo -e "${GREEN}=== INFORMAÇÕES DE ACESSO ===${NC}"
echo ""
echo -e "${BLUE}Frontend Angular:${NC} http://localhost:4200"
echo -e "${BLUE}Admin Panel Vue.js:${NC} http://localhost:3000"
echo -e "${BLUE}API .NET:${NC} http://localhost:5177"
echo -e "${BLUE}RabbitMQ Management:${NC} http://localhost:15672 (guest/guest)"
echo ""
echo -e "${GREEN}=== CREDENCIAIS PADRÃO ===${NC}"
echo ""
echo -e "${BLUE}Admin:${NC} admin@estoque.com / Admin@123"
echo -e "${BLUE}Funcionário:${NC} funcionario@estoque.com / Funcionario@123"
echo ""
echo -e "${GREEN}=== COMANDOS ÚTEIS ===${NC}"
echo ""
echo -e "${BLUE}Ver logs:${NC} docker-compose logs -f"
echo -e "${BLUE}Parar containers:${NC} docker-compose down"
echo -e "${BLUE}Reiniciar:${NC} docker-compose restart"
echo -e "${BLUE}Status:${NC} docker-compose ps"
echo -e "${BLUE}Deploy:${NC} ./deploy.sh"
echo -e "${BLUE}Testes:${NC} ./tests/e2e/test-api.sh"
echo ""
echo -e "${GREEN}=== PRÓXIMOS PASSOS ===${NC}"
echo ""
echo "1. Acesse http://localhost:4200 para o frontend principal"
echo "2. Acesse http://localhost:3000 para o painel administrativo"
echo "3. Faça login com as credenciais de admin"
echo "4. Configure categorias e fornecedores"
echo "5. Cadastre produtos"
echo "6. Comece a registrar movimentações de estoque"
echo ""
echo -e "${YELLOW}Documentação completa disponível em: docs/${NC}"
echo ""
log "Sistema pronto para uso!"