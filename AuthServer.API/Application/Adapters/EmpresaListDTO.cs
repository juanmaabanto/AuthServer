namespace Expertec.Sigeco.AuthServer.API.Application.Adapters
{
    public class EmpresaListDTO
    {
        public int EmpresaId { get; set; }
        public string Codigo { get; set; }
        public string Ruc { get; set; }
        public string RazonSocial { get; set; }
        public bool Principal { get; set; }
    }
}