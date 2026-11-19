IF DB_ID(N'UniCampusDB') IS NULL
BEGIN
    CREATE DATABASE UniCampusDB;
END
GO

USE UniCampusDB;
GO

/* ---------- Limpieza (orden inverso a las dependencias) ---------- */
IF OBJECT_ID(N'dbo.Resena', N'U') IS NOT NULL DROP TABLE dbo.Resena;
IF OBJECT_ID(N'dbo.Inscripcion', N'U') IS NOT NULL DROP TABLE dbo.Inscripcion;
IF OBJECT_ID(N'dbo.ActividadAcademica', N'U') IS NOT NULL DROP TABLE dbo.ActividadAcademica;
IF OBJECT_ID(N'dbo.TipoActividad', N'U') IS NOT NULL DROP TABLE dbo.TipoActividad;
IF OBJECT_ID(N'dbo.Usuario', N'U') IS NOT NULL DROP TABLE dbo.Usuario;
GO

CREATE TABLE dbo.TipoActividad (
    TipoId       INT IDENTITY(1,1) CONSTRAINT PK_TipoActividad PRIMARY KEY,
    Nombre       NVARCHAR(50)  NOT NULL CONSTRAINT UQ_TipoActividad_Nombre UNIQUE,
    Descripcion  NVARCHAR(200) NULL
);
GO

CREATE TABLE dbo.Usuario (
    UsuarioId     INT IDENTITY(1,1) CONSTRAINT PK_Usuario PRIMARY KEY,
    Nombre        NVARCHAR(100)  NOT NULL,
    Correo        NVARCHAR(150)  NOT NULL CONSTRAINT UQ_Usuario_Correo UNIQUE,
    Programa      NVARCHAR(100)  NULL,
    Rol           NVARCHAR(30)   NOT NULL,
    PasswordHash  VARBINARY(32)  NOT NULL,
    PasswordSalt  VARBINARY(16)  NOT NULL,
    Activo        BIT            NOT NULL CONSTRAINT DF_Usuario_Activo DEFAULT (1),
    CONSTRAINT CK_Usuario_Rol CHECK (Rol IN (N'Administrador', N'Estudiante'))
);
GO

CREATE TABLE dbo.ActividadAcademica (
    ActividadId    INT IDENTITY(1,1) CONSTRAINT PK_ActividadAcademica PRIMARY KEY,
    Titulo         NVARCHAR(120)  NOT NULL,
    Descripcion    NVARCHAR(1000) NOT NULL,
    TipoId         INT            NOT NULL,
    FechaInicio    DATE           NOT NULL,
    FechaFin       DATE           NOT NULL,
    HoraInicio     TIME(0)        NOT NULL,
    HoraFin        TIME(0)        NOT NULL,
    Modalidad      NVARCHAR(20)   NOT NULL,
    Lugar          NVARCHAR(150)  NOT NULL,
    Cupos          INT            NOT NULL,
    Estado         NVARCHAR(20)   NOT NULL CONSTRAINT DF_Actividad_Estado DEFAULT (N'Próxima'),
    FechaCreacion  DATETIME2(0)   NOT NULL CONSTRAINT DF_Actividad_FechaCreacion DEFAULT (SYSDATETIME()),
    CONSTRAINT FK_Actividad_Tipo FOREIGN KEY (TipoId) REFERENCES dbo.TipoActividad(TipoId),
    CONSTRAINT CK_Actividad_Cupos CHECK (Cupos BETWEEN 1 AND 500),
    CONSTRAINT CK_Actividad_Fechas CHECK (FechaFin >= FechaInicio),
    CONSTRAINT CK_Actividad_Horas CHECK (HoraFin > HoraInicio),
    CONSTRAINT CK_Actividad_Modalidad CHECK (Modalidad IN (N'Presencial', N'Virtual', N'Híbrida')),
    CONSTRAINT CK_Actividad_Estado CHECK (Estado IN (N'Activa', N'Próxima', N'Finalizada', N'Cancelada')),
    CONSTRAINT UQ_Actividad_Titulo_Fecha UNIQUE (Titulo, FechaInicio)
);
GO

