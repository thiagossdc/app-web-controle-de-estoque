<template>
  <div class="suppliers-container">
    <div class="page-header">
      <h2 class="page-title">Fornecedores</h2>
      <button @click="showCreateModal = true" class="btn-primary">
        Novo Fornecedor
      </button>
    </div>

    <!-- Suppliers Table -->
    <div class="data-table">
      <div class="table-header">
        <h3 class="table-title">Lista de Fornecedores</h3>
      </div>
      <table>
        <thead>
          <tr>
            <th>Nome</th>
            <th>Contato</th>
            <th>Email</th>
            <th>Telefone</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="supplier in suppliers" :key="supplier.id">
            <td class="supplier-name">{{ supplier.name }}</td>
            <td class="supplier-contact">{{ supplier.contactName }}</td>
            <td class="supplier-email">{{ supplier.email }}</td>
            <td class="supplier-phone">{{ supplier.phone }}</td>
            <td class="action-buttons">
              <button @click="editSupplier(supplier)" class="btn-edit">
                Editar
              </button>
              <button @click="deleteSupplier(supplier.id)" class="btn-delete">
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

<style scoped>
.suppliers-container {
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

.supplier-name {
  font-weight: 600;
  color: #1e293b;
}

.supplier-contact {
  color: #64748b;
}

.supplier-email {
  color: #667eea;
  font-weight: 500;
}

.supplier-phone {
  color: #64748b;
  font-family: monospace;
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
