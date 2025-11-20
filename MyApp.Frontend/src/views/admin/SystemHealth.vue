<template>
  <div class="system-health">
    <div class="row mb-4">
      <div class="col">
        <h2 class="mb-0">
          <i class="bi bi-heart-pulse-fill text-warning me-2"></i>
          System Health
        </h2>
        <p class="text-muted mb-0">Monitor system performance and status</p>
      </div>
      <div class="col-auto">
        <button class="btn btn-outline-primary" @click="refreshHealth">
          <i class="bi bi-arrow-clockwise me-2"></i>Refresh
        </button>
      </div>
    </div>

    <!-- Status Cards -->
    <div class="row g-4 mb-4">
      <div class="col-md-3">
        <div class="card shadow-sm h-100">
          <div class="card-body text-center">
            <i class="bi bi-cpu fs-1 text-primary mb-2"></i>
            <h6 class="text-muted">CPU Usage</h6>
            <h2 class="mb-0">{{ health.cpu }}%</h2>
            <div class="progress mt-3" style="height: 6px;">
              <div 
                class="progress-bar"
                :class="getCpuClass(health.cpu)"
                :style="{ width: health.cpu + '%' }"
              ></div>
            </div>
          </div>
        </div>
      </div>

      <div class="col-md-3">
        <div class="card shadow-sm h-100">
          <div class="card-body text-center">
            <i class="bi bi-memory fs-1 text-warning mb-2"></i>
            <h6 class="text-muted">Memory</h6>
            <h2 class="mb-0">{{ health.memory }}%</h2>
            <div class="progress mt-3" style="height: 6px;">
              <div 
                class="progress-bar bg-warning"
                :style="{ width: health.memory + '%' }"
              ></div>
            </div>
          </div>
        </div>
      </div>

      <div class="col-md-3">
        <div class="card shadow-sm h-100">
          <div class="card-body text-center">
            <i class="bi bi-hdd fs-1 text-info mb-2"></i>
            <h6 class="text-muted">Disk Space</h6>
            <h2 class="mb-0">{{ health.disk }}%</h2>
            <div class="progress mt-3" style="height: 6px;">
              <div 
                class="progress-bar bg-info"
                :style="{ width: health.disk + '%' }"
              ></div>
            </div>
          </div>
        </div>
      </div>

      <div class="col-md-3">
        <div class="card shadow-sm h-100">
          <div class="card-body text-center">
            <i class="bi bi-clock fs-1 text-success mb-2"></i>
            <h6 class="text-muted">Uptime</h6>
            <h2 class="mb-0">{{ health.uptime }}</h2>
            <small class="text-muted d-block mt-2">Days</small>
          </div>
        </div>
      </div>
    </div>

    <!-- Services Status -->
    <div class="card shadow-sm mb-4">
      <div class="card-header bg-white py-3">
        <h5 class="mb-0">
          <i class="bi bi-diagram-3 me-2"></i>
          Services Status
        </h5>
      </div>
      <div class="card-body">
        <div class="row g-3">
          <div class="col-md-6" v-for="service in services" :key="service.name">
            <div class="d-flex align-items-center justify-content-between p-3 border rounded">
              <div class="d-flex align-items-center">
                <i :class="service.icon" class="fs-4 me-3"></i>
                <div>
                  <h6 class="mb-0">{{ service.name }}</h6>
                  <small class="text-muted">{{ service.description }}</small>
                </div>
              </div>
              <span 
                class="badge"
                :class="service.status === 'healthy' ? 'bg-success' : 'bg-danger'"
              >
                <i 
                  :class="service.status === 'healthy' ? 'bi bi-check-circle' : 'bi bi-x-circle'"
                  class="me-1"
                ></i>
                {{ service.status }}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Recent Errors -->
    <div class="card shadow-sm">
      <div class="card-header bg-white py-3">
        <h5 class="mb-0">
          <i class="bi bi-exclamation-triangle me-2"></i>
          Recent Errors
        </h5>
      </div>
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr>
                <th>Time</th>
                <th>Type</th>
                <th>Message</th>
                <th>Action</th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="errors.length === 0">
                <td colspan="4" class="text-center text-muted py-4">
                  <i class="bi bi-check-circle text-success fs-1 d-block mb-2"></i>
                  No errors detected. System is running smoothly!
                </td>
              </tr>
              <tr v-for="error in errors" :key="error.id">
                <td>
                  <small class="text-muted">{{ formatTime(error.time) }}</small>
                </td>
                <td>
                  <span class="badge bg-danger">{{ error.type }}</span>
                </td>
                <td>{{ error.message }}</td>
                <td>
                  <button class="btn btn-sm btn-outline-primary">
                    <i class="bi bi-info-circle me-1"></i>Details
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useToast } from 'vue-toastification'
import api from '@/services/api'

const toast = useToast()
const loading = ref(true)

const health = ref({
  cpu: 0,
  memory: 0,
  disk: 0,
  uptime: 0
})

const services = ref<any[]>([])

const loadSystemHealth = async () => {
  try {
    loading.value = true
    const response = await api.get('/system-health')
    health.value = {
      cpu: response.data.cpu,
      memory: response.data.memory,
      disk: response.data.disk,
      uptime: response.data.uptime
    }
    services.value = response.data.services
  } catch (error: any) {
    console.error('Failed to load system health:', error)
    toast.error('Failed to load system health')
  } finally {
    loading.value = false
  }
}

interface ErrorLog {
  id: number
  time: Date
  type: string
  message: string
}

const errors = ref<ErrorLog[]>([])

const getCpuClass = (value: number) => {
  if (value < 50) return 'bg-success'
  if (value < 75) return 'bg-warning'
  return 'bg-danger'
}

const formatTime = (date: Date) => {
  return new Intl.DateTimeFormat('en-US', {
    hour: '2-digit',
    minute: '2-digit'
  }).format(new Date(date))
}

const refreshHealth = async () => {
  toast.info('Refreshing system health...')
  await loadSystemHealth()
  toast.success('Health data refreshed')
}

onMounted(() => {
  loadSystemHealth()
})
</script>

<style scoped>
.card {
  border: none;
}

.card-header {
  border-bottom: 2px solid var(--bs-border-color, #f0f0f0);
}

.progress {
  border-radius: 10px;
}

.progress-bar {
  border-radius: 10px;
}

.table th {
  font-weight: 600;
  font-size: 0.875rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}
</style>
