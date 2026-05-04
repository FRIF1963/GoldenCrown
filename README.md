# 💰 Golden Crown API

Учебный REST API-сервис — упрощённый банковский бэкенд на ASP.NET Core. Проект охватывает типичный стек коммерческой разработки: регистрацию и авторизацию пользователей с токен-сессиями, финансовые операции (пополнение счёта, переводы, история транзакций), Entity Framework + SQL Server, кастомный авторизационный middleware и фоновую очистку сессий.

---

## 📖 Описание проекта

Golden Crown предоставляет следующие возможности:

- Регистрация и авторизация пользователей
- Токен-сессии (живут 1 час, автоматически удаляются фоновой службой)
- Получение баланса счёта
- Пополнение счёта
- Перевод денег другому пользователю
- История транзакций с фильтром по дате и пагинацией
- Защита финансовых эндпоинтов через кастомный `AuthorizationMiddleware`

**Бонусная часть** (реализована дополнительно):
- Docker + docker-compose (API, SQL Server, RabbitMQ, консюмер)
- FluentValidation вместо DataAnnotations
- CQRS с MediatR
- Поддержка нескольких счетов в разных валютах
- Интеграция с RabbitMQ (отправка событий о транзакциях)

---

## 🚀 Инструкция по запуску

### 1. Требования

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads) (или LocalDB / Docker)
- [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/ssms/download-sql-server-management-studio-ssms) (опционально)
- Для бонусной части: [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### 2. Клонирование репозитория

bash
git clone https://github.com/your-username/GoldenCrown.git
cd GoldenCrown
### 3. Настройка базы данных
Отредактируйте строку подключения в appsettings.json:

json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=GoldenCrownDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}

### 4. Применение миграций
В Package Manager Console или терминале:

bash
dotnet ef database update
### 5. Запуск API
bash
dotnet run --project GoldenCrown.API
Swagger UI будет доступен по адресу:
https://localhost:7000/swagger (или http://localhost:5000/swagger)

### 6. Запуск через Docker (бонус)
bash
docker-compose up -d
Приложение, SQL Server, RabbitMQ и консюмер запустятся вместе.
Миграции накатятся автоматически при старте API.

## 📡 Примеры API запросов
Все финансовые эндпоинты (/finance/*) требуют передачи токена в заголовке:

text
Authorization: your-token-here
### 1. Регистрация
POST /api/user/register

json
{
  "login": "alexey",
  "name": "Алексей",
  "password": "123456"
}
Ответ: 200 OK

### 2. Авторизация
POST /api/user/login

json
{
  "login": "alexey",
  "password": "123456"
}
Ответ:

json
{
  "token": "a1b2c3d4-e5f6-7890-1234-567890abcdef"
}
### 3. Получить баланс
GET /api/finance/balance

Заголовок: Authorization: a1b2c3d4-e5f6-7890-1234-567890abcdef

Ответ:

json
{
  "balance": 2500.00
}
### 4. Пополнить счёт
POST /api/finance/deposit

Заголовок: Authorization: ...

json
{
  "amount": 500.00
}
Ответ: 200 OK

### 5. Перевод другому пользователю
POST /api/finance/transfer

Заголовок: Authorization: ...

json
{
  "receiverLogin": "elena",
  "amount": 150.00
}
Ответ: 200 OK

Возможные ошибки:

401 – не авторизован

404 – получатель не найден

400 – недостаточно средств

### 6. История транзакций
**GET /api/finance/history?from=2025-01-01&to=2025-12-31&limit=10&offset=0**

Заголовок: Authorization: ...

Ответ:

json
[
  {
    "id": 1,
    "senderId": 1,
    "receiverId": 2,
    "date": "2025-05-04T10:00:00Z",
    "amount": 150.00
  },
  ...
]
## 🗄️ Структура базы данных
Проект использует Entity Framework Core с подходом Code First. Ниже перечислены основные сущности:

1. Users
Колонка	Тип	Описание
Id	int (PK)	Уникальный идентификатор
Login	nvarchar(50)	Логин пользователя
Name	nvarchar(100)	Имя
Password	nvarchar(100)	Хеш пароля
2. Accounts
Колонка	Тип	Описание
Id	int (PK)	
UserId	int (FK)	Ссылка на пользователя (1:1)
Balance	decimal(18,2)	Текущий баланс счёта
3. Sessions
Колонка	Тип	Описание
UserId	int (FK)	
Token	nvarchar(255)	Уникальная строка сессии
ExpiresAt	datetime2	UTC-время истечения (через 1ч)
Фоновая служба каждые 10 минут удаляет сессии с ExpiresAt < UTC_Now.

4. Transactions
Колонка	Тип	Описание
Id	int (PK)	
SenderId	int (FK)	null для пополнения
ReceiverId	int (FK)	
Date	datetime2	UTC-время транзакции
Amount	decimal(18,2)	
Связи:

User ←→ Account (один к одному)

User ←→ Session (один к одному)

Transaction → User (отправитель) и → User (получатель)

## 🛠️ Технологии
ASP.NET Core 8 Web API

Entity Framework Core (SQL Server)

Swagger / OpenAPI

BackgroundService (очистка сессий)

Middleware (кастомная авторизация)