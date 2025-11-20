<template>
  <div class="business-content-admin">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2><i class="bi bi-building me-2"></i>Business Content</h2>
      <button @click="saveContent" class="btn btn-primary" :disabled="saving">
        <span v-if="saving">
          <span class="spinner-border spinner-border-sm me-2"></span>
          Saving...
        </span>
        <span v-else>
          <i class="bi bi-save me-2"></i>
          Save Changes
        </span>
      </button>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border" role="status" style="color: var(--bs-primary);">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>

    <div v-else>
      <!-- Hero Section -->
      <div class="card mb-4">
        <div class="card-header">
          <h5 class="mb-0"><i class="bi bi-star me-2"></i>Hero Section</h5>
        </div>
        <div class="card-body">
          <div class="mb-3">
            <label class="form-label">Hero Title</label>
            <input v-model="content.heroTitle" type="text" class="form-control" />
          </div>
          <div class="mb-3">
            <label class="form-label">Hero Subtitle</label>
            <textarea v-model="content.heroSubtitle" class="form-control" rows="2"></textarea>
          </div>
          <div class="mb-3">
            <label class="form-label">Hero Button Text</label>
            <input v-model="content.heroButtonText" type="text" class="form-control" />
          </div>
          <div class="mb-3">
            <label class="form-label">Hero Image URL</label>
            <input v-model="content.heroImage" type="text" class="form-control" placeholder="https://example.com/hero.jpg" />
          </div>
        </div>
      </div>

      <!-- About Section -->
      <div class="card mb-4">
        <div class="card-header">
          <h5 class="mb-0"><i class="bi bi-info-circle me-2"></i>About Section</h5>
        </div>
        <div class="card-body">
          <div class="mb-3">
            <label class="form-label">About Title</label>
            <input v-model="content.aboutTitle" type="text" class="form-control" />
          </div>
          <div class="mb-3">
            <label class="form-label">About Description</label>
            <textarea v-model="content.aboutDescription" class="form-control" rows="4"></textarea>
          </div>
          <div class="mb-3">
            <label class="form-label">About Image URL</label>
            <input v-model="content.aboutImage" type="text" class="form-control" placeholder="https://example.com/about.jpg" />
          </div>
          <div class="mb-3">
            <label class="form-label">Mission Statement</label>
            <textarea v-model="content.missionStatement" class="form-control" rows="3"></textarea>
          </div>
        </div>
      </div>

      <!-- Stats Section -->
      <div class="card mb-4">
        <div class="card-header">
          <h5 class="mb-0"><i class="bi bi-bar-chart me-2"></i>Statistics</h5>
        </div>
        <div class="card-body">
          <div class="row">
            <div class="col-md-3 mb-3">
              <label class="form-label">Years in Business</label>
              <input v-model.number="content.yearsInBusiness" type="number" class="form-control" />
            </div>
            <div class="col-md-3 mb-3">
              <label class="form-label">Projects Completed</label>
              <input v-model.number="content.projectsCompleted" type="number" class="form-control" />
            </div>
            <div class="col-md-3 mb-3">
              <label class="form-label">Happy Clients</label>
              <input v-model.number="content.happyClients" type="number" class="form-control" />
            </div>
            <div class="col-md-3 mb-3">
              <label class="form-label">Team Members</label>
              <input v-model.number="content.teamMembers" type="number" class="form-control" />
            </div>
          </div>
        </div>
      </div>

      <!-- Contact Information -->
      <div class="card mb-4">
        <div class="card-header">
          <h5 class="mb-0"><i class="bi bi-telephone me-2"></i>Contact Information</h5>
        </div>
        <div class="card-body">
          <div class="row">
            <div class="col-md-6 mb-3">
              <label class="form-label">Phone Number</label>
              <input v-model="content.contactPhone" type="text" class="form-control" />
            </div>
            <div class="col-md-6 mb-3">
              <label class="form-label">Email Address</label>
              <input v-model="content.contactEmail" type="email" class="form-control" />
            </div>
          </div>
          <div class="mb-3">
            <label class="form-label">Physical Address</label>
            <input v-model="content.contactAddress" type="text" class="form-control" />
          </div>
          <div class="mb-3">
            <label class="form-label">Business Hours</label>
            <input v-model="content.businessHours" type="text" class="form-control" placeholder="Monday - Friday: 8:00 AM - 5:00 PM" />
          </div>
          <div class="mb-3">
            <label class="form-label">Google Maps Embed URL</label>
            <input v-model="content.googleMapsUrl" type="text" class="form-control" placeholder="https://www.google.com/maps/embed?pb=..." />
            <small class="text-muted">Get this from Google Maps → Share → Embed a map</small>
          </div>
        </div>
      </div>

      <!-- Social Media -->
      <div class="card mb-4">
        <div class="card-header">
          <h5 class="mb-0"><i class="bi bi-share me-2"></i>Social Media</h5>
        </div>
        <div class="card-body">
          <div class="row">
            <div class="col-md-6 mb-3">
              <label class="form-label">Facebook URL</label>
              <input v-model="content.facebookUrl" type="text" class="form-control" placeholder="https://facebook.com/yourpage" />
            </div>
            <div class="col-md-6 mb-3">
              <label class="form-label">Instagram URL</label>
              <input v-model="content.instagramUrl" type="text" class="form-control" placeholder="https://instagram.com/yourpage" />
            </div>
            <div class="col-md-6 mb-3">
              <label class="form-label">LinkedIn URL</label>
              <input v-model="content.linkedInUrl" type="text" class="form-control" placeholder="https://linkedin.com/company/yourpage" />
            </div>
            <div class="col-md-6 mb-3">
              <label class="form-label">Twitter/X URL</label>
              <input v-model="content.twitterUrl" type="text" class="form-control" placeholder="https://twitter.com/yourpage" />
            </div>
          </div>
        </div>
      </div>

      <!-- Quote Form Settings -->
      <div class="card mb-4">
        <div class="card-header">
          <h5 class="mb-0"><i class="bi bi-envelope-paper me-2"></i>Quote Request Settings</h5>
        </div>
        <div class="card-body">
          <div class="mb-3">
            <div class="form-check form-switch">
              <input v-model="content.enableQuoteRequests" class="form-check-input" type="checkbox" id="enableQuotes">
              <label class="form-check-label" for="enableQuotes">
                Enable Quote Requests
              </label>
            </div>
          </div>
          <div class="mb-3">
            <label class="form-label">Quote Form Title</label>
            <input v-model="content.quoteFormTitle" type="text" class="form-control" />
          </div>
          <div class="mb-3">
            <label class="form-label">Quote Form Description</label>
            <textarea v-model="content.quoteFormDescription" class="form-control" rows="2"></textarea>
          </div>
          <div class="mb-3">
            <label class="form-label">Notification Email (for new quotes)</label>
            <input v-model="content.quoteNotificationEmail" type="email" class="form-control" />
          </div>
        </div>
      </div>

      <div v-if="saveSuccess" class="alert alert-success">
        <i class="bi bi-check-circle-fill me-2"></i>
        Business content updated successfully!
      </div>

      <div v-if="saveError" class="alert alert-danger">
        <i class="bi bi-exclamation-circle-fill me-2"></i>
        {{ saveError }}
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import settingsApi from '@/services/settingsApi'
import { useToast } from 'vue-toastification'

