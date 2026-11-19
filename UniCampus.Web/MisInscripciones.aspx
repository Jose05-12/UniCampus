<%@ Page Title="Mis inscripciones" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="MisInscripciones.aspx.cs" Inherits="UniCampus.Web.MisInscripciones" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Literal ID="litToast" runat="server" EnableViewState="false" />

    <section class="banda">
        <div class="contenedor banda-interior">
            <div>
                <h1>Mis inscripciones</h1>
                <p>Las actividades en las que participas y las reseñas que has escrito.</p>
            </div>
            <a class="btn btn-claro" href="Actividades.aspx"><i class="bi bi-search"></i> Buscar actividades</a>
        </div>
    </section>

    <div class="contenedor cuerpo">
        <asp:Panel ID="pnlSoloEstudiantes" runat="server" CssClass="panel panel-cuerpo" Visible="false">
            <div class="aviso">
                <i class="bi bi-info-circle" aria-hidden="true"></i>
                <span>Esta sección es para estudiantes. Como administrador, inscribes y cancelas inscripciones desde el detalle de cada
                    actividad en <a href="Actividades.aspx">Actividades</a>.</span>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlContenido" runat="server" CssClass="rejilla-2">
            <section class="panel" aria-labelledby="titulo-inscritas">
                <div class="panel-cabecera">
                    <div class="panel-titulo">
                        <span class="panel-icono" aria-hidden="true"><i class="bi bi-list-check"></i></span>
                        <h2 id="titulo-inscritas">Actividades inscritas (<asp:Literal ID="litCantidad" runat="server" Text="0" />)</h2>
                    </div>
                </div>
                <asp:Repeater ID="rptInscripciones" runat="server" OnItemCommand="rptInscripciones_ItemCommand">
                    <HeaderTemplate><ul class="eventos"></HeaderTemplate>
                    <ItemTemplate>
                        <li class="evento con-accion">
                            <span class="fecha" aria-hidden="true">
                                <span class="fecha-dia"><%# Dia(Eval("FechaInicioActividad")) %></span>
                                <span class="fecha-mes"><%# Mes(Eval("FechaInicioActividad")) %></span>
                            </span>
                            <div style="min-width: 0;">
                                <span class="evento-titulo"><%#: Eval("TituloActividad") %></span>
                                <p class="evento-meta"><%#: Eval("TipoActividad") %> · <%#: Eval("LugarActividad") %> · inscrito el <%# Eval("FechaInscripcion", "{0:dd/MM/yyyy}") %></p>
                            </div>
                            <span class='<%# ClaseEstado(Eval("EstadoActividad")) %>'><%#: Eval("EstadoActividad") %></span>
                            <div>
                                <asp:LinkButton ID="btnCancelar" runat="server" CssClass="btn btn-peligro btn-sm" CommandName="Cancelar"
                                    CommandArgument='<%# Eval("InscripcionId") %>' Visible='<%# (string)Eval("EstadoActividad") != "Finalizada" %>'
                                    OnClientClick="return confirm('¿Cancelar tu inscripción? Liberarás el cupo.');">Cancelar</asp:LinkButton>
                            </div>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate></ul></FooterTemplate>
                </asp:Repeater>
                <asp:Panel ID="pnlSinInscripciones" runat="server" CssClass="vacio" Visible="false">
                    <i class="bi bi-bookmark" aria-hidden="true"></i>
                    Aún no te has inscrito en ninguna actividad. <a href="Actividades.aspx">Revisa el listado</a> y elige una.
                </asp:Panel>
            </section>

            <section class="panel" aria-labelledby="titulo-resenas">
                <div class="panel-cabecera">
                    <div class="panel-titulo">
                        <span class="panel-icono" aria-hidden="true"><i class="bi bi-star"></i></span>
                        <h2 id="titulo-resenas">Mis reseñas</h2>
                    </div>
                </div>
                <div class="panel-cuerpo">
                    <asp:Repeater ID="rptResenas" runat="server" OnItemCommand="rptResenas_ItemCommand">
                        <ItemTemplate>
                            <article class="resena">
                                <span class="iniciales" aria-hidden="true"><i class="bi bi-chat-quote"></i></span>
                                <div>
                                    <div class="resena-cabecera">
                                        <span class="estrellas" title='<%# Eval("Calificacion") %> de 5'><%# Eval("Estrellas") %></span>
                                        <span class="suave"><%# Eval("FechaCreacion", "{0:dd/MM/yyyy}") %></span>
                                        <asp:LinkButton ID="btnEliminarResena" runat="server" CssClass="btn-icono peligro" CommandName="Eliminar"
                                            CommandArgument='<%# Eval("ResenaId") %>' ToolTip="Eliminar reseña"
                                            OnClientClick="return confirm('¿Eliminar esta reseña?');"><i class="bi bi-trash3"></i><span class="oculto-visual">Eliminar reseña</span></asp:LinkButton>
                                    </div>
                                    <p><%#: Eval("Comentario") %></p>
                                </div>
                            </article>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Panel ID="pnlSinResenas" runat="server" CssClass="vacio" Visible="false">
                        <i class="bi bi-star" aria-hidden="true"></i>
                        Todavía no has escrito reseñas. Podrás hacerlo en las actividades donde estés inscrito, cuando hayan iniciado.
                    </asp:Panel>
                </div>
            </section>
        </asp:Panel>
    </div>
</asp:Content>
