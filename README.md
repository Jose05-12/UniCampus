# UniCampus - Registro de Actividades Académicas

Aplicación web para gestionar actividades académicas (registro, consulta, búsqueda, inscripciones, reseñas y reportes)
construida con arquitectura de tres capas y el patrón MVP en la capa de presentación.

Corporación Universitaria Americana · Desarrollo de Aplicaciones Web II · Proyecto integrador (tercer corte).

**Repositorio:** https://github.com/Jose05-12/UniCampus

## Integrantes

| Nombre | Usuario GitHub | Responsabilidad |
|---|---|---|
| Adrian Carmona | Jose05-12 | _(p. ej. capa de datos y script SQL)_ |
| Luis Fuentes | LuisDa21 | Capa de negocio y validaciones |
| Rafael Acevedo | rafaelacevedo10 | Páginas Web Forms y Presenters |
| Samuel Henriquez | _(completar)_ | _(p. ej. pruebas y documentación)_ |

## Tecnologías

| Herramienta | Uso |
|---|---|
| C# + ASP.NET Web Forms (.NET Framework 4.8) | Páginas `.aspx`, code-behind y lógica |
| ADO.NET (`System.Data.SqlClient`) | Acceso a datos con consultas parametrizadas y transacciones |
| SQL Server / SQL Server LocalDB | Persistencia |
| Visual Studio 2022 o superior | Edición, compilación y depuración |
| Git + GitHub | Control de versiones y colaboración |

## Requisitos para ejecutar

1. **Visual Studio 2022 o superior** con la carga de trabajo *Desarrollo de ASP.NET y web*
   (incluye IIS Express y el paquete de desarrollo de **.NET Framework 4.8**).
2. **SQL Server LocalDB** (se instala con Visual Studio) o **SQL Server Express / Developer**.
3. **SQL Server Management Studio (SSMS)** o la ventana *Explorador de objetos de SQL Server* de Visual Studio
   para ejecutar el script.

No se usan paquetes NuGet externos: la solución compila sin restaurar dependencias.

## Estructura de la solución

```
UniCampus.sln
├── UniCampus.Web        Presentación: páginas .aspx, code-behind, contratos de vista (Views) y Presenters (MVP)
├── UniCampus.BLL        Negocio: servicios, validaciones y reglas (cupos, fechas, estados, permisos)
├── UniCampus.DAL        Datos: repositorios ADO.NET con SQL parametrizado y transacciones
├── UniCampus.Entities   Entidades/DTO compartidas entre capas
├── Database/
│   ├── 03_BaseDatos.sql     Script de creación de la base, tablas, restricciones y datos de prueba
│   └── 04_ConsultaDatos.sql Consulta de verificación de actividades e inscritos
└── Docs/
    ├── 01_Informe_Tecnico.pdf     Informe técnico de la entrega
    └── capturas/                  Evidencias de funcionamiento y de SQL Server
```

Referencias entre proyectos (flujo de dependencias):
`UniCampus.Web → UniCampus.BLL → UniCampus.DAL → SQL Server`, y los tres usan `UniCampus.Entities`.
La capa Web **no** referencia a la capa de datos ni a `System.Data.SqlClient`.

## Configuración de la base de datos

1. Abra SSMS y conéctese a `(localdb)\MSSQLLocalDB` (autenticación de Windows).
   Si usa SQL Server Express conéctese a `.\SQLEXPRESS`.
2. Abra `Database/03_BaseDatos.sql` y ejecútelo completo (F5).
   El script crea la base `UniCampusDB` si no existe, **elimina y vuelve a crear** las tablas y carga los datos de prueba,
   por lo que se puede ejecutar varias veces y siempre deja el mismo estado.
3. (Opcional) Ejecute `Database/04_ConsultaDatos.sql` para verificar las actividades y el número de inscritos.

### Cadena de conexión

Está en `UniCampus.Web/Web.config` con el nombre `UniCampusDb` y usa autenticación integrada de Windows (sin
usuario ni contraseña):

```xml
<add name="UniCampusDb"
     connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=UniCampusDB;Integrated Security=True;TrustServerCertificate=True"
     providerName="System.Data.SqlClient" />
```

- Con SQL Server Express cambie `Data Source` por `.\SQLEXPRESS`.
- **No** suba al repositorio cadenas con usuario y contraseña. Si su servidor las necesita, configúrelas solo en su
  equipo y no haga commit de ese cambio.

## Ejecución

1. Abra `UniCampus.sln` en Visual Studio.
2. Verifique que **UniCampus.Web** sea el proyecto de inicio (clic derecho → *Establecer como proyecto de inicio*).
3. Compile con **Compilar → Compilar solución** (Ctrl+Mayús+B).
4. Ejecute con **F5** o **Ctrl+F5**. IIS Express abre `http://localhost:51480/`.
5. Inicie sesión con uno de los usuarios de prueba.

### Usuarios de prueba

| Rol | Correo | Contraseña |
|---|---|---|
| Administrador | admin@unicampus.edu | Admin123* |
| Estudiante | maria.lopez@unicampus.edu | Estudiante123* |
| Estudiante | ana.torres@unicampus.edu | Estudiante123* |

Son datos ficticios solo para pruebas. Todos los estudiantes del script usan `Estudiante123*`.
Las contraseñas se guardan como hash **PBKDF2-SHA256 con salt**, nunca en texto plano.

> Los datos de prueba tienen fechas entre agosto y diciembre de 2026. Si revisa el proyecto mucho después, algunas
> actividades "Activa"/"Próxima" ya habrán terminado y no admitirán inscripciones (regla de negocio). Edite las fechas
> desde la aplicación o en el script.

