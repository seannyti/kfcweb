<template>
  <div class="email-sent-container">
    <div class="email-sent-card">
      <div class="card shadow-lg">
        <div class="card-body text-center p-5">
          <i class="bi bi-envelope-check text-primary display-1 mb-4"></i>
          <h1 class="mb-3">Check Your Email</h1>
          <p class="lead mb-4">
            We've sent a verification link to your email address. Please check your inbox and click the link to activate your account.
          </p>
          
          <div class="alert alert-info text-start">
            <h6 class="alert-heading">
              <i class="bi bi-info-circle me-2"></i>What to do next:
            </h6>
            <ol class="mb-0">
              <li>Check your email inbox (and spam folder)</li>
              <li>Click the verification link in the email</li>
              <li>You'll be redirected back to log in</li>
            </ol>
          </div>

          <div class="mt-4">
            <p class="text-muted small">Didn't receive the email?</p>
            <button @click="resendEmail" class="btn btn-outline-primary" :disabled="resending || cooldown > 0">
              <span v-if="resending">
                <span class="spinner-border spinner-border-sm me-2"></span>Sending...
              </span>
              <span v-else-if="cooldown > 0">
                Resend in {{ cooldown }}s
              </span>
              <span v-else>
                <i class="bi bi-arrow-clockwise me-2"></i>Resend Verification Email
              </span>
            </button>
          </div>

          <hr class="my-4">

          <router-link to="/login" class="btn btn-link">
            <i class="bi bi-arrow-left me-2"></i>Back to Login
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useToast } from 'vue-toastification'
import api from '@/services/api'

const toast = useToast()
const resending = ref(false)
const cooldown = ref(0)

const resendEmail = async () => {
  const email = prompt('Please enter your email address:')
  if (!email) return

  try {
    resending.value = true
    const response = await api.post('/auth/resend-verification', { email })
    toast.success(response.data.message)
    
    // Start cooldown timer
    cooldown.value = 60
    const interval = setInterval(() => {
      cooldown.value--
      if (cooldown.value <= 0) {
        clearInterval(interval)
      }
    }, 1000)
  } catch (error: any) {
    toast.error(error.response?.data?.message || 'Failed to resend verification email')
  } finally {
    resending.value = false
  }
}
</script>

<style scoped>
.email-sent-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 20px;
}

.email-sent-card {
  width: 100%;
  max-width: 700px;
}

.card {
  border: none;
  border-radius: 15px;
}

.display-1 {
  font-size: 5rem;
}
</style>
