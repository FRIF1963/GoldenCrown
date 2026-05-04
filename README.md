# 💰 Golden Crown API

Учебный REST API-сервис — упрощённый банковский бэкенд на ASP.NET Core.  
Проект охватывает типичный стек коммерческой разработки:

- регистрация и авторизация пользователей с токен-сессиями  
- финансовые операции (пополнение счёта, переводы, история транзакций)  
- Entity Framework + SQL Server  
- кастомный middleware авторизации  
- фоновая очистка сессий  

---

## 📖 Описание проекта

**Golden Crown** предоставляет следующие возможности:

- ✅ Регистрация и авторизация пользователей  
- 🔐 Токен-сессии (живут 1 час, очищаются фоновой службой)  
- 💰 Получение баланса счёта  
- ➕ Пополнение счёта  
- 🔁 Перевод денег другому пользователю  
- 📊 История транзакций (фильтр по дате + пагинация)  
- 🛡️ Защита эндпоинтов через `AuthorizationMiddleware`  

### 🎁 Бонусная часть

- Docker + docker-compose (API, SQL Server, RabbitMQ, consumer)  
- FluentValidation вместо DataAnnotations  
- CQRS + MediatR  
- Поддержка нескольких счетов (multi-currency)  
- Интеграция с RabbitMQ (события транзакций)  

---

## 🚀 Инструкция по запуску

### 1. Требования

- .NET 8 SDK  
- SQL Server Express / LocalDB / Docker  
- SSMS (опционально)  
- Docker Desktop (для бонусной части)  

---

### 2. Клонирование репозитория

git clone https://github.com/your-username/GoldenCrown.git  
cd GoldenCrown  

---

### 3. Настройка базы данных

Отредактируйте `appsettings.json`:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\mssqllocaldb;Database=GoldenCrownDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}

---

### 4. Применение миграций

dotnet ef database update

---

### 5. Запуск API

dotnet run --project GoldenCrown.API

Swagger будет доступен:
https://localhost:7000/swagger  
http://localhost:5000/swagger  

---

### 6. Запуск через Docker (бонус)

docker-compose up -d

- API  
- SQL Server  
- RabbitMQ  
- Consumer  

Миграции применяются автоматически при старте.

---

## 📡 Примеры API-запросов

### 🔑 Авторизация

Все `/finance/*` эндпоинты требуют заголовок:

Authorization: your-token-here

---

### 1. Регистрация

POST /api/user/register

{
  "login": "alexey",
  "name": "Алексей",
  "password": "123456"
}

---

### 2. Авторизация

POST /api/user/login

{
  "login": "alexey",
  "password": "123456"
}

Ответ:

{
  "token": "a1b2c3d4-e5f6-7890-1234-567890abcdef"
}

---

### 3. Получить баланс

GET /api/finance/balance

{
  "balance": 2500.00
}

---

### 4. Пополнить счёт

POST /api/finance/deposit

{
  "amount": 500.00
}

---

### 5. Перевод

POST /api/finance/transfer

{
  "receiverLogin": "elena",
  "amount": 150.00
}

Возможные ошибки:

401 — не авторизован  
404 — получатель не найден  
400 — недостаточно средств  

---

### 6. История транзакций

GET /api/finance/history?from=2025-01-01&to=2025-12-31&limit=10&offset=0

[
  {
    "id": 1,
    "senderId": 1,
    "receiverId": 2,
    "date": "2025-05-04T10:00:00Z",
    "amount": 150.00
  }
]

---

## 🗄️ Структура базы данных

Users:
- Id (int, PK)  
- Login (nvarchar(50))  
- Name (nvarchar(100))  
- Password (nvarchar(100))  

Accounts:
- Id (int, PK)  
- UserId (int, FK)  
- Balance (decimal(18,2))  

Sessions:
- UserId (int, FK)  
- Token (nvarchar(255))  
- ExpiresAt (datetime2)  

Очистка выполняется каждые 10 минут.

Transactions:
- Id (int, PK)  
- SenderId (int, FK)  
- ReceiverId (int, FK)  
- Date (datetime2)  
- Amount (decimal(18,2))  

Связи:
- User ↔ Account (1:1)  
- User ↔ Session (1:1)  
- Transaction → User (sender + receiver)  

---

## 🛠️ Технологии

- ASP.NET Core 8 Web API  
- Entity Framework Core  
- SQL Server  
- Swagger / OpenAPI  
- BackgroundService  
- Middleware (кастомная авторизация)  
