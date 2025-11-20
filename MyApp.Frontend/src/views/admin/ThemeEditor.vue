<template>
  <div class="theme-editor">
    <!-- Unsaved Changes Warning -->
    <div v-if="themeStore.hasUnsavedChanges" class="alert alert-warning alert-dismissible fade show mb-4" role="alert">
      <i class="bi bi-exclamation-triangle-fill me-2"></i>
      <strong>Unsaved Changes</strong> - You have unsaved theme changes. Click "Save Theme" to apply them globally.
      <button type="button" class="btn-close" @click="themeStore.hasUnsavedChanges = false"></button>
    </div>

    <div class="row g-4">
      <!-- Left Panel: Theme Editor -->
      <div class="col-lg-7">
        <!-- Color Palette Card -->
        <div class="card shadow-sm mb-4">
          <div class="card-header bg-white border-bottom">
            <h5 class="mb-0">
              <i class="bi bi-palette-fill text-primary me-2"></i>
              Color Palette
            </h5>
          </div>
          <div class="card-body">
            <div class="row g-3">
              <!-- Primary Color -->
              <div class="col-md-6">
                <label class="form-label fw-semibold">Primary Color</label>
                <div class="input-group">
                  <input 
                    type="color" 
                    class="form-control form-control-color" 
                    v-model="localTheme.primaryColor"
                    @input="onThemeChange"
                    title="Choose primary color"
                  >
                  <input 
                    type="text" 
                    class="form-control" 
                    v-model="localTheme.primaryColor"
                    @input="onThemeChange"
                    placeholder="#f97316"
                  >
                </div>
              </div>

              <!-- Secondary Color -->
              <div class="col-md-6">
                <label class="form-label fw-semibold">Secondary Color</label>
                <div class="input-group">
                  <input 
                    type="color" 
                    class="form-control form-control-color" 
                    v-model="localTheme.secondaryColor"
                    @input="onThemeChange"
                  >
                  <input 
                    type="text" 
                    class="form-control" 
                    v-model="localTheme.secondaryColor"
                    @input="onThemeChange"
                  >
                </div>
              </div>

              <!-- Success Color -->
              <div class="col-md-6">
                <label class="form-label fw-semibold">Success Color</label>
                <div class="input-group">
                  <input 
                    type="color" 
                    class="form-control form-control-color" 
                    v-model="localTheme.successColor"
                    @input="onThemeChange"
                  >
                  <input 
                    type="text" 
                    class="form-control" 
                    v-model="localTheme.successColor"
                    @input="onThemeChange"
                  >
                </div>
              </div>

              <!-- Danger Color -->
              <div class="col-md-6">
                <label class="form-label fw-semibold">Danger Color</label>
                <div class="input-group">
                  <input 
                    type="color" 
                    class="form-control form-control-color" 
                    v-model="localTheme.dangerColor"
                    @input="onThemeChange"
                  >
                  <input 
                    type="text" 
                    class="form-control" 
                    v-model="localTheme.dangerColor"
                    @input="onThemeChange"
                  >
                </div>
              </div>

              <!-- Warning Color -->
              <div class="col-md-6">
                <label class="form-label fw-semibold">Warning Color</label>
                <div class="input-group">
                  <input 
                    type="color" 
                    class="form-control form-control-color" 
                    v-model="localTheme.warningColor"
                    @input="onThemeChange"
                  >
                  <input 
                    type="text" 
                    class="form-control" 
                    v-model="localTheme.warningColor"
                    @input="onThemeChange"
                  >
                </div>
              </div>

              <!-- Info Color -->
              <div class="col-md-6">
                <label class="form-label fw-semibold">Info Color</label>
                <div class="input-group">
                  <input 
                    type="color" 
                    class="form-control form-control-color" 
                    v-model="localTheme.infoColor"
                    @input="onThemeChange"
                  >
                  <input 
                    type="text" 
                    class="form-control" 
                    v-model="localTheme.infoColor"
                    @input="onThemeChange"
                  >
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Typography & Layout Card -->
        <div class="card shadow-sm mb-4">
          <div class="card-header bg-white border-bottom">
            <h5 class="mb-0">
              <i class="bi bi-type text-primary me-2"></i>
              Typography & Layout
            </h5>
          </div>
          <div class="card-body">
            <div class="row g-3">
              <!-- Font Family -->
              <div class="col-md-8">
                <label class="form-label fw-semibold">Font Family</label>
                <select class="form-select" v-model="localTheme.fontFamily" @change="onThemeChange">
                  <option value="Inter, system-ui, -apple-system, sans-serif">Inter</option>
                  <option value="Poppins, sans-serif">Poppins</option>
                  <option value="Roboto, sans-serif">Roboto</option>
                  <option value="'Open Sans', sans-serif">Open Sans</option>
                  <option value="Montserrat, sans-serif">Montserrat</option>
                  <option value="-apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif">System Default</option>
                </select>
              </div>

              <!-- Border Radius -->
              <div class="col-md-4">
                <label class="form-label fw-semibold">
                  Border Radius 
                  <span class="text-muted">({{ localTheme.borderRadius }})</span>
                </label>
                <input 
                  type="range" 
                  class="form-range" 
                  min="0" 
                  max="24" 
                  :value="parseInt(localTheme.borderRadius)"
                  @input="(e) => { const target = e.target as HTMLInputElement; localTheme.borderRadius = target.value + 'px'; onThemeChange() }"
                >
              </div>
            </div>
          </div>
        </div>

        <!-- Theme Presets Card -->
        <div class="card shadow-sm mb-4">
          <div class="card-header bg-white border-bottom">
            <h5 class="mb-0">
              <i class="bi bi-stars text-primary me-2"></i>
              Quick Presets
            </h5>
          </div>
          <div class="card-body">
            <div class="row g-3">
              <div class="col-md-4" v-for="preset in presets" :key="preset.id">
                <button 
                  class="btn w-100 text-start preset-btn"
                  :class="{ 'active': currentPreset === preset.id }"
                  @click="applyPreset(preset.id)"
                >
                  <div class="d-flex align-items-center">
                    <div 
                      class="preset-color me-2" 
                      :style="{ background: preset.gradient }"
                    ></div>
                    <div>
                      <div class="fw-semibold">{{ preset.name }}</div>
                      <small class="text-muted">{{ currentPreset === preset.id ? 'Current theme' : preset.description }}</small>
                    </div>
                  </div>
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Advanced Options Card -->
        <div class="card shadow-sm mb-4">
          <div class="card-header bg-white border-bottom">
            <h5 class="mb-0">
              <i class="bi bi-sliders text-primary me-2"></i>
              Advanced Options
            </h5>
          </div>
          <div class="card-body">
            <div class="row g-3">
              <div class="col-md-6">
                <div class="form-check form-switch">
                  <input 
                    class="form-check-input" 
                    type="checkbox" 
                    id="darkModeDefault"
                    v-model="localTheme.darkModeDefault"
                    @change="onThemeChange"
                  >
                  <label class="form-check-label" for="darkModeDefault">
                    <i class="bi bi-moon-stars me-1"></i>
                    Dark Mode by Default
                  </label>
                </div>
              </div>

              <div class="col-md-6">
                <div class="form-check form-switch">
                  <input 
                    class="form-check-input" 
                    type="checkbox" 
                    id="forceDarkMode"
                    v-model="localTheme.forceDarkMode"
                    @change="onThemeChange"
                  >
                  <label class="form-check-label" for="forceDarkMode">
                    <i class="bi bi-lock-fill me-1"></i>
                    Force Dark Mode for All
                  </label>
                </div>
              </div>

              <div class="col-md-6">
                <div class="form-check form-switch">
                  <input 
                    class="form-check-input" 
                    type="checkbox" 
                    id="useGradientBg"
                    v-model="localTheme.useGradientBg"
                    @change="onThemeChange"
                  >
                  <label class="form-check-label" for="useGradientBg">
                    <i class="bi bi-rainbow me-1"></i>
                    Gradient Backgrounds
                  </label>
                </div>
              </div>

              <div class="col-md-6">
                <div class="form-check form-switch">
                  <input 
                    class="form-check-input" 
                    type="checkbox" 
                    id="useGlassmorphism"
                    v-model="localTheme.useGlassmorphism"
                    @change="onThemeChange"
                  >
                  <label class="form-check-label" for="useGlassmorphism">
                    <i class="bi bi-droplet-half me-1"></i>
                    Glassmorphism Effect
                  </label>
                </div>
              </div>

              <div class="col-md-6">
                <div class="form-check form-switch">
                  <input 
                    class="form-check-input" 
                    type="checkbox" 
                    id="animatedBg"
                    v-model="localTheme.animatedBg"
                    @change="onThemeChange"
                  >
                  <label class="form-check-label" for="animatedBg">
                    <i class="bi bi-arrow-repeat me-1"></i>
                    Animated Backgrounds
                  </label>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Actions Card -->
        <div class="card shadow-sm">
          <div class="card-body">
            <div class="row g-2">
              <div class="col-md-3">
                <button 
                  class="btn btn-primary w-100"
                  @click="saveTheme"
                  :disabled="themeStore.loading || !themeStore.hasUnsavedChanges"
                >
                  <i class="bi bi-check-circle-fill me-1"></i>
                  Save Theme
                </button>
              </div>
              <div class="col-md-3">
                <button 
                  class="btn btn-outline-secondary w-100"
                  @click="resetTheme"
                  :disabled="themeStore.loading"
                >
                  <i class="bi bi-arrow-counterclockwise me-1"></i>
                  Reset
                </button>
              </div>
              <div class="col-md-3">
                <button 
                  class="btn btn-outline-primary w-100"
                  @click="exportTheme"
                >
                  <i class="bi bi-download me-1"></i>
                  Export
                </button>
              </div>
              <div class="col-md-3">
                <label class="btn btn-outline-primary w-100 mb-0">
                  <i class="bi bi-upload me-1"></i>
                  Import
                  <input 
                    type="file" 
                    class="d-none" 
                    accept=".json"
                    @change="importTheme"
                  >
                </label>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Right Panel: Live Preview -->
      <div class="col-lg-5">
        <div class="card shadow-sm sticky-top" style="top: 100px;">
          <div class="card-header bg-white border-bottom">
            <h5 class="mb-0">
              <i class="bi bi-eye-fill text-primary me-2"></i>
              Live Preview
            </h5>
          </div>
          <div class="card-body p-0">
            <div 
              class="preview-container" 
              :style="previewStyles"
              :class="{ 'glassmorphism': localTheme.useGlassmorphism, 'animated-bg': localTheme.animatedBg }"
            >
              <!-- Preview Header -->
              <div class="preview-header" :style="{ background: localTheme.primaryColor }">
                <div class="d-flex justify-content-between align-items-center px-4 py-3 text-white">
                  <h6 class="mb-0 fw-bold">Your Brand</h6>
                  <div class="d-flex gap-3">
                    <span>Home</span>
                    <span>About</span>
                    <span>Contact</span>
                  </div>
                </div>
              </div>

              <!-- Preview Hero -->
              <div class="preview-hero p-4" :style="heroStyles">
                <h2 class="fw-bold mb-2">Welcome to Your Site</h2>
                <p class="text-muted mb-3">Experience the new design with live theme updates</p>
                <button 
                  class="btn"
                  :style="{ 
                    background: localTheme.primaryColor, 
                    color: 'white',
                    borderRadius: localTheme.borderRadius,
                    border: 'none'
                  }"
                >
                  Get Started
                </button>
              </div>

              <!-- Preview Cards -->
              <div class="p-4">
                <div class="row g-3">
                  <div class="col-6">
                    <div 
                      class="preview-card p-3"
                      :style="cardStyles"
                    >
                      <div 
                        class="rounded-circle d-inline-flex align-items-center justify-content-center mb-2"
                        :style="{ 
                          width: '48px', 
                          height: '48px',
                          background: localTheme.successColor + '20',
                          color: localTheme.successColor
                        }"
                      >
                        <i class="bi bi-check-circle-fill fs-5"></i>
                      </div>
                      <h6 class="fw-semibold mb-1">Success</h6>
                      <small class="text-muted">Everything works!</small>
                    </div>
                  </div>

                  <div class="col-6">
                    <div 
                      class="preview-card p-3"
                      :style="cardStyles"
                    >
                      <div 
                        class="rounded-circle d-inline-flex align-items-center justify-content-center mb-2"
                        :style="{ 
                          width: '48px', 
                          height: '48px',
                          background: localTheme.infoColor + '20',
                          color: localTheme.infoColor
                        }"
                      >
                        <i class="bi bi-info-circle-fill fs-5"></i>
                      </div>
                      <h6 class="fw-semibold mb-1">Info</h6>
                      <small class="text-muted">Learn more here</small>
                    </div>
                  </div>

                  <div class="col-6">
                    <div 
                      class="preview-card p-3"
                      :style="cardStyles"
                    >
                      <div 
                        class="rounded-circle d-inline-flex align-items-center justify-content-center mb-2"
                        :style="{ 
                          width: '48px', 
                          height: '48px',
                          background: localTheme.warningColor + '20',
                          color: localTheme.warningColor
                        }"
                      >
                        <i class="bi bi-exclamation-triangle-fill fs-5"></i>
                      </div>
                      <h6 class="fw-semibold mb-1">Warning</h6>
                      <small class="text-muted">Be careful!</small>
                    </div>
                  </div>

                  <div class="col-6">
                    <div 
                      class="preview-card p-3"
                      :style="cardStyles"
                    >
                      <div 
                        class="rounded-circle d-inline-flex align-items-center justify-content-center mb-2"
                        :style="{ 
                          width: '48px', 
                          height: '48px',
                          background: localTheme.dangerColor + '20',
                          color: localTheme.dangerColor
                        }"
                      >
                        <i class="bi bi-x-circle-fill fs-5"></i>
                      </div>
                      <h6 class="fw-semibold mb-1">Danger</h6>
                      <small class="text-muted">Critical alert</small>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Preview Footer -->
              <div class="preview-footer text-center py-3" :style="{ background: localTheme.darkSurface, color: localTheme.darkText }">
                <small>© 2025 Your Brand. All rights reserved.</small>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useThemeStore } from '@/stores/theme'
