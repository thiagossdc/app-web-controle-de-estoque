<template>
  <div class="notifications-container">
    <div class="page-header">
      <h2 class="page-title">Notificações</h2>
      <button
        v-if="unreadNotifications.length > 0"
        @click="markAllAsRead"
        class="btn-secondary"
      >
        Marcar Todas como Lidas
      </button>
    </div>

    <!-- Notifications List -->
    <div class="notifications-list">
      <div
        v-for="notification in notifications"
        :key="notification.id"
        :class="['notification-card', { unread: !notification.readAt }]"
      >
        <div class="notification-content">
          <div class="notification-header">
            <h3 class="notification-title">{{ notification.title }}</h3>
            <div class="notification-status">
              <span v-if="!notification.readAt" class="unread-badge">Não lida</span>
              <span v-else class="read-badge">Lida</span>
            </div>
          </div>
          <p class="notification-message">{{ notification.message }}</p>
          <div class="notification-footer">
            <span class="notification-date">{{ formatDate(notification.createdAtUtc) }}</span>
            <div class="notification-actions">
              <button
                v-if="!notification.readAt"
                @click="markAsRead(notification.id)"
                class="btn-read"
              >
                Marcar como lida
              </button>
              <button
                @click="deleteNotification(notification.id)"
                class="btn-delete"
              >
                Excluir
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div v-if="notifications.length === 0" class="empty-state">
      <div class="empty-icon">
        <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9"></path>
          <path d="M13.73 21a2 2 0 0 1-3.46 0"></path>
        </svg>
      </div>
      <h3>Nenhuma notificação</h3>
      <p>Você está em dia! Não há notificações pendentes.</p>
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

<style scoped>
.notifications-container {
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

.btn-secondary {
  background: #f1f5f9;
  color: #334155;
  border: none;
  padding: 12px 24px;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-secondary:hover {
  background: #e2e8f0;
}

.notifications-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.notification-card {
  background: white;
  border-radius: 12px;
  padding: 20px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  border-left: 4px solid #e2e8f0;
  transition: all 0.2s;
}

.notification-card.unread {
  border-left-color: #667eea;
  background: #f8fafc;
}

.notification-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.notification-content {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.notification-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.notification-title {
  font-size: 16px;
  font-weight: 600;
  color: #1e293b;
  margin: 0;
}

.notification-status {
  display: flex;
  align-items: center;
}

.unread-badge {
  background: #667eea;
  color: white;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
}

.read-badge {
  background: #e2e8f0;
  color: #64748b;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
}

.notification-message {
  color: #64748b;
  font-size: 14px;
  line-height: 1.5;
  margin: 0;
}

.notification-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 8px;
}

.notification-date {
  font-size: 12px;
  color: #94a3b8;
}

.notification-actions {
  display: flex;
  gap: 8px;
}

.btn-read {
  background: #dcfce7;
  color: #166534;
  border: none;
  padding: 6px 12px;
  border-radius: 6px;
  font-size: 12px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-read:hover {
  background: #bbf7d0;
}

.btn-delete {
  background: #fef2f2;
  color: #dc2626;
  border: none;
  padding: 6px 12px;
  border-radius: 6px;
  font-size: 12px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-delete:hover {
  background: #fee2e2;
}

.empty-state {
  background: white;
  border-radius: 12px;
  padding: 48px 24px;
  text-align: center;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.empty-icon {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 80px;
  height: 80px;
  background: #f1f5f9;
  border-radius: 50%;
  margin-bottom: 24px;
  color: #94a3b8;
}

.empty-state h3 {
  font-size: 18px;
  font-weight: 600;
  color: #1e293b;
  margin: 0 0 8px 0;
}

.empty-state p {
  color: #64748b;
  font-size: 14px;
  margin: 0;
}
</style>
