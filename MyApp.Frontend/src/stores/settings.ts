import { defineStore } from 'pinia'
import { ref } from 'vue'
import api from '@/services/api'

export interface GeneralSettings {
  siteName: string
  tagline: string
  description: string
  logo: string | null
  favicon: string | null
  timezone: string
  dateFormat: string
  allowRegistration: boolean
  forceHttps: boolean
}

export const useSettingsStore = defineStore('settings', () => {
  const settings = ref<GeneralSettings>({
    siteName: 'Knudson Family Construction',
    tagline: 'Building Excellence Together',
    description: 'Professional construction services',
    logo: null,
    favicon: null,
    timezone: 'America/New_York',
    dateFormat: 'MM/DD/YYYY',
    allowRegistration: true,
    forceHttps: false
  })

  const loading = ref(false)

  async function loadSettings() {
    try {
      loading.value = true
      const response = await api.get('/settings/general')
      if (response.data) {
        settings.value = response.data
      }
    } catch (error) {
      console.error('Failed to load settings:', error)
    } finally {
      loading.value = false
    }
  }

  return {
    settings,
    loading,
    loadSettings
  }
})
