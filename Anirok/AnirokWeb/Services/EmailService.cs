using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Data.SqlClient;

namespace AnirokWeb.Services
{
    public static class EmailService
    {
        public static bool Configurado { get { return !string.IsNullOrWhiteSpace(GetSetting("ANIROK_SMTP_HOST")); } }

        public static void EnviarComprobante(string destinatario, int ventaId, byte[] pdf)
        {
            Enviar(destinatario, "Anirok - comprobante de compra #" + ventaId, "Gracias por tu compra. Adjuntamos el comprobante de tu pedido.", "Anirok-venta-" + ventaId + ".pdf", pdf);
        }

        private static void Enviar(string destinatario, string asunto, string cuerpo, string nombreAdjunto, byte[] adjunto)
        {
            var connectionString = AnirokWeb.Models.AnirokDbContext.ResolveConnectionString();
            var id = Registrar(connectionString, "Compra", destinatario, asunto, cuerpo, nombreAdjunto, adjunto);
            try { EnviarInterno(destinatario, asunto, cuerpo, nombreAdjunto, adjunto); Marcar(connectionString, id, "Enviado", null); }
            catch (Exception ex) { Marcar(connectionString, id, "Fallido", ex.Message); throw; }
        }

        private static void EnviarInterno(string destinatario, string asunto, string cuerpo, string nombreAdjunto, byte[] adjunto)
        {
            var host = GetSetting("ANIROK_SMTP_HOST");
            var user = GetSetting("ANIROK_SMTP_USER");
            var password = GetSetting("ANIROK_SMTP_PASSWORD");
            var from = GetSetting("ANIROK_SMTP_FROM") ?? user;
            var port = 587;
            int.TryParse(GetSetting("ANIROK_SMTP_PORT"), out port);
            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(from)) throw new InvalidOperationException("SMTP no está configurado.");
            using (var message = new MailMessage(from, destinatario, asunto, cuerpo))
            using (var stream = new MemoryStream(adjunto ?? new byte[0]))
            {
                message.Attachments.Add(new Attachment(stream, nombreAdjunto, "application/pdf"));
                using (var client = new SmtpClient(host, port))
                {
                    client.EnableSsl = true; client.Credentials = new NetworkCredential(user, password); client.Send(message);
                }
            }
        }

        private static int Registrar(string cs, string tipo, string destino, string asunto, string cuerpo, string nombre, byte[] adjunto) { using(var c=new SqlConnection(cs)) using(var q=new SqlCommand("INSERT INTO email_envio(tipo,destinatario,asunto,cuerpo,adjunto,nombre_adjunto,estado,intentos,ultimo_intento) OUTPUT INSERTED.id VALUES(@t,@d,@a,@c,@f,@n,'Pendiente',1,SYSDATETIME())",c)){q.Parameters.AddWithValue("@t",tipo);q.Parameters.AddWithValue("@d",destino);q.Parameters.AddWithValue("@a",asunto);q.Parameters.AddWithValue("@c",cuerpo);q.Parameters.Add("@f",System.Data.SqlDbType.VarBinary,-1).Value=(object)adjunto??DBNull.Value;q.Parameters.AddWithValue("@n",(object)nombre??DBNull.Value);c.Open();return (int)q.ExecuteScalar();} }
        private static void Marcar(string cs,int id,string estado,string error) { using(var c=new SqlConnection(cs)) using(var q=new SqlCommand("UPDATE email_envio SET estado=@e,error=@x,ultimo_intento=SYSDATETIME() WHERE id=@id",c)){q.Parameters.AddWithValue("@e",estado);q.Parameters.AddWithValue("@x",(object)error??DBNull.Value);q.Parameters.AddWithValue("@id",id);c.Open();q.ExecuteNonQuery();} }

        private static string GetSetting(string key)
        {
            var value = Environment.GetEnvironmentVariable(key);
            if (!string.IsNullOrWhiteSpace(value)) return value.Trim();
            foreach (var start in new[] { AppDomain.CurrentDomain.BaseDirectory, Environment.CurrentDirectory })
            {
                var directory = new DirectoryInfo(start);
                while (directory != null)
                {
                    var file = Path.Combine(directory.FullName, ".env.local");
                    if (File.Exists(file))
                    {
                        foreach (var line in File.ReadAllLines(file))
                        {
                            var clean = line.Trim();
                            if (clean.StartsWith(key + "=", StringComparison.OrdinalIgnoreCase))
                                return clean.Substring(key.Length + 1).Trim().Trim('"');
                        }
                    }
                    directory = directory.Parent;
                }
            }
            return null;
        }
    }
}
