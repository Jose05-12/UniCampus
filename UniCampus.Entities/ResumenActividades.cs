namespace UniCampus.Entities
{
    public class ResumenActividades
    {
        public int TotalActividades { get; set; }
        public int EstudiantesInscritos { get; set; }
        public int ActividadesActivas { get; set; }
        public int ActividadesProximas { get; set; }
        public int ActividadesFinalizadas { get; set; }
        public int TotalResenas { get; set; }
        public decimal? PromedioGeneral { get; set; }
    }
}
