<template>
  <div class="products-container">
    <div class="page-header">
      <h2 class="page-title">Produtos</h2>
      <button @click="showCreateModal = true" class="btn-primary">
        Novo Produto
      </button>
    </div>

    <!-- Products Table -->
    <div class="data-table">
      <div class="table-header">
        <h3 class="table-title">Lista de Produtos</h3>
      </div>
      <table>
        <thead>
          <tr>
            <th>Nome</th>
            <th>SKU</th>
            <th>Categoria</th>
            <th>Estoque</th>
            <th>Preço</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="product in products" :key="product.id">
            <td class="product-name">{{ product.name }}</td>
            <td class="product-sku">{{ product.sku }}</td>
            <td class="product-category">{{ product.category.name }}</td>
            <td class="product-stock">{{ product.currentStock }} (mín: {{ product.minStock }})</td>
            <td class="product-price">R$ {{ product.unitPrice }}</td>
            <td class="action-buttons">
              <button @click="editProduct(product)" class="btn-edit">
                Editar
              </button>
              <button @click="deleteProduct(product.id)" class="btn-delete">
                Excluir
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Create/Edit Modal -->
    <div v-if="showCreateModal || showEditModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full">
      <div class="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white">
        <div class="mt-3">
          <h3 class="text-lg font-medium text-gray-900 mb-4">
            {{ showCreateModal ? 'Novo Produto' : 'Editar Produto' }}
          </h3>
          <form @submit.prevent="showCreateModal ? createProduct() : updateProduct()">
            <div class="space-y-4">
              <div>
                <label class="block text-sm font-medium text-gray-700">Nome</label>
                <input
                  type="text"
                  v-model="formData.name"
                  required
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700">SKU</label>
                <input
                  type="text"
                  v-model="formData.sku"
                  required
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700">Categoria</label>
                <select
                  v-model="formData.categoryId"
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm"
                >
                  <option v-for="category in categories" :key="category.id" :value="category.id">
                    {{ category.name }}
                  </option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700">Fornecedor</label>
                <select
                  v-model="formData.supplierId"
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm"
                >
                  <option v-for="supplier in suppliers" :key="supplier.id" :value="supplier.id">
                    {{ supplier.name }}
                  </option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700">Preço Unitário</label>
                <input
                  type="number"
                  step="0.01"
                  v-model="formData.unitPrice"
                  required
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700">Estoque Atual</label>
                <input
                  type="number"
                  v-model="formData.currentStock"
                  required
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700">Estoque Mínimo</label>
                <input
                  type="number"
                  v-model="formData.minStock"
                  required
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm"
                />
              </div>
            </div>
            <div class="mt-5 flex justify-end space-x-2">
              <button
                type="button"
                @click="showCreateModal = false; showEditModal = false"
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
  name: 'Products',
  data() {
    return {
      products: [],
      categories: [],
      suppliers: [],
      showCreateModal: false,
      showEditModal: false,
      formData: {
        name: '',
        sku: '',
        categoryId: '',
        supplierId: '',
        unitPrice: '',
        currentStock: '',
        minStock: ''
      },
      editingProductId: null
    }
  },
  methods: {
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
    
    async loadCategories() {
      try {
        const response = await axios.get('http://localhost:5177/api/categories', {
          headers: {
            Authorization: `Bearer ${this.authStore.token}`
          }
        })
        this.categories = response.data
      } catch (error) {
        console.error('Erro ao carregar categorias:', error)
      }
    },
    
    async loadSuppliers() {
      try {
        const response = await axios.get('http://localhost:5177/api/suppliers', {
          headers: {
            Authorization: `Bearer ${this.authStore.token}`
          }
        })
        this.suppliers = response.data
      } catch (error) {
        console.error('Erro ao carregar fornecedores:', error)
      }
    },
    
    async createProduct() {
      try {
        await axios.post('http://localhost:5177/api/products', this.formData, {
          headers: {
            Authorization: `Bearer ${this.authStore.token}`
          }
        })
        this.showCreateModal = false
        this.formData = {
          name: '', sku: '', categoryId: '', supplierId: '',
          unitPrice: '', currentStock: '', minStock: ''
        }
        await this.loadProducts()
      } catch (error) {
        console.error('Erro ao criar produto:', error)
      }
    },
    
    editProduct(product) {
      this.editingProductId = product.id
      this.formData = {
        name: product.name,
        sku: product.sku,
        categoryId: product.categoryId,
        supplierId: product.supplierId,
        unitPrice: product.unitPrice,
        currentStock: product.currentStock,
        minStock: product.minStock
      }
      this.showEditModal = true
    },
    
    async updateProduct() {
      try {
        await axios.put(`http://localhost:5177/api/products/${this.editingProductId}`, this.formData, {
          headers: {
            Authorization: `Bearer ${this.authStore.token}`
          }
        })
        this.showEditModal = false
        this.editingProductId = null
        this.formData = {
          name: '', sku: '', categoryId: '', supplierId: '',
          unitPrice: '', currentStock: '', minStock: ''
        }
        await this.loadProducts()
      } catch (error) {
        console.error('Erro ao atualizar produto:', error)
      }
    },
    
    async deleteProduct(productId) {
      if (confirm('Tem certeza que deseja excluir este produto?')) {
        try {
          await axios.delete(`http://localhost:5177/api/products/${productId}`, {
            headers: {
              Authorization: `Bearer ${this.authStore.token}`
            }
          })
          await this.loadProducts()
        } catch (error) {
          console.error('Erro ao excluir produto:', error)
        }
      }
    }
  },
  mounted() {
    this.loadProducts()
    this.loadCategories()
    this.loadSuppliers()
  }
}
</script>

<style scoped>
.products-container {
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

.product-name {
  font-weight: 600;
  color: #1e293b;
}

.product-sku {
  color: #64748b;
  font-size: 12px;
}

.product-category {
  color: #667eea;
  font-weight: 500;
}

.product-stock {
  font-weight: 600;
}

.product-price {
  font-weight: 600;
  color: #198754;
}

.action-buttons {
  display: flex;
  gap: 8px;
}

.btn-edit {
  background: #f1f5f9;
  color: #334155;
  border: none;
  padding: 6px 12px;
  border-radius: 6px;
  font-size: 12px;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-edit:hover {
  background: #e2e8f0;
}

.btn-delete {
  background: #fef2f2;
  color: #dc2626;
  border: none;
  padding: 6px 12px;
  border-radius: 6px;
  font-size: 12px;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-delete:hover {
  background: #fee2e2;
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
