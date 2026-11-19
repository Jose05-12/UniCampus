<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="UniCampus.Web.Login" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>UniCampus · Iniciar sesión</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&amp;display=swap" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" />
    <link rel="stylesheet" href="~/Content/site.css" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
        <main class="acceso">
            <asp:Panel ID="pnlLogin" runat="server" CssClass="acceso-caja" DefaultButton="btnIngresar">
                <div class="acceso-cabecera">
                    <span class="marca">
                        <span class="marca-sello" aria-hidden="true"><i class="bi bi-mortarboard-fill"></i></span>
                        <span>UniCampus</span>
                    </span>
                    <h1>Iniciar sesión</h1>
                    <p>Sistema de Gestión Académica</p>
                </div>

                <div class="acceso-cuerpo">
                    <asp:Panel ID="pnlError" runat="server" CssClass="alerta alerta-error" Visible="false" role="alert">
                        <i class="bi bi-exclamation-circle" aria-hidden="true"></i> <span><asp:Literal ID="litError" runat="server" /></span>
                    </asp:Panel>

                    <div>
                        <label for="<%= txtCorreo.ClientID %>">Correo institucional</label>
                        <asp:TextBox ID="txtCorreo" runat="server" TextMode="Email" MaxLength="150" placeholder="nombre@unicampus.edu" autocomplete="username" />
                    </div>
                    <div>
                        <label for="<%= txtPassword.ClientID %>">Contraseña</label>
                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" MaxLength="100" autocomplete="current-password" />
                    </div>

                    <asp:Button ID="btnIngresar" runat="server" Text="Ingresar" CssClass="btn btn-primario btn-bloque" OnClick="btnIngresar_Click" />
                    <p class="acceso-nota">Los usuarios de prueba están en el archivo README.md del proyecto.</p>
                </div>
            </asp:Panel>
        </main>
    </form>
</body>
</html>
