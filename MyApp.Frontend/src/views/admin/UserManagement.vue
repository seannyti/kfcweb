<template>
  <div class="user-management">
    <div class="row mb-4">
      <div class="col">
        <h2 class="mb-0">
          <i class="bi bi-people-fill text-warning me-2"></i>
          User Management
        </h2>
        <p class="text-muted mb-0">Manage user accounts and permissions</p>
      </div>
      <div class="col-auto">
        <button class="btn btn-primary" @click="showAddUserModal = true">
          <i class="bi bi-person-plus me-2"></i>Add New User
        </button>
      </div>
    </div>

    <!-- Search and Filter -->
    <div class="card shadow-sm mb-4">
      <div class="card-body">
        <div class="row g-3">
          <div class="col-md-8">
            <div class="input-group">
              <span class="input-group-text"><i class="bi bi-search"></i></span>
              <input 
                type="text" 
                class="form-control" 
                placeholder="Search users by name or email..."
                v-model="searchQuery"
              >
            </div>
          </div>
          <div class="col-md-4">
            <select class="form-select" v-model="filterRole">
              <option value="">All Roles</option>
              <option value="SuperAdmin">SuperAdmin</option>
              <option value="Admin">Admin</option>
              <option value="User">User</option>
            </select>
          </div>
        </div>
      </div>
    </div>

    <!-- Users Table -->
    <div class="card shadow-sm">
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr>
                <th>User</th>
                <th>Email</th>
                <th>Role</th>
                <th>Email Status</th>
                <th>Account Status</th>
                <th>Created</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="user in filteredUsers" :key="user.id">
                <td>
                  <div class="d-flex align-items-center">
                    <div class="bg-primary bg-opacity-10 rounded-circle d-flex align-items-center justify-content-center me-2" 
                         style="width: 40px; height: 40px; min-width: 40px; flex-shrink: 0;">
                      <i class="bi bi-person-fill text-primary"></i>
                    </div>
                    <div>
                      <div class="fw-bold">{{ user.name }}</div>
                    </div>
                  </div>
                </td>
                <td>{{ user.email }}</td>
                <td>
                  <span 
                    class="badge"
                    :class="{
                      'bg-danger': user.role === 'SuperAdmin',
                      'bg-primary': user.role === 'Admin',
                      'bg-success': user.role === 'User'
                    }"
                  >
                    {{ user.role }}
                  </span>
                </td>
                <td>
                  <span 
                    class="badge"
                    :class="user.emailVerified ? 'bg-success' : 'bg-warning'"
                  >
                    <i :class="user.emailVerified ? 'bi bi-check-circle-fill' : 'bi bi-exclamation-circle-fill'" class="me-1"></i>
                    {{ user.emailVerified ? 'Verified' : 'Unverified' }}
                  </span>
                </td>
                <td>
                  <span 
                    class="badge"
                    :class="isUserActive(user) ? 'bg-success' : 'bg-danger'"
                  >
                    {{ isUserActive(user) ? 'Active' : 'Locked' }}
                  </span>
                </td>
                <td>
                  <small class="text-muted">{{ formatDate(user.createdAt) }}</small>
                </td>
                <td>
                  <div class="btn-group btn-group-sm">
                    <button 
                      class="btn btn-outline-primary" 
                      @click="editUser(user)"
                      title="Edit"
                    >
                      <i class="bi bi-pencil"></i>
                    </button>
                    <button 
                      v-if="!user.emailVerified"
                      class="btn btn-outline-success" 
                      @click="verifyUserEmail(user)"
                      title="Verify Email"
                    >
                      <i class="bi bi-check-circle"></i>
                    </button>
                    <button 
                      class="btn btn-outline-warning" 
                      @click="toggleUserStatus(user)"
                      :title="isUserActive(user) ? 'Lock Account' : 'Unlock Account'"
                    >
                      <i :class="isUserActive(user) ? 'bi bi-lock' : 'bi bi-unlock'"></i>
                    </button>
                    <button 
                      class="btn btn-outline-danger" 
                      @click="deleteUser(user)"
                      title="Delete"
                      v-if="user.role !== 'SuperAdmin' || authStore.user?.role === 'SuperAdmin'"
                    >
                      <i class="bi bi-trash"></i>
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- Edit User Modal -->
    <div class="modal fade" :class="{ 'show d-block': showEditUserModal }" tabindex="-1" v-if="showEditUserModal">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Edit User Role</h5>
            <button type="button" class="btn-close" @click="showEditUserModal = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">User Name</label>
              <input 
                type="text" 
                class="form-control" 
                v-model="editingUser.name"
                :disabled="authStore.user?.role !== 'SuperAdmin'"
                placeholder="Enter user name"
              >
              <small class="text-muted" v-if="authStore.user?.role === 'SuperAdmin'">
                You can edit the user's name as SuperAdmin
              </small>
            </div>
            <div class="mb-3">
              <label class="form-label">Email</label>
              <input type="email" class="form-control" :value="editingUser?.email" disabled>
            </div>
            <div class="mb-3">
              <label class="form-label">Last Login IP Address</label>
              <div class="input-group">
                <span class="input-group-text"><i class="bi bi-router"></i></span>
                <input 
                  type="text" 
                  class="form-control" 
                  :value="editingUser?.lastLoginIp || 'Never logged in'" 
                  disabled
                >
              </div>
              <small class="text-muted">IP address from the user's most recent login</small>
            </div>
            <div class="mb-3">
              <label class="form-label">Role</label>
              <select class="form-select" v-model="editingUser.role">
                <option value="User">User</option>
                <option value="Admin">Admin</option>
                <option value="SuperAdmin" v-if="authStore.user?.role === 'SuperAdmin'">SuperAdmin</option>
              </select>
              <small class="text-muted">Only SuperAdmin can assign SuperAdmin role</small>
            </div>

            <hr class="my-4">

            <div class="mb-3">
              <div class="d-flex align-items-center mb-2">
                <i class="bi bi-shield-lock text-warning me-2"></i>
                <label class="form-label mb-0 fw-bold">Force Password Reset</label>
              </div>
              <small class="text-muted d-block mb-3">Set a new password for this user. They will be able to change it after logging in.</small>
              
              <div class="mb-3">
                <label class="form-label">New Password</label>
                <input 
                  type="password" 
                  class="form-control" 
                  v-model="newPassword"
                  placeholder="Enter new password (min 8 characters)"
                  minlength="8"
                >
              </div>
              <div class="mb-3">
                <label class="form-label">Confirm Password</label>
                <input 
                  type="password" 
                  class="form-control" 
                  v-model="confirmPassword"
                  placeholder="Confirm new password"
                >
                <small class="text-danger" v-if="newPassword && confirmPassword && newPassword !== confirmPassword">
                  Passwords do not match
                </small>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="showEditUserModal = false" :disabled="saving">Cancel</button>
            <button type="button" class="btn btn-primary" @click="saveUser" :disabled="saving">
              {{ saving ? 'Saving...' : 'Save Changes' }}
            </button>
          </div>
        </div>
      </div>
    </div>
    <div class="modal-backdrop fade show" v-if="showEditUserModal"></div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { useToast } from 'vue-toastification'
