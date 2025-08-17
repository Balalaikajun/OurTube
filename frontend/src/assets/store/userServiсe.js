import api from '@/assets/utils/api.js'

export const fetchUserData = async () => {
  try {
    const response = await api.get('/users/me')
    console.log(response.data)
    return response.data
  } catch (err) {
    console.error('Ошибка при получении данных пользователя:', err)
    return null
  }
}

export const saveUserDataToLocalStorage = async (isAuth = false) => {
  console.log(isAuth)
  if (!localStorage.getItem('userData') && !isAuth) return
  const userData = await fetchUserData()
  if (userData) {
    localStorage.setItem('userData', JSON.stringify(userData))
    console.log('userData', localStorage.getItem('userData'))
    return userData
  }
  return null
}