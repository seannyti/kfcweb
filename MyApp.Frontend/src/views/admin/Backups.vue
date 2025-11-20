<template>
  <div class="backups">
    <div class="row mb-4">
      <div class="col">
        <h2 class="mb-0">
          <i class="bi bi-cloud-arrow-up-fill text-warning me-2"></i>
          Backups
        </h2>
        <p class="text-muted mb-0">Manage database and file backups</p>
      </div>
      <div class="col-auto">
        <button class="btn btn-primary" @click="createBackup" :disabled="creating">
          <i class="bi bi-plus-circle me-2"></i>
          {{ creating ? 'Creating...' : 'Create Backup Now' }}
        </button>
      </div>
    </div>

    <!-- Backup Settings -->
    <div class="card shadow-sm mb-4">
      <div class="card-header bg-white py-3">
        <h5 class="mb-0">
          <i class="bi bi-gear me-2"></i>
          Backup Settings
        </h5>
      </div>
      <div class="card-body">
        <div class="row g-3 align-items-end">
          <div class="col-md-3">
            <div class="form-check form-switch">
              <input 
                class="form-check-input" 
                type="checkbox" 
                role="switch" 
                id="autoBackup"
                v-model="settings.autoBackupEnabled"
                @change="saveSettings"
              >
              <label class="form-check-label" for="autoBackup">
                Enable Automatic Backups
              </label>
            </div>
          </div>
          <div class="col-md-3" v-if="settings.autoBackupEnabled">
            <label class="form-label small text-muted">Frequency</label>
            <select class="form-select" v-model="settings.frequency" @change="saveSettings">
              <option value="daily">Daily</option>
              <option value="weekly">Weekly (Sundays)</option>
              <option value="monthly">Monthly (1st)</option>
            </select>
          </div>
          <div class="col-md-3" v-if="settings.autoBackupEnabled">
            <label class="form-label small text-muted">Time</label>
            <input 
              type="time" 
              class="form-control" 
              v-model="settings.scheduledTime"
              @change="saveSettings"
            >
          </div>
          <div class="col-md-3" v-if="settings.autoBackupEnabled && settings.lastBackupDate">
            <label class="form-label small text-muted">Last Backup</label>
            <div class="text-muted small">
              {{ formatDate(settings.lastBackupDate) }}
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Backup List -->
    <div class="card shadow-sm">
      <div class="card-header bg-white py-3">
        <h5 class="mb-0">
          <i class="bi bi-clock-history me-2"></i>
          Backup History
        </h5>
      </div>
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr>
                <th>Backup Name</th>
                <th>Size</th>
                <th>Created</th>
                <th>Type</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="backup in backups" :key="backup.id">
                <td>
                  <i class="bi bi-file-earmark-zip me-2 text-primary"></i>
                  {{ backup.name }}
                </td>
                <td>{{ formatSize(backup.sizeInBytes) }}</td>
                <td>
                  <small class="text-muted">{{ formatDate(backup.createdAt) }}</small>
                </td>
                <td>
                  <span class="badge bg-info">{{ backup.type }}</span>
                </td>
                <td>
                  <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" @click="downloadBackup(backup)" title="Download">
                      <i class="bi bi-download"></i>
                    </button>
                    <button class="btn btn-outline-success" @click="restoreBackup(backup)" title="Restore">
                      <i class="bi bi-arrow-counterclockwise"></i>
                    </button>
                    <button class="btn btn-outline-danger" @click="deleteBackup(backup)" title="Delete">
                      <i class="bi bi-trash"></i>
                    </button>
                  </div>
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

const creating = ref(false)
const loading = ref(true)

const settings = ref({
  autoBackupEnabled: false,
  frequency: 'daily',
  scheduledTime: '02:00',
  lastBackupDate: null as Date | null
})

const backups = ref<any[]>([])

const loadSettings = async () => {
  try {
    const response = await api.get('/backup-settings')
    settings.value = response.data
  } catch (error: any) {
    console.error('Failed to load backup settings:', error)
  }
}

const saveSettings = async () => {
  try {
    await api.post('/backup-settings', settings.value)
    toast.success('Backup settings saved successfully')
    await loadSettings()
  } catch (error: any) {
    console.error('Failed to save backup settings:', error)
    toast.error('Failed to save backup settings')
  }
}

const loadBackups = async () => {
  try {
    loading.value = true
    const response = await api.get('/backups')
    backups.value = response.data
  } catch (error: any) {
    console.error('Failed to load backups:', error)
    toast.error('Failed to load backups')
  } finally {
    loading.value = false
  }
}

const formatSize = (bytes: number) => {
  if (bytes === 0) return '0 Bytes'
  const k = 1024
  const sizes = ['Bytes', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return Math.round((bytes / Math.pow(k, i)) * 100) / 100 + ' ' + sizes[i]
}

const createBackup = async () => {
  creating.value = true
  try {
    const response = await api.post('/backups', { type: 'manual' })
    toast.success('Backup created successfully!')
    await loadBackups()
  } catch (error: any) {
    console.error('Failed to create backup:', error)
    toast.error(error.response?.data?.message || 'Failed to create backup')
  } finally {
    creating.value = false
  }
}

const downloadBackup = async (backup: any) => {
  try {
    const response = await api.get(`/backups/${backup.id}/download`, { responseType: 'blob' })
    const url = window.URL.createObjectURL(new Blob([response.data]))
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', backup.fileName || backup.name)
    document.body.appendChild(link)
    link.click()
    link.remove()
    window.URL.revokeObjectURL(url)
    toast.success('Backup downloaded successfully')
  } catch (error) {
    console.error('Failed to download backup:', error)
    toast.error('Failed to download backup')
  }
}

const restoreBackup = (backup: any) => {
  if (confirm(`Are you sure you want to restore from ${backup.name}? This will overwrite current data.`)) {
    toast.warning('Restore functionality requires backend implementation')
  }
}

const deleteBackup = async (backup: any) => {
  if (confirm(`Delete ${backup.name}?`)) {
    try {
      await api.delete(`/backups/${backup.id}`)
      toast.success('Backup deleted successfully')
      await loadBackups()
    } catch (error) {
      console.error('Failed to delete backup:', error)
      toast.error('Failed to delete backup')
    }
  }
}

onMounted(() => {
  loadBackups()
  loadSettings()
})

const formatDate = (date: Date) => {
  return new Intl.DateTimeFormat('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(new Date(date))
}
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

.btn-primary {
  background-color: #f97316;
  border-color: #f97316;
}

.btn-primary:hover {
  background-color: #ea580c;
  border-color: #ea580c;
}

.form-check-input:checked {
  background-color: #f97316;
  border-color: #f97316;
}
</style>
