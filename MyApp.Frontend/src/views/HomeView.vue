<template>
  <div class="home-view">
    <!-- Hero Section -->
    <section class="hero bg-construction text-white py-5">
      <div class="container py-5">
        <div class="row align-items-center">
          <div class="col-lg-6">
            <h1 class="display-4 fw-bold mb-4">
              {{ businessContent?.heroTitle || 'Building Your Dreams Into Reality' }}
            </h1>
            <p class="lead mb-4">
              {{ businessContent?.heroSubtitle || 'Professional construction services with quality craftsmanship and attention to detail.' }}
            </p>
            <div class="d-flex gap-3 flex-wrap">
              <RouterLink 
                to="/contact" 
                class="btn btn-warning btn-lg text-dark fw-bold"
              >
                <i class="bi bi-envelope-fill me-2"></i>
                {{ businessContent?.heroButtonText || 'Get a Free Quote' }}
              </RouterLink>
              <RouterLink 
                to="/projects" 
                class="btn btn-outline-light btn-lg fw-bold"
              >
                <i class="bi bi-images me-2"></i>
                View Our Work
              </RouterLink>
            </div>
          </div>
          <div class="col-lg-6 text-center mt-4 mt-lg-0">
            <img 
              v-if="businessContent?.heroImage" 
              :src="businessContent.heroImage" 
              alt="Hero" 
              class="img-fluid rounded shadow-lg"
            />
            <i v-else class="bi bi-building display-1 construction-icon"></i>
          </div>
        </div>
      </div>
    </section>

    <!-- Stats Section -->
    <section class="stats-section py-5 bg-light">
      <div class="container py-4">
        <div class="row g-4 text-center">
          <div class="col-md-3 col-6">
            <div class="stat-card">
              <i class="bi bi-calendar-check display-4 mb-2" style="color: var(--bs-primary);"></i>
              <h2 class="display-5 fw-bold mb-1">{{ businessContent?.yearsInBusiness || 20 }}+</h2>
              <p class="text-muted mb-0">Years Experience</p>
            </div>
          </div>
          <div class="col-md-3 col-6">
            <div class="stat-card">
              <i class="bi bi-building display-4 mb-2" style="color: var(--bs-success);"></i>
              <h2 class="display-5 fw-bold mb-1">{{ businessContent?.projectsCompleted || 150 }}+</h2>
              <p class="text-muted mb-0">Projects Completed</p>
            </div>
          </div>
          <div class="col-md-3 col-6">
            <div class="stat-card">
              <i class="bi bi-people-fill display-4 mb-2" style="color: var(--bs-warning);"></i>
              <h2 class="display-5 fw-bold mb-1">{{ businessContent?.happyClients || 200 }}+</h2>
              <p class="text-muted mb-0">Happy Clients</p>
            </div>
          </div>
          <div class="col-md-3 col-6">
            <div class="stat-card">
              <i class="bi bi-person-badge display-4 mb-2" style="color: var(--bs-info);"></i>
              <h2 class="display-5 fw-bold mb-1">{{ businessContent?.teamMembers || 15 }}+</h2>
              <p class="text-muted mb-0">Team Members</p>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Services Preview -->
    <section class="services-preview py-5">
      <div class="container py-5">
        <h2 class="text-center mb-2">Our Services</h2>
        <p class="text-center text-muted mb-5">Expert construction services for your every need</p>
        
        <div v-if="loadingServices" class="text-center">
          <div class="spinner-border" role="status" style="color: var(--bs-primary);">
            <span class="visually-hidden">Loading...</span>
          </div>
        </div>

        <div v-else-if="services.length > 0" class="row g-4">
          <div v-for="service in services.slice(0, 3)" :key="service.id" class="col-md-4">
            <div class="card h-100 border-0 shadow-sm">
              <div class="card-body text-center p-4">
                <i :class="`bi ${service.icon} display-4 mb-3`" style="color: var(--bs-primary);"></i>
                <h4>{{ service.title }}</h4>
                <p class="text-muted">{{ service.description.substring(0, 120) }}{{ service.description.length > 120 ? '...' : '' }}</p>
              </div>
            </div>
          </div>
        </div>

        <div class="text-center mt-5">
          <RouterLink to="/services" class="btn btn-primary btn-lg">
            View All Services
            <i class="bi bi-arrow-right ms-2"></i>
          </RouterLink>
        </div>
      </div>
    </section>

    <!-- Featured Projects -->
    <section class="featured-projects py-5 bg-light">
      <div class="container py-5">
        <h2 class="text-center mb-2">Featured Projects</h2>
        <p class="text-center text-muted mb-5">See some of our recent completed work</p>
        
        <div v-if="loadingProjects" class="text-center">
          <div class="spinner-border" role="status" style="color: var(--bs-primary);">
            <span class="visually-hidden">Loading...</span>
          </div>
        </div>

        <div v-else-if="featuredProjects.length > 0" class="row g-4">
          <div v-for="project in featuredProjects.slice(0, 3)" :key="project.id" class="col-md-4">
            <div class="card h-100 border-0 shadow-sm project-card">
              <div class="project-image-container">
                <img 
                  :src="project.imageUrl || 'https://via.placeholder.com/400x300'" 
                  :alt="project.title" 
                  class="card-img-top"
                />
                <div class="project-overlay">
                  <span class="badge bg-primary">{{ project.category }}</span>
                </div>
              </div>
              <div class="card-body p-4">
                <h4 class="card-title mb-3">{{ project.title }}</h4>
                <p class="card-text text-muted">{{ project.description.substring(0, 100) }}{{ project.description.length > 100 ? '...' : '' }}</p>
              </div>
            </div>
          </div>
        </div>

        <div class="text-center mt-5">
          <RouterLink to="/projects" class="btn btn-primary btn-lg">
            View All Projects
            <i class="bi bi-arrow-right ms-2"></i>
          </RouterLink>
        </div>
      </div>
    </section>

    <!-- CTA Section -->
    <section class="cta py-5 bg-construction text-white">
      <div class="container text-center py-5">
        <h2 class="display-5 mb-4">Ready to Start Your Project?</h2>
        <p class="lead mb-4">
          Contact us today for a free consultation and quote. We're here to bring your vision to life.
        </p>
        <RouterLink to="/contact" class="btn btn-warning btn-lg text-dark fw-bold">
          <i class="bi bi-envelope-fill me-2"></i>
          Request a Free Quote
        </RouterLink>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { RouterLink } from 'vue-router'
