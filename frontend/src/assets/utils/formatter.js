function countFormatter (count, context = '') {
  if (!count && count !== 0) return '0'

  // Для всех контекстов сначала форматируем число
  let formattedCount = count > 1000 ? (count / 1000).toFixed(1) : count.toString()

  // Добавляем правильные склонения в зависимости от контекста
  switch (context) {
    case 'subs':
      return formatWithDeclension(count, formattedCount, ['подписчик', 'подписчика', 'подписчиков'])

    case 'views':
      return formatWithDeclension(count, formattedCount, ['просмотр', 'просмотра', 'просмотров'])

    case '':
      return formattedCount

    case 'comments':
      return formatWithDeclension(count, formattedCount, ['комментарий', 'комментария', 'комментариев'])

    default:
      return formattedCount
  }
}

function formatWithDeclension (count, formattedCount, words) {
  const cases = [2, 0, 1, 1, 1, 2]
  const wordIndex = (count % 100 > 4 && count % 100 < 20)
    ? 2
    : cases[Math.min(count % 10, 5)]

  if (count > 1000) return `${formattedCount} тыс. ${words[wordIndex]}`
  return `${formattedCount} ${words[wordIndex]}`
}

function formatRussianDate (dateString) {
  if (!dateString) return ''

  const date = new Date(dateString)
  return new Intl.DateTimeFormat('ru-RU', {
    day: 'numeric',
    month: 'long',
    year: 'numeric'
  })
    .format(date)
    .replace(' г.', '')
}

const formatDuration = (duration) => {
  if (!duration) return '0:00'

  // Разбиваем строку на часы, минуты, секунды
  const parts = duration.split(':')
  if (parts.length !== 3) return duration

  let [hours, minutes, seconds] = parts.map(Number)

  // Округляем секунды в большую сторону
  seconds = Math.ceil(seconds)
  if (seconds === 60) {
    seconds = 0
    minutes += 1
  }

  // Форматируем в зависимости от наличия часов
  if (hours > 0) {
    return `${hours}:${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`
  } else {
    return `${minutes}:${seconds.toString().padStart(2, '0')}`
  }
}
export default {
  formatRussianDate,
  countFormatter,
  formatDuration
}