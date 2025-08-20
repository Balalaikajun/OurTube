# OurTube.deployment

## Настройки

### Общее

Далее перечислен общий список переменных окружения приложения, которые вы можете встретить в ENV или настройках, их
примеры и описание.

| Переменная                                  | Где используется                      | Описание / назначение                                         | Значение (пример)                                                            |
|---------------------------------------------|---------------------------------------|---------------------------------------------------------------|------------------------------------------------------------------------------|
| **ASPNETCORE\_ENVIRONMENT**                 | Backend (.NET)                        | Указывает окружение приложения (Development / Production).    | Production                                                                   |
| **ALLOWED\_HOSTS**                          | Backend (.NET)                        | Список разрешённых хостов для ASP.NET Core.                   | localhost                                                                    |
| **CORS\_\_ALLOWEDORIGINS**                  | Backend (.NET)                        | Разрешённые источники для CORS-запросов.                      | http://localhost;                                                            |
| **DOMAIN**                                  | Frontend                              | Основной домен приложения.                                    | localhost                                                                    |
| **POSTGRES\_DB**                            | PostgreSQL                            | Название базы данных.                                         | OurTube                                                                      |
| **POSTGRES\_USER**                          | PostgreSQL                            | Пользователь базы данных.                                     | postgres                                                                     |
| **POSTGRES\_PASSWORD**                      | PostgreSQL                            | Пароль пользователя базы данных.                              | postgres                                                                     |
| **POSTGRES\_HOST**                          | PostgreSQL                            | Хост PostgreSQL сервиса (имя контейнера или адрес).           | postgres                                                                     |
| **POSTGRES\_PORT**                          | PostgreSQL                            | Порт подключения к PostgreSQL.                                | 5432                                                                         |
| **CONNECTIONSTRINGS\_\_DEFAULTSCONNECTION** | Backend (.NET)                        | Строка подключения к базе данных.                             | Host=postgres;Port=5432;Database=OurTube;Username=postgres;Password=postgres |
| **MINIO\_ROOT\_USER**                       | MinIO контейнер                       | Админ-пользователь MinIO.                                     | minio                                                                        |
| **MINIO\_ROOT\_PASSWORD**                   | MinIO контейнер                       | Пароль для администратора MinIO.                              | minio1234                                                                    |
| **MINIO\_SERVER\_URL**                      | MinIO контейнер                       | URL MinIO для доступа внутри контейнера или локально.         | http://localhost:9000                                                        |
| **MINIO\_BROWSER\_REDIRECT\_URL**           | MinIO контейнер                       | URL для веб-интерфейса MinIO.                                 | http://localhost:9090                                                        |
| **MINIO__VIDEOBUCKET**                      | Init-Minio контейнер / Backend (.NET) | Название бакета для видео                                     | videos                                                                       |
| **MINIO__USERBUCKET**                       | Init-Minio контейнер / Backend (.NET) | Название бакета для пользователей                             | users                                                                        |
| **MINIO\_\_ACCESSKEY**                      | Backend (.NET)                        | Ключ доступа для приложения к MinIO.                          | minio                                                                        |
| **MINIO\_\_SECRETKEY**                      | Backend (.NET)                        | Секретный ключ для приложения к MinIO.                        | minio1234                                                                    |
| **MINIO\_\_ENDPOINT**                       | Backend (.NET)                        | Host:port сервера Minio, для отправки запросов.               | minio:9000                                                                   |
| **SMTP\_\_SERVER**                          | Backend (.NET)                        | SMTP сервер для отправки почты.                               | smtp.yourhost.com                                                            |
| **SMTP\_\_PORT**                            | Backend (.NET)                        | Порт SMTP сервера.                                            | 587                                                                          |
| **SMTP\_\_EMAIL**                           | Backend (.NET)                        | Email отправителя.                                            | youremail                                                                    |
| **SMTP\_\_PASSWORD**                        | Backend (.NET)                        | Пароль SMPT. (Не пароль от электронной почты)                 | secure-smtp-password                                                         |
| **VITE\_API\_BASE\_URL**                    | Frontend (Vite)                       | Базовый URL / Префикс API для фронтенда.                      | https://localhost:7027                                                       |
| **VITE\_MINIO\_BASE\_URL**                  | Frontend (Vite)                       | URL / префикс для доступа к MinIO-ресурсам (картинки, видео). | http://localhost:9000                                                        |

### Backend

