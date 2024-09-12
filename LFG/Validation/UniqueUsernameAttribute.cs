using LFG.Data;
using System.ComponentModel.DataAnnotations;

namespace LFG.Validation;

public class UniqueUsernameAttribute : ValidationAttribute
{
  protected override ValidationResult IsValid(object value,
    ValidationContext validationContext)
  {
    if (value == null)
    {
      return ValidationResult.Success;
    }

    var context = (LFGContext)validationContext.GetService(typeof(LFGContext));

    if (!context.Users.Any(a => a.Username == value.ToString()))
    {
      return ValidationResult.Success;
    }
    return new ValidationResult("Username taken");
  }
}