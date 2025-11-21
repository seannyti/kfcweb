# Docker Setup Guide - Rocky Linux 9

## Complete Step-by-Step Guide for Deploying KFC Web Application

This guide will walk you through setting up Docker on a fresh Rocky Linux 9 server and deploying the KFC Vue web application from scratch.

---

## Prerequisites

- Rocky Linux 9 server with root or sudo access
- Server IP: Your server's public IP (e.g., 23.239.26.52)
- SSH access to the server
- GitHub repository access
- Azure SQL Database credentials

---

## Part 1: Initial Server Setup

### Step 1: Update System
```bash
# Connect to your server via SSH
ssh root@your-server-ip

# Update all packages
sudo dnf update -y

# Install essential tools
sudo dnf install -y git curl wget nano
```

### Step 2: Install Docker
```bash
# Remove any old Docker versions
sudo dnf remove -y docker docker-client docker-client-latest docker-common docker-latest docker-latest-logrotate docker-logrotate docker-engine podman runc

# Add Docker repository
sudo dnf config-manager --add-repo https://download.docker.com/linux/centos/docker-ce.repo

# Install Docker
sudo dnf install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin

# Start Docker service
sudo systemctl start docker

# Enable Docker to start on boot
sudo systemctl enable docker

# Verify Docker installation
docker --version
# Should output: Docker version 24.x.x or higher

# Test Docker
sudo docker run hello-world
# Should download and run a test container
```

### Step 3: Install Docker Compose (if not installed with Docker)
```bash
# Check if Docker Compose is already installed
docker compose version

# If not installed, download Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose

# Make it executable
sudo chmod +x /usr/local/bin/docker-compose

# Verify installation
docker-compose --version
# Should output: Docker Compose version v2.x.x or higher
```

### Step 4: Configure Firewall
```bash
# Allow HTTP and HTTPS traffic
sudo firewall-cmd --permanent --add-service=http
sudo firewall-cmd --permanent --add-service=https

# Reload firewall
sudo firewall-cmd --reload

# Verify rules
sudo firewall-cmd --list-all
# Should show http and https in services
```

---

## Part 2: Clone and Configure Application

### Step 1: Create Directory Structure
```bash
# Navigate to home directory
cd ~

# Create source directory
mkdir -p source/repos

# Navigate to repos directory
cd source/repos
```

### Step 2: Clone GitHub Repository
```bash
# Clone your repository (replace with your actual repo URL)
git clone https://github.com/yourusername/kfcvue.git

# Navigate into the project
cd kfcvue

# Verify files
ls -la
# Should see: MyApp.Frontend, MyUsers.Api, MySettings.Api, docker-compose.yml, nginx, etc.
```

### Step 3: Create Environment File
```bash
# Create .env file in the project root
nano .env
```

**Copy and paste the following** (update with your actual credentials):

```env
# Database Connection Strings
DB_CONNECTION_STRING_USERS=Server=tcp:kfconstruction.database.windows.net,1433;Initial Catalog=kfconstructiondb-users;User ID=kfconstruction;Password=YourPassword;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;

DB_CONNECTION_STRING_SETTINGS=Server=tcp:kfconstruction.database.windows.net,1433;Initial Catalog=kfconstructiondb-settings;User ID=kfconstruction;Password=YourPassword;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;

# JWT Settings
JWT_SECRET_KEY=GmDlwHVrgx/vxnmhrDmtMqao5wrdtOZ+eWyEZ/Yynuc=
JWT_ISSUER=KFCWeb
JWT_AUDIENCE=KFCWebUsers
```

**Save and exit:** Press `Ctrl + X`, then `Y`, then `Enter`

**Important Notes:**
- Replace `YourPassword` with your actual Azure SQL password
- **AVOID using `$` character in passwords** - it breaks Docker Compose variable interpolation
- If your password contains `$`, change it in Azure SQL Server to a simpler password

### Step 4: Verify Environment File
```bash
# Check that .env file was created correctly
cat .env

# Ensure the file has proper permissions (should not be world-readable)
chmod 600 .env

# Verify permissions
ls -l .env
# Should show: -rw------- (only owner can read/write)
```

---

## Part 3: Build and Deploy with Docker

### Step 1: Understand Docker Compose Configuration

The `docker-compose.yml` file defines 3 services:

1. **nginx** - Web server (port 80/443)
   - Serves frontend static files
   - Reverse proxy to backend APIs
   
2. **users-api** - User management API (internal port 5000)
   - Authentication (login, register, logout)
   - User management (/api/admin)
   - API keys, backups, system health
   
