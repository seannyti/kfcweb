FROM node:20-alpine AS builder

WORKDIR /app

# Copy frontend source
COPY MyApp.Frontend/package*.json ./
RUN npm ci

COPY MyApp.Frontend/ ./
RUN npm run build

# Production nginx image
FROM nginx:alpine

# Copy built frontend
COPY --from=builder /app/dist /usr/share/nginx/html

# Copy nginx config
COPY ./nginx/nginx.conf /etc/nginx/nginx.conf

# Set proper permissions
RUN chmod -R 755 /usr/share/nginx/html && \
    chown -R nginx:nginx /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
