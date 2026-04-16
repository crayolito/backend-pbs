# BancoSol API - Prueba Técnica

API REST para gestión de catálogo de productos construida con .NET 10 y Clean Architecture.

---

## Requisitos previos

Antes de empezar verificá que tenés instalado:

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Git

Verificar que .NET está instalado correctamente:
```bash
dotnet --version
```
Debe mostrar `10.x.x`. Si no aparece, instalarlo desde el link de arriba.

---

## Paso 1 - Clonar el repositorio

Abrí una terminal, Git Bash o PowerShell y ejecutá:

```bash
git clone https://github.com/tu-usuario/bancosol-api.git
cd bancosol-api
```

---

## Paso 2 - Correr el proyecto

Elegí la opción según tu editor:

---

### Opción A - Visual Studio 2022

1. Abrí Visual Studio 2022
2. En la pantalla de inicio seleccioná **Open a project or solution**
3. Navegá hasta la carpeta clonada y abrí el archivo `BancoSol.sln`
4. Esperá que Visual Studio restaure los paquetes NuGet automáticamente - hay una barra de progreso en la parte inferior
5. En el **Explorador de soluciones** (panel derecho), click derecho sobre `BancoSol.API`
6. Seleccioná **Set as Startup Project**
7. Presioná **F5** o el botón verde ▶ en la barra superior
8. El navegador abre automáticamente en Swagger:
```
http://localhost:5292/swagger
```

> Si Visual Studio pregunta sobre instalar un certificado HTTPS, aceptá la instalación.

---

### Opción B - VS Code

