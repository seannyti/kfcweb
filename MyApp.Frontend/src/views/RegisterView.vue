<template>
  <div class="register-view py-5">
    <div class="container">
      <div class="row justify-content-center">
        <div class="col-md-6 col-lg-5">
          <div class="card shadow-lg border-0">
            <div class="card-body p-5">
              <div class="text-center mb-4">
                <i class="bi bi-person-plus text-success display-4"></i>
                <h2 class="mt-3 mb-2">Create Account</h2>
                <p class="text-muted">Sign up to get started</p>
              </div>

              <form @submit.prevent="handleRegister">
                <div class="mb-3">
                  <label for="name" class="form-label">Full Name</label>
                  <div class="input-group">
                    <span class="input-group-text">
                      <i class="bi bi-person"></i>
                    </span>
                    <input
                      type="text"
                      class="form-control"
                      id="name"
                      v-model="name"
                      placeholder="Enter your full name"
                      required
                      minlength="2"
                      :disabled="loading"
                    />
                  </div>
                </div>

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
                      placeholder="Create a password"
                      required
                      minlength="6"
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
                  <div class="form-text">
                    Password must be at least 6 characters long
                  </div>
                </div>

                <div class="mb-3">
                  <label for="confirmPassword" class="form-label">Confirm Password</label>
                  <div class="input-group">
                    <span class="input-group-text">
                      <i class="bi bi-lock-fill"></i>
                    </span>
                    <input
                      :type="showPassword ? 'text' : 'password'"
                      class="form-control"
                      id="confirmPassword"
                      v-model="confirmPassword"
                      placeholder="Confirm your password"
                      required
                      :disabled="loading"
                    />
                  </div>
                </div>

                <div class="alert alert-danger" v-if="error">
                  <i class="bi bi-exclamation-triangle me-2"></i>
                  {{ error }}
                </div>

                <button
                  type="submit"
                  class="btn btn-success w-100 py-2"
                  :disabled="loading"
                >
                  <span v-if="loading">
                    <span class="spinner-border spinner-border-sm me-2" role="status"></span>
                    Creating account...
                  </span>
                  <span v-else>
                    <i class="bi bi-person-plus me-2"></i>
                    Create Account
                  </span>
                </button>
              </form>

              <div class="text-center mt-4">
                <p class="text-muted mb-0">
                  Already have an account?
                  <RouterLink to="/login" class="text-decoration-none">
                    Login here
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

const name = ref('')
const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const showPassword = ref(false)
const loading = ref(false)
const error = ref<string | null>(null)

const handleRegister = async () => {
  // Validation
  if (!name.value || !email.value || !password.value || !confirmPassword.value) {
    error.value = 'Please fill in all fields'
    return
  }

  if (password.value.length < 6) {
    error.value = 'Password must be at least 6 characters long'
    return
  }

  if (password.value !== confirmPassword.value) {
    error.value = 'Passwords do not match'
    return
  }

  try {
    loading.value = true
    error.value = null
    
    await authStore.register(email.value, password.value, name.value)
    
    showToast('Registration successful! Welcome to MyApp.', 'success')
    
    // Redirect to dashboard
    router.push('/dashboard')
  } catch (err: any) {
    error.value = err.response?.data?.message || 'Registration failed. Please try again.'
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
.register-view {
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