import { useToast } from 'vue-toastification'

const themeStore = useThemeStore()
const toast = useToast()

const localTheme = ref({ ...themeStore.theme })
const currentPreset = ref('')

const presets = [
  {
    id: 'orange-fire',
    name: 'Orange Fire',
    description: 'Bold & energetic',
    gradient: 'linear-gradient(135deg, #f97316 0%, #fb923c 100%)'
  },
  {
    id: 'emerald-ocean',
    name: 'Emerald Ocean',
    description: 'Fresh & calm',
    gradient: 'linear-gradient(135deg, #10b981 0%, #06b6d4 100%)'
  },
  {
    id: 'purple-nebula',
    name: 'Purple Nebula',
    description: 'Modern & bold',
    gradient: 'linear-gradient(135deg, #a855f7 0%, #ec4899 100%)'
  },
  {
    id: 'midnight-blue',
    name: 'Midnight Blue',
    description: 'Professional',
    gradient: 'linear-gradient(135deg, #3b82f6 0%, #1e40af 100%)'
  },
  {
    id: 'sunset-coral',
    name: 'Sunset Coral',
    description: 'Warm & inviting',
    gradient: 'linear-gradient(135deg, #f43f5e 0%, #fb7185 100%)'
  },
  {
    id: 'forest-green',
    name: 'Forest Green',
    description: 'Natural & eco',
    gradient: 'linear-gradient(135deg, #16a34a 0%, #059669 100%)'
  }
]

