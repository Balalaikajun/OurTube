#!/usr/bin/env sh
set -e

# Проверяем наличие сертификата
if [ -f /etc/letsencrypt/live/$DOMAIN/fullchain.pem ]; then
  export DO_REDIRECT=1
  echo "▶️ Certs found — enabling HTTPS config and redirect"
  # копируем (активируем) файл https.conf
  cp /etc/nginx/conf.d/https.conf /etc/nginx/conf.d/https.conf.enabled
else
  export DO_REDIRECT=0
  echo "▶️ No certs — HTTP-only mode"
  # удаляем (деактивируем) https.conf, если он был
  rm -f /etc/nginx/conf.d/https.conf.enabled
fi

# Запускаем Nginx в форграунде
nginx -g 'daemon off;'
