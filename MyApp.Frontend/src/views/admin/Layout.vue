<template>
  <div class="admin-layout">
    <div class="d-flex">
      <!-- Sidebar -->
      <aside class="sidebar bg-dark text-white">
        <div class="sidebar-header p-3 border-bottom border-secondary">
          <h5 class="mb-0">
            <i class="bi bi-shield-lock-fill text-warning me-2"></i>
            Admin Panel
          </h5>
        </div>
        
        <nav class="nav flex-column p-2">
          <RouterLink 
            v-for="item in menuItems" 
            :key="item.path"
            :to="item.path" 
            class="nav-link text-white"
            :class="{ 'active': isActiveRoute(item.path) }"
          >
            <i :class="item.icon" class="me-2"></i>
            {{ item.label }}
          </RouterLink>
        </nav>
      </aside>

      <!-- Main Content -->
      <main class="flex-grow-1 p-4">
        <RouterView />
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { RouterLink, RouterView, useRoute } from 'vue-router'
import { useThemeStore } from '@/stores/theme'

const themeStore = useThemeStore()
const route = useRoute()

const menuItems = [
  { path: '/admin', label: 'Dashboard', icon: 'bi bi-speedometer2' },
  { path: '/admin/business-content', label: 'Business Content', icon: 'bi bi-building' },
  { path: '/admin/services', label: 'Services', icon: 'bi bi-tools' },
  { path: '/admin/projects-management', label: 'Projects', icon: 'bi bi-image' },
  { path: '/admin/quote-requests', label: 'Quote Requests', icon: 'bi bi-envelope-paper' },
  { path: '/admin/general-settings', label: 'General Settings', icon: 'bi bi-gear-fill' },
  { path: '/admin/theme', label: 'Theme & Colors', icon: 'bi bi-palette' },
  { path: '/admin/security-settings', label: 'Security', icon: 'bi bi-shield-lock-fill' },
  { path: '/admin/email-configuration', label: 'Email', icon: 'bi bi-envelope-fill' },
  { path: '/admin/maintenance', label: 'Maintenance', icon: 'bi bi-wrench' },
  { path: '/admin/users', label: 'Users', icon: 'bi bi-people-fill' },
  { path: '/admin/activity-logs', label: 'Activity Logs', icon: 'bi bi-journal-text' },
  { path: '/admin/backups', label: 'Backups', icon: 'bi bi-cloud-arrow-up-fill' },
  { path: '/admin/api-keys', label: 'API Keys', icon: 'bi bi-key-fill' },
  { path: '/admin/system-health', label: 'System Health', icon: 'bi bi-heart-pulse-fill' }
]

const isActiveRoute = (path: string) => {
  return route.path === path
}

onMounted(async () => {
  await themeStore.loadTheme()
})
</script>

<style scoped>
.admin-layout {
  min-height: 100vh;
}

.sidebar {
  width: 250px;
  min-height: calc(100vh - 56px);
  position: sticky;
  top: 0;
}

.sidebar-header {
  background-color: rgba(0, 0, 0, 0.2);
}

.nav-link {
  padding: 0.75rem 1rem;
  border-left: 3px solid transparent;
  transition: all 0.2s;
}

.nav-link:hover {
  background-color: rgba(255, 255, 255, 0.1);
  border-left-color: var(--bs-warning);
}

.nav-link.active {
  background-color: rgba(255, 255, 255, 0.15);
  border-left-color: var(--bs-warning);
}
</style>

