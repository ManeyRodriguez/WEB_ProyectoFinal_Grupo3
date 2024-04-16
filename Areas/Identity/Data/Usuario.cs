using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WEB_ProyectoFinal_Grupo3.Models;

namespace WEB_ProyectoFinal_Grupo3.Areas.Identity.Data;


// Add profile data for application users by adding properties to the Usuario class
public class Usuario : IdentityUser
{

    [Required]
    [ForeignKey("Membresia")]
    public required TiposMembresias IdMembresia { get; set; } = TiposMembresias.Estandar;


    [Required(ErrorMessage = "El campo de Nombre es requerido.")]
    [DisplayName("Nombres")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Debe tener entre 3 y 100 caracteres.")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "El campo de Apellidos es requerido.")]
    [DisplayName("Apellidos")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Debe tener entre 3 y 100 caracteres.")]
    public required string LastName { get; set; }


    [Required(ErrorMessage = "El campo de la cédula es obligatorio")]
    [DisplayName("Cedula")]
    [RegularExpression(@"^\d{3}-\d{7}-\d{1}$", ErrorMessage = "El formato debe ser 000-0000000-0")]
    [StringLength(13, MinimumLength = 12, ErrorMessage = "Debe tener exactamente 12 caracteres numericos")]
    [Display(Name = "Cédula")]
    [ProtectedPersonalData]
    public required string Cedula { get; set; }

    [Required(ErrorMessage = "El campo Teléfono es requerido.")]
    [RegularExpression(@"^\d{3}-\d{3}-\d{4}$", ErrorMessage = "El formato debe ser 000-000-0000")]
    [StringLength(13, MinimumLength = 12, ErrorMessage = "Debe tener exactamente 12 caracteres numericos")]
    public override string? PhoneNumber { get; set; }


    [Required(ErrorMessage = "El campo Email es requerido.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Error en el formato del correo.")]
    public override string? Email { get; set; }

    [Display(Name = "Nombre Completo")]
    public string FullName
    {
        get
        {

            // Dividir el texto en palabras usando espacios en blanco como delimitador
            string[] nombres = (FirstName + " " + LastName).Split(' ');

            // Crear un objeto TextInfo para manejar las reglas de capitalización
            TextInfo ti = new CultureInfo("es-ES", false).TextInfo;

            // Capitalizar la primera letra de cada palabra
            for (int i = 0; i < nombres.Length; i++)
            {
                nombres[i] = ti.ToTitleCase(nombres[i]);
            }

            // Unir las palabras nuevamente en un solo texto
            string resultado = string.Join(" ", nombres);

            return resultado;

        }
    }

}


