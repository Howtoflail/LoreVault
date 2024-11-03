using LoreVault.Domain.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace LoreVault.Service
{
    public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
    {
        protected override Task HandleRequirementAsync(
          AuthorizationHandlerContext context,
          HasScopeRequirement requirement
        )
        {
            var scopeClaim = context.User.FindFirst(c => c.Type == "scope" && c.Issuer == requirement.Issuer);

            if (scopeClaim == null)
            {
                Console.WriteLine("Scope claim not found");
                return Task.CompletedTask;
            }

            // Split the scopes string into an array
            var scopes = scopeClaim.Value.Split(' ');

            // Succeed if the scope array contains the required scope
            if (scopes.Any(s => s == requirement.Scope))
            { 
                context.Succeed(requirement);
            }
            else
            {
                Console.WriteLine("Required scope not found");
            }

            return Task.CompletedTask;
        }
    }
}
