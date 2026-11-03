using System;
namespace EjemploABM.Modelo { public class EmailEnvio { public int Id {get;set;} public string Tipo {get;set;} public string Destinatario {get;set;} public string Asunto {get;set;} public string Estado {get;set;} public string Error {get;set;} public int Intentos {get;set;} public DateTime Fecha {get;set;} } }