import settingsApi from '@/services/settingsApi'

interface BusinessContent {
  heroTitle: string
  heroSubtitle: string
  heroButtonText: string
  heroImage: string
  yearsInBusiness: number
  projectsCompleted: number
  happyClients: number
  teamMembers: number
}

interface Service {
  id: number
  title: string
  description: string
  icon: string
}

interface Project {
  id: number
  title: string
  description: string
  category: string
  imageUrl: string
}

const businessContent = ref<BusinessContent | null>(null)
const services = ref<Service[]>([])
const featuredProjects = ref<Project[]>([])
const loadingServices = ref(true)
const loadingProjects = ref(true)

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
    services.value = response.data
  } catch (error) {
    console.error('Error loading services:', error)
  } finally {
    loadingServices.value = false
  }
}

const loadFeaturedProjects = async () => {
  try {
    const response = await settingsApi.get('/Projects/featured')
    featuredProjects.value = response.data
  } catch (error) {
    console.error('Error loading featured projects:', error)
  } finally {
    loadingProjects.value = false
  }
}

onMounted(() => {
  loadBusinessContent()
  loadServices()
  loadFeaturedProjects()
})
</script>

<style scoped>
.bg-construction {
  background: linear-gradient(135deg, var(--bs-primary) 0%, color-mix(in srgb, var(--bs-primary) 80%, #000) 100%);
}

.construction-icon {
  animation: float 3s ease-in-out infinite;
}

@keyframes float {
  0%, 100% {
    transform: translateY(0);
  }
  50% {
    transform: translateY(-20px);
  }
}

.stat-card {
  transition: transform 0.3s ease;
}

.stat-card:hover {
  transform: translateY(-5px);
}

.card {
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.card:hover {
  transform: translateY(-5px);
  box-shadow: 0 1rem 3rem rgba(0, 0, 0, 0.175) !important;
}

.project-card {
  overflow: hidden;
}

.project-image-container {
  height: 250px;
  overflow: hidden;
  position: relative;
}

.project-image-container img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s ease;
}

.project-card:hover .project-image-container img {
  transform: scale(1.1);
}

.project-overlay {
  position: absolute;
  top: 10px;
  left: 10px;
  z-index: 1;
}
</style>
