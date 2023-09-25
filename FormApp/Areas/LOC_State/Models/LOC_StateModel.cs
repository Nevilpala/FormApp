using System.ComponentModel.DataAnnotations;

namespace FormApp.Areas.LOC_State.Models
{
    public class LOC_StateModel
    {

        public int? StateID { get; set; }

        [Required(ErrorMessage = "Select Country Name")]
        public int CountryID { get; set; }

        [Required(ErrorMessage = "Enter State Name")]
        public string StateName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter State Code")]
        public string StateCode { get; set; } = string.Empty;



        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

    }
    public class LOC_State_DropDown
    {
        public int StateID { get; set; }
		public int? CountryID { get; set; }

		public string StateName { get; set; } = string.Empty;

    }
}
