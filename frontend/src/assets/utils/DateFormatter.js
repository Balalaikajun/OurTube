export function formatRussianDate(dateString) 
{
    if (!dateString) return '';
    
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('ru-RU', {
      day: 'numeric',
      month: 'long',
      year: 'numeric'
    })
      .format(date)
      .replace(' г.', '');
}