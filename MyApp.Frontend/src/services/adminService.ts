import api from './api'

export interface SiteSettings {
  // General
  siteName: string
  tagline: string
  description: string
  logo: string | null
  favicon: string | null
  timezone: string
  dateFormat: string
  allowRegistration: boolean
  forceHttps: boolean
  
  // Email
  emailEnabled: boolean
  smtpServer: string
  smtpPort: number
  useSsl: boolean
  smtpUsername: string
  smtpPassword: string
  fromEmail: string
  fromName: string
  
  // Maintenance
  maintenanceMode: boolean
  maintenanceMessage: string
  enableApiAccess: boolean
  
  // Security
  enforce2FA: boolean
  sessionTimeout: number
  maxLoginAttempts: number
  enableIpWhitelist: boolean
  whitelistedIps: string
  minPasswordLength: number
  requireUppercase: boolean
  requireNumbers: boolean
  requireSpecialChars: boolean
}

export interface EmailLog {
  id: number
  sentAt: Date
  to: string
  subject: string
  status: 'sent' | 'pending' | 'failed'
}

export interface ActivityLog {
  id: number
  timestamp: Date
  type: string
  user: string
  action: string
  ipAddress: string
}

export interface Backup {
  id: number
  name: string
  size: string
  createdAt: Date
  type: string
}

export interface ApiKey {
  id: number
  name: string
  key: string
  permissions: string[]
  isActive: boolean
  createdAt: Date
  lastUsed: Date | null
}

export interface SystemHealth {
  cpu: number
  memory: number
  disk: number
  uptime: number
}

export interface DashboardStats {
  totalUsers: number
  emailsSentToday: number
  emailsTotal: number
  uptime: string
  lastBackup: string
}

export interface ThemeSettings {
  primaryColor: string
  secondaryColor: string
  successColor: string
  dangerColor: string
  warningColor: string
  infoColor: string
  darkBg: string
  darkSurface: string
  darkText: string
  lightBg: string
  lightSurface: string
  lightText: string
  borderRadius: string
  fontFamily: string
  darkModeDefault: boolean
  forceDarkMode: boolean
  useGradientBg: boolean
  useGlassmorphism: boolean
  animatedBg: boolean
  accentGradient: string
  customCss: string
}

const adminService = {
  // General Settings (MySettings.Api)
  async getGeneralSettings() {
    const response = await api.get('/settings/general')
    return response.data
  },

  async saveGeneralSettings(settings: any): Promise<void> {
    await api.put('/settings/general', settings)
  },

  // Site Settings (MySettings.Api)
  async getSettings(): Promise<SiteSettings> {
    const response = await api.get('/admin/settings')
    return response.data
  },

  async saveSettings(settings: Partial<SiteSettings>): Promise<void> {
    await api.put('/admin/settings', settings)
  },

  // Email
  async getEmailSettings(): Promise<Partial<SiteSettings>> {
    const response = await api.get('/admin/settings')
    return response.data
  },

  async saveEmailSettings(settings: Partial<SiteSettings>): Promise<void> {
    await api.put('/admin/settings', settings)
  },

  async sendTestEmail(): Promise<void> {
    await api.post('/admin/email/test')
  },

  async getEmailHistory(): Promise<EmailLog[]> {
    // Email history feature not implemented
    return []
  },

  // Maintenance
  async toggleMaintenanceMode(enabled: boolean): Promise<void> {
    await api.post('/admin/maintenance/toggle', { enabled })
  },

  async clearCache(): Promise<void> {
    await api.post('/admin/cache/clear')
  },

  async optimizeDatabase(): Promise<void> {
    await api.post('/admin/database/optimize')
  },

  async checkUpdates(): Promise<any> {
    const response = await api.get('/admin/system/updates')
    return response.data
  },

  // Security Settings (MySettings.Api)
  async getSecuritySettings(): Promise<Partial<SiteSettings>> {
    const response = await api.get('/admin/settings')
    return response.data
  },

  async saveSecuritySettings(settings: Partial<SiteSettings>): Promise<void> {
    await api.put('/admin/settings', settings)
  },

  // User Management (MyUsers.Api)
  async getUsers(): Promise<any[]> {
    const response = await api.get('/admin/users')
    return response.data
  },

  async updateUserRole(userId: number, role: string): Promise<any> {
    const response = await api.put('/admin/users/role', { userId, role })
    return response.data
  },

  async deleteUser(userId: number): Promise<void> {
    await api.delete(`/admin/users/${userId}`)
  },

  async getUserStatistics(): Promise<any> {
    const response = await api.get('/admin/statistics')
    return response.data
  },

  // Activity Logs (Not implemented)
  async getActivityLogs(params?: { type?: string; date?: string }): Promise<ActivityLog[]> {
    // Activity logs feature not implemented
    return []
  },

  async exportActivityLogs(): Promise<Blob> {
    // Activity logs feature not implemented
    return new Blob()
  },

  // Backups (Not implemented - use SQL Server backup tools)
  async getBackups(): Promise<Backup[]> {
    // Backup management should be done via SQL Server Management Studio
    return []
  },

  async createBackup(): Promise<void> {
    // Backup management should be done via SQL Server Management Studio
  },

  async downloadBackup(id: number): Promise<Blob> {
    // Backup management should be done via SQL Server Management Studio
    return new Blob()
  },

  async restoreBackup(id: number): Promise<void> {
    // Backup management should be done via SQL Server Management Studio
  },

  async deleteBackup(id: number): Promise<void> {
    // Backup management should be done via SQL Server Management Studio
  },

  // API Keys (Not implemented)
  async getApiKeys(): Promise<ApiKey[]> {
    // API key management feature not implemented
    return []
  },

  async createApiKey(name: string, permissions: string[]): Promise<ApiKey> {
    // API key management feature not implemented
    return {} as ApiKey
  },

  async toggleApiKey(id: number): Promise<void> {
    // API key management feature not implemented
  },

  async deleteApiKey(id: number): Promise<void> {
    // API key management feature not implemented
  },

  // System Health (MySettings.Api - fake data)
  async getSystemHealth(): Promise<SystemHealth> {
    const response = await api.get('/admin/system/health')
    return response.data
  },

  // Real System Health (MyUsers.Api - real metrics)
  async getRealSystemHealth(): Promise<SystemHealth> {
    const response = await api.get('/system-health')
    return response.data
  },

  // Dashboard
  async getDashboardStats(): Promise<DashboardStats> {
    const response = await api.get('/admin/dashboard/stats')
    return response.data
  },

  async getRecentActions(): Promise<any[]> {
    const response = await api.get('/admin/dashboard/recent-actions')
    return response.data
  },

  // Theme
  async getTheme(): Promise<ThemeSettings> {
    const response = await api.get('/admin/theme')
    return response.data
  },

  async saveTheme(theme: ThemeSettings): Promise<void> {
    await api.put('/admin/theme', theme)
  },

  async resetTheme(): Promise<void> {
    await api.post('/admin/theme/reset')
  }
}

export default adminService
