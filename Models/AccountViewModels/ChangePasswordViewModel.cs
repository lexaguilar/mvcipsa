using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace mvcIpsa.Models.AccountViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]        
        public string username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La {0} debe tener la menos {2} y un maximo de {1} caracteres de longitud.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La {0} debe tener la menos {2} y un maximo de {1} caracteres de longitud.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "La nueva contraseña y la contraseña de confirmación no son iguales.")]
        public string ConfirmPassword { get; set; }
       
    }
}
