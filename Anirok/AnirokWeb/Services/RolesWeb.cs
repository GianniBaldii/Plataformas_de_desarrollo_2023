namespace AnirokWeb.Services
{
    public static class RolesWeb
    {
        public const int Administrador = 1;
        public const int Vendedor = 2;
        public const int Cliente = 3;

        public static bool EsCliente(object valor)
        {
            int rol;
            return valor != null && int.TryParse(valor.ToString(), out rol) && rol == Cliente;
        }
    }
}
