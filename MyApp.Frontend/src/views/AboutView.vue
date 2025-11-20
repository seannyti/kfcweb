<template>
  <div class="about-view">
    <!-- Hero Section -->
    <section class="hero-section text-white py-5">
      <div class="container py-5">
        <div class="row align-items-center">
          <div class="col-lg-6">
            <h1 class="display-4 fw-bold mb-4">{{ businessContent?.heroTitle || 'About Our Company' }}</h1>
            <p class="lead">{{ businessContent?.heroSubtitle || 'Building quality since day one.' }}</p>
          </div>
          <div class="col-lg-6 text-center">
            <img 
              v-if="businessContent?.heroImage" 
              :src="businessContent.heroImage" 
              alt="Hero" 
              class="img-fluid rounded shadow-lg"
            />
            <i v-else class="bi bi-building display-1"></i>
          </div>
        </div>
      </div>
    </section>

    <!-- About Section -->
    <section class="about-section py-5">
      <div class="container py-5">
        <div class="row align-items-center">
          <div class="col-lg-6 mb-4 mb-lg-0">
            <img 
              v-if="businessContent?.aboutImage" 
              :src="businessContent.aboutImage" 
              alt="About Us" 
              class="img-fluid rounded shadow"
            />
            <div v-else class="bg-light rounded d-flex align-items-center justify-content-center" style="height: 400px;">
              <i class="bi bi-image display-1 text-muted"></i>
            </div>
          </div>
          <div class="col-lg-6">
            <h2 class="mb-4">{{ businessContent?.aboutTitle || 'About Us' }}</h2>
            <p class="lead text-muted mb-4">
              {{ businessContent?.aboutDescription || 'We are a family-owned construction company dedicated to quality workmanship.' }}
            </p>
            <div class="mb-4">
              <h4 class="mb-3">Our Mission</h4>
              <p class="text-muted">
                {{ businessContent?.missionStatement || 'To deliver exceptional construction services with integrity, quality, and customer satisfaction.' }}
              </p>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Stats Section -->
    <section class="stats-section py-5 bg-light">
      <div class="container py-5">
        <div class="row g-4 text-center">
          <div class="col-md-3">
            <div class="stat-card">
              <i class="bi bi-calendar-check display-4 mb-3" style="color: var(--bs-primary);"></i>
              <h2 class="display-4 fw-bold mb-2">{{ businessContent?.yearsInBusiness || 0 }}+</h2>
              <p class="text-muted">Years in Business</p>
            </div>
          </div>
          <div class="col-md-3">
            <div class="stat-card">
              <i class="bi bi-building display-4 mb-3" style="color: var(--bs-success);"></i>
              <h2 class="display-4 fw-bold mb-2">{{ businessContent?.projectsCompleted || 0 }}+</h2>
              <p class="text-muted">Projects Completed</p>
            </div>
          </div>
          <div class="col-md-3">
            <div class="stat-card">
              <i class="bi bi-people-fill display-4 mb-3" style="color: var(--bs-warning);"></i>
              <h2 class="display-4 fw-bold mb-2">{{ businessContent?.happyClients || 0 }}+</h2>
              <p class="text-muted">Happy Clients</p>
            </div>
          </div>
          <div class="col-md-3">
            <div class="stat-card">
              <i class="bi bi-person-badge display-4 mb-3" style="color: var(--bs-info);"></i>
              <h2 class="display-4 fw-bold mb-2">{{ businessContent?.teamMembers || 0 }}+</h2>
              <p class="text-muted">Team Members</p>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Contact Info Section -->
    <section class="contact-info-section py-5">
      <div class="container py-5">
        <h2 class="text-center mb-5">Get In Touch</h2>
        <div class="row g-4">
          <div class="col-md-4">
            <div class="card border-0 shadow-sm h-100">
              <div class="card-body text-center p-4">
                <i class="bi bi-telephone-fill display-4 mb-3" style="color: var(--bs-primary);"></i>
                <h5>Phone</h5>
                <p class="text-muted mb-0">
                  <a :href="`tel:${businessContent?.contactPhone}`" class="text-decoration-none">
                    {{ businessContent?.contactPhone || '(555) 123-4567' }}
                  </a>
                </p>
              </div>
            </div>
          </div>
          <div class="col-md-4">
            <div class="card border-0 shadow-sm h-100">
              <div class="card-body text-center p-4">
                <i class="bi bi-envelope-fill display-4 mb-3" style="color: var(--bs-success);"></i>
                <h5>Email</h5>
                <p class="text-muted mb-0">
                  <a :href="`mailto:${businessContent?.contactEmail}`" class="text-decoration-none">
                    {{ businessContent?.contactEmail || 'info@example.com' }}
                  </a>
                </p>
              </div>
            </div>
          </div>
          <div class="col-md-4">
            <div class="card border-0 shadow-sm h-100">
              <div class="card-body text-center p-4">
                <i class="bi bi-geo-alt-fill display-4 mb-3" style="color: var(--bs-warning);"></i>
                <h5>Address</h5>
                <p class="text-muted mb-0">
                  {{ businessContent?.contactAddress || '123 Main St, City, ST 12345' }}
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Social Media Section -->
    <section class="social-section py-5 bg-light" v-if="hasSocialLinks">
      <div class="container text-center py-4">
        <h3 class="mb-4">Follow Us</h3>
        <div class="d-flex justify-content-center gap-3">
          <a 
            v-if="businessContent?.facebookUrl" 
            :href="businessContent.facebookUrl" 
            target="_blank" 
            class="btn btn-lg rounded-circle social-btn"
          >
            <i class="bi bi-facebook"></i>
          </a>
          <a 
            v-if="businessContent?.instagramUrl" 
            :href="businessContent.instagramUrl" 
            target="_blank" 
            class="btn btn-lg rounded-circle social-btn"
          >
            <i class="bi bi-instagram"></i>
          </a>
          <a 
            v-if="businessContent?.linkedInUrl" 
            :href="businessContent.linkedInUrl" 
            target="_blank" 
            class="btn btn-lg rounded-circle social-btn"
          >
            <i class="bi bi-linkedin"></i>
          </a>
          <a 
            v-if="businessContent?.twitterUrl" 
            :href="businessContent.twitterUrl" 
            target="_blank" 
            class="btn btn-lg rounded-circle social-btn"
          >
            <i class="bi bi-twitter-x"></i>
          </a>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import settingsApi from '@/services/settingsApi'

