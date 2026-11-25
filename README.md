# SentinelAI

A security-focused administration and auditing platform built with ASP.NET Core and PostgreSQL.

SentinelAI demonstrates modern authentication, role-based access control (RBAC), user administration, and tamper-evident audit logging through a web-based administration portal and audit dashboard.

---

## Features

### Authentication

- JWT-based authentication
- Secure password hashing
- Login tracking
- Account enable/disable support

### Authorization

- Role-Based Access Control (RBAC)

Supported roles:

- employee
- hr
- finance
- it
- admin

### User Administration

- View users
- Enable user accounts
- Disable user accounts
- Change user roles
- Track last login timestamps

### Audit System

- Tamper-evident audit chain
- Audit verification endpoint
- Audit dashboard
- Login success logging
- Login failure logging
- Administrative action logging

### Admin Portal

- Secure administrator login
- User management interface
- Role editor
- Account status management

---

## Technology Stack

### Backend

- ASP.NET Core
- C#
- Entity Framework Core
- PostgreSQL

### Security

- JWT Authentication
- Role-Based Authorization
- Password Hashing

### Frontend

- HTML
- CSS
- JavaScript

---

## Project Structure

```text
SentinelAPI
│
├── Controllers
├── Data
├── Models
├── Services
├── Migrations
├── Properties
└── wwwroot
    ├── admin.html
    └── dashboard.html
```

---

## Security Features

- JWT token authentication
- Role-based authorization
- Password hashing
- Account enable/disable support
- Login activity tracking
- Audit logging
- Tamper detection through chained audit records

---

## Admin Portal

The Admin Portal provides:

- Administrator login
- User management
- Role management
- Enable/disable controls
- Last login visibility

Accessible at:

```text
/admin.html
```

---

## Audit Dashboard

The Audit Dashboard provides:

- Audit event history
- Verification status
- Chain integrity checks
- Administrative activity tracking

Accessible at:

```text
/dashboard.html
```

---

## Running the Project

### Prerequisites

- .NET SDK
- PostgreSQL
- Visual Studio

### Setup

1. Clone the repository.
2. Create the PostgreSQL database.
3. Configure ASP.NET User Secrets.
4. Run Entity Framework migrations.
5. Start the application.

### Access

Admin Portal:

```text
https://localhost:7198/admin.html
```

Audit Dashboard:

```text
https://localhost:7198/dashboard.html
```

---

## Development Notes

Sensitive information is stored using ASP.NET User Secrets and is not committed to source control.

Examples include:

- JWT signing keys
- Audit API keys
- Demo passwords
- Database credentials

---

## Future Enhancements

- Session persistence
- Logout functionality
- Password reset workflows
- Audit export capabilities
- Azure deployment
- Single Sign-On (SSO)
- AI model integration

---

## Version

**v1.0**

---

## Author

Built as a portfolio project demonstrating:

- Backend Development
- Authentication & Authorization
- Security Engineering
- Database Design
- Audit Systems
- Administrative Interfaces
- ASP.NET Core Development