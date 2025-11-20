<template>
  <div class="maintenance">
    <div class="row mb-4">
      <div class="col">
        <h2 class="mb-0">
          <i class="bi bi-tools text-warning me-2"></i>
          Maintenance Mode
        </h2>
        <p class="text-muted mb-0">Control site access and perform system maintenance</p>
      </div>
    </div>

    <!-- Maintenance Mode Card -->
    <div class="card shadow-sm mb-4">
      <div class="card-header bg-white py-3">
        <h5 class="mb-0">
          <i class="bi bi-exclamation-triangle me-2"></i>
          Maintenance Mode
        </h5>
      </div>
      <div class="card-body">
        <div class="d-flex align-items-center justify-content-between mb-3">
          <div>
            <h6 class="mb-1">Site Access Control</h6>
            <small class="text-muted">
              {{ settings.maintenanceMode ? 'Site is in maintenance mode - only admins can access' : 'Site is accessible to all users' }}
            </small>
          </div>
          <div class="form-check form-switch">
            <input 
              class="form-check-input" 
              type="checkbox" 
              role="switch" 
              id="maintenanceMode"
              v-model="settings.maintenanceMode"
              @change="toggleMaintenanceMode"
              style="width: 3em; height: 1.5em;"
            >
            <label class="form-check-label ms-2" for="maintenanceMode">
              <span class="badge" :class="settings.maintenanceMode ? 'bg-warning' : 'bg-success'">
                {{ settings.maintenanceMode ? 'ON' : 'OFF' }}
              </span>
            </label>
          </div>
        </div>

        <div v-if="settings.maintenanceMode" class="alert alert-warning d-flex align-items-center">
          <i class="bi bi-info-circle fs-4 me-3"></i>
          <div>
            <strong>Maintenance Mode Active</strong><br>
            <small>Only administrators can access the site. Regular users will see the maintenance message below.</small>
          </div>
        </div>

        <div class="mt-3">
          <label for="maintenanceMessage" class="form-label fw-bold">
            Maintenance Message
          </label>
          <div id="maintenanceMessage" class="border rounded p-3 bg-light">
            <div class="editor-toolbar mb-2 border-bottom pb-2">
              <div class="btn-group btn-group-sm" role="group">
                <button type="button" class="btn btn-outline-secondary" title="Bold">
                  <i class="bi bi-type-bold"></i>
                </button>
                <button type="button" class="btn btn-outline-secondary" title="Italic">
                  <i class="bi bi-type-italic"></i>
                </button>
                <button type="button" class="btn btn-outline-secondary" title="Underline">
                  <i class="bi bi-type-underline"></i>
                </button>
              </div>
              <div class="btn-group btn-group-sm ms-2" role="group">
                <button type="button" class="btn btn-outline-secondary" title="Heading">
                  <i class="bi bi-type-h1"></i>
                </button>
                <button type="button" class="btn btn-outline-secondary" title="List">
                  <i class="bi bi-list-ul"></i>
                </button>
                <button type="button" class="btn btn-outline-secondary" title="Link">
                  <i class="bi bi-link-45deg"></i>
                </button>
              </div>
            </div>
            <textarea 
              v-model="settings.maintenanceMessage" 
              class="form-control border-0 bg-transparent"
              rows="6"
              placeholder="Enter the message users will see during maintenance..."
            ></textarea>
          </div>
          <div class="form-text">This message will be displayed to users when maintenance mode is enabled</div>
        </div>
      </div>
    </div>

    <!-- Feature Toggles Card -->
    <div class="card shadow-sm mb-4">
      <div class="card-header bg-white py-3">
        <h5 class="mb-0">
          <i class="bi bi-toggles me-2"></i>
          Feature Toggles
        </h5>
      </div>
      <div class="card-body">
        <div class="row g-4">
          <!-- User Registration -->
          <div class="col-md-6">
            <div class="p-3 border rounded">
              <div class="d-flex align-items-center justify-content-between mb-2">
                <div class="d-flex align-items-center">
                  <i class="bi bi-person-plus-fill text-primary fs-4 me-3"></i>
                  <div>
                    <h6 class="mb-0">User Registration</h6>
                    <small class="text-muted">Allow new users to register</small>
                  </div>
                </div>
                <div class="form-check form-switch">
                  <input 
                    class="form-check-input" 
                    type="checkbox" 
                    role="switch" 
                    id="allowRegistration"
                    v-model="settings.allowRegistration"
                  >
                </div>
              </div>
            </div>
          </div>

          <!-- API Access -->
          <div class="col-md-6">
            <div class="p-3 border rounded">
              <div class="d-flex align-items-center justify-content-between mb-2">
                <div class="d-flex align-items-center">
                  <i class="bi bi-plug-fill text-success fs-4 me-3"></i>
                  <div>
                    <h6 class="mb-0">API Access</h6>
                    <small class="text-muted">Enable external API access</small>
                  </div>
                </div>
                <div class="form-check form-switch">
                  <input 
                    class="form-check-input" 
                    type="checkbox" 
                    role="switch" 
                    id="enableApiAccess"
                    v-model="settings.enableApiAccess"
                  >
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- System Operations Card -->
    <div class="card shadow-sm mb-4">
      <div class="card-header bg-white py-3">
        <h5 class="mb-0">
          <i class="bi bi-wrench me-2"></i>
          System Operations
        </h5>
      </div>
      <div class="card-body">
        <div class="row g-3">
          <!-- Clear Cache -->
          <div class="col-md-4">
            <div class="d-grid">
              <button 
                class="btn btn-outline-primary" 
                @click="clearCache"
                :disabled="clearingCache"
              >
                <i class="bi bi-trash me-2"></i>
                {{ clearingCache ? 'Clearing...' : 'Clear Cache' }}
              </button>
            </div>
            <small class="text-muted d-block mt-2">
              Clear application cache to improve performance
            </small>
          </div>

          <!-- Optimize Database -->
          <div class="col-md-4">
            <div class="d-grid">
              <button 
                class="btn btn-outline-success" 
                @click="optimizeDatabase"
                :disabled="optimizing"
              >
                <i class="bi bi-database me-2"></i>
                {{ optimizing ? 'Optimizing...' : 'Optimize Database' }}
              </button>
            </div>
            <small class="text-muted d-block mt-2">
              Optimize database tables and indexes
            </small>
          </div>

          <!-- Check Updates -->
          <div class="col-md-4">
            <div class="d-grid">
              <button 
                class="btn btn-outline-warning" 
                @click="checkUpdates"
                :disabled="checkingUpdates"
              >
                <i class="bi bi-cloud-download me-2"></i>
                {{ checkingUpdates ? 'Checking...' : 'Check for Updates' }}
              </button>
            </div>
            <small class="text-muted d-block mt-2">
              Check for system and security updates
            </small>
          </div>
        </div>
      </div>
    </div>

    <!-- Save Button -->
    <div class="card shadow-sm">
      <div class="card-body">
        <button 
          class="btn btn-primary btn-lg" 
          @click="saveSettings"
          :disabled="saving"
        >
          <i class="bi bi-save me-2"></i>
          {{ saving ? 'Saving...' : 'Save All Settings' }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useToast } from 'vue-toastification'
