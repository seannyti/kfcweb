<template>
  <div class="login-view py-5">
    <div class="container">
      <div class="row justify-content-center">
        <div class="col-md-6 col-lg-5">
          <div class="card shadow-lg border-0">
            <div class="card-body p-5">
              <div class="text-center mb-4">
                <i class="bi bi-box-arrow-in-right text-primary display-4"></i>
                <h2 class="mt-3 mb-2">Welcome Back</h2>
                <p class="text-muted">Login to your account</p>
              </div>

              <form @submit.prevent="handleLogin">
                <div class="mb-3">
                  <label for="email" class="form-label">Email address</label>
                  <div class="input-group">
                    <span class="input-group-text">
                      <i class="bi bi-envelope"></i>
                    </span>
                    <input
                      type="email"
                      class="form-control"
                      id="email"
                      v-model="email"
                      placeholder="Enter your email"
                      required
                      :disabled="loading"
                    />
                  </div>
                </div>

                <div class="mb-3">
                  <label for="password" class="form-label">Password</label>
                  <div class="input-group">
                    <span class="input-group-text">
                      <i class="bi bi-lock"></i>
                    </span>
                    <input
                      :type="showPassword ? 'text' : 'password'"
                      class="form-control"
                      id="password"
                      v-model="password"
                      placeholder="Enter your password"
                      required
                      :disabled="loading"
                    />
                    <button
                      class="btn btn-outline-secondary"
                      type="button"
                      @click="showPassword = !showPassword"
                      :disabled="loading"
                    >
                      <i :class="showPassword ? 'bi bi-eye-slash' : 'bi bi-eye'"></i>
                    </button>
                  </div>
                </div>

                <div class="alert alert-danger" v-if="error">
                  <i class="bi bi-exclamation-triangle me-2"></i>
                  {{ error }}
                </div>

                <button
                  type="submit"
                  class="btn btn-primary w-100 py-2"
                  :disabled="loading"
                >
                  <span v-if="loading">
                    <span class="spinner-border spinner-border-sm me-2" role="status"></span>
                    Logging in...
                  </span>
                  <span v-else>
                    <i class="bi bi-box-arrow-in-right me-2"></i>
                    Login
                  </span>
                </button>
              </form>

              <div class="text-center mt-4">
                <p class="text-muted mb-0">
                  Don't have an account?
                  <RouterLink to="/register" class="text-decoration-none">
                    Sign up here
                  </RouterLink>
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter, RouterLink } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const email = ref('')
const password = ref('')
const showPassword = ref(false)
const loading = ref(false)
const error = ref<string | null>(null)

const handleLogin = async () => {
  if (!email.value || !password.value) {
    error.value = 'Please fill in all fields'
    return
  }

  try {
    loading.value = true
    error.value = null
    
    const success = await authStore.login(email.value, password.value)
    
    if (success) {
      showToast('Login successful! Welcome back.', 'success')
      
      // Redirect to dashboard or intended page
      router.push('/dashboard')
    }
  } catch (err: any) {
    error.value = err.response?.data?.message || 'Invalid email or password'
    console.error('Login error:', err)
  } finally {
    loading.value = false
  }
}

const showToast = (message: string, type: string = 'info') => {
  const toastElement = document.getElementById('liveToast')
  const toastMessage = document.getElementById('toastMessage')
  
  if (toastElement && toastMessage) {
    toastMessage.textContent = message
    const toast = new (window as any).bootstrap.Toast(toastElement)
    toast.show()
  }
}
</script>

<style scoped>
.login-view {
  min-height: 70vh;
  display: flex;
  align-items: center;
  background: linear-gradient(135deg, var(--theme-light-surface, #f5f7fa) 0%, color-mix(in srgb, var(--bs-primary) 20%, var(--theme-light-bg, #fff)) 100%);
}

.card {
  border-radius: var(--bs-border-radius, 1rem);
}

.input-group-text {
  background-color: var(--theme-light-surface, #f8f9fa);
  border-right: none;
}

.form-control {
  border-left: none;
}

.form-control:focus {
  box-shadow: none;
  border-color: var(--bs-border-color);
}

.input-group:focus-within .input-group-text {
  border-color: var(--bs-primary);
  background-color: color-mix(in srgb, var(--bs-primary) 10%, white);
}

.input-group:focus-within .form-control {
  border-color: var(--bs-primary);
}
</style>
