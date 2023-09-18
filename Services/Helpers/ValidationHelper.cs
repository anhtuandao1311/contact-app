using System.ComponentModel.DataAnnotations;

namespace Services.Helpers
{
  public class ValidationHelper
  {
    internal static void ModelValidation(object obj)
    {
      ValidationContext validationContext = new(obj);
      List<ValidationResult> validationResults = new();
      if (!Validator.TryValidateObject(obj, validationContext, validationResults, true))
      {
        throw new ArgumentException(validationResults.FirstOrDefault()?.ErrorMessage);
      }
    }
  }
}
