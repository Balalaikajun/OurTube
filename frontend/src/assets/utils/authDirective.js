export const authDirective = {
  mounted(el, binding) {
    const isAuth = Boolean(localStorage.getItem('userData'));
    const shouldShow = binding.value !== false; // По умолчанию true
    
    if ((shouldShow && !isAuth) || (!shouldShow && isAuth)) {
      el.remove(); // или el.style.display = 'none'
    }
  }
};