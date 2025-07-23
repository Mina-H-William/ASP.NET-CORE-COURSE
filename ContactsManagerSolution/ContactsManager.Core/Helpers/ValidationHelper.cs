using System.ComponentModel.DataAnnotations;


namespace Services.Helpers
{
    public class ValidationHelper
    {
        public static void ModelValidation(object obj)
        {
            // to validate PersonAddRequest
            ValidationContext validationContext = new ValidationContext(obj);

            List<ValidationResult> validationResults = new List<ValidationResult>();

            //***forth parameter is for validate all property if it false it will validate only required paramters****
            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);

            if (!isValid)
            {
                throw new ArgumentException(validationResults.FirstOrDefault()?.ErrorMessage);
            }
        }
    }
}
