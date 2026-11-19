namespace UniCampus.Entities
{
    public class ReporteTipo
    {
        public string Tipo { get; set; }
        public int CantidadActividades { get; set; }
        public int TotalCupos { get; set; }
        public int TotalInscritos { get; set; }

        public int PorcentajeOcupacion
        {
            get { return TotalCupos <= 0 ? 0 : (int)System.Math.Round(TotalInscritos * 100.0 / TotalCupos); }
        }
    }
}
