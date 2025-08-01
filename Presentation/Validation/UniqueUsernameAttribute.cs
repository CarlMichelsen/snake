using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Auth;

namespace Presentation.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class UniqueUsernameAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }
        
        if (value is not string username)
        {
            return new ValidationResult("This field is not a string or a string type.");
        }
        
        var userContainer = validationContext.GetRequiredService<ISessionContainer>();
        var sameNameUser = userContainer.Sessions.Values.FirstOrDefault(u =>
            u.User.Name.Equals(username, StringComparison.InvariantCultureIgnoreCase));

        if (sameNameUser is not null)
        {
            return new ValidationResult("This username is already taken.");
        }
        
        return ValidationResult.Success;
    }
}