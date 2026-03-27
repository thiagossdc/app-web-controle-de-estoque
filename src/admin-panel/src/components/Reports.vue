<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h2 class="text-2xl font-bold text-gray-900">Relatórios</h2>
      <div class="flex space-x-2">
        <button
          @click="generateReport('inventory')"
          class="bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700"
        >
          Exportar Inventário
        </button>
        <button
          @click="generateReport('movements')"
          class="bg-green-600 text-white px-4 py-2 rounded-md hover:bg-green-700"
        >
          Exportar Movimentações
        </button>
      </div>
    </div>

    <!-- Inventory Summary -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
      <div class="bg-white p-6 rounded-lg shadow">
        <h3 class="text-lg font-semibold text-gray-900 mb-2">Total de Produtos</h3>
        <p class="text-3xl font-bold text-blue-600">{{ summary.totalProducts }}</p>
      </div>
      <div class="bg-white p-6 rounded-lg shadow">
        <h3 class="text-lg font-semibold text-gray-900 mb-2">Produtos em Estoque Baixo</h3>
        <p class="text-3xl font-bold text-red-600">{{ summary.lowStockProducts }}</p>
      </div>
      <div class="bg-white p-6 rounded-lg shadow">
        <h3 class="text-lg font-semibold text-gray-900 mb-2">Valor Total em Estoque</h3>
        <p class="text-3xl font-bold text-green-600">R$ {{ summary.totalValue.toLocaleString('pt-BR', { minimumFractionDigits: 2 }) }}</p>
      </div>
    </div>

    <!-- Low Stock Alert -->
    <div v-if="lowStockProducts.length > 0" class="bg-red-50 border border-red-200 rounded-lg p-4 mb-6">
      <h3 class="text-lg font-semibold text-red-900 mb-2">Produtos com Estoque Baixo</h3>
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        <div v-for="product in lowStockProducts" :key="product.id" class="bg-white p-4 rounded-lg shadow">
          <h4 class="font-medium text-gray-900">{{ product.name }}</h4>
          <p class="text-sm text-gray-600">Estoque: {{ product.currentStock }} (mín: {{ product.minStock }})</p>
        </div>
      </div>
    </div>

    <!-- Recent Movements -->
    <div class="bg-white shadow rounded-lg overflow-hidden">
      <div class="px-6 py-4 border-b border-gray-200">
        <h3 class="text-lg font-semibold text-gray-900">Movimentações Recentes</h3>
      </div>
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Data
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Produto
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Tipo
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Quantidade
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Saldo
            </th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="movement in recentMovements" :key="movement.id">
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ formatDate(movement.createdAtUtc) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
              {{ movement.product.name }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm">
              <span
                :class="movement.type === 'Entrada' ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'"
                class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full"
              >
                {{ movement.type }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ movement.quantity }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ movement.product.currentStock }}
            </td>
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
        
        // Calculate summary
        this.summary.totalProducts = products.length
        this.summary.lowStockProducts = products.filter(p => p.currentStock <= p.minStock).length
        this.summary.totalValue = products.reduce((total, product) => {
          return total + (product.currentStock * product.unitPrice)
        }, 0)
        
        // Get low stock products
        this.lowStockProducts = products
          .filter(p => p.currentStock <= p.minStock)
          .slice(0, 6) // Show only first 6
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