CREATE TABLE dbo.Inscripcion (
    InscripcionId     INT IDENTITY(1,1) CONSTRAINT PK_Inscripcion PRIMARY KEY,
    ActividadId       INT          NOT NULL,
    UsuarioId         INT          NOT NULL,
    FechaInscripcion  DATETIME2(0) NOT NULL CONSTRAINT DF_Inscripcion_Fecha DEFAULT (SYSDATETIME()),
    Estado            NVARCHAR(20) NOT NULL CONSTRAINT DF_Inscripcion_Estado DEFAULT (N'Activa'),
    CONSTRAINT FK_Inscripcion_Actividad FOREIGN KEY (ActividadId) REFERENCES dbo.ActividadAcademica(ActividadId),
    CONSTRAINT FK_Inscripcion_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuario(UsuarioId),
    CONSTRAINT UQ_Inscripcion_Actividad_Usuario UNIQUE (ActividadId, UsuarioId),
    CONSTRAINT CK_Inscripcion_Estado CHECK (Estado IN (N'Activa', N'Cancelada'))
);
GO

CREATE TABLE dbo.Resena (
    ResenaId       INT IDENTITY(1,1) CONSTRAINT PK_Resena PRIMARY KEY,
    ActividadId    INT            NOT NULL,
    UsuarioId      INT            NOT NULL,
    Calificacion   TINYINT        NOT NULL,
    Comentario     NVARCHAR(500)  NOT NULL,
    FechaCreacion  DATETIME2(0)   NOT NULL CONSTRAINT DF_Resena_Fecha DEFAULT (SYSDATETIME()),
    CONSTRAINT FK_Resena_Actividad FOREIGN KEY (ActividadId) REFERENCES dbo.ActividadAcademica(ActividadId),
    CONSTRAINT FK_Resena_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuario(UsuarioId),
    CONSTRAINT CK_Resena_Calificacion CHECK (Calificacion BETWEEN 1 AND 5),
    CONSTRAINT UQ_Resena_Actividad_Usuario UNIQUE (ActividadId, UsuarioId)
);
GO

CREATE INDEX IX_Inscripcion_Usuario ON dbo.Inscripcion(UsuarioId);
CREATE INDEX IX_Resena_Actividad ON dbo.Resena(ActividadId);
GO

/* ---------- Datos de prueba (ficticios) ---------- */
SET IDENTITY_INSERT dbo.TipoActividad ON;
INSERT INTO dbo.TipoActividad (TipoId, Nombre, Descripcion) VALUES
(1, N'Taller',      N'Sesión práctica guiada.'),
(2, N'Conferencia', N'Charla magistral con invitados.'),
(3, N'Seminario',   N'Espacio académico de discusión.'),
(4, N'Curso',       N'Formación de varias sesiones.'),
(5, N'Encuentro',   N'Reunión de comunidad académica.'),
(6, N'Feria',       N'Exposición de proyectos.');
SET IDENTITY_INSERT dbo.TipoActividad OFF;
GO

SET IDENTITY_INSERT dbo.Usuario ON;
INSERT INTO dbo.Usuario (UsuarioId, Nombre, Correo, Programa, Rol, PasswordSalt, PasswordHash) VALUES
(1, N'Coordinación Académica', N'admin@unicampus.edu',       NULL,                          N'Administrador', 0x0B84C45EAAF15768773FC9430D564C92, 0x357E4440B057C236D38CCB9FDB9E7C90E2E9B5D2E27AF6D649AB4F008F326202),
(2, N'María López',            N'maria.lopez@unicampus.edu', N'Ingeniería de Sistemas',     N'Estudiante',    0x1C1946F34018E82699DEE12F0E7DB223, 0x82965B49FEB837AB819A12F3C81E8CF23E99B6A3B016E12016999C4904B63C83),
(3, N'Ana Torres',             N'ana.torres@unicampus.edu',  N'Ingeniería de Sistemas',     N'Estudiante',    0x78A3E6E4D8BEC1DAE06E1CF24346C51C, 0xD699E9F4D5CAB7E182F803F1DB2F8F30AA5378681A7B76343A89D8FBDE63AD76),
(4, N'Carlos Pérez',           N'carlos.perez@unicampus.edu',N'Diseño Gráfico',             N'Estudiante',    0x9E75ED322074897F6DF283A65351699E, 0xE445B931F2292060F9C8A928483301FB50B6E81BD581151A94A537A6CA72C73B),
(5, N'Laura Gómez',            N'laura.gomez@unicampus.edu', N'Administración',             N'Estudiante',    0x56E2F406216051038ADE42884673006D, 0x0370E5830544AFBFC31A5861874D3BD38DDD3A3737270810D778507C5E6D8E1E),
(6, N'Julián Martínez',        N'julian.martinez@unicampus.edu', N'Ingeniería Industrial',  N'Estudiante',    0x662548913F060E51C5C267FF28394291, 0x4353B5139C23E6908C321537DBD98C402B022544FB0EFF7214EE82D2FFD38CD2),
(7, N'Sofía Herrera',          N'sofia.herrera@unicampus.edu',   N'Contaduría Pública',     N'Estudiante',    0x9895A6A8C6B519883E60E105196FE3BE, 0xAA41B83E79D9FB1F076277148851E433E706B7C6FEA3CF22ABB1569EB4DD5539),
(8, N'Andrés Castillo',        N'andres.castillo@unicampus.edu', N'Ingeniería de Sistemas', N'Estudiante',    0x3B2A312FA801926A736EEA10382CE0DE, 0x1F5BEECEB63965134EA78A78D2082A5C26A3AA688F865F868360B584482353C9),
(9, N'Valentina Ruiz',         N'valentina.ruiz@unicampus.edu',  N'Psicología',             N'Estudiante',    0x84BD10F3F69D110E05DEAF02814CBA05, 0x63E3C790081B3DFEC63444E6C1DC534EEDF65283756ED58CCD75969CD02DA494);
SET IDENTITY_INSERT dbo.Usuario OFF;
GO

