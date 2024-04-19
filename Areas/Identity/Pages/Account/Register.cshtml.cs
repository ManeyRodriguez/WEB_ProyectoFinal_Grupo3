// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using WEB_ProyectoFinal_Grupo3.Areas.Identity.Data;
using WEB_ProyectoFinal_Grupo3.Models;

namespace WEB_ProyectoFinal_Grupo3.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UserManager<Usuario> _userManager;
        private readonly IUserStore<Usuario> _userStore;
        private readonly IUserEmailStore<Usuario> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<Usuario> userManager,
            IUserStore<Usuario> userStore,
            SignInManager<Usuario> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
           
            [Required(ErrorMessage = "El campo de Nombre es requerido.")]
            [DisplayName("Nombres")]
            [StringLength(100, MinimumLength = 3, ErrorMessage = "Debe tener entre 3 y 100 caracteres.")]
            [Display(Name = "Nombres")]
            public string FirstName { get; set; }


            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "El campo de Apellidos es requerido.")]
            [DisplayName("Nombres")]
            [StringLength(100, MinimumLength = 3, ErrorMessage = "Debe tener entre 3 y 100 caracteres.")]
            [Display(Name = "Apellidos")]
            public string LastName { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>       
            [Required(ErrorMessage = "El campo de la cédula es obligatorio")]
            [DisplayName("Cedula")]
            [RegularExpression(@"^\d{3}-\d{7}-\d{1}$", ErrorMessage = "El formato debe ser 000-0000000-0")]
            [StringLength(13, MinimumLength = 12, ErrorMessage = "Debe tener exactamente 12 caracteres numericos")]
            [Display(Name = "Cédula")]
            [ProtectedPersonalData]
            public string Cedula { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>       
            [Required(ErrorMessage = "El campo Teléfono es requerido.")]
            [RegularExpression(@"^\d{3}-\d{3}-\d{4}$", ErrorMessage = "El formato debe ser 000-000-0000")]
            [StringLength(12, MinimumLength = 11, ErrorMessage = "Debe tener exactamente 12 caracteres numericos")]
            public string PhoneNumber { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y un máximo de {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
            public string ConfirmPassword { get; set; }


            [Required(ErrorMessage = "Seleccione una membresia.")]
            [Display(Name = "Membresia")]
            [Range(1, 3, ErrorMessage = "Seleccione una membresia.")]
            public int IdMembresia { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

             
                if (Input.IdMembresia == 1)
                {
                    user.IdMembresia = TiposMembresias.Estandar;
                }
                else if (Input.IdMembresia == 2)
                {
                    user.IdMembresia = TiposMembresias.Premium;
                }
                else if (Input.IdMembresia == 3)
                {
                    user.IdMembresia = TiposMembresias.VIP;
                }

               
                user.Cedula = Input.Cedula;
                user.PhoneNumber = Input.PhoneNumber;
                user.FirstName = Input.FirstName.ToLower();
                user.LastName = Input.LastName.ToLower();
                user.EmailConfirmed = true;

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);




             
                if (result.Succeeded)
                {
                    var signInResult = await _signInManager.PasswordSignInAsync(user.Email, Input.Password, isPersistent: false, lockoutOnFailure: false);
                    _logger.LogInformation("User created a new account with password.");
                    // Redirigir al usuario a la página de inicio
                    return RedirectToAction("Index", "Home");
                  
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private Usuario CreateUser()
        {
            try
            {
                return Activator.CreateInstance<Usuario>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(Usuario)}'. " +
                    $"Ensure that '{nameof(Usuario)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<Usuario> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<Usuario>)_userStore;
        }
    }
}
