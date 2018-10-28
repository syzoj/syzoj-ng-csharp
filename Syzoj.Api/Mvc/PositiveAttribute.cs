using System;
using System.ComponentModel.DataAnnotations;

namespace Syzoj.Api.Mvc
{
    public class PositiveAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            bool isValid;
            switch(validationContext.ObjectInstance)
            {
                case int intValue:
                    isValid = intValue > 0;
                    break;
                case char charValue:
                    isValid = charValue > 0;
                    break;
                case byte byteValue:
                    isValid = byteValue > 0;
                    break;
                case decimal decimalValue:
                    isValid = decimalValue > 0;
                    break;
                case double doubleValue:
                    isValid = doubleValue > 0;
                    break;
                case float floatValue:
                    isValid = floatValue > 0;
                    break;
                case long longValue:
                    isValid = longValue > 0;
                    break;
                case short shortValue:
                    isValid = shortValue > 0;
                    break;
                case sbyte sbyteValue:
                    isValid = sbyteValue > 0;
                    break;
                case uint uintValue:
                    isValid = true;
                    break;
                case ulong ulongValue:
                    isValid = true;
                    break;
                case ushort ushortValue:
                    isValid = true;
                    break;
                default:
                    isValid = false;
                    break;
            }
            
            if(!isValid)
                return new ValidationResult("Value is not non-negative");
            return ValidationResult.Success;
        }
    }
}