### Problemas frecuentes

| Síntoma | Causa y solución |
|---|---|
| "No fue posible iniciar sesión" y en `App_Data/Logs` aparece `Cannot open database "UniCampusDB"` | La base no existe en ese servidor. Ejecute `Database/03_BaseDatos.sql` en la misma instancia indicada en `Web.config`. |
| `A network-related or instance-specific error` | La instancia no existe. Revise `Data Source` (LocalDB vs. SQLEXPRESS). |
| Error de compilación por versión de .NET | Instale el paquete de desarrollo de .NET Framework 4.8 desde el instalador de Visual Studio. |

Los errores técnicos se registran en `UniCampus.Web/App_Data/Logs/errores-AAAAMMDD.log`; al usuario solo se le muestra
un mensaje entendible.

## Funciones implementadas

- [x] Inicio de sesión con roles (Administrador / Estudiante); el usuario se toma de la sesión, nunca de un control.
- [x] Registrar, consultar, actualizar y eliminar/desactivar actividades (CRUD).
- [x] Listado paginado con búsqueda por texto y filtros por tipo, estado y fecha.
- [x] Detalle de la actividad y vista "como la ve el estudiante".
- [x] Inscripciones con control de cupos y duplicados en **transacción** (sin sobrecupo con solicitudes simultáneas).
- [x] Inscripción de estudiantes por el administrador y auto-inscripción del estudiante; cancelación de inscripciones.
- [x] Reseñas y valoraciones (1 a 5) solo para inscritos, una por actividad.
- [x] Reportes de ocupación, inscritos por tipo y calificaciones (solo administrador).
- [x] Mensajes claros de éxito, validación y error; registro técnico de errores en `App_Data/Logs`.

## Arquitectura y flujo MVP (ejemplo: Guardar actividad)

1. El administrador completa el formulario en `Actividades.aspx` y pulsa **Guardar**.
2. El code-behind (`Actividades.aspx.cs`, que implementa `IActividadesView`) llama a `ActividadesPresenter.Guardar()`.
3. El Presenter lee los valores de la vista, los convierte en una entidad `Actividad` y llama a `ActividadService.Guardar`.
4. `ActividadService` verifica permisos, valida campos y reglas de negocio y, si hay errores, devuelve un
   `ResultadoOperacion` con la lista de mensajes.
5. Si todo es válido, el servicio llama a `ActividadRepository.Insertar`, que ejecuta un `INSERT` parametrizado con
   ADO.NET y devuelve el nuevo `ActividadId`.
6. El Presenter indica a la vista el mensaje de éxito o de error y recarga el listado.

## Reglas de negocio principales

| Regla | Dónde se aplica |
|---|---|
| Campos obligatorios y longitudes | `ActividadService.Validar` |
| Fecha fin ≥ fecha inicio; hora fin > hora inicio | `ActividadService.Validar` + `CHECK` en BD |
| Cupos entre 1 y 500 y nunca menores que los inscritos | `ActividadService.Validar` + `CHECK` en BD |
| Una actividad nueva no puede iniciar en el pasado | `ActividadService.Validar` |
| "Finalizada" solo si la fecha de fin ya pasó | `ActividadService.Validar` |
| No duplicar título + fecha de inicio | `ActividadService.Validar` + `UNIQUE` en BD |
| Eliminar: si tiene inscripciones o reseñas se desactiva (Cancelada) | `ActividadService.Eliminar` |
| Inscripción solo en actividades Activa/Próxima no terminadas | `InscripcionService.Inscribir` |
| Sin sobrecupo ni inscripción duplicada | `InscripcionRepository.Inscribir` (transacción + `UPDLOCK`) + `UNIQUE` en BD |
| Reseña: solo inscritos, actividad iniciada, una por persona, calificación 1-5 | `ResenaService` + `CHECK`/`UNIQUE` en BD |
| Solo el administrador crea/edita/elimina actividades y ve reportes | Servicios BLL + Presenters |

## Pruebas funcionales

| # | Caso | Acción | Resultado esperado |
|---|---|---|---|
| 1 | Válida | Guardar una actividad con todos los datos correctos | Se guarda, aparece en el listado y en SQL Server |
| 2 | Obligatorio | Guardar sin título | Mensaje "El título es obligatorio."; no inserta |
| 3 | Cupo inválido | Cupos = 0 o negativo | Se rechaza la operación |
| 4 | Fechas | Fecha de fin anterior a la de inicio | Mensaje de validación; no inserta |
| 5 | Consulta | Buscar la actividad guardada por texto | Se recuperan los datos correctos |
| 6 | Cupo agotado | Inscribirse en "Taller de Git y GitHub" (3 de 3) | Mensaje de cupos agotados; no inscribe |
| 7 | Duplicado | Inscribir dos veces al mismo estudiante | Mensaje de inscripción existente; no duplica |
| 8 | Reseña | Calificar una actividad en la que no está inscrito | Se rechaza la reseña |
| 9 | Permisos | Un estudiante intenta abrir Reportes | Acceso denegado |
| 10 | Eliminar con dependencias | Eliminar una actividad con inscritos | Se desactiva (Cancelada) en lugar de borrarse |

Las evidencias de cada caso están en el informe técnico (`Docs/01_Informe_Tecnico.pdf`) y en `Docs/capturas`.

## Trabajo con GitHub

- Cada integrante clona el repositorio, trabaja en una rama por funcionalidad (`feature/...`) y abre un pull request.
- Mensajes de commit sugeridos: `feat: ...`, `fix: ...`, `docs: ...`.
- El `.gitignore` excluye `bin`, `obj`, `.vs`, archivos de usuario y los logs de `App_Data/Logs`.
