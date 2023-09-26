using FormApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace FormApp.Controllers
{
	public class AccountController : Controller
	{
		#region Configuration
		private const string SessionKeyUsername = "Username";
		private const string SessionKeyUserID = "UserID";
		//private const string LoginPage = "login1";
		private const string LoginPage = "Login";

		private readonly IConfiguration ConnectionString;

		public AccountController(IConfiguration _configuration)
		{
			ConnectionString = _configuration;
		}
		#endregion

		#region Login
		[Route("/Login")]
		public IActionResult Login()
		{
			var model = new ViewLoginRegisterModel();
			model.LoginModel = new LoginModel();
			model.RegisterModel = new RegisterModel();
			return View(LoginPage);
		}
		#endregion

		#region LoginSave

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
		#endregion

		#region Register
		public IActionResult Register()
		{
			//	//var vmodel = new ViewLoginRegisterModel();
			//	//vmodel.LoginModel = new LoginModel();
			//	//vmodel.RegisterModel = new RegisterModel();
			//	//return View(LoginPage);

			HttpContext.Session.SetString("OtpDateTime", DateTime.Now.ToString());

			DateTime dt = Convert.ToDateTime(HttpContext.Session.GetString("OtpDateTime"));
			TimeSpan ts = DateTime.Now - dt;
			Console.WriteLine(ts.TotalSeconds > 1);
			return View();
		}
		#endregion

		#region RegisterSave
		public IActionResult RegisterSave(RegisterModel model)
		{

			if (IsValidUsername(model.Username))
			{
				ModelState.AddModelError("Username", "Username Already Exist");
			}
			else
			{
				//UserModel userModel = new UserModel();
				//userModel.Username = model.Username;
				//userModel.Email = model.Email;
				//RedirectToAction("ForgotPassword", model);
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
						Console.WriteLine(obj);
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
			return View("Register", model);

		}
		#endregion

		#region Logout
		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Login", "Account");
		}
		#endregion

		#region Guest
		public IActionResult Guest()
		{
			HttpContext.Session.Clear();
			HttpContext.Session.SetInt32("Guest", 1);
			return RedirectToAction("Index", "Home");
		}
		#endregion

		#region IsValidUsername
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
		#endregion

		#region IsValidUser
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
					HttpContext.Session.SetString("UserEmail", rdr["Email"].ToString());
				}
				conn1.Close();
				return true;

			}
			conn1.Close();
			return false;

		}
		#endregion

		#region ForgotPassword
		public IActionResult ForgotPassword(UserModel model)
		{
			
			return View("ForgotPassword");
		}
		
		public void GenrateOTP(string email)
		{
			DateTime dt = Convert.ToDateTime(HttpContext.Session.GetString("OtpDateTime"));
			TimeSpan ts = DateTime.Now - dt;
			Console.WriteLine(ts.TotalSeconds > 1);

			try{
				MailAddress m = new MailAddress(email);
				Console.WriteLine("Valid Mail");


			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return;
			if (HttpContext.Session.GetInt32("Otp") != null) {
				bool f = false;
				email ??= "nevilpala5@gmail.com";
				Random rnd = new Random();
				int otp = rnd.Next(1000, 9999);
				HttpContext.Session.SetInt32("Otp", otp);
				string time = DateTime.Now.ToString();
				HttpContext.Session.SetString("OtpDateTime", time);
				ViewData["msgotp"] = otp;
				Console.WriteLine("OTP : " + otp);
				string msg = "Your OTP from Address Book is " + otp;
				f = SendOTP(email, "Subjected to OTP", msg);
				if (f)
				{
					Console.WriteLine($"=====      otp sent successfully to {email}      ======");
				}
				else
				{
					Console.WriteLine("otp not sent");
				}
			}
			
		}
		private bool SendOTP(string to, string subject, string body)
		{
			bool f = false;
			string from = "addressbook.noreply@gmail.com";
			string pass = "offo jxyn bbac sfxf";
			try
			{

				using (MailMessage mm = new MailMessage(from, to))
				{
					mm.Subject = subject;
					mm.Body = body;

					mm.IsBodyHtml = true;
					using (SmtpClient smtp = new SmtpClient())
					{
						smtp.Host = "smtp.gmail.com";
						smtp.EnableSsl = true;
						NetworkCredential NetworkCred = new NetworkCredential(from, pass);
						smtp.UseDefaultCredentials = false;
						smtp.Credentials = NetworkCred;
						smtp.Port = 587;
						smtp.Send(mm);
						TempData["mailmessege"] = "Successfully mail sended to " + to;
					}
				}


				f = true;
			}
			catch (Exception ex)
			{
				f = false;
				//Console.WriteLine(ex.ToString());
				Console.WriteLine(ex.Message);
			}
			return f;
		}

		private bool OtpCheck()
		{
			bool check = true;

			return check;
		}

		#endregion

		#region ManageUser
		public IActionResult ManageUser()
		{
			return View();
		}
		#endregion
	}
}
