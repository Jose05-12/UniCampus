using System;
using System.Collections.Generic;
using System.Globalization;
using UniCampus.Entities;
using UniCampus.Web.Infraestructura;
using UniCampus.Web.Presenters;
using UniCampus.Web.Views;

namespace UniCampus.Web
{
    public partial class Reportes : PaginaBase, IReportesView
    {
        private ReportesPresenter _presenter;

        protected void Page_Init(object sender, EventArgs e)
        {
            _presenter = new ReportesPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) _presenter.Cargar();
        }

        protected string Valoracion(object promedio, object total)
        {
            if (promedio == null || promedio == DBNull.Value) return "<span class=\"suave\">Sin reseñas</span>";
            decimal valor = Convert.ToDecimal(promedio);
            return "<span class=\"estrellas\" aria-hidden=\"true\">★</span> " + valor.ToString("0.0", CultureInfo.InvariantCulture)
                   + " <span class=\"suave\">(" + Convert.ToInt32(total) + ")</span>";
        }

        protected void Tabla_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (e.Row.RowType == System.Web.UI.WebControls.DataControlRowType.Header)
                e.Row.TableSection = System.Web.UI.WebControls.TableRowSection.TableHeader;
        }

        public void MostrarResumen(ResumenActividades r)
        {
            litTotal.Text = r.TotalActividades.ToString(CultureInfo.InvariantCulture);
            litInscritos.Text = r.EstudiantesInscritos.ToString(CultureInfo.InvariantCulture);
            litPromedio.Text = r.PromedioGeneral.HasValue ? r.PromedioGeneral.Value.ToString("0.0", CultureInfo.InvariantCulture) : "-";
            litResenas.Text = r.TotalResenas.ToString(CultureInfo.InvariantCulture);
        }

        public void MostrarActividades(IList<Actividad> actividades)
        {
            gvActividades.DataSource = actividades;
            gvActividades.DataBind();
        }

        public void MostrarPorTipo(IList<ReporteTipo> tipos)
        {
            gvTipos.DataSource = tipos;
            gvTipos.DataBind();
        }

        public void MostrarAccesoDenegado()
        {
            pnlDenegado.Visible = true;
            pnlReportes.Visible = false;
            pnlCifras.Visible = false;
        }

        public void MostrarError(string mensaje)
        {
            MostrarToast(litToast, false, mensaje, null);
        }
    }
}
