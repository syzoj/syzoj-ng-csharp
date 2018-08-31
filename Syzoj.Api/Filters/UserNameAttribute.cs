using System.ComponentModel.DataAnnotations;
using Syzoj.Api.Utils;

namespace Syzoj.Api.Filters
{
    public class UserNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string UserName = (string) value;
            if (!MiscUtils.CheckUserName(UserName))
            {
                return new ValidationResult("Invalid username.");
            }
            return ValidationResult.Success;
        }
    }
}