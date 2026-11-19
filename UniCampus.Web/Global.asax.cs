using System;
using System.Web;
using UniCampus.Web.Infraestructura;

namespace UniCampus.Web
{
    public class Global : HttpApplication
    {
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
