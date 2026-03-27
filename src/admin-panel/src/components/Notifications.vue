<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h2 class="text-2xl font-bold text-gray-900">Notificações</h2>
      <button
        @click="markAllAsRead"
        v-if="unreadNotifications.length > 0"
        class="bg-gray-600 text-white px-4 py-2 rounded-md hover:bg-gray-700"
      >
        Marcar Todas como Lidas
      </button>
    </div>

    <!-- Notifications List -->
    <div class="space-y-4">
      <div
        v-for="notification in notifications"
        :key="notification.id"
        :class="[
          'bg-white p-4 rounded-lg shadow',
          !notification.readAt ? 'border-l-4 border-yellow-500' : 'border-l-4 border-gray-300'
        ]"
      >
        <div class="flex justify-between items-start">
          <div class="flex-1">
            <h3 class="text-lg font-semibold text-gray-900">{{ notification.title }}</h3>
            <p class="text-gray-600 mt-1">{{ notification.message }}</p>
            <div class="mt-2 text-sm text-gray-500">
              <span>{{ formatDate(notification.createdAtUtc) }}</span>
              <span v-if="notification.readAt" class="ml-2 text-green-600">• Lida</span>
            </div>
          </div>
          <div class="ml-4 flex space-x-2">
            <button
              v-if="!notification.readAt"
              @click="markAsRead(notification.id)"
              class="text-sm text-blue-600 hover:text-blue-800"
            >
              Marcar como lida
            </button>
            <button
              @click="deleteNotification(notification.id)"
              class="text-sm text-red-600 hover:text-red-800"
            >
              Excluir
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div v-if="notifications.length === 0" class="bg-white p-8 rounded-lg shadow text-center">
      <div class="text-gray-500">Nenhuma notificação no momento</div>
    </div>
  </div>
</template>

<script>
import axios from 'axios'

export default {
  name: 'Notifications',
  data() {
    return {
      notifications: []
    }
  },
  computed: {
    unreadNotifications() {
      return this.notifications.filter(n => !n.readAt)
    }
  },
  methods: {
    async loadNotifications() {
      try {
        const response = await axios.get('http://localhost:5177/api/notifications', {
          headers: {
            Authorization: `Bearer ${this.authStore.token}`
          }
        })
        this.notifications = response.data
      } catch (error) {
        console.error('Erro ao carregar notificações:', error)
      }
    },
    
    async markAsRead(notificationId) {
      try {
        await axios.put(`http://localhost:5177/api/notifications/${notificationId}/read`, {}, {
          headers: {
            Authorization: `Bearer ${this.authStore.token}`
          }
        })
        this.loadNotifications()
      } catch (error) {
        console.error('Erro ao marcar notificação como lida:', error)
      }
    },
    
    async markAllAsRead() {
      try {
        await axios.put('http://localhost:5177/api/notifications/mark-all-read', {}, {
          headers: {
            Authorization: `Bearer ${this.authStore.token}`
          }
        })
        this.loadNotifications()
      } catch (error) {
        console.error('Erro ao marcar todas notificações como lidas:', error)
      }
    },
    
    async deleteNotification(notificationId) {
      if (confirm('Tem certeza que deseja excluir esta notificação?')) {
        try {
          await axios.delete(`http://localhost:5177/api/notifications/${notificationId}`, {
            headers: {
              Authorization: `Bearer ${this.authStore.token}`
            }
          })
          this.loadNotifications()
        } catch (error) {
          console.error('Erro ao excluir notificação:', error)
        }
      }
    },
    
    formatDate(dateString) {
      const date = new Date(dateString)
      return date.toLocaleString('pt-BR')
    }
  },
  mounted() {
    this.loadNotifications()
  }
}
</script>