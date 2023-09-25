using FormApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace FormApp.Controllers
{
	public class AccountController : Controller
	{

		private const string SessionKeyUsername = "Username";
		private const string SessionKeyUserID = "UserID";
		//private const string LoginPage = "login1";
		private const string LoginPage = "Login";

		private readonly IConfiguration ConnectionString;

		public AccountController(IConfiguration _configuration)
		{
			ConnectionString = _configuration;
		}

		[Route("/Login")]
		public IActionResult Login()
		{
			var model = new ViewLoginRegisterModel();
			model.LoginModel = new LoginModel();
			model.RegisterModel = new RegisterModel();
			return View(LoginPage);
		}

		[Route("/Login")]
		[HttpPost]
		public IActionResult LoginSave(LoginModel model)
		{


			if (ModelState.IsValid)
			{

				if (IsValidUser(model.Username, model.Password))
				{
					return RedirectToAction("Index", "Home");
				}
				else
				{
					ViewData["LoginError"] = "Invalid Username or Password";
					ViewData["ErrorMessage"] = "Invalid Username or Password";

				}
				ModelState.AddModelError("", "Invalid Username or Password");
			}
			else
			{
				ViewData["LoginError"] = "Invalid Model";
				ViewData["ErrorMessage"] = "Invalid Model";


			}
			//var vmodel = new ViewLoginRegisterModel { LoginModel = model };
			//return View("Login", LoginPage == "login1" ? vmodel : model);
			return View("Login", model);
		}

		public IActionResult Register()
		{
			//	//var vmodel = new ViewLoginRegisterModel();
			//	//vmodel.LoginModel = new LoginModel();
			//	//vmodel.RegisterModel = new RegisterModel();
			//	//return View(LoginPage);
			return View();
		}
		public IActionResult RegisterSave(RegisterModel model)
		{
			
			if (IsValidUsername(model.Username))
			{
				ModelState.AddModelError("Username", "Username Already Exist");
			}
			else
			{
				if (ModelState.IsValid)
				{
					string connectionStr = ConnectionString.GetConnectionString("sql");
					SqlConnection conn1 = new SqlConnection(connectionStr);
					conn1.Open();
					SqlCommand cmd = conn1.CreateCommand();
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "PR_Login_Insert";
					cmd.Parameters.AddWithValue("@Username", model.Username);
					cmd.Parameters.AddWithValue("@Password", model.Password);
					cmd.Parameters.AddWithValue("@Email", model.Email);
					int obj = Convert.ToInt32(cmd.ExecuteNonQuery());
					if (obj == 0)
					{
						ViewData["RegisterError"] = "Register Failed";
						ViewData["ErrorMessage"] = "Register Failed";

						ModelState.AddModelError("", "Register User Failed");

					}
					else
					{
						if (IsValidUser(model.Username, model.Password))
						{
							return RedirectToAction("Index", "Home");

						}

					}

				}
				else
				{
					ViewData["RegisterError"] = "Invalid Model";
					ViewData["ErrorMessage"] = "Invalid Model";
				}
			}
			//var vmodel = new ViewLoginRegisterModel { RegisterModel = model };
			//return View( LoginPage,LoginPage == "login1" ? vmodel : model);
			return View("Register",model);

		}
		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Login", "Account");
		}

		public IActionResult Guest()
		{
			HttpContext.Session.Clear();
			HttpContext.Session.SetInt32("Guest", 1);
			return RedirectToAction("Index", "Home");
		}

		private bool IsValidUsername(string username)
		{
			string connectionStr = ConnectionString.GetConnectionString("sql");
			SqlConnection conn1 = new SqlConnection(connectionStr);
			conn1.Open();
			SqlCommand cmd = conn1.CreateCommand();
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "PR_Login_UsernameCheck";
			cmd.Parameters.AddWithValue("@Username", username);
			SqlDataReader rdr = cmd.ExecuteReader();
			if (rdr.HasRows)
			{

				return true;
			}
			conn1.Close();
			return false;



		}

		private bool IsValidUser(string username, string password)
		{
			string connectionStr = ConnectionString.GetConnectionString("sql");
			SqlConnection conn1 = new SqlConnection(connectionStr);
			conn1.Open();
			SqlCommand cmd = conn1.CreateCommand();
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "PR_Login_Check";
			cmd.Parameters.AddWithValue("@Username", username);
			cmd.Parameters.AddWithValue("@Password", password);
			SqlDataReader rdr = cmd.ExecuteReader();
			//bool check = Convert.ToBoolean(rdr);
			//Console.WriteLine("Login : "  + res + " " );

			if (rdr.HasRows)
			{
				while (rdr.Read())
				{
					HttpContext.Session.SetInt32(SessionKeyUserID, Convert.ToInt32(rdr["UserID"]));
					HttpContext.Session.SetString(SessionKeyUsername, rdr["Username"].ToString());
				}
				conn1.Close();
				return true;

			}
			conn1.Close();
			return false;

		}

	}
}
