using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using UniCampus.Entities;
using UniCampus.Web.Infraestructura;
using UniCampus.Web.Presenters;
using UniCampus.Web.Views;

namespace UniCampus.Web
{
    public partial class MisInscripciones : PaginaBase, IMisInscripcionesView
    {
        private MisInscripcionesPresenter _presenter;

        protected void Page_Init(object sender, EventArgs e)
        {
            _presenter = new MisInscripcionesPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) _presenter.Cargar();
        }

        protected void rptInscripciones_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id;
            if (e.CommandName == "Cancelar" && int.TryParse(Convert.ToString(e.CommandArgument), out id))
                _presenter.Cancelar(id);
        }

        protected void rptResenas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id;
            if (e.CommandName == "Eliminar" && int.TryParse(Convert.ToString(e.CommandArgument), out id))
                _presenter.EliminarResena(id);
        }

        public void MostrarInscripciones(IList<Inscripcion> inscripciones)
        {
            litCantidad.Text = inscripciones.Count.ToString();
            rptInscripciones.DataSource = inscripciones;
            rptInscripciones.DataBind();
            pnlSinInscripciones.Visible = inscripciones.Count == 0;
        }

        public void MostrarResenas(IList<Resena> resenas)
        {
            rptResenas.DataSource = resenas;
            rptResenas.DataBind();
            pnlSinResenas.Visible = resenas.Count == 0;
        }

        public void MostrarSoloEstudiantes()
        {
            pnlSoloEstudiantes.Visible = true;
            pnlContenido.Visible = false;
        }

        public void MostrarExito(string mensaje)
        {
            MostrarToast(litToast, true, mensaje, null);
        }

        public void MostrarErrores(string titulo, IList<string> errores)
        {
            MostrarToast(litToast, false, titulo, errores);
        }
    }
}
