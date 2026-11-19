SELECT a.ActividadId, a.Titulo, t.Nombre AS Tipo, a.FechaInicio, a.FechaFin, a.Modalidad, a.Lugar, a.Cupos,
       (SELECT COUNT(*) FROM Inscripcion i WHERE i.ActividadId = a.ActividadId AND i.Estado = 'Activa') AS Inscritos, a.Estado
FROM ActividadAcademica a JOIN TipoActividad t ON t.TipoId = a.TipoId
ORDER BY a.ActividadId;
