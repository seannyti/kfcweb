<template>
  <div class="general-settings">
    <div class="row mb-4">
      <div class="col">
        <h2 class="mb-0">
          <i class="bi bi-gear-fill text-warning me-2"></i>
          General Settings
        </h2>
        <p class="text-muted mb-0">Configure basic site information and preferences</p>
      </div>
    </div>

    <!-- Site Information Card -->
    <div class="card shadow-sm mb-4">
      <div class="card-header bg-white py-3">
        <h5 class="mb-0">
          <i class="bi bi-info-circle me-2"></i>
          Site Information
        </h5>
      </div>
      <div class="card-body">
        <div class="row g-3">
          <div class="col-md-6">
            <label for="siteName" class="form-label">Site Name</label>
            <input 
              type="text" 
              class="form-control" 
              id="siteName"
              v-model="settings.siteName"
              placeholder="Knudson Family Construction"
            >
          </div>

          <div class="col-md-6">
            <label for="tagline" class="form-label">Tagline</label>
            <input 
              type="text" 
              class="form-control" 
              id="tagline"
              v-model="settings.tagline"
              placeholder="Building Excellence Since 1995"
            >
          </div>

          <div class="col-12">
            <label for="description" class="form-label">Site Description</label>
            <textarea 
              class="form-control" 
              id="description"
              v-model="settings.description"
              rows="3"
              placeholder="Brief description of your business..."
            ></textarea>
          </div>
        </div>
      </div>
    </div>

    <!-- Branding Card -->
    <div class="card shadow-sm mb-4">
      <div class="card-header bg-white py-3">
        <h5 class="mb-0">
          <i class="bi bi-palette me-2"></i>
          Branding
        </h5>
      </div>
      <div class="card-body">
        <div class="row g-4">
          <div class="col-md-6">
            <label class="form-label">Logo</label>
            <div class="border rounded p-3 text-center">
              <div v-if="settings.logo" class="mb-3">
                <img :src="settings.logo" alt="Logo" class="img-fluid" style="max-height: 100px;">
                <div class="mt-2">
                  <button type="button" class="btn btn-sm btn-outline-danger" @click="removeLogo">
                    <i class="bi bi-trash me-1"></i>Remove
                  </button>
                </div>
              </div>
              <div v-else>
                <i class="bi bi-image text-muted fs-1 d-block mb-2"></i>
                <input type="file" class="form-control" ref="logoInput" @change="uploadLogo" accept="image/*">
                <small class="text-muted d-block mt-2">PNG or JPG, max 2MB</small>
              </div>
            </div>
          </div>

          <div class="col-md-6">
            <label class="form-label">Favicon</label>
            <div class="border rounded p-3 text-center">
              <div v-if="settings.favicon" class="mb-3">
                <img :src="settings.favicon" alt="Favicon" style="width: 32px; height: 32px;">
                <div class="mt-2">
                  <button type="button" class="btn btn-sm btn-outline-danger" @click="removeFavicon">
                    <i class="bi bi-trash me-1"></i>Remove
                  </button>
                </div>
              </div>
              <div v-else>
                <i class="bi bi-image text-muted fs-1 d-block mb-2"></i>
                <input type="file" class="form-control" ref="faviconInput" @change="uploadFavicon" accept="image/*">
                <small class="text-muted d-block mt-2">ICO or PNG, 32x32px</small>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Regional Settings Card -->
    <div class="card shadow-sm mb-4">
      <div class="card-header bg-white py-3">
        <h5 class="mb-0">
          <i class="bi bi-globe me-2"></i>
          Regional Settings
        </h5>
      </div>
      <div class="card-body">
        <div class="row g-3">
          <div class="col-md-6">
            <label for="timezone" class="form-label">Timezone</label>
            <select class="form-select" id="timezone" v-model="settings.timezone">
              <option value="America/New_York">Eastern Time (ET)</option>
              <option value="America/Chicago">Central Time (CT)</option>
              <option value="America/Denver">Mountain Time (MT)</option>
              <option value="America/Los_Angeles">Pacific Time (PT)</option>
            </select>
          </div>

          <div class="col-md-6">
            <label for="dateFormat" class="form-label">Date Format</label>
            <select class="form-select" id="dateFormat" v-model="settings.dateFormat">
              <option value="MM/DD/YYYY">MM/DD/YYYY</option>
              <option value="DD/MM/YYYY">DD/MM/YYYY</option>
              <option value="YYYY-MM-DD">YYYY-MM-DD</option>
            </select>
          </div>
        </div>
      </div>
    </div>

    <!-- Security Options Card -->
    <div class="card shadow-sm mb-4">
      <div class="card-header bg-white py-3">
        <h5 class="mb-0">
          <i class="bi bi-shield-check me-2"></i>
          Security Options
        </h5>
      </div>
      <div class="card-body">
        <div class="form-check form-switch mb-3">
          <input 
            class="form-check-input" 
            type="checkbox" 
            role="switch" 
            id="allowRegistration"
            v-model="settings.allowRegistration"
          >
          <label class="form-check-label" for="allowRegistration">
            Allow User Registration
            <small class="text-muted d-block">Let new users create accounts</small>
          </label>
        </div>

        <div class="form-check form-switch">
          <input 
            class="form-check-input" 
            type="checkbox" 
            role="switch" 
            id="forceHttps"
            v-model="settings.forceHttps"
          >
          <label class="form-check-label" for="forceHttps">
            Force HTTPS
            <small class="text-muted d-block">Redirect all HTTP requests to HTTPS</small>
          </label>
        </div>
      </div>
    </div>

    <!-- Save Button -->
    <div class="text-end">
      <button 
        class="btn btn-primary btn-lg" 
        @click="saveSettings"
        :disabled="saving"
      >
        <i class="bi bi-save me-2"></i>
        {{ saving ? 'Saving...' : 'Save Settings' }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useToast } from 'vue-toastification'
