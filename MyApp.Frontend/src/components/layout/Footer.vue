<template>
  <footer class="bg-dark text-white py-5 mt-auto">
    <div class="container">
      <div class="row g-4">
        <!-- Company Info -->
        <div class="col-md-4">
          <h5 class="mb-3">{{ settingsStore.settings.siteName }}</h5>
          <p class="text-white-50">{{ settingsStore.settings.tagline }}</p>
          <div v-if="hasSocialLinks" class="d-flex gap-2 mt-3">
            <a 
              v-if="businessContent?.facebookUrl" 
              :href="businessContent.facebookUrl" 
              target="_blank" 
              class="btn btn-sm btn-outline-light rounded-circle social-icon"
            >
              <i class="bi bi-facebook"></i>
            </a>
            <a 
              v-if="businessContent?.instagramUrl" 
              :href="businessContent.instagramUrl" 
              target="_blank" 
              class="btn btn-sm btn-outline-light rounded-circle social-icon"
            >
              <i class="bi bi-instagram"></i>
            </a>
            <a 
              v-if="businessContent?.linkedInUrl" 
              :href="businessContent.linkedInUrl" 
              target="_blank" 
              class="btn btn-sm btn-outline-light rounded-circle social-icon"
            >
              <i class="bi bi-linkedin"></i>
            </a>
            <a 
              v-if="businessContent?.twitterUrl" 
              :href="businessContent.twitterUrl" 
              target="_blank" 
              class="btn btn-sm btn-outline-light rounded-circle social-icon"
            >
              <i class="bi bi-twitter-x"></i>
            </a>
          </div>
        </div>

        <!-- Quick Links -->
        <div class="col-md-2">
          <h6 class="mb-3">Quick Links</h6>
          <ul class="list-unstyled">
            <li class="mb-2"><RouterLink to="/" class="text-white-50 text-decoration-none footer-link">Home</RouterLink></li>
            <li class="mb-2"><RouterLink to="/about" class="text-white-50 text-decoration-none footer-link">About</RouterLink></li>
            <li class="mb-2"><RouterLink to="/services" class="text-white-50 text-decoration-none footer-link">Services</RouterLink></li>
            <li class="mb-2"><RouterLink to="/projects" class="text-white-50 text-decoration-none footer-link">Projects</RouterLink></li>
            <li class="mb-2"><RouterLink to="/contact" class="text-white-50 text-decoration-none footer-link">Contact</RouterLink></li>
          </ul>
        </div>

        <!-- Services Preview -->
        <div class="col-md-3">
          <h6 class="mb-3">Our Services</h6>
          <ul v-if="services.length > 0" class="list-unstyled text-white-50 small">
            <li v-for="service in services.slice(0, 4)" :key="service.id" class="mb-1">
              {{ service.title }}
            </li>
          </ul>
          <ul v-else class="list-unstyled text-white-50 small">
            <li class="mb-1">Professional Services</li>
          </ul>
        </div>

        <!-- Contact Info -->
        <div class="col-md-3">
          <h6 class="mb-3">Contact Us</h6>
          <div class="text-white-50 small">
            <div v-if="businessContent?.contactPhone" class="mb-2">
              <i class="bi bi-telephone-fill me-2"></i>
              <a :href="`tel:${businessContent.contactPhone}`" class="text-white-50 text-decoration-none footer-link">
                {{ businessContent.contactPhone }}
              </a>
            </div>
            <div v-if="businessContent?.contactEmail" class="mb-2">
              <i class="bi bi-envelope-fill me-2"></i>
              <a :href="`mailto:${businessContent.contactEmail}`" class="text-white-50 text-decoration-none footer-link">
                {{ businessContent.contactEmail }}
              </a>
            </div>
            <div v-if="businessContent?.contactAddress" class="mb-2">
              <i class="bi bi-geo-alt-fill me-2"></i>
              {{ businessContent.contactAddress }}
            </div>
            <div v-if="businessContent?.businessHours" class="mt-3">
              <strong>Hours:</strong><br>
              {{ businessContent.businessHours }}
            </div>
          </div>
        </div>
      </div>

      <hr class="my-4 border-secondary">

      <div class="row">
        <div class="col-md-6 text-center text-md-start">
          <p class="mb-0 small text-white-50">
            &copy; {{ currentYear }} {{ settingsStore.settings.siteName }}. All rights reserved.
          </p>
        </div>
        <div class="col-md-6 text-center text-md-end">
          <p class="mb-0 small text-white-50">
            <RouterLink to="/login" class="text-white-50 text-decoration-none footer-link">Admin Login</RouterLink>
          </p>
        </div>
      </div>
    </div>
  </footer>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { RouterLink } from 'vue-router'
import { useSettingsStore } from '@/stores/settings'
import settingsApi from '@/services/settingsApi'

interface BusinessContent {
  contactPhone: string
  contactEmail: string
  contactAddress: string
  businessHours: string
  facebookUrl: string
  instagramUrl: string
  linkedInUrl: string
  twitterUrl: string
}

interface Service {
  id: number
  title: string
  description: string
  icon: string
  displayOrder: number
  isActive: boolean
}

const settingsStore = useSettingsStore()
const businessContent = ref<BusinessContent | null>(null)
const services = ref<Service[]>([])
const currentYear = computed(() => new Date().getFullYear())

const hasSocialLinks = computed(() => {
  return businessContent.value?.facebookUrl || 
         businessContent.value?.instagramUrl || 
         businessContent.value?.linkedInUrl || 
         businessContent.value?.twitterUrl
})

const loadBusinessContent = async () => {
  try {
    const response = await settingsApi.get('/BusinessContent')
    businessContent.value = response.data
  } catch (error) {
    console.error('Error loading business content:', error)
  }
}

const loadServices = async () => {
  try {
    const response = await settingsApi.get('/Services')
    services.value = response.data.filter((s: Service) => s.isActive)
  } catch (error) {
    console.error('Error loading services:', error)
  }
}

onMounted(() => {
  settingsStore.loadSettings()
  loadBusinessContent()
  loadServices()
})
</script>

<style scoped>
footer {
  margin-top: auto;
}

.social-icon {
  width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0;
  transition: all 0.3s ease;
}

.social-icon:hover {
  background-color: var(--bs-primary);
  border-color: var(--bs-primary);
  color: white;
  transform: translateY(-2px);
}

footer a.footer-link:hover {
  color: var(--bs-warning) !important;
  transition: color 0.3s ease;
}
</style>
