using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace FlintSoft.Permissions;

public class PermissionAuthorizationPolicyProvider
: DefaultAuthorizationPolicyProvider
{
    public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
    : base(options)
    {
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var builder = new AuthorizationPolicyBuilder();

        if (policyName == "EntraId")
        {
            return builder.AddAuthenticationSchemes("EntraId")
                .RequireAuthenticatedUser()
                .Build();
        }
        else
        {
            //Haben wir das Multischeme?
            var multipolicy = await base.GetPolicyAsync("MultiScheme");
            if (multipolicy is null)
            {
                builder
                    .AddAuthenticationSchemes("EntraId", "Local")
                    .RequireAuthenticatedUser();
            }

            var policy = await base.GetPolicyAsync(policyName);
            if (policy is not null)
            {
                return policy;
            }

            return builder
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();
        }
    }
}
