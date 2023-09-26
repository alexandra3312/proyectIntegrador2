using Microsoft.AspNetCore.Mvc;

using SistemaVenta.BLL.Interfaces;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using Microsoft.AspNetCore.Authorization;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class DashBoardController : Controller
    {
        private readonly IDashBoardService _dashboardServico;
        public DashBoardController(IDashBoardService dashBoardServico) {
         _dashboardServico = dashBoardServico;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task <IActionResult> ObtenerResumen()
        {
            GenericResponse <VMDashBoard> gResponse = new GenericResponse<VMDashBoard> ();
            try
            {
                VMDashBoard vmDashBoad = new VMDashBoard ();

                vmDashBoad.TotalVentas = await _dashboardServico.TotalVentasUltimaSemana();
                vmDashBoad.TotalIngesos = await _dashboardServico.TotalIngresosUltimaSemana();
                vmDashBoad.TotalProductos = await _dashboardServico.TotalProductos();
                vmDashBoad.TotalCategorias = await _dashboardServico.TotalCategorias();

                List<VMVentaSemana>listaVentasSemana = new List<VMVentaSemana> ();
                List<VMProductosSemana> listaProductosSemana = new List<VMProductosSemana>();

                foreach (KeyValuePair<string, int> item in await _dashboardServico.VentasUltimaSemana())
                {
                    listaVentasSemana.Add(new VMVentaSemana()
                    {
                        Fecha = item.Key, 
                    Total = item.Value,
                    });
                }

                foreach (KeyValuePair<string, int> item in await _dashboardServico.ProductosTopUltimaSemana())
                {
                    listaProductosSemana.Add(new VMProductosSemana()
                    {
                        Producto = item.Key,
                        Cantidad = item.Value,
                    });
                }

                vmDashBoad.VentasUltimaSemana = listaVentasSemana;
                vmDashBoad.ProductosTopUltimaSemana= listaProductosSemana;

                gResponse.Estado = true;
                gResponse.Objeto = vmDashBoad;

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }


            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

    }
}
