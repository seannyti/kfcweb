<template>
  <div class="dashboard-overview">
    <div class="row mb-4">
      <div class="col">
        <h2 class="mb-0">
          <i class="bi bi-speedometer2 text-warning me-2"></i>
          Dashboard Overview
        </h2>
        <p class="text-muted mb-0">System statistics and recent activity</p>
      </div>
    </div>

    <!-- Error Alert -->
    <div v-if="error" class="alert alert-danger alert-dismissible fade show" role="alert">
      <i class="bi bi-exclamation-triangle me-2"></i>
      {{ error }}
      <button type="button" class="btn-close" @click="error = null"></button>
    </div>

    <!-- Loading Spinner -->
    <div v-if="loading && !stats.totalUsers" class="text-center py-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
      <p class="text-muted mt-3">Loading dashboard data...</p>
    </div>

    <!-- Stats Cards -->
    <div v-else class="row g-4 mb-4">
      <div class="col-md-3">
        <div class="card shadow-sm border-0 h-100">
          <div class="card-body">
            <div class="d-flex align-items-center">
              <div class="bg-primary bg-opacity-10 rounded p-3 me-3">
                <i class="bi bi-people-fill text-primary fs-3"></i>
              </div>
              <div>
                <h6 class="text-muted mb-1">Total Users</h6>
                <h3 class="mb-0">{{ stats.totalUsers }}</h3>
                <small class="text-success">
                  <i class="bi bi-arrow-up"></i> +12 this week
                </small>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="col-md-3">
        <div class="card shadow-sm border-0 h-100">
          <div class="card-body">
            <div class="d-flex align-items-center">
              <div class="bg-success bg-opacity-10 rounded p-3 me-3">
                <i class="bi bi-envelope-fill text-success fs-3"></i>
              </div>
              <div>
                <h6 class="text-muted mb-1">Emails Sent Today</h6>
                <h3 class="mb-0">{{ stats.emailsSentToday }}</h3>
                <small class="text-muted">{{ stats.emailsTotal }} total</small>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="col-md-3">
        <div class="card shadow-sm border-0 h-100">
          <div class="card-body">
            <div class="d-flex align-items-center">
              <div class="bg-warning bg-opacity-10 rounded p-3 me-3">
                <i class="bi bi-clock-fill text-warning fs-3"></i>
              </div>
              <div>
                <h6 class="text-muted mb-1">System Uptime</h6>
                <h3 class="mb-0">{{ stats.uptime }}</h3>
                <small class="text-success">
                  <i class="bi bi-circle-fill" style="font-size: 8px;"></i> Healthy
                </small>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="col-md-3">
        <div class="card shadow-sm border-0 h-100">
          <div class="card-body">
            <div class="d-flex align-items-center">
              <div class="bg-info bg-opacity-10 rounded p-3 me-3">
                <i class="bi bi-cloud-arrow-up-fill text-info fs-3"></i>
              </div>
              <div>
                <h6 class="text-muted mb-1">Last Backup</h6>
                <h3 class="mb-0 fs-6">{{ stats.lastBackup }}</h3>
                <small class="text-muted">Auto backup enabled</small>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- System Status -->
    <div class="row g-4 mb-4">
      <div class="col-lg-8">
        <div class="card shadow-sm border-0">
          <div class="card-header bg-white py-3">
            <h5 class="mb-0">
              <i class="bi bi-activity me-2"></i>
              Recent Admin Actions
            </h5>
          </div>
          <div class="card-body p-0">
            <div class="table-responsive">
              <table class="table table-hover mb-0">
                <thead class="table-light">
                  <tr>
                    <th>Action</th>
                    <th>User</th>
                    <th>Time</th>
                    <th>Status</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="action in recentActions" :key="action.id">
                    <td>
                      <i :class="action.icon" class="me-2"></i>
                      {{ action.action }}
                    </td>
                    <td>{{ action.user }}</td>
                    <td>
                      <small class="text-muted">{{ formatTime(action.time) }}</small>
                    </td>
                    <td>
                      <span class="badge" :class="action.statusClass">
                        {{ action.status }}
                      </span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>

      <div class="col-lg-4">
        <div class="card shadow-sm border-0">
          <div class="card-header bg-white py-3">
            <h5 class="mb-0">
              <i class="bi bi-heart-pulse me-2"></i>
              System Health
            </h5>
          </div>
          <div class="card-body">
            <div class="mb-3">
              <div class="d-flex justify-content-between align-items-center mb-2">
                <span class="small">CPU Usage</span>
                <span class="small fw-bold">{{ systemHealth.cpu }}%</span>
              </div>
              <div class="progress" style="height: 8px;">
                <div 
                  class="progress-bar bg-success" 
                  :style="{ width: systemHealth.cpu + '%' }"
                ></div>
              </div>
            </div>

            <div class="mb-3">
              <div class="d-flex justify-content-between align-items-center mb-2">
                <span class="small">Memory Usage</span>
                <span class="small fw-bold">{{ systemHealth.memory }}%</span>
              </div>
              <div class="progress" style="height: 8px;">
                <div 
                  class="progress-bar bg-warning" 
                  :style="{ width: systemHealth.memory + '%' }"
                ></div>
              </div>
            </div>

            <div class="mb-3">
              <div class="d-flex justify-content-between align-items-center mb-2">
                <span class="small">Disk Usage</span>
                <span class="small fw-bold">{{ systemHealth.disk }}%</span>
              </div>
              <div class="progress" style="height: 8px;">
                <div 
                  class="progress-bar bg-info" 
                  :style="{ width: systemHealth.disk + '%' }"
                ></div>
              </div>
            </div>

            <hr>

            <div class="d-flex justify-content-between align-items-center mb-2">
              <span class="small">Database</span>
              <span class="badge bg-success">
                <i class="bi bi-check-circle me-1"></i>Connected
              </span>
            </div>

            <div class="d-flex justify-content-between align-items-center">
              <span class="small">Email Service</span>
              <span class="badge bg-success">
                <i class="bi bi-check-circle me-1"></i>Active
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import adminService from '@/services/adminService'

