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

		#region Registers
		public IActionResult Register()
		{
			return View();
		}
		#endregion

		#region RegisterSave
		public IActionResult RegisterSave(RegisterModel model)
		{
			bool otpError = false;
			DateTime dt = Convert.ToDateTime(HttpContext.Session.GetString("OtpDateTime"));
			TimeSpan ts = DateTime.Now - dt;
			double second = Convert.ToDouble(ts.TotalSeconds);

			if (IsValidUsername(model.Username))
			{
				ModelState.AddModelError("Username", "Username Already Exist");
			}
			if (model.OTP != null && model.OTP != HttpContext.Session.GetInt32("Otp"))
			{
				otpError = true;
				ModelState.AddModelError("OTP", "Otp  not Correct");
			}
			if (model.OTP != null && second > 60)
			{
				HttpContext.Session.Clear();
				otpError = true;
				ModelState.AddModelError("OTP", "OTP Expire, Genrate new OTP");
			}
			if (model.OTP != HttpContext.Session.GetInt32("Otp"))
			{
				otpError = true;
				ModelState.AddModelError("OTP", "Otp not Correct");
			}

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
				Console.WriteLine("OBJ : "+ obj);

				if (obj == 0)
				{
					ViewData["RegisterError"] = "Register Failed";
					//ViewData["ErrorMessage"] = "Register Failed";

					ModelState.AddModelError("", "Register User Failed");

				}
				else
				{
					if (IsValidUser(model.Username, model.Password))
					{
						Console.WriteLine("-> Record Successfully Inserted");


						return RedirectToAction("Index", "Home");
					}

				}

			}
			else
			{
				//ViewData["RegisterError"] = "Invalid Model";
				//ViewData["ErrorMessage"] = "Invalid Model";
			}
			if (otpError) model.OTP = null;
			ViewData["otpError"] = otpError;

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
		private bool IsValidUsername(string username,bool sessionStore = false)
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
				if (sessionStore)
				{
					while (rdr.Read())
					{ 
						HttpContext.Session.SetString(SessionKeyUsername, rdr["Username"].ToString());
						HttpContext.Session.SetString("UserEmail", rdr["Email"].ToString());
					}

				}

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
		public IActionResult ForgotPassword(LoginModel model)
		{
			 
			if (string.IsNullOrEmpty(model.Username))
			{

				ViewData["LoginPage"] = "forgot";
				ViewData["FormName"] = "Forgot Password"; 
				return View("Login",model);
			}
			else
			{
				ViewData["LoginPage"] = "forgot";
				ViewData["FormName"] = "Forgot Password";
				if (IsValidUsername(model.Username, true))
				{

					return View();
				}
				else
				{

					ModelState.AddModelError("Username", "Username does not exist");
					return View("Login", model);
				}
			}
			

			//ViewData["LoginPage"] = null;
			//return View("ForgotPassword");
 
		}

		public bool GenrateOTP(string email,bool Resend=false)
		{

			DateTime dt = Convert.ToDateTime(HttpContext.Session.GetString("OtpDateTime"));
			TimeSpan ts = DateTime.Now - dt;
			Console.WriteLine("\n======================================================");
			Console.WriteLine("HttpClient OTP		: " + HttpContext.Session.GetInt32("Otp"));
			Console.WriteLine("HttpClient DATETIME	: " + HttpContext.Session.GetString("OtpDateTime"));
			Console.WriteLine("TotalSeconds			: " + ts.TotalSeconds);

			Console.WriteLine("======================================================\n");
			if (HttpContext.Session.GetInt32("Otp") != null && ts.TotalSeconds > 60)
			{
				return true;
			}
			if (HttpContext.Session.GetInt32("Otp") == null || Resend)
			{
				HttpContext.Session.Clear();
				bool f = false;
				email ??= "nevilpala5@gmail.com";
				Random rnd = new Random();
				int otp = rnd.Next(1000, 9999);
				HttpContext.Session.SetInt32("Otp", otp);
				string time = DateTime.Now.ToString();
				HttpContext.Session.SetString("OtpDateTime", time);
				HttpContext.Session.SetString("Email", email);
				ViewData["msgotp"] = otp;
				Console.WriteLine("OTP : " + otp);
				string msg = "Your OTP from Address Book is " + otp;
				f = SendOTP(email, "Subjected to OTP", msg);
				if (f)
				{

					Console.WriteLine($"=====      OTP sent successfully to {email}      ======");
				}
				else
				{
					Console.WriteLine("otp not sent");
				}
				return f;

			}
			return false;
		}
		private bool SendOTP(string to, string subject, string body)
		{
			bool f = false;
			string pass = "offo jxyn bbac sfxf";

			string from = "addressbook.noreply@gmail.com";
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
