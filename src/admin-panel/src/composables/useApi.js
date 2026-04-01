import { ref } from 'vue'

export function useApi() {
  const loading = ref(false)
  const error = ref(null)

  const request = async (url, options = {}) => {
    loading.value = true
    error.value = null

    try {
      const token = localStorage.getItem('token')
      
      const defaultHeaders = {
        'Content-Type': 'application/json',
        ...(token && { 'Authorization': `Bearer ${token}` })
      }

      const response = await fetch(url, {
        ...options,
        headers: {
          ...defaultHeaders,
          ...options.headers
        }
      })

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}))
        throw new Error(errorData.message || `Erro ${response.status}: ${response.statusText}`)
      }

      const data = await response.json()
      return data
    } catch (err) {
      error.value = err.message
      throw err
    } finally {
      loading.value = false
    }
  }

  const get = (url) => request(url)
  
  const post = (url, data) => request(url, {
    method: 'POST',
    body: JSON.stringify(data)
  })
  
  const put = (url, data) => request(url, {
    method: 'PUT',
    body: JSON.stringify(data)
  })
  
  const del = (url) => request(url, {
    method: 'DELETE'
  })

  return {
    loading,
    error,
    request,
    get,
    post,
    put,
    del
  }
}