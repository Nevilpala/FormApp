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
using FormApp.BAL;
using Humanizer;
using Unity.Interception.Utilities;

namespace FormApp.Controllers
{
	public class AccountController : Controller
	{
		#region Configuration
		private const string SessionKeyUsername = "Username";
		private const string SessionKeyUserID = "UserID";

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
				Console.WriteLine("OBJ : " + obj);

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
						Console.WriteLine("-> Register Record Successfully Inserted");

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
		private bool IsValidUsername(string username, bool sessionStore = false)
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
		public IActionResult ForgotPassword([Bind("Username")] IFormCollection fc)
		{

			Console.WriteLine("USER : " + fc["Username"]);
			if (string.IsNullOrEmpty(fc["Username"]))
			{

				ViewData["LoginPage"] = "forgot";
				ViewData["FormName"] = "Forgot Password";
				ModelState.AddModelError("Username", "Enter Username");

				return View("Login");
			}
			else
			{
				ViewData["LoginPage"] = "forgot";
				ViewData["FormName"] = "Forgot Password";
				if (IsValidUsername(fc["Username"], true))
				{

					bool IsSend = GenrateOTP(HttpContext.Session.GetString("UserEmail"));

					if (IsSend)
					{
						return View();

					}
					else
					{
						return View("Login");
					}
				}
				else
				{

					ModelState.AddModelError("Username", "Username does not exist");
					return View("Login");
				}
			}


		}

		[Route("ResetPassword")]

		public IActionResult OtpVerifyCheck(IFormCollection fc)
		{

			string otp = fc["1"] + fc["2"] + fc["3"] + fc["4"];
			if (HttpContext.Session.GetInt32("Otp").ToString() == otp)
			{
				ViewData["LoginPage"] = "ResetPassword";
				ViewData["FormName"] = "Reset Password";
				return View("Login");
			}
			else
			{
				ViewData["Error"] = "Please Enter Correct OTP";
			}
			return View("ForgotPassword");
		}

		public IActionResult NewPasswordSet(IFormCollection fc)
		{
			ViewData["LoginPage"] = "ResetPassword";
			ViewData["FormName"] = "Reset Password";
			string user = HttpContext.Session.GetString(SessionKeyUsername);
			string newPass = fc["Password"];
			string conPass = fc["ConfirmPassword"];
			Console.WriteLine(user);
			if (newPass == conPass && user != null)
			{
				string connectionStr = ConnectionString.GetConnectionString("sql");
				SqlConnection conn1 = new SqlConnection(connectionStr);
				conn1.Open();
				SqlCommand cmd = conn1.CreateCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "PR_Login_ResetPassword";
				cmd.Parameters.AddWithValue("@Username", user);
				cmd.Parameters.AddWithValue("@Password", newPass);
				int obj = Convert.ToInt32(cmd.ExecuteNonQuery());

				if (obj == 0)
				{
					ModelState.AddModelError("Password", "Change Password Failed");
					return View("Login");
				}
				else
				{
					Console.WriteLine("Correct");
					HttpContext.Session.Clear();
					return RedirectToAction("Login");

				}

			}
			else
			{
				Console.WriteLine("Incoorect");
				ModelState.AddModelError("Password", "Password Does Not Match");
			}
			return View("Login");
		}

		public bool GenrateOTP(string email, bool Resend = false)
		{

			DateTime dt = Convert.ToDateTime(HttpContext.Session.GetString("OtpDateTime"));
			TimeSpan ts = DateTime.Now - dt;
			Console.WriteLine("\n======================================================");
			Console.WriteLine("HttpClient OTP		: " + HttpContext.Session.GetInt32("Otp"));
			Console.WriteLine("HttpClient DATETIME	: " + HttpContext.Session.GetString("OtpDateTime"));
			Console.WriteLine("TotalSeconds			: " + ts.TotalSeconds);
			Console.WriteLine("Email				: " + HttpContext.Session.GetString("Email"));
			Console.WriteLine("======================================================\n");
			if ((HttpContext.Session.GetInt32("Otp") != null && ts.TotalSeconds > 60) && !Resend)
			{
				Console.WriteLine("-> OTP time Limit not Over");
				return true;
			}
			if (HttpContext.Session.GetInt32("Otp") == null || Resend || HttpContext.Session.GetString("Email") != email)
			{
				if (Resend)
				{
					HttpContext.Session.Clear();
				}
				bool f = false;
				email ??= "nevilpala5@gmail.com";
				Random rnd = new Random();
				int otp = rnd.Next(1000, 9999);
				HttpContext.Session.SetInt32("Otp", otp);
				string time = DateTime.Now.ToString();
				HttpContext.Session.SetString("OtpDateTime", time);
				HttpContext.Session.SetString("Email", email);
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



		#endregion

		#region ManageUser
		public IActionResult ManageUser()
		{
			return View();
		}
		#endregion
	}
}
