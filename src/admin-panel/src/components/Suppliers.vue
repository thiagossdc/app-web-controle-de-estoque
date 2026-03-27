<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h2 class="text-2xl font-bold text-gray-900">Fornecedores</h2>
      <button
        @click="showCreateModal = true"
        class="bg-indigo-600 text-white px-4 py-2 rounded-md hover:bg-indigo-700"
      >
        Novo Fornecedor
      </button>
    </div>

    <!-- Suppliers Table -->
    <div class="bg-white shadow rounded-lg overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Nome
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Contato
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Email
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Telefone
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Ações
            </th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="supplier in suppliers" :key="supplier.id">
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
              {{ supplier.name }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ supplier.contactName }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ supplier.email }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ supplier.phone }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium space-x-2">
              <button
                @click="editSupplier(supplier)"
                class="text-indigo-600 hover:text-indigo-900"
              >
                Editar
              </button>
              <button
                @click="deleteSupplier(supplier.id)"
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
            {{ showCreateModal ? 'Novo Fornecedor' : 'Editar Fornecedor' }}
          </h3>
          <form @submit.prevent="showCreateModal ? createSupplier() : updateSupplier()">
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
                <label class="block text-sm font-medium text-gray-700">Nome do Contato</label>
                <input
                  type="text"
                  v-model="formData.contactName"
                  required
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700">Email</label>
                <input
                  type="email"
                  v-model="formData.email"
                  required
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700">Telefone</label>
                <input
                  type="tel"
                  v-model="formData.phone"
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
  name: 'Suppliers',
  data() {
    return {
      suppliers: [],
      showCreateModal: false,
      showEditModal: false,
      formData: {
        name: '',
        contactName: '',
        email: '',
        phone: ''
      },
      editingSupplierId: null
    }
  },
  methods: {
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
    
    async createSupplier() {
      try {
        await axios.post('http://localhost:5177/api/suppliers', this.formData, {
          headers: {
            Authorization: `Bearer ${this.authStore.token}`
          }
        })
        this.showCreateModal = false
        this.formData = { name: '', contactName: '', email: '', phone: '' }
        await this.loadSuppliers()
      } catch (error) {
        console.error('Erro ao criar fornecedor:', error)
      }
    },
    
    editSupplier(supplier) {
      this.editingSupplierId = supplier.id
      this.formData = {
        name: supplier.name,
        contactName: supplier.contactName,
        email: supplier.email,
        phone: supplier.phone
      }
      this.showEditModal = true
    },
    
    async updateSupplier() {
      try {
        await axios.put(`http://localhost:5177/api/suppliers/${this.editingSupplierId}`, this.formData, {
          headers: {
            Authorization: `Bearer ${this.authStore.token}`
          }
        })
        this.showEditModal = false
        this.editingSupplierId = null
        this.formData = { name: '', contactName: '', email: '', phone: '' }
        await this.loadSuppliers()
      } catch (error) {
        console.error('Erro ao atualizar fornecedor:', error)
      }
    },
    
    async deleteSupplier(supplierId) {
      if (confirm('Tem certeza que deseja excluir este fornecedor?')) {
        try {
          await axios.delete(`http://localhost:5177/api/suppliers/${supplierId}`, {
            headers: {
              Authorization: `Bearer ${this.authStore.token}`
            }
          })
          await this.loadSuppliers()
        } catch (error) {
          console.error('Erro ao excluir fornecedor:', error)
        }
      }
    }
  },
  mounted() {
    this.loadSuppliers()
  }
}
</script>