<template>
  <div class="api-keys">
    <div class="row mb-4">
      <div class="col">
        <h2 class="mb-0">
          <i class="bi bi-key-fill text-warning me-2"></i>
          API Keys
        </h2>
        <p class="text-muted mb-0">Manage API access keys and permissions</p>
      </div>
      <div class="col-auto">
        <button class="btn btn-primary" @click="showCreateModal = true">
          <i class="bi bi-plus-circle me-2"></i>Generate New Key
        </button>
      </div>
    </div>

    <!-- API Keys List -->
    <div class="row g-4">
      <div class="col-md-6" v-for="apiKey in apiKeys" :key="apiKey.id">
        <div class="card shadow-sm h-100">
          <div class="card-body">
            <div class="d-flex justify-content-between align-items-start mb-3">
              <div>
                <h5 class="mb-1">{{ apiKey.name }}</h5>
                <small class="text-muted">Created {{ formatDate(apiKey.createdAt) }}</small>
              </div>
              <span 
                class="badge"
                :class="apiKey.isActive ? 'bg-success' : 'bg-secondary'"
              >
                {{ apiKey.isActive ? 'Active' : 'Disabled' }}
              </span>
            </div>

            <div class="mb-3">
              <label class="form-label small text-muted">API Key</label>
              <div class="input-group input-group-sm">
                <input 
                  type="text" 
                  class="form-control font-monospace" 
                  :value="apiKey.key"
                  readonly
                >
                <button 
                  class="btn btn-outline-secondary" 
                  @click="copyToClipboard(apiKey.key)"
                  title="Copy"
                >
                  <i class="bi bi-clipboard"></i>
                </button>
              </div>
            </div>

            <div class="mb-3">
              <label class="form-label small text-muted">Permissions</label>
              <div class="d-flex flex-wrap gap-1">
                <span 
                  v-for="perm in apiKey.permissions" 
                  :key="perm"
                  class="badge bg-primary"
                >
                  {{ perm }}
                </span>
              </div>
            </div>

            <div class="d-flex justify-content-between align-items-center">
              <small class="text-muted">
                Last used: {{ apiKey.lastUsedAt ? formatDate(apiKey.lastUsedAt) : 'Never' }}
              </small>
              <div class="btn-group btn-group-sm">
                <button 
                  class="btn btn-outline-warning" 
                  @click="toggleApiKey(apiKey)"
                  :title="apiKey.isActive ? 'Disable' : 'Enable'"
                >
                  <i :class="apiKey.isActive ? 'bi bi-pause' : 'bi bi-play'"></i>
                </button>
                <button 
                  class="btn btn-outline-danger" 
                  @click="deleteApiKey(apiKey)"
                  title="Delete"
                >
                  <i class="bi bi-trash"></i>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div class="col-12" v-if="apiKeys.length === 0">
        <div class="card shadow-sm">
          <div class="card-body text-center py-5">
            <i class="bi bi-key text-muted" style="font-size: 4rem;"></i>
            <h5 class="mt-3 mb-2">No API Keys</h5>
            <p class="text-muted mb-4">Create your first API key to get started</p>
            <button class="btn btn-primary" @click="showCreateModal = true">
              <i class="bi bi-plus-circle me-2"></i>Generate API Key
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Create API Key Modal -->
    <div v-if="showCreateModal" class="modal fade show d-block" tabindex="-1" style="background-color: rgba(0,0,0,0.5);">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              {{ createdKey ? 'API Key Created' : 'Generate New API Key' }}
            </h5>
            <button type="button" class="btn-close" @click="closeCreateModal"></button>
          </div>
          
          <div v-if="!createdKey" class="modal-body">
            <div class="mb-3">
              <label class="form-label">Name</label>
              <input 
                type="text" 
                class="form-control" 
                v-model="newKeyData.name"
                placeholder="e.g., Production API, Mobile App"
              >
            </div>

            <div class="mb-3">
              <label class="form-label">Permissions</label>
              <div class="form-check">
                <input 
                  class="form-check-input" 
                  type="checkbox" 
                  id="perm-read"
                  :checked="newKeyData.permissions.includes('read')"
                  @change="togglePermission('read')"
                >
                <label class="form-check-label" for="perm-read">
                  Read - View data
                </label>
              </div>
              <div class="form-check">
                <input 
                  class="form-check-input" 
                  type="checkbox" 
                  id="perm-write"
                  :checked="newKeyData.permissions.includes('write')"
                  @change="togglePermission('write')"
                >
                <label class="form-check-label" for="perm-write">
                  Write - Create and update data
                </label>
              </div>
              <div class="form-check">
                <input 
                  class="form-check-input" 
                  type="checkbox" 
                  id="perm-delete"
                  :checked="newKeyData.permissions.includes('delete')"
                  @change="togglePermission('delete')"
                >
                <label class="form-check-label" for="perm-delete">
                  Delete - Remove data
                </label>
              </div>
            </div>
          </div>

          <div v-else class="modal-body">
            <div class="alert alert-warning">
              <i class="bi bi-exclamation-triangle me-2"></i>
              <strong>{{ createdKey.message }}</strong>
            </div>

            <div class="mb-3">
              <label class="form-label">Your API Key</label>
              <div class="input-group">
                <input 
                  type="text" 
                  class="form-control font-monospace" 
                  :value="createdKey.key"
                  readonly
                >
                <button 
                  class="btn btn-outline-secondary" 
                  @click="copyToClipboard(createdKey.key)"
                >
                  <i class="bi bi-clipboard"></i> Copy
                </button>
              </div>
            </div>

            <div class="alert alert-info mb-0">
              <small>
                <i class="bi bi-info-circle me-2"></i>
                Store this key securely. It will not be shown again.
              </small>
            </div>
          </div>

          <div class="modal-footer">
            <button 
              v-if="!createdKey"
              type="button" 
              class="btn btn-secondary" 
              @click="closeCreateModal"
            >
              Cancel
            </button>
            <button 
              v-if="!createdKey"
              type="button" 
              class="btn btn-primary" 
              @click="createApiKey"
            >
              Generate Key
            </button>
            <button 
              v-else
              type="button" 
              class="btn btn-primary" 
              @click="closeCreateModal"
            >
              Done
            </button>
          </div>
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