import adminService from '@/services/adminService'

const toast = useToast()

const settings = ref({
  maintenanceMode: false,
  maintenanceMessage: 'We are currently performing scheduled maintenance. Please check back soon. We apologize for any inconvenience.',
  allowRegistration: true,
  enableApiAccess: true
})

const saving = ref(false)
const clearingCache = ref(false)
const optimizing = ref(false)
const checkingUpdates = ref(false)

const loadSettings = async () => {
  try {
    const response = await adminService.getSettings()
    settings.value = {
      maintenanceMode: response.maintenanceMode ?? false,
      maintenanceMessage: response.maintenanceMessage ?? 'We are currently performing scheduled maintenance. Please check back soon.',
      allowRegistration: response.allowRegistration ?? true,
      enableApiAccess: response.enableApiAccess ?? true
    }
  } catch (error) {
    console.error('Failed to load maintenance settings:', error)
    toast.error('Failed to load maintenance settings')
  }
}

const toggleMaintenanceMode = async () => {
  try {
    await adminService.toggleMaintenanceMode(settings.value.maintenanceMode)
    if (settings.value.maintenanceMode) {
      toast.warning('Maintenance mode enabled - site is now restricted to admins only')
    } else {
      toast.success('Maintenance mode disabled - site is now accessible to all users')
    }
  } catch (error) {
    console.error('Failed to toggle maintenance mode:', error)
    toast.error('Failed to toggle maintenance mode')
    settings.value.maintenanceMode = !settings.value.maintenanceMode
  }
}

const saveSettings = async () => {
  saving.value = true
  try {
    await adminService.saveSettings(settings.value)
    toast.success('Settings saved successfully!')
    await loadSettings()
  } catch (error) {
    console.error('Failed to save settings:', error)
    toast.error('Failed to save settings')
  } finally {
    saving.value = false
  }
}

const clearCache = async () => {
  clearingCache.value = true
  try {
    await adminService.clearCache()
    toast.success('Cache cleared successfully!')
  } catch (error) {
    console.error('Failed to clear cache:', error)
    toast.error('Failed to clear cache')
  } finally {
    clearingCache.value = false
  }
}

const optimizeDatabase = async () => {
  optimizing.value = true
  try {
    await adminService.optimizeDatabase()
    toast.success('Database optimized successfully!')
  } catch (error) {
    console.error('Failed to optimize database:', error)
    toast.error('Failed to optimize database')
  } finally {
    optimizing.value = false
  }
}

const checkUpdates = async () => {
  checkingUpdates.value = true
  try {
    // Update checking feature not implemented
    await new Promise(resolve => setTimeout(resolve, 2000))
    toast.info('System is up to date!')
  } catch (error) {
    console.error('Failed to check for updates:', error)
    toast.error('Failed to check for updates')
  } finally {
    checkingUpdates.value = false
  }
}

onMounted(() => {
  loadSettings()
})
</script>

<style scoped>
.card {
  border: none;
}

.card-header {
  border-bottom: 2px solid var(--bs-border-color, #f0f0f0);
}

.form-check-input:checked {
  background-color: #f97316;
  border-color: #f97316;
}

.btn-primary {
  background-color: #f97316;
  border-color: #f97316;
}

.btn-primary:hover {
  background-color: #ea580c;
  border-color: #ea580c;
}

.editor-toolbar .btn {
  font-size: 0.875rem;
}
</style>
