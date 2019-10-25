using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace BugHelper.Extensions
{
    public static class IdentityExtensions
    {
            public static string GetFavoriSorular(this IIdentity identity)
            {
                var claim = ((ClaimsIdentity)identity).FindFirst("FavoriSorular");
                // Test for null to avoid issues during local testing
                return (claim != null) ? claim.Value : string.Empty;
            }
    }
}