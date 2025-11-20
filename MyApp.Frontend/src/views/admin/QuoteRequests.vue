<template>
  <div class="quote-requests">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2><i class="bi bi-envelope-paper me-2"></i>Quote Requests</h2>
      <button @click="loadQuoteRequests" class="btn btn-outline-primary">
        <i class="bi bi-arrow-clockwise me-2"></i>
        Refresh
      </button>
    </div>

    <!-- Stats Cards -->
    <div v-if="stats" class="row g-3 mb-4">
      <div class="col-md-2">
        <div class="card text-center">
          <div class="card-body">
            <h3 class="mb-1">{{ stats.total }}</h3>
            <small class="text-muted">Total</small>
          </div>
        </div>
      </div>
      <div class="col-md-2">
        <div class="card text-center border-warning">
          <div class="card-body">
            <h3 class="mb-1 text-warning">{{ stats.newRequests }}</h3>
            <small class="text-muted">New</small>
          </div>
        </div>
      </div>
      <div class="col-md-2">
        <div class="card text-center border-info">
          <div class="card-body">
            <h3 class="mb-1 text-info">{{ stats.contacted }}</h3>
            <small class="text-muted">Contacted</small>
          </div>
        </div>
      </div>
      <div class="col-md-2">
        <div class="card text-center border-primary">
          <div class="card-body">
            <h3 class="mb-1 text-primary">{{ stats.quoted }}</h3>
            <small class="text-muted">Quoted</small>
          </div>
        </div>
      </div>
      <div class="col-md-2">
        <div class="card text-center border-success">
          <div class="card-body">
            <h3 class="mb-1 text-success">{{ stats.won }}</h3>
            <small class="text-muted">Won</small>
          </div>
        </div>
      </div>
      <div class="col-md-2">
        <div class="card text-center border-secondary">
          <div class="card-body">
            <h3 class="mb-1 text-secondary">{{ stats.lost }}</h3>
            <small class="text-muted">Lost</small>
          </div>
        </div>
      </div>
    </div>

    <!-- Filter Buttons -->
    <div class="mb-3">
      <div class="btn-group" role="group">
        <button 
          v-for="status in statuses" 
          :key="status" 
          @click="filterStatus = status"
          :class="['btn', filterStatus === status ? 'btn-primary' : 'btn-outline-primary']"
        >
          {{ status }}
        </button>
      </div>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border" role="status" style="color: var(--bs-primary);">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>

    <div v-else>
      <div class="table-responsive">
        <table class="table table-hover">
          <thead>
            <tr>
              <th>Date</th>
              <th>Name</th>
              <th>Email</th>
              <th>Phone</th>
              <th>Project Type</th>
              <th>Location</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="quote in filteredQuotes" :key="quote.id">
              <td>{{ formatDate(quote.createdAt) }}</td>
              <td>{{ quote.name }}</td>
              <td><a :href="`mailto:${quote.email}`">{{ quote.email }}</a></td>
              <td><a :href="`tel:${quote.phone}`">{{ quote.phone }}</a></td>
              <td><span class="badge bg-info">{{ quote.projectType }}</span></td>
              <td>{{ quote.location }}</td>
              <td>
                <span :class="['badge', getStatusClass(quote.status)]">
                  {{ quote.status }}
                </span>
              </td>
              <td>
                <button @click="viewQuote(quote)" class="btn btn-sm btn-outline-primary">
                  <i class="bi bi-eye"></i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- View/Edit Modal -->
    <div v-if="showModal" class="modal show d-block" tabindex="-1">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Quote Request Details</h5>
            <button type="button" class="btn-close" @click="closeModal"></button>
          </div>
          <div class="modal-body">
            <div class="row mb-3">
              <div class="col-md-6">
                <strong>Name:</strong> {{ currentQuote.name }}
              </div>
              <div class="col-md-6">
                <strong>Date:</strong> {{ formatDate(currentQuote.createdAt) }}
              </div>
            </div>
            <div class="row mb-3">
              <div class="col-md-6">
                <strong>Email:</strong> <a :href="`mailto:${currentQuote.email}`">{{ currentQuote.email }}</a>
              </div>
              <div class="col-md-6">
                <strong>Phone:</strong> <a :href="`tel:${currentQuote.phone}`">{{ currentQuote.phone }}</a>
              </div>
            </div>
            <div class="row mb-3">
              <div class="col-md-4">
                <strong>Project Type:</strong> {{ currentQuote.projectType }}
              </div>
              <div class="col-md-4">
                <strong>Timeline:</strong> {{ currentQuote.timeline || 'Not specified' }}
              </div>
              <div class="col-md-4">
                <strong>Budget:</strong> {{ currentQuote.budget || 'Not specified' }}
              </div>
            </div>
            <div class="mb-3">
              <strong>Location:</strong> {{ currentQuote.location }}
            </div>
            <div class="mb-3">
              <strong>Project Description:</strong>
              <p class="mt-2">{{ currentQuote.description }}</p>
            </div>
            <hr>
            <div class="mb-3">
              <label class="form-label"><strong>Status</strong></label>
              <select v-model="currentQuote.status" class="form-select">
                <option value="New">New</option>
                <option value="Contacted">Contacted</option>
                <option value="Quoted">Quoted</option>
                <option value="Won">Won</option>
                <option value="Lost">Lost</option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label"><strong>Internal Notes</strong></label>
              <textarea v-model="currentQuote.notes" class="form-control" rows="4" placeholder="Add notes about this quote request..."></textarea>
            </div>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-danger" @click="deleteQuote" v-if="currentQuote.id">
              <i class="bi bi-trash me-2"></i>
              Delete
            </button>
            <button type="button" class="btn btn-secondary" @click="closeModal">Close</button>
            <button type="button" class="btn btn-primary" @click="updateQuote" :disabled="saving">
              <span v-if="saving">
                <span class="spinner-border spinner-border-sm me-2"></span>
                Saving...
              </span>
              <span v-else>Save Changes</span>
            </button>
          </div>
        </div>
      </div>
    </div>
    <div v-if="showModal" class="modal-backdrop show"></div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import settingsApi from '@/services/settingsApi'
