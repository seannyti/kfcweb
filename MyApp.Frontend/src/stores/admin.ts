import { defineStore } from 'pinia'
import { ref } from 'vue'
import adminService, { type SiteSettings, type SystemHealth, type DashboardStats } from '@/services/adminService'

export const useAdminStore = defineStore('admin', () => {
  const settings = ref<SiteSettings | null>(null)
  const systemHealth = ref<SystemHealth | null>(null)
  const dashboardStats = ref<DashboardStats | null>(null)
  const loading = ref(false)

  // Load all settings
  const loadSettings = async () => {
    loading.value = true
    try {
      settings.value = await adminService.getSettings()
    } catch (error) {
      console.error('Failed to load settings:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  // Save settings
  const saveSettings = async (newSettings: Partial<SiteSettings>) => {
    try {
      await adminService.saveSettings(newSettings)
      if (settings.value) {
        settings.value = { ...settings.value, ...newSettings }
      }
    } catch (error) {
      console.error('Failed to save settings:', error)
      throw error
    }
  }

  // Load system health
  const loadSystemHealth = async () => {
    try {
      systemHealth.value = await adminService.getSystemHealth()
    } catch (error) {
      console.error('Failed to load system health:', error)
      throw error
    }
  }

  // Load dashboard stats
  const loadDashboardStats = async () => {
    try {
      dashboardStats.value = await adminService.getDashboardStats()
    } catch (error) {
      console.error('Failed to load dashboard stats:', error)
      throw error
    }
  }

  // Toggle maintenance mode
  const toggleMaintenanceMode = async (enabled: boolean) => {
    try {
      await adminService.toggleMaintenanceMode(enabled)
      if (settings.value) {
        settings.value.maintenanceMode = enabled
      }
    } catch (error) {
      console.error('Failed to toggle maintenance mode:', error)
      throw error
    }
  }

  return {
    settings,
    systemHealth,
    dashboardStats,
    loading,
    loadSettings,
    saveSettings,
    loadSystemHealth,
    loadDashboardStats,
    toggleMaintenanceMode
  }
})
