using FormApp.Areas.MST_Student.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using FormApp.Areas.LOC_State.Models;
using FormApp.Areas.LOC_City.Models;
using FormApp.Areas.MST_Branch.Models;
using FormApp.Areas.LOC_Country.Models;
using FormApp.BAL;

namespace FormApp.Areas.MST_Student.Controllers
{
	[CheckAccess]
	[Area("MST_Student")]
	[Route("MST_Student/[Controller]/[Action]")]

	public class MST_StudentController : Controller
	{


		#region Configuration
		private readonly IConfiguration Configuration;

		public MST_StudentController(IConfiguration _Configuration)
		{
			Configuration = _Configuration;
		}
		#endregion

		#region SelectALL
		public IActionResult Index()
		{
			Country_DD();
			City_DD();
			State_DD();
			Branch_DD();
			string connectionStr = Configuration.GetConnectionString("sql");
			DataTable dt = new DataTable();
			SqlConnection conn = new SqlConnection(connectionStr);
			conn.Open();
			SqlCommand objCmd = conn.CreateCommand();
			objCmd.CommandType = CommandType.StoredProcedure;
			objCmd.CommandText = "PR_Student_SelectAll";
			objCmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetInt32("UserID"));
			SqlDataReader objSDR = objCmd.ExecuteReader();
			dt.Load(objSDR);
			conn.Close();
			return View("Index", dt);
		}
		#endregion

