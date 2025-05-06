const getUserAvatar = async (userId) => {
    try {
      const response = await api.get(`/api/User/${userId}/avatar`);
      return response.data.url || '/default-avatar.jpg';
    } catch {
      return '/default-avatar.jpg';
    }
  }