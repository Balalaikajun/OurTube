# Сборка фронта
FROM node:18-alpine AS build-stage

WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
ARG VITE_API_BASE_URL
ARG VITE_MINIO_BASE_URL
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL
ENV VITE_MINIO_BASE_URL=$VITE_MINIO_BASE_URL
RUN npm run build

# Nginx
FROM nginx:stable-alpine AS production-stage

RUN apk add --no-cache curl bash

COPY --from=build-stage /app/dist /usr/share/nginx/html

EXPOSE 80

ENTRYPOINT ["nginx"]
CMD ["-g", "daemon off;"]
