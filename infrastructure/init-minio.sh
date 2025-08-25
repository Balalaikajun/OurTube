#!/bin/bash
set -e

# Значения по умолчанию, если переменные не заданы
MINIO_ENDPOINT=http://minio:9000
MINIO_ACCESS_KEY=${MINIO_ROOT_USER}
MINIO_SECRET_KEY=${MINIO_ROOT_PASSWORD}
VIDEO_BUCKET=${MINIO__VIDEOBUCKET}
USER_BUCKET=${MINIO__USERBUCKET}

# Экспортируем root-пользователя для mc
export MINIO_ROOT_USER=$MINIO_ACCESS_KEY
export MINIO_ROOT_PASSWORD=$MINIO_SECRET_KEY

# Настройка alias для MinIO client (mc)
until mc alias set myminio $MINIO_ENDPOINT $MINIO_ACCESS_KEY $MINIO_SECRET_KEY
do
    echo "Waiting for MinIO to start..."
    sleep 2
done

# Создание бакетов, если их нет
mc mb --ignore-existing myminio/$VIDEO_BUCKET
mc mb --ignore-existing myminio/$USER_BUCKET

# Публичная read-only политика
mc anonymous set download myminio/$VIDEO_BUCKET
mc anonymous set download myminio/$USER_BUCKET

echo "Buckets and policies configured successfully!"
