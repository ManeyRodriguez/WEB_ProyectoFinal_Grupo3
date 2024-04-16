using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WEB_ProyectoFinal_Grupo3.Data;
using WEB_ProyectoFinal_Grupo3.Models;

namespace WEB_ProyectoFinal_Grupo3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly DBContext _context;


   
        public HomeController(ILogger<HomeController> logger, DBContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: Categorias
        public async Task<IActionResult> Index()
        {

            var categorias = await _context.Categorias.ToListAsync(); // Obtener tus categorías de alguna manera
            var paquetes = await _context.Paquetes.ToListAsync(); // Obtener tus paquetes de alguna manera

            var viewModel = new CategoriaPaqueteViewModel
            {
                Categorias =  categorias,
                Paquetes =  paquetes
            };

            return View(viewModel);



          
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}