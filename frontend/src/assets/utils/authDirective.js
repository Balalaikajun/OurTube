export const authDirective = {
  mounted(el, binding) {
    const checkAuth = () => {
      const isAuth = Boolean(localStorage.getItem('userData'));
      const shouldShow = binding.value !== false;
      
      if ((shouldShow && !isAuth) || (!shouldShow && isAuth)) {
        el.style.display = 'none';
      } else {
        el.style.display = '';
      }
    };

    // Проверяем сразу
    checkAuth();
    
    // Добавляем обработчик событий для обновлений localStorage
    window.addEventListener('storage', checkAuth);
    
    // Очищаем обработчик при демонтировании
    el._authDirectiveCleanup = () => {
      window.removeEventListener('storage', checkAuth);
    };
  },
  unmounted(el) {
    if (el._authDirectiveCleanup) {
      el._authDirectiveCleanup();
    }
  }
};