using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WEB_ProyectoFinal_Grupo3.Data;
using WEB_ProyectoFinal_Grupo3.Models;

namespace WEB_ProyectoFinal_Grupo3.Controllers
{
    public class PaquetesController : Controller
    {
        private readonly DBContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PaquetesController(DBContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Paquetes
        public async Task<IActionResult> Index()
        {
            var dBContext = _context.Paquetes.Include(p => p.Categoria);
            return View(await dBContext.ToListAsync());
        }

        // GET: Paquetes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Paquetes == null)
            {
                return NotFound();
            }

            var paquete = await _context.Paquetes
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paquete == null)
            {
                return NotFound();
            }

            return View(paquete);
        }

        // GET: Paquetes/Create
        public IActionResult Create()
        {
            ViewData["IdCategoria"] = new SelectList(_context.Categorias, "Id", "Nombre");
            return View();
        }

        // POST: Paquetes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion,Costo,Duracion,Estado,ImagenArchivo,IdCategoria")] Paquete paquete)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si la carpeta de imágenes de paquetes existe, de lo contrario, crearla
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "img/Paquetes");
                    if (!Directory.Exists(imagePath))
                    {
                        Directory.CreateDirectory(imagePath);
                    }

                    // Lógica para guardar la imagen del paquete, si es necesario
                    if (paquete.ImagenArchivo != null)
                    {
                        // Obtener la extensión del archivo
                        string extension = Path.GetExtension(paquete.ImagenArchivo.FileName);

                        // Verificar si la extensión es jpg, png o jpeg
                        if (extension != ".jpg" && extension != ".png" && extension != ".jpeg")
                        {
                            TempData["Error"] = "Solo se permiten imágenes con extensiones jpg, png o jpeg.";
                            ViewData["IdCategoria"] = new SelectList(_context.Categorias, "Id", "Descripcion", paquete.IdCategoria);
                            return View(paquete);
                        }

                        // Obtener el nombre de archivo único
                        string uniqueFileName = Guid.NewGuid().ToString() + extension;
                        string filePath = Path.Combine(imagePath, uniqueFileName);

                        try
                        {
                            // Guardar el archivo en el servidor
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await paquete.ImagenArchivo.CopyToAsync(stream);
                            }
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = $"No se pudo guardar la imagen del paquete por: {ex.Message}";
                            ViewData["IdCategoria"] = new SelectList(_context.Categorias, "Id", "Nombre", paquete.IdCategoria);
                            return View(paquete);
                        }

                        // Asignar la ruta de la imagen al objeto del paquete
                        paquete.ImagenPaquetePath = "/img/Paquetes/" + uniqueFileName; // Ruta relativa al directorio web
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"No se pudo crear el paquete por: {ex.Message}";
                    ViewData["IdCategoria"] = new SelectList(_context.Categorias, "Id", "Descripcion", paquete.IdCategoria);
                    return View(paquete);
                }

                // Agregar el paquete al contexto y guardar los cambios
                paquete.Estado = EstadosPaquetes.Disponible;
                _context.Add(paquete);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Paquete: {paquete.Nombre}, creado exitosamente!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdCategoria"] = new SelectList(_context.Categorias, "Id", "Descripcion", paquete.IdCategoria);
            return View(paquete);
        }


        // GET: Paquetes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Paquetes == null)
            {
                return NotFound();
            }

            var paquete = await _context.Paquetes.FindAsync(id);
            if (paquete == null)
            {
                return NotFound();
            }
            ViewData["IdCategoria"] = new SelectList(_context.Categorias, "Id", "Nombre", paquete.IdCategoria);
            return View(paquete);
        }

        // POST: Paquetes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion,Costo,Duracion,Estado,ImagenArchivo,ImagenPaquetePath,IdCategoria")] Paquete paquete)
        {
            if (id != paquete.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si la carpeta de imágenes de paquetes existe, de lo contrario, crearla
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "img/Paquetes");
                    if (!Directory.Exists(imagePath))
                    {
                        Directory.CreateDirectory(imagePath);
                    }

                    // Si hay una imagen actualmente asociada al paquete, eliminarla del servidor
                    if (!string.IsNullOrEmpty(paquete.ImagenPaquetePath))
                    {
                        string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, paquete.ImagenPaquetePath.TrimStart('/'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Lógica para guardar la nueva imagen del paquete, si se proporciona
                    if (paquete.ImagenArchivo != null)
                    {
                        // Obtener la extensión del archivo
                        string extension = Path.GetExtension(paquete.ImagenArchivo.FileName);

                        // Verificar si la extensión es jpg, png o jpeg
                        if (extension != ".jpg" && extension != ".png" && extension != ".jpeg")
                        {
                            TempData["Error"] = "Solo se permiten imágenes con extensiones jpg, png o jpeg.";
                            ViewData["IdCategoria"] = new SelectList(_context.Categorias, "Id", "Descripcion", paquete.IdCategoria);
                            return View(paquete);
                        }

                        // Obtener el nombre de archivo único
                        string uniqueFileName = Guid.NewGuid().ToString() + extension;
                        string filePath = Path.Combine(imagePath, uniqueFileName);

                        // Guardar el archivo en el servidor
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await paquete.ImagenArchivo.CopyToAsync(stream);
                        }

                        // Asignar la ruta de la nueva imagen al objeto del paquete
                        paquete.ImagenPaquetePath = "/img/Paquetes/" + uniqueFileName; // Ruta relativa al directorio web
                    }

                    // Actualizar el paquete en el contexto y guardar los cambios
                    _context.Update(paquete);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Paquete: {paquete.Nombre}, actualizado exitosamente!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaqueteExists(paquete.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"No se pudo editar el paquete: {ex.Message}";
                    ViewData["IdCategoria"] = new SelectList(_context.Categorias, "Id", "Descripcion", paquete.IdCategoria);
                    return View(paquete);
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCategoria"] = new SelectList(_context.Categorias, "Id", "Nombre", paquete.IdCategoria);
            return View(paquete);
        }

        // GET: Paquetes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Paquetes == null)
            {
                return NotFound();
            }

            var paquete = await _context.Paquetes
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paquete == null)
            {
                return NotFound();
            }

            return View(paquete);
        }

        // POST: Paquetes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Paquetes == null)
            {
                return Problem("Entity set 'DBContext.Paquetes'  is null.");
            }


 
            var paquete = await _context.Paquetes.FindAsync(id);
            if (paquete != null)
            {

                try 
                {
                    // Verificar si la imagen existe y eliminarla si es así
                    if (!string.IsNullOrEmpty(paquete.ImagenPaquetePath))
                    {
                        string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, paquete.ImagenPaquetePath.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                }
                catch (Exception ex)
                {

                    TempData["Error"] = $"No se pudo eliminar la imagen del paquete {paquete.Nombre} por: {ex.Message}!";
                    return View(paquete);
                }
             

                _context.Paquetes.Remove(paquete);
                TempData["Sucess"] = $"El paquete {paquete.Nombre} ha sido elminado correctamente!";

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = $"Error con el paquete!";
            return RedirectToAction(nameof(Index));
        }

        private bool PaqueteExists(int id)
        {
          return (_context.Paquetes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
