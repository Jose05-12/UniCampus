using System;
using System.Collections.Generic;
using System.Globalization;
using UniCampus.Entities;
using UniCampus.Web.Infraestructura;
using UniCampus.Web.Presenters;
using UniCampus.Web.Views;

namespace UniCampus.Web
{
    public partial class _Default : PaginaBase, IInicioView
    {
        private InicioPresenter _presenter;

        protected void Page_Init(object sender, EventArgs e)
        {
            _presenter = new InicioPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                litNombre.Text = Server.HtmlEncode(UsuarioActual.Nombre);
                _presenter.Cargar();
            }
        }

        public void MostrarResumen(ResumenActividades r)
        {
            litTotal.Text = r.TotalActividades.ToString(CultureInfo.InvariantCulture);
            litActivas.Text = r.ActividadesActivas.ToString(CultureInfo.InvariantCulture);
            litProximas.Text = r.ActividadesProximas.ToString(CultureInfo.InvariantCulture);
            litResenas.Text = r.TotalResenas.ToString(CultureInfo.InvariantCulture);
        }

        public void MostrarProximas(IList<Actividad> actividades)
        {
            rptProximas.DataSource = actividades;
            rptProximas.DataBind();
            pnlSinProximas.Visible = actividades.Count == 0;
        }

        public void MostrarMisInscripciones(IList<Inscripcion> inscripciones)
        {
            rptMisInscripciones.DataSource = inscripciones;
            rptMisInscripciones.DataBind();
            pnlSinInscripciones.Visible = inscripciones.Count == 0;

            if (UsuarioActual.EsAdministrador)
            {
                litTituloInscripciones.Text = "Inscripciones";
                litSinInscripciones.Text = "Como administrador, inscribes estudiantes desde el detalle de cada actividad.";
            }
            else
            {
                litTituloInscripciones.Text = "Mis inscripciones (" + inscripciones.Count + ")";
                litSinInscripciones.Text = "Aún no te has inscrito en ninguna actividad. Revisa la agenda y elige una.";
            }
        }

        public void MostrarError(string mensaje)
        {
            MostrarToast(litToast, false, mensaje, null);
        }
    }
}
