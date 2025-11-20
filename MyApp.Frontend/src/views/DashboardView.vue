<template>
  <div class="dashboard-view py-5">
    <div class="container">
      <!-- Welcome Header -->
      <div class="row mb-4">
        <div class="col">
          <div class="card bg-primary text-white shadow">
            <div class="card-body p-4">
              <h1 class="display-5 mb-2">
                <i class="bi bi-kanban me-3"></i>
                Welcome, {{ authStore.user?.name }}!
              </h1>
              <p class="lead mb-0">
                Manage your construction projects and team coordination
              </p>
            </div>
          </div>
        </div>
      </div>

      <!-- Stats Cards -->
      <!-- <div class="row g-4 mb-4">
        <div class="col-md-4">
          <div class="card h-100 border-0 shadow-sm">
            <div class="card-body text-center p-4">
              <i class="bi bi-diagram-3 text-warning display-4 mb-3"></i>
              <h5 class="card-title">Active Projects</h5>
              <p class="display-6 fw-bold text-primary mb-0">3</p>
              <p class="text-muted small mb-0">In Progress</p>
            </div>
          </div>
        </div>

        <div class="col-md-4">
          <div class="card h-100 border-0 shadow-sm">
            <div class="card-body text-center p-4">
              <i class="bi bi-people text-info display-4 mb-3"></i>
              <h5 class="card-title">Team Members</h5>
              <p class="display-6 fw-bold text-primary mb-0">12</p>
              <p class="text-muted small mb-0">Collaborators</p>
            </div>
          </div>
        </div>

        <div class="col-md-4">
          <div class="card h-100 border-0 shadow-sm">
            <div class="card-body text-center p-4">
              <i class="bi bi-clipboard-check text-success display-4 mb-3"></i>
              <h5 class="card-title">Tasks Completed</h5>
              <p class="display-6 fw-bold text-primary mb-0">87%</p>
              <p class="text-muted small mb-0">This Month</p>
            </div>
          </div>
        </div>
      </div> -->

      <!-- User Information -->
      <div class="row mb-4">
        <div class="col-lg-8">
          <div class="card shadow-sm border-0">
            <div class="card-header bg-white py-3">
              <h5 class="mb-0">
                <i class="bi bi-person-badge me-2"></i>
                Contractor Profile
              </h5>
            </div>
            <div class="card-body">
              <table class="table table-borderless mb-0">
                <tbody>
                  <tr>
                    <td class="text-muted" style="width: 40%">
                      <i class="bi bi-hash me-2"></i>User ID
                    </td>
                    <td class="fw-bold">{{ authStore.user?.id }}</td>
                  </tr>
                  <tr>
                    <td class="text-muted">
                      <i class="bi bi-person me-2"></i>Full Name
                    </td>
                    <td class="fw-bold">{{ authStore.user?.name }}</td>
                  </tr>
                  <tr>
                    <td class="text-muted">
                      <i class="bi bi-shield-check me-2"></i>Role
                    </td>
                    <td>
                      <span 
                        class="badge" 
                        :class="{
                          'bg-danger': authStore.user?.role === 'SuperAdmin',
                          'bg-primary': authStore.user?.role === 'Admin',
                          'bg-success': authStore.user?.role === 'User'
                        }"
                      >
                        {{ authStore.user?.role }}
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td class="text-muted">
                      <i class="bi bi-envelope me-2"></i>Email Address
                    </td>
                    <td class="fw-bold">{{ authStore.user?.email }}</td>
                  </tr>
                  <tr>
                    <td class="text-muted">
                      <i class="bi bi-calendar-plus me-2"></i>Account Created
                    </td>
                    <td class="fw-bold">{{ formatDateTime(authStore.user?.createdAt) }}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <div class="col-lg-4">
          <div class="card shadow-sm border-0">
            <div class="card-header bg-white py-3">
              <h5 class="mb-0">
                <i class="bi bi-lightning-charge me-2"></i>
                Quick Actions
              </h5>
            </div>
            <div class="card-body">
              <div class="d-grid gap-2">
                <button class="btn btn-outline-primary" @click="refreshUserData">
                  <i class="bi bi-arrow-clockwise me-2"></i>
                  Refresh Data
                </button>
                <RouterLink to="/api-docs" class="btn btn-outline-info">
                  <i class="bi bi-book me-2"></i>
                  View API Docs
                </RouterLink>
                <button class="btn btn-outline-danger" @click="handleLogout">
                  <i class="bi bi-box-arrow-right me-2"></i>
                  Logout
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Features Grid -->
      <div class="row">
        <div class="col">
          <div class="card shadow-sm border-0">
            <div class="card-header bg-white py-3">
              <h5 class="mb-0">
                <i class="bi bi-grid-3x3-gap me-2"></i>
                Knudson Family Construction Features
              </h5>
            </div>
            <div class="card-body">
              <div class="row g-3">
                <div class="col-md-6">
                  <div class="d-flex align-items-center p-3 bg-light rounded">
                    <i class="bi bi-kanban text-warning fs-3 me-3"></i>
                    <div>
                      <h6 class="mb-1">Project Boards</h6>
                      <small class="text-muted">Kanban-style task management</small>
                    </div>
                  </div>
                </div>
                <div class="col-md-6">
                  <div class="d-flex align-items-center p-3 bg-light rounded">
                    <i class="bi bi-file-earmark-text text-info fs-3 me-3"></i>
                    <div>
                      <h6 class="mb-1">Document Storage</h6>
                      <small class="text-muted">Plans, specs, and photos</small>
                    </div>
                  </div>
                </div>
                <div class="col-md-6">
                  <div class="d-flex align-items-center p-3 bg-light rounded">
                    <i class="bi bi-clipboard-check text-success fs-3 me-3"></i>
                    <div>
                      <h6 class="mb-1">Punch Lists</h6>
                      <small class="text-muted">Track inspections & issues</small>
                    </div>
                  </div>
                </div>
                <div class="col-md-6">
                  <div class="d-flex align-items-center p-3 bg-light rounded">
                    <i class="bi bi-people-fill text-primary fs-3 me-3"></i>
                    <div>
                      <h6 class="mb-1">Team Collaboration</h6>
                      <small class="text-muted">Real-time updates & chat</small>
                    </div>
                  </div>
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
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const formatDate = (dateString: string | undefined) => {
  if (!dateString) return 'N/A'
  const date = new Date(dateString)
  return date.toLocaleDateString('en-US', { 
    year: 'numeric', 
    month: 'long', 
    day: 'numeric' 
  })
}

