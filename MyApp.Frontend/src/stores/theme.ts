import { defineStore } from 'pinia'
import { ref } from 'vue'
import api from '@/services/api'
import adminService, { type ThemeSettings } from '@/services/adminService'

// Helper function to convert hex to RGB
function hexToRgb(hex: string): string {
  const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex)
  if (result) {
    const r = parseInt(result[1], 16)
    const g = parseInt(result[2], 16)
    const b = parseInt(result[3], 16)
    return `${r}, ${g}, ${b}`
  }
  return '0, 0, 0'
}

export const useThemeStore = defineStore('theme', () => {
  const theme = ref<ThemeSettings>({
    primaryColor: '#f97316',
    secondaryColor: '#0d9488',
    successColor: '#22c55e',
    dangerColor: '#ef4444',
    warningColor: '#f59e0b',
    infoColor: '#3b82f6',
    darkBg: '#1a1a1a',
    darkSurface: '#2d2d2d',
    darkText: '#f5f5f5',
    lightBg: '#ffffff',
    lightSurface: '#f8f9fa',
    lightText: '#212529',
    borderRadius: '8px',
    fontFamily: 'Inter, system-ui, -apple-system, sans-serif',
    darkModeDefault: false,
    forceDarkMode: false,
    useGradientBg: false,
    useGlassmorphism: false,
    animatedBg: false,
    accentGradient: 'linear-gradient(135deg, #f97316 0%, #fb923c 100%)',
    customCss: ''
  })

  const loading = ref(false)
  const hasUnsavedChanges = ref(false)

  async function loadTheme() {
    loading.value = true
    try {
      // Use public endpoint so all users (logged in or not) can load theme
      const response = await api.get('/settings/theme')
      if (response.data) {
        theme.value = response.data
        applyThemeToDom(theme.value)
      }
      hasUnsavedChanges.value = false
    } catch (error) {
      console.error('Failed to load theme:', error)
      // Apply default theme if load fails
      applyThemeToDom(theme.value)
    } finally {
      loading.value = false
    }
  }

  async function saveTheme(themeData: ThemeSettings) {
    loading.value = true
    try {
      await adminService.saveTheme(themeData)
      theme.value = themeData
      applyThemeToDom(theme.value)
      hasUnsavedChanges.value = false
      return true
    } catch (error: any) {
      // Error will be logged by API interceptor
      return false
    } finally {
      loading.value = false
    }
  }

  async function resetTheme() {
    loading.value = true
    try {
      await adminService.resetTheme()
      await loadTheme()
      hasUnsavedChanges.value = false
      return true
    } catch (error) {
      console.error('Failed to reset theme:', error)
      return false
    } finally {
      loading.value = false
    }
  }

  function applyPreset(presetName: string) {
    const presets: Record<string, Partial<ThemeSettings>> = {
      'orange-fire': {
        primaryColor: '#f97316',
        secondaryColor: '#0d9488',
        successColor: '#22c55e',
        dangerColor: '#ef4444',
        warningColor: '#f59e0b',
        infoColor: '#3b82f6',
        accentGradient: 'linear-gradient(135deg, #f97316 0%, #fb923c 100%)'
      },
      'emerald-ocean': {
        primaryColor: '#10b981',
        secondaryColor: '#06b6d4',
        successColor: '#22c55e',
        dangerColor: '#ef4444',
        warningColor: '#f59e0b',
        infoColor: '#3b82f6',
        accentGradient: 'linear-gradient(135deg, #10b981 0%, #06b6d4 100%)'
      },
      'purple-nebula': {
        primaryColor: '#a855f7',
        secondaryColor: '#ec4899',
        successColor: '#22c55e',
        dangerColor: '#ef4444',
        warningColor: '#f59e0b',
        infoColor: '#3b82f6',
        accentGradient: 'linear-gradient(135deg, #a855f7 0%, #ec4899 100%)'
      },
      'midnight-blue': {
        primaryColor: '#3b82f6',
        secondaryColor: '#1e40af',
        successColor: '#22c55e',
        dangerColor: '#ef4444',
        warningColor: '#f59e0b',
        infoColor: '#3b82f6',
        accentGradient: 'linear-gradient(135deg, #3b82f6 0%, #1e40af 100%)'
      },
      'sunset-coral': {
        primaryColor: '#f43f5e',
        secondaryColor: '#fb7185',
        successColor: '#22c55e',
        dangerColor: '#ef4444',
        warningColor: '#f59e0b',
        infoColor: '#3b82f6',
        accentGradient: 'linear-gradient(135deg, #f43f5e 0%, #fb7185 100%)'
      },
      'forest-green': {
        primaryColor: '#16a34a',
        secondaryColor: '#059669',
        successColor: '#22c55e',
        dangerColor: '#ef4444',
        warningColor: '#f59e0b',
        infoColor: '#3b82f6',
        accentGradient: 'linear-gradient(135deg, #16a34a 0%, #059669 100%)'
      }
    }

    const preset = presets[presetName]
    if (preset) {
      theme.value = { ...theme.value, ...preset }
      hasUnsavedChanges.value = true
    }
  }

  function applyThemeToDom(themeData: ThemeSettings) {
    const root = document.documentElement
    
    // Bootstrap 5 color variables
    root.style.setProperty('--bs-primary', themeData.primaryColor)
    root.style.setProperty('--bs-primary-rgb', hexToRgb(themeData.primaryColor))
    root.style.setProperty('--bs-secondary', themeData.secondaryColor)
    root.style.setProperty('--bs-secondary-rgb', hexToRgb(themeData.secondaryColor))
    root.style.setProperty('--bs-success', themeData.successColor)
    root.style.setProperty('--bs-success-rgb', hexToRgb(themeData.successColor))
    root.style.setProperty('--bs-danger', themeData.dangerColor)
    root.style.setProperty('--bs-danger-rgb', hexToRgb(themeData.dangerColor))
    root.style.setProperty('--bs-warning', themeData.warningColor)
    root.style.setProperty('--bs-warning-rgb', hexToRgb(themeData.warningColor))
    root.style.setProperty('--bs-info', themeData.infoColor)
    root.style.setProperty('--bs-info-rgb', hexToRgb(themeData.infoColor))
    
    // Dark/Light mode colors
    root.style.setProperty('--theme-dark-bg', themeData.darkBg)
    root.style.setProperty('--theme-dark-surface', themeData.darkSurface)
    root.style.setProperty('--theme-dark-text', themeData.darkText)
    root.style.setProperty('--theme-light-bg', themeData.lightBg)
    root.style.setProperty('--theme-light-surface', themeData.lightSurface)
    root.style.setProperty('--theme-light-text', themeData.lightText)
    
    // Typography and borders
    root.style.setProperty('--bs-border-radius', themeData.borderRadius)
    root.style.setProperty('--bs-body-font-family', themeData.fontFamily)
    root.style.setProperty('--theme-gradient', themeData.accentGradient)

    // Apply dark mode based on settings
    if (themeData.forceDarkMode) {
      // Force dark mode - override everything
      document.documentElement.setAttribute('data-bs-theme', 'dark')
      document.body.classList.add('dark-mode')
      localStorage.setItem('forcedDarkMode', 'true')
    } else if (themeData.darkModeDefault) {
      // Dark mode by default, but user can change it
      const userPreference = localStorage.getItem('darkMode')
      if (userPreference === null) {
        // No user preference set, use default
        document.documentElement.setAttribute('data-bs-theme', 'dark')
        document.body.classList.add('dark-mode')
      } else {
        // Respect user preference
        const isDark = userPreference === 'true'
        document.documentElement.setAttribute('data-bs-theme', isDark ? 'dark' : 'light')
        document.body.classList.toggle('dark-mode', isDark)
      }
      localStorage.removeItem('forcedDarkMode')
    } else {
      // Light mode by default
      const userPreference = localStorage.getItem('darkMode')
      const isDark = userPreference === 'true'
      document.documentElement.setAttribute('data-bs-theme', isDark ? 'dark' : 'light')
      document.body.classList.toggle('dark-mode', isDark)
      localStorage.removeItem('forcedDarkMode')
    }

    // Apply gradient backgrounds
    if (themeData.useGradientBg) {
      document.body.classList.add('gradient-bg')
      root.style.setProperty('--body-gradient', themeData.accentGradient)
    } else {
      document.body.classList.remove('gradient-bg')
    }

    // Apply glassmorphism
    if (themeData.useGlassmorphism) {
      document.body.classList.add('glassmorphism-enabled')
    } else {
      document.body.classList.remove('glassmorphism-enabled')
    }

    // Apply animated background
    if (themeData.animatedBg) {
      document.body.classList.add('animated-bg')
    } else {
      document.body.classList.remove('animated-bg')
    }

    // Apply custom CSS if provided
    let customStyleEl = document.getElementById('custom-theme-styles')
    if (themeData.customCss) {
      if (!customStyleEl) {
        customStyleEl = document.createElement('style')
        customStyleEl.id = 'custom-theme-styles'
        document.head.appendChild(customStyleEl)
      }
      customStyleEl.textContent = themeData.customCss
    } else if (customStyleEl) {
      customStyleEl.remove()
    }
  }

  function updateTheme(updates: Partial<ThemeSettings>) {
    theme.value = { ...theme.value, ...updates }
    hasUnsavedChanges.value = true
  }

  function exportTheme(): string {
    return JSON.stringify(theme.value, null, 2)
  }

  function importTheme(jsonString: string): boolean {
    try {
      const imported = JSON.parse(jsonString)
      theme.value = { ...theme.value, ...imported }
      hasUnsavedChanges.value = true
      return true
    } catch (error) {
      console.error('Invalid theme JSON:', error)
      return false
    }
  }

  return {
    theme,
    loading,
    hasUnsavedChanges,
    loadTheme,
    saveTheme,
    resetTheme,
    applyPreset,
    updateTheme,
    exportTheme,
    importTheme,
    applyThemeToDom
  }
})
