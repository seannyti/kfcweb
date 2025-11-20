import axios, { type AxiosInstance } from 'axios'

// Create axios instance for Settings API
const settingsApi: AxiosInstance = axios.create({
  baseURL: import.meta.env.VITE_SETTINGS_API_URL || '/api',
  timeout: 10000,
  withCredentials: true,
  headers: {
    'Content-Type': 'application/json'
  }
})

export default settingsApi