| Раздел             | Ключ            | Описание / назначение                                                                       | Значение (пример)                 | Где используется |
|--------------------|-----------------|---------------------------------------------------------------------------------------------|-----------------------------------|------------------|
| **DataProtection** | KeysPath        | Путь, где .NET хранит ключи DataProtection для шифрования данных (cookies, токенов и т.д.). | bin/protectionKeys                | Backend (.NET)   |
| **VideoSettings**  | Resolutions     | Список разрешений для кодирования видео                                                     | \["240","360","480","720","1080"] | Backend (.NET)   |
| **FFmpeg**         | AutoDownload    | Флаг автозагрузки FFmpeg, если не установлен.                                               | true                              | Backend (.NET)   |
|                    | ExecutablesPath | Путь к исполняемым файлам FFmpeg для конвертации видео.                                     | bin/ffmpeg                        | Backend (.NET)   |

## Развёртывание

Далее будут приведены руководства по развёртыванию различных вариантов окружения.

### Development

Вариант развёртывания для разработчиков предполагает следующие элементы:

- Локально развёрнутый фронтенд
- Локально развёрнутый бэкенд
- PostgreSQL в контейнере
- Minio в контейнере

#### 0. Подготовка окружения

- Docker ≥ 27.1.1
- Docker Compose ≥ 2.29.2
- Git ≥ 2.46.0
- NET SDK 9.0
- Node.js 20.x

Клонируйте репозиторий и перейдите в папку репозитория

```bash
  git clone https://github.com/Balalaikajun/OurTube.git
  cd OurTube
  git checkout develop
```

#### 1. Развёртывание инфраструктуры

##### 1.1. Создайте переменных окружения при помощи команды

```bash
    cd infrastructure
    cp .env.development.example .env.development
```

##### 1.2. Разверните базовое окружение

```bash
    cd infrastructure
    docker-compose -f docker-compose.yml -f docker-compose.development.yml up -d
```

#### 2. Развёртывание бэкенда

##### 2.1. Настройте приложение

Настройте секреты сами по примеру из backend/src/OurTube.Api/secrets.example.json или с помощью команд.

- Перейдите в проект API:

```bash
    cd backend/src/OurTube.Api
```

- Инициализируйте user-secrets (один раз для проекта):

```bash
    dotnet user-secrets init
```

- Загрузите пример секрета (Windows PowerShell):

```bash
    type .\secrets.example.json | dotnet user-secrets set
```

- Linux / macOS:

```bash
cat ./secrets.example.json | dotnet user-secrets set --json @-
```

При необходимости вы можете поменять прочие настройки приложения в appsettings.json и appsettings.Development.json по
пути backend/src/OurTube.Api/.

##### 2.2. Запустите приложение

```bash
    cd backend/src/OurTube.Api
    dotnet restore
    dotnet watch run 
```

#### 3. Развёртывание фронтэнда

##### 3.1. Настройте приложение

Настройте секреты сами по примеру из frontend/.env.example или с помощью команды:

```bash
    cd frontend
    cp .env.example .env 
```

##### 4. Доступ к приложению

- Приложение: http://localhost:5174
- Документация Swagger: http://localhost:7027/swagger
- Админка MinIO: http://localhost:9090
    - Login: minio
    - Password: minio1234
- PostgreSQL:
    - Host: localhost
    - Port: 5432
    - Login: postgres
    - Password: postgres

### Staging

Данный вариант окружения предполагает развёртывание на http, при необходимости вы можете предоставить к нему удалённый
доступ, для чего понадобиться указать ваш хост в переменных окружения.

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

### Production

Данный вариант окружения предполагает развёртывание на https и требует от вас наличия сертификатов.

#### 0. Требования

- Docker ≥ 27.1.1
- Docker Compose ≥ 2.29.2
- Git ≥ 2.46.0
- Подготовленные ssl сертификаты на ваш домен
    - *-crt.pem
    - *-key.pem
    - *-chain.pem

#### 1. Клонируйте и перейдите в папку репозитория

```bash
  git clone https://github.com/Balalaikajun/OurTube.git
  cd OurTube
  git checkout develop
```

#### 2. Настройте приложение

##### 2.1. Добавьте переменные окружения

Создайте переменных окружения при помощи команды и донастройте при необходимости.

```bash
    cd infrastructure
    cp .env.production.example .env.production
```

##### 2.2. Укажите ваш домен в переменных окружения infrastructure/.env.production:3

##### 2.3. Скопируйте сертификаты в папку infrastructure/certs

##### 2.4. Укажите названия сертификатов в infrastructure/conf.d/https.conf:25

#### 3. Запустите приложение

```bash
    cd infrastructure
    docker-compose -f docker-compose.yml -f docker-compose.production.yml up -d
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
    docker-compose -f docker-compose.yml -f docker-compose.production.yml down
```