const toast = useToast()

const content = ref({
  heroTitle: '',
  heroSubtitle: '',
  heroButtonText: '',
  heroImage: '',
  aboutTitle: '',
  aboutDescription: '',
  aboutImage: '',
  missionStatement: '',
  yearsInBusiness: 20,
  projectsCompleted: 150,
  happyClients: 200,
  teamMembers: 15,
  contactPhone: '',
  contactEmail: '',
  contactAddress: '',
  businessHours: '',
  googleMapsUrl: '',
  facebookUrl: '',
  instagramUrl: '',
  linkedInUrl: '',
  twitterUrl: '',
  quoteFormTitle: '',
  quoteFormDescription: '',
  enableQuoteRequests: true,
  quoteNotificationEmail: ''
})

const loading = ref(true)
const saving = ref(false)
const saveSuccess = ref(false)
const saveError = ref('')

const loadContent = async () => {
  try {
      const response = await settingsApi.get('/BusinessContent')
    content.value = response.data
  } catch (error) {
    console.error('Error loading business content:', error)
  } finally {
    loading.value = false
  }
}

const saveContent = async () => {
  saving.value = true
  saveSuccess.value = false
  saveError.value = ''

  try {
    await settingsApi.put('/BusinessContent', content.value)
    toast.success('Business content updated successfully!')
    saveSuccess.value = false
  } catch (error: any) {
    console.error('Error saving business content:', error)
    const errorMsg = error.response?.data?.message || 'Failed to save business content'
    toast.error(errorMsg)
    saveError.value = errorMsg
  } finally {
    saving.value = false
  }
}

onMounted(() => {
  loadContent()
})
</script>

<style scoped>
.card-header {
  background-color: var(--bs-light);
}
</style>