import { useToast } from 'vue-toastification'

const toast = useToast()

interface QuoteRequest {
  id?: number
  name: string
  email: string
  phone: string
  projectType: string
  description: string
  location: string
  timeline: string
  budget: string
  status: string
  notes: string
  createdAt: string
  updatedAt: string
}

interface Stats {
  total: number
  newRequests: number
  contacted: number
  quoted: number
  won: number
  lost: number
  conversionRate: number
}

const quotes = ref<QuoteRequest[]>([])
const stats = ref<Stats | null>(null)
const loading = ref(true)
const saving = ref(false)
const showModal = ref(false)
const filterStatus = ref('All')
const statuses = ['All', 'New', 'Contacted', 'Quoted', 'Won', 'Lost']
const currentQuote = ref<QuoteRequest>({
  name: '',
  email: '',
  phone: '',
  projectType: '',
  description: '',
  location: '',
  timeline: '',
  budget: '',
  status: 'New',
  notes: '',
  createdAt: '',
  updatedAt: ''
})

const filteredQuotes = computed(() => {
  if (filterStatus.value === 'All') return quotes.value
  return quotes.value.filter(q => q.status === filterStatus.value)
})

const getStatusClass = (status: string) => {
  const classes: Record<string, string> = {
    'New': 'bg-warning text-dark',
    'Contacted': 'bg-info',
    'Quoted': 'bg-primary',
    'Won': 'bg-success',
    'Lost': 'bg-secondary'
  }
  return classes[status] || 'bg-secondary'
}

const formatDate = (dateString: string) => {
  if (!dateString) return ''
  const date = new Date(dateString)
  return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric', hour: '2-digit', minute: '2-digit' })
}

const loadQuoteRequests = async () => {
  loading.value = true
  try {
    const [quotesRes, statsRes] = await Promise.all([
      settingsApi.get('/QuoteRequests'),
      settingsApi.get('/QuoteRequests/admin/stats')
    ])
    quotes.value = quotesRes.data
    stats.value = statsRes.data
  } catch (error) {
    console.error('Error loading quote requests:', error)
  } finally {
    loading.value = false
  }
}

const viewQuote = (quote: QuoteRequest) => {
  currentQuote.value = { ...quote }
  showModal.value = true
}

const updateQuote = async () => {
  if (!currentQuote.value.id) return
  
  saving.value = true
  try {
    await settingsApi.put(`/QuoteRequests/${currentQuote.value.id}`, {
      status: currentQuote.value.status,
      notes: currentQuote.value.notes
    }, {
      withCredentials: true
    })
    toast.success('Quote request updated successfully!')
    await loadQuoteRequests()
    closeModal()
  } catch (error: any) {
    console.error('Error updating quote request:', error)
    toast.error(error.response?.data?.message || 'Failed to update quote request')
  } finally {
    saving.value = false
  }
}

const deleteQuote = async () => {
  if (!currentQuote.value.id || !confirm('Are you sure you want to delete this quote request?')) return

  try {
    await settingsApi.delete(`/QuoteRequests/${currentQuote.value.id}`, {
      withCredentials: true
    })
    toast.success('Quote request deleted successfully!')
    await loadQuoteRequests()
    closeModal()
  } catch (error: any) {
    console.error('Error deleting quote request:', error)
    toast.error(error.response?.data?.message || 'Failed to delete quote request')
  }
}

const closeModal = () => {
  showModal.value = false
  currentQuote.value = {
    name: '',
    email: '',
    phone: '',
    projectType: '',
    description: '',
    location: '',
    timeline: '',
    budget: '',
    status: 'New',
    notes: '',
    createdAt: '',
    updatedAt: ''
  }
}

onMounted(() => {
  loadQuoteRequests()
})
</script>

<style scoped>
.modal {
  background: rgba(0, 0, 0, 0.5);
}

.card {
  transition: transform 0.2s;
}

.card:hover {
  transform: translateY(-2px);
}
</style>
