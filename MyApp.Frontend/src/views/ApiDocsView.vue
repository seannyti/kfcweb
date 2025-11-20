<template>
  <div class="api-docs-view">
    <!-- Header -->
    <section class="docs-header bg-gradient text-white py-5">
      <div class="container">
        <div class="row align-items-center">
          <div class="col-lg-8">
            <h1 class="display-4 fw-bold mb-3">
              <i class="bi bi-code-square me-3"></i>
              API Documentation
            </h1>
            <p class="lead mb-0">
              Complete API reference for Knudson Family Construction platform
            </p>
          </div>
          <div class="col-lg-4 text-lg-end">
            <div class="api-version-badge">
              <span class="badge bg-light text-dark fs-5">v1.0</span>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Navigation Tabs -->
    <section class="docs-nav bg-light border-bottom sticky-top">
      <div class="container">
        <ul class="nav nav-pills py-3">
          <li class="nav-item">
            <a class="nav-link" :class="{ active: activeTab === 'overview' }" @click="activeTab = 'overview'" href="#overview">
              <i class="bi bi-house-door me-2"></i>Overview
            </a>
          </li>
          <li class="nav-item">
            <a class="nav-link" :class="{ active: activeTab === 'auth' }" @click="activeTab = 'auth'" href="#auth">
              <i class="bi bi-shield-lock me-2"></i>Authentication
            </a>
          </li>
          <li class="nav-item">
            <a class="nav-link" :class="{ active: activeTab === 'business' }" @click="activeTab = 'business'" href="#business">
              <i class="bi bi-building me-2"></i>Business
            </a>
          </li>
          <li class="nav-item">
            <a class="nav-link" :class="{ active: activeTab === 'services' }" @click="activeTab = 'services'" href="#services">
              <i class="bi bi-tools me-2"></i>Services
            </a>
          </li>
          <li class="nav-item">
            <a class="nav-link" :class="{ active: activeTab === 'projects' }" @click="activeTab = 'projects'" href="#projects">
              <i class="bi bi-image me-2"></i>Projects
            </a>
          </li>
          <li class="nav-item">
            <a class="nav-link" :class="{ active: activeTab === 'quotes' }" @click="activeTab = 'quotes'" href="#quotes">
              <i class="bi bi-envelope-paper me-2"></i>Quotes
            </a>
          </li>
          <li class="nav-item">
            <a class="nav-link" :class="{ active: activeTab === 'settings' }" @click="activeTab = 'settings'" href="#settings">
              <i class="bi bi-gear me-2"></i>Settings
            </a>
          </li>
        </ul>
      </div>
    </section>

    <!-- Content -->
    <section class="docs-content py-5">
      <div class="container">
        <!-- Overview Tab -->
        <div v-show="activeTab === 'overview'" id="overview">
          <h2 class="mb-4">Getting Started</h2>
          
          <div class="card mb-4">
            <div class="card-body">
              <h5 class="card-title">Base URLs</h5>
              <div class="code-block">
                <code>
                  <strong>Users API:</strong> {{ apiUrl }}<br>
                  <strong>Settings API:</strong> {{ settingsApiUrl }}
                </code>
              </div>
            </div>
          </div>

          <div class="card mb-4">
            <div class="card-body">
              <h5 class="card-title">Response Format</h5>
              <p>All API responses are in JSON format. Successful responses return the requested data, while errors include a message field.</p>
              <div class="code-block">
                <pre><code>{
  "id": 1,
  "title": "Residential Construction",
  "description": "Custom home building...",
  "isActive": true
}</code></pre>
              </div>
            </div>
          </div>

          <div class="card">
            <div class="card-body">
              <h5 class="card-title">HTTP Status Codes</h5>
              <table class="table table-sm">
                <tbody>
                  <tr><td><span class="badge bg-success">200</span></td><td>OK - Request successful</td></tr>
                  <tr><td><span class="badge bg-success">201</span></td><td>Created - Resource created</td></tr>
                  <tr><td><span class="badge bg-warning text-dark">400</span></td><td>Bad Request - Invalid data</td></tr>
                  <tr><td><span class="badge bg-warning text-dark">401</span></td><td>Unauthorized - Authentication required</td></tr>
                  <tr><td><span class="badge bg-warning text-dark">403</span></td><td>Forbidden - Insufficient permissions</td></tr>
                  <tr><td><span class="badge bg-warning text-dark">404</span></td><td>Not Found - Resource not found</td></tr>
                  <tr><td><span class="badge bg-danger">500</span></td><td>Server Error - Internal error</td></tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <!-- Authentication Tab -->
        <div v-show="activeTab === 'auth'" id="auth">
          <h2 class="mb-4">Authentication</h2>
          
          <div class="endpoint-card mb-4">
            <div class="endpoint-header">
              <span class="badge bg-success">POST</span>
              <code>/auth/login</code>
            </div>
            <div class="endpoint-body">
              <p>Authenticate a user and receive a JWT token in an HTTP-only cookie.</p>
              <h6>Request Body:</h6>
              <div class="code-block mb-3">
                <pre><code>{
  "email": "user@example.com",
  "password": "yourpassword"
}</code></pre>
              </div>
              <h6>Response (200):</h6>
              <div class="code-block">
                <pre><code>{
  "user": {
    "id": 1,
    "email": "user@example.com",
    "name": "John Doe",
    "role": "User"
  },
  "message": "Login successful"
}</code></pre>
              </div>
            </div>
          </div>

          <div class="endpoint-card mb-4">
            <div class="endpoint-header">
              <span class="badge bg-success">POST</span>
              <code>/auth/register</code>
            </div>
            <div class="endpoint-body">
              <p>Register a new user account.</p>
              <h6>Request Body:</h6>
              <div class="code-block">
                <pre><code>{
  "email": "newuser@example.com",
  "name": "Jane Doe",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!"
}</code></pre>
              </div>
            </div>
          </div>

          <div class="endpoint-card">
            <div class="endpoint-header">
              <span class="badge bg-warning text-dark">POST</span>
              <code>/auth/logout</code>
            </div>
            <div class="endpoint-body">
              <p>Logout the current user and clear the JWT cookie.</p>
              <p class="mb-0"><i class="bi bi-shield-lock text-warning"></i> Requires authentication</p>
            </div>
          </div>
        </div>

        <!-- Business Content Tab -->
        <div v-show="activeTab === 'business'" id="business">
          <h2 class="mb-4">Business Content API</h2>
          
          <div class="endpoint-card mb-4">
            <div class="endpoint-header">
              <span class="badge bg-primary">GET</span>
              <code>/api/BusinessContent</code>
            </div>
            <div class="endpoint-body">
              <p>Get business content including hero section, about info, stats, contact details, and social media links.</p>
              <p><i class="bi bi-unlock text-success"></i> Public endpoint - No authentication required</p>
              <h6>Response (200):</h6>
              <div class="code-block">
                <pre><code>{
  "id": 1,
  "heroTitle": "Building Your Dreams Into Reality",
  "heroSubtitle": "Professional construction services...",
  "heroButtonText": "Get a Free Quote",
  "heroImage": "https://...",
  "aboutTitle": "About Knudson Family Construction",
  "aboutDescription": "We are a family-owned...",
  "missionStatement": "To deliver exceptional...",
  "yearsInBusiness": 20,
  "projectsCompleted": 150,
  "happyClients": 200,
  "teamMembers": 15,
  "contactPhone": "(555) 123-4567",
  "contactEmail": "info@example.com",
  "contactAddress": "123 Main St...",
  "businessHours": "Monday - Friday: 8AM - 5PM",
  "facebookUrl": "https://facebook.com/...",
  "instagramUrl": "https://instagram.com/...",
  "quoteFormTitle": "Request a Quote",
  "enableQuoteRequests": true
}</code></pre>
              </div>
            </div>
          </div>

          <div class="endpoint-card">
            <div class="endpoint-header">
              <span class="badge bg-warning text-dark">PUT</span>
              <code>/api/BusinessContent</code>
            </div>
            <div class="endpoint-body">
              <p>Update business content.</p>
              <p><i class="bi bi-shield-lock text-warning"></i> Requires Admin or SuperAdmin role</p>
            </div>
          </div>
        </div>

        <!-- Services Tab -->
        <div v-show="activeTab === 'services'" id="services">
          <h2 class="mb-4">Services API</h2>
          
          <div class="endpoint-card mb-4">
            <div class="endpoint-header">
              <span class="badge bg-primary">GET</span>
              <code>/api/Services</code>
            </div>
            <div class="endpoint-body">
              <p>Get all active services (public endpoint).</p>
              <h6>Response (200):</h6>
              <div class="code-block">
                <pre><code>[
  {
    "id": 1,
    "title": "Residential Construction",
    "description": "Custom home building...",
    "icon": "bi-house-fill",
    "image": "",
    "displayOrder": 1,
    "isActive": true
  }
]</code></pre>
              </div>
            </div>
          </div>

          <div class="endpoint-card mb-4">
            <div class="endpoint-header">
              <span class="badge bg-primary">GET</span>
              <code>/api/Services/admin</code>
            </div>
            <div class="endpoint-body">
              <p>Get all services including inactive ones.</p>
              <p><i class="bi bi-shield-lock text-warning"></i> Requires Admin or SuperAdmin role</p>
            </div>
          </div>

          <div class="endpoint-card mb-4">
            <div class="endpoint-header">
              <span class="badge bg-success">POST</span>
              <code>/api/Services</code>
            </div>
            <div class="endpoint-body">
              <p>Create a new service.</p>
              <p><i class="bi bi-shield-lock text-warning"></i> Requires Admin or SuperAdmin role</p>
              <h6>Request Body:</h6>
              <div class="code-block">
                <pre><code>{
  "title": "Kitchen Remodeling",
  "description": "Transform your kitchen...",
  "icon": "bi-hammer",
  "image": "https://...",
  "displayOrder": 7,
  "isActive": true
}</code></pre>
              </div>
            </div>
          </div>

          <div class="endpoint-card mb-4">
            <div class="endpoint-header">
              <span class="badge bg-warning text-dark">PUT</span>
              <code>/api/Services/{id}</code>
            </div>
            <div class="endpoint-body">
              <p>Update an existing service.</p>
              <p><i class="bi bi-shield-lock text-warning"></i> Requires Admin or SuperAdmin role</p>
            </div>
          </div>

          <div class="endpoint-card">
            <div class="endpoint-header">
              <span class="badge bg-danger">DELETE</span>
              <code>/api/Services/{id}</code>
            </div>
            <div class="endpoint-body">
              <p>Delete a service.</p>
              <p><i class="bi bi-shield-lock text-warning"></i> Requires Admin or SuperAdmin role</p>
            </div>
          </div>
        </div>

        <!-- Projects Tab -->
        <div v-show="activeTab === 'projects'" id="projects">
          <h2 class="mb-4">Projects API</h2>
          
          <div class="endpoint-card mb-4">
            <div class="endpoint-header">
              <span class="badge bg-primary">GET</span>
              <code>/api/Projects</code>
            </div>
            <div class="endpoint-body">
              <p>Get all active projects.</p>
              <p><i class="bi bi-unlock text-success"></i> Public endpoint</p>
            </div>
          </div>

          <div class="endpoint-card mb-4">
            <div class="endpoint-header">
              <span class="badge bg-primary">GET</span>
              <code>/api/Projects/featured</code>
            </div>
            <div class="endpoint-body">
              <p>Get featured projects (max 6).</p>
              <p><i class="bi bi-unlock text-success"></i> Public endpoint</p>
              <h6>Response (200):</h6>
              <div class="code-block">
                <pre><code>[
  {
    "id": 1,
    "title": "Modern Family Home",
    "description": "A beautiful 3,500 sq ft...",
    "category": "Residential",
    "location": "Portland, OR",
    "imageUrl": "https://...",
    "galleryImages": ["url1", "url2"],
    "completionDate": "2024-06-15",
    "isFeatured": true,
    "isActive": true,
    "displayOrder": 1
  }
]</code></pre>
              </div>
            </div>
          </div>

          <div class="endpoint-card mb-4">
            <div class="endpoint-header">
              <span class="badge bg-success">POST</span>
              <code>/api/Projects</code>
            </div>
            <div class="endpoint-body">
              <p>Create a new project.</p>
              <p><i class="bi bi-shield-lock text-warning"></i> Requires Admin or SuperAdmin role</p>
            </div>
          </div>

          <div class="endpoint-card mb-4">
            <div class="endpoint-header">
              <span class="badge bg-warning text-dark">PUT</span>
              <code>/api/Projects/{id}</code>
            </div>
            <div class="endpoint-body">
              <p>Update an existing project.</p>
              <p><i class="bi bi-shield-lock text-warning"></i> Requires Admin or SuperAdmin role</p>
            </div>
          </div>

          <div class="endpoint-card">
            <div class="endpoint-header">
              <span class="badge bg-danger">DELETE</span>
              <code>/api/Projects/{id}</code>
            </div>
            <div class="endpoint-body">
              <p>Delete a project.</p>
              <p><i class="bi bi-shield-lock text-warning"></i> Requires Admin or SuperAdmin role</p>
            </div>
          </div>
        </div>

        <!-- Quote Requests Tab -->
        <div v-show="activeTab === 'quotes'" id="quotes">
          <h2 class="mb-4">Quote Requests API</h2>
          
          <div class="endpoint-card mb-4">
            <div class="endpoint-header">
              <span class="badge bg-success">POST</span>
              <code>/api/QuoteRequests</code>
            </div>
            <div class="endpoint-body">
              <p>Submit a new quote request.</p>
              <p><i class="bi bi-unlock text-success"></i> Public endpoint</p>
              <h6>Request Body:</h6>
              <div class="code-block">
                <pre><code>{
  "name": "John Smith",
  "email": "john@example.com",
  "phone": "(555) 987-6543",
  "projectType": "Residential",
  "description": "I need to build a new garage...",
  "location": "Seattle, WA",
  "timeline": "3-6 months",
  "budget": "$50k - $100k"
}</code></pre>
              </div>
            </div>
          </div>

          <div class="endpoint-card mb-4">
            <div class="endpoint-header">
              <span class="badge bg-primary">GET</span>
              <code>/api/QuoteRequests</code>
            </div>
            <div class="endpoint-body">
              <p>Get all quote requests.</p>
              <p><i class="bi bi-shield-lock text-warning"></i> Requires Admin or SuperAdmin role</p>
            </div>
          </div>

          <div class="endpoint-card mb-4">
            <div class="endpoint-header">
              <span class="badge bg-primary">GET</span>
              <code>/api/QuoteRequests/admin/stats</code>
            </div>
            <div class="endpoint-body">
              <p>Get quote request statistics.</p>
              <p><i class="bi bi-shield-lock text-warning"></i> Requires Admin or SuperAdmin role</p>
              <h6>Response (200):</h6>
              <div class="code-block">
                <pre><code>{
  "total": 45,
  "newRequests": 8,
  "contacted": 12,
  "quoted": 15,
  "won": 7,
  "lost": 3,
  "conversionRate": 15.56
}</code></pre>
              </div>
            </div>
          </div>

          <div class="endpoint-card mb-4">
            <div class="endpoint-header">
              <span class="badge bg-warning text-dark">PUT</span>
              <code>/api/QuoteRequests/{id}</code>
            </div>
            <div class="endpoint-body">
              <p>Update quote request status and notes.</p>
              <p><i class="bi bi-shield-lock text-warning"></i> Requires Admin or SuperAdmin role</p>
              <h6>Request Body:</h6>
              <div class="code-block">
                <pre><code>{
  "status": "Contacted",
  "notes": "Called and discussed project details"
}</code></pre>
              </div>
            </div>
          </div>

          <div class="endpoint-card">
            <div class="endpoint-header">
              <span class="badge bg-danger">DELETE</span>
              <code>/api/QuoteRequests/{id}</code>
            </div>
            <div class="endpoint-body">
              <p>Delete a quote request.</p>
              <p><i class="bi bi-shield-lock text-warning"></i> Requires Admin or SuperAdmin role</p>
            </div>
          </div>
        </div>

        <!-- Settings Tab -->
        <div v-show="activeTab === 'settings'" id="settings">
          <h2 class="mb-4">Settings API</h2>
          
          <div class="endpoint-card mb-4">
            <div class="endpoint-header">
              <span class="badge bg-primary">GET</span>
              <code>/settings/general</code>
            </div>
            <div class="endpoint-body">
              <p>Get general site settings.</p>
              <p><i class="bi bi-unlock text-success"></i> Public endpoint</p>
              <h6>Response (200):</h6>
              <div class="code-block">
                <pre><code>{
  "siteName": "Knudson Family Construction",
  "tagline": "Building Excellence Since 2005",
  "description": "Professional construction...",
  "logo": "https://...",
  "timezone": "America/Los_Angeles",
  "dateFormat": "MM/DD/YYYY",
  "maintenanceMode": false
}</code></pre>
              </div>
            </div>
          </div>

          <div class="endpoint-card mb-4">
            <div class="endpoint-header">
              <span class="badge bg-primary">GET</span>
              <code>/settings/theme</code>
            </div>
            <div class="endpoint-body">
              <p>Get theme and color settings.</p>
              <p><i class="bi bi-unlock text-success"></i> Public endpoint</p>
            </div>
          </div>

          <div class="endpoint-card mb-4">
            <div class="endpoint-header">
              <span class="badge bg-warning text-dark">PUT</span>
              <code>/admin/settings/general</code>
            </div>
            <div class="endpoint-body">
              <p>Update general settings.</p>
              <p><i class="bi bi-shield-lock text-warning"></i> Requires Admin or SuperAdmin role</p>
            </div>
          </div>

          <div class="endpoint-card">
            <div class="endpoint-header">
              <span class="badge bg-warning text-dark">PUT</span>
              <code>/admin/settings/theme</code>
            </div>
            <div class="endpoint-body">
              <p>Update theme settings.</p>
              <p><i class="bi bi-shield-lock text-warning"></i> Requires Admin or SuperAdmin role</p>
            </div>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

