<template>
  <div class="security-settings">
    <div class="row mb-4">
      <div class="col">
        <h2 class="mb-0">
          <i class="bi bi-shield-lock-fill text-warning me-2"></i>
          Security Settings
        </h2>
        <p class="text-muted mb-0">Configure authentication and security policies</p>
      </div>
    </div>

    <!-- Two-Factor Authentication -->
    <div class="card shadow-sm mb-4">
      <div class="card-header bg-white py-3">
        <h5 class="mb-0">
          <i class="bi bi-shield-fill-check me-2"></i>
          Two-Factor Authentication
        </h5>
      </div>
      <div class="card-body">
        <div class="form-check form-switch">
          <input 
            class="form-check-input" 
            type="checkbox" 
            role="switch" 
            id="enforce2FA"
            v-model="settings.enforce2FA"
          >
          <label class="form-check-label" for="enforce2FA">
            Enforce 2FA for All Administrators
            <small class="text-muted d-block">Require two-factor authentication for admin accounts</small>
          </label>
        </div>
      </div>
    </div>

    <!-- Session Management -->
    <div class="card shadow-sm mb-4">
      <div class="card-header bg-white py-3">
        <h5 class="mb-0">
          <i class="bi bi-clock-history me-2"></i>
          Session Management
        </h5>
      </div>
      <div class="card-body">
        <div class="row g-3">
          <div class="col-md-6">
            <label for="sessionTimeout" class="form-label">
              Session Timeout (minutes)
            </label>
            <input 
              type="number" 
              class="form-control" 
              id="sessionTimeout"
              v-model.number="settings.sessionTimeout"
              min="5"
              max="1440"
            >
            <small class="text-muted">Users will be logged out after this period of inactivity</small>
          </div>

          <div class="col-md-6">
            <label for="maxLoginAttempts" class="form-label">
              Max Login Attempts
            </label>
            <input 
              type="number" 
              class="form-control" 
              id="maxLoginAttempts"
              v-model.number="settings.maxLoginAttempts"
              min="3"
              max="10"
            >
            <small class="text-muted">Lock account after this many failed attempts</small>
          </div>
        </div>
      </div>
    </div>

    <!-- IP Whitelist -->
    <div class="card shadow-sm mb-4">
      <div class="card-header bg-white py-3">
        <h5 class="mb-0">
          <i class="bi bi-geo-alt me-2"></i>
          Admin IP Whitelist
        </h5>
      </div>
      <div class="card-body">
        <div class="form-check form-switch mb-3">
          <input 
            class="form-check-input" 
            type="checkbox" 
            role="switch" 
            id="enableIpWhitelist"
            v-model="settings.enableIpWhitelist"
          >
          <label class="form-check-label" for="enableIpWhitelist">
            Enable IP Whitelist
            <small class="text-muted d-block">Only allow admin access from specified IP addresses</small>
          </label>
        </div>

        <div v-if="settings.enableIpWhitelist">
          <label for="ipAddresses" class="form-label">Allowed IP Addresses</label>
          <textarea 
            class="form-control" 
            id="ipAddresses"
            v-model="settings.whitelistedIps"
            rows="4"
            placeholder="192.168.1.1&#10;10.0.0.1&#10;One IP per line"
          ></textarea>
        </div>
      </div>
    </div>

    <!-- Password Requirements -->
    <div class="card shadow-sm mb-4">
      <div class="card-header bg-white py-3">
        <h5 class="mb-0">
          <i class="bi bi-key-fill me-2"></i>
          Password Requirements
        </h5>
      </div>
      <div class="card-body">
        <div class="row g-3">
          <div class="col-md-6">
            <label for="minPasswordLength" class="form-label">
              Minimum Password Length
            </label>
            <input 
              type="number" 
              class="form-control" 
              id="minPasswordLength"
              v-model.number="settings.minPasswordLength"
              min="6"
              max="32"
            >
          </div>

          <div class="col-md-6">
            <label class="form-label">Password Complexity</label>
            <div class="form-check">
              <input 
                class="form-check-input" 
                type="checkbox" 
                id="requireUppercase"
                v-model="settings.requireUppercase"
              >
              <label class="form-check-label" for="requireUppercase">
                Require uppercase letters
              </label>
            </div>
            <div class="form-check">
              <input 
                class="form-check-input" 
                type="checkbox" 
                id="requireNumbers"
                v-model="settings.requireNumbers"
              >
              <label class="form-check-label" for="requireNumbers">
                Require numbers
              </label>
            </div>
            <div class="form-check">
              <input 
                class="form-check-input" 
                type="checkbox" 
                id="requireSpecialChars"
                v-model="settings.requireSpecialChars"
              >
              <label class="form-check-label" for="requireSpecialChars">
                Require special characters
              </label>
            </div>
          </div>
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
        {{ saving ? 'Saving...' : 'Save Security Settings' }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useToast } from 'vue-toastification'

const toast = useToast()

const settings = ref({
  enforce2FA: false,
  sessionTimeout: 60,
  maxLoginAttempts: 5,
  enableIpWhitelist: false,
  whitelistedIps: '',
  minPasswordLength: 8,
  requireUppercase: true,
  requireNumbers: true,
  requireSpecialChars: true
})

const saving = ref(false)

const saveSettings = async () => {
  saving.value = true
  try {
    await new Promise(resolve => setTimeout(resolve, 1000))
    toast.success('Security settings saved successfully!')
  } catch (error) {
    toast.error('Failed to save security settings')
  } finally {
    saving.value = false
  }
}
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
