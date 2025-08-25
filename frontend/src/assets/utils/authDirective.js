export const authDirective = {
  mounted (el, binding) {
    const checkAuth = () => {
      const isAuth = Boolean(localStorage.getItem('userData'))
      const shouldShow = binding.value !== false

      if ((shouldShow && !isAuth) || (!shouldShow && isAuth)) {
        el.style.display = 'none'
        // Добавляем атрибут для проверки в компоненте
        el.setAttribute('data-unauthorized', 'true')
      } else {
        el.style.display = ''
        el.removeAttribute('data-unauthorized')
      }
    }

    checkAuth()

    // Слушаем изменения в localStorage
    const storageListener = () => checkAuth()
    window.addEventListener('storage', storageListener)

    // Кастомное событие для обновления в текущем окне
    el._authUpdate = () => checkAuth()
    window.addEventListener('auth-update', el._authUpdate)

    el._cleanup = () => {
      window.removeEventListener('storage', storageListener)
      window.removeEventListener('auth-update', el._authUpdate)
    }
  },
  updated (el, binding) {
    if (el._authUpdate) el._authUpdate()
  },
  unmounted (el) {
    if (el._cleanup) el._cleanup()
  }
}