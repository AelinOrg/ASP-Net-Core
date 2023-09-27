using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace API.Authorization;

public class AuthPolicyHandler : AuthorizationHandler<AuthPolicy>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthPolicy requirement)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}