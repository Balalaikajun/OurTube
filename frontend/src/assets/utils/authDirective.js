export const authDirective = {
    mounted(el) {
      const isAuth = Boolean(localStorage.getItem('userData'));
      if (!isAuth) {
        el.remove(); // или el.style.display = 'none'
      }
    }
  };