interface BusinessContent {
  aboutTitle: string
  aboutDescription: string
  aboutImage: string
  missionStatement: string
  yearsInBusiness: number
  projectsCompleted: number
  happyClients: number
  teamMembers: number
  contactPhone: string
  contactEmail: string
  contactAddress: string
  businessHours: string
  facebookUrl: string
  instagramUrl: string
  linkedInUrl: string
  twitterUrl: string
  heroTitle: string
  heroSubtitle: string
  heroImage: string
}

const businessContent = ref<BusinessContent | null>(null)
const loading = ref(true)

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
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadBusinessContent()
})
</script>

<style scoped>
.hero-section {
  background: linear-gradient(135deg, var(--bs-primary) 0%, color-mix(in srgb, var(--bs-primary) 80%, #000) 100%);
}

.stat-card {
  padding: 2rem;
  transition: transform 0.3s ease;
}

.stat-card:hover {
  transform: translateY(-10px);
}

.card {
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.card:hover {
  transform: translateY(-5px);
  box-shadow: 0 1rem 3rem rgba(0, 0, 0, 0.175) !important;
}

.social-btn {
  width: 60px;
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--bs-primary);
  color: white;
  transition: all 0.3s ease;
}

.social-btn:hover {
  transform: scale(1.1);
  background: color-mix(in srgb, var(--bs-primary) 80%, #000);
  color: white;
}
</style>
