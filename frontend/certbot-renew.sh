echo "[INFO] Renewing cert..."
certbot renew --nginx --quiet
nginx -s reload