<template>
  <div class="maintenance-page min-vh-100 d-flex align-items-center justify-content-center">
    <div class="container">
      <div class="row justify-content-center">
        <div class="col-md-8 col-lg-6 text-center">
          <div class="card shadow-lg border-0">
            <div class="card-body p-5">
              <div class="mb-4">
                <i class="bi bi-tools text-warning" style="font-size: 4rem;"></i>
              </div>
              
              <h1 class="mb-3">We'll Be Right Back</h1>
              
              <div class="alert alert-warning" role="alert">
                <i class="bi bi-exclamation-triangle me-2"></i>
                <strong>Maintenance Mode</strong>
              </div>
              
              <div class="maintenance-message mb-4" v-html="maintenanceMessage"></div>
              
              <div class="text-muted small">
                <p class="mb-0">Thank you for your patience.</p>
              </div>

              <!-- Admin login section -->
              <div class="mt-4 pt-4 border-top">
                <div v-if="!authStore.isAuthenticated">
                  <p class="text-muted small mb-3">
                    <i class="bi bi-shield-lock-fill me-1"></i>
                    Administrator Access
                  </p>
                  <RouterLink to="/login" class="btn btn-outline-primary btn-sm">
                    <i class="bi bi-box-arrow-in-right me-2"></i>
                    Admin Login
                  </RouterLink>
                </div>
                <div v-else-if="isAdmin">
                  <p class="text-muted small mb-3">
                    <i class="bi bi-shield-lock-fill me-1"></i>
                    Logged in as {{ authStore.user?.name }}
                  </p>
                  <RouterLink to="/admin" class="btn btn-primary">
                    <i class="bi bi-unlock-fill me-2"></i>
                    Access Admin Panel
                  </RouterLink>
                </div>
                <div v-else>
                  <p class="text-muted small mb-3">
                    You are logged in but do not have administrator access.
                  </p>
                  <button @click="handleLogout" class="btn btn-outline-secondary btn-sm">
                    <i class="bi bi-box-arrow-right me-2"></i>
                    Logout
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { RouterLink } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import api from '@/services/api'

const authStore = useAuthStore()
const maintenanceMessage = ref('We are currently performing scheduled maintenance. Please check back soon.')

const isAdmin = computed(() => {
  const role = authStore.user?.role
  return role === 'Admin' || role === 'SuperAdmin'
})

const handleLogout = () => {
  authStore.logout()
  window.location.reload()
}

onMounted(async () => {
  try {
    const response = await api.get('/settings/general')
    if (response.data?.maintenanceMessage) {
      maintenanceMessage.value = response.data.maintenanceMessage
    }
  } catch (error) {
    console.error('Failed to load maintenance message:', error)
  }
})
</script>

<style scoped>
.maintenance-page {
  background: linear-gradient(135deg, var(--bs-primary) 0%, color-mix(in srgb, var(--bs-primary) 70%, var(--bs-secondary)) 100%);
}

.card {
  animation: fadeIn 0.5s ease-in;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.maintenance-message {
  white-space: pre-wrap;
  line-height: 1.6;
}
</style>
