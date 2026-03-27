<template>
  <div>
    <!-- Navigation -->
    <nav v-if="authStore.checkAuth()" class="bg-gray-800 text-white">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-16">
          <div class="flex items-center">
            <h1 class="text-xl font-bold">Admin Panel</h1>
          </div>
          <div class="flex items-center space-x-4">
            <router-link to="/" class="hover:bg-gray-700 px-3 py-2 rounded">Dashboard</router-link>
            <router-link to="/users" class="hover:bg-gray-700 px-3 py-2 rounded">Usuários</router-link>
            <router-link to="/products" class="hover:bg-gray-700 px-3 py-2 rounded">Produtos</router-link>
            <router-link to="/stock-movements" class="hover:bg-gray-700 px-3 py-2 rounded">Movimentações</router-link>
            <router-link to="/suppliers" class="hover:bg-gray-700 px-3 py-2 rounded">Fornecedores</router-link>
            <router-link to="/categories" class="hover:bg-gray-700 px-3 py-2 rounded">Categorias</router-link>
            <router-link to="/reports" class="hover:bg-gray-700 px-3 py-2 rounded">Relatórios</router-link>
            <router-link to="/notifications" class="hover:bg-gray-700 px-3 py-2 rounded">Notificações</router-link>
            <button @click="authStore.logout()" class="hover:bg-gray-700 px-3 py-2 rounded">Sair</button>
          </div>
        </div>
      </div>
    </nav>
    
    <!-- Main Content -->
    <router-view />
  </div>
</template>

<script>
import { useAuthStore } from './stores/auth'

export default {
  name: 'App',
  setup() {
    const authStore = useAuthStore()
    return { authStore }
  },
  created() {
    // Protege as rotas que exigem autenticação
    this.$router.beforeEach((to, from, next) => {
      if (to.meta.requiresAuth && !this.authStore.checkAuth()) {
        next('/login')
      } else if (to.path === '/login' && this.authStore.checkAuth()) {
        next('/')
      } else {
        next()
      }
    })
  }
}
</script>
