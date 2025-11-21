# HTTPS Migration Guide

## Overview
This guide covers everything you need to change in the codebase when migrating from HTTP to HTTPS with a domain name.

---

## Code Changes Required

### 1. AuthController Cookie Security (Already Fixed ✅)
**File:** `MyUsers.Api/Controllers/AuthController.cs`

The code has been updated to auto-detect HTTPS:

```csharp
Secure = Request.IsHttps, // Auto-detect HTTPS
```

**No manual change needed** - the cookie will automatically become secure when you switch to HTTPS.

---

### 2. Frontend Environment Variables
**File:** `MyApp.Frontend/.env.production`

**CURRENT (HTTP):**
```env
VITE_API_URL=http://23.239.26.52/api
VITE_SETTINGS_API_URL=http://23.239.26.52/api
```

**CHANGE TO (HTTPS):**
```env
VITE_API_URL=https://yourdomain.com/api
VITE_SETTINGS_API_URL=https://yourdomain.com/api
```

Replace `yourdomain.com` with your actual domain name.

---

### 3. Nginx Configuration
**File:** `nginx/nginx.conf`

Add SSL certificate configuration and HTTP to HTTPS redirect:

```nginx
# Redirect HTTP to HTTPS
server {
    listen 80;
    server_name yourdomain.com www.yourdomain.com;
    
    # Redirect all HTTP traffic to HTTPS
    return 301 https://$server_name$request_uri;
}

# HTTPS Server
server {
    listen 443 ssl http2;
    server_name yourdomain.com www.yourdomain.com;

    # SSL Certificate Configuration
    ssl_certificate /etc/nginx/ssl/certificate.crt;
    ssl_certificate_key /etc/nginx/ssl/private.key;
    
    # SSL Settings (Modern Configuration)
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers 'ECDHE-ECDSA-AES128-GCM-SHA256:ECDHE-RSA-AES128-GCM-SHA256:ECDHE-ECDSA-AES256-GCM-SHA384:ECDHE-RSA-AES256-GCM-SHA384';
    ssl_prefer_server_ciphers off;
    
    # Security Headers
    add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;

    # ... rest of your existing configuration (root, locations, etc.) ...
}
```

**Update docker-compose.yml volumes:**
```yaml
nginx:
  volumes:
    - ./nginx/nginx.conf:/etc/nginx/nginx.conf:ro
    - ./nginx/ssl:/etc/nginx/ssl:ro  # Add SSL certificate directory
    - ./dist:/usr/share/nginx/html:ro
```

---

### 4. CORS Configuration (If Needed)
**Files:** `MyUsers.Api/Program.cs` and `MySettings.Api/Program.cs`

If you have specific CORS origins configured, update them from HTTP to HTTPS:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://yourdomain.com") // Change from http:// to https://
              .AllowCredentials()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

**Current configuration:** You're using `AllowAnyOrigin()` which allows all origins. This is fine for testing but consider restricting to your domain in production.

---

## SSL Certificate Setup

### Option 1: Let's Encrypt (Free, Recommended)
Let's Encrypt provides free SSL certificates with automatic renewal.

**On your Rocky Linux server:**

```bash
# Install certbot
sudo dnf install certbot

# Stop nginx temporarily
docker compose down nginx

# Obtain certificate (replace with your domain)
sudo certbot certonly --standalone -d yourdomain.com -d www.yourdomain.com

# Certificates will be saved to:
# /etc/letsencrypt/live/yourdomain.com/fullchain.pem
# /etc/letsencrypt/live/yourdomain.com/privkey.pem

# Copy certificates to your project
sudo mkdir -p ~/source/repos/kfcvue/nginx/ssl
sudo cp /etc/letsencrypt/live/yourdomain.com/fullchain.pem ~/source/repos/kfcvue/nginx/ssl/certificate.crt
sudo cp /etc/letsencrypt/live/yourdomain.com/privkey.pem ~/source/repos/kfcvue/nginx/ssl/private.key
sudo chown $(whoami):$(whoami) ~/source/repos/kfcvue/nginx/ssl/*

# Restart nginx
docker compose up -d nginx
```

**Auto-renewal:**
```bash
# Add cron job for auto-renewal (runs daily)
sudo crontab -e

# Add this line:
0 3 * * * certbot renew --quiet && cp /etc/letsencrypt/live/yourdomain.com/*.pem ~/source/repos/kfcvue/nginx/ssl/ && docker compose -f ~/source/repos/kfcvue/docker-compose.yml restart nginx
```

### Option 2: Commercial SSL Certificate
If you purchased an SSL certificate from a provider (GoDaddy, Namecheap, etc.):

1. Download your certificate files (usually `.crt` and `.key`)
2. Copy them to `nginx/ssl/` directory
3. Update nginx.conf with the correct filenames

---

## DNS Configuration

**Before switching to HTTPS, ensure your domain points to your server:**

1. Go to your domain registrar's DNS settings
2. Add/Update A record:
   - **Type:** A
   - **Name:** @ (or yourdomain.com)
   - **Value:** 23.239.26.52
   - **TTL:** 3600 (or default)

