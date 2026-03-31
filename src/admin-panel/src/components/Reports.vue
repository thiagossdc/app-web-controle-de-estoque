<template>
  <div class="reports-container">
    <div class="page-header">
      <h2 class="page-title">Relatórios</h2>
      <div class="header-actions">
        <button @click="generateReport('inventory')" class="btn-primary">
          Exportar Inventário
        </button>
        <button @click="generateReport('movements')" class="btn-success">
          Exportar Movimentações
        </button>
      </div>
    </div>

    <!-- Summary Cards -->
    <div class="summary-cards">
      <div class="summary-card">
        <div class="card-icon blue">
          <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z"></path>
          </svg>
        </div>
        <div class="card-content">
          <h3>Total de Produtos</h3>
          <p class="card-value blue">{{ summary.totalProducts }}</p>
        </div>
      </div>
      
      <div class="summary-card">
        <div class="card-icon red">
          <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"></path>
            <line x1="12" y1="9" x2="12" y2="13"></line>
            <line x1="12" y1="17" x2="12.01" y2="17"></line>
          </svg>
        </div>
        <div class="card-content">
          <h3>Produtos com Estoque Baixo</h3>
          <p class="card-value red">{{ summary.lowStockProducts }}</p>
        </div>
      </div>
      
      <div class="summary-card">
        <div class="card-icon green">
          <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <line x1="12" y1="1" x2="12" y2="23"></line>
            <path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"></path>
          </svg>
        </div>
        <div class="card-content">
          <h3>Valor Total em Estoque</h3>
          <p class="card-value green">R$ {{ summary.totalValue.toLocaleString('pt-BR', { minimumFractionDigits: 2 }) }}</p>
        </div>
      </div>
    </div>

    <!-- Low Stock Alert -->
    <div v-if="lowStockProducts.length > 0" class="alert-section">
      <div class="alert-header">
        <h3>Produtos com Estoque Baixo</h3>
      </div>
      <div class="alert-grid">
        <div v-for="product in lowStockProducts" :key="product.id" class="alert-card">
          <h4>{{ product.name }}</h4>
          <p>Estoque: {{ product.currentStock }} (mín: {{ product.minStock }})</p>
        </div>
      </div>
    </div>

    <!-- Recent Movements -->
    <div class="data-table">
      <div class="table-header">
        <h3 class="table-title">Movimentações Recentes</h3>
      </div>
      <table>
        <thead>
          <tr>
            <th>Data</th>
            <th>Produto</th>
            <th>Tipo</th>
            <th>Quantidade</th>
            <th>Saldo</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="movement in recentMovements" :key="movement.id">
            <td class="movement-date">{{ formatDate(movement.createdAtUtc) }}</td>
            <td class="movement-product">{{ movement.product?.name }}</td>
            <td class="movement-type">
              <span :class="movement.type === 'Entrada' ? 'badge-success' : 'badge-danger'">
                {{ movement.type }}
              </span>
            </td>
            <td class="movement-quantity">{{ movement.quantity }}</td>
            <td class="movement-balance">{{ movement.product?.currentStock }}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script>
import axios from 'axios'

export default {
  name: 'Reports',
  data() {
    return {
      summary: {
        totalProducts: 0,
        lowStockProducts: 0,
        totalValue: 0
      },
      lowStockProducts: [],
      recentMovements: []
    }
  },
  methods: {
    async loadReports() {
      try {
        const [productsResponse, movementsResponse] = await Promise.all([
          axios.get('http://localhost:5177/api/products', {
            headers: {
              Authorization: `Bearer ${this.authStore.token}`
            }
          }),
          axios.get('http://localhost:5177/api/stock-transactions', {
            headers: {
              Authorization: `Bearer ${this.authStore.token}`
            },
            params: {
              limit: 10
            }
          })
        ])
        
        const products = productsResponse.data
        this.recentMovements = movementsResponse.data
        
        // Calcular resumo
        this.summary.totalProducts = products.length
        this.summary.lowStockProducts = products.filter(p => p.currentStock <= p.minStock).length
        this.summary.totalValue = products.reduce((total, product) => {
          return total + (product.currentStock * product.unitPrice)
        }, 0)
        
        // Obter produtos com estoque baixo
        this.lowStockProducts = products
          .filter(p => p.currentStock <= p.minStock)
          .slice(0, 6) // Mostrar apenas os primeiros 6
      } catch (error) {
        console.error('Erro ao carregar relatórios:', error)
      }
    },
    
    async generateReport(type) {
      try {
        const response = await axios.get(`http://localhost:5177/api/reports/${type}`, {
          headers: {
            Authorization: `Bearer ${this.authStore.token}`
          },
          responseType: 'blob'
        })
        
        const blob = new Blob([response.data], { type: 'application/pdf' })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `${type}-report-${new Date().toISOString().slice(0, 10)}.pdf`
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
        window.URL.revokeObjectURL(url)
      } catch (error) {
        console.error('Erro ao gerar relatório:', error)
      }
    },
    
    formatDate(dateString) {
      const date = new Date(dateString)
      return date.toLocaleString('pt-BR')
    }
  },
  mounted() {
    this.loadReports()
  }
}
</script>

