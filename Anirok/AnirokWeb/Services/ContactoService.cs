using AnirokWeb.Models;
using System;
using System.Data.SqlClient;

namespace AnirokWeb.Services
{
    public static class ContactoService
    {
        public static int Guardar(ContactoViewModel contacto, string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(@"INSERT INTO dbo.contacto (asunto, email, mensaje, fecha, estado) OUTPUT INSERTED.id VALUES (@asunto, @email, @mensaje, SYSDATETIME(), 'Pendiente')", connection))
            {
                command.Parameters.AddWithValue("@asunto", contacto.Asunto.Trim());
                command.Parameters.AddWithValue("@email", contacto.Email.Trim());
                command.Parameters.AddWithValue("@mensaje", contacto.Mensaje.Trim());
                connection.Open();
                return (int)command.ExecuteScalar();
            }
        }
    }
}
