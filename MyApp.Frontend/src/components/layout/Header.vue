<template>
  <header class="navbar navbar-expand-lg navbar-dark bg-dark">
    <div class="container">
      <RouterLink to="/" class="navbar-brand">
        <img v-if="settingsStore.settings.logo" :src="settingsStore.settings.logo" alt="Logo" style="height: 30px; margin-right: 8px;">
        <i v-else class="bi bi-hammer me-2"></i>
        {{ settingsStore.settings.siteName }}
      </RouterLink>
      
      <button 
        class="navbar-toggler" 
        type="button" 
        data-bs-toggle="collapse" 
        data-bs-target="#navbarNav"
      >
        <span class="navbar-toggler-icon"></span>
      </button>
      
      <div 
        class="collapse navbar-collapse" 
        id="navbarNav"
      >
        <ul class="navbar-nav ms-auto">
          <li class="nav-item">
            <RouterLink to="/" class="nav-link" @click="closeNavbar">Home</RouterLink>
          </li>
          <li class="nav-item">
            <RouterLink to="/about" class="nav-link" @click="closeNavbar">About</RouterLink>
          </li>
          <li class="nav-item">
            <RouterLink to="/services" class="nav-link" @click="closeNavbar">Services</RouterLink>
          </li>
          <li class="nav-item">
            <RouterLink to="/projects" class="nav-link" @click="closeNavbar">Projects</RouterLink>
          </li>
          <li class="nav-item">
            <RouterLink to="/contact" class="nav-link" @click="closeNavbar">Contact</RouterLink>
          </li>
          
          <template v-if="authStore.isAuthenticated">
            <li class="nav-item">
              <RouterLink to="/dashboard" class="nav-link" @click="closeNavbar">Dashboard</RouterLink>
            </li>
            <li class="nav-item" v-if="isAdmin">
              <RouterLink to="/admin" class="nav-link" @click="closeNavbar">Admin</RouterLink>
            </li>
            <li class="nav-item">
              <a href="#" @click.prevent="handleLogout" class="nav-link">Logout</a>
            </li>
          </template>
          
          <template v-else>
            <li class="nav-item">
              <RouterLink to="/login" class="nav-link" @click="closeNavbar">Login</RouterLink>
            </li>
          </template>
        </ul>
      </div>
    </div>
  </header>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useSettingsStore } from '@/stores/settings'

const authStore = useAuthStore()
const settingsStore = useSettingsStore()
const router = useRouter()

onMounted(() => {
  settingsStore.loadSettings()
})

const isAdmin = computed(() => {
  const role = authStore.user?.role
  return role === 'Admin' || role === 'SuperAdmin'
})

const handleLogout = () => {
  authStore.logout()
  router.push('/login')
}

const closeNavbar = () => {
  const navbarCollapse = document.getElementById('navbarNav')
  if (navbarCollapse?.classList.contains('show')) {
    navbarCollapse.classList.remove('show')
  }
}
</script>

<style scoped>
.navbar {
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}
</style>
