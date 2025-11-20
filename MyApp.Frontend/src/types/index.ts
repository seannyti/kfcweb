export interface User {
  id: number
  email: string
  name: string
  role: string
  createdAt: string
}

export type UserRole = 'SuperAdmin' | 'Admin' | 'User'

export interface AuthResponse {
  token: string
  refreshToken: string
  user: User
}

export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  email: string
  password: string
  name: string
}
