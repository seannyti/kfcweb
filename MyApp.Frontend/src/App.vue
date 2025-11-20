<template>
  <div id="app" class="d-flex flex-column min-vh-100">
    <Header v-if="!isMaintenancePage" />
    
    <main class="flex-grow-1">
      <RouterView />
    </main>
    
    <Footer v-if="!isMaintenancePage" />
    
    <!-- Toast Container for notifications -->
    <div class="toast-container position-fixed bottom-0 end-0 p-3">
      <div id="liveToast" class="toast" role="alert" aria-live="assertive" aria-atomic="true">
        <div class="toast-header">
          <i class="bi bi-bell-fill me-2"></i>
          <strong class="me-auto">Notification</strong>
          <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
        </div>
        <div class="toast-body" id="toastMessage">
          <!-- Dynamic message will be inserted here -->
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, computed } from 'vue'
import { RouterView, useRoute } from 'vue-router'
import Header from './components/layout/Header.vue'
import Footer from './components/layout/Footer.vue'
import { useThemeStore } from '@/stores/theme'

const themeStore = useThemeStore()
const route = useRoute()

const isMaintenancePage = computed(() => route.name === 'maintenance')

// Load and apply saved theme on app start
onMounted(async () => {
  await themeStore.loadTheme()
})
</script>

<style>
/* Global styles */
:root {
  --bs-body-font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
}

body {
  min-height: 100vh;
}

#app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

/* Apply theme colors to Bootstrap components */
.btn-primary {
  background-color: var(--bs-primary) !important;
  border-color: var(--bs-primary) !important;
}

.btn-primary:hover,
.btn-primary:focus {
  background-color: color-mix(in srgb, var(--bs-primary) 85%, black) !important;
  border-color: color-mix(in srgb, var(--bs-primary) 80%, black) !important;
}

.btn-outline-primary {
  color: var(--bs-primary) !important;
  border-color: var(--bs-primary) !important;
}

.btn-outline-primary:hover {
  background-color: var(--bs-primary) !important;
  border-color: var(--bs-primary) !important;
}

.text-primary {
  color: var(--bs-primary) !important;
}

.bg-primary {
  background-color: var(--bs-primary) !important;
}

.border-primary {
  border-color: var(--bs-primary) !important;
}

.badge.bg-primary {
  background-color: var(--bs-primary) !important;
}

.alert-primary {
  background-color: color-mix(in srgb, var(--bs-primary) 15%, white) !important;
  border-color: color-mix(in srgb, var(--bs-primary) 30%, white) !important;
  color: color-mix(in srgb, var(--bs-primary) 80%, black) !important;
}

a {
  color: var(--bs-primary);
}

a:hover {
  color: color-mix(in srgb, var(--bs-primary) 80%, black);
}

/* Custom utilities */
.min-vh-100 {
  min-height: 100vh;
}

.card {
  transition: box-shadow 0.3s ease;
}

.card:hover {
  box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.15);
}

/* Form improvements */
.form-control:focus,
.form-select:focus {
  border-color: color-mix(in srgb, var(--bs-primary) 40%, white);
  box-shadow: 0 0 0 0.25rem color-mix(in srgb, var(--bs-primary) 25%, transparent);
}

/* Smooth transitions */
* {
  transition: background-color 0.2s ease, color 0.2s ease;
}

/* Gradient Background Effect */
body.gradient-bg {
  background: var(--body-gradient, linear-gradient(135deg, #f97316 0%, #fb923c 100%)) !important;
  background-attachment: fixed !important;
  min-height: 100vh;
}

body.gradient-bg #app {
  background: transparent;
}

/* Animated Background Effect */
body.animated-bg {
  position: relative;
  overflow-x: hidden;
}

body.animated-bg::before {
  content: '';
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(
    45deg,
    color-mix(in srgb, var(--bs-primary) 20%, transparent) 0%,
    color-mix(in srgb, var(--bs-secondary) 20%, transparent) 100%
  );
  background-size: 400% 400%;
  animation: gradientShift 15s ease infinite;
  z-index: -1;
  pointer-events: none;
}

@keyframes gradientShift {
  0% { background-position: 0% 50%; }
  50% { background-position: 100% 50%; }
  100% { background-position: 0% 50%; }
}

/* Glassmorphism Effect */
body.glassmorphism-enabled .card,
body.glassmorphism-enabled .modal-content,
body.glassmorphism-enabled .dropdown-menu {
  background: rgba(255, 255, 255, 0.7) !important;
  backdrop-filter: blur(10px) !important;
  -webkit-backdrop-filter: blur(10px) !important;
  border: 1px solid rgba(255, 255, 255, 0.2) !important;
}

[data-bs-theme="dark"] body.glassmorphism-enabled .card,
[data-bs-theme="dark"] body.glassmorphism-enabled .modal-content,
[data-bs-theme="dark"] body.glassmorphism-enabled .dropdown-menu {
  background: rgba(0, 0, 0, 0.5) !important;
  backdrop-filter: blur(10px) !important;
  -webkit-backdrop-filter: blur(10px) !important;
  border: 1px solid rgba(255, 255, 255, 0.1) !important;
}

/* Dark Mode Styles */
[data-bs-theme="dark"] body {
  background-color: var(--theme-dark-bg, #1a1a1a) !important;
  color: var(--theme-dark-text, #f5f5f5) !important;
}

[data-bs-theme="dark"] .card {
  background-color: var(--theme-dark-surface, #2d2d2d) !important;
  color: var(--theme-dark-text, #f5f5f5) !important;
}

</style>
