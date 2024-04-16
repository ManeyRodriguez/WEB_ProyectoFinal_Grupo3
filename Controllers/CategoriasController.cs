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
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.StaticFiles;
using NuGet.Packaging.Signing;
using System.Drawing;
using Microsoft.EntityFrameworkCore.ChangeTracking;


namespace WEB_ProyectoFinal_Grupo3.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly DBContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;



        public CategoriasController(DBContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Categorias
        public async Task<IActionResult> Index()
        {
              return _context.Categorias != null ? 
                          View(await _context.Categorias.ToListAsync()) :
                          Problem("Entity set 'DBContext.Categorias'  is null.");
        }

        // GET: Categorias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Categorias == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        // GET: Categorias/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categorias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion,ImagenArchivo")] Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si la carpeta de imágenes de categorías existe, de lo contrario, crearla
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "img/Categorias");
                    if (!Directory.Exists(imagePath))
                    {
                        Directory.CreateDirectory(imagePath);
                    }

                    // Lógica para guardar la imagen de la categoría, si es necesario
                    if (categoria.ImagenArchivo != null)
                    {
                        // Obtener la extensión del archivo
                        string extension = Path.GetExtension(categoria.ImagenArchivo.FileName);

                        // Verificar si la extensión es jpg, png o jpeg
                        if (extension != ".jpg" && extension != ".png" && extension != ".jpeg")
                        {
                            TempData["Error"] = "Solo se permiten imágenes con extensiones jpg, png o jpeg.";
                            return View(categoria);
                        }

                        // Obtener el nombre de archivo único
                        string uniqueFileName = Guid.NewGuid().ToString() + extension;
                        string filePath = Path.Combine(imagePath, uniqueFileName);

                        try
                        {
                            // Guardar el archivo en el servidor


                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await categoria.ImagenArchivo.CopyToAsync(stream);
                            }
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = $"No se pudo guardar la imagen de categoría por: {ex.Message}";
                            return View(categoria);
                        }

                        // Asignar la ruta de la imagen al objeto de la categoría
                        categoria.ImagenCategoriaPath = "/img/Categorias/" + uniqueFileName; // Ruta relativa al directorio web
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"No se pudo crear la categoría por: {ex.Message}";
                    return View(categoria);
                }

                // Agregar la categoría al contexto y guardar los cambios
                _context.Add(categoria);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Categoría: {categoria.Nombre}, creada exitosamente!";
                return RedirectToAction(nameof(Index));
            }

            return View(categoria);
        }

     
        // GET: Categorias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }

        // POST: Categorias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion,ImagenArchivo,ImagenCategoriaPath")] Categoria categoria)
        {
            if (id != categoria.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si la carpeta de imágenes de categorías existe, de lo contrario, crearla
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "img/Categorias");
                    if (!Directory.Exists(imagePath))
                    {
                        Directory.CreateDirectory(imagePath);
                    }

                    // Si hay una imagen actualmente asociada a la categoría, eliminarla del servidor
                    if (!string.IsNullOrEmpty(categoria.ImagenCategoriaPath))
                    {
                        string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, categoria.ImagenCategoriaPath.TrimStart('/'));

                       

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Lógica para guardar la nueva imagen de la categoría, si se proporciona
                    if (categoria.ImagenArchivo != null)
                    {
                        // Obtener la extensión del archivo
                        string extension = Path.GetExtension(categoria.ImagenArchivo.FileName);

                        // Verificar si la extensión es jpg, png o jpeg
                        if (extension != ".jpg" && extension != ".png" && extension != ".jpeg")
                        {
                            TempData["Error"] = "Solo se permiten imágenes con extensiones jpg, png o jpeg.";
                            return View(categoria);
                        }

                        // Obtener el nombre de archivo único
                        string uniqueFileName = Guid.NewGuid().ToString() + extension;
                        string filePath = Path.Combine(imagePath, uniqueFileName);

                        // Guardar el archivo en el servidor
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await categoria.ImagenArchivo.CopyToAsync(stream);
                        }

                        // Asignar la ruta de la nueva imagen al objeto de la categoría
                        categoria.ImagenCategoriaPath = "/img/Categorias/" + uniqueFileName; // Ruta relativa al directorio web
                    }

                    // Actualizar la categoría en el contexto y guardar los cambios
                    _context.Update(categoria);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Categoría: {categoria.Nombre}, actualizada exitosamente!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaExists(categoria.Id))
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
                    TempData["Error"] = $"No se pudo editar la categoría: {ex.Message}";
                    return View(categoria);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }


        // GET: Categorias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categorias == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        // POST: Categorias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categorias == null)
            {
                return Problem("Entity set 'DBContext.Categorias'  is null.");
            }

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }

            // Verificar si la imagen existe y eliminarla si es así
            if (!string.IsNullOrEmpty(categoria.ImagenCategoriaPath))
            {
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, categoria.ImagenCategoriaPath.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool CategoriaExists(int id)
        {
          return (_context.Categorias?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