import api from '@/services/api'

const authStore = useAuthStore()
const toast = useToast()

const searchQuery = ref('')
const filterRole = ref('')
const showAddUserModal = ref(false)
const showEditUserModal = ref(false)
const editingUser = ref<any>(null)
const loading = ref(true)
const saving = ref(false)
const newPassword = ref('')
const confirmPassword = ref('')

const users = ref<any[]>([])

const loadUsers = async () => {
  try {
    loading.value = true
    const response = await api.get('/admin/users')
    users.value = response.data
  } catch (error: any) {
    console.error('Failed to load users:', error)
    toast.error('Failed to load users')
  } finally {
    loading.value = false
  }
}

const filteredUsers = computed(() => {
  return users.value.filter(user => {
    const matchesSearch = user.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
                         user.email.toLowerCase().includes(searchQuery.value.toLowerCase())
    const matchesRole = !filterRole.value || user.role === filterRole.value
    return matchesSearch && matchesRole
  })
})

const isUserActive = (user: any) => {
  if (!user.lockoutEnd) return true
  return new Date(user.lockoutEnd) < new Date()
}

const formatDate = (date: Date) => {
  return new Intl.DateTimeFormat('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric'
  }).format(new Date(date))
}

const editUser = (user: any) => {
  editingUser.value = { ...user }
  newPassword.value = ''
  confirmPassword.value = ''
  showEditUserModal.value = true
}

