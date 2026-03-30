<template>
  <div class="users-container">
    <div class="page-header">
      <h2 class="page-title">Gestão de Usuários</h2>
      <button @click="showCreateModal = true" class="btn-primary">
        Novo Usuário
      </button>
    </div>

    <!-- Users Table -->
    <div class="data-table">
      <div class="table-header">
        <h3 class="table-title">Lista de Usuários</h3>
      </div>
      <table>
        <thead>
          <tr>
            <th>Nome</th>
            <th>Email</th>
            <th>Perfil</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="user in users" :key="user.id">
            <td class="user-name">{{ user.name }}</td>
            <td class="user-email">{{ user.email }}</td>
            <td class="user-role">
              <span :class="user.role === 1 ? 'badge-admin' : 'badge-user'">
                {{ user.role === 1 ? 'Administrador' : 'Funcionário' }}
              </span>
            </td>
            <td class="action-buttons">
              <button @click="editUser(user)" class="btn-edit">
                Editar
              </button>
              <button
                v-if="user.id !== authStore.user?.id"
                @click="deleteUser(user.id)"
                class="btn-delete"
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

<style scoped>
.users-container {
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

.user-name {
  font-weight: 600;
  color: #1e293b;
}

.user-email {
  color: #64748b;
}

.user-role {
  text-align: center;
}

.badge-admin {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
}

.badge-user {
  background: #e2e8f0;
  color: #334155;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
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
