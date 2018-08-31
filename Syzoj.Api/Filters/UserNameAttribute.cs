using System.ComponentModel.DataAnnotations;
using Syzoj.Api.Utils;

namespace Syzoj.Api.Filters
{
    /// <summary>
    /// Ensures that usernames consist of only lowercase letters, digits,
    /// underscore and hyphen, and is between 3 and 32 characters.
    /// </summary>
    /// <see cref="MiscUtils.CheckUserName(string)" />
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