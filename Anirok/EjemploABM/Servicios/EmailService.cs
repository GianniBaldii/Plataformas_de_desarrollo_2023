using System;
using System.Net;
using System.Net.Mail;
using System.Data.SqlClient;
using EjemploABM.Controladores;
using System.IO;

namespace EjemploABM.Servicios
{
    internal static class EmailService
    {
        public static bool Configurado { get { return !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ANIROK_SMTP_HOST")); } }

        public static void EnviarContraseñaTemporal(string destinatario, string nombre, string temporal)
        {
            var asunto = "Anirok - contraseña temporal";
            var cuerpo = "Hola " + nombre + ",\n\nTu contraseña temporal es: " + temporal + "\n\nIngresá y cambiala cuanto antes.";
            var id = Registrar("Reset contraseña", destinatario, asunto, cuerpo);
            try { EnviarInterno(destinatario, asunto, cuerpo); Marcar(id, "Enviado", null); }
            catch (Exception ex) { Marcar(id, "Fallido", ex.Message); throw; }
        }

        public static void Reintentar(int id)
        {
            string destino = null, asunto = null, cuerpo = null, nombreAdjunto = null;
            byte[] adjunto = null;
            using (var c = CreateConnection())
            using (var q = new SqlCommand("SELECT destinatario, asunto, cuerpo, adjunto, nombre_adjunto FROM email_envio WHERE id=@id", c))
            {
                q.Parameters.AddWithValue("@id", id);
                c.Open();
                using (var r = q.ExecuteReader())
                {
                    if (!r.Read()) throw new InvalidOperationException("No se encontró el envío.");
                    destino = r.GetString(0); asunto = r.GetString(1); cuerpo = r.GetString(2);
                    if (!r.IsDBNull(3)) adjunto = (byte[])r[3];
                    if (!r.IsDBNull(4)) nombreAdjunto = r.GetString(4);
                }
            }
            try { IncrementarIntento(id); Marcar(id, "Pendiente", null); EnviarInterno(destino, asunto, cuerpo, adjunto, nombreAdjunto); Marcar(id, "Enviado", null); } catch(Exception ex) { Marcar(id,"Fallido",ex.Message); throw; }
        }

        private static void EnviarInterno(string destinatario, string asunto, string cuerpo, byte[] adjunto = null, string nombreAdjunto = null)
        {
            var host = Environment.GetEnvironmentVariable("ANIROK_SMTP_HOST");
            var user = Environment.GetEnvironmentVariable("ANIROK_SMTP_USER");
            var password = Environment.GetEnvironmentVariable("ANIROK_SMTP_PASSWORD");
            var from = Environment.GetEnvironmentVariable("ANIROK_SMTP_FROM") ?? user;
            var port = 587; int.TryParse(Environment.GetEnvironmentVariable("ANIROK_SMTP_PORT"), out port);
            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(from)) throw new InvalidOperationException("SMTP no está configurado.");
            using (var message = new MailMessage(from, destinatario, asunto, cuerpo))
            using (var client = new SmtpClient(host, port))
            {
                if (adjunto != null && adjunto.Length > 0) message.Attachments.Add(new Attachment(new MemoryStream(adjunto), nombreAdjunto ?? "adjunto.pdf", "application/pdf"));
                client.EnableSsl = true; client.Credentials = new NetworkCredential(user, password); client.Send(message);
            }
        }

        private static SqlConnection CreateConnection()
        {
            if (DB_Controller.connection == null) DB_Controller.initialize();
            return new SqlConnection(DB_Controller.connection.ConnectionString);
        }
        private static int Registrar(string tipo,string destino,string asunto,string cuerpo){using(var c=CreateConnection())using(var q=new SqlCommand("INSERT INTO email_envio(tipo,destinatario,asunto,cuerpo,estado,intentos,ultimo_intento) OUTPUT INSERTED.id VALUES(@t,@d,@a,@c,'Pendiente',1,SYSDATETIME())",c)){q.Parameters.AddWithValue("@t",tipo);q.Parameters.AddWithValue("@d",destino);q.Parameters.AddWithValue("@a",asunto);q.Parameters.AddWithValue("@c",cuerpo);c.Open();return(int)q.ExecuteScalar();}}
        private static void IncrementarIntento(int id){using(var c=CreateConnection())using(var q=new SqlCommand("UPDATE email_envio SET intentos=intentos+1,ultimo_intento=SYSDATETIME() WHERE id=@id",c)){q.Parameters.AddWithValue("@id",id);c.Open();q.ExecuteNonQuery();}}
        private static void Marcar(int id,string estado,string error){using(var c=CreateConnection())using(var q=new SqlCommand("UPDATE email_envio SET estado=@e,error=@x,ultimo_intento=SYSDATETIME() WHERE id=@id",c)){q.Parameters.AddWithValue("@e",estado);q.Parameters.AddWithValue("@x",(object)error??DBNull.Value);q.Parameters.AddWithValue("@id",id);c.Open();q.ExecuteNonQuery();}}
    }
}
