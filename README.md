# OurTube
**OurTube** — учебный видеохостинг (курсовой проект).

Коротко: минимально жизнеспособный продукт (MVP) с базовым функционалом видеохостинга: загрузка и просмотр видео, комментарии, плейлисты, реакции, история просмотров и рекомендации.

## Демонстрация

## Функционал 
В рамках проект нашей задачей было реализовать следующий функционал:
- Регистрация и авторизация пользователей
- Загрузка и просмотр видео
- Комментарии к видео
- Плейлисты
- Реакции (лайки/дизлайки) для видео и комментариев
- История просмотров
- Рекомендации

## Стек технологий
- Backend: ASP.NET 9.0
- Frontend: Vue.js 3.5.x
- База данных: PostgreSQL 17.x
- Хранение файлов: MinIO
- Контейнеризация: Docker, Docker Compose
- Reverse proxy: Nginx

## Архитектура и решения
Подробнее о решении и выводы о проделанной работе:
- [Бекэнд и развёртывание](./Documentation/Backend.md)
- [Фронтенд и дизайн](./Documentation/Frontend.md) 

## Установка
Далее приведена инструкция по развёртыванию стендовой версии проекта, для тестирования. В данной конфигурации не настроена рассылка почты из-за чего некоторый второстепенный функционал будет недоступен.
Ознакомится с другими вариантами развёртывания можете по [ссылке](./Documentation/Backend.md#развёртывание).

0. Требования
- Docker ≥ 27.1.1
- Docker Compose ≥ 2.29.2
- Git ≥ 2.46.0

1. Клонируйте и перейдите в папку репозитория
```bash
  git clone https://github.com/Balalaikajun/OurTube.git
  cd OurTube
  git checkout develop
```
2. Создайте файл стендовых настроек .env.staging в OurTube/infrastructure на основе OurTube/infrastructure/.env.staging.example
```bash
    cd infrastructure
    cp .env.staging.example .env.staging
```
3. Запустить
```bash
    docker-compose -f docker-compose.yml -f docker-compose.staging.yml up -d
```
4. Доступ к приложению
- Приложение: http://localhost
- Документация Swagger: http://localhost:8080/swagger
- Админка MinIO: http://localhost:9090 
  - Login: your-minio-access-key
  - Password: your-minio-secret-key
- PostgreSQL:
  - Host: localhost
  - Port: 5432
  - Login: postgres
  - Password: postgres
5. Остановить
```bash
    docker-compose -f docker-compose.yml -f docker-compose.staging.yml down
```

## Участники
- [Balalaikajun ](https://github.com/Balalaikajun) - Бэкенд и развёртывание 
- [KrakishMusta](https://github.com/KrakishMusta) - Фронтенд и дизайн
