using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using UniCampus.Entities;
using UniCampus.Web.Infraestructura;
using UniCampus.Web.Presenters;
using UniCampus.Web.Views;

namespace UniCampus.Web
{
    /// <summary>
    /// Vista (MVP): solo lee/escribe controles y delega cada acción en el Presenter.
    /// </summary>
    public partial class Actividades : PaginaBase, IActividadesView
    {
        private const int TamanoPagina = 8;

        private ActividadesPresenter _presenter;
        private bool _puedeModerarResenas;

        protected bool PuedeGestionarInscritos { get; private set; }

        private int PaginaActual
        {
            get { return ViewState["Pagina"] is int ? (int)ViewState["Pagina"] : 0; }
            set { ViewState["Pagina"] = value; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            _presenter = new ActividadesPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _presenter.Inicializar();
            }
        }

        // ================================================================ Eventos

        protected void btnNueva_Click(object sender, EventArgs e) { _presenter.NuevaActividad(); }
        protected void btnCerrarEditor_Click(object sender, EventArgs e) { _presenter.CerrarFormulario(); }
        protected void btnGuardar_Click(object sender, EventArgs e) { _presenter.Guardar(); }
        protected void btnLimpiar_Click(object sender, EventArgs e) { _presenter.Limpiar(); }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            PaginaActual = 0;
            _presenter.Buscar();
        }