const stats = ref({
  totalUsers: 0,
  emailsSentToday: 0,
  emailsTotal: 0,
  uptime: '0%',
  lastBackup: 'Loading...'
})

const systemHealth = ref({
  cpu: 0,
  memory: 0,
  disk: 0
})

const loading = ref(true)
const error = ref<string | null>(null)
let refreshInterval: number | null = null

const recentActions = ref<any[]>([])

const formatTime = (date: Date) => {
  const now = Date.now()
  const diff = now - date.getTime()
  const minutes = Math.floor(diff / 60000)
  const hours = Math.floor(diff / 3600000)
  
  if (minutes < 60) {
    return `${minutes} min ago`
  } else if (hours < 24) {
    return `${hours} hr ago`
  } else {
    return new Intl.DateTimeFormat('en-US', {
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    }).format(date)
  }
}

const loadDashboardData = async () => {
  try {
    loading.value = true
    error.value = null
    
    // Load dashboard stats, system health, and recent activity logs in parallel
    const [dashboardStats, health, activityLogs] = await Promise.all([
      adminService.getDashboardStats(),
      adminService.getRealSystemHealth(),
      adminService.getActivityLogs({ type: 'Admin' })
    ])
    
    stats.value = dashboardStats
    systemHealth.value = health
    
    // Map activity logs to recent actions with icons and formatting
    recentActions.value = activityLogs.slice(0, 5).map((log: any) => ({
      id: log.id,
      action: log.action,
      user: log.userName || 'System',
      time: new Date(log.timestamp),
      status: 'Success',
      statusClass: getStatusClass(log.type),
      icon: getActionIcon(log.action)
    }))
  } catch (err: any) {
    error.value = err.response?.data?.message || 'Failed to load dashboard data'
    console.error('Dashboard load error:', err)
  } finally {
    loading.value = false
  }
}

const getStatusClass = (type: string) => {
  switch (type) {
    case 'Error': return 'bg-danger'
    case 'Warning': return 'bg-warning'
    case 'Admin': return 'bg-success'
    default: return 'bg-info'
  }
}

const getActionIcon = (action: string) => {
  const actionLower = action.toLowerCase()
  if (actionLower.includes('user') || actionLower.includes('role')) return 'bi bi-person-check text-success'
  if (actionLower.includes('email')) return 'bi bi-envelope-check text-success'
  if (actionLower.includes('backup')) return 'bi bi-cloud-check text-success'
  if (actionLower.includes('delete') || actionLower.includes('remove')) return 'bi bi-trash text-danger'
  if (actionLower.includes('create') || actionLower.includes('add')) return 'bi bi-plus-circle text-success'
  if (actionLower.includes('update') || actionLower.includes('edit')) return 'bi bi-pencil text-primary'
  if (actionLower.includes('settings') || actionLower.includes('config')) return 'bi bi-gear text-info'
  return 'bi bi-info-circle text-primary'
}

onMounted(() => {
  // Load data immediately
  loadDashboardData()
  
  // Refresh every 30 seconds
  refreshInterval = window.setInterval(() => {
    loadDashboardData()
  }, 30000)
})

onUnmounted(() => {
  if (refreshInterval) {
    clearInterval(refreshInterval)
  }
})
</script>

<style scoped>
.card {
  border: none;
}

.card-header {
  border-bottom: 2px solid var(--bs-border-color, #f0f0f0);
}

.table th {
  font-weight: 600;
  font-size: 0.875rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.progress {
  border-radius: 10px;
}

.progress-bar {
  border-radius: 10px;
}
</style>
