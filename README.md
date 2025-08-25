# OurTube

**OurTube** — минимально жизнеспособный продукт (MVP) реализующий базовый функционал видеохостинга: загрузка и
просмотр видео,
комментарии, плейлисты, реакции, история просмотров и рекомендации.

Проект разрабатывался двумя студентами третьего курса, за некоторым исключением разработка проекта заняла порядка 6
месяцев. С принятыми решениями и нашими выводами о проделанной работе можете ознакомиться по ссылкам:

- [Бэкенд](./documentation/backend_review.md)
- [Фронтенд и дизайн](./documentation/frontend.md)

## Демонстрация

![](./documentation/assets/example.gif)

## Функционал

В рамках проекта нашей задачей было реализовать следующий функционал:

- Регистрация и авторизация пользователей
- Загрузка и просмотр видео
- Комментарии к видео
- Плейлисты
- Реакции (лайки/дизлайки) для видео и комментариев
- История просмотров
- Рекомендации и поиск

## Стек технологий

- Backend: ASP.NET 9.0
- Frontend: Vue.js 3.5.x
- База данных: PostgreSQL 17.x
- Хранение файлов: MinIO
- Контейнеризация: Docker, Docker Compose
- Веб-сервер, обратный прокси: Nginx
- Система контроля версий: Git/GitHub

## Архитектура и решения

![img.png](./documentation/assets/Architecture.png)

Приложение состоит из SPA фронтенда на Vue.js, который раздаётся Nginx и через него же проксирует API-запросы
к бэкенду. Бэкенд — монолит, по чистой архитектуре на ASP.NET — реализует всю бизнес-логику, аутентификацию и работу с
данными: структурные
данные хранятся в PostgreSQL, файлы и blob — в MinIO (S3-совместимое объектное хранилище). Всё запускается через
docker-compose для удобного развёртывания.

Подробнее о реализации и проделанной работе работе:

- [Бекэнд и развёртывание](./documentation/backend.md)
- [Фронтенд и дизайн](./documentation/frontend.md)

## Установка

Далее приведена инструкция по развёртыванию стендовой версии проекта, для тестирования. В данной конфигурации не
настроена рассылка почты из-за чего некоторый второстепенный функционал будет недоступен.
Ознакомится с другими вариантами развёртывания можете по [ссылке](./documentation/deployment.md).

#### 0. Требования

- Docker ≥ 27.1.1
- Docker Compose ≥ 2.29.2
- Git ≥ 2.46.0

#### 1. Клонируйте и перейдите в папку репозитория

```bash
  git clone https://github.com/Balalaikajun/OurTube.git
  cd OurTube
  git checkout develop
```

#### 2. Создайте переменных окружения при помощи команды

```bash
    cd infrastructure
    cp .env.staging.example .env.staging
```

#### 3. Запустите приложение

```bash
    cd infrastructure
    docker-compose -f docker-compose.yml -f docker-compose.staging.yml up -d
```

#### 4. Доступ к приложению

- Приложение: http://localhost
- Документация Swagger: http://localhost:8080/swagger
- Админка MinIO: http://localhost:9090
    - Login: minio
    - Password: minio1234
- PostgreSQL:
    - Host: localhost
    - Port: 5432
    - Login: postgres
    - Password: postgres

#### 5. Остановите приложение

```bash
    cd infrastructure
    docker-compose -f docker-compose.yml -f docker-compose.staging.yml down
```