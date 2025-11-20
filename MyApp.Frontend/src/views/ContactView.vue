<template>
  <div class="contact-view">
    <!-- Hero Section -->
    <section class="hero-section text-white py-5">
      <div class="container py-5 text-center">
        <h1 class="display-4 fw-bold mb-4">{{ businessContent?.quoteFormTitle || 'Request a Quote' }}</h1>
        <p class="lead">{{ businessContent?.quoteFormDescription || 'Tell us about your project and we\'ll get back to you soon.' }}</p>
      </div>
    </section>

    <!-- Contact Form & Info Section -->
    <section class="contact-section py-5">
      <div class="container py-5">
        <div class="row g-5">
          <!-- Quote Request Form -->
          <div class="col-lg-7">
            <div class="card border-0 shadow-sm">
              <div class="card-body p-4">
                <h3 class="mb-4">Get a Free Quote</h3>
                
                <div v-if="!businessContent?.enableQuoteRequests" class="alert alert-warning">
                  <i class="bi bi-exclamation-triangle-fill me-2"></i>
                  Quote requests are currently disabled. Please contact us directly.
                </div>

                <div v-else-if="submitSuccess" class="alert alert-success">
                  <i class="bi bi-check-circle-fill me-2"></i>
                  Thank you! Your quote request has been submitted. We'll get back to you soon.
                </div>

                <form v-else @submit.prevent="submitQuoteRequest">
                  <div class="row g-3 mb-3">
                    <div class="col-md-6">
                      <label class="form-label">Name *</label>
                      <input 
                        v-model="form.name" 
                        type="text" 
                        class="form-control" 
                        required
                      />
                    </div>
                    <div class="col-md-6">
                      <label class="form-label">Email *</label>
                      <input 
                        v-model="form.email" 
                        type="email" 
                        class="form-control" 
                        required
                      />
                    </div>
                  </div>

                  <div class="row g-3 mb-3">
                    <div class="col-md-6">
                      <label class="form-label">Phone *</label>
                      <input 
                        v-model="form.phone" 
                        type="tel" 
                        class="form-control" 
                        required
                      />
                    </div>
                    <div class="col-md-6">
                      <label class="form-label">Project Type *</label>
                      <select v-model="form.projectType" class="form-select" required>
                        <option value="">Select...</option>
                        <option value="Residential">Residential</option>
                        <option value="Commercial">Commercial</option>
                        <option value="Renovation">Renovation</option>
                        <option value="Other">Other</option>
                      </select>
                    </div>
                  </div>

                  <div class="mb-3">
                    <label class="form-label">Project Location *</label>
                    <input 
                      v-model="form.location" 
                      type="text" 
                      class="form-control" 
                      required
                    />
                  </div>

                  <div class="mb-3">
                    <label class="form-label">Project Description *</label>
                    <textarea 
                      v-model="form.description" 
                      class="form-control" 
                      rows="4" 
                      required
                      placeholder="Please describe your project in detail..."
                    ></textarea>
                  </div>

                  <div class="row g-3 mb-3">
                    <div class="col-md-6">
                      <label class="form-label">Timeline</label>
                      <select v-model="form.timeline" class="form-select">
                        <option value="">Select...</option>
                        <option value="ASAP">ASAP</option>
                        <option value="1-3 months">1-3 months</option>
                        <option value="3-6 months">3-6 months</option>
                        <option value="6+ months">6+ months</option>
                        <option value="Flexible">Flexible</option>
                      </select>
                    </div>
                    <div class="col-md-6">
                      <label class="form-label">Budget Range</label>
                      <select v-model="form.budget" class="form-select">
                        <option value="">Select...</option>
                        <option value="Under $10k">Under $10k</option>
                        <option value="$10k - $50k">$10k - $50k</option>
                        <option value="$50k - $100k">$50k - $100k</option>
                        <option value="$100k - $250k">$100k - $250k</option>
                        <option value="$250k+">$250k+</option>
                      </select>
                    </div>
                  </div>

                  <div v-if="submitError" class="alert alert-danger">
                    <i class="bi bi-exclamation-circle-fill me-2"></i>
                    {{ submitError }}
                  </div>

                  <button 
                    type="submit" 
                    class="btn btn-primary btn-lg w-100" 
                    :disabled="submitting"
                  >
                    <span v-if="submitting">
                      <span class="spinner-border spinner-border-sm me-2"></span>
                      Submitting...
                    </span>
                    <span v-else>
                      <i class="bi bi-send-fill me-2"></i>
                      Submit Quote Request
                    </span>
                  </button>
                </form>
              </div>
            </div>
          </div>

          <!-- Contact Information -->
          <div class="col-lg-5">
            <div class="card border-0 shadow-sm mb-4">
              <div class="card-body p-4">
                <h3 class="mb-4">Contact Information</h3>
                
                <div class="contact-item mb-4">
                  <div class="d-flex align-items-start">
                    <i class="bi bi-telephone-fill fs-4 me-3" style="color: var(--bs-primary);"></i>
                    <div>
                      <h6 class="mb-1">Phone</h6>
                      <a :href="`tel:${businessContent?.contactPhone}`" class="text-decoration-none">
                        {{ businessContent?.contactPhone || '(555) 123-4567' }}
                      </a>
                    </div>
                  </div>
                </div>

                <div class="contact-item mb-4">
                  <div class="d-flex align-items-start">
                    <i class="bi bi-envelope-fill fs-4 me-3" style="color: var(--bs-success);"></i>
                    <div>
                      <h6 class="mb-1">Email</h6>
                      <a :href="`mailto:${businessContent?.contactEmail}`" class="text-decoration-none">
                        {{ businessContent?.contactEmail || 'info@example.com' }}
                      </a>
                    </div>
                  </div>
                </div>

                <div class="contact-item mb-4">
                  <div class="d-flex align-items-start">
                    <i class="bi bi-geo-alt-fill fs-4 me-3" style="color: var(--bs-warning);"></i>
                    <div>
                      <h6 class="mb-1">Address</h6>
                      <p class="mb-0 text-muted">
                        {{ businessContent?.contactAddress || '123 Main St, City, ST 12345' }}
                      </p>
                    </div>
                  </div>
                </div>

                <div class="contact-item">
                  <div class="d-flex align-items-start">
                    <i class="bi bi-clock-fill fs-4 me-3" style="color: var(--bs-info);"></i>
                    <div>
                      <h6 class="mb-1">Business Hours</h6>
                      <p class="mb-0 text-muted">
                        {{ businessContent?.businessHours || 'Monday - Friday: 8:00 AM - 5:00 PM' }}
                      </p>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Social Media -->
            <div v-if="hasSocialLinks" class="card border-0 shadow-sm">
              <div class="card-body p-4">
                <h3 class="mb-4">Follow Us</h3>
                <div class="d-flex gap-3">
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
            </div>
          </div>
        </div>

        <!-- Map Section -->
        <div v-if="businessContent?.googleMapsUrl" class="row mt-5">
          <div class="col-12">
            <div class="card border-0 shadow-sm">
              <div class="card-body p-0">
                <iframe 
                  :src="businessContent.googleMapsUrl" 
                  width="100%" 
                  height="400" 
                  style="border:0;" 
                  allowfullscreen
                  loading="lazy"
                  referrerpolicy="no-referrer-when-downgrade"
                ></iframe>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import settingsApi from '@/services/settingsApi'

