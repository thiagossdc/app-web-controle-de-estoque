import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export const useProductsStore = defineStore('products', () => {
  const products = ref([])
  const loading = ref(false)
  const error = ref(null)

  const totalProducts = computed(() => products.value.length)
  const lowStockProducts = computed(() => 
    products.value.filter(p => p.currentStock <= p.minStock)
  )

  async function fetchProducts(search = '', page = 1, pageSize = 20) {
    loading.value = true
    error.value = null
    
    try {
      const params = new URLSearchParams({
        page: page.toString(),
        pageSize: pageSize.toString()
      })
      
      if (search) {
        params.append('search', search)
      }

      const response = await fetch(`/api/products?${params}`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      })

      if (!response.ok) {
        throw new Error('Erro ao carregar produtos')
      }

      const data = await response.json()
      products.value = data.items
      return data
    } catch (err) {
      error.value = err.message
      throw err
    } finally {
      loading.value = false
    }
  }

  async function createProduct(product) {
    loading.value = true
    error.value = null
    
    try {
      const response = await fetch('/api/products', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(product)
      })

      if (!response.ok) {
        throw new Error('Erro ao criar produto')
      }

      const newProduct = await response.json()
      products.value.push(newProduct)
      return newProduct
    } catch (err) {
      error.value = err.message
      throw err
    } finally {
      loading.value = false
    }
  }

  async function updateProduct(id, product) {
    loading.value = true
    error.value = null
    
    try {
      const response = await fetch(`/api/products/${id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(product)
      })

      if (!response.ok) {
        throw new Error('Erro ao atualizar produto')
      }

      const updatedProduct = await response.json()
      const index = products.value.findIndex(p => p.id === id)
      if (index !== -1) {
        products.value[index] = updatedProduct
      }
      return updatedProduct
    } catch (err) {
      error.value = err.message
      throw err
    } finally {
      loading.value = false
    }
  }

  async function deleteProduct(id) {
    loading.value = true
    error.value = null
    
    try {
      const response = await fetch(`/api/products/${id}`, {
        method: 'DELETE',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      })

      if (!response.ok) {
        throw new Error('Erro ao deletar produto')
      }

      products.value = products.value.filter(p => p.id !== id)
    } catch (err) {
      error.value = err.message
      throw err
    } finally {
      loading.value = false
    }
  }

  return {
    products,
    loading,
    error,
    totalProducts,
    lowStockProducts,
    fetchProducts,
    createProduct,
    updateProduct,
    deleteProduct
  }
})