3. **settings-api** - Settings management API (internal port 5001)
   - Site settings (/api/admin/settings)
   - Services, team members, portfolio items

### Step 2: Build Docker Images
```bash
# Make sure you're in the project directory
cd ~/source/repos/kfcvue

# Build all images (this takes 5-10 minutes on first run)
docker compose build

# You'll see output like:
# [+] Building 245.3s (45/45) FINISHED
# - frontend: Vite build, npm install, 233 modules
# - users-api: .NET restore, build, publish
# - settings-api: .NET restore, build, publish
# - nginx: Copy configs and frontend dist
```

**What happens during build:**
- Frontend: Node.js dependencies installed, Vue app built with Vite
- APIs: .NET dependencies restored, projects compiled and published
- Nginx: Configuration copied, ready to serve

### Step 3: Start Containers
```bash
# Start all containers in detached mode
docker compose up -d

# Verify all containers are running
docker compose ps

# Should see:
# NAME                 STATUS         PORTS
# kfcweb-nginx         Up X seconds   0.0.0.0:80->80/tcp, 0.0.0.0:443->443/tcp
# kfcweb-users-api     Up X seconds   5000/tcp
# kfcweb-settings-api  Up X seconds   5001/tcp
```

**Expected Output:**
```
[+] Running 4/4
 ✔ Network kfcvue_default           Created
 ✔ Container kfcweb-users-api       Started
 ✔ Container kfcweb-settings-api    Started
 ✔ Container kfcweb-nginx           Started
```

### Step 4: Verify Deployment
```bash
# Check container health
docker compose ps

# View logs for all containers
docker compose logs

# View logs for specific container
docker compose logs nginx
docker compose logs users-api
docker compose logs settings-api

# Follow logs in real-time (Ctrl+C to exit)
docker compose logs -f
```

**Healthy logs should show:**
- nginx: `start worker process` (no errors)
- users-api: `Now listening on: http://[::]:5000` (no exceptions)
- settings-api: `Now listening on: http://[::]:5001` (no exceptions)

### Step 5: Test the Application
```bash
# Test from server command line
curl http://localhost

# Should return HTML content (Vue app)

# Test API endpoints
curl http://localhost/api/auth/me
# Should return 401 Unauthorized (expected - not logged in)

curl -I http://localhost/api/admin/settings
# Should return 401 Unauthorized (expected - requires auth)
```

**Test from your browser:**
1. Open browser and navigate to: `http://your-server-ip`
2. You should see the KFC Construction website with full styling
3. Click "Login" - you should see the login page
4. Try logging in with admin credentials

**If you see the site with Bootstrap styling ✅ - SUCCESS!**

---

## Part 4: Managing Your Deployment

### View Container Status
```bash
# List all containers
docker compose ps

# Detailed container info
docker ps -a

# View resource usage
docker stats
```

### View Logs
```bash
# All containers
docker compose logs

# Last 100 lines
docker compose logs --tail=100

# Real-time logs (Ctrl+C to exit)
docker compose logs -f

# Specific container
docker compose logs nginx
docker compose logs users-api
docker compose logs settings-api

# Logs since specific time
docker compose logs --since 2024-01-01T10:00:00
```

### Restart Containers
```bash
# Restart all containers
docker compose restart

# Restart specific container
docker compose restart nginx
docker compose restart users-api
docker compose restart settings-api

# Stop all containers
docker compose stop

# Start all containers
docker compose start

# Stop and remove all containers
docker compose down

# Stop, remove, and delete volumes (CAUTION: removes data)
docker compose down -v
```

### Update Application Code
```bash
# Navigate to project directory
cd ~/source/repos/kfcvue

# Pull latest changes from GitHub
git pull

# Rebuild images (only rebuilds changed services)
docker compose build

# Restart containers with new images
docker compose up -d

# Alternative: Force rebuild without cache
docker compose build --no-cache

# Remove old images (optional, frees space)
docker image prune -f
```

**Full update workflow:**
```bash
cd ~/source/repos/kfcvue
git pull
docker compose build --no-cache
docker compose down
docker compose up -d
docker compose logs -f
```

---

## Part 5: Troubleshooting

### Issue 1: Container Won't Start
```bash
# Check container status
docker compose ps

# View error logs
docker compose logs container-name

# Common causes:
# - Environment variables missing/incorrect (.env file)
# - Port already in use (check with: sudo netstat -tulpn | grep :80)
# - Database connection failed (check connection string)
```

**Solution:**
```bash
# Stop all containers
docker compose down

# Remove all containers and volumes
docker compose down -v

# Verify .env file
cat .env

# Rebuild and restart
docker compose build --no-cache
docker compose up -d
```

