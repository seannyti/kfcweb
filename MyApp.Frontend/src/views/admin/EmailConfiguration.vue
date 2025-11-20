<template>
  <div class="email-configuration">
    <div class="row mb-4">
      <div class="col">
        <h2 class="mb-0">
          <i class="bi bi-envelope-fill text-warning me-2"></i>
          Email Configuration
        </h2>
        <p class="text-muted mb-0">Configure SMTP settings and manage email delivery</p>
      </div>
    </div>

    <!-- Email System Status Card -->
    <div class="card shadow-sm mb-4">
      <div class="card-header bg-white py-3">
        <h5 class="mb-0">
          <i class="bi bi-power me-2"></i>
          Email System Status
        </h5>
      </div>
      <div class="card-body">
        <div class="d-flex align-items-center justify-content-between">
          <div>
            <h6 class="mb-1">Email Delivery</h6>
            <small class="text-muted">
              {{ settings.emailEnabled ? 'Emails are being sent via SMTP' : 'Emails are logged but not sent' }}
            </small>
          </div>
          <div class="form-check form-switch">
            <input 
              class="form-check-input" 
              type="checkbox" 
              role="switch" 
              id="emailEnabled"
              v-model="settings.emailEnabled"
              style="width: 3em; height: 1.5em;"
            >
            <label class="form-check-label ms-2" for="emailEnabled">
              <span class="badge" :class="settings.emailEnabled ? 'bg-success' : 'bg-secondary'">
                {{ settings.emailEnabled ? 'ON' : 'OFF' }}
              </span>
            </label>
          </div>
        </div>
      </div>
    </div>

    <!-- SMTP Configuration Card -->
    <div class="card shadow-sm mb-4">
      <div class="card-header bg-white py-3">
        <h5 class="mb-0">
          <i class="bi bi-server me-2"></i>
          SMTP Server Configuration
        </h5>
      </div>
      <div class="card-body">
        <form @submit.prevent="saveSettings">
          <div class="row g-3">
            <!-- SMTP Server -->
            <div class="col-md-8">
              <label for="smtpServer" class="form-label">
                SMTP Server <span class="text-danger">*</span>
              </label>
              <input 
                type="text" 
                class="form-control" 
                id="smtpServer"
                v-model="settings.smtpServer"
                placeholder="smtp.gmail.com"
                required
              >
            </div>

            <!-- SMTP Port -->
            <div class="col-md-4">
              <label for="smtpPort" class="form-label">
                SMTP Port <span class="text-danger">*</span>
              </label>
              <input 
                type="number" 
                class="form-control" 
                id="smtpPort"
                v-model.number="settings.smtpPort"
                placeholder="587"
                required
              >
            </div>

            <!-- Use SSL -->
            <div class="col-12">
              <div class="form-check">
                <input 
                  class="form-check-input" 
                  type="checkbox" 
                  id="useSsl"
                  v-model="settings.useSsl"
                >
                <label class="form-check-label" for="useSsl">
                  Use SSL/TLS (Recommended for port 587 or 465)
                </label>
              </div>
            </div>

            <!-- SMTP Username -->
            <div class="col-md-6">
              <label for="smtpUsername" class="form-label">
                SMTP Username <span class="text-danger">*</span>
              </label>
              <input 
                type="text" 
                class="form-control" 
                id="smtpUsername"
                v-model="settings.smtpUsername"
                placeholder="your-email@example.com"
                required
              >
              <div class="form-text">Usually your full email address</div>
            </div>

            <!-- SMTP Password -->
            <div class="col-md-6">
              <label for="smtpPassword" class="form-label">
                SMTP Password <span class="text-danger">*</span>
              </label>
              <div class="input-group">
                <input 
                  :type="showPassword ? 'text' : 'password'" 
                  class="form-control" 
                  id="smtpPassword"
                  v-model="settings.smtpPassword"
                  placeholder="••••••••"
                  required
                >
                <button 
                  class="btn btn-outline-secondary" 
                  type="button"
                  @click="showPassword = !showPassword"
                >
                  <i :class="showPassword ? 'bi bi-eye-slash' : 'bi bi-eye'"></i>
                </button>
              </div>
              <div class="form-text">App password for Gmail/Yahoo, or account password</div>
            </div>

            <!-- From Email Address -->
            <div class="col-md-6">
              <label for="fromEmail" class="form-label">
                From Email Address <span class="text-danger">*</span>
              </label>
              <input 
                type="email" 
                class="form-control" 
                id="fromEmail"
                v-model="settings.fromEmail"
                placeholder="noreply@example.com"
                required
              >
              <div class="form-text">Email address that appears as sender</div>
            </div>

            <!-- From Display Name -->
            <div class="col-md-6">
              <label for="fromName" class="form-label">
                From Display Name <span class="text-danger">*</span>
              </label>
              <input 
                type="text" 
                class="form-control" 
                id="fromName"
                v-model="settings.fromName"
                placeholder="Knudson Family Construction"
                required
              >
              <div class="form-text">Name that appears as sender</div>
            </div>
          </div>

          <div class="mt-4 d-flex gap-2">
            <button 
              type="submit" 
              class="btn btn-primary"
              :disabled="saving"
            >
              <i class="bi bi-save me-2"></i>
              {{ saving ? 'Saving...' : 'Save Configuration' }}
            </button>
            <button 
              type="button" 
              class="btn btn-outline-secondary"
              @click="sendTestEmail"
              :disabled="sendingTest || !settings.emailEnabled"
            >
              <i class="bi bi-envelope-check me-2"></i>
              {{ sendingTest ? 'Sending...' : 'Send Test Email' }}
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Email History Card -->
    <div class="card shadow-sm">
      <div class="card-header bg-white py-3 d-flex justify-content-between align-items-center">
        <h5 class="mb-0">
          <i class="bi bi-clock-history me-2"></i>
          Recent Email Activity
        </h5>
        <span class="badge bg-secondary">Last 20 emails</span>
      </div>
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr>
                <th>Date/Time</th>
                <th>To</th>
                <th>Subject</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="emailHistory.length === 0">
                <td colspan="4" class="text-center text-muted py-4">
                  <i class="bi bi-inbox fs-1 d-block mb-2"></i>
                  No email activity yet
                </td>
              </tr>
              <tr v-for="email in emailHistory" :key="email.id">
                <td>
                  <small>{{ formatDate(email.sentAt) }}</small>
                </td>
                <td>{{ email.to }}</td>
                <td>{{ email.subject }}</td>
                <td>
                  <span 
                    class="badge" 
                    :class="{
                      'bg-success': email.status === 'sent',
                      'bg-warning': email.status === 'pending',
                      'bg-danger': email.status === 'failed'
                    }"
                  >
                    <i 
                      :class="{
                        'bi bi-check-circle': email.status === 'sent',
                        'bi bi-clock': email.status === 'pending',
                        'bi bi-x-circle': email.status === 'failed'
                      }"
                      class="me-1"
                    ></i>
                    {{ email.status }}
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
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
  emailEnabled: true,
  smtpServer: 'smtp.gmail.com',
  smtpPort: 587,
  useSsl: true,
  smtpUsername: 'knudsonfamilyconstruction@yahoo.com',
  smtpPassword: '',
  fromEmail: 'noreply@knudsonfamilyconstruction.com',
  fromName: 'Knudson Family Construction'
})

