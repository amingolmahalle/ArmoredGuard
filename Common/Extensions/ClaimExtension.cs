using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Common.Extensions
{
    public static class ClaimExtension
    {
        public static void AddUserId(this ICollection<Claim> claims, string userId)
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
        }

        public static void AddUserName(this ICollection<Claim> claims, string userName)
        {
            claims.Add(new Claim(ClaimTypes.Name, userName));
        }

        public static void AddSecurityStamp(this ICollection<Claim> claims, string securityStamp)
        {
            claims.Add(new Claim(new ClaimsIdentityOptions().SecurityStampClaimType, securityStamp));
        }

        public static void AddRoles(this ICollection<Claim> claims, IEnumerable<string> roles)
        {
            roles.ToList().ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role)));
        }

        public static void AddFirstName(this ICollection<Claim> claims, string firstName)
        {
            claims.Add(new Claim("FirstName", firstName ?? ""));
        }

        public static void AddLastName(this ICollection<Claim> claims, string lastName)
        {
            claims.Add(new Claim("LastName", lastName ?? ""));
        }

        public static void AddMobileNumber(this ICollection<Claim> claims, string mobileNumber)
        {
            claims.Add(new Claim("MobileNumber", mobileNumber ?? ""));
        }
    }
}