const activeTab = ref('overview')
const apiUrl = computed(() => import.meta.env.VITE_API_URL || 'http://localhost:5000')
const settingsApiUrl = computed(() => import.meta.env.VITE_SETTINGS_API_URL || 'http://localhost:5001')
</script>

<style scoped>
.bg-gradient {
  background: linear-gradient(135deg, var(--bs-primary) 0%, color-mix(in srgb, var(--bs-primary) 80%, #000) 100%);
}

.api-version-badge {
  display: inline-block;
}

.docs-nav {
  z-index: 100;
}

.nav-pills .nav-link {
  color: #666;
  border-radius: 0.5rem;
  transition: all 0.3s ease;
}

.nav-pills .nav-link:hover {
  background-color: rgba(0, 0, 0, 0.05);
}

.nav-pills .nav-link.active {
  background-color: var(--bs-primary);
  color: white;
}

.endpoint-card {
  border: 1px solid #dee2e6;
  border-radius: 0.5rem;
  overflow: hidden;
}

.endpoint-header {
  background-color: #f8f9fa;
  padding: 1rem 1.5rem;
  border-bottom: 1px solid #dee2e6;
  display: flex;
  align-items: center;
  gap: 1rem;
}

.endpoint-header code {
  font-size: 1.1rem;
  font-weight: 600;
  color: #333;
  background: transparent;
}

.endpoint-body {
  padding: 1.5rem;
}

.code-block {
  background-color: #1e1e1e;
  color: #d4d4d4;
  padding: 1rem;
  border-radius: 0.375rem;
  overflow-x: auto;
}

.code-block code,
.code-block pre {
  background: transparent;
  color: inherit;
  margin: 0;
  padding: 0;
}

.code-block pre code {
  display: block;
  white-space: pre;
}

.card {
  border: 1px solid #dee2e6;
  transition: box-shadow 0.3s ease;
}

.card:hover {
  box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.1);
}

h2 {
  color: var(--bs-primary);
  font-weight: 600;
}

h6 {
  color: #666;
  font-weight: 600;
  margin-top: 1rem;
  margin-bottom: 0.5rem;
}

.table-sm td {
  padding: 0.5rem;
  vertical-align: middle;
}

.sticky-top {
  top: 0;
}
</style>

