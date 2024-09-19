using LFG.Data;
using System.ComponentModel.DataAnnotations;

namespace LFG.Validation;

public class UniqueGroupNameAttribute : ValidationAttribute
{
  protected override ValidationResult IsValid(object value,
    ValidationContext validationContext)
  {
    if (value == null)
    {
      return ValidationResult.Success;
    }

    var context = (LFGContext)validationContext.GetService(typeof(LFGContext));

    if (!context.Groups.Any(a => a.Name == value.ToString()))
    {
      return ValidationResult.Success;
    }
    return new ValidationResult("Group Name taken");
  }
}