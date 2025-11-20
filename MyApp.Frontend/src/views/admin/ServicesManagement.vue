<template>
  <div class="services-management">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2><i class="bi bi-tools me-2"></i>Services Management</h2>
      <button @click="showAddModal = true" class="btn btn-primary">
        <i class="bi bi-plus-circle me-2"></i>
        Add Service
      </button>
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
              <th>Order</th>
              <th>Icon</th>
              <th>Title</th>
              <th>Description</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="service in services" :key="service.id">
              <td>{{ service.displayOrder }}</td>
              <td><i :class="`bi ${service.icon} fs-4`"></i></td>
              <td>{{ service.title }}</td>
              <td>{{ service.description.substring(0, 80) }}...</td>
              <td>
                <span :class="['badge', service.isActive ? 'bg-success' : 'bg-secondary']">
                  {{ service.isActive ? 'Active' : 'Inactive' }}
                </span>
              </td>
              <td>
                <button @click="editService(service)" class="btn btn-sm btn-outline-primary me-2">
                  <i class="bi bi-pencil"></i>
                </button>
                <button @click="service.id && deleteService(service.id)" class="btn btn-sm btn-outline-danger">
                  <i class="bi bi-trash"></i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Add/Edit Modal -->
    <div v-if="showAddModal || showEditModal" class="modal show d-block" tabindex="-1">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ showEditModal ? 'Edit Service' : 'Add Service' }}</h5>
            <button type="button" class="btn-close" @click="closeModal"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Title</label>
              <input v-model="currentService.title" type="text" class="form-control" required />
            </div>
            <div class="mb-3">
              <label class="form-label">Description</label>
              <textarea v-model="currentService.description" class="form-control" rows="4" required></textarea>
            </div>
            <div class="mb-3">
              <label class="form-label">Icon (Bootstrap Icon class)</label>
              <input v-model="currentService.icon" type="text" class="form-control" placeholder="bi-hammer" />
              <small class="text-muted">Browse icons at <a href="https://icons.getbootstrap.com/" target="_blank">Bootstrap Icons</a></small>
            </div>
            <div class="mb-3">
              <label class="form-label">Image URL (optional)</label>
              <input v-model="currentService.image" type="text" class="form-control" placeholder="https://example.com/service.jpg" />
            </div>
            <div class="row">
              <div class="col-md-6 mb-3">
                <label class="form-label">Display Order</label>
                <input v-model.number="currentService.displayOrder" type="number" class="form-control" />
              </div>
              <div class="col-md-6 mb-3">
                <label class="form-label">Status</label>
                <select v-model="currentService.isActive" class="form-select">
                  <option :value="true">Active</option>
                  <option :value="false">Inactive</option>
                </select>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="closeModal">Cancel</button>
            <button type="button" class="btn btn-primary" @click="saveService" :disabled="saving">
              <span v-if="saving">
                <span class="spinner-border spinner-border-sm me-2"></span>
                Saving...
              </span>
              <span v-else>Save</span>
            </button>
          </div>
        </div>
      </div>
    </div>
    <div v-if="showAddModal || showEditModal" class="modal-backdrop show"></div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import settingsApi from '@/services/settingsApi'
import { useToast } from 'vue-toastification'

const toast = useToast()

interface Service {
  id?: number
  title: string
  description: string
  icon: string
  image: string
  displayOrder: number
  isActive: boolean
}

const services = ref<Service[]>([])
const loading = ref(true)
const saving = ref(false)
const showAddModal = ref(false)
const showEditModal = ref(false)
const currentService = ref<Service>({
  title: '',
  description: '',
  icon: 'bi-tools',
  image: '',
  displayOrder: 0,
  isActive: true
})

const loadServices = async () => {
  try {
    const response = await settingsApi.get('/Services/admin', {
      withCredentials: true
    })
    services.value = response.data
  } catch (error) {
    console.error('Error loading services:', error)
  } finally {
    loading.value = false
  }
}

const editService = (service: Service) => {
  currentService.value = { ...service }
  showEditModal.value = true
}

const saveService = async () => {
  saving.value = true
  try {
    if (showEditModal.value && currentService.value.id) {
      await settingsApi.put(`/Services/${currentService.value.id}`, currentService.value, {
        withCredentials: true
      })
      toast.success('Service updated successfully!')
    } else {
      await settingsApi.post('/Services', currentService.value, {
        withCredentials: true
      })
      toast.success('Service added successfully!')
    }
    await loadServices()
    closeModal()
  } catch (error: any) {
    console.error('Error saving service:', error)
    toast.error(error.response?.data?.message || 'Failed to save service')
  } finally {
    saving.value = false
  }
}

const deleteService = async (id: number) => {
  if (!confirm('Are you sure you want to delete this service?')) return

  try {
    await settingsApi.delete(`/Services/${id}`, {
      withCredentials: true
    })
    toast.success('Service deleted successfully!')
    await loadServices()
  } catch (error: any) {
    console.error('Error deleting service:', error)
    toast.error(error.response?.data?.message || 'Failed to delete service')
  }
}

const closeModal = () => {
  showAddModal.value = false
  showEditModal.value = false
  currentService.value = {
    title: '',
    description: '',
    icon: 'bi-tools',
    image: '',
    displayOrder: 0,
    isActive: true
  }
}

onMounted(() => {
  loadServices()
})
</script>

<style scoped>
.modal {
  background: rgba(0, 0, 0, 0.5);
}
</style>
