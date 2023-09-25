using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using FormApp.Areas.LOC_Country.Models;
using FormApp.Areas.LOC_State.Models;
using FormApp.Areas.LOC_City.Models;
using FormApp.BAL;

namespace FormApp.Areas.LOC_City.Controllers
{
	[CheckAccess]
	[Area("LOC_City")]
	[Route("LOC_City/[Controller]/[Action]")]

	public class LOC_CityController : Controller
	{
		#region Configuration
		private readonly IConfiguration Configuration;

		public LOC_CityController(IConfiguration _Configuration)
		{
			Configuration = _Configuration;
		}
		#endregion

		#region SelectALL

		public IActionResult Index(int? CountryID, int? StateID, string? CityName, string? Citycode, bool filter = false)
		{
			Country_DD();
			State_DD();
			string connectionStr = Configuration.GetConnectionString("sql");
			DataTable dt = new DataTable();
			SqlConnection conn = new SqlConnection(connectionStr);
			conn.Open();
			SqlCommand objCmd = conn.CreateCommand();
			objCmd.CommandType = CommandType.StoredProcedure;
			if (Convert.ToBoolean(filter))
			{
				objCmd.CommandText = "PR_City_Search";
				objCmd.Parameters.AddWithValue("@Citycode", Citycode);
				objCmd.Parameters.AddWithValue("@CityName", CityName);
				objCmd.Parameters.AddWithValue("@StateID", StateID);
				objCmd.Parameters.AddWithValue("@CountryID", CountryID);


			}
			else
			{
				objCmd.CommandText = "PR_City_SelectAll";

			}
			objCmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetInt32("UserID"));

			SqlDataReader objSDR = objCmd.ExecuteReader();
			dt.Load(objSDR);
			conn.Close();
			return View(dt);
		}
		#endregion


		#region AddEdit
		public IActionResult AddEdit(int? CityID = null)
		{
			Country_DD();


			if (CityID == null)
			{
				State_DD();
				@ViewData["addEdit"] = "ADD";
				return View("LOC_City_AddEdit");
			}
			else
			{
				@ViewData["addEdit"] = "EDIT";
				string connectionStr2 = Configuration.GetConnectionString("sql");
				SqlConnection conn1 = new SqlConnection(connectionStr2);
				conn1.Open();
				SqlCommand cmd = conn1.CreateCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "PR_City_SelectByPK";
				cmd.Parameters.AddWithValue("@CityID", CityID);
				cmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetInt32("UserID"));


				SqlDataReader objSDR = cmd.ExecuteReader();
				LOC_CityModel cModel = new LOC_CityModel();
				if (objSDR.HasRows)
				{
					while (objSDR.Read())
					{
						cModel.CityID = Convert.ToInt32(objSDR["CityID"]);
						cModel.CityName = objSDR["CityName"].ToString();
						cModel.Citycode = objSDR["Citycode"].ToString();
						cModel.CountryID = Convert.ToInt32(objSDR["CountryID"]);
						cModel.StateID = Convert.ToInt32(objSDR["StateID"]);
					}
				}
				conn1.Close();
				State_DD(cModel.CountryID);

				return View("LOC_City_AddEdit", cModel);
			}
		}
		#endregion

		#region AddEdit_Save
		public IActionResult AddEditSave(LOC_CityModel cModel)
		{
			string connectionStr = Configuration.GetConnectionString("sql");
			SqlConnection conn = new SqlConnection(connectionStr);
			conn.Open();
			SqlCommand cmd = conn.CreateCommand();
			cmd.CommandType = CommandType.StoredProcedure;
			if (cModel.CityID == null)
			{
				cmd.CommandText = "PR_City_Insert";
			}
			else
			{
				cmd.CommandText = "PR_City_Update";
				cmd.Parameters.AddWithValue("@CityID", cModel.CityID);
			}
			cmd.Parameters.AddWithValue("@CityName", cModel.CityName);
			cmd.Parameters.AddWithValue("@CountryID", Convert.ToInt32(cModel.CountryID));
			cmd.Parameters.AddWithValue("@StateID", Convert.ToInt32(cModel.StateID));
			cmd.Parameters.AddWithValue("@Citycode", cModel.Citycode);
			cmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetInt32("UserID"));

			cmd.ExecuteNonQuery();
			conn.Close();
			return RedirectToAction("Index");
		}
		#endregion

		#region Delete
		public IActionResult DeleteCity(int CityID)
		{
			try
			{
				string connectionStr = Configuration.GetConnectionString("sql");
				SqlConnection conn = new SqlConnection(connectionStr);
				conn.Open();
				SqlCommand cmd = conn.CreateCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "PR_City_Delete";
				cmd.Parameters.AddWithValue("@CityID", CityID);
				cmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetInt32("UserID"));

				cmd.ExecuteNonQuery();
				conn.Close();
				return RedirectToAction("Index");

			}
			catch (Exception ex)
			{
				return RedirectToAction("Index");

			}
		}
		#endregion

		#region Country_DD
		public IActionResult DropDownByCountry(int CountryID)
		{
			return Json(StateList_DD(CountryID));
		}
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
			}
			conn.Close();
		}

		#endregion

		#region State_DD
		public void State_DD(int? CountryID = null)
		{
			ViewBag.StateList = StateList_DD(CountryID);
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
					sModel.CountryID = Convert.ToInt32(sobj["CountryID"]);
					Statelist.Add(sModel);
				}

			}
			sconn.Close();

			return Statelist;
		}
		#endregion

	}
}
