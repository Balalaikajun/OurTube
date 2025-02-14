# Развёртывание 
1. Установить PostgreSQL на компьютер;
2. В файле OurTube\backend\src\OurTube.Api\appsettings.json отредактировать дефолтную строку полключения, обновив имя, пароль, прочее по необходимости;
3. Открыть консоль в дирректории \OurTube\backend;
4. Исполнить команду  dotnet ef database update --project src/OurTube.Infrastructure --startup-project src/OurTube.Api  --context ApplicationDbContext
5. Запустить проект Api;
