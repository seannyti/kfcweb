# KFC Web - Construction Business Platform

A modern full-stack web application for construction business management built with **.NET 9**, **Vue 3**, and **Docker**.

## 🚀 Features

- **Public Website**: Company information, services, projects portfolio, and contact forms
- **Admin Dashboard**: Manage site content, settings, and user accounts
- **User Management**: Role-based access control (SuperAdmin, Admin, User)
- **Content Management**: Services, projects, team members, and business information
- **Email Configuration**: SMTP settings for automated notifications
- **Theme Customization**: Customize colors, fonts, and appearance
- **Responsive Design**: Mobile-first Bootstrap 5 interface
- **Secure Authentication**: JWT-based auth with HTTP-only cookies
- **Docker Deployment**: Production-ready containerized deployment

## 📋 Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- [Docker](https://www.docker.com/) (for production)
- Azure SQL Database or SQL Server

## 🏗️ Architecture

```
/kfcweb
├── MyUsers.Api/              # .NET 9 - User Management & Auth
│   ├── Controllers/          # Auth, Admin, Backup endpoints
│   ├── Models/              # User entity
│   ├── Services/            # Auth, Token, Email services
│   └── Data/                # Database context
│
├── MySettings.Api/          # .NET 9 - Site Settings & Content
│   ├── Controllers/         # Settings, Services, Projects
│   ├── Models/              # Content entities
│   └── Data/                # Database context
│
├── MyApp.Frontend/          # Vue 3 + TypeScript
│   ├── src/
│   │   ├── views/           # Pages (Home, About, Admin)
│   │   ├── components/      # UI components
│   │   ├── stores/          # State management
│   │   └── services/        # API clients
│   └── package.json
│
├── nginx/                   # Reverse proxy
│   └── nginx.conf
│
└── docker-compose.yml       # Container orchestration
```

## 🚀 Quick Start

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/seannyti/kfcweb.git
   cd kfcweb
   ```

2. **Configure environment**
   
   Create `.env` file in project root:
   ```env
   # Database connection strings
   DB_CONNECTION_STRING_USERS=Server=...;Database=...;User ID=...;Password=...;
   DB_CONNECTION_STRING_SETTINGS=Server=...;Database=...;User ID=...;Password=...;
   
   # JWT Settings
   JWT_SECRET_KEY=your-secret-key-minimum-32-characters
   JWT_ISSUER=KFCWeb
   JWT_AUDIENCE=KFCWebUsers
   ```

3. **Run Users API**
   ```bash
   cd MyUsers.Api
   dotnet restore
   dotnet run
   # http://localhost:5000
   ```

4. **Run Settings API** (new terminal)
   ```bash
   cd MySettings.Api
   dotnet restore
   dotnet run
   # http://localhost:5001
   ```

5. **Run Frontend** (new terminal)
   ```bash
   cd MyApp.Frontend
   npm install
   npm run dev
   # http://localhost:5173
   ```

### Docker Deployment (Production)

See **[DOCKER_SETUP_GUIDE.md](DOCKER_SETUP_GUIDE.md)** for complete deployment instructions.

**Quick deployment:**
```bash
# On your server
git clone https://github.com/seannyti/kfcweb.git
cd kfcweb

# Create .env file with your credentials (see .env.example)
nano .env

# Build and run
docker compose build
docker compose up -d

# Check status
docker compose ps
docker compose logs -f
```

## 🔐 User Roles

- **SuperAdmin**: Full system access, manage all users and settings
- **Admin**: Manage content and users (except SuperAdmin accounts)
- **User**: View public content and assigned projects

Default admin account is created on first run:
- Email: `admin@kfcweb.com`
- Password: `Admin123!`
- **⚠️ Change immediately after first login**

## 🌐 API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login (sets HTTP-only cookie)
- `POST /api/auth/logout` - Logout
- `GET /api/auth/me` - Get current user

### Admin - Users (MyUsers.Api)
- `GET /api/admin/users` - List all users
- `PUT /api/admin/users/role` - Update user role
- `DELETE /api/admin/users/{id}` - Delete user
- `GET /api/admin/statistics` - User statistics

### Admin - Settings (MySettings.Api)
- `GET /api/admin/settings/settings` - Get site settings
- `PUT /api/admin/settings/settings` - Update settings
- `POST /api/admin/settings/maintenance/toggle` - Toggle maintenance mode
- `POST /api/admin/settings/email/test` - Send test email
- `GET /api/admin/settings/theme` - Get theme settings
- `PUT /api/admin/settings/theme` - Update theme

### Public Content (MySettings.Api)
- `GET /api/services` - List services
- `GET /api/projects` - List projects
- `GET /api/team` - Team members
- `POST /api/contact` - Submit contact form

## 🔒 Security

- **Password Hashing**: BCrypt with automatic salt
- **JWT Tokens**: Stored in HTTP-only cookies
- **HTTPS Ready**: Auto-detects HTTPS for secure cookies
- **CORS**: Configured for frontend domain
- **SQL Injection**: Protected by Entity Framework
- **XSS**: Vue 3 automatic escaping

### HTTPS Migration

When ready for HTTPS, see **[HTTPS_MIGRATION_GUIDE.md](HTTPS_MIGRATION_GUIDE.md)** for:
- SSL certificate setup (Let's Encrypt)
- Nginx HTTPS configuration
- Frontend URL updates
- Cookie security settings

## 📦 Technology Stack

### Backend
- **.NET 9** - High-performance framework
- **ASP.NET Core Web API** - RESTful APIs
- **Entity Framework Core** - ORM
- **Azure SQL Database** - Cloud database
- **JWT Bearer Auth** - Token-based security
- **BCrypt.Net** - Password hashing

### Frontend
- **Vue 3** - Progressive framework
- **TypeScript** - Type-safe JavaScript
- **Vite** - Build tool
- **Pinia** - State management
- **Vue Router** - Routing
- **Axios** - HTTP client
- **Bootstrap 5** - UI framework

### DevOps
- **Docker** - Containerization
- **Docker Compose** - Multi-container orchestration
- **Nginx** - Reverse proxy & static files
- **GitHub Actions** - CI/CD (optional)

## 🐛 Troubleshooting

### Database Connection Errors
```bash
# Verify connection string format
# Ensure no special characters like $ in passwords
# Check Azure SQL firewall rules
```

### Container Issues
```bash
# View logs
docker compose logs -f

# Restart containers
docker compose restart

# Rebuild from scratch
docker compose down
docker compose build --no-cache
docker compose up -d
```

### 502 Bad Gateway
```bash
# Check API containers are running
docker compose ps

# Check API logs
docker compose logs users-api
docker compose logs settings-api
```

## 📚 Documentation

- **[DOCKER_SETUP_GUIDE.md](DOCKER_SETUP_GUIDE.md)** - Complete Linux deployment guide
- **[HTTPS_MIGRATION_GUIDE.md](HTTPS_MIGRATION_GUIDE.md)** - SSL/HTTPS setup instructions
- `.env.example` - Environment variable template

## 🤝 Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Open Pull Request

## 📝 License

MIT License - see LICENSE file for details

## 🔗 Links

- **Repository**: https://github.com/seannyti/kfcweb
- **Issues**: https://github.com/seannyti/kfcweb/issues

---

**Built with ❤️ using .NET 9, Vue 3, and Docker**