const showPassword = ref(false)
const saving = ref(false)
const sendingTest = ref(false)

const emailHistory = ref([
  {
    id: 1,
    sentAt: new Date(Date.now() - 3600000),
    to: 'user@example.com',
    subject: 'Welcome to Knudson Family Construction',
    status: 'sent'
  },
  {
    id: 2,
    sentAt: new Date(Date.now() - 7200000),
    to: 'admin@example.com',
    subject: 'Password Reset Request',
    status: 'sent'
  },
  {
    id: 3,
    sentAt: new Date(Date.now() - 10800000),
    to: 'test@example.com',
    subject: 'Test Email',
    status: 'sent'
  }
])

const formatDate = (date: Date) => {
  return new Intl.DateTimeFormat('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(new Date(date))
}

const loadSettings = async () => {
  try {
    const response = await adminService.getEmailSettings()
    if (response) {
      settings.value = {
        emailEnabled: response.emailEnabled ?? true,
        smtpServer: response.smtpServer ?? 'smtp.gmail.com',
        smtpPort: response.smtpPort ?? 587,
        useSsl: response.useSsl ?? true,
        smtpUsername: response.smtpUsername ?? '',
        smtpPassword: response.smtpPassword || '',
        fromEmail: response.fromEmail ?? '',
        fromName: response.fromName ?? 'Knudson Family Construction'
      }
    }
  } catch (error) {
    console.error('Failed to load email settings:', error)
    toast.error('Failed to load email settings')
  }
}

const saveSettings = async () => {
  saving.value = true
  try {
    await adminService.saveEmailSettings(settings.value)
    toast.success('Email settings saved successfully!')
  } catch (error) {
    console.error('Failed to save email settings:', error)
    toast.error('Failed to save email settings')
  } finally {
    saving.value = false
  }
}

const sendTestEmail = async () => {
  if (!settings.value.emailEnabled) {
    toast.warning('Email system is disabled. Enable it first.')
    return
  }

  sendingTest.value = true
  try {
    await adminService.sendTestEmail()
    
    // Add to history
    emailHistory.value.unshift({
      id: Date.now(),
      sentAt: new Date(),
      to: settings.value.smtpUsername,
      subject: 'Test Email from Admin Panel',
      status: 'sent'
    })
    
    toast.success('Test email sent successfully! Check your inbox.')
  } catch (error: any) {
    console.error('Failed to send test email:', error)
    toast.error(error.response?.data?.message || 'Failed to send test email. Check your SMTP settings.')
  } finally {
    sendingTest.value = false
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

.table th {
  font-weight: 600;
  font-size: 0.875rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}
</style>
