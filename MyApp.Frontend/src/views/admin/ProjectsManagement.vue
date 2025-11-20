<template>
  <div class="projects-management">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2><i class="bi bi-image me-2"></i>Projects Management</h2>
      <button @click="showAddModal = true" class="btn btn-primary">
        <i class="bi bi-plus-circle me-2"></i>
        Add Project
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
              <th>Image</th>
              <th>Title</th>
              <th>Category</th>
              <th>Location</th>
              <th>Featured</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="project in projects" :key="project.id">
              <td>{{ project.displayOrder }}</td>
              <td>
                <img :src="project.imageUrl || 'https://via.placeholder.com/100x70'" alt="" style="width: 100px; height: 70px; object-fit: cover;" class="rounded">
              </td>
              <td>{{ project.title }}</td>
              <td><span class="badge bg-primary">{{ project.category }}</span></td>
              <td>{{ project.location }}</td>
              <td>
                <span v-if="project.isFeatured" class="badge bg-warning text-dark">
                  <i class="bi bi-star-fill"></i> Featured
                </span>
              </td>
              <td>
                <span :class="['badge', project.isActive ? 'bg-success' : 'bg-secondary']">
                  {{ project.isActive ? 'Active' : 'Inactive' }}
                </span>
              </td>
              <td>
                <button @click="editProject(project)" class="btn btn-sm btn-outline-primary me-2">
                  <i class="bi bi-pencil"></i>
                </button>
                <button @click="project.id && deleteProject(project.id)" class="btn btn-sm btn-outline-danger">
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
      <div class="modal-dialog modal-xl">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ showEditModal ? 'Edit Project' : 'Add Project' }}</h5>
            <button type="button" class="btn-close" @click="closeModal"></button>
          </div>
          <div class="modal-body">
            <div class="row">
              <div class="col-md-8 mb-3">
                <label class="form-label">Title</label>
                <input v-model="currentProject.title" type="text" class="form-control" required />
              </div>
              <div class="col-md-4 mb-3">
                <label class="form-label">Category</label>
                <select v-model="currentProject.category" class="form-select">
                  <option value="Residential">Residential</option>
                  <option value="Commercial">Commercial</option>
                  <option value="Renovation">Renovation</option>
                </select>
              </div>
            </div>
            <div class="mb-3">
              <label class="form-label">Description</label>
              <textarea v-model="currentProject.description" class="form-control" rows="4" required></textarea>
            </div>
            <div class="row">
              <div class="col-md-6 mb-3">
                <label class="form-label">Location</label>
                <input v-model="currentProject.location" type="text" class="form-control" />
              </div>
              <div class="col-md-6 mb-3">
                <label class="form-label">Completion Date</label>
                <input v-model="currentProject.completionDate" type="date" class="form-control" />
              </div>
            </div>
            <div class="mb-3">
              <label class="form-label">Main Image URL</label>
              <input v-model="currentProject.imageUrl" type="text" class="form-control" placeholder="https://example.com/project.jpg" />
            </div>
            <div class="mb-3">
              <label class="form-label">Gallery Images (one per line)</label>
              <textarea v-model="galleryImagesText" class="form-control" rows="3" placeholder="https://example.com/image1.jpg&#10;https://example.com/image2.jpg"></textarea>
            </div>
            <div class="row">
              <div class="col-md-4 mb-3">
                <label class="form-label">Display Order</label>
                <input v-model.number="currentProject.displayOrder" type="number" class="form-control" />
              </div>
              <div class="col-md-4 mb-3">
                <div class="form-check mt-4">
                  <input v-model="currentProject.isFeatured" class="form-check-input" type="checkbox" id="featured">
                  <label class="form-check-label" for="featured">
                    Featured Project
                  </label>
                </div>
              </div>
              <div class="col-md-4 mb-3">
                <label class="form-label">Status</label>
                <select v-model="currentProject.isActive" class="form-select">
                  <option :value="true">Active</option>
                  <option :value="false">Inactive</option>
                </select>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="closeModal">Cancel</button>
            <button type="button" class="btn btn-primary" @click="saveProject" :disabled="saving">
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
import { ref, computed, onMounted } from 'vue'
import settingsApi from '@/services/settingsApi'
import { useToast } from 'vue-toastification'

const toast = useToast()

interface Project {
  id?: number
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
const saving = ref(false)
const showAddModal = ref(false)
const showEditModal = ref(false)
const currentProject = ref<Project>({
  title: '',
  description: '',
  category: 'Residential',
  location: '',
  imageUrl: '',
  galleryImages: [],
  completionDate: null,
  isFeatured: false,
  isActive: true,
  displayOrder: 0
})

const galleryImagesText = computed({
  get: () => currentProject.value.galleryImages.join('\n'),
  set: (value: string) => {
    currentProject.value.galleryImages = value.split('\n').filter(url => url.trim())
  }
})

const loadProjects = async () => {
  try {
    const response = await settingsApi.get('/Projects/admin', {
      withCredentials: true
    })
    projects.value = response.data
  } catch (error) {
    console.error('Error loading projects:', error)
  } finally {
    loading.value = false
  }
}

const editProject = (project: Project) => {
  currentProject.value = { ...project }
  showEditModal.value = true
}

const saveProject = async () => {
  saving.value = true
  try {
    if (showEditModal.value && currentProject.value.id) {
      await settingsApi.put(`/Projects/${currentProject.value.id}`, currentProject.value, {
        withCredentials: true
      })
      toast.success('Project updated successfully!')
    } else {
      await settingsApi.post('/Projects', currentProject.value, {
        withCredentials: true
      })
      toast.success('Project added successfully!')
    }
    await loadProjects()
    closeModal()
  } catch (error: any) {
    console.error('Error saving project:', error)
    toast.error(error.response?.data?.message || 'Failed to save project')
  } finally {
    saving.value = false
  }
}

const deleteProject = async (id: number) => {
  if (!confirm('Are you sure you want to delete this project?')) return

  try {
    await settingsApi.delete(`/Projects/${id}`, {
      withCredentials: true
    })
    toast.success('Project deleted successfully!')
    await loadProjects()
  } catch (error: any) {
    console.error('Error deleting project:', error)
    toast.error(error.response?.data?.message || 'Failed to delete project')
  }
}

const closeModal = () => {
  showAddModal.value = false
  showEditModal.value = false
  currentProject.value = {
    title: '',
    description: '',
    category: 'Residential',
    location: '',
    imageUrl: '',
    galleryImages: [],
    completionDate: null,
    isFeatured: false,
    isActive: true,
    displayOrder: 0
  }
}

onMounted(() => {
  loadProjects()
})
</script>

<style scoped>
.modal {
  background: rgba(0, 0, 0, 0.5);
}
</style>
