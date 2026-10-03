

using Entities.DTO;
using System.ComponentModel.DataAnnotations;

namespace Entities.Services.Helpers
{
    public class ValidationHelper
    {
        public static void ModelValidation(object obj)
        {
            ValidationContext validationContext = new(obj);
            List<ValidationResult> validationresults = [];
            bool isValid = Validator.TryValidateObject(obj, validationContext, validationresults, true);
            if (!isValid)
            {
                throw new ArgumentException(validationresults.FirstOrDefault()?.ErrorMessage);
            }
        }
    }
}
