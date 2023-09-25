using System.ComponentModel.DataAnnotations;

namespace FormApp.Areas.LOC_Country.Models
{
    public class LOC_CountryModel
    {
        public int? CountryID { get; set; }

        [Required(ErrorMessage = "Enter Country Name")]
        public string CountryName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter Country Code")]
        public string CountryCode { get; set; } = string.Empty;

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }

    public class LOC_Country_DropDown
    {
        public int CountryID { get; set; }
        public string CountryName { get; set; } = string.Empty;

    }

}