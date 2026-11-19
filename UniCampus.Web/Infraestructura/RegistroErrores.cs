using System;
using System.Diagnostics;
using System.IO;
using System.Web.Hosting;

namespace UniCampus.Web.Infraestructura
{
    public static class RegistroErrores
    {
        private static readonly object Bloqueo = new object();

        public static void Registrar(Exception ex, string contexto)
        {
            string texto = string.Format("[{0:yyyy-MM-dd HH:mm:ss}] {1}{2}{3}{2}{2}",
                DateTime.Now, contexto, Environment.NewLine, ex);

            Trace.TraceError(texto);

            try
            {
                string carpeta = HostingEnvironment.MapPath("~/App_Data/Logs");
                if (carpeta == null) return;

                lock (Bloqueo)
                {
                    Directory.CreateDirectory(carpeta);
                    File.AppendAllText(Path.Combine(carpeta, "errores-" + DateTime.Now.ToString("yyyyMMdd") + ".log"), texto);
                }
            }
            catch
            {
            }
        }
    }
}
