<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h2 class="text-2xl font-bold text-gray-900">Categorias</h2>
      <button
        @click="showCreateModal = true"
        class="bg-indigo-600 text-white px-4 py-2 rounded-md hover:bg-indigo-700"
      >
        Nova Categoria
      </button>
    </div>

    <!-- Categories Table -->
    <div class="bg-white shadow rounded-lg overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Nome
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Descrição
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Ações
            </th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="category in categories" :key="category.id">
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
              {{ category.name }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ category.description }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium space-x-2">
              <button
                @click="editCategory(category)"
                class="text-indigo-600 hover:text-indigo-900"
              >
                Editar
              </button>
              <button
                @click="deleteCategory(category.id)"
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
            {{ showCreateModal ? 'Nova Categoria' : 'Editar Categoria' }}
          </h3>
          <form @submit.prevent="showCreateModal ? createCategory() : updateCategory()">
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
                <label class="block text-sm font-medium text-gray-700">Descrição</label>
                <textarea
                  v-model="formData.description"
                  rows="3"
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm"
                ></textarea>
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
  name: 'Categories',
  data() {
    return {
      categories: [],
      showCreateModal: false,
      showEditModal: false,
      formData: {
        name: '',
        description: ''
      },
      editingCategoryId: null
    }
  },
  methods: {
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
    
    async createCategory() {
      try {
        await axios.post('http://localhost:5177/api/categories', this.formData, {
          headers: {
            Authorization: `Bearer ${this.authStore.token}`
          }
        })
        this.showCreateModal = false
        this.formData = { name: '', description: '' }
        await this.loadCategories()
      } catch (error) {
        console.error('Erro ao criar categoria:', error)
      }
    },
    
    editCategory(category) {
      this.editingCategoryId = category.id
      this.formData = {
        name: category.name,
        description: category.description
      }
      this.showEditModal = true
    },
    
    async updateCategory() {
      try {
        await axios.put(`http://localhost:5177/api/categories/${this.editingCategoryId}`, this.formData, {
          headers: {
            Authorization: `Bearer ${this.authStore.token}`
          }
        })
        this.showEditModal = false
        this.editingCategoryId = null
        this.formData = { name: '', description: '' }
        await this.loadCategories()
      } catch (error) {
        console.error('Erro ao atualizar categoria:', error)
      }
    },
    
    async deleteCategory(categoryId) {
      if (confirm('Tem certeza que deseja excluir esta categoria?')) {
        try {
          await axios.delete(`http://localhost:5177/api/categories/${categoryId}`, {
            headers: {
              Authorization: `Bearer ${this.authStore.token}`
            }
          })
          await this.loadCategories()
        } catch (error) {
          console.error('Erro ao excluir categoria:', error)
        }
      }
    }
  },
  mounted() {
    this.loadCategories()
  }
}
</script>