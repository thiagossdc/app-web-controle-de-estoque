import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useSuppliersStore = defineStore('suppliers', () => {
  const suppliers = ref([])
  const loading = ref(false)
  const error = ref(null)

  async function fetchSuppliers() {
    loading.value = true
    error.value = null
    
    try {
      const response = await fetch('/api/suppliers', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      })

      if (!response.ok) {
        throw new Error('Erro ao carregar fornecedores')
      }

      suppliers.value = await response.json()
    } catch (err) {
      error.value = err.message
      throw err
    } finally {
      loading.value = false
    }
  }

  async function createSupplier(supplier) {
    loading.value = true
    error.value = null
    
    try {
      const response = await fetch('/api/suppliers', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(supplier)
      })

      if (!response.ok) {
        throw new Error('Erro ao criar fornecedor')
      }

      const newSupplier = await response.json()
      suppliers.value.push(newSupplier)
      return newSupplier
    } catch (err) {
      error.value = err.message
      throw err
    } finally {
      loading.value = false
    }
  }

  async function updateSupplier(id, supplier) {
    loading.value = true
    error.value = null
    
    try {
      const response = await fetch(`/api/suppliers/${id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(supplier)
      })

      if (!response.ok) {
        throw new Error('Erro ao atualizar fornecedor')
      }

      const updatedSupplier = await response.json()
      const index = suppliers.value.findIndex(s => s.id === id)
      if (index !== -1) {
        suppliers.value[index] = updatedSupplier
      }
      return updatedSupplier
    } catch (err) {
      error.value = err.message
      throw err
    } finally {
      loading.value = false
    }
  }

  async function deleteSupplier(id) {
    loading.value = true
    error.value = null
    
    try {
      const response = await fetch(`/api/suppliers/${id}`, {
        method: 'DELETE',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      })

      if (!response.ok) {
        throw new Error('Erro ao deletar fornecedor')
      }

      suppliers.value = suppliers.value.filter(s => s.id !== id)
    } catch (err) {
      error.value = err.message
      throw err
    } finally {
      loading.value = false
    }
  }

  return {
    suppliers,
    loading,
    error,
    fetchSuppliers,
    createSupplier,
    updateSupplier,
    deleteSupplier
  }
})