	using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using FormApp.Areas.MST_Branch.Models;
using FormApp.BAL;

namespace FormApp.Areas.MST_Branch.Controllers
{
	[CheckAccess]
	[Area("MST_Branch")]
	[Route("MST_Branch/[Controller]/[Action]")]

	public class MST_BranchController : Controller
	{
		#region Configuration
		private readonly IConfiguration Configuration;

		public MST_BranchController(IConfiguration _Configuration)
		{
			Configuration = _Configuration;
		}
		#endregion

		#region SelectALL
		public IActionResult Index(string? SearchName)
		{
			string connectionStr = Configuration.GetConnectionString("sql");
			DataTable dt = new DataTable();
			SqlConnection conn = new SqlConnection(connectionStr);
			conn.Open();
			SqlCommand objCmd = conn.CreateCommand();
			objCmd.CommandType = CommandType.StoredProcedure;
			if (SearchName != null && SearchName != "")
			{
				ViewBag.SearchName = SearchName;
				objCmd.CommandText = "PR_Branch_SelectALLByBranchName";
				objCmd.Parameters.AddWithValue("@SearchName", SearchName);
			}
			else
			{
				objCmd.CommandText = "PR_Branch_SelectAll";

			}
			objCmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetInt32("UserID"));
			SqlDataReader objSDR = objCmd.ExecuteReader();
			dt.Load(objSDR);
			conn.Close();
			return View("Index", dt);
		}
		#endregion

		#region ADDEDIT
		public IActionResult AddEdit(int? BranchID)
		{

			//List<MST_Branch_DropDown> list = new List<MST_Branch_DropDown>();
			//string connectionStr = Configuration.GetConnectionString("sql");
			//SqlConnection conn = new SqlConnection(connectionStr);
			//conn.Open();
			//SqlCommand ddcmd = conn.CreateCommand();
			//ddcmd.CommandType = CommandType.StoredProcedure;
			//ddcmd.CommandText = "PR_Branch_SelectBYCountryName_DD";
			//ddcmd.Parameters.AddWithValue("@BranchName", "");
			//SqlDataReader obj = ddcmd.ExecuteReader();
			//if (obj.HasRows)
			//{
			//	while (obj.Read())
			//	{
			//		MST_Branch_DropDown cModel = new MST_Branch_DropDown();
			//		cModel.BranchID = Convert.ToInt32(obj["BranchID"]);
			//		cModel.BranchName = obj["BranchName"].ToString();
			//		list.Add(cModel);
			//	}
			//	ViewBag.BranchList = list;
			//}
			//conn.Close();

			if (BranchID == null)
			{
				@ViewData["addEdit"] = "ADD";
				return View("MST_Branch_AddEdit");
			}
			else
			{
				@ViewData["addEdit"] = "EDIT";
				string connectionStr2 = Configuration.GetConnectionString("sql");
				SqlConnection conn1 = new SqlConnection(connectionStr2);
				conn1.Open();
				SqlCommand cmd = conn1.CreateCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "PR_Branch_SelectByPK";
				cmd.Parameters.AddWithValue("@BranchID", BranchID);
				cmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetInt32("UserID"));
				SqlDataReader objSDR = cmd.ExecuteReader();
				MST_BranchModel bModel = new MST_BranchModel();
				if (objSDR.HasRows)
				{
					while (objSDR.Read())
					{
						bModel.BranchID = Convert.ToInt32(objSDR["BranchID"]);
						bModel.BranchName = objSDR["BranchName"].ToString();
						bModel.BranchCode = objSDR["BranchCode"].ToString();
					}
				}
				conn1.Close();
				return View("MST_Branch_AddEdit", bModel);
			}
		}
		#endregion

		public IActionResult AddEditSave(MST_BranchModel bModel)
		{
			string connectionStr = Configuration.GetConnectionString("sql");
			SqlConnection conn = new SqlConnection(connectionStr);
			conn.Open();
			SqlCommand cmd = conn.CreateCommand();
			cmd.CommandType = CommandType.StoredProcedure;
			if (bModel.BranchID == null)
			{
				cmd.CommandText = "PR_Branch_Insert";
			}
			else
			{
				cmd.CommandText = "PR_Branch_Update";
				cmd.Parameters.AddWithValue("@BranchID", bModel.BranchID);
			}
			cmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetInt32("UserID"));
			cmd.Parameters.AddWithValue("@BranchName", bModel.BranchName);
			cmd.Parameters.AddWithValue("@BranchCode", bModel.BranchCode);
			cmd.ExecuteNonQuery();
			conn.Close();
			return RedirectToAction("Index");
		}
		public IActionResult DeleteBranch(int BranchID)
		{
			string connectionStr = Configuration.GetConnectionString("sql");
			SqlConnection conn = new SqlConnection(connectionStr);
			conn.Open();
			SqlCommand cmd = conn.CreateCommand();
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "PR_Branch_Delete";
			cmd.Parameters.AddWithValue("@BranchID", BranchID);
			cmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetInt32("UserID"));
			cmd.ExecuteNonQuery();
			conn.Close();
			return RedirectToAction("Index");
		}


	}
}