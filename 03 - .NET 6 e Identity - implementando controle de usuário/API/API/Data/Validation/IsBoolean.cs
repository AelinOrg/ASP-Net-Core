using System.ComponentModel.DataAnnotations;

namespace API.Data.Validation;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class IsBooleanAttribute : ValidationBase
{

    public IsBooleanAttribute(bool IsOptional = false) : base(IsOptional)
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null && IsOptional == true)
        {
            return ValidationResult.Success;
        }

        if (value is bool)
        {
            return ValidationResult.Success;
        }

        return new ValidationResult("IS_NOT_BOOLEAN");
    }
}