using DocumentFormat.OpenXml.Office2013.Excel;
using Microsoft.AspNetCore.Mvc.Rendering;
using SockenJac.Models;
using SockenJac.ViewModels;


namespace SockenJac.ViewModels
{
    public class paginador
    {
        public int paginaActual { get; set; }
        public int cantRegistros { get; set; }
        public int cantRegistrosPagina { get; set; }

        public int cantPaginas => (int)Math.Ceiling((decimal)cantRegistros / cantRegistrosPagina);

        public Dictionary<string, string> filtros { get; set; } = new Dictionary<string, string>();

    }
}
