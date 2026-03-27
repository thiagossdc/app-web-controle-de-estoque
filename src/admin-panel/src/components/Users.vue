<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h2 class="text-2xl font-bold text-gray-900">Gestão de Usuários</h2>
      <button
        @click="showCreateModal = true"
        class="bg-indigo-600 text-white px-4 py-2 rounded-md hover:bg-indigo-700"
      >
        Novo Usuário
      </button>
    </div>

    <!-- Users Table -->
    <div class="bg-white shadow rounded-lg overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Nome
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Email
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Perfil
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Ações
            </th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="user in users" :key="user.id">
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
              {{ user.name }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ user.email }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ user.role === 1 ? 'Administrador' : 'Funcionário' }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium space-x-2">
              <button
                @click="editUser(user)"
                class="text-indigo-600 hover:text-indigo-900"
              >
                Editar
              </button>
              <button
                v-if="user.id !== authStore.user?.id"
                @click="deleteUser(user.id)"
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
            {{ showCreateModal ? 'Novo Usuário' : 'Editar Usuário' }}
          </h3>
          <form @submit.prevent="showCreateModal ? createUser() : updateUser()">
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
                <label class="block text-sm font-medium text-gray-700">Email</label>
                <input
                  type="email"
                  v-model="formData.email"
                  required
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700">Senha</label>
                <input
                  type="password"
                  v-model="formData.password"
                  :required="showCreateModal"
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700">Perfil</label>
                <select
                  v-model="formData.role"
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm"
                >
                  <option value="2">Funcionário</option>
                  <option value="1">Administrador</option>
                </select>
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
  name: 'Users',
  data() {
    return {
      users: [],
      showCreateModal: false,
      showEditModal: false,
      formData: {
        name: '',
        email: '',
        password: '',
        role: 2
      },
      editingUserId: null
    }
  },
  methods: {
    async loadUsers() {
      try {
        const response = await axios.get('http://localhost:5177/api/users', {
          headers: {
            Authorization: `Bearer ${this.authStore.token}`
          }
        })
        this.users = response.data
      } catch (error) {
        console.error('Erro ao carregar usuários:', error)
      }
    },
    
    async createUser() {
      try {
        await axios.post('http://localhost:5177/api/users', this.formData, {
          headers: {
            Authorization: `Bearer ${this.authStore.token}`
          }
        })
        this.showCreateModal = false
        this.formData = { name: '', email: '', password: '', role: 2 }
        await this.loadUsers()
      } catch (error) {
        console.error('Erro ao criar usuário:', error)
      }
    },
    
    editUser(user) {
      this.editingUserId = user.id
      this.formData = {
        name: user.name,
        email: user.email,
        password: '',
        role: user.role
      }
      this.showEditModal = true
    },
    
    async updateUser() {
      try {
        await axios.put(`http://localhost:5177/api/users/${this.editingUserId}`, this.formData, {
          headers: {
            Authorization: `Bearer ${this.authStore.token}`
          }
        })
        this.showEditModal = false
        this.editingUserId = null
        this.formData = { name: '', email: '', password: '', role: 2 }
        await this.loadUsers()
      } catch (error) {
        console.error('Erro ao atualizar usuário:', error)
      }
    },
    
    async deleteUser(userId) {
      if (confirm('Tem certeza que deseja excluir este usuário?')) {
        try {
          await axios.delete(`http://localhost:5177/api/users/${userId}`, {
            headers: {
              Authorization: `Bearer ${this.authStore.token}`
            }
          })
          await this.loadUsers()
        } catch (error) {
          console.error('Erro ao excluir usuário:', error)
        }
      }
    }
  },
  mounted() {
    this.loadUsers()
  }
}
</script>