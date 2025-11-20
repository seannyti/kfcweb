<template>
  <div class="activity-logs">
    <div class="row mb-4">
      <div class="col">
        <h2 class="mb-0">
          <i class="bi bi-journal-text text-warning me-2"></i>
          Activity Logs
        </h2>
        <p class="text-muted mb-0">Monitor system and user activity</p>
      </div>
      <div class="col-auto">
        <button class="btn btn-outline-primary" @click="exportLogs">
          <i class="bi bi-download me-2"></i>Export CSV
        </button>
      </div>
    </div>

    <!-- Filters -->
    <div class="card shadow-sm mb-4">
      <div class="card-body">
        <div class="row g-3">
          <div class="col-md-3">
            <select class="form-select" v-model="filterType">
              <option value="">All Types</option>
              <option value="user">User Activity</option>
              <option value="admin">Admin Actions</option>
              <option value="system">System Events</option>
              <option value="security">Security</option>
            </select>
          </div>
          <div class="col-md-3">
            <input type="date" class="form-control" v-model="filterDate">
          </div>
          <div class="col-md-6">
            <input 
              type="text" 
              class="form-control" 
              placeholder="Search logs..."
              v-model="searchQuery"
            >
          </div>
        </div>
      </div>
    </div>

    <!-- Logs Table -->
    <div class="card shadow-sm">
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr>
                <th>Timestamp</th>
                <th>Type</th>
                <th>User</th>
                <th>Action</th>
                <th>IP Address</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="log in filteredLogs" :key="log.id">
                <td>
                  <small>{{ formatDateTime(log.timestamp) }}</small>
                </td>
                <td>
                  <span 
                    class="badge"
                    :class="{
                      'bg-primary': log.type === 'user',
                      'bg-warning': log.type === 'admin',
                      'bg-info': log.type === 'system',
                      'bg-danger': log.type === 'security'
                    }"
                  >
                    {{ log.type }}
                  </span>
                </td>
                <td>{{ log.userName || 'System' }}</td>
                <td>
                  <i :class="log.icon" class="me-2"></i>
                  {{ log.action }}
                </td>
                <td>
                  <small class="text-muted">{{ log.ipAddress }}</small>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
      <div class="card-footer bg-white">
        <div class="d-flex justify-content-between align-items-center">
          <span class="text-muted">Showing {{ filteredLogs.length }} of {{ logs.length }} logs</span>
          <nav>
            <ul class="pagination pagination-sm mb-0">
              <li class="page-item"><a class="page-link" href="#">Previous</a></li>
              <li class="page-item active"><a class="page-link" href="#">1</a></li>
              <li class="page-item"><a class="page-link" href="#">2</a></li>
              <li class="page-item"><a class="page-link" href="#">3</a></li>
              <li class="page-item"><a class="page-link" href="#">Next</a></li>
            </ul>
          </nav>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useToast } from 'vue-toastification'
import api from '@/services/api'

const toast = useToast()

const filterType = ref('')
const filterDate = ref('')
const searchQuery = ref('')
const loading = ref(true)

const logs = ref<any[]>([])

const loadLogs = async () => {
  try {
    loading.value = true
    const response = await api.get('/admin/activity-logs', {
      params: { type: filterType.value || undefined, limit: 100 }
    })
    logs.value = response.data.map((log: any) => ({
      ...log,
      icon: getLogIcon(log.type, log.action)
    }))
  } catch (error: any) {
    toast.error(`Failed to load activity logs: ${error.response?.data?.message || error.message}`)
  } finally {
    loading.value = false
  }
}

const getLogIcon = (type: string, action: string) => {
  if (action.toLowerCase().includes('login')) return 'bi bi-box-arrow-in-right text-primary'
  if (action.toLowerCase().includes('logout')) return 'bi bi-box-arrow-right text-secondary'
  if (action.toLowerCase().includes('locked')) return 'bi bi-lock text-danger'
  if (action.toLowerCase().includes('unlocked')) return 'bi bi-unlock text-success'
  if (action.toLowerCase().includes('deleted')) return 'bi bi-trash text-danger'
  if (action.toLowerCase().includes('role')) return 'bi bi-person-check text-success'
  if (action.toLowerCase().includes('failed')) return 'bi bi-exclamation-triangle text-danger'
  if (type === 'security') return 'bi bi-shield-exclamation text-warning'
  if (type === 'system') return 'bi bi-gear text-info'
  return 'bi bi-circle-fill text-primary'
}

const filteredLogs = computed(() => {
  return logs.value.filter(log => {
    const matchesType = !filterType.value || log.type === filterType.value
    const userName = log.userName || 'System'
    const matchesSearch = !searchQuery.value || 
                         log.action.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
                         userName.toLowerCase().includes(searchQuery.value.toLowerCase())
    return matchesType && matchesSearch
  })
})

const formatDateTime = (date: Date) => {
  return new Intl.DateTimeFormat('en-US', {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(new Date(date))
}

const exportLogs = () => {
  try {
    const csv = [
      ['Timestamp', 'Type', 'User', 'Action', 'IP Address'].join(','),
      ...filteredLogs.value.map(log => [
        formatDateTime(log.timestamp),
        log.type,
        log.userName || 'System',
        log.action,
        log.ipAddress || 'N/A'
      ].join(','))
    ].join('\n')
    
    const blob = new Blob([csv], { type: 'text/csv' })
    const url = window.URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = `activity-logs-${new Date().toISOString()}.csv`
    a.click()
    window.URL.revokeObjectURL(url)
    toast.success('Activity logs exported successfully')
  } catch (error) {
    toast.error('Failed to export logs')
  }
}

onMounted(() => {
  loadLogs()
})
</script>

<style scoped>
.card {
  border: none;
}

.table th {
  font-weight: 600;
  font-size: 0.875rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}
</style>
