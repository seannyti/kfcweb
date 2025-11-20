<template>
  <div class="services-view">
    <!-- Hero Section -->
    <section class="hero-section text-white py-5">
      <div class="container py-5 text-center">
        <h1 class="display-4 fw-bold mb-4">Our Services</h1>
        <p class="lead">Quality construction services tailored to your needs</p>
      </div>
    </section>

    <!-- Services Grid -->
    <section class="services-section py-5">
      <div class="container py-5">
        <div v-if="loading" class="text-center py-5">
          <div class="spinner-border" role="status" style="color: var(--bs-primary);">
            <span class="visually-hidden">Loading...</span>
          </div>
        </div>

        <div v-else-if="services.length === 0" class="text-center py-5">
          <i class="bi bi-tools display-1 text-muted mb-3"></i>
          <p class="text-muted">No services available at this time.</p>
        </div>

        <div v-else class="row g-4">
          <div v-for="service in services" :key="service.id" class="col-md-6 col-lg-4">
            <div class="card h-100 border-0 shadow-sm service-card">
              <div v-if="service.image" class="service-image-container">
                <img :src="service.image" :alt="service.title" class="card-img-top" />
              </div>
              <div class="card-body p-4">
                <div class="text-center mb-3">
                  <i :class="`bi ${service.icon} display-4`" style="color: var(--bs-primary);"></i>
                </div>
                <h4 class="card-title text-center mb-3">{{ service.title }}</h4>
                <p class="card-text text-muted">{{ service.description }}</p>
              </div>
              <div class="card-footer bg-transparent border-0 p-4 pt-0">
                <RouterLink to="/contact" class="btn btn-outline-primary w-100">
                  Request Quote
                  <i class="bi bi-arrow-right ms-2"></i>
                </RouterLink>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- CTA Section -->
    <section class="cta-section py-5 bg-light">
      <div class="container text-center py-5">
        <h2 class="display-5 mb-4">Ready to Get Started?</h2>
        <p class="lead mb-4">
          Contact us today for a free consultation and quote on your construction project.
        </p>
        <RouterLink to="/contact" class="btn btn-primary btn-lg">
          <i class="bi bi-envelope-fill me-2"></i>
          Request a Quote
        </RouterLink>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { RouterLink } from 'vue-router'
import settingsApi from '@/services/settingsApi'

interface Service {
  id: number
  title: string
  description: string
  icon: string
  image: string
  displayOrder: number
  isActive: boolean
}

const services = ref<Service[]>([])
const loading = ref(true)

const loadServices = async () => {
  try {
    const response = await settingsApi.get('/Services')
    services.value = response.data
  } catch (error) {
    console.error('Error loading services:', error)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadServices()
})
</script>

<style scoped>
.hero-section {
  background: linear-gradient(135deg, var(--bs-primary) 0%, color-mix(in srgb, var(--bs-primary) 80%, #000) 100%);
}

.service-card {
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.service-card:hover {
  transform: translateY(-10px);
  box-shadow: 0 1rem 3rem rgba(0, 0, 0, 0.175) !important;
}

.service-image-container {
  height: 200px;
  overflow: hidden;
}

.service-image-container img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s ease;
}

.service-card:hover .service-image-container img {
  transform: scale(1.1);
}
</style>