		#region Search
		public IActionResult SearchFilter(string? StudentName, string? Email, int? Age, DateTime? BirthDate, int? CityID, int? StateID, int? CountryID, int? BranchID, string? MobileNoStudent, string? Gender,DateTime? JoinEndDate, DateTime? JoinStartDate, bool? IsActive)
		{
			City_DD();
			Branch_DD();
			Country_DD();
			State_DD();
			string connectionStr = Configuration.GetConnectionString("sql");
			DataTable dt = new DataTable();
			SqlConnection conn = new SqlConnection(connectionStr);
			conn.Open();
			SqlCommand objCmd = conn.CreateCommand();
			objCmd.CommandType = CommandType.StoredProcedure;
			objCmd.CommandText = "PR_Student_Search";
			objCmd.Parameters.AddWithValue("@StudentName", StudentName);
			objCmd.Parameters.AddWithValue("@Email", Email);
			objCmd.Parameters.AddWithValue("@Age", Age);
			objCmd.Parameters.AddWithValue("@BirthDate", BirthDate);
			objCmd.Parameters.AddWithValue("@CityID", CityID);
			objCmd.Parameters.AddWithValue("@StateID", StateID);
			objCmd.Parameters.AddWithValue("@CountryID", CountryID);
			objCmd.Parameters.AddWithValue("@BranchID", BranchID);
			objCmd.Parameters.AddWithValue("@MobileNoStudent", MobileNoStudent);
			objCmd.Parameters.AddWithValue("@Gender", Gender);
			objCmd.Parameters.AddWithValue("@IsActive",IsActive);
			objCmd.Parameters.AddWithValue("@JoinStartDate", JoinStartDate);
			objCmd.Parameters.AddWithValue("@JoinEndDate", JoinEndDate);
			objCmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetInt32("UserID"));
			SqlDataReader objSDR = objCmd.ExecuteReader();
			dt.Load(objSDR);
			conn.Close();
			return View("Index", dt);

		}
		#endregion

		#region ADDEDIT
		public IActionResult AddEdit(int? StudentID)
		{

			City_DD();
			Branch_DD();


			if (StudentID == null)
			{
				@ViewData["addEdit"] = "REGISTER";
				return View("MST_Student_AddEdit");
			}
			else
			{
				@ViewData["addEdit"] = "EDIT";
				string connectionStr2 = Configuration.GetConnectionString("sql");
				SqlConnection conn1 = new SqlConnection(connectionStr2);
				conn1.Open();
				SqlCommand cmd = conn1.CreateCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "PR_Student_SelectByPK";
				cmd.Parameters.AddWithValue("@StudentID", StudentID);
				cmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetInt32("UserID"));
				SqlDataReader objSDR = cmd.ExecuteReader();
				MST_StudentModel bModel = new MST_StudentModel();
				if (objSDR.HasRows)
				{
					while (objSDR.Read())
					{
						bModel.StudentID = Convert.ToInt32(objSDR["StudentID"]);
						bModel.StudentName = objSDR["StudentName"].ToString();
						bModel.Email = objSDR["Email"].ToString();
						bModel.Age = Convert.ToInt32(objSDR["Age"]);
						bModel.BirthDate = Convert.ToDateTime(objSDR["BirthDate"]);
						bModel.Address = objSDR["Address"].ToString();
						bModel.CityID = Convert.ToInt32(objSDR["CityID"]);
						bModel.BranchID = Convert.ToInt32(objSDR["BranchID"]);
						bModel.MobileNoStudent = objSDR["MobileNoStudent"].ToString();
						bModel.MobileNoFather = objSDR["MobileNoFather"].ToString();
						bModel.Gender = objSDR["Gender"].ToString();
						bModel.IsActive = Convert.ToBoolean(objSDR["IsActive"]);
						bModel.Password = objSDR["Password"].ToString();

					}
				}
				conn1.Close();
				return View("MST_Student_AddEdit", bModel);
			}
		}
		#endregion

		#region AddEditSave
		public IActionResult AddEditSave(MST_StudentModel bModel)
		{
			string connectionStr = Configuration.GetConnectionString("sql");
			SqlConnection conn = new SqlConnection(connectionStr);
			conn.Open();
			SqlCommand cmd = conn.CreateCommand();
			cmd.CommandType = CommandType.StoredProcedure;
			if (bModel.StudentID == null)
			{
				cmd.CommandText = "PR_Student_Insert";
				
			}
			else
			{
				cmd.CommandText = "PR_Student_Update";
				cmd.Parameters.AddWithValue("@StudentID", bModel.StudentID);
			}
			cmd.Parameters.AddWithValue("@StudentName", bModel.StudentName);
			cmd.Parameters.AddWithValue("@Email", bModel.Email);
			cmd.Parameters.AddWithValue("@Age", bModel.Age);
			cmd.Parameters.AddWithValue("@BirthDate", bModel.BirthDate);
			cmd.Parameters.AddWithValue("@Address", bModel.Address);
			cmd.Parameters.AddWithValue("@CityID", bModel.CityID);
			cmd.Parameters.AddWithValue("@BranchID", bModel.BranchID);
			cmd.Parameters.AddWithValue("@MobileNoStudent", bModel.MobileNoStudent);
			cmd.Parameters.AddWithValue("@MobileNoFather", bModel.MobileNoFather);
			cmd.Parameters.AddWithValue("@Gender", bModel.Gender);
			cmd.Parameters.AddWithValue("@IsActive", bModel.IsActive);
			cmd.Parameters.AddWithValue("@Password", bModel.Password);
			cmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetInt32("UserID"));
			cmd.ExecuteNonQuery();
			conn.Close();
			return RedirectToAction("Index");
		}
		#endregion

		#region Delete
		public IActionResult DeleteStudent(int StudentID)
		{
			string connectionStr = Configuration.GetConnectionString("sql");
			SqlConnection conn = new SqlConnection(connectionStr);
			conn.Open();
			SqlCommand cmd = conn.CreateCommand();
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "PR_Student_Delete";
			cmd.Parameters.AddWithValue("@StudentID", StudentID);
			cmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetInt32("UserID"));
			cmd.ExecuteNonQuery();
			conn.Close();
			return RedirectToAction("Index");
		}
		#endregion

		#region Active Change
		public IActionResult ActiveChange(int StudentID, bool IsActive)
		{

			
			string connectionStr = Configuration.GetConnectionString("sql");
			SqlConnection conn1 = new SqlConnection(connectionStr);
			conn1.Open();
			SqlCommand cmd = conn1.CreateCommand();
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "PR_Student_UpdateByActive";
			cmd.Parameters.AddWithValue("@StudentID", StudentID);
			cmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetInt32("UserID"));
			cmd.ExecuteNonQuery();
			conn1.Close();
			return RedirectToAction("Index");
		}
		#endregion

		#region Branch_DD
		public void Branch_DD()
		{

			string connectionStr = Configuration.GetConnectionString("sql");
			List<MST_Branch_DropDown> Branchlist = new List<MST_Branch_DropDown>();
			SqlConnection sconn = new SqlConnection(connectionStr);
			sconn.Open();
			SqlCommand scmd = sconn.CreateCommand();
			scmd.CommandType = CommandType.StoredProcedure;
			scmd.CommandText = "PR_Branch_DropDown";
			scmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetInt32("UserID"));
			SqlDataReader sobj = scmd.ExecuteReader();
			if (sobj.HasRows)
			{
				while (sobj.Read())
				{
					MST_Branch_DropDown sModel = new MST_Branch_DropDown();
					sModel.BranchID = Convert.ToInt32(sobj["BranchID"]);
					sModel.BranchName = sobj["BranchName"].ToString();
					Branchlist.Add(sModel);
				}
				ViewBag.BranchList = Branchlist;
			}
			sconn.Close();
		}
		#endregion

		#region Country_DD
		public void Country_DD()
		{
			List<LOC_Country_DropDown> Countrylist = new List<LOC_Country_DropDown>();
			string connectionStr = Configuration.GetConnectionString("sql");
			SqlConnection conn = new SqlConnection(connectionStr);
			conn.Open();
			SqlCommand ddcmd = conn.CreateCommand();
			ddcmd.CommandType = CommandType.StoredProcedure;
			ddcmd.CommandText = "PR_Country_DropDown";
			ddcmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetInt32("UserID"));
			SqlDataReader obj = ddcmd.ExecuteReader();
			if (obj.HasRows)
			{
				while (obj.Read())
				{
					LOC_Country_DropDown cModel = new LOC_Country_DropDown();
					cModel.CountryID = Convert.ToInt32(obj["CountryID"]);
					cModel.CountryName = obj["CountryName"].ToString();
					Countrylist.Add(cModel);
				}
				ViewBag.CountryList = Countrylist;
			}else
			{

			}
			conn.Close();
		}
		public IActionResult DropDownByCountry(int CountryID)
		{
			return Json(StateList_DD(CountryID));
		}

		#endregion

		#region State_DD
		public void State_DD(int? CountryID = null)
		{
			ViewBag.StateList = StateList_DD(CountryID);
		}
		public IActionResult DropDownByState(int StateID)
		{
			return Json(CityList_DD(StateID));
		}

		public List<LOC_State_DropDown> StateList_DD(int? CountryID = null)
		{
			string connectionStr = Configuration.GetConnectionString("sql");
			List<LOC_State_DropDown> Statelist = new List<LOC_State_DropDown>();
			SqlConnection sconn = new SqlConnection(connectionStr);
			sconn.Open();
			SqlCommand scmd = sconn.CreateCommand();
			scmd.CommandType = CommandType.StoredProcedure;
			scmd.CommandText = "PR_State_SelectBYStateName";
			scmd.Parameters.AddWithValue("@CountryID", CountryID);
			scmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetInt32("UserID"));
			SqlDataReader sobj = scmd.ExecuteReader();

			if (sobj.HasRows)
			{
				while (sobj.Read())
				{
					LOC_State_DropDown sModel = new LOC_State_DropDown();
					sModel.StateID = Convert.ToInt32(sobj["StateID"]);
					sModel.StateName = sobj["StateName"].ToString();
					Statelist.Add(sModel);
				}

			}
			sconn.Close();

			return Statelist;
		}

		#endregion

		#region City_DD
		public void City_DD(int? StateID = null)
		{
			ViewBag.CityList = CityList_DD(StateID);
		}
		public List<LOC_City_DropDown> CityList_DD(int? StateID = null)
		{
			string connectionStr = Configuration.GetConnectionString("sql");
			List<LOC_City_DropDown> Citylist = new List<LOC_City_DropDown>();
			SqlConnection sconn = new SqlConnection(connectionStr);
			sconn.Open();
			SqlCommand scmd = sconn.CreateCommand();
			scmd.CommandType = CommandType.StoredProcedure;
			scmd.CommandText = "PR_City_DropDown";
			scmd.Parameters.AddWithValue("@StateID", StateID);
			scmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetInt32("UserID"));
			SqlDataReader sobj = scmd.ExecuteReader();

			if (sobj.HasRows)
			{
				while (sobj.Read())
				{
					LOC_City_DropDown sModel = new LOC_City_DropDown();
					sModel.CityID = Convert.ToInt32(sobj["CityID"]);
					sModel.CityName = sobj["CityName"].ToString();
					Citylist.Add(sModel);
				}

			}
			sconn.Close();

			return Citylist;
		}
		#endregion

		#region StudentProfile
		public IActionResult StudentProfile(int StudentID)
		{
			string connectionStr2 = Configuration.GetConnectionString("sql");
			SqlConnection conn1 = new SqlConnection(connectionStr2);
			conn1.Open();
			SqlCommand cmd = conn1.CreateCommand();
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "PR_Student_SelectByPK";
			cmd.Parameters.AddWithValue("@StudentID", StudentID);
			cmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetInt32("UserID"));
			SqlDataReader objSDR = cmd.ExecuteReader();
			MST_StudentModel bModel = new MST_StudentModel();
			if (objSDR.HasRows)
			{
				while (objSDR.Read())
				{
					bModel.StudentID = Convert.ToInt32(objSDR["StudentID"]);
					bModel.StudentName = objSDR["StudentName"].ToString();
					bModel.Email = objSDR["Email"].ToString();
					bModel.Age = Convert.ToInt32(objSDR["Age"]);
					bModel.BirthDate = Convert.ToDateTime(objSDR["BirthDate"]);
					bModel.Address = objSDR["Address"].ToString();
					bModel.CityID = Convert.ToInt32(objSDR["CityID"]);
					bModel.CityName = objSDR["CityName"].ToString();
					bModel.BranchName = objSDR["BranchName"].ToString();
					bModel.BranchID = Convert.ToInt32(objSDR["BranchID"]);
					bModel.MobileNoStudent = objSDR["MobileNoStudent"].ToString();
					bModel.MobileNoFather = objSDR["MobileNoFather"].ToString();
					bModel.Gender = objSDR["Gender"].ToString();
					bModel.IsActive = Convert.ToBoolean(objSDR["IsActive"]);
					bModel.Password = objSDR["Password"].ToString();

				}
			}
			conn1.Close();
			return View(bModel);
		}
		#endregion
	}
}