### Issue 2: 502 Bad Gateway
**Symptoms:** Nginx returns 502 error when accessing site

**Causes:**
- Backend APIs not running
- API containers crash-looping
- Network issues between containers

**Solution:**
```bash
# Check which containers are running
docker compose ps

# Check API logs for crashes
docker compose logs users-api
docker compose logs settings-api

# Common crash causes:
# - Database connection string incorrect
# - Environment variables formatted wrong
# - Database not accessible from server

# Restart APIs
docker compose restart users-api settings-api
```

### Issue 3: Database Connection Errors
**Error in logs:** `Format of the initialization string does not conform to specification`

**Causes:**
- Connection string syntax error
- Password contains special characters (`$`, `!`, etc.)
- Environment variable mapping incorrect

**Solution:**
```bash
# Verify .env file connection string format
cat .env | grep DB_CONNECTION

# Ensure connection string is ONE line, no line breaks
# Format: Server=tcp:server.database.windows.net,1433;Initial Catalog=dbname;User ID=username;Password=password;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;

# If password has $ character, change it in Azure SQL
# Rebuild containers after fixing .env
docker compose down
docker compose build --no-cache
docker compose up -d
```

### Issue 4: Environment Variables Not Working
**Symptoms:** APIs crash with configuration errors

**Solution:**
```bash
# Verify environment variables are loaded
docker compose exec users-api printenv

# Should see:
# ConnectionStrings__DefaultConnection=Server=tcp:...
# JwtSettings__SecretKey=...

# If not showing, check docker-compose.yml mapping:
# - DB_CONNECTION_STRING_USERS → ConnectionStrings__DefaultConnection
# - JWT_SECRET_KEY → JwtSettings__SecretKey

# Note: .NET uses __ (double underscore) for hierarchy
```

### Issue 5: No Styling (Bootstrap Not Loading)
**Symptoms:** Site loads but has no styling, plain HTML

**Causes:**
- Bootstrap not bundled in frontend build
- Nginx not serving /assets/ correctly
- Frontend build failed

**Solution:**
```bash
# Check frontend build logs
docker compose logs nginx | grep -i vite

# Should see: "built 233 modules" and CSS bundle size

# If not, rebuild frontend with no cache
docker compose build --no-cache frontend
docker compose up -d nginx

# Verify assets directory exists
docker compose exec nginx ls -la /usr/share/nginx/html/assets/
# Should show CSS and JS files
```

### Issue 6: Login Not Working
**Symptoms:** Login shows "failed" or doesn't redirect

**Causes:**
- JWT cookie not being set
- Cookie Secure flag blocking cookies over HTTP
- CORS issues

**Solution:**
```bash
# Check browser DevTools → Network → Response Headers
# Should see: Set-Cookie: jwt=...

# If cookie not set, check API logs
docker compose logs users-api | grep -i login

# Verify cookie settings in AuthController:
# - HttpOnly = true
# - Secure = false (for HTTP) or Request.IsHttps (auto-detect)
# - SameSite = Lax

# Test login API directly
curl -X POST http://localhost/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"yourpassword"}' \
  -v

# Should return 200 OK with Set-Cookie header
```

### Issue 7: Port 80 Already in Use
```bash
# Check what's using port 80
sudo netstat -tulpn | grep :80

# If Apache/httpd is running
sudo systemctl stop httpd
sudo systemctl disable httpd

# If other Docker container using port 80
docker ps
docker stop container-name

# Then restart your containers
docker compose up -d
```

### Issue 8: Out of Disk Space
```bash
# Check disk usage
df -h

# Clean up Docker resources
docker system prune -a --volumes

# WARNING: This removes:
# - All stopped containers
# - All unused networks
# - All unused images
# - All unused volumes

# Less aggressive cleanup (removes only stopped containers)
docker container prune

# Remove unused images
docker image prune -a
```

---

## Part 6: Useful Docker Commands

### Container Management
```bash
# Start containers
docker compose up -d

# Stop containers
docker compose stop

# Restart containers
docker compose restart

# Remove containers
docker compose down

# Force recreate containers
docker compose up -d --force-recreate

# Scale specific service (not used in this app)
docker compose up -d --scale users-api=3
```

### Image Management
```bash
# List images
docker images

# Remove specific image
docker rmi image-name

# Remove all unused images
docker image prune -a

# Build without cache
docker compose build --no-cache

# Pull latest base images
docker compose pull
```

### Logs and Debugging
```bash
# View logs
docker compose logs

# Follow logs
docker compose logs -f

# Logs for specific service
docker compose logs nginx

# Last N lines
docker compose logs --tail=50

# Logs with timestamps
docker compose logs -t

# Execute command in running container
docker compose exec nginx sh
docker compose exec users-api bash

# Run one-off command
docker compose run users-api dotnet --version
```