interface BusinessContent {
  quoteFormTitle: string
  quoteFormDescription: string
  enableQuoteRequests: boolean
  contactPhone: string
  contactEmail: string
  contactAddress: string
  businessHours: string
  googleMapsUrl: string
  facebookUrl: string
  instagramUrl: string
  linkedInUrl: string
  twitterUrl: string
}

const businessContent = ref<BusinessContent | null>(null)
const form = ref({
  name: '',
  email: '',
  phone: '',
  projectType: '',
  description: '',
  location: '',
  timeline: '',
  budget: ''
})

const submitting = ref(false)
const submitSuccess = ref(false)
const submitError = ref('')

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

const submitQuoteRequest = async () => {
  submitting.value = true
  submitError.value = ''
  
  try {
    await settingsApi.post('/QuoteRequests', {
      fullName: form.value.name,
      email: form.value.email,
      phone: form.value.phone,
      address: form.value.location,
      projectType: form.value.projectType,
      description: form.value.description,
      budget: form.value.budget,
      timeline: form.value.timeline
    })
    submitSuccess.value = true
    
    // Reset form
    form.value = {
      name: '',
      email: '',
      phone: '',
      projectType: '',
      description: '',
      location: '',
      timeline: '',
      budget: ''
    }
  } catch (error: any) {
    console.error('Error submitting quote request:', error)
    submitError.value = error.response?.data?.message || 'An error occurred while submitting your request. Please try again.'
  } finally {
    submitting.value = false
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

.contact-item {
  padding-bottom: 1rem;
  border-bottom: 1px solid #dee2e6;
}

.contact-item:last-child {
  border-bottom: none;
  padding-bottom: 0;
}

.social-btn {
  width: 50px;
  height: 50px;
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
