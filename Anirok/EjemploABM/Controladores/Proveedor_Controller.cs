using EjemploABM.Modelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EjemploABM.Controladores
{
    public static class Proveedor_Controller
    {
        public static List<Proveedor> ObtenerTodos(string filtro = "")
        {
            var lista = new List<Proveedor>();
            bool cerrar = AbrirConexion();
            try
            {
                using (var command = new SqlCommand(@"SELECT id, nombre, contacto, telefono, email, direccion, cuit, esta_activo FROM dbo.proveedor WHERE (@filtro = '' OR nombre LIKE '%' + @filtro + '%' OR cuit LIKE '%' + @filtro + '%') ORDER BY nombre", DB_Controller.connection))
                {
                    command.Parameters.AddWithValue("@filtro", filtro ?? string.Empty);
                    using (var reader = command.ExecuteReader())
                    while (reader.Read()) lista.Add(new Proveedor { Id = reader.GetInt32(0), Nombre = reader.GetString(1), Contacto = reader.IsDBNull(2) ? "" : reader.GetString(2), Telefono = reader.IsDBNull(3) ? "" : reader.GetString(3), Email = reader.IsDBNull(4) ? "" : reader.GetString(4), Direccion = reader.IsDBNull(5) ? "" : reader.GetString(5), Cuit = reader.IsDBNull(6) ? "" : reader.GetString(6), Estado = reader.IsDBNull(7) ? "Activa" : reader.GetString(7) });
                }
            }
            finally { if (cerrar) DB_Controller.connection.Close(); }
            return lista;
        }

        public static void Guardar(Proveedor proveedor)
        {
            string sql = proveedor.Id == 0 ? @"INSERT INTO dbo.proveedor (nombre, contacto, telefono, email, direccion, cuit, esta_activo) VALUES (@nombre,@contacto,@telefono,@email,@direccion,@cuit,'Activa')" : @"UPDATE dbo.proveedor SET nombre=@nombre, contacto=@contacto, telefono=@telefono, email=@email, direccion=@direccion, cuit=@cuit WHERE id=@id";
            bool cerrar = AbrirConexion();
            try
            {
                using (var command = new SqlCommand(sql, DB_Controller.connection))
                { command.Parameters.AddWithValue("@nombre", proveedor.Nombre); command.Parameters.AddWithValue("@contacto", (object)proveedor.Contacto ?? DBNull.Value); command.Parameters.AddWithValue("@telefono", (object)proveedor.Telefono ?? DBNull.Value); command.Parameters.AddWithValue("@email", (object)proveedor.Email ?? DBNull.Value); command.Parameters.AddWithValue("@direccion", (object)proveedor.Direccion ?? DBNull.Value); command.Parameters.AddWithValue("@cuit", (object)proveedor.Cuit ?? DBNull.Value); command.Parameters.AddWithValue("@id", proveedor.Id); command.ExecuteNonQuery(); }
            }
            finally { if (cerrar) DB_Controller.connection.Close(); }
        }

        public static void Desactivar(int id)
        {
            bool cerrar = AbrirConexion();
            try { using (var command = new SqlCommand("UPDATE dbo.proveedor SET esta_activo='Inactiva' WHERE id=@id", DB_Controller.connection)) { command.Parameters.AddWithValue("@id", id); command.ExecuteNonQuery(); } }
            finally { if (cerrar) DB_Controller.connection.Close(); }
        }

        private static bool AbrirConexion()
        {
            if (DB_Controller.connection == null) DB_Controller.initialize();
            if (DB_Controller.connection.State == ConnectionState.Open) return false;
            DB_Controller.connection.Open();
            return true;
        }
    }
}