const previewStyles = computed(() => ({
  fontFamily: localTheme.value.fontFamily,
  background: localTheme.value.useGradientBg 
    ? localTheme.value.accentGradient 
    : localTheme.value.lightBg
}))

const heroStyles = computed(() => ({
  background: localTheme.value.useGradientBg 
    ? `linear-gradient(135deg, ${localTheme.value.primaryColor}10 0%, ${localTheme.value.secondaryColor}10 100%)`
    : localTheme.value.lightSurface,
  borderRadius: localTheme.value.borderRadius
}))

const cardStyles = computed(() => ({
  background: localTheme.value.useGlassmorphism 
    ? 'rgba(255, 255, 255, 0.7)'
    : localTheme.value.lightSurface,
  borderRadius: localTheme.value.borderRadius,
  backdropFilter: localTheme.value.useGlassmorphism ? 'blur(10px)' : 'none',
  border: localTheme.value.useGlassmorphism ? '1px solid rgba(255,255,255,0.2)' : '1px solid #e5e7eb',
  transition: 'transform 0.2s'
}))

function onThemeChange() {
  themeStore.updateTheme(localTheme.value)
  themeStore.applyThemeToDom(localTheme.value)
}

function applyPreset(presetId: string) {
  currentPreset.value = presetId
  themeStore.applyPreset(presetId)
  localTheme.value = { ...themeStore.theme }
  themeStore.applyThemeToDom(localTheme.value)
  // Don't show success toast - it confuses users into thinking it saved
  // toast.success(`${presets.find(p => p.id === presetId)?.name} preset applied!`)
}

