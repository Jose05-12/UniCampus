<%@ Page Title="Reportes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Reportes.aspx.cs" Inherits="UniCampus.Web.Reportes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Literal ID="litToast" runat="server" EnableViewState="false" />

    <section class="banda">
        <div class="contenedor banda-interior">
            <div>
                <h1>Reportes</h1>
                <p>Ocupación de cupos, inscripciones por tipo y valoraciones de las actividades.</p>
                <asp:Panel ID="pnlCifras" runat="server" CssClass="cifras">
                    <div class="cifra"><strong><asp:Literal ID="litTotal" runat="server" /></strong><span>Actividades registradas</span></div>
                    <div class="cifra"><strong><asp:Literal ID="litInscritos" runat="server" /></strong><span>Inscripciones activas</span></div>
                    <div class="cifra"><strong><asp:Literal ID="litResenas" runat="server" /></strong><span>Reseñas</span></div>
                    <div class="cifra"><strong><asp:Literal ID="litPromedio" runat="server" /></strong><span>Calificación promedio</span></div>
                </asp:Panel>
            </div>
            <button type="button" class="btn btn-claro no-imprimir" onclick="window.print()"><i class="bi bi-printer"></i> Imprimir</button>
        </div>
    </section>

    <div class="contenedor cuerpo">
        <asp:Panel ID="pnlDenegado" runat="server" CssClass="panel panel-cuerpo" Visible="false">
            <div class="aviso"><i class="bi bi-shield-lock" aria-hidden="true"></i><span>Los reportes están disponibles solo para la coordinación académica.</span></div>
        </asp:Panel>

        <asp:Panel ID="pnlReportes" runat="server" CssClass="rejilla-2" style="grid-template-columns: minmax(0, 1.6fr) minmax(0, 1fr); align-items: start;">
            <section class="panel" aria-labelledby="titulo-ocupacion">
                <div class="panel-cabecera">
                    <div class="panel-titulo">
                        <span class="panel-icono" aria-hidden="true"><i class="bi bi-bar-chart-line"></i></span>
                        <h2 id="titulo-ocupacion">Ocupación por actividad</h2>
                    </div>
                </div>
                <div class="tabla-adaptable">
                    <asp:GridView ID="gvActividades" runat="server" AutoGenerateColumns="false" CssClass="tabla" GridLines="None"
                        OnRowDataBound="Tabla_RowDataBound" EmptyDataText="No hay actividades registradas.">
                        <Columns>
                            <asp:TemplateField HeaderText="Actividad">
                                <ItemTemplate><div class="titulo-celda"><strong><%#: Eval("Titulo") %></strong><span><%#: Eval("TipoNombre") %></span></div></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Estado">
                                <ItemTemplate><span class="<%# ClaseEstado(Eval("Estado")) %>"><%#: Eval("Estado") %></span></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Ocupación">
                                <ItemTemplate>
                                    <div class="medidor-cupos">
                                        <span><%# Eval("Inscritos") %> de <%# Eval("Cupos") %> · <%# Eval("PorcentajeOcupacion") %> %</span>
                                        <span class='<%# (int)Eval("PorcentajeOcupacion") >= 100 ? "medidor lleno" : "medidor" %>'><span style='width: <%# Math.Min(100, (int)Eval("PorcentajeOcupacion")) %>%'></span></span>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Valoración">
                                <ItemTemplate><%# Valoracion(Eval("PromedioCalificacion"), Eval("TotalResenas")) %></ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </section>

            <section class="panel" aria-labelledby="titulo-tipos">
                <div class="panel-cabecera">
                    <div class="panel-titulo">
                        <span class="panel-icono" aria-hidden="true"><i class="bi bi-pie-chart"></i></span>
                        <h2 id="titulo-tipos">Por tipo de actividad</h2>
                    </div>
                </div>
                <div class="tabla-adaptable">
                    <asp:GridView ID="gvTipos" runat="server" AutoGenerateColumns="false" CssClass="tabla" GridLines="None" OnRowDataBound="Tabla_RowDataBound">
                        <Columns>
                            <asp:BoundField DataField="Tipo" HeaderText="Tipo" />
                            <asp:BoundField DataField="CantidadActividades" HeaderText="Actividades" ItemStyle-CssClass="num" HeaderStyle-CssClass="num" />
                            <asp:BoundField DataField="TotalInscritos" HeaderText="Inscritos" ItemStyle-CssClass="num" HeaderStyle-CssClass="num" />
                            <asp:TemplateField HeaderText="Ocupación">
                                <ItemTemplate>
                                    <div class="medidor-cupos">
                                        <span><%# Eval("PorcentajeOcupacion") %> %</span>
                                        <span class="medidor"><span style='width: <%# Math.Min(100, (int)Eval("PorcentajeOcupacion")) %>%'></span></span>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </section>
        </asp:Panel>
    </div>
</asp:Content>
