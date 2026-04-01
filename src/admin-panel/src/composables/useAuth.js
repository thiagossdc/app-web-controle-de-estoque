import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'

const token = ref(localStorage.getItem('token'))
const userName = ref(localStorage.getItem('userName'))
const userRole = ref(localStorage.getItem('userRole'))

export function useAuth() {
  const router = useRouter()

  const isAuthenticated = computed(() => !!token.value)
  const isAdmin = computed(() => userRole.value === 'Admin')

  const login = async (email, password) => {
    try {
      const response = await fetch('/api/auth/login', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ email, password })
      })

      if (!response.ok) {
        throw new Error('Credenciais inválidas')
      }

      const data = await response.json()
      
      token.value = data.accessToken
      userName.value = data.userName
      userRole.value = data.role

      localStorage.setItem('token', data.accessToken)
      localStorage.setItem('userName', data.userName)
      localStorage.setItem('userRole', data.role)

      return data
    } catch (error) {
      throw error
    }
  }

  const logout = () => {
    token.value = null
    userName.value = null
    userRole.value = null

    localStorage.removeItem('token')
    localStorage.removeItem('userName')
    localStorage.removeItem('userRole')

    router.push('/login')
  }

  const getToken = () => token.value

  return {
    token,
    userName,
    userRole,
    isAuthenticated,
    isAdmin,
    login,
    logout,
    getToken
  }
}