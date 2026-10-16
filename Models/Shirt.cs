using System.ComponentModel.DataAnnotations;
using WebApiDemo2.Models.Validations;
namespace WebApiDemo2.Models
{
    public class Shirt
    {
        public int ShirtId { get; set; }
        [Required] //thewreite model validation
        public string? Brand { get; set; }
        [Required]
        public string? Color { get; set; }
        [Shirt_EnsureCorrectSizing] //sthn onomasia den xreiazetai na balw to attribute otan kanw data annotation apo to validation
        public int? Size { get; set; }
        [Required]
        public string? Gender { get; set; }

        public double? Price { get; set; }

        //ta data mas edw anaparistoune diafora collums sto table tis basis
    }
}
