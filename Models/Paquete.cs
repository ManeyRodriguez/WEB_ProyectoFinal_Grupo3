using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEB_ProyectoFinal_Grupo3.Models
{
    public class Paquete
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres.")]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public required string Descripcion { get; set; }

        [Required(ErrorMessage = "El costo es obligatorio.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "El costo debe tener hasta dos decimales.")]
        [Range(0, 1000000, ErrorMessage = "El costo debe estar entre 0 y 1,000,000.")]
        public required decimal Costo { get; set; }

        [Required(ErrorMessage = "La duración es obligatoria.")]
        [Range(1, 44640, ErrorMessage = "La duración debe estar entre 1 y 44,640 minutos (1 mes).")]
        public int Duracion { get; set; }


        [Required]
        public required EstadosPaquetes Estado { get; set; } = EstadosPaquetes.Disponible;


        [NotMapped] // Esta propiedad no se mapeará a la base de datos
        [Display(Name = "Imagen del Paquete")]
        public IFormFile? ImagenArchivo { get; set; }

        [Display(Name = "Ruta Imagen")]
        public string? ImagenPaquetePath { get; set; }

        [Required]
        [ForeignKey("Categoria")]
        public required int IdCategoria { get; set; }



        public Categoria? Categoria { get; set; 
}

    }
}
