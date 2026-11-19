using System;
using System.Linq;
using UniCampus.BLL;
using UniCampus.Web.Infraestructura;
using UniCampus.Web.Views;

namespace UniCampus.Web.Presenters
{
    public class ReportesPresenter
    {
        private readonly IReportesView _view;
        private readonly ActividadService _actividades;

        public ReportesPresenter(IReportesView view)
        {
            _view = view;
            _actividades = new ActividadService();
        }

        public void Cargar()
        {
            if (!_view.UsuarioActual.EsAdministrador)
            {
                _view.MostrarAccesoDenegado();
                return;
            }

            try
            {
                _view.MostrarResumen(_actividades.ObtenerResumen());
                _view.MostrarActividades(_actividades.Listar(null)
                    .OrderByDescending(a => a.PorcentajeOcupacion)
                    .ThenBy(a => a.Titulo)
                    .ToList());
                _view.MostrarPorTipo(_actividades.ReportePorTipo());
            }
            catch (Exception ex)
            {
                RegistroErrores.Registrar(ex, "ReportesPresenter.Cargar");
                _view.MostrarError("No fue posible generar los reportes. Intente nuevamente.");
            }
        }
    }
}
