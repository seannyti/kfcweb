import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authApi } from '@/services/api'
import type { User } from '@/types'

export const useAuthStore = defineStore('auth', () => {
  // State - tokens are now in HTTP-only cookies, we only track user data
  const user = ref<User | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Getters
  const isAuthenticated = computed(() => !!user.value)

  // Actions
  async function login(email: string, password: string): Promise<boolean> {
    try {
      loading.value = true
      error.value = null
      
      const response = await authApi.login({ email, password })
      
      // Store user data only (token is in HTTP-only cookie)
      user.value = response.data.user
      
      return true
    } catch (err: unknown) {
      const errorMessage = err instanceof Error ? err.message : 'Login failed'
      error.value = (err as any).response?.data?.message || errorMessage
      throw err
    } finally {
      loading.value = false
    }
  }

  async function register(email: string, password: string, name: string): Promise<boolean> {
    try {
      loading.value = true
      error.value = null
      
      const response = await authApi.register({ email, password, name })
      
      // Store user data only (token is in HTTP-only cookie)
      user.value = response.data.user
      
      return true
    } catch (err: unknown) {
      const errorMessage = err instanceof Error ? err.message : 'Registration failed'
      error.value = (err as any).response?.data?.message || errorMessage
      throw err
    } finally {
      loading.value = false
    }
  }

  async function fetchCurrentUser(): Promise<void> {
    try {
      loading.value = true
      const response = await authApi.getCurrentUser()
      user.value = response.data
    } catch (err: unknown) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to fetch user'
      error.value = (err as any).response?.data?.message || errorMessage
      // Don't auto-logout here, let the interceptor handle it
      throw err
    } finally {
      loading.value = false
    }
  }

  async function logout(): Promise<void> {
    try {
      // Call logout endpoint to clear HTTP-only cookie
      await authApi.logout()
    } catch (err) {
      // Continue even if API call fails
    } finally {
      // Clear state
      user.value = null
      error.value = null
    }
  }

  // Initialize user by fetching from API (validates cookie)
  async function initializeAuth(): Promise<void> {
    try {
      await fetchCurrentUser()
    } catch (e) {
      // Not authenticated or cookie expired
      user.value = null
    }
  }

  return {
    user,
    loading,
    error,
    isAuthenticated,
    login,
    register,
    fetchCurrentUser,
    logout,
    initializeAuth
  }
})