3. Add/Update A record for www subdomain:
   - **Type:** A
   - **Name:** www
   - **Value:** 23.239.26.52
   - **TTL:** 3600

4. Wait for DNS propagation (can take up to 48 hours, usually much faster)

**Verify DNS propagation:**
```bash
nslookup yourdomain.com
# Should return 23.239.26.52
```

---

## Deployment Checklist

### Step 1: Get Domain & Configure DNS
- [ ] Purchase/configure domain name
- [ ] Point A record to 23.239.26.52
- [ ] Verify DNS with `nslookup yourdomain.com`

### Step 2: Obtain SSL Certificate
- [ ] Stop nginx: `docker compose down nginx`
- [ ] Install certbot: `sudo dnf install certbot`
- [ ] Get certificate: `sudo certbot certonly --standalone -d yourdomain.com -d www.yourdomain.com`
- [ ] Copy certificates to `nginx/ssl/` directory
- [ ] Set proper permissions on certificate files

### Step 3: Update Code
- [ ] Update `MyApp.Frontend/.env.production` with HTTPS URLs
- [ ] Update `nginx/nginx.conf` with SSL configuration and redirect
- [ ] Update `docker-compose.yml` to mount SSL directory
- [ ] Add `nginx/ssl/*.key` and `nginx/ssl/*.pem` to `.gitignore` (if not already)

### Step 4: Rebuild and Deploy
```bash
# On your local machine
git add .
git commit -m "Add HTTPS support with SSL configuration"
git push

# On your server
cd ~/source/repos/kfcvue
git pull
docker compose build frontend
docker compose up -d
```

### Step 5: Verify HTTPS
- [ ] Visit https://yourdomain.com (should load with green padlock)
- [ ] Visit http://yourdomain.com (should redirect to HTTPS)
- [ ] Test login (cookies should work with Secure flag)
- [ ] Test all admin features
- [ ] Check browser console for mixed content warnings

### Step 6: Monitor & Maintain
- [ ] Set up SSL certificate auto-renewal (cron job)
- [ ] Monitor certificate expiry (Let's Encrypt certs expire every 90 days)
- [ ] Test HTTPS regularly

---

## Troubleshooting

### Issue: "NET::ERR_CERT_AUTHORITY_INVALID"
**Cause:** Browser doesn't trust the certificate  
**Solution:** 
- Ensure you're using a certificate from a trusted CA (Let's Encrypt, commercial provider)
- For testing only, you can bypass in Chrome with: `chrome://flags/#allow-insecure-localhost`

### Issue: Mixed Content Warnings
**Cause:** Loading HTTP resources on HTTPS page  
**Solution:**
- Check browser console for specific HTTP resources
- Ensure all API calls use HTTPS
- Update any hardcoded HTTP URLs in frontend

### Issue: Login Not Working After HTTPS
**Cause:** Cookie Secure flag or SameSite settings  
**Solution:**
- Verify `Request.IsHttps` returns `true` in AuthController
- Check browser DevTools → Application → Cookies
- Cookie should have `Secure` flag set

### Issue: 502 Bad Gateway After HTTPS
**Cause:** nginx can't connect to backend APIs  
**Solution:**
- Check docker network: `docker network inspect kfcvue_default`
- Verify all containers running: `docker compose ps`
- Check nginx logs: `docker compose logs nginx`

---

## Security Best Practices

### 1. Update Security Headers
Already included in nginx config above:
- `Strict-Transport-Security` (HSTS) - Force HTTPS for 1 year
- `X-Frame-Options` - Prevent clickjacking
- `X-Content-Type-Options` - Prevent MIME sniffing
- `X-XSS-Protection` - Enable XSS filter

### 2. Restrict CORS Origins
Update `Program.cs` in both APIs to use specific domain instead of `AllowAnyOrigin()`:

```csharp
policy.WithOrigins("https://yourdomain.com")
      .AllowCredentials()
      .AllowAnyMethod()
      .AllowAnyHeader();
```

### 3. Update Cookie Settings
Cookie security is already handled by `Request.IsHttps` - no additional changes needed.

### 4. Keep Secrets Out of Git
Ensure `.gitignore` includes:
```
.env
.env.*
!.env.example
nginx/ssl/*.key
nginx/ssl/*.pem
nginx/ssl/*.crt
```

---

## Summary

**What Changes in Code:**
1. ✅ `AuthController.cs` - Already auto-detects HTTPS (no change needed)
2. ⚠️ `MyApp.Frontend/.env.production` - Change URLs to `https://yourdomain.com/api`
3. ⚠️ `nginx/nginx.conf` - Add SSL config and HTTP redirect
4. ⚠️ `docker-compose.yml` - Add SSL volume mount

**What Doesn't Change:**
- All backend API code (no changes needed)
- Database connection strings
- JWT settings
- Frontend components/views
- Admin functionality

**External Requirements:**
- Domain name pointed to 23.239.26.52
- SSL certificate (Let's Encrypt recommended)
- DNS propagation time (up to 48 hours)

That's it! The codebase is already HTTPS-ready. Just update the URLs, add SSL certificates, and you're good to go. 🚀
