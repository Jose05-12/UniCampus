<%@ Page Title="Actividades académicas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Actividades.aspx.cs" Inherits="UniCampus.Web.Actividades" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Literal ID="litToast" runat="server" EnableViewState="false" />
    <asp:HiddenField ID="hfSeleccionada" runat="server" Value="0" />
    <asp:HiddenField ID="hfActividadEnEdicion" runat="server" Value="0" />

    <section class="banda">
        <div class="contenedor banda-interior">
            <div>
                <h1>Registro de actividades académicas</h1>
                <p>Registra, consulta y gestiona las actividades de la institución y sus inscripciones.</p>
                <div class="cifras">
                    <div class="cifra"><strong><asp:Literal ID="litTotal" runat="server" Text="0" /></strong><span>Actividades registradas</span></div>
                    <div class="cifra"><strong><asp:Literal ID="litInscritos" runat="server" Text="0" /></strong><span>Estudiantes inscritos</span></div>
                    <div class="cifra"><strong><asp:Literal ID="litProximas" runat="server" Text="0" /></strong><span>Actividades próximas</span></div>
                    <div class="cifra"><strong><asp:Literal ID="litFinalizadas" runat="server" Text="0" /></strong><span>Actividades finalizadas</span></div>
                </div>
            </div>
            <asp:LinkButton ID="btnNueva" runat="server" CssClass="btn btn-claro" OnClick="btnNueva_Click">
                <i class="bi bi-plus-lg"></i> Nueva actividad
            </asp:LinkButton>
        </div>
    </section>

    <div class="contenedor cuerpo">

        <asp:Panel ID="pnlEditor" runat="server" CssClass="panel" DefaultButton="btnGuardar" ClientIDMode="Static">
            <div class="panel-cabecera" id="registro">
                <div class="panel-titulo">
                    <span class="panel-icono" aria-hidden="true"><i class="bi bi-pencil-square"></i></span>
                    <div>
                        <h2><asp:Literal ID="litTituloFormulario" runat="server" Text="Nueva actividad académica" /></h2>
                        <p>Completa los datos y guarda. Los campos con <span class="req">*</span> son obligatorios.</p>
                    </div>
                </div>
                <asp:LinkButton ID="btnCerrarEditor" runat="server" CssClass="btn btn-fantasma btn-sm" OnClick="btnCerrarEditor_Click" Visible="false">
                    <i class="bi bi-x-lg"></i> Cancelar edición
                </asp:LinkButton>
            </div>

            <div class="panel-cuerpo formulario">
                <div class="ancho-2">
                    <label for="<%= txtTitulo.ClientID %>">Título de la actividad <span class="req">*</span></label>
                    <asp:TextBox ID="txtTitulo" runat="server" MaxLength="120" placeholder="Ej. Taller de Programación Web" />
                </div>
                <div>
                    <label for="<%= ddlTipo.ClientID %>">Tipo de actividad <span class="req">*</span></label>
                    <asp:DropDownList ID="ddlTipo" runat="server" />
                </div>
                <div>
                    <label for="<%= ddlEstado.ClientID %>">Estado <span class="req">*</span></label>
                    <asp:DropDownList ID="ddlEstado" runat="server" />
                </div>

                <div class="ancho-2 alto-2">
                    <label for="<%= txtDescripcion.ClientID %>">Descripción <span class="req">*</span></label>
                    <asp:TextBox ID="txtDescripcion" runat="server" TextMode="MultiLine" Rows="5" MaxLength="1000"
                        placeholder="¿De qué trata la actividad y a quién está dirigida?" />
                </div>
                <div>
                    <label for="<%= txtFechaInicio.ClientID %>">Fecha de inicio <span class="req">*</span></label>
                    <asp:TextBox ID="txtFechaInicio" runat="server" TextMode="Date" />
                </div>
                <div>
                    <label for="<%= txtFechaFin.ClientID %>">Fecha de fin <span class="req">*</span></label>
                    <asp:TextBox ID="txtFechaFin" runat="server" TextMode="Date" />
                </div>
                <div>
                    <label for="<%= txtHoraInicio.ClientID %>">Hora de inicio <span class="req">*</span></label>
                    <asp:TextBox ID="txtHoraInicio" runat="server" TextMode="Time" />
                </div>
                <div>
                    <label for="<%= txtHoraFin.ClientID %>">Hora de fin <span class="req">*</span></label>
                    <asp:TextBox ID="txtHoraFin" runat="server" TextMode="Time" />
                </div>

                <div>
                    <label for="<%= ddlModalidad.ClientID %>">Modalidad <span class="req">*</span></label>
                    <asp:DropDownList ID="ddlModalidad" runat="server" />
                </div>
                <div class="ancho-2">
                    <label for="<%= txtLugar.ClientID %>">Lugar o plataforma <span class="req">*</span></label>
                    <asp:TextBox ID="txtLugar" runat="server" MaxLength="150" placeholder="Ej. Laboratorio 3 o Microsoft Teams" />
                </div>
                <div>
                    <label for="<%= txtCupos.ClientID %>">Cupos disponibles <span class="req">*</span></label>
                    <asp:TextBox ID="txtCupos" runat="server" TextMode="Number" placeholder="Entre 1 y 500" />
                </div>
            </div>

            <div class="panel-pie">
                <asp:LinkButton ID="btnGuardar" runat="server" CssClass="btn btn-primario" OnClick="btnGuardar_Click">
                    <i class="bi bi-floppy"></i> <asp:Literal ID="litTextoGuardar" runat="server" Text="Guardar actividad" />
                </asp:LinkButton>
                <asp:LinkButton ID="btnLimpiar" runat="server" CssClass="btn btn-secundario" OnClick="btnLimpiar_Click">
                    <i class="bi bi-arrow-counterclockwise"></i> Limpiar
                </asp:LinkButton>
                <span class="nota">Los datos se validan antes de guardar.</span>
            </div>
        </asp:Panel>

        <section class="panel" aria-labelledby="titulo-listado">
            <div class="panel-cabecera">
                <div class="panel-titulo">
                    <span class="panel-icono" aria-hidden="true"><i class="bi bi-list-ul"></i></span>
                    <div>
                        <h2 id="titulo-listado">Listado de actividades</h2>
                        <p>Pulsa <i class="bi bi-eye"></i> para ver el detalle, los inscritos y las reseñas de una actividad.</p>
                    </div>
                </div>
                <span class="suave"><asp:Literal ID="litPaginacion" runat="server" /></span>
            </div>

            <asp:Panel ID="pnlHerramientas" runat="server" CssClass="herramientas" DefaultButton="btnBuscar">
                <div class="buscador">
                    <i class="bi bi-search" aria-hidden="true"></i>
                    <label for="<%= txtBuscar.ClientID %>" class="oculto-visual">Buscar actividad</label>
                    <asp:TextBox ID="txtBuscar" runat="server" placeholder="Buscar por título, descripción o lugar" MaxLength="100" />
                </div>
                <asp:DropDownList ID="ddlFiltroTipo" runat="server" ToolTip="Filtrar por tipo" />
                <asp:DropDownList ID="ddlFiltroEstado" runat="server" ToolTip="Filtrar por estado" />
                <asp:TextBox ID="txtFiltroFecha" runat="server" TextMode="Date" ToolTip="Actividades que se realizan en esta fecha" />
                <asp:LinkButton ID="btnBuscar" runat="server" CssClass="btn btn-primario btn-sm" OnClick="btnBuscar_Click"><i class="bi bi-search"></i> Buscar</asp:LinkButton>
                <asp:LinkButton ID="btnFiltrar" runat="server" Visible="false" OnClick="btnBuscar_Click" />
                <asp:LinkButton ID="btnLimpiarFiltros" runat="server" CssClass="btn btn-fantasma btn-sm" OnClick="btnLimpiarFiltros_Click">Quitar filtros</asp:LinkButton>
            </asp:Panel>

            <div class="tabla-adaptable">
                <asp:Repeater ID="rptActividades" runat="server" OnItemCommand="rptActividades_ItemCommand">
                    <HeaderTemplate>
                        <table class="tabla">
                            <thead>
                                <tr>
                                    <th class="indice">#</th>
                                    <th>Actividad</th>
                                    <th>Fecha</th>
                                    <th>Lugar</th>
                                    <th>Cupos</th>
                                    <th>Estado</th>
                                    <th class="acciones">Acciones</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class='<%# ClaseFila(Eval("ActividadId")) %>'>
                            <td class="indice"><%# Desplazamiento + Container.ItemIndex + 1 %></td>
                            <td class="titulo-celda"><strong><%#: Eval("Titulo") %></strong><span><%#: Eval("TipoNombre") %> · <%#: Eval("Modalidad") %></span></td>
                            <td class="fecha-celda"><%# Eval("FechasTexto") %><span><%# Eval("HorarioTexto") %></span></td>
                            <td><%#: Eval("Lugar") %></td>
                            <td>
                                <div class="medidor-cupos">
                                    <span><%# Eval("Inscritos") %> de <%# Eval("Cupos") %></span>
                                    <span class='<%# Ocupacion(Eval("Inscritos"), Eval("Cupos")) >= 100 ? "medidor lleno" : "medidor" %>'><span style='width: <%# Ocupacion(Eval("Inscritos"), Eval("Cupos")) %>%'></span></span>
                                </div>
                            </td>
                            <td><span class='<%# ClaseEstado(Eval("Estado")) %>'><%#: Eval("Estado") %></span></td>
                            <td class="acciones">
                                <asp:LinkButton ID="btnVer" runat="server" CssClass='<%# EsSeleccionada(Eval("ActividadId")) ? "btn-icono activo" : "btn-icono" %>'
                                    CommandName="Ver" CommandArgument='<%# Eval("ActividadId") %>' ToolTip="Ver detalle"><i class="bi bi-eye"></i><span class="oculto-visual">Ver detalle</span></asp:LinkButton>
                                <asp:LinkButton ID="btnEditar" runat="server" CssClass="btn-icono" CommandName="Editar" Visible='<%# EsAdministrador %>'
                                    CommandArgument='<%# Eval("ActividadId") %>' ToolTip="Editar"><i class="bi bi-pencil"></i><span class="oculto-visual">Editar</span></asp:LinkButton>
                                <asp:LinkButton ID="btnEliminar" runat="server" CssClass="btn-icono peligro" CommandName="Eliminar" Visible='<%# EsAdministrador %>'
                                    CommandArgument='<%# Eval("ActividadId") %>' ToolTip="Eliminar o desactivar"
                                    OnClientClick="return confirm('¿Eliminar esta actividad? Si tiene inscripciones o reseñas quedará como Cancelada.');"><i class="bi bi-trash3"></i><span class="oculto-visual">Eliminar</span></asp:LinkButton>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>

            <asp:Panel ID="pnlSinActividades" runat="server" CssClass="vacio" Visible="false">
                <i class="bi bi-calendar-x" aria-hidden="true"></i>
                No hay actividades con esos criterios. Prueba otra búsqueda o quita los filtros.
            </asp:Panel>

            <asp:Panel ID="pnlPaginador" runat="server" CssClass="pie-tabla">
                <span class="suave"><asp:Literal ID="litPaginaActual" runat="server" /></span>
                <nav class="paginador" aria-label="Paginación">
                    <asp:LinkButton ID="btnAnterior" runat="server" OnClick="btnAnterior_Click" ToolTip="Página anterior"><i class="bi bi-chevron-left"></i></asp:LinkButton>
                    <asp:Repeater ID="rptPaginas" runat="server" OnItemCommand="rptPaginas_ItemCommand">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnPagina" runat="server" CommandName="Pagina" CommandArgument='<%# Eval("Indice") %>'
                                CssClass='<%# (bool)Eval("Actual") ? "actual" : "" %>'><%# Eval("Numero") %></asp:LinkButton>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:LinkButton ID="btnSiguiente" runat="server" OnClick="btnSiguiente_Click" ToolTip="Página siguiente"><i class="bi bi-chevron-right"></i></asp:LinkButton>
                </nav>
            </asp:Panel>
        </section>

        <div id="seleccion"></div>
        <asp:Panel ID="pnlDetalle" runat="server">
            <div class="seleccion-titulo">
                <span class="fecha" aria-hidden="true">
                    <span class="fecha-dia"><asp:Literal ID="litDetDia" runat="server" /></span>
                    <span class="fecha-mes"><asp:Literal ID="litDetMes" runat="server" /></span>
                </span>
                <div>
                    <span class="suave">Actividad seleccionada</span>
                    <h2><asp:Literal ID="litDetTitulo" runat="server" /></h2>
                </div>
            </div>

            <div class="rejilla-3" style="margin-top: 16px;">
                <section class="panel">
                    <div class="panel-cabecera">
                        <div class="panel-titulo">
                            <span class="panel-icono" aria-hidden="true"><i class="bi bi-info-circle"></i></span>
                            <h2>Detalles de la actividad</h2>
                        </div>
                    </div>
                    <div class="panel-cuerpo">
                        <dl class="ficha">
                            <div><dt>Tipo</dt><dd><span class="etiqueta"><asp:Literal ID="litDetTipo" runat="server" /></span></dd></div>
                            <div><dt>Estado</dt><dd><asp:Literal ID="litDetEstado" runat="server" /></dd></div>
                            <div><dt>Descripción</dt><dd style="font-weight: 500;"><asp:Literal ID="litDetDescripcion" runat="server" /></dd></div>
                            <div><dt>Fecha</dt><dd class="num"><asp:Literal ID="litDetFecha" runat="server" /></dd></div>
                            <div><dt>Horario</dt><dd class="num"><asp:Literal ID="litDetHora" runat="server" /></dd></div>
                            <div><dt>Modalidad</dt><dd><asp:Literal ID="litDetModalidad" runat="server" /></dd></div>
                            <div><dt>Lugar</dt><dd><asp:Literal ID="litDetLugar" runat="server" /></dd></div>
                            <div><dt>Cupos</dt><dd class="num"><asp:Literal ID="litDetCupos" runat="server" /></dd></div>
                            <div><dt>Ocupación</dt><dd class="num"><asp:Literal ID="litDetOcupacion" runat="server" /></dd></div>
                        </dl>
                    </div>
                </section>

                <section class="panel">
                    <div class="panel-cabecera">
                        <div class="panel-titulo">
                            <span class="panel-icono" aria-hidden="true"><i class="bi bi-people"></i></span>
                            <h2>Estudiantes inscritos (<asp:Literal ID="litCantidadInscritos" runat="server" Text="0" />)</h2>
                        </div>
                        <asp:LinkButton ID="btnMostrarInscribir" runat="server" CssClass="btn btn-primario btn-sm" OnClick="btnMostrarInscribir_Click">
                            <i class="bi bi-person-plus"></i> Inscribir
                        </asp:LinkButton>
                    </div>
                    <div class="panel-cuerpo">
                        <asp:Panel ID="pnlInscribir" runat="server" CssClass="inscribir" Visible="false" DefaultButton="btnConfirmarInscripcion">
                            <label for="<%= ddlEstudiantes.ClientID %>" class="oculto-visual">Estudiante a inscribir</label>
                            <asp:DropDownList ID="ddlEstudiantes" runat="server" />
                            <asp:LinkButton ID="btnConfirmarInscripcion" runat="server" CssClass="btn btn-primario btn-sm" OnClick="btnConfirmarInscripcion_Click">Inscribir</asp:LinkButton>
                            <asp:LinkButton ID="btnCancelarInscribir" runat="server" CssClass="btn btn-fantasma btn-sm" OnClick="btnCancelarInscribir_Click">Cancelar</asp:LinkButton>
                        </asp:Panel>

                        <div class="tabla-adaptable">
                            <asp:GridView ID="gvInscritos" runat="server" AutoGenerateColumns="false" CssClass="tabla"
                                GridLines="None" OnRowCommand="gvInscritos_RowCommand" OnRowDataBound="gvInscritos_RowDataBound"
                                EmptyDataText="Todavía nadie se ha inscrito en esta actividad.">
                                <EmptyDataRowStyle CssClass="vacio" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Estudiante">
                                        <ItemTemplate>
                                            <div class="persona">
                                                <span class="iniciales" aria-hidden="true"><%#: Iniciales(Eval("NombreEstudiante")) %></span>
                                                <div><strong><%#: Eval("NombreEstudiante") %></strong><span><%#: Eval("Programa") %></span></div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="FechaInscripcion" HeaderText="Inscripción" DataFormatString="{0:dd/MM/yyyy}" ItemStyle-CssClass="num" HeaderStyle-CssClass="num" />
                                    <asp:TemplateField ItemStyle-CssClass="acciones" HeaderStyle-CssClass="acciones">
                                        <HeaderTemplate><span class="oculto-visual">Acciones</span></HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnQuitar" runat="server" CssClass="btn-icono peligro" CommandName="Cancelar"
                                                CommandArgument='<%# Eval("InscripcionId") %>' ToolTip="Cancelar inscripción" Visible='<%# PuedeGestionarInscritos %>'
                                                OnClientClick="return confirm('¿Cancelar la inscripción de este estudiante?');"><i class="bi bi-person-dash"></i><span class="oculto-visual">Cancelar inscripción</span></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </section>

                <section class="panel">
                    <div class="panel-cabecera">
                        <div class="panel-titulo">
                            <span class="panel-icono" aria-hidden="true"><i class="bi bi-phone"></i></span>
                            <h2>Vista del estudiante</h2>
                        </div>
                    </div>
                    <div class="panel-cuerpo">
                        <article class="tarjeta-estudiante">
                            <img src="Content/img/actividad.svg" alt="" />
                            <div class="tarjeta-estudiante-cuerpo">
                                <h3><asp:Literal ID="litVistaTitulo" runat="server" /></h3>
                                <div class="etiquetas">
                                    <span class="etiqueta"><asp:Literal ID="litVistaTipo" runat="server" /></span>
                                    <asp:Literal ID="litVistaEstado" runat="server" />
                                </div>
                                <ul class="lista-iconos">
                                    <li><i class="bi bi-calendar3"></i> <asp:Literal ID="litVistaFechas" runat="server" /></li>
                                    <li><i class="bi bi-clock"></i> <asp:Literal ID="litVistaHorario" runat="server" /></li>
                                    <li><i class="bi bi-geo-alt"></i> <asp:Literal ID="litVistaLugar" runat="server" /></li>
                                    <li><i class="bi bi-people"></i> <asp:Literal ID="litVistaCupos" runat="server" /></li>
                                </ul>
                                <p class="suave"><asp:Literal ID="litVistaDescripcion" runat="server" /></p>
                                <asp:LinkButton ID="btnInscribirme" runat="server" CssClass="btn btn-primario btn-bloque" OnClick="btnInscribirme_Click">
                                    <i class="bi bi-person-check"></i> Inscribirme
                                </asp:LinkButton>
                                <asp:Panel ID="pnlAvisoVista" runat="server" CssClass="aviso">
                                    <i class="bi bi-info-circle" aria-hidden="true"></i> <span><asp:Literal ID="litAvisoVista" runat="server" /></span>
                                </asp:Panel>
                            </div>
                        </article>
                    </div>
                </section>
            </div>

            <section class="panel" style="margin-top: 22px;">
                <div class="panel-cabecera">
                    <div class="panel-titulo">
                        <span class="panel-icono" aria-hidden="true"><i class="bi bi-star"></i></span>
                        <h2>Reseñas y valoraciones (<asp:Literal ID="litCantidadResenas" runat="server" Text="0" />)</h2>
                    </div>
                    <span class="promedio"><asp:Literal ID="litPromedio" runat="server" /></span>
                </div>
                <div class="panel-cuerpo resenas">
                    <div>
                        <asp:Repeater ID="rptResenas" runat="server" OnItemCommand="rptResenas_ItemCommand">
                            <ItemTemplate>
                                <article class="resena">
                                    <span class="iniciales" aria-hidden="true"><%#: Iniciales(Eval("NombreUsuario")) %></span>
                                    <div>
                                        <div class="resena-cabecera">
                                            <strong><%#: Eval("NombreUsuario") %></strong>
                                            <span class="estrellas" title='<%# Eval("Calificacion") %> de 5'><%# Eval("Estrellas") %></span>
                                            <span class="suave"><%# Eval("FechaCreacion", "{0:dd/MM/yyyy}") %></span>
                                            <asp:LinkButton ID="btnEliminarResena" runat="server" CssClass="btn-icono peligro" CommandName="Eliminar"
                                                CommandArgument='<%# Eval("ResenaId") %>' Visible='<%# PuedeEliminarResena(Eval("UsuarioId")) %>'
                                                ToolTip="Eliminar reseña" OnClientClick="return confirm('¿Eliminar esta reseña?');"><i class="bi bi-trash3"></i><span class="oculto-visual">Eliminar reseña</span></asp:LinkButton>
                                        </div>
                                        <p><%#: Eval("Comentario") %></p>
                                    </div>
                                </article>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:Panel ID="pnlSinResenas" runat="server" CssClass="vacio" Visible="false">
                            <i class="bi bi-chat-square-text" aria-hidden="true"></i>
                            Aún no hay reseñas. Quienes participen podrán calificarla cuando inicie.
                        </asp:Panel>
                    </div>

                    <div>
                        <asp:Panel ID="pnlFormResena" runat="server" CssClass="nueva-resena" DefaultButton="btnPublicarResena">
                            <h3>Deja tu reseña</h3>
                            <div>
                                <span class="etiqueta-campo">Calificación <span class="req">*</span></span>
                                <asp:RadioButtonList ID="rblCalificacion" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="calificacion">
                                    <asp:ListItem Value="1">1 ★</asp:ListItem>
                                    <asp:ListItem Value="2">2 ★</asp:ListItem>
                                    <asp:ListItem Value="3">3 ★</asp:ListItem>
                                    <asp:ListItem Value="4">4 ★</asp:ListItem>
                                    <asp:ListItem Value="5">5 ★</asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                            <div>
                                <label for="<%= txtComentario.ClientID %>">Comentario <span class="req">*</span></label>
                                <asp:TextBox ID="txtComentario" runat="server" TextMode="MultiLine" Rows="3" MaxLength="500"
                                    placeholder="¿Qué te pareció? Entre 10 y 500 caracteres." />
                            </div>
                            <asp:LinkButton ID="btnPublicarResena" runat="server" CssClass="btn btn-primario" OnClick="btnPublicarResena_Click">
                                <i class="bi bi-send"></i> Publicar reseña
                            </asp:LinkButton>
                        </asp:Panel>
                        <asp:Panel ID="pnlMotivoResena" runat="server" CssClass="aviso">
                            <i class="bi bi-info-circle" aria-hidden="true"></i> <span><asp:Literal ID="litMotivoResena" runat="server" /></span>
                        </asp:Panel>
                    </div>
                </div>
            </section>
        </asp:Panel>

        <asp:Panel ID="pnlDetalleVacio" runat="server" CssClass="panel vacio" Visible="false">
            <i class="bi bi-hand-index" aria-hidden="true"></i>
            Elige una actividad del listado para ver sus detalles, sus inscritos y sus reseñas.
        </asp:Panel>
    </div>
</asp:Content>
