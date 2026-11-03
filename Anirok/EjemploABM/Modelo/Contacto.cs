using System;

namespace EjemploABM.Modelo
{
    public class Contacto
    {
        public int Id { get; set; }
        public string Asunto { get; set; }
        public string Email { get; set; }
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }
    }
}