const showCreateModal = ref(false)
const loading = ref(true)
const newKeyData = ref({
  name: '',
  permissions: [] as string[]
})
const createdKey = ref<any>(null)
const apiKeys = ref<any[]>([])

const loadApiKeys = async () => {
  try {
    loading.value = true
    const response = await api.get('/api-keys')
    apiKeys.value = response.data
  } catch (error: any) {
    console.error('Failed to load API keys:', error)
    toast.error('Failed to load API keys')
  } finally {
    loading.value = false
  }
}

const createApiKey = async () => {
  if (!newKeyData.value.name) {
    toast.error('Please enter a name for the API key')
    return
  }

  try {
    const response = await api.post('/api-keys', newKeyData.value)
    createdKey.value = response.data
    toast.success('API key created successfully!')
    await loadApiKeys()
    // Don't close modal immediately - show the key first
  } catch (error: any) {
    console.error('Failed to create API key:', error)
    toast.error('Failed to create API key')
  }
}

const closeCreateModal = () => {
  showCreateModal.value = false
  createdKey.value = null
  newKeyData.value = { name: '', permissions: [] }
}

const togglePermission = (permission: string) => {
  const index = newKeyData.value.permissions.indexOf(permission)
  if (index > -1) {
    newKeyData.value.permissions.splice(index, 1)
  } else {
    newKeyData.value.permissions.push(permission)
  }
}

const formatDate = (date: Date) => {
  return new Intl.DateTimeFormat('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric'
  }).format(new Date(date))
}

const copyToClipboard = (text: string) => {
  navigator.clipboard.writeText(text)
  toast.success('API key copied to clipboard!')
}

const toggleApiKey = async (apiKey: any) => {
  try {
    await api.post(`/api-keys/${apiKey.id}/toggle`)
    toast.success(`API key ${apiKey.isActive ? 'disabled' : 'enabled'}`)
    await loadApiKeys()
  } catch (error) {
    console.error('Failed to toggle API key:', error)
    toast.error('Failed to update API key')
  }
}

const deleteApiKey = async (apiKey: any) => {
  if (confirm(`Delete API key "${apiKey.name}"? This cannot be undone.`)) {
    try {
      await api.delete(`/api-keys/${apiKey.id}`)
      toast.success('API key deleted successfully')
      await loadApiKeys()
    } catch (error) {
      console.error('Failed to delete API key:', error)
      toast.error('Failed to delete API key')
    }
  }
}

onMounted(() => {
  loadApiKeys()
})
</script>

<style scoped>
.card {
  border: none;
}

/* Primary button colors now inherit from theme */

.font-monospace {
  font-size: 0.875rem;
}
</style>
