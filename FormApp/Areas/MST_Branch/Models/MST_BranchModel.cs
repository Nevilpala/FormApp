using System.ComponentModel.DataAnnotations;

namespace FormApp.Areas.MST_Branch.Models
{
	public class MST_BranchModel
	{
		public int? BranchID { get; set; }


		[Required(ErrorMessage = "Enter Branch Name")]
		public string BranchName { get; set; } = string.Empty;


		[Required(ErrorMessage = "Enter Branch Code")]
		public string BranchCode { get; set; } = string.Empty;

		public DateTime Created { get; set; }

		public DateTime Modified { get; set; }

	}
	public class MST_Branch_DropDown
	{
		public int BranchID { get; set; }
		public string BranchName { get; set; } = string.Empty;

	}
}
