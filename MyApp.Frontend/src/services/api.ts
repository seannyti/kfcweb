import axios, { type AxiosInstance, type InternalAxiosRequestConfig } from 'axios'
import { useAuthStore } from '@/stores/auth'

// Create axios instance
const api: AxiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_URL || '/api',
  timeout: 10000,
  withCredentials: true, // Send cookies with requests
  headers: {
    'Content-Type': 'application/json'
  }
})

// Request interceptor (no longer needs to add Authorization header - using cookies)
api.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    // Token is now sent automatically via HTTP-only cookie
    // No need to manually add Authorization header
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Response interceptor for error handling
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    // Handle 401 Unauthorized errors
    if (error.response?.status === 401) {
      const url = error.config?.url || ''
      // Only auto-logout/redirect if user was authenticated (has user data in store)
      if (!url.includes('/auth/me') && !url.includes('/auth/login')) {
        const authStore = useAuthStore()
        // Only logout if we thought we were authenticated
        if (authStore.isAuthenticated) {
          authStore.logout()
          
          // Only redirect if not already on login page
          if (window.location.pathname !== '/login') {
            window.location.href = '/login'
          }
        }
      }
    }
    
    return Promise.reject(error)
  }
)

// Auth API endpoints
export const authApi = {
  register: (data: { email: string; password: string; name: string }) =>
    api.post('/auth/register', data),
  
  login: (data: { email: string; password: string }) =>
    api.post('/auth/login', data),
  
  logout: () =>
    api.post('/auth/logout'),
  
  getCurrentUser: () =>
    api.get('/auth/me'),
  
  refreshToken: (refreshToken: string) =>
    api.post('/auth/refresh', { refreshToken })
}

export default api
