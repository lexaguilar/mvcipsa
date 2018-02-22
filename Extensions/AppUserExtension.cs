using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace mvcIpsa.Extensions
{
    static class AppUserExtension
    {
        class AppClaimTypes
        {
            public const string username = "username";
            public const string ncentrocosto = "ncentrocosto";
            public const string cajaid = "ncaja";
            public const string description = "description";
            public const string Token = "Token";
            public const string roles = "roles";
        }

        internal static ClaimsPrincipal CreatePrincipal(this AppUser usr)
        {
            var principal = new ClaimsPrincipal();

            var claims = new ClaimsIdentity(
                new Claim[] {
                                new Claim(ClaimTypes.Name,usr.username),
                                new Claim(AppClaimTypes.cajaid,usr.cajaid.ToString()),
                                new Claim(AppClaimTypes.description,usr.description.ToString()),
                                new Claim(AppClaimTypes.ncentrocosto,usr.ncentrocosto.ToString()),                                     
                                new Claim(AppClaimTypes.roles,String.Join(",", usr.roles))
                }, "Password");

            if (usr.Token != null)
                claims.AddClaim(new Claim(AppClaimTypes.Token, usr.Token));

            principal.AddIdentity(claims);

            return principal;
        }

        internal static void LoadFromClaimsPrincipal(this AppUser usr, ClaimsPrincipal principal)
        {
            foreach (var claim in principal.Claims)
            {
                if (claim.Type == AppClaimTypes.description)
                    usr.description = Convert.ToString(claim.Value);

                if (claim.Type == AppClaimTypes.cajaid)
                    usr.cajaid = Convert.ToInt32(claim.Value);

                if (claim.Type == AppClaimTypes.ncentrocosto)
                    usr.ncentrocosto = Convert.ToInt32(claim.Value);

                if (claim.Type == AppClaimTypes.Token)
                    usr.Token = claim.Value;

                if (claim.Type == AppClaimTypes.roles)
                    usr.roles = claim.Value.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
            }
        }
    }

    public class AppUser : IAppUser
    {
        public string username { get; set; }
        public int ncentrocosto { get; set; }
        public int cajaid { get; set; }
        public int[] roles { get; set; }
        internal string Token { get; set; }
        public string description { get; set; }

    }
}
