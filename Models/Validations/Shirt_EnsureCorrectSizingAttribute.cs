using System.ComponentModel.DataAnnotations;

namespace WebApiDemo2.Models.Validations
{
    public class Shirt_EnsureCorrectSizingAttribute : ValidationAttribute
    {

        //edw dimiourgw ta attribute gia na mporw na kanw data annotation sto modelo shirt
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {

            var shirt = validationContext.ObjectInstance as Shirt;
            //pairnw to object Instance apo to validationContext opote exw to object me auton ton tropo
            if (shirt != null && !string.IsNullOrWhiteSpace(shirt.Gender)) 
            {
                if (shirt.Gender.Equals("men", StringComparison.OrdinalIgnoreCase) && shirt.Size < 8)
                {
                    return new ValidationResult("For men The size has to be greater or equal to 8");
                }
                else if (shirt.Gender.Equals("women", StringComparison.OrdinalIgnoreCase) && shirt.Size < 6)
                {
                    return new ValidationResult("For women The size has to be greater or equal to 6");
                }
                
            }
            return ValidationResult.Success;
        }
    }
}