SET IDENTITY_INSERT dbo.ActividadAcademica ON;
INSERT INTO dbo.ActividadAcademica
(ActividadId, Titulo, Descripcion, TipoId, FechaInicio, FechaFin, HoraInicio, HoraFin, Modalidad, Lugar, Cupos, Estado) VALUES
(1,  N'Taller de Programación Web',      N'Espacio práctico para reforzar conceptos de programación web con ASP.NET.', 1, '2026-11-03', '2026-11-06', '08:00', '12:00', N'Presencial', N'Laboratorio 3',        30,  N'Activa'),
(2,  N'Conferencia de IA',               N'Panorama actual de la inteligencia artificial y sus aplicaciones en la educación.', 2, '2026-11-20', '2026-11-20', '14:00', '17:00', N'Híbrida',    N'Auditorio principal',  100, N'Próxima'),
(3,  N'Seminario de Bases de Datos',     N'Modelado relacional, normalización y buenas prácticas en SQL Server.', 3, '2026-12-05', '2026-12-05', '09:00', '12:00', N'Presencial', N'Sala 204',             50,  N'Próxima'),
(4,  N'Curso de Diseño UX/UI',           N'Fundamentos de experiencia de usuario, prototipado y pruebas de usabilidad.', 4, '2026-12-10', '2026-12-12', '15:00', '18:00', N'Virtual',    N'Microsoft Teams',      40,  N'Próxima'),
(5,  N'Encuentro de Investigación',      N'Socialización de los semilleros y proyectos de investigación institucionales.', 5, '2026-10-15', '2026-10-16', '08:00', '17:00', N'Presencial', N'Auditorio principal',  80,  N'Finalizada'),
(6,  N'Feria de proyectos tecnológicos', N'Presentación de proyectos desarrollados por estudiantes.', 6, '2026-11-18', '2026-11-18', '09:00', '16:00', N'Presencial', N'Plazoleta central',    40,  N'Próxima'),
(7,  N'Taller de seguridad digital',     N'Buenas prácticas para proteger información y cuentas.', 1, '2026-11-04', '2026-11-05', '14:00', '17:00', N'Presencial', N'Laboratorio 2',        25,  N'Activa'),
(8,  N'Curso de Python para análisis de datos', N'Manipulación de datos con pandas y visualización básica.', 4, '2026-11-02', '2026-11-27', '18:00', '20:00', N'Virtual', N'Microsoft Teams',  35,  N'Activa'),
(9,  N'Conferencia de emprendimiento',   N'Experiencias de egresados que crearon empresas de base tecnológica.', 2, '2026-10-08', '2026-10-08', '10:00', '12:00', N'Presencial', N'Auditorio principal',  120, N'Finalizada'),
(10, N'Seminario de ética profesional',  N'Dilemas éticos en el ejercicio de la profesión.', 3, '2026-09-25', '2026-09-25', '08:00', '11:00', N'Híbrida',    N'Sala 101',             60,  N'Finalizada'),
(11, N'Taller de Git y GitHub',          N'Control de versiones, ramas y pull requests para proyectos en equipo.', 1, '2026-11-12', '2026-11-12', '08:00', '11:00', N'Presencial', N'Laboratorio 1',        3,   N'Próxima'),
(12, N'Club de lectura científica',      N'Lectura y discusión de artículos científicos recientes.', 5, '2026-08-20', '2026-08-20', '16:00', '18:00', N'Presencial', N'Biblioteca',           20,  N'Finalizada');
SET IDENTITY_INSERT dbo.ActividadAcademica OFF;
GO

