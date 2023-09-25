using System.ComponentModel.DataAnnotations;

namespace FormApp.Areas.MST_Student.Models
{
	public class MST_StudentModel
	{

		public int? StudentID { get; set; }

		[Required(ErrorMessage = "Select Branch Name")]
		public int BranchID { get; set; }
		public string BranchName { get; set; }

		[Required(ErrorMessage = "Select City Name")]
		public int CityID { get; set; }
		public string CityName { get; set; }

		[Required(ErrorMessage = "Enter Student Name")]
		public string StudentName { get; set; } = string.Empty;


		[Required(ErrorMessage = "Enter Student Mobile Number")]
		public string MobileNoStudent { get; set; } = string.Empty;


		[Required(ErrorMessage = "Enter Student Email")]
		public string Email { get; set; } = string.Empty;


		[Required(ErrorMessage = "Enter Student Father Mobile Number")]
		public string MobileNoFather { get; set; } = string.Empty;


		[Required(ErrorMessage = "Enter Student Address")]
		public string Address { get; set; } = string.Empty;

		[Required(ErrorMessage = "Enter Student BirthDate")]
		public DateTime BirthDate { get; set; }

		[Required(ErrorMessage = "Enter Student Age")]
		public int Age { get; set; }

		[Required(ErrorMessage = "Enter Student Is Active")]
		public bool IsActive { get; set; }

		[Required(ErrorMessage = "Enter Student Gender")]
		public string Gender { get; set; } = string.Empty;

		[Required(ErrorMessage = "Enter Student Password")]
		[StringLength(maximumLength:20,MinimumLength =8)]
		public string Password	{ get; set; } = string.Empty;

		public DateTime Created { get; set; }

		public DateTime Modified { get; set; }
	}
}
