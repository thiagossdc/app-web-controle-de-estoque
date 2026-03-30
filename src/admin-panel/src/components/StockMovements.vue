<template>
  <div class="stock-movements-container">
    <div class="page-header">
      <h2 class="page-title">Movimentação de Estoque</h2>
      <button @click="showCreateModal = true" class="btn-primary">
        Nova Movimentação
      </button>
    </div>

    <!-- Filters -->
    <div class="filters-section">
      <div class="filters-grid">
        <div class="filter-group">
          <label class="filter-label">Tipo</label>
          <select v-model="filters.type" class="filter-select">
            <option value="">Todos</option>
            <option value="Entrada">Entrada</option>
            <option value="Saída">Saída</option>
          </select>
        </div>
        <div class="filter-group">
          <label class="filter-label">Produto</label>
          <select v-model="filters.productId" class="filter-select">
            <option value="">Todos</option>
            <option v-for="product in products" :key="product.id" :value="product.id">
              {{ product.name }}
            </option>
          </select>
        </div>
        <div class="filter-group">
          <label class="filter-label">Data Início</label>
          <input type="date" v-model="filters.startDate" class="filter-input" />
        </div>
        <div class="filter-group">
          <label class="filter-label">Data Fim</label>
          <input type="date" v-model="filters.endDate" class="filter-input" />
        </div>
      </div>
      <div class="filter-actions">
        <button @click="loadMovements" class="btn-filter">Filtrar</button>
        <button @click="resetFilters" class="btn-clear">Limpar</button>
      </div>
    </div>

    <!-- Movements Table -->
    <div class="data-table">
      <div class="table-header">
        <h3 class="table-title">Histórico de Movimentações</h3>
      </div>
      <table>
        <thead>
          <tr>
            <th>Data</th>
            <th>Produto</th>
            <th>Tipo</th>
            <th>Quantidade</th>
            <th>Motivo</th>
            <th>Usuário</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="movement in movements" :key="movement.id">
            <td class="movement-date">{{ formatDate(movement.createdAtUtc) }}</td>
            <td class="movement-product">{{ movement.product.name }}</td>
            <td class="movement-type">
              <span :class="movement.type === 'Entrada' ? 'badge-success' : 'badge-danger'">
                {{ movement.type }}
              </span>
            </td>
            <td class="movement-quantity">{{ movement.quantity }}</td>
            <td class="movement-reason">{{ movement.reason }}</td>
            <td class="movement-user">{{ movement.user.name }}</td>
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

<style scoped>
.stock-movements-container {
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

.filters-section {
  background: white;
  border-radius: 12px;
  padding: 20px;
  margin-bottom: 24px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.filters-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 16px;
  margin-bottom: 16px;
}

.filter-group {
  display: flex;
  flex-direction: column;
}

.filter-label {
  font-size: 14px;
  font-weight: 600;
  color: #374151;
  margin-bottom: 6px;
}

.filter-select,
.filter-input {
  padding: 10px 12px;
  border: 2px solid #e2e8f0;
  border-radius: 6px;
  font-size: 14px;
  transition: all 0.2s;
}

.filter-select:focus,
.filter-input:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.filter-actions {
  display: flex;
  gap: 12px;
}

.btn-filter {
  background: #667eea;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-filter:hover {
  background: #5a67d8;
}

.btn-clear {
  background: #f1f5f9;
  color: #334155;
  border: none;
  padding: 10px 20px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-clear:hover {
  background: #e2e8f0;
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

.movement-reason {
  color: #64748b;
  font-size: 14px;
}

.movement-user {
  color: #64748b;
  font-size: 14px;
}

.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal {
  background: white;
  border-radius: 12px;
  padding: 24px;
  width: 100%;
  max-width: 500px;
  max-height: 90vh;
  overflow-y: auto;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.modal-title {
  font-size: 18px;
  font-weight: 600;
  color: #1e293b;
  margin: 0;
}

.modal-close {
  background: none;
  border: none;
  font-size: 24px;
  color: #64748b;
  cursor: pointer;
}

.form-group {
  margin-bottom: 16px;
}

.form-label {
  display: block;
  font-size: 14px;
  font-weight: 600;
  color: #374151;
  margin-bottom: 6px;
}

.form-input {
  width: 100%;
  padding: 10px 12px;
  border: 2px solid #e2e8f0;
  border-radius: 6px;
  font-size: 14px;
  transition: all 0.2s;
}

.form-input:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  margin-top: 24px;
}

.btn-secondary {
  background: #f1f5f9;
  color: #334155;
  border: none;
  padding: 10px 20px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-secondary:hover {
  background: #e2e8f0;
}

.btn-primary-modal {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-primary-modal:hover {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
}
</style>
