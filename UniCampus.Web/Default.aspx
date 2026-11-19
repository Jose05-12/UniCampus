<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="UniCampus.Web._Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Literal ID="litToast" runat="server" EnableViewState="false" />

    <section class="banda">
        <div class="contenedor banda-interior">
            <div>
                <h1>Hola, <asp:Literal ID="litNombre" runat="server" /></h1>
                <p>Este es el resumen de las actividades académicas de la institución.</p>
                <div class="cifras">
                    <div class="cifra"><strong><asp:Literal ID="litTotal" runat="server" Text="0" /></strong><span>Actividades registradas</span></div>
                    <div class="cifra"><strong><asp:Literal ID="litActivas" runat="server" Text="0" /></strong><span>En curso</span></div>
                    <div class="cifra"><strong><asp:Literal ID="litProximas" runat="server" Text="0" /></strong><span>Próximas</span></div>
                    <div class="cifra"><strong><asp:Literal ID="litResenas" runat="server" Text="0" /></strong><span>Reseñas publicadas</span></div>
                </div>
            </div>
            <a class="btn btn-claro" href="Actividades.aspx"><i class="bi bi-calendar3"></i> Ir a actividades</a>
        </div>
    </section>

    <div class="contenedor cuerpo">
        <div class="rejilla-2">
            <section class="panel" aria-labelledby="titulo-proximas">
                <div class="panel-cabecera">
                    <div class="panel-titulo">
                        <span class="panel-icono" aria-hidden="true"><i class="bi bi-calendar-event"></i></span>
                        <h2 id="titulo-proximas">Próximas actividades</h2>
                    </div>
                    <a href="Actividades.aspx">Ver todas</a>
                </div>
                <asp:Repeater ID="rptProximas" runat="server">
                    <HeaderTemplate><ul class="eventos"></HeaderTemplate>
                    <ItemTemplate>
                        <li class="evento">
                            <span class="fecha" aria-hidden="true">
                                <span class="fecha-dia"><%# Dia(Eval("FechaInicio")) %></span>
                                <span class="fecha-mes"><%# Mes(Eval("FechaInicio")) %></span>
                            </span>
                            <div style="min-width: 0;">
                                <span class="evento-titulo"><%#: Eval("Titulo") %></span>
                                <p class="evento-meta"><%#: Eval("TipoNombre") %> · <%# Eval("HorarioTexto") %> · <%#: Eval("Lugar") %></p>
                            </div>
                            <div class="evento-lado">
                                <span class='<%# ClaseEstado(Eval("Estado")) %>'><%#: Eval("Estado") %></span>
                                <span><%# Eval("CuposDisponibles") %> cupos libres</span>
                            </div>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate></ul></FooterTemplate>
                </asp:Repeater>
                <asp:Panel ID="pnlSinProximas" runat="server" CssClass="vacio" Visible="false">
                    <i class="bi bi-calendar-check" aria-hidden="true"></i>
                    No hay actividades abiertas por ahora.
                </asp:Panel>
            </section>

            <section class="panel" aria-labelledby="titulo-inscripciones">
                <div class="panel-cabecera">
                    <div class="panel-titulo">
                        <span class="panel-icono" aria-hidden="true"><i class="bi bi-bookmark-check"></i></span>
                        <h2 id="titulo-inscripciones"><asp:Literal ID="litTituloInscripciones" runat="server" Text="Mis inscripciones" /></h2>
                    </div>
                    <a href="MisInscripciones.aspx">Gestionar</a>
                </div>
                <asp:Repeater ID="rptMisInscripciones" runat="server">
                    <HeaderTemplate><ul class="eventos"></HeaderTemplate>
                    <ItemTemplate>
                        <li class="evento">
                            <span class="fecha" aria-hidden="true">
                                <span class="fecha-dia"><%# Dia(Eval("FechaInicioActividad")) %></span>
                                <span class="fecha-mes"><%# Mes(Eval("FechaInicioActividad")) %></span>
                            </span>
                            <div style="min-width: 0;">
                                <span class="evento-titulo"><%#: Eval("TituloActividad") %></span>
                                <p class="evento-meta"><%#: Eval("TipoActividad") %> · <%#: Eval("LugarActividad") %></p>
                            </div>
                            <span class='<%# ClaseEstado(Eval("EstadoActividad")) %>'><%#: Eval("EstadoActividad") %></span>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate></ul></FooterTemplate>
                </asp:Repeater>
                <asp:Panel ID="pnlSinInscripciones" runat="server" CssClass="vacio" Visible="false">
                    <i class="bi bi-bookmark" aria-hidden="true"></i>
                    <asp:Literal ID="litSinInscripciones" runat="server" />
                </asp:Panel>
            </section>
        </div>
    </div>
</asp:Content>
