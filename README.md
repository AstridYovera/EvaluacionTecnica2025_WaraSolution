# 🏢 WARA – Sistema de Gestión de Reservas de Salas

Este proyecto corresponde a la **Evaluación Técnica 2025 – IV (Trainee)**.  
Consiste en un sistema de gestión de reservas de salas de reuniones, con **backend en C# .NET Core**, **base de datos SQL Server** y consumo de **APIs REST**, además de una interfaz web sencilla.

🚀 Tecnologías utilizadas
- **C# .NET Core 7 / ASP.NET Core**
- **Entity Framework Core** (migraciones, repositorios)
- **SQL Server** (persistencia de datos)
- **JWT (JSON Web Tokens)** para autenticación y autorización
- **Swagger** para documentación de la API
- **NUnit / xUnit** para pruebas unitarias
- **HTML + JavaScript** para la interfaz
- **SMTP (Gmail)** para notificaciones por correo

🔐 Funcionalidades principales

🔑 Login con Roles
- Uso de JWT para sesiones seguras.
- Redirección según rol:  
  - **Admin** → Dashboard de gestión de salas.  
  - **Usuario** → Listado de salas disponibles y reservas.
- Restricciones: no se puede acceder a rutas de otro rol ni sin login.

📋 Listado de Salas (Usuario)
- Muestra: **ID, Nombre, Capacidad, Estado**.
- Filtros disponibles:  
  - Capacidad mínima  
  - Estado (Disponible / Ocupada)  
  - Fecha/hora de consulta (para disponibilidad futura)
- Mensaje claro si no hay resultados.

📝 Formulario de Reserva
- Campos: Sala, Fecha, Hora Inicio, Hora Fin, Motivo.
- Validaciones:  
  - `Hora Inicio < Hora Fin`  
  - No se permite reservar en el pasado  
  - No se permite reservar una sala ocupada  
  - Todos los campos son obligatorios
- Al confirmar la reserva:  
  - Muestra mensaje de éxito ✔️  
  - Limpia el formulario  
  - Envía correo de confirmación al usuario  

🛠️ Panel Admin
- Métricas principales:  
  - Total de salas  
  - Ocupadas  
  - Disponibles  
  - Reservas próximas 24h  
- CRUD completo de salas (crear, actualizar, eliminar).  
- Validaciones de duplicados y reglas de integridad.  

✉️ Notificación por Email
- Confirmación automática de reserva con:  
  - Sala  
  - Fecha y horario  
  - Motivo  
- Envío con formato HTML ejecutivo (branding WARA).  

🌐 Endpoints principales
- Swagger: https://localhost:7162/swagger
- Login: https://localhost:7162/login.html
- Usuario: https://localhost:7162/user
- Admin: https://localhost:7162/admin

👤 Autor
Astrid Yovera Tinoco – Evaluación Técnica WARA 2025 IV (Trainee)