### Network and Volumes
```bash
# List networks
docker network ls

# Inspect network
docker network inspect kfcvue_default

# List volumes
docker volume ls

# Inspect volume
docker volume inspect kfcvue_volume-name

# Remove unused volumes
docker volume prune
```

### Resource Monitoring
```bash
# Real-time stats
docker stats

# Container details
docker inspect container-name

# Container processes
docker compose top

# Disk usage
docker system df
```

---

## Part 7: Production Best Practices

### Security
```bash
# Run Docker as non-root user
sudo usermod -aG docker $USER

# Logout and login for group to take effect
exit
# SSH back in

# Now you can run docker without sudo
docker ps

# Keep secrets out of git
# Ensure .env is in .gitignore
grep .env .gitignore

# Restrict .env file permissions
chmod 600 .env

# Regularly update Docker and images
sudo dnf update -y docker-ce docker-ce-cli containerd.io
docker compose pull
docker compose up -d
```

### Monitoring
```bash
# Set up log rotation (prevent logs from filling disk)
sudo nano /etc/docker/daemon.json

# Add:
{
  "log-driver": "json-file",
  "log-opts": {
    "max-size": "10m",
    "max-file": "3"
  }
}

# Restart Docker
sudo systemctl restart docker

# Monitor container health
docker compose ps

# Set up automated health checks (already in docker-compose.yml)
# Check health status
docker inspect --format='{{.State.Health.Status}}' kfcweb-users-api
```

### Backups
```bash
# Backup .env file
cp .env .env.backup

# Backup nginx config
cp nginx/nginx.conf nginx/nginx.conf.backup

# Backup entire project (excluding node_modules, docker volumes)
tar -czf ~/kfcvue-backup-$(date +%Y%m%d).tar.gz \
  --exclude='node_modules' \
  --exclude='dist' \
  --exclude='obj' \
  --exclude='bin' \
  ~/source/repos/kfcvue
```

### Automation
```bash
# Create update script
nano ~/update-kfcweb.sh

# Add:
#!/bin/bash
cd ~/source/repos/kfcvue
git pull
docker compose build
docker compose up -d
docker compose logs --tail=100

# Make executable
chmod +x ~/update-kfcweb.sh

# Run update
~/update-kfcweb.sh
```

---

## Part 8: Quick Reference

### Essential Commands
```bash
# Deploy from scratch
cd ~/source/repos/kfcvue
docker compose build
docker compose up -d

# Update application
git pull
docker compose build
docker compose up -d

# View status
docker compose ps
docker compose logs

# Restart everything
docker compose restart

# Stop everything
docker compose down

# Full rebuild
docker compose down
docker compose build --no-cache
docker compose up -d
```

### File Locations
```
~/source/repos/kfcvue/              # Project root
├── .env                             # Environment variables (NOT in git)
├── docker-compose.yml               # Container orchestration
├── nginx/
│   ├── nginx.conf                   # Nginx configuration
│   └── ssl/                         # SSL certificates (for HTTPS)
├── MyApp.Frontend/
│   ├── .env.production              # Frontend build environment
│   └── dist/                        # Built frontend (created by Docker)
├── MyUsers.Api/                     # Users & Auth API
└── MySettings.Api/                  # Settings API
```

### Port Reference
- **80** - HTTP (nginx)
- **443** - HTTPS (nginx, not used yet)
- **5000** - Users API (internal only)
- **5001** - Settings API (internal only)

### Log Files
```bash
# Container logs (in memory, not files)
docker compose logs

# System Docker logs
journalctl -u docker

# Nginx access logs (if enabled in config)
docker compose exec nginx cat /var/log/nginx/access.log

# Nginx error logs (if enabled in config)
docker compose exec nginx cat /var/log/nginx/error.log
```

---

## Summary

**You've successfully set up:**
✅ Rocky Linux 9 server  
✅ Docker and Docker Compose  
✅ Firewall configuration  
✅ GitHub repository cloned  
✅ Environment variables configured  
✅ 3 Docker containers running (nginx, users-api, settings-api)  
✅ Full-stack web application deployed  

**Your site is now live at:** `http://your-server-ip`

**To update in the future:**
```bash
cd ~/source/repos/kfcvue
git pull
docker compose build
docker compose up -d
```

**For HTTPS setup:** See `HTTPS_MIGRATION_GUIDE.md`

**Need help?** Check the Troubleshooting section above or run `docker compose logs` to see what's happening.

**Happy deploying! 🚀**
