using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;

namespace FormApp.Models
{
	public class ViewLoginRegisterModel
	{
		public LoginModel LoginModel { get; set; }

		public RegisterModel RegisterModel { get; set; } 
	}

	public class UserModel {

		public int? UserID { get; set; }
		public string Username { get; set; }

		public string Email { get; set; }

	}

	public class LoginModel
	{
		[Required(ErrorMessage = "Username is required.")]
		public string Username { get; set; }


		[Required(ErrorMessage = "Password is required.")]
		public string Password { get; set; }



	}
	public class ForgotModel
	{
		public string Username { get; set; }

		public string Email { get; set; }

		public string Password { get; set; }
	}
	public class RegisterModel 
	{

		[Required(ErrorMessage = "Username is required.")]
		public string Username { get; set; }


		[Required(ErrorMessage = "Password is required.")]
		public string Password { get; set; } 

		[Required(ErrorMessage = "Confirm Password is required.")]
		[Compare("Password", ErrorMessage = "Password does not match.")]
		public string ConfirmPassword { get; set; } 


		[Required(ErrorMessage = "Email Address is required.")]
		public string Email { get; set; } = string.Empty;

		public int? OTP { get; set; }

	}
}