1. Abrí VS Code
2. **File** → **Open Folder** → seleccioná la carpeta `bancosol-api`
3. Si aparece un popup pidiendo instalar la extensión de C# (`ms-dotnettools.csharp`), instalala
4. Abrí la terminal integrada con **Ctrl + `** (tecla backtick, la que está bajo el Esc)
5. Restaurá los paquetes:
```bash
dotnet restore
```
6. Corré la API:
```bash
dotnet run --project BancoSol.API
```
7. Abrí el navegador manualmente y entrá a:
```
http://localhost:5292/swagger
```

---

## Paso 3 - Autenticación API Key

Todos los endpoints requieren este header en cada request:

```
X-Api-Key: bancosol-2026-api-key
```

Sin este header la API responde `401 Unauthorized`.

**En Swagger:** hacé click en el botón **Authorize** (ícono del candado) arriba a la derecha, pegá el valor `bancosol-2026-api-key` y hacé click en **Authorize**.

---

## Paso 4 - Probar con Postman

### Importar la colección

1. Abrí Postman
2. Click en **Import** (arriba a la izquierda)
3. Seleccioná la pestaña **Raw text**
4. Pegá el JSON de la colección que está más abajo
5. Click en **Import**
6. Repetí el proceso para el JSON del entorno
7. En el selector de entornos (arriba a la derecha en Postman) seleccioná **BancoSol Local**

---

### JSON - Colección Postman

```json
{
  "info": {
    "name": "BancoSol API",
    "_postman_id": "7d4f9c95-c9db-47d1-bdea-c4f6d2a8f001",
    "description": "Colección completa para probar todos los endpoints de BancoSol API.",
    "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
  },
  "variable": [
    { "key": "baseUrl", "value": "http://localhost:5292" },
    { "key": "apiKey", "value": "bancosol-2026-api-key" },
    { "key": "productId", "value": "22222222-2222-2222-2222-222222222101" }
  ],
  "item": [
    {
      "name": "1 - GET todos los productos",
      "request": {
        "method": "GET",
        "header": [{ "key": "X-Api-Key", "value": "{{apiKey}}" }],
        "url": { "raw": "{{baseUrl}}/api/products", "host": ["{{baseUrl}}"], "path": ["api","products"] }
      },
      "event": [{ "listen": "test", "script": { "exec": [
        "pm.test('Status 200', () => pm.response.to.have.status(200));",
        "pm.test('Respuesta paginada', () => { const j = pm.response.json(); pm.expect(j).to.have.property('data'); pm.expect(j).to.have.property('total'); });"
      ]}}]
    },
    {
      "name": "2 - GET buscar por nombre o SKU",
      "request": {
        "method": "GET",
        "header": [{ "key": "X-Api-Key", "value": "{{apiKey}}" }],
        "url": { "raw": "{{baseUrl}}/api/products?search=auricular", "host": ["{{baseUrl}}"], "path": ["api","products"], "query": [{ "key": "search", "value": "auricular" }] }
      },
      "event": [{ "listen": "test", "script": { "exec": [
        "pm.test('Status 200', () => pm.response.to.have.status(200));",
        "pm.test('Hay resultados', () => pm.expect(pm.response.json().data.length).to.be.above(0));"
      ]}}]
    },
    {
      "name": "3 - GET filtrar por moneda USD",
      "request": {
        "method": "GET",
        "header": [{ "key": "X-Api-Key", "value": "{{apiKey}}" }],
        "url": { "raw": "{{baseUrl}}/api/products?currency=USD", "host": ["{{baseUrl}}"], "path": ["api","products"], "query": [{ "key": "currency", "value": "USD" }] }
      }
    },
    {
      "name": "4 - GET solo con stock disponible",
      "request": {
        "method": "GET",
        "header": [{ "key": "X-Api-Key", "value": "{{apiKey}}" }],
        "url": { "raw": "{{baseUrl}}/api/products?inStock=true", "host": ["{{baseUrl}}"], "path": ["api","products"], "query": [{ "key": "inStock", "value": "true" }] }
      }
    },
    {
      "name": "5 - GET filtrar por rango de precio",
      "request": {
        "method": "GET",
        "header": [{ "key": "X-Api-Key", "value": "{{apiKey}}" }],
        "url": { "raw": "{{baseUrl}}/api/products?minPrice=50&maxPrice=300", "host": ["{{baseUrl}}"], "path": ["api","products"], "query": [{ "key": "minPrice", "value": "50" },{ "key": "maxPrice", "value": "300" }] }
      }
    },
    {
      "name": "6 - GET ordenar por precio descendente",
      "request": {
        "method": "GET",
        "header": [{ "key": "X-Api-Key", "value": "{{apiKey}}" }],
        "url": { "raw": "{{baseUrl}}/api/products?sortBy=price&order=desc", "host": ["{{baseUrl}}"], "path": ["api","products"], "query": [{ "key": "sortBy", "value": "price" },{ "key": "order", "value": "desc" }] }
      }
    },
    {
      "name": "7 - GET paginación página 2",
      "request": {
        "method": "GET",
        "header": [{ "key": "X-Api-Key", "value": "{{apiKey}}" }],
        "url": { "raw": "{{baseUrl}}/api/products?page=2&size=5", "host": ["{{baseUrl}}"], "path": ["api","products"], "query": [{ "key": "page", "value": "2" },{ "key": "size", "value": "5" }] }
      }
    },
    {
      "name": "8 - GET combinación completa de filtros",
      "request": {
        "method": "GET",
        "header": [{ "key": "X-Api-Key", "value": "{{apiKey}}" }],
        "url": { "raw": "{{baseUrl}}/api/products?currency=USD&inStock=true&minPrice=10&maxPrice=500&sortBy=price&order=desc&page=1&size=10", "host": ["{{baseUrl}}"], "path": ["api","products"], "query": [{ "key": "currency", "value": "USD" },{ "key": "inStock", "value": "true" },{ "key": "minPrice", "value": "10" },{ "key": "maxPrice", "value": "500" },{ "key": "sortBy", "value": "price" },{ "key": "order", "value": "desc" },{ "key": "page", "value": "1" },{ "key": "size", "value": "10" }] }
      }
    },
    {
      "name": "9 - PATCH actualizar precio (exitoso)",
      "request": {
        "method": "PATCH",
        "header": [{ "key": "X-Api-Key", "value": "{{apiKey}}" },{ "key": "Content-Type", "value": "application/json" }],
        "body": { "mode": "raw", "raw": "{\n  \"price\": 99.90,\n  \"currency\": \"BOB\"\n}" },
        "url": { "raw": "{{baseUrl}}/api/products/{{productId}}/price", "host": ["{{baseUrl}}"], "path": ["api","products","{{productId}}","price"] }
      },
      "event": [{ "listen": "test", "script": { "exec": [
        "pm.test('Status 200', () => pm.response.to.have.status(200));",
        "pm.test('Precio actualizado correctamente', () => pm.expect(pm.response.json().price).to.eql(99.90));"
      ]}}]
    },
    {
      "name": "10 - PATCH precio inválido - espera 400",
      "request": {
        "method": "PATCH",
        "header": [{ "key": "X-Api-Key", "value": "{{apiKey}}" },{ "key": "Content-Type", "value": "application/json" }],
        "body": { "mode": "raw", "raw": "{\n  \"price\": -10,\n  \"currency\": \"BOB\"\n}" },
        "url": { "raw": "{{baseUrl}}/api/products/{{productId}}/price", "host": ["{{baseUrl}}"], "path": ["api","products","{{productId}}","price"] }
      },
      "event": [{ "listen": "test", "script": { "exec": [
        "pm.test('Status 400 por precio inválido', () => pm.response.to.have.status(400));"
      ]}}]
    },
    {
      "name": "11 - PATCH producto inexistente - espera 404",
      "request": {
        "method": "PATCH",
        "header": [{ "key": "X-Api-Key", "value": "{{apiKey}}" },{ "key": "Content-Type", "value": "application/json" }],
        "body": { "mode": "raw", "raw": "{\n  \"price\": 50.00,\n  \"currency\": \"BOB\"\n}" },
        "url": { "raw": "{{baseUrl}}/api/products/11111111-1111-1111-1111-111111111111/price", "host": ["{{baseUrl}}"], "path": ["api","products","11111111-1111-1111-1111-111111111111","price"] }
      },
      "event": [{ "listen": "test", "script": { "exec": [
        "pm.test('Status 404 producto no existe', () => pm.response.to.have.status(404));"
      ]}}]
    },
    {
      "name": "12 - GET sin API Key - espera 401",
      "request": {
        "method": "GET",
        "header": [],
        "url": { "raw": "{{baseUrl}}/api/products", "host": ["{{baseUrl}}"], "path": ["api","products"] }
      },
      "event": [{ "listen": "test", "script": { "exec": [
        "pm.test('Status 401 sin API Key', () => pm.response.to.have.status(401));"
      ]}}]
    }
  ]
}
```

---

### JSON - Entorno Postman

```json
{
  "name": "BancoSol Local",
  "values": [
    { "key": "baseUrl", "value": "http://localhost:5292", "enabled": true },
    { "key": "apiKey", "value": "bancosol-2026-api-key", "enabled": true },
    { "key": "productId", "value": "22222222-2222-2222-2222-222222222101", "enabled": true }
  ],
  "_postman_variable_scope": "environment"
}
```

---

## Paso 5 - Correr los tests

### Visual Studio 2022

1. Menú superior **Test** → **Run All Tests**
2. O abrí el panel con `Ctrl + E, T` y hacé click en ▶ **Run All**
3. Los resultados aparecen en el panel **Test Explorer**

### VS Code o terminal

```bash
dotnet test
```

Ver resultados detallados:

```bash
dotnet test --logger "console;verbosity=detailed"
```

Correr solo los tests de Application:

```bash
dotnet test BancoSol.Application.Tests/BancoSol.Application.Tests.csproj
```

---

## Paso 6 - Ver los logs

Mientras la app está corriendo los logs aparecen en la consola. También se guardan en archivos diarios:

```
logs/
  app-20260416.log
  app-20260417.log
