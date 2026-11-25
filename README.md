# SentinelAI

SentinelAI is a security-focused ASP.NET Core application that demonstrates authentication, authorization, user administration, and tamper-evident audit logging.

## Features

### Authentication

- JWT-based authentication
- Password hashing with secure storage
- Login tracking
- Account enable/disable controls

### Authorization

- Role-Based Access Control (RBAC)
- Roles:
  - employee
  - hr
  - finance
  - it
  - admin

### User Administration

- View users
- Change user roles
- Enable accounts
- Disable accounts
- Track last login timestamps

### Audit System

- Tamper-evident audit chain
- Audit verification endpoint
- Audit dashboard
- Login success logging
- Login failure logging
- Administrative action logging

### Admin Portal

- Admin login
- User management interface
- Role editor
- Account status management

## Technology Stack

- ASP.NET Core
- C#
- PostgreSQL
- Entity Framework Core
- JWT Authentication
- HTML
- JavaScript

## Project Structure

```text
Controllers/
Data/
Models/
Services/
wwwroot/
 ├── dashboard.html
 └── admin.html
```

## Security Features

- JWT token authentication
- Role-based authorization
- Password hashing
- Account disable support
- Audit logging
- Tamper detection

## Screenshots

### Admin Portal

Displays:

- User accounts
- Roles
- Login history
- Account status
- Enable/Disable controls

### Audit Dashboard

Displays:

- Audit events
- Verification status
- Chain integrity checks

## Running the Project

1. Clone the repository.
2. Configure User Secrets.
3. Create the PostgreSQL database.
4. Run Entity Framework migrations.
5. Start the API.
6. Open:

```text
https://localhost:7198/dashboard.html
```

or

```text
https://localhost:7198/admin.html
```

## Development Notes

Sensitive information is stored using ASP.NET User Secrets and is not committed to source control.

Examples include:

- JWT signing keys
- Audit API keys
- Demo passwords

## Version

Current Version: v1.0

## Author

Built as a portfolio project demonstrating:

- Backend development
- Authentication
- Authorization
- Security engineering
- Database design
- Audit systems
- Administrative interfaces