<template>
  <div class="dashboard-container">
    <!-- Navigation -->
    <nav class="navbar">
      <div class="nav-content">
        <div class="nav-brand">
          <h1>Painel Administrativo</h1>
        </div>
        <div class="nav-user">
          <span class="user-greeting">Bem-vindo, {{ authStore.user?.name }}</span>
          <button @click="authStore.logout()" class="logout-btn">
            Sair
          </button>
        </div>
      </div>
    </nav>

    <!-- Sidebar Navigation -->
    <div class="dashboard-layout">
      <aside class="sidebar">
        <nav class="sidebar-nav">
          <router-link to="/" class="nav-link" exact-active-class="active">
            Dashboard
          </router-link>
          <router-link to="/products" class="nav-link" active-class="active">
            Produtos
          </router-link>
          <router-link to="/categories" class="nav-link" active-class="active">
            Categorias
          </router-link>
          <router-link to="/suppliers" class="nav-link" active-class="active">
            Fornecedores
          </router-link>
          <router-link to="/stock-movements" class="nav-link" active-class="active">
            Movimentações
          </router-link>
          <router-link v-if="authStore.isAdmin()" to="/users" class="nav-link" active-class="active">
            Usuários
          </router-link>
        </nav>
      </aside>

      <!-- Main Content -->
      <main class="main-content">
        <router-view />
      </main>
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

<style scoped>
.dashboard-container {
  min-height: 100vh;
  background: #f8f9fa;
}

.navbar {
  background: white;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  padding: 0 20px;
}

.nav-content {
  max-width: 1200px;
  margin: 0 auto;
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: 64px;
}

.nav-brand h1 {
  margin: 0;
  font-size: 20px;
  font-weight: 600;
  color: #1e293b;
}

.nav-user {
  display: flex;
  align-items: center;
  gap: 16px;
}

.user-greeting {
  font-size: 14px;
  color: #64748b;
}

.logout-btn {
  background: none;
  border: 1px solid #e2e8f0;
  color: #64748b;
  padding: 8px 16px;
  border-radius: 6px;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.2s;
}

.logout-btn:hover {
  background: #f1f5f9;
  color: #334155;
}

.sidebar {
  width: 256px;
  background: white;
  min-height: calc(100vh - 64px);
  border-right: 1px solid #e2e8f0;
}

.sidebar-nav {
  padding: 20px 16px;
}

.nav-link {
  display: flex;
  align-items: center;
  padding: 12px 16px;
  margin-bottom: 4px;
  border-radius: 8px;
  color: #64748b;
  text-decoration: none;
  font-size: 14px;
  font-weight: 500;
  transition: all 0.2s;
}

.nav-link:hover {
  background: #f1f5f9;
  color: #334155;
}

.nav-link.active {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.main-content {
  flex: 1;
  padding: 32px;
  background: #f8f9fa;
  min-height: calc(100vh - 64px);
}
</style>
