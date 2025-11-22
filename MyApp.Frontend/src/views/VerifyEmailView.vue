<template>
  <div class="verify-email-container">
    <div class="verify-email-card">
      <div class="card shadow-lg">
        <div class="card-body text-center p-5">
          <div v-if="loading">
            <div class="spinner-border text-primary mb-3" role="status">
              <span class="visually-hidden">Verifying...</span>
            </div>
            <h3>Verifying your email...</h3>
            <p class="text-muted">Please wait while we verify your account</p>
          </div>

          <div v-else-if="success">
            <i class="bi bi-check-circle text-success display-1 mb-3"></i>
            <h2>Email Verified!</h2>
            <p class="text-muted mb-4">Your email has been successfully verified. You can now log in to your account.</p>
            <router-link to="/login" class="btn btn-primary btn-lg">
              <i class="bi bi-box-arrow-in-right me-2"></i>Go to Login
            </router-link>
          </div>

          <div v-else>
            <i class="bi bi-x-circle text-danger display-1 mb-3"></i>
            <h2>Verification Failed</h2>
            <p class="text-danger mb-4">{{ errorMessage }}</p>
            <router-link to="/email-sent" class="btn btn-outline-primary me-2">
              <i class="bi bi-arrow-left me-2"></i>Back
            </router-link>
            <button @click="resendEmail" class="btn btn-primary" :disabled="resending">
              <span v-if="resending">
                <span class="spinner-border spinner-border-sm me-2"></span>Sending...
              </span>
              <span v-else>
                <i class="bi bi-envelope me-2"></i>Resend Verification Email
              </span>
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useToast } from 'vue-toastification'
import api from '@/services/api'

const route = useRoute()
const toast = useToast()

const loading = ref(true)
const success = ref(false)
const errorMessage = ref('')
const resending = ref(false)
const userEmail = ref('')

onMounted(async () => {
  const token = route.query.token as string
  
  if (!token) {
    success.value = false
    errorMessage.value = 'Invalid verification link'
    loading.value = false
    return
  }

  try {
    const response = await api.get('/auth/verify-email', { params: { token } })
    success.value = true
    toast.success(response.data.message)
  } catch (error: any) {
    success.value = false
    errorMessage.value = error.response?.data?.message || 'Verification failed. The link may have expired.'
    toast.error(errorMessage.value)
  } finally {
    loading.value = false
  }
})

const resendEmail = async () => {
  const email = prompt('Please enter your email address:')
  if (!email) return

  try {
    resending.value = true
    const response = await api.post('/auth/resend-verification', { email })
    toast.success(response.data.message)
  } catch (error: any) {
    toast.error(error.response?.data?.message || 'Failed to resend verification email')
  } finally {
    resending.value = false
  }
}
</script>

<style scoped>
.verify-email-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 20px;
}

.verify-email-card {
  width: 100%;
  max-width: 600px;
}

.card {
  border: none;
  border-radius: 15px;
}

.display-1 {
  font-size: 5rem;
}
</style>
