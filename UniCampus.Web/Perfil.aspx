<%@ Page Title="Perfil" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Perfil.aspx.cs" Inherits="UniCampus.Web.Perfil" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Literal ID="litToast" runat="server" EnableViewState="false" />

    <section class="banda">
        <div class="contenedor banda-interior">
            <div>
                <h1>Perfil</h1>
                <p>Los datos de tu cuenta en UniCampus.</p>
            </div>
        </div>
    </section>

    <div class="contenedor cuerpo">
        <asp:Panel ID="pnlPerfil" runat="server" CssClass="panel panel-cuerpo">
            <div class="perfil">
                <span class="iniciales" aria-hidden="true"><asp:Literal ID="litInicial" runat="server" /></span>
                <div>
                    <h2><asp:Literal ID="litNombre" runat="server" /></h2>
                    <div class="etiquetas">
                        <span class="etiqueta"><asp:Literal ID="litRol" runat="server" /></span>
                        <asp:Literal ID="litEstado" runat="server" />
                    </div>
                    <dl class="ficha">
                        <div><dt>Correo</dt><dd><asp:Literal ID="litCorreo" runat="server" /></dd></div>
                        <div><dt>Programa</dt><dd><asp:Literal ID="litPrograma" runat="server" /></dd></div>
                        <div><dt>Inscripciones activas</dt><dd class="num"><asp:Literal ID="litInscripciones" runat="server" Text="0" /></dd></div>
                        <div><dt>Reseñas publicadas</dt><dd class="num"><asp:Literal ID="litResenas" runat="server" Text="0" /></dd></div>
                    </dl>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