import { useSettingsStore } from '@/stores/settings'
import api from '@/services/api'

const toast = useToast()
const settingsStore = useSettingsStore()

const settings = ref({
  siteName: 'Knudson Family Construction',
  tagline: 'Building Excellence Together',
  description: 'Professional construction services for residential and commercial projects',
  logo: null as string | null,
  favicon: null as string | null,
  timezone: 'America/New_York',
  dateFormat: 'MM/DD/YYYY',
  allowRegistration: true,
  forceHttps: true
})

const saving = ref(false)
const loading = ref(true)

const loadSettings = async () => {
  try {
    loading.value = true
    const response = await api.get('/settings/general')
    
    if (response.data) {
      settings.value = response.data
    }
  } catch (error: any) {
    toast.error('Failed to load settings')
  } finally {
    loading.value = false
  }
}

const uploadLogo = (event: Event) => {
  const file = (event.target as HTMLInputElement).files?.[0]
  if (file) {
    // Check file size (2MB = 2 * 1024 * 1024 bytes)
    if (file.size > 2 * 1024 * 1024) {
      toast.error('Logo file size must be less than 2MB')
      return
    }
    
    const reader = new FileReader()
    reader.onload = (e) => {
      settings.value.logo = e.target?.result as string
      toast.success('Logo uploaded successfully. Click Save to apply changes.')
    }
    reader.onerror = () => {
      toast.error('Failed to read logo file')
    }
    reader.readAsDataURL(file)
  }
}

const uploadFavicon = (event: Event) => {
  const file = (event.target as HTMLInputElement).files?.[0]
  if (file) {
    // Check file size (2MB limit)
    if (file.size > 2 * 1024 * 1024) {
      toast.error('Favicon file size must be less than 2MB')
      return
    }
    
    const reader = new FileReader()
    reader.onload = (e) => {
      settings.value.favicon = e.target?.result as string
      toast.success('Favicon uploaded successfully. Click Save to apply changes.')
    }
    reader.onerror = () => {
      toast.error('Failed to read favicon file')
    }
    reader.readAsDataURL(file)
  }
}

const removeLogo = () => {
  settings.value.logo = null
  toast.info('Logo removed. Click Save to apply changes.')
}

const removeFavicon = () => {
  settings.value.favicon = null
  toast.info('Favicon removed. Click Save to apply changes.')
}

const saveSettings = async () => {
  if (saving.value) return
  
  saving.value = true
  try {
    const response = await api.put('/settings/general', settings.value)
    
    // Update local state with server response to ensure sync
    if (response.data) {
      settings.value = response.data
    }
    
    // Reload settings store so header/footer update immediately
    await settingsStore.loadSettings()
    
    toast.success('Settings saved successfully!')
  } catch (error: any) {
    toast.error(error.response?.data?.message || 'Failed to save settings')
  } finally {
    saving.value = false
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
</style>