INSERT INTO dbo.Inscripcion (ActividadId, UsuarioId, FechaInscripcion, Estado) VALUES
-- Taller de Programación Web (27 de 30 cupos disponibles)
(1, 3, '2026-10-28T10:00:00', N'Activa'),
(1, 4, '2026-10-29T11:30:00', N'Activa'),
(1, 5, '2026-10-30T09:15:00', N'Activa'),
-- Conferencia de IA
(2, 6, '2026-10-20T08:00:00', N'Activa'),
(2, 7, '2026-10-21T08:00:00', N'Activa'),
(2, 8, '2026-10-22T08:00:00', N'Activa'),
(2, 9, '2026-10-23T08:00:00', N'Activa'),
(2, 3, '2026-10-24T08:00:00', N'Activa'),
-- Seminario de Bases de Datos
(3, 3, '2026-11-01T14:00:00', N'Activa'),
(3, 2, '2026-11-02T14:00:00', N'Activa'),
-- Encuentro de Investigación (finalizada)
(5, 2, '2026-10-01T09:00:00', N'Activa'),
(5, 3, '2026-10-01T10:00:00', N'Activa'),
(5, 8, '2026-10-02T10:00:00', N'Activa'),
(5, 9, '2026-10-03T10:00:00', N'Activa'),
-- Taller de seguridad digital
(7, 2, '2026-10-25T16:00:00', N'Activa'),
(7, 5, '2026-10-26T16:00:00', N'Activa'),
-- Curso de Python
(8, 8, '2026-10-30T19:00:00', N'Activa'),
(8, 4, '2026-10-31T19:00:00', N'Activa'),
-- Conferencia de emprendimiento (finalizada)
(9, 2, '2026-09-30T10:00:00', N'Activa'),
(9, 4, '2026-09-30T11:00:00', N'Activa'),
(9, 5, '2026-10-01T11:00:00', N'Activa'),
-- Taller de Git y GitHub (cupo lleno: 3 de 3)
(11, 3, '2026-11-01T08:00:00', N'Activa'),
(11, 6, '2026-11-01T09:00:00', N'Activa'),
(11, 7, '2026-11-02T09:00:00', N'Activa'),
-- Club de lectura (finalizada)
(12, 9, '2026-08-10T10:00:00', N'Activa'),
(12, 7, '2026-08-11T10:00:00', N'Activa');
GO

INSERT INTO dbo.Resena (ActividadId, UsuarioId, Calificacion, Comentario, FechaCreacion) VALUES
(5,  3, 5, N'Excelente organización y ponencias muy claras.', '2026-10-17T10:00:00'),
(5,  8, 4, N'Muy buen espacio para conocer los semilleros de investigación.', '2026-10-17T12:00:00'),
(5,  9, 5, N'Me motivó a vincularme a un proyecto de investigación.', '2026-10-18T09:00:00'),
(9,  4, 4, N'Historias inspiradoras y consejos muy prácticos.', '2026-10-09T08:00:00'),
(9,  5, 3, N'Interesante, aunque faltó tiempo para las preguntas.', '2026-10-09T09:30:00'),
(7,  5, 5, N'Aprendí a configurar la verificación en dos pasos en todas mis cuentas.', '2026-11-04T18:00:00'),
(12, 7, 4, N'Buena selección de artículos y discusión enriquecedora.', '2026-08-21T10:00:00');
GO


SELECT a.ActividadId, a.Titulo, a.Estado, a.Cupos,
       (SELECT COUNT(1) FROM dbo.Inscripcion i WHERE i.ActividadId = a.ActividadId AND i.Estado = N'Activa') AS Inscritos
FROM dbo.ActividadAcademica a
ORDER BY a.ActividadId;
GO
