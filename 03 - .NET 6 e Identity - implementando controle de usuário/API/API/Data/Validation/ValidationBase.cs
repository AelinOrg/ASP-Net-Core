using System.ComponentModel.DataAnnotations;

namespace API.Data.Validation;

public class ValidationBase : ValidationAttribute
{
    public bool IsOptional { get; set; }

    public ValidationBase(bool IsOptional = false)
    {
        this.IsOptional = IsOptional;
    }
}