const saveUser = async () => {
  if (!editingUser.value) return
  
  // Validate password fields if provided
  if (newPassword.value || confirmPassword.value) {
    if (newPassword.value !== confirmPassword.value) {
      toast.error('Passwords do not match')
      return
    }
    if (newPassword.value.length < 8) {
      toast.error('Password must be at least 8 characters')
      return
    }
  }
  
  try {
    saving.value = true
    
    const originalUser = users.value.find(u => u.id === editingUser.value!.id)
    let nameUpdated = false
    let roleUpdated = false
    let passwordReset = false
    
    // Update name if SuperAdmin and name changed
    if (authStore.user?.role === 'SuperAdmin' && originalUser && editingUser.value.name !== originalUser.name) {
      await api.put(`/admin/users/${editingUser.value.id}/name`, {
        name: editingUser.value.name
      })
      nameUpdated = true
    }
    
    // Update role only if it changed
    if (originalUser && editingUser.value.role !== originalUser.role) {
      await api.put('/admin/users/role', {
        userId: editingUser.value.id,
        role: editingUser.value.role
      })
      roleUpdated = true
    }
    
    // Reset password if provided
    if (newPassword.value) {
      await api.post('/admin/users/reset-password', {
        userId: editingUser.value.id,
        newPassword: newPassword.value
      })
      passwordReset = true
    }
    
    // Show appropriate success message
    if (passwordReset) {
      toast.success('User updated and password reset successfully')
    } else if (nameUpdated || roleUpdated) {
      toast.success('User updated successfully')
    } else {
      toast.info('No changes were made')
    }
    
    showEditUserModal.value = false
    newPassword.value = ''
    confirmPassword.value = ''
    await loadUsers()
  } catch (error: any) {
    console.error('Failed to update user:', error)
    toast.error(error.response?.data?.message || 'Failed to update user')
  } finally {
    saving.value = false
  }
}

const toggleUserStatus = async (user: any) => {
  const isActive = isUserActive(user)
  const action = isActive ? 'lock' : 'unlock'
  const confirmMessage = isActive 
    ? `Are you sure you want to lock ${user.name}'s account? They will not be able to log in.`
    : `Are you sure you want to unlock ${user.name}'s account?`
  
  if (!confirm(confirmMessage)) return
  
  try {
    await api.post(`/admin/users/${user.id}/${action}`)
    toast.success(`User account ${action}ed successfully`)
    await loadUsers()
  } catch (error: any) {
    console.error('Failed to toggle user status:', error)
    toast.error(error.response?.data?.message || `Failed to ${action} user account`)
  }
}

const verifyUserEmail = async (user: any) => {
  if (!confirm(`Manually verify email for ${user.name} (${user.email})?`)) return
  
  try {
    await api.post(`/admin/users/${user.id}/verify-email`)
    toast.success('Email verified successfully')
    await loadUsers()
  } catch (error: any) {
    console.error('Failed to verify email:', error)
    toast.error(error.response?.data?.message || 'Failed to verify email')
  }
}

const deleteUser = async (user: any) => {
  if (confirm(`Are you sure you want to delete ${user.name}?`)) {
    try {
      await api.delete(`/admin/users/${user.id}`)
      toast.success('User deleted successfully')
      await loadUsers()
    } catch (error: any) {
      console.error('Failed to delete user:', error)
      toast.error(error.response?.data?.message || 'Failed to delete user')
    }
  }
}

onMounted(() => {
  loadUsers()
})
</script>

<style scoped>
.card {
  border: none;
}

.table th {
  font-weight: 600;
  font-size: 0.875rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

/* Primary button colors now inherit from theme */
</style>