async function saveTheme() {
  try {
    const success = await themeStore.saveTheme(localTheme.value)
    if (success) {
      toast.success('Theme saved successfully! ✨')
    } else {
      toast.error('Failed to save theme. Check browser console for details.')
    }
  } catch (error) {
    console.error('Save theme error:', error)
    toast.error('Failed to save theme. Check browser console for details.')
  }
}

async function resetTheme() {
  if (confirm('Are you sure you want to reset to the default theme? This cannot be undone.')) {
    const success = await themeStore.resetTheme()
    if (success) {
      localTheme.value = { ...themeStore.theme }
      currentPreset.value = 'orange-fire'
      toast.success('Theme reset to default')
    } else {
      toast.error('Failed to reset theme')
    }
  }
}

function exportTheme() {
  const json = themeStore.exportTheme()
  const blob = new Blob([json], { type: 'application/json' })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = `theme-${Date.now()}.json`
  a.click()
  URL.revokeObjectURL(url)
  toast.success('Theme exported successfully')
}

function importTheme(event: Event) {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]
  if (!file) return

  const reader = new FileReader()
  reader.onload = (e) => {
    const json = e.target?.result as string
    const success = themeStore.importTheme(json)
    if (success) {
      localTheme.value = { ...themeStore.theme }
      themeStore.applyThemeToDom(localTheme.value)
      toast.success('Theme imported successfully')
    } else {
      toast.error('Invalid theme file')
    }
  }
  reader.readAsText(file)
  target.value = '' // Reset input
}