        protected void btnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            txtBuscar.Text = "";
            ddlFiltroTipo.SelectedIndex = 0;
            ddlFiltroEstado.SelectedIndex = 0;
            txtFiltroFecha.Text = "";
            PaginaActual = 0;
            _presenter.Buscar();
        }

        protected void rptActividades_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;

            switch (e.CommandName)
            {
                case "Ver": _presenter.Seleccionar(id); DesplazarA("seleccion"); break;
                case "Editar": _presenter.Editar(id); break;
                case "Eliminar": _presenter.Eliminar(id); break;
            }
        }

        protected void btnAnterior_Click(object sender, EventArgs e) { IrAPagina(PaginaActual - 1); }
        protected void btnSiguiente_Click(object sender, EventArgs e) { IrAPagina(PaginaActual + 1); }

        protected void rptPaginas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int indice;
            if (e.CommandName == "Pagina" && int.TryParse(Convert.ToString(e.CommandArgument), out indice))
            {
                IrAPagina(indice);
            }
        }

        private void IrAPagina(int indice)
        {
            PaginaActual = Math.Max(0, indice);
            _presenter.Buscar();
        }

        protected void btnMostrarInscribir_Click(object sender, EventArgs e) { pnlInscribir.Visible = true; }
        protected void btnCancelarInscribir_Click(object sender, EventArgs e) { pnlInscribir.Visible = false; }

        protected void btnConfirmarInscripcion_Click(object sender, EventArgs e)
        {
            _presenter.InscribirEstudiante();
            pnlInscribir.Visible = false;
        }

        protected void gvInscritos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id;
            if (e.CommandName == "Cancelar" && int.TryParse(Convert.ToString(e.CommandArgument), out id))
            {
                _presenter.CancelarInscripcion(id);
            }
        }

        protected void gvInscritos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header) e.Row.TableSection = System.Web.UI.WebControls.TableRowSection.TableHeader;
        }

        protected void btnInscribirme_Click(object sender, EventArgs e) { _presenter.Inscribirme(); }
        protected void btnPublicarResena_Click(object sender, EventArgs e) { _presenter.PublicarResena(); }

        protected void rptResenas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id;
            if (e.CommandName == "Eliminar" && int.TryParse(Convert.ToString(e.CommandArgument), out id))
            {
                _presenter.EliminarResena(id);
            }
        }

        // ================================================================ Ayudas para el marcado

        protected bool EsSeleccionada(object actividadId)
        {
            return Convert.ToInt32(actividadId) == ActividadSeleccionadaId;
        }

        protected string ClaseFila(object actividadId)
        {
            return EsSeleccionada(actividadId) ? "seleccionada" : "";
        }

        /// <summary>Número de fila absoluto, teniendo en cuenta la página actual.</summary>
        protected int Desplazamiento
        {
            get { return PaginaActual * TamanoPagina; }
        }

        /// <summary>Desplaza la página hasta una sección después de cargar (tras restaurar la posición).</summary>
        private void DesplazarA(string idElemento)
        {
            // Sin esto, ASP.NET restauraría la posición anterior y anularía el desplazamiento.
            MaintainScrollPositionOnPostBack = false;
            string script = "window.addEventListener('load', function () { setTimeout(function () {" +
                            " var e = document.getElementById('" + idElemento + "');" +
                            " if (e) { e.scrollIntoView({ behavior: 'smooth', block: 'start' }); } }, 120); });";
            ClientScript.RegisterStartupScript(GetType(), "desplazar", script, true);
        }

        protected bool PuedeEliminarResena(object usuarioId)
        {
            return _puedeModerarResenas || Convert.ToInt32(usuarioId) == UsuarioActual.UsuarioId;
        }

        // ================================================================ IActividadesView: datos

        public int ActividadEnEdicionId
        {
            get { int v; return int.TryParse(hfActividadEnEdicion.Value, out v) ? v : 0; }
            set { hfActividadEnEdicion.Value = value.ToString(CultureInfo.InvariantCulture); }
        }

        public string Titulo { get { return txtTitulo.Text; } set { txtTitulo.Text = value; } }
        public string TipoId { get { return ddlTipo.SelectedValue; } set { Seleccionar(ddlTipo, value); } }
        public string Descripcion { get { return txtDescripcion.Text; } set { txtDescripcion.Text = value; } }
        public string FechaInicio { get { return txtFechaInicio.Text; } set { txtFechaInicio.Text = value; } }
        public string FechaFin { get { return txtFechaFin.Text; } set { txtFechaFin.Text = value; } }
        public string HoraInicio { get { return txtHoraInicio.Text; } set { txtHoraInicio.Text = value; } }
        public string HoraFin { get { return txtHoraFin.Text; } set { txtHoraFin.Text = value; } }
        public string Modalidad { get { return ddlModalidad.SelectedValue; } set { Seleccionar(ddlModalidad, value); } }
        public string Lugar { get { return txtLugar.Text; } set { txtLugar.Text = value; } }
        public string Cupos { get { return txtCupos.Text; } set { txtCupos.Text = value; } }
        public string Estado { get { return ddlEstado.SelectedValue; } set { Seleccionar(ddlEstado, value); } }

        public string FiltroTexto { get { return txtBuscar.Text; } }
        public string FiltroTipoId { get { return ddlFiltroTipo.SelectedValue; } }
        public string FiltroEstado { get { return ddlFiltroEstado.SelectedValue; } }
        public string FiltroFecha { get { return txtFiltroFecha.Text; } }

        public int ActividadSeleccionadaId
        {
            get { int v; return int.TryParse(hfSeleccionada.Value, out v) ? v : 0; }
            set { hfSeleccionada.Value = value.ToString(CultureInfo.InvariantCulture); }
        }

        public string EstudianteAInscribirId { get { return ddlEstudiantes.SelectedValue; } }
        public string ResenaCalificacion { get { return rblCalificacion.SelectedValue; } }
        public string ResenaComentario { get { return txtComentario.Text; } }

        // ================================================================ IActividadesView: presentación

        public void CargarCatalogos(IList<TipoActividad> tipos, IList<string> modalidades, IList<string> estados)
        {
            ddlTipo.Items.Clear();
            ddlTipo.Items.Add(new ListItem("Selecciona un tipo", ""));
            ddlFiltroTipo.Items.Clear();
            ddlFiltroTipo.Items.Add(new ListItem("Todos los tipos", ""));
            foreach (var t in tipos)
            {
                string id = t.TipoId.ToString(CultureInfo.InvariantCulture);
                ddlTipo.Items.Add(new ListItem(t.Nombre, id));
                ddlFiltroTipo.Items.Add(new ListItem(t.Nombre, id));
            }

            ddlModalidad.Items.Clear();
            foreach (var m in modalidades) ddlModalidad.Items.Add(new ListItem(m, m));

            ddlEstado.Items.Clear();
            ddlFiltroEstado.Items.Clear();
            ddlFiltroEstado.Items.Add(new ListItem("Todos los estados", ""));
            foreach (var s in estados)
            {
                ddlEstado.Items.Add(new ListItem(s, s));
                ddlFiltroEstado.Items.Add(new ListItem(s, s));
            }
        }

        public void ConfigurarPermisos(bool esAdministrador)
        {
            btnNueva.Visible = esAdministrador;
            pnlEditor.Visible = esAdministrador;
            btnMostrarInscribir.Visible = esAdministrador;
        }

        public void MostrarModoEdicion(bool editando)
        {
            litTituloFormulario.Text = editando ? "Editar actividad académica" : "Nueva actividad académica";
            litTextoGuardar.Text = editando ? "Guardar cambios" : "Guardar actividad";
            btnCerrarEditor.Visible = editando;
            pnlEditor.CssClass = editando ? "panel editando" : "panel";
        }

        /// <summary>
        /// El formulario siempre está visible para el administrador; "mostrarlo" significa
        /// llevar al usuario hasta él.
        /// </summary>
        public void MostrarFormulario(bool visible)
        {
            if (visible && EsAdministrador) DesplazarA("registro");
        }

        public void MostrarResumen(ResumenActividades resumen)
        {
            litTotal.Text = resumen.TotalActividades.ToString(CultureInfo.InvariantCulture);
            litInscritos.Text = resumen.EstudiantesInscritos.ToString(CultureInfo.InvariantCulture);
            litProximas.Text = resumen.ActividadesProximas.ToString(CultureInfo.InvariantCulture);
            litFinalizadas.Text = resumen.ActividadesFinalizadas.ToString(CultureInfo.InvariantCulture);
        }

        public void MostrarActividades(IList<Actividad> actividades)
        {
            int total = actividades.Count;
            int paginas = Math.Max(1, (int)Math.Ceiling(total / (double)TamanoPagina));
            if (PaginaActual >= paginas) PaginaActual = paginas - 1;
            int actual = PaginaActual;

            rptActividades.DataSource = actividades.Skip(actual * TamanoPagina).Take(TamanoPagina).ToList();
            rptActividades.DataBind();
            rptActividades.Visible = total > 0;
            pnlSinActividades.Visible = total == 0;

            int desde = total == 0 ? 0 : actual * TamanoPagina + 1;
            int hasta = Math.Min(total, (actual + 1) * TamanoPagina);
            litPaginacion.Text = total == 0
                ? "Sin resultados"
                : string.Format("{0} {1}", total, total == 1 ? "actividad encontrada" : "actividades encontradas");
            litPaginaActual.Text = string.Format("Mostrando {0} a {1} de {2} registros", desde, hasta, total);

            rptPaginas.DataSource = Enumerable.Range(0, paginas)
                .Select(i => new { Indice = i, Numero = i + 1, Actual = i == actual })
                .ToList();
            rptPaginas.DataBind();

            btnAnterior.Enabled = actual > 0;
            btnSiguiente.Enabled = actual < paginas - 1;
            pnlPaginador.Visible = paginas > 1;
        }

        public void MostrarDetalle(Actividad a)
        {
            pnlDetalle.Visible = a != null;
            pnlDetalleVacio.Visible = a == null;
            if (a == null) return;

            litDetDia.Text = Dia(a.FechaInicio);
            litDetMes.Text = Mes(a.FechaInicio);
            litDetTitulo.Text = Server.HtmlEncode(a.Titulo);
            litDetTipo.Text = Server.HtmlEncode(a.TipoNombre);
            litDetDescripcion.Text = Server.HtmlEncode(a.Descripcion);
            litDetFecha.Text = a.FechasTexto;
            litDetHora.Text = a.HorarioTexto;
            litDetModalidad.Text = Server.HtmlEncode(a.Modalidad);
            litDetLugar.Text = Server.HtmlEncode(a.Lugar);
            litDetCupos.Text = a.CuposDisponibles + " libres de " + a.Cupos;
            litDetOcupacion.Text = a.PorcentajeOcupacion + " %";
            litDetEstado.Text = Etiqueta(a.Estado);
        }

        public void MostrarInscritos(IList<Inscripcion> inscritos, bool puedeGestionar)
        {
            PuedeGestionarInscritos = puedeGestionar;
            litCantidadInscritos.Text = inscritos.Count.ToString(CultureInfo.InvariantCulture);
            gvInscritos.DataSource = inscritos;
            gvInscritos.DataBind();
        }

        public void MostrarEstudiantesDisponibles(IList<Usuario> estudiantes)
        {
            ddlEstudiantes.Items.Clear();
            ddlEstudiantes.Items.Add(new ListItem("Selecciona un estudiante", ""));
            foreach (var u in estudiantes)
            {
                ddlEstudiantes.Items.Add(new ListItem(u.Nombre + " · " + u.Programa, u.UsuarioId.ToString(CultureInfo.InvariantCulture)));
            }
            btnMostrarInscribir.Enabled = estudiantes.Count > 0 && ActividadSeleccionadaId > 0;
        }

        public void MostrarVistaEstudiante(Actividad a, bool puedeInscribirse, string estadoInscripcion)
        {
            if (a == null) return;

            litVistaTitulo.Text = Server.HtmlEncode(a.Titulo);
            litVistaTipo.Text = Server.HtmlEncode(a.TipoNombre);
            litVistaEstado.Text = Etiqueta(a.Estado);
            litVistaFechas.Text = a.FechasTexto;
            litVistaHorario.Text = a.HorarioTexto;
            litVistaLugar.Text = Server.HtmlEncode(a.Lugar) + " · " + Server.HtmlEncode(a.Modalidad);
            litVistaCupos.Text = a.CuposDisponibles + " de " + a.Cupos + " cupos disponibles";
            litVistaDescripcion.Text = Server.HtmlEncode(a.Descripcion);

            btnInscribirme.Visible = puedeInscribirse;
            pnlAvisoVista.Visible = !string.IsNullOrEmpty(estadoInscripcion);
            litAvisoVista.Text = Server.HtmlEncode(estadoInscripcion ?? "");
            pnlAvisoVista.CssClass = estadoInscripcion != null && estadoInscripcion.StartsWith("Ya estás")
                ? "aviso positivo" : "aviso";
        }

        public void MostrarResenas(IList<Resena> resenas, Actividad actividad, string motivoNoPuedeResenar, bool puedeModerar)
        {
            _puedeModerarResenas = puedeModerar;
            litCantidadResenas.Text = resenas.Count.ToString(CultureInfo.InvariantCulture);

            if (resenas.Count > 0)
            {
                double promedio = resenas.Average(r => r.Calificacion);
                int redondeado = (int)Math.Round(promedio);
                litPromedio.Text = "<span class=\"estrellas\" aria-hidden=\"true\">" + new string('★', redondeado) + new string('☆', 5 - redondeado)
                                   + "</span> " + promedio.ToString("0.0", CultureInfo.InvariantCulture) + " de 5";
            }
            else
            {
                litPromedio.Text = "";
            }

            rptResenas.DataSource = resenas;
            rptResenas.DataBind();
            pnlSinResenas.Visible = resenas.Count == 0;

            pnlFormResena.Visible = actividad != null && motivoNoPuedeResenar == null;
            pnlMotivoResena.Visible = actividad != null && motivoNoPuedeResenar != null;
            litMotivoResena.Text = Server.HtmlEncode(motivoNoPuedeResenar ?? "");
        }

        public void LimpiarResena()
        {
            rblCalificacion.ClearSelection();
            txtComentario.Text = "";
        }

        public void MostrarExito(string mensaje) { MostrarToast(litToast, true, mensaje, null); }

        public void MostrarErrores(string titulo, IList<string> errores) { MostrarToast(litToast, false, titulo, errores); }

        // ================================================================ Utilidades

        private string Etiqueta(string estado)
        {
            return "<span class=\"" + ClaseEstado(estado) + "\">" + Server.HtmlEncode(estado) + "</span>";
        }

        private static void Seleccionar(DropDownList lista, string valor)
        {
            lista.ClearSelection();
            var item = lista.Items.FindByValue(valor ?? "");
            if (item != null) item.Selected = true;
        }
    }
}
