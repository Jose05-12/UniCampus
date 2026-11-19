using System;
using System.Web;
using UniCampus.Web.Infraestructura;

namespace UniCampus.Web
{
    public class Global : HttpApplication
    {
        /// <summary>Registra cualquier error no controlado antes de mostrar Error.aspx.</summary>
        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            if (ex != null && !(ex is HttpException && ((HttpException)ex).GetHttpCode() == 404))
            {
                RegistroErrores.Registrar(ex, "Error no controlado en " + Request.RawUrl);
            }
        }
    }
}
