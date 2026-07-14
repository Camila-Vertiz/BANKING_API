# Banking API - Banco Digital Nova

API REST bancaria desarrollada con **.NET 8** para administrar clientes, cuentas bancarias y transferencias.

El sistema permite registrar usuarios, autenticar mediante JWT, gestionar clientes, crear cuentas bancarias, consultar saldos, realizar transferencias y consultar el historial de movimientos.

---

# Arquitectura del proyecto

El proyecto utiliza una arquitectura basada en **Clean Architecture**, separando responsabilidades en diferentes capas:

```text
Banking
│
├── Banking.Api
│   ├── Controllers
│   ├── Middleware
│   ├── Configuración JWT
│   └── Swagger/OpenAPI
│
├── Banking.Application
│   ├── Services
│   ├── Requests
│   ├── Responses
│   ├── Validators
│   └── Lógica de aplicación
│
├── Banking.Domain
│   ├── Entities
│   ├── Enums
│   └── Interfaces del dominio
│
├── Banking.Infrastructure
│   ├── Entity Framework Core
│   ├── DbContext
│   └── Implementación de repositorios
│
└── Banking.Tests
    ├── Pruebas unitarias
    ├── xUnit
    ├── Moq
    └── FluentAssertions

```
---

# Tecnologías utilizadas

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT Authentication
- Swagger / OpenAPI
- FluentValidation
- xUnit
- Moq
- FluentAssertions

---

# Requisitos previos

Antes de ejecutar el proyecto se requiere:

- .NET SDK 8 instalado.
- SQL Server instalado.
- Visual Studio 2022/2026 o Visual Studio Code.
- Base de datos configurada.

---

# Configuración de base de datos

Modificar el archivo:
```
Banking.Api/appsettings.json
```


Configurar la cadena de conexión:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=SERVIDOR;Database=BankingDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

---

# Migraciones de base de datos
Desde la carpeta raíz del proyecto ejecutar:
```
dotnet ef database update --project Banking.Infrastructure --startup-project Banking.Api
```

---

# Ejecutar la API
Desde la carpeta raíz:
```
dotnet run --project Banking.Api
```
La API estará disponible en:
```
https://localhost:7046
```

---

# Swagger
La documentación interactiva de la API está disponible en:
```
https://localhost:7046/swagger
```
Swagger permite probar los endpoints y visualizar los modelos disponibles.

---

# Autenticación JWT
La API utiliza autenticación mediante tokens JWT.
Flujo de autenticación:

### 1. Registro de usuario
Endpoint:
```
POST /api/auth/register
```

### 2. Inicio de sesión
Endpoint:
```
POST /api/auth/login
```
El sistema devuelve un token JWT.

### 3. Enviar token
Para endpoints protegidos utilizar:
```
Authorization: Bearer {token}
```

---
# Roles del sistema

### Admin
Permisos:
- Registrar clientes.
- Crear cuentas bancarias.
- Consultar información administrativa.
- Realizar depósitos.

### Customer
Permisos:
- Consultar sus cuentas.
- Consultar saldo.
- Realizar transferencias.
- Consultar movimientos propios.

---
# Usuarios de prueba

### Administrador
```
User: admin
Password: Admin123!
```

---
# Endpoints principales

### Autenticación
```
POST /api/auth/register
POST /api/auth/login
```

### Clientes
```
GET    /api/customers
POST   /api/customers
PUT    /api/customers
GET    /api/customers/{id}
```

### Cuentas bancarias
```
GET  /api/accounts
POST /api/accounts

GET /api/accounts/{id}

GET /api/accounts/{id}/balance

GET /api/accounts/{id}/transactions
```

### Transferencias
```
POST /api/transfers
```
Una transferencia genera:

- Débito en la cuenta origen.
- Crédito en la cuenta destino.
- Identificador TraceId para auditoría.

### Auditoría

Las transferencias generan un identificador único TraceId que permite relacionar:

- Movimiento débito de la cuenta origen.
- Movimiento crédito de la cuenta destino.

Esto permite realizar seguimiento y trazabilidad de operaciones.

### Depósitos
```
POST /api/transfers/deposit
```
Disponible únicamente para usuarios con rol Admin.

---
# Validaciones implementadas
La API controla:

- Usuarios duplicados.
- Documentos duplicados.
- Correos duplicados.
- Cuentas inexistentes.
- Montos inválidos.
- Saldo insuficiente.
- Acceso no autorizado a cuentas de otros clientes.
- Manejo centralizado de excepciones.

---
# Manejo de errores

La API utiliza middleware global para manejar excepciones y devolver códigos HTTP adecuados:

| Código | Descripción                    |
| ------ | ------------------------------ |
| 400    | Datos inválidos                |
| 401    | Usuario no autenticado         |
| 403    | Acceso prohibido               |
| 404    | Recurso no encontrado          |
| 409    | Conflicto por datos duplicados |
| 500    | Error interno                  |


---
# Pruebas unitarias

Ejecutar:

```
dotnet test
```

### Pruebas implementadas:

- Login exitoso.
- Login fallido.
- Creación de cuentas.
- Consulta de saldo.
- Transferencia exitosa.
- Saldo insuficiente.
- Monto inválido.
- Cuenta origen inexistente.
- Cuenta destino inexistente.
- Validación de permisos.

### Herramientas utilizadas:

- xUnit.
- Moq.
- FluentAssertions.

---
# Estructura de seguridad

La aplicación implementa:

- Autenticación mediante JWT.
- Control de acceso por roles.
- Validación de propietario de cuentas.
- Protección de endpoints sensibles.

