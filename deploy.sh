#!/bin/bash

# Script de Deploy do Sistema de Estoque
# Uso: ./deploy.sh [ambiente]
# Exemplo: ./deploy.sh production

set -e

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

# Verificar se Docker está instalado
check_docker() {
    if ! command -v docker &> /dev/null; then
        error "Docker não está instalado. Por favor, instale o Docker primeiro."
    fi
    
    if ! command -v docker-compose &> /dev/null; then
        error "Docker Compose não está instalado. Por favor, instale o Docker Compose primeiro."
    fi
    
    log "Docker e Docker Compose encontrados"
}

# Verificar se arquivo .env existe
check_env() {
    if [ ! -f .env ]; then
        warn "Arquivo .env não encontrado. Criando a partir do .env.example..."
        if [ -f .env.example ]; then
            cp .env.example .env
            log "Arquivo .env criado. Por favor, edite as configurações se necessário."
        else
            error "Arquivo .env.example não encontrado."
        fi
    fi
}

# Parar containers existentes
stop_containers() {
    log "Parando containers existentes..."
    docker-compose down --remove-orphans || true
}

# Build das imagens
build_images() {
    log "Construindo imagens Docker..."
    docker-compose build --no-cache
}

# Iniciar containers
start_containers() {
    log "Iniciando containers..."
    docker-compose up -d
}

# Verificar saúde dos containers
check_health() {
    log "Verificando saúde dos containers..."
    sleep 10
    
    # Verificar se todos os containers estão rodando
    if docker-compose ps | grep -q "Exit"; then
        error "Alguns containers não estão rodando. Verifique os logs com: docker-compose logs"
    fi
    
    log "Todos os containers estão rodando"
}

# Mostrar informações de acesso
show_access_info() {
    log "Deploy concluído com sucesso!"
    echo ""
    echo "=== INFORMAÇÕES DE ACESSO ==="
    echo "Frontend Angular: http://localhost:4200"
    echo "Admin Panel Vue.js: http://localhost:3000"
    echo "API .NET: http://localhost:5177"
    echo "RabbitMQ Management: http://localhost:15672 (guest/guest)"
    echo ""
    echo "=== CREDENCIAIS PADRÃO ==="
    echo "Admin: admin@estoque.com / Admin@123"
    echo "Funcionário: funcionario@estoque.com / Funcionario@123"
    echo ""
    echo "=== COMANDOS ÚTEIS ==="
    echo "Ver logs: docker-compose logs -f"
    echo "Parar containers: docker-compose down"
    echo "Reiniciar: docker-compose restart"
    echo "Status: docker-compose ps"
}

# Função principal
main() {
    local environment=${1:-development}
    
    log "Iniciando deploy do Sistema de Estoque (ambiente: $environment)"
    
    check_docker
    check_env
    stop_containers
    build_images
    start_containers
    check_health
    show_access_info
}

# Executar função principal
main "$@"