<template>
  <div class="projects-view">
    <!-- Hero Section -->
    <section class="hero-section text-white py-5">
      <div class="container py-5 text-center">
        <h1 class="display-4 fw-bold mb-4">Our Projects</h1>
        <p class="lead">Showcasing our finest work and craftsmanship</p>
      </div>
    </section>

    <!-- Filter Section -->
    <section class="filter-section py-4 bg-light">
      <div class="container">
        <div class="d-flex justify-content-center gap-2 flex-wrap">
          <button 
            @click="selectedCategory = 'All'" 
            :class="['btn', selectedCategory === 'All' ? 'btn-primary' : 'btn-outline-primary']"
          >
            All Projects
          </button>
          <button 
            @click="selectedCategory = 'Residential'" 
            :class="['btn', selectedCategory === 'Residential' ? 'btn-primary' : 'btn-outline-primary']"
          >
            Residential
          </button>
          <button 
            @click="selectedCategory = 'Commercial'" 
            :class="['btn', selectedCategory === 'Commercial' ? 'btn-primary' : 'btn-outline-primary']"
          >
            Commercial
          </button>
          <button 
            @click="selectedCategory = 'Renovation'" 
            :class="['btn', selectedCategory === 'Renovation' ? 'btn-primary' : 'btn-outline-primary']"
          >
            Renovation
          </button>
        </div>
      </div>
    </section>

    <!-- Projects Grid -->
    <section class="projects-section py-5">
      <div class="container py-5">
        <div v-if="loading" class="text-center py-5">
          <div class="spinner-border" role="status" style="color: var(--bs-primary);">
            <span class="visually-hidden">Loading...</span>
          </div>
        </div>

        <div v-else-if="filteredProjects.length === 0" class="text-center py-5">
          <i class="bi bi-building display-1 text-muted mb-3"></i>
          <p class="text-muted">No projects found in this category.</p>
        </div>

        <div v-else class="row g-4">
          <div v-for="project in filteredProjects" :key="project.id" class="col-md-6 col-lg-4">
            <div class="card h-100 border-0 shadow-sm project-card">
              <div class="project-image-container position-relative">
                <img 
                  :src="project.imageUrl || 'https://via.placeholder.com/400x300'" 
                  :alt="project.title" 
                  class="card-img-top"
                />
                <div class="project-overlay">
                  <span class="badge bg-primary">{{ project.category }}</span>
                  <span v-if="project.isFeatured" class="badge bg-warning text-dark ms-2">
                    <i class="bi bi-star-fill"></i> Featured
                  </span>
                </div>
              </div>
              <div class="card-body p-4">
                <h4 class="card-title mb-3">{{ project.title }}</h4>
                <p class="card-text text-muted mb-3">{{ project.description }}</p>
                <div class="d-flex align-items-center text-muted small mb-2">
                  <i class="bi bi-geo-alt-fill me-2"></i>
                  <span>{{ project.location }}</span>
                </div>
                <div v-if="project.completionDate" class="d-flex align-items-center text-muted small">
                  <i class="bi bi-calendar-check me-2"></i>
                  <span>Completed: {{ formatDate(project.completionDate) }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- CTA Section -->
    <section class="cta-section py-5 bg-light">
      <div class="container text-center py-5">
        <h2 class="display-5 mb-4">Let's Build Your Vision</h2>
        <p class="lead mb-4">
          Ready to start your next construction project? Contact us for a free consultation.
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
import { ref, computed, onMounted } from 'vue'
import { RouterLink } from 'vue-router'
import axios from 'axios'
import settingsApi from '@/services/settingsApi'

interface Project {
  id: number
  title: string
  description: string
  category: string
  location: string
  imageUrl: string
  galleryImages: string[]
  completionDate: string | null
  isFeatured: boolean
  isActive: boolean
  displayOrder: number
}

const projects = ref<Project[]>([])
const loading = ref(true)
const selectedCategory = ref('All')

const filteredProjects = computed(() => {
  if (selectedCategory.value === 'All') {
    return projects.value
  }
  return projects.value.filter(p => p.category === selectedCategory.value)
})

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('en-US', { month: 'long', year: 'numeric' })
}

const loadProjects = async () => {
  try {
    const response = await settingsApi.get('/Projects')
    projects.value = response.data
  } catch (error) {
    console.error('Error loading projects:', error)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadProjects()
})
</script>

<style scoped>
.hero-section {
  background: linear-gradient(135deg, var(--bs-primary) 0%, color-mix(in srgb, var(--bs-primary) 80%, #000) 100%);
}

.project-card {
  transition: transform 0.3s ease, box-shadow 0.3s ease;
  overflow: hidden;
}

.project-card:hover {
  transform: translateY(-10px);
  box-shadow: 0 1rem 3rem rgba(0, 0, 0, 0.175) !important;
}

.project-image-container {
  height: 250px;
  overflow: hidden;
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
