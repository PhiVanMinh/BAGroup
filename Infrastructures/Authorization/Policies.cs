using Domain.Master;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructures.Authorization
{
    public static class Policies
    {
        public const string UserView = "user.view";
        public const string UserCreate = "user.create";
        public const string UserUpdate = "user.update";
        public const string UserDelete = "user.delete";
        public const string CreateOrUpdateUser = "user.createOrEdit";

        public static AuthorizationPolicy ViewPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(UserView).Build();
        }

        public static AuthorizationPolicy CreatePolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(UserCreate).Build();
        }
        public static AuthorizationPolicy UpdatePolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(UserUpdate).Build();
        }
        public static AuthorizationPolicy DeletePolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(UserDelete).Build();
        }
        public static AuthorizationPolicy CreateOrUpdatePolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(CreateOrUpdateUser).Build();
        }
    }
}
