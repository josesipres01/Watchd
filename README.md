# Watchd

Watchd es una aplicación web MVC para explorar un catálogo de películas y desarrollar funciones de registro, inicio de sesión y reseñas. Está construida con ASP.NET Core y SQL Server.

> **Estado del proyecto:** algunas partes todavía están en desarrollo. El formulario de reseñas valida sus datos, pero aún no los guarda. La autenticación tampoco mantiene una sesión autenticada ni protege rutas. Consulta [Limitaciones conocidas](#limitaciones-conocidas) antes de tratar estas funciones como listas para producción.

## Funcionalidades

- Catálogo de películas con búsqueda por título o director y opciones de ordenamiento.
- Página de detalle y formulario para agregar películas.
- Formularios de registro e inicio de sesión de usuarios.
- Formulario para registrar una visualización con una calificación entre 1 y 5.
- Validación de datos con Data Annotations y vistas Razor.

## Tecnologías

- .NET 10 y ASP.NET Core MVC
- SQL Server
- Dapper y Microsoft.Data.SqlClient
- Bootstrap
- MSTest para las pruebas automatizadas

## Requisitos

- .NET SDK 10
- Una instancia de SQL Server accesible desde la aplicación
- Una base de datos `Watchd` con las tablas y columnas que consultan los controladores

El repositorio no incluye scripts de creación de la base de datos. Asegúrate de preparar el esquema antes de ejecutar los flujos que acceden a SQL Server.

## Configuración local

Configura la cadena de conexión con .NET User Secrets desde la raíz del repositorio:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=Watchd;Integrated Security=true;TrustServerCertificate=True;" --project Watchd/Watchd.csproj
```

Adapta el servidor y el método de autenticación a tu instalación local. User Secrets guarda este valor fuera del repositorio. No agregues contraseñas ni cadenas de conexión personales a `appsettings.json` o `appsettings.Development.json`.

Para otros entornos, configura `ConnectionStrings:DefaultConnection` mediante el gestor de secretos del proveedor o la variable de entorno `ConnectionStrings__DefaultConnection`.

## Ejecutar la aplicación

```powershell
dotnet restore Watchd.slnx
dotnet run --project Watchd/Watchd.csproj
```

Usa la URL HTTPS que muestre la salida de `dotnet run`.

## Compilar y probar

Compilar la solución en modo Release:

```powershell
dotnet build Watchd.slnx --configuration Release
```

Ejecutar las pruebas:

```powershell
dotnet test Watchd.slnx --configuration Release
```

El proyecto `Watchd.Tests` contiene pruebas de validación de modelos. No son pruebas de integración y no verifican la conexión a SQL Server.

## Flujo de contribución

1. Actualiza `master` y crea una rama breve por cada cambio, por ejemplo `feat/catalogo`, `fix/validacion` o `docs/readme`.
2. Mantén los cambios enfocados y ejecuta compilación y pruebas antes de abrir un pull request.
3. Usa mensajes de commit claros, preferiblemente con Conventional Commits: `feat: agregar búsqueda de películas`, `fix: validar calificación` o `docs: actualizar instrucciones`.
4. Abre un pull request hacia `master` y describe el cambio y las comprobaciones realizadas.
5. Integra el pull request cuando la CI requerida pase y se cumplan las reglas de protección configuradas en GitHub.

No incluyas secretos, archivos locales de Visual Studio ni salidas de compilación en los commits.

## Integración continua

GitHub Actions ejecuta el workflow de CI en cada pull request hacia `master` y en cada push a esa rama. El workflow restaura dependencias, compila en Release y ejecuta las pruebas.

Para hacer cumplir CI, configura una regla de protección para `master` que requiera pull requests y el check **Build**. El check aparecerá para seleccionarlo después de que el workflow se haya ejecutado al menos una vez.

## Despliegue

El despliegue automático no está configurado porque aún no se ha elegido un proveedor de hosting. Antes de publicar, configura la cadena de conexión y cualquier otro secreto en el entorno de producción; nunca los guardes en el repositorio.

## Limitaciones conocidas

- `ResenasController.Crear` solo valida el formulario y devuelve la vista; no guarda la reseña en la base de datos.
- El inicio de sesión guarda el nombre de usuario en `TempData`, que solo dura una redirección. No crea una sesión de autenticación ni habilita autorización de rutas.
- El controlador de películas expone la acción `Catalogo`, mientras algunas vistas enlazan a una acción `Index`. Revisa la correspondencia de rutas antes de considerar todo el flujo del catálogo terminado.
- No hay scripts versionados para crear o migrar el esquema de SQL Server.
- Las pruebas actuales cubren validaciones de modelos, no acceso a datos ni flujos completos de la aplicación.
