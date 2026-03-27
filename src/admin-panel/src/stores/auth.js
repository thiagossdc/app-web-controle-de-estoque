import { defineStore } from 'pinia'
import axios from 'axios'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null,
    token: localStorage.getItem('token'),
    isAuthenticated: !!localStorage.getItem('token')
  }),
  
  actions: {
    async login(email, password) {
      try {
        const response = await axios.post('http://localhost:5177/api/auth/login', {
          email,
          password
        })
        
        this.token = response.data.token
        this.user = response.data.user
        this.isAuthenticated = true
        
        localStorage.setItem('token', this.token)
        return { success: true }
      } catch (error) {
        return { success: false, error: error.response?.data?.message || 'Login failed' }
      }
    },
    
    logout() {
      this.token = null
      this.user = null
      this.isAuthenticated = false
      localStorage.removeItem('token')
    },
    
    checkAuth() {
      return this.isAuthenticated && !!this.token
    },
    
    isAdmin() {
      return this.user?.role === 1
    }
  }
})