const formatDateTime = (dateString: string | undefined) => {
  if (!dateString) return 'N/A'
  const date = new Date(dateString)
  return date.toLocaleDateString('en-US', { 
    year: 'numeric', 
    month: 'long', 
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const refreshUserData = async () => {
  try {
    await authStore.fetchCurrentUser()
    showToast('User data refreshed successfully')
  } catch (error) {
    showToast('Failed to refresh user data', 'error')
  }
}

const handleLogout = () => {
  authStore.logout()
  router.push('/login')
  showToast('Logged out successfully')
}

const showToast = (message: string, type: string = 'info') => {
  const toastElement = document.getElementById('liveToast')
  const toastMessage = document.getElementById('toastMessage')
  
  if (toastElement && toastMessage) {
    toastMessage.textContent = message
    const toast = new (window as any).bootstrap.Toast(toastElement)
    toast.show()
  }
}
</script>

<style scoped>
.dashboard-view {
  min-height: 70vh;
  background: linear-gradient(135deg, var(--theme-light-surface, #f5f7fa) 0%, color-mix(in srgb, var(--bs-primary) 10%, var(--theme-light-bg, #fff)) 100%);
}

.card {
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.card:hover {
  transform: translateY(-2px);
}

.bg-light {
  background-color: var(--theme-light-surface, #f8f9fa) !important;
}
</style>