```

Cada día se crea un archivo nuevo automáticamente. Los archivos anteriores se conservan.

---

## Decisiones técnicas

**Arquitectura por capas** - (API, Application, Domain, Infrastructure) para separar responsabilidades, facilitar pruebas y permitir evolución por módulos sin acoplamiento fuerte.

**IMemoryCache** - cache en memoria solo para la consulta base (sin filtros, `page=1`, `size=10`, orden por defecto), TTL de 5 minutos e invalidación automática en cada `PATCH` exitoso. Redis sería innecesario para este contexto.

**Serilog** - logging estructurado con salida a consola y archivo rolling diario (`logs/app-.log`), con nivel mínimo `Information`. Es una estrategia simple y efectiva para trazabilidad, diagnóstico y auditoría básica sin sobreingeniería.

**Seed data automático** - 200+ productos cargados en `OnModelCreating`. La base de datos tiene datos desde el primer arranque sin pasos manuales.

---

## Posibles mejoras

**JWT** - reemplazar la API Key estática por tokens con expiración y roles.

**Secretos en variables de entorno** - mover el API Key fuera de `appsettings.json` hacia variables de entorno o Azure Key Vault.

**Migraciones versionadas** - reemplazar `EnsureCreated` por `Database.Migrate()` para entornos productivos.

**Rate limiting** - limitar requests por IP para proteger la API de abuso.

**Health checks** - endpoint `/health` para monitorear estado de la API y base de datos.

**Tests de integración** - usar `WebApplicationFactory` para probar endpoints completos incluyendo middleware.

**Observabilidad/Telemetry** - integrar **Application Insights** (Azure Monitor) como herramienta única para centralizar métricas, trazas y errores. Serilog cubre bien el logging local, y Application Insights agrega visibilidad operativar.