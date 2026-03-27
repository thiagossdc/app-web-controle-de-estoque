<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h2 class="text-2xl font-bold text-gray-900">Movimentação de Estoque</h2>
      <button
        @click="showCreateModal = true"
        class="bg-indigo-600 text-white px-4 py-2 rounded-md hover:bg-indigo-700"
      >
        Nova Movimentação
      </button>
    </div>

    <!-- Filters -->
    <div class="bg-white p-4 rounded-lg shadow mb-6">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Tipo</label>
          <select
            v-model="filters.type"
            class="w-full border-gray-300 rounded-md shadow-sm"
          >
            <option value="">Todos</option>
            <option value="Entrada">Entrada</option>
            <option value="Saída">Saída</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Produto</label>
          <select
            v-model="filters.productId"
            class="w-full border-gray-300 rounded-md shadow-sm"
          >
            <option value="">Todos</option>
            <option v-for="product in products" :key="product.id" :value="product.id">
              {{ product.name }}
            </option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Data Início</label>
          <input
            type="date"
            v-model="filters.startDate"
            class="w-full border-gray-300 rounded-md shadow-sm"
          />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Data Fim</label>
          <input
            type="date"
            v-model="filters.endDate"
            class="w-full border-gray-300 rounded-md shadow-sm"
          />
        </div>
      </div>
      <div class="mt-4 flex space-x-2">
        <button
          @click="loadMovements"
          class="bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700"
        >
          Filtrar
        </button>
        <button
          @click="resetFilters"
          class="bg-gray-300 text-gray-800 px-4 py-2 rounded-md hover:bg-gray-400"
        >
          Limpar
        </button>
      </div>
    </div>

    <!-- Movements Table -->
    <div class="bg-white shadow rounded-lg overflow-hidden">
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
              Motivo
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Usuário
            </th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="movement in movements" :key="movement.id">
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
              {{ movement.reason }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ movement.user.name }}
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Create Modal -->
    <div v-if="showCreateModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full">
      <div class="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white">
        <div class="mt-3">
          <h3 class="text-lg font-medium text-gray-900 mb-4">Nova Movimentação</h3>
          <form @submit.prevent="createMovement">
            <div class="space-y-4">
              <div>
                <label class="block text-sm font-medium text-gray-700">Tipo</label>
                <select
                  v-model="movementForm.type"
                  required
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm"
                >
                  <option value="Entrada">Entrada</option>
                  <option value="Saída">Saída</option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700">Produto</label>
                <select
                  v-model="movementForm.productId"
                  required
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm"
                >
                  <option v-for="product in products" :key="product.id" :value="product.id">
                    {{ product.name }} (Estoque: {{ product.currentStock }})
                  </option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700">Quantidade</label>
                <input
                  type="number"
                  v-model="movementForm.quantity"
                  required
                  min="1"
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700">Motivo</label>
                <input
                  type="text"
                  v-model="movementForm.reason"
                  required
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm"
                />
              </div>
            </div>
            <div class="mt-5 flex justify-end space-x-2">
              <button
                type="button"
                @click="showCreateModal = false"
                class="px-4 py-2 bg-gray-300 text-gray-800 rounded-md hover:bg-gray-400"
              >
                Cancelar
              </button>
              <button
                type="submit"
                class="px-4 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700"
              >
                Salvar
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios'

export default {
  name: 'StockMovements',
  data() {
    return {
      movements: [],
      products: [],
      showCreateModal: false,
      movementForm: {
        type: 'Entrada',
        productId: '',
        quantity: '',
        reason: ''
      },
      filters: {
        type: '',
        productId: '',
        startDate: '',
        endDate: ''
      }
    }
  },
  methods: {
    async loadMovements() {
      try {
        const params = { ...this.filters }
        if (this.filters.startDate) {
          params.startDate = new Date(this.filters.startDate).toISOString()
        }
        if (this.filters.endDate) {
          params.endDate = new Date(this.filters.endDate).toISOString()
        }
        
        const response = await axios.get('http://localhost:5177/api/stock-transactions', {
          headers: {
            Authorization: `Bearer ${this.authStore.token}`
          },
          params
        })
        this.movements = response.data
      } catch (error) {
        console.error('Erro ao carregar movimentações:', error)
      }
    },
    
    async loadProducts() {
      try {
        const response = await axios.get('http://localhost:5177/api/products', {
          headers: {
            Authorization: `Bearer ${this.authStore.token}`
          }
        })
        this.products = response.data
      } catch (error) {
        console.error('Erro ao carregar produtos:', error)
      }
    },
    
    async createMovement() {
      try {
        await axios.post('http://localhost:5177/api/stock-transactions', this.movementForm, {
          headers: {
            Authorization: `Bearer ${this.authStore.token}`
          }
        })
        this.showCreateModal = false
        this.movementForm = {
          type: 'Entrada',
          productId: '',
          quantity: '',
          reason: ''
        }
        await this.loadMovements()
        await this.loadProducts() // Atualiza o estoque dos produtos
      } catch (error) {
        console.error('Erro ao criar movimentação:', error)
      }
    },
    
    resetFilters() {
      this.filters = {
        type: '',
        productId: '',
        startDate: '',
        endDate: ''
      }
      this.loadMovements()
    },
    
    formatDate(dateString) {
      const date = new Date(dateString)
      return date.toLocaleString('pt-BR')
    }
  },
  mounted() {
    this.loadMovements()
    this.loadProducts()
  }
}
</script>