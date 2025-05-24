# Secure Document Storage System

A secure document storage web application built with ASP.NET Core Web API (backend) and a frontend (e.g., Angular/React). It supports user authentication with JWT tokens, file upload/download with versioning, and stores documents in SQL Server.

---

## Features

- User registration and login with JWT authentication
- Upload documents with version control per user
- List latest document versions
- Download documents securely
- Swagger API documentation available
- CORS enabled for frontend integration

---

## Technologies Used

- ASP.NET Core 7 Web API
- Entity Framework Core with SQL Server
- JWT Authentication
- Swagger for API documentation
- CORS for frontend-backend communication

---

## Prerequisites

- [.NET 7 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or any SQL Server instance
- (Optional) Postman or similar tool to test APIs
- Your frontend app running on `http://localhost:5500` (adjust CORS policy accordingly)

---

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/yourusername/secure-document-storage.git
cd secure-document-storage
