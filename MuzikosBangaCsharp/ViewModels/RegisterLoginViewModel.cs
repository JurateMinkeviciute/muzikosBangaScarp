using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MuzikosBangaCsharp.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Privalomas laukas Naudotojo vardas")]
        [Display(Name = "Naudotojo vardas")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Privalomas laukas Slaptažodis vardas")]
        [DataType(DataType.Password)]
        [Display(Name = "Slaptažodis")]
        public string Password { get; set; }
    }
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Privalomas laukas Naudotojo vardas")]
        [Display(Name = "Nadotojo vardas")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Privalomas laukas Vardas")]
        [Display(Name = "Vardas")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Privalomas laukas Pavardė")]
        [Display(Name = "Pavardė")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Privalomas laukas Naudotojo El. paštas. ")]
        [Display(Name = "El. paštas")]
        [EmailAddress(ErrorMessage = "El. paštas netitinka formato")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Privalomas laukas pakartotinas El. paštas. ")]
        [Display(Name = "Pakartoti el. paštą. ")]
        [EmailAddress(ErrorMessage = "El. paštas netitinka formato")]
        [Compare("Email", ErrorMessage = "El. paštas and pakartotinas El. paštas nesutampa. ")]
        public string EmailConfirm { get; set; }

        [Required(ErrorMessage = "Privalomas laukas Slaptažodis")]
        [Display(Name = "Slaptažodis")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Privalomas laukas pakartotinas Slaptažodis")]
        [DataType(DataType.Password)]
        [Display(Name = "Pakartoti slaptažodį")]
        [Compare("Password", ErrorMessage = "Slaptažodis ir partotinas slaptažodis nesutampa.")]
        public string PasswordConfirm { get; set; }
    }
}