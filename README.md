# UdpGame — сетевой прототип

Учебный проект: UDP-клиент/сервер на C# (.NET 8) для будущей мультиплеерной игры.

## Запуск
dotnet run --project src/Server
dotnet run --project src/Client

## Документация
- [Архитектура](docs/Architecture_Design.md)
- [Спецификация протокола](docs/Protocol_Specification.md)

## Возможности
- Команды MOVEMENT и SHOOT, ответы сервера
- Логирование команд (server.log)
- CRC32 в заголовке, авторизация (alice/123, bob/qwerty)