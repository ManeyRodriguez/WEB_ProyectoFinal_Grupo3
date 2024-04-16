using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;


namespace WEB_ProyectoFinal_Grupo3.Models
{
    public class Categoria
    {
        [Key]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres.")]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [Display(Name = "Descripcion")]
        public required string Descripcion { get; set; }

        [NotMapped] // Esta propiedad no se mapeará a la base de datos
        [Display(Name = "Imagen de la Categoría")]
        public IFormFile? ImagenArchivo { get; set; }

        [Display(Name = "Ruta Imagen")]
        public string? ImagenCategoriaPath { get; set; }
    }
}