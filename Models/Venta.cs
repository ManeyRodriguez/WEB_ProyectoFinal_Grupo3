using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEB_ProyectoFinal_Grupo3.Models
{
    public class Venta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required DateTime Fecha { get; set; }

        [Required]
        public required string IdCliente { get; set; }

        [Required]
        [ForeignKey("Paquete")]
        public required int IdPaquete { get; set; }

        [Required]
        public required decimal Total { get; set; }

        public Paquete? Paquete { get; set; }
    }



}
