<template>
    <div class="loading-state">
      <div class="spinner"></div>
      <p>Загрузка...</p>
    </div>
  </template>
  
  <script setup>
  // Можно добавить props, если нужно кастомизировать сообщение
  </script>
  
  <style scoped>
  .loading-state {
    display: flex;
    width: 100%;
    flex-direction: column;
    justify-self: center;
    align-items: center;
    gap: 15px;
    padding: 40px 0;
    color: #f39e60;
  }
  
  .spinner {
    width: 40px;
    height: 40px;
    border: 4px solid rgba(243, 158, 96, 0.2);
    border-radius: 50%;
    border-top-color: #f39e60;
    animation: spin 1s linear infinite;
  }
  
  @keyframes spin {
    to { transform: rotate(360deg); }
  }
  </style>