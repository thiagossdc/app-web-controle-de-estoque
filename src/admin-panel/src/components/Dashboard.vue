<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Navigation -->
    <nav class="bg-white shadow-sm">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-16">
          <div class="flex items-center">
            <h1 class="text-xl font-semibold text-gray-900">Painel Administrativo</h1>
          </div>
          <div class="flex items-center space-x-4">
            <span class="text-sm text-gray-700">Bem-vindo, {{ authStore.user?.name }}</span>
            <button
              @click="authStore.logout()"
              class="text-sm text-gray-500 hover:text-gray-700"
            >
              Sair
            </button>
          </div>
        </div>
      </div>
    </nav>

    <!-- Sidebar Navigation -->
    <div class="flex">
      <div class="w-64 bg-white min-h-screen border-r">
        <nav class="mt-5 px-2">
          <router-link
            to="/"
            class="group flex items-center px-2 py-2 text-sm font-medium rounded-md text-gray-600 hover:bg-gray-50 hover:text-gray-900"
          >
            Dashboard
          </router-link>
          <router-link
            to="/products"
            class="group flex items-center px-2 py-2 text-sm font-medium rounded-md text-gray-600 hover:bg-gray-50 hover:text-gray-900"
          >
            Produtos
          </router-link>
          <router-link
            to="/categories"
            class="group flex items-center px-2 py-2 text-sm font-medium rounded-md text-gray-600 hover:bg-gray-50 hover:text-gray-900"
          >
            Categorias
          </router-link>
          <router-link
            to="/suppliers"
            class="group flex items-center px-2 py-2 text-sm font-medium rounded-md text-gray-600 hover:bg-gray-50 hover:text-gray-900"
          >
            Fornecedores
          </router-link>
          <router-link
            to="/stock-movements"
            class="group flex items-center px-2 py-2 text-sm font-medium rounded-md text-gray-600 hover:bg-gray-50 hover:text-gray-900"
          >
            Movimentações
          </router-link>
          <router-link
            v-if="authStore.isAdmin()"
            to="/users"
            class="group flex items-center px-2 py-2 text-sm font-medium rounded-md text-gray-600 hover:bg-gray-50 hover:text-gray-900"
          >
            Usuários
          </router-link>
        </nav>
      </div>

      <!-- Main Content -->
      <div class="flex-1 p-8">
        <router-view />
      </div>
    </div>
  </div>
</template>

<script>
import { useAuthStore } from '../stores/auth'

export default {
  name: 'Dashboard',
  setup() {
    const authStore = useAuthStore()
    return { authStore }
  }
}
</script>