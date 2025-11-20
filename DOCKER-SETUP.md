# Simple Docker Deployment Guide

## Quick Setup on Linux Server

1. **Install Docker** (if not already installed)
```bash
# Rocky Linux
sudo dnf install docker-ce docker-ce-cli containerd.io docker-compose-plugin -y
sudo systemctl start docker
sudo systemctl enable docker

# Ubuntu/Debian
sudo apt install docker.io docker-compose -y
sudo systemctl start docker
sudo systemctl enable docker
```

2. **Upload your code to server**
```bash
# Create directory
mkdir -p ~/kfcweb
cd ~/kfcweb

# Upload files via SCP or git clone
```

3. **Create .env file with your secrets**
```bash
cp .env.example .env
nano .env
# Edit and add your database password and JWT key
```

4. **Build and run**
```bash
docker compose up -d --build
```

5. **Check status**
```bash
docker compose ps
docker compose logs -f
```

## Your application will be available at:
- Frontend + APIs: `http://YOUR_SERVER_IP`

nginx handles routing:
- Frontend: `http://YOUR_SERVER_IP/`
- APIs: `http://YOUR_SERVER_IP/api/*`

## Update Frontend URLs
In `MyApp.Frontend/.env.production`:
```
VITE_API_URL=http://YOUR_SERVER_IP
VITE_SETTINGS_API_URL=http://YOUR_SERVER_IP
```

## Management Commands
```bash
# View logs
docker compose logs -f

# Restart
docker compose restart

# Stop
docker compose down

# Rebuild after code changes
docker compose up -d --build
```
