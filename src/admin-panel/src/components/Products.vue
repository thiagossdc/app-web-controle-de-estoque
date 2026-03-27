<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h2 class="text-2xl font-bold text-gray-900">Produtos</h2>
      <button
        @click="showCreateModal = true"
        class="bg-indigo-600 text-white px-4 py-2 rounded-md hover:bg-indigo-700"
      >
        Novo Produto
      </button>
    </div>

    <!-- Products Table -->
    <div class="bg-white shadow rounded-lg overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Nome
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              SKU
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Categoria
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Estoque
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Preço
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Ações
            </th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="product in products" :key="product.id">
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
              {{ product.name }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ product.sku }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ product.category.name }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ product.currentStock }} (mín: {{ product.minStock }})
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              R$ {{ product.unitPrice }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium space-x-2">
              <button
                @click="editProduct(product)"
                class="text-indigo-600 hover:text-indigo-900"
              >
                Editar
              </button>
              <button
                @click="deleteProduct(product.id)"
                class="text-red-600 hover:text-red-900"
              >
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