<style scoped>
.reports-container {
  padding: 24px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.page-title {
  font-size: 24px;
  font-weight: 700;
  color: #1e293b;
  margin: 0;
}

.header-actions {
  display: flex;
  gap: 12px;
}

.btn-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-primary:hover {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
}

.btn-success {
  background: linear-gradient(135deg, #198754 0%, #146c43 100%);
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-success:hover {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(25, 135, 84, 0.4);
}

.summary-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 24px;
  margin-bottom: 24px;
}

.summary-card {
  background: white;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  display: flex;
  align-items: center;
  gap: 16px;
}

.card-icon {
  width: 48px;
  height: 48px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
}

.card-icon.blue {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.card-icon.red {
  background: linear-gradient(135deg, #dc3545 0%, #c82333 100%);
}

.card-icon.green {
  background: linear-gradient(135deg, #198754 0%, #146c43 100%);
}

.card-content h3 {
  font-size: 14px;
  color: #64748b;
  margin: 0 0 8px 0;
  font-weight: 500;
}

.card-value {
  font-size: 24px;
  font-weight: 700;
  margin: 0;
}

.card-value.blue {
  color: #667eea;
}

.card-value.red {
  color: #dc3545;
}

.card-value.green {
  color: #198754;
}

.alert-section {
  background: #fef2f2;
  border: 1px solid #fecaca;
  border-radius: 12px;
  padding: 20px;
  margin-bottom: 24px;
}

.alert-header {
  margin-bottom: 16px;
}

.alert-header h3 {
  font-size: 18px;
  font-weight: 600;
  color: #dc2626;
  margin: 0;
}

.alert-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 12px;
}

.alert-card {
  background: white;
  border-radius: 8px;
  padding: 12px;
  border-left: 4px solid #dc2626;
}

.alert-card h4 {
  font-size: 14px;
  font-weight: 600;
  color: #1e293b;
  margin: 0 0 4px 0;
}

.alert-card p {
  font-size: 12px;
  color: #64748b;
  margin: 0;
}

.data-table {
  background: white;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.table-header {
  padding: 20px 24px;
  border-bottom: 1px solid #e2e8f0;
}

.table-title {
  font-size: 18px;
  font-weight: 600;
  color: #1e293b;
  margin: 0;
}

table {
  width: 100%;
  border-collapse: collapse;
}

th {
  padding: 12px 24px;
  text-align: left;
  font-size: 12px;
  font-weight: 600;
  color: #64748b;
  text-transform: uppercase;
  background: #f8fafc;
  border-bottom: 1px solid #e2e8f0;
}

td {
  padding: 16px 24px;
  border-bottom: 1px solid #e2e8f0;
}

.movement-date {
  color: #64748b;
  font-size: 12px;
}

.movement-product {
  font-weight: 600;
  color: #1e293b;
}

.movement-type {
  text-align: center;
}

.badge-success {
  background: #dcfce7;
  color: #166534;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
}

.badge-danger {
  background: #fef2f2;
  color: #dc2626;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
}

.movement-quantity {
  font-weight: 600;
  color: #1e293b;
}

.movement-balance {
  font-weight: 600;
  color: #1e293b;
}
</style>
