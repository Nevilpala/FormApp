using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using FormApp.Areas.LOC_Country.Models;
using FormApp.Models;
using FormApp.BAL;

namespace FormApp.Areas.LOC_Country.Controllers
{

	[CheckAccess]
	[Area("LOC_Country")]
	[Route("LOC_Country/[Controller]/[Action]")]
	public class LOC_CountryController : Controller
	{
		private readonly IConfiguration Configuration;


		public LOC_CountryController(IConfiguration _Configuration)
		{
			Configuration = _Configuration;
		}


		#region List
		//[HttpPost]
		public IActionResult Index(string? SearchName, bool filter = false)
		{
			string connectionStr = Configuration.GetConnectionString("sql");
			DataTable dt = new DataTable();
			SqlConnection conn = new SqlConnection(connectionStr);
			conn.Open();
			SqlCommand objCmd = conn.CreateCommand();
			objCmd.CommandType = CommandType.StoredProcedure;
			if (Convert.ToBoolean(filter))
			{
				objCmd.CommandText = "PR_Country_Search";
				objCmd.Parameters.AddWithValue("@SearchName", SearchName);
			}
			else
			{
				objCmd.CommandText = "PR_Country_SelectAll";

			}
			objCmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetInt32("UserID"));
			SqlDataReader objSDR = objCmd.ExecuteReader();
			dt.Load(objSDR);
			conn.Close();
			if (filter)
			{
				return PartialView("_table", dt);
			}
			else
			{
				return View("Index", dt);

			}
		}
		#endregion

		#region AddEdit
		public IActionResult AddEdit(int? CountryID)
		{
			if (CountryID == null)
			{
				@ViewData["addEdit"] = "ADD";
				return View("LOC_Country_AddEdit");
			}
			else
			{
				@ViewData["addEdit"] = "EDIT";
				string connectionStr = Configuration.GetConnectionString("sql");
				SqlConnection conn = new SqlConnection(connectionStr);
				conn.Open();
				SqlCommand cmd = conn.CreateCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "PR_Country_SelectByPK";
				cmd.Parameters.AddWithValue("@CountryID", CountryID);
				SqlDataReader objSDR = cmd.ExecuteReader();
				LOC_CountryModel countryModel = new LOC_CountryModel();
				if (objSDR.HasRows)
				{
					while (objSDR.Read())
					{
						countryModel.CountryID = Convert.ToInt32(objSDR["CountryID"]);
						countryModel.CountryName = objSDR["CountryName"].ToString();
						countryModel.CountryCode = objSDR["CountryCode"].ToString();
					}
				}
				conn.Close();
				return View("LOC_Country_AddEdit", countryModel);
			}
		}
		#endregion

		#region AddEditSaveForm
		//[HttpPost]

		public IActionResult AddEditSave(LOC_CountryModel countryModel)
		{
			string connectionStr = Configuration.GetConnectionString("sql");
			SqlConnection conn = new SqlConnection(connectionStr);
			conn.Open();
			SqlCommand cmd = conn.CreateCommand();
			cmd.CommandType = CommandType.StoredProcedure;
			if (countryModel.CountryID == null)
			{
				cmd.CommandText = "PR_Country_Insert";
			}
			else
			{
				cmd.CommandText = "PR_Country_Update";
				cmd.Parameters.AddWithValue("@CountryID", countryModel.CountryID);
			}

			cmd.Parameters.AddWithValue("@CountryName", countryModel.CountryName);
			cmd.Parameters.AddWithValue("@CountryCode", countryModel.CountryCode);
			cmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetInt32("UserID"));
			cmd.ExecuteNonQuery();
			conn.Close();
			return RedirectToAction("Index");
				
		}
		#endregion

		#region Delete

		//[HttpPost]
		public IActionResult DeleteCountry(int CountryID)
		{
			string connectionStr = Configuration.GetConnectionString("sql");
			SqlConnection conn = new SqlConnection(connectionStr);
			conn.Open();
			SqlCommand cmd = conn.CreateCommand();
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "PR_Country_Delete";
			cmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetInt32("UserID"));
			cmd.Parameters.AddWithValue("@CountryID", CountryID);
			cmd.ExecuteNonQuery();
			conn.Close();
			return RedirectToAction("Index");
		}
		#endregion
	}
}
