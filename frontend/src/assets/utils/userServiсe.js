const getUserAvatar = async (userId) => {
    try {
      const response = await api.get(`/User/${userId}/avatar`);
      return response.data.url || '/default-avatar.jpg';
    } catch {
      return '/default-avatar.jpg';
    }
  }