namespace UniCampus.Entities
{
    /// <summary>
    /// Valores permitidos que coinciden con las restricciones CHECK de la base de datos.
    /// </summary>
    public static class EstadosActividad
    {
        public const string Activa = "Activa";
        public const string Proxima = "Próxima";
        public const string Finalizada = "Finalizada";
        public const string Cancelada = "Cancelada";

        public static readonly string[] Todos = { Activa, Proxima, Finalizada, Cancelada };
    }

    public static class Modalidades
    {
        public const string Presencial = "Presencial";
        public const string Virtual = "Virtual";
        public const string Hibrida = "Híbrida";

        public static readonly string[] Todas = { Presencial, Virtual, Hibrida };
    }

    public static class Roles
    {
        public const string Administrador = "Administrador";
        public const string Estudiante = "Estudiante";
    }

    public static class EstadosInscripcion
    {
        public const string Activa = "Activa";
        public const string Cancelada = "Cancelada";
    }
}
