
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace mvcIpsa.Extensions
{
    public static class ControllerExtensions
    {
        internal static AppUser GetServiceUser(this Controller controller)
        {
            AppUser usr = null;
            var identity = controller.User.Identity;
            if (!identity.IsAuthenticated)
                return null;

            usr = new AppUser { username = identity.Name };
            usr.LoadFromClaimsPrincipal(controller.User);
            return usr;
        }

        public static AppUser GetServiceUser(this ClaimsPrincipal principal)
        {
            AppUser usr = null;
            var identity = principal.Identity;
            if (!identity.IsAuthenticated)
                return null;

            usr = new AppUser { username = identity.Name };
            usr.LoadFromClaimsPrincipal(principal);
            return usr;
        }
    }
}
