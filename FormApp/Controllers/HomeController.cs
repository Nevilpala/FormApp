using FormApp.BAL;
using FormApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;



namespace FormApp.Controllers
{
    [CheckAccess]
	public class HomeController : Controller
	{ 
		private readonly IConfiguration ConnectionString;

		public HomeController( IConfiguration _configuration)
		{
			ConnectionString = _configuration; 
				

		}

		public IActionResult Index()
		{
			string connectionStr = ConnectionString.GetConnectionString("sql");
			SqlConnection conn1 = new SqlConnection(connectionStr);
			conn1.Open();
			SqlCommand cmd = conn1.CreateCommand();
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "PR_Table_RecordCount";
			cmd.Parameters.AddWithValue("@UserID", HttpContext.Session.GetInt32("UserID"));
			SqlDataReader rdr = cmd.ExecuteReader();
			if(rdr.HasRows)
			{
				while(rdr.Read())
				{
					string name = rdr["TableName"].ToString().Split("_")[1];
					name = name + "Count"; 
					ViewData[name] = rdr["RecordCount"].ToString(); 
				}
			}
			return View();
		}
 
         
        [Route("/Error")]
		[Route("/Error/{id?}")]
        public IActionResult Error(int statuscode)
        {
            return View();        
        }

	
	}
}