// Detect which preset matches the current theme
const detectCurrentPreset = () => {
  const theme = localTheme.value
  
  // Define preset signatures (primary color is the main identifier)
  const presetSignatures: Record<string, string> = {
    'orange-fire': '#f97316',
    'emerald-ocean': '#10b981',
    'purple-nebula': '#a855f7',
    'midnight-blue': '#3b82f6',
    'sunset-coral': '#f43f5e',
    'forest-green': '#16a34a'
  }
  
  // Check if current theme matches any preset
  for (const [presetId, primaryColor] of Object.entries(presetSignatures)) {
    if (theme.primaryColor.toLowerCase() === primaryColor.toLowerCase()) {
      currentPreset.value = presetId
      return
    }
  }
  
  // If no match, it's a custom theme
  currentPreset.value = ''
}

// Watch for external theme changes
watch(() => themeStore.theme, (newTheme) => {
  localTheme.value = { ...newTheme }
  detectCurrentPreset()
}, { deep: true })

onMounted(async () => {
  await themeStore.loadTheme()
  localTheme.value = { ...themeStore.theme }
  detectCurrentPreset()
})
</script>

<style scoped>
.theme-editor {
  animation: fadeIn 0.3s ease-in;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.preset-btn {
  border: 2px solid var(--bs-border-color, #e5e7eb);
  background: white;
  transition: all 0.2s;
}

.preset-btn:hover {
  border-color: var(--bs-primary);
  transform: translateY(-2px);
  box-shadow: 0 4px 12px color-mix(in srgb, var(--bs-primary) 15%, transparent);
}

.preset-btn.active {
  border-color: var(--bs-primary);
  background: color-mix(in srgb, var(--bs-primary) 5%, white);
}

.preset-color {
  width: 40px;
  height: 40px;
  border-radius: 8px;
  flex-shrink: 0;
}

.preview-container {
  min-height: 600px;
  transition: all 0.3s ease;
  overflow: hidden;
}

.preview-header {
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}

.preview-hero {
  transition: all 0.3s ease;
}

.preview-card {
  transition: all 0.3s ease;
}

.preview-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 16px rgba(0,0,0,0.1);
}

.glassmorphism .preview-card {
  backdrop-filter: blur(10px);
  -webkit-backdrop-filter: blur(10px);
}

.animated-bg {
  animation: gradientShift 8s ease infinite;
  background-size: 200% 200%;
}

@keyframes gradientShift {
  0% {
    background-position: 0% 50%;
  }
  50% {
    background-position: 100% 50%;
  }
  100% {
    background-position: 0% 50%;
  }
}

.form-control-color {
  width: 60px;
  height: 40px;
  padding: 4px;
}

.sticky-top {
  position: sticky;
  z-index: 1020;
}
</style>
