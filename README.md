Мессенджер API (Onion Architecture)
Этот проект — бэкенд RESTful API для мессенджера, разработанный на ASP.NET Core с использованием Onion Architecture. Он предоставляет функционал для регистрации и авторизации пользователей, управления сообщениями и чата в реальном времени через SignalR. Проект использует Entity Framework Core для работы с PostgreSQL и JWT для авторизации.
Содержание

Возможности
Технологии
Установка
Запуск с Docker
Использование
Структура проекта
Контрибьютинг
Лицензия

Возможности

REST API для регистрации, авторизации и управления пользователями.
CRUD-операции для сообщений между пользователями.
Чат в реальном времени через SignalR (/chat хаб).
JWT-авторизация для защиты эндпоинтов.
Интеграция с PostgreSQL через Entity Framework Core.
Поддержка миграций базы данных.
Интерактивная документация API через Swagger UI.
Контейнеризация с Docker и Docker Compose.

Технологии

C# и ASP.NET Core 9.0.
Entity Framework Core: ORM для работы с PostgreSQL.
SignalR: Для чата в реальном времени.
JWT: Для авторизации.
PostgreSQL: База данных.
Swashbuckle/Swagger: Для документации API.
Microsoft.Extensions.DependencyInjection: Для внедрения зависимостей.

Установка

Клонируйте репозиторий:
git clone https://github.com/fried-boiled-onions/backend.git
cd backend
git checkout dev
cd Backend


Убедитесь, что установлен .NET SDK:Требуется .NET 9.0. Проверьте версию:
dotnet --version


Восстановите зависимости:Установите NuGet-пакеты:
dotnet restore


Настройте PostgreSQL:Убедитесь, что PostgreSQL запущен. Настройте строку подключения в appsettings.json:
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=messenger-db;Username=postgres;Password=yourpassword"
  }
}


Примените миграции базы данных:В папке Backend выполните:
dotnet ef database update

Это применит существующие миграции (например, 20250524211730_InitialCreate.cs).

Запустите приложение:
dotnet run

API будет доступен по адресу https://localhost:5001 или http://localhost:5000 (смотрите Properties/launchSettings.json). Документация API доступна по /swagger. SignalR хаб для чата — по /chat.


Запуск с Docker

Убедитесь, что Docker и Docker Compose установлены.

Настройте docker-compose.yml (например, переменные окружения для PostgreSQL).

Запустите приложение:
docker-compose up --build

API будет доступен по адресу, указанному в docker-compose.yml (обычно http://localhost:5000).


Использование
REST Эндпоинты

Авторизация:

POST /api/auth/register — регистрация пользователя.curl -X POST "http://localhost:5000/api/auth/register" -H "Content-Type: application/json" -d '{"username":"testuser","password":"Test123!"}'

Ответ: { "token": "jwt-token" }
POST /api/auth/login — вход пользователя.curl -X POST "http://localhost:5000/api/auth/login" -H "Content-Type: application/json" -d '{"username":"testuser","password":"Test123!"}'

Ответ: { "token": "jwt-token" }


Пользователи:

GET /api/users — получить список пользователей (требуется JWT).curl -X GET "http://localhost:5000/api/users" -H "Authorization: Bearer <jwt-token>"

Ответ: [ { "id": 1, "username": "testuser" }, ... ]


Сообщения:

GET /api/messages/{userId} — получить сообщения с пользователем (требуется JWT).curl -X GET "http://localhost:5000/api/messages/2" -H "Authorization: Bearer <jwt-token>"

Ответ: [ { "id": 1, "senderId": 1, "senderName": "user1", "receiverId": 2, "receiverName": "user2", "content": "Hello!", "sentAt": "2025-06-02T10:00:00", "isRead": false }, ... ]


Документация API доступна через Swagger UI по адресу /swagger.


SignalR (Чат)

Подключитесь к хабу /chat через WebSocket (требуется JWT-токен).
Пример подключения (JavaScript):const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5000/chat", { accessTokenFactory: () => "<jwt-token>" })
    .build();
connection.start().then(() => console.log("Connected to chat hub"));
connection.on("ReceiveMessage", (message) => console.log("New message:", message));



Структура проекта
Backend/
├── Config/
│   └── JwtSettings.cs        # Настройки JWT
├── Controllers/
│   ├── AuthController.cs     # Эндпоинты для регистрации и входа
│   ├── MessagesController.cs # Эндпоинты для сообщений
│   ├── UsersController.cs    # Эндпоинты для пользователей
├── Data/
│   └── AppDbContext.cs       # Контекст EF Core
├── Hubs/
│   └── ChatHub.cs            # SignalR хаб для чата
├── Migrations/
│   ├── 20250524211730_InitialCreate.cs # Миграции БД
│   └── ...
├── Models/
│   ├── Dtos/
│   │   ├── SendMessageDto.cs # DTO для отправки сообщений
│   │   ├── UserDto.cs        # DTO для пользователей
│   ├── Message.cs            # Модель сообщения
│   ├── User.cs               # Модель пользователя
├── Repositories/
│   ├── MessageRepository.cs  # Репозиторий для сообщений
│   ├── UserRepository.cs     # Репозиторий для пользователей
├── Services/
│   ├── ChatService.cs        # Сервис для чата
│   ├── JwtService.cs         # Сервис для JWT
│   ├── UserService.cs        # Сервис для пользователей
├── Utils/
│   └── PasswordHasher.cs     # Утилита для хеширования паролей
├── appsettings.json          # Конфигурация (БД, JWT)
├── Program.cs                # Точка входа
├── Dockerfile                # Конфигурация Docker
├── docker-compose.yml        # Docker Compose
└── README.md                 # Этот файл

Контрибьютинг

Сделайте форк репозитория.
Создайте ветку (git checkout -b feature/ваша-фича).
Внесите изменения (git commit -m "Добавлена фича").
Запушьте (git push origin feature/ваша-фича).
Откройте Pull Request.

Лицензия
Проект распространяется под лицензией MIT. Подробности в файле LICENSE.
