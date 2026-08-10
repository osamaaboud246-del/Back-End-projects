using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Globalization;

namespace StudentPortalWeb.Constraints
{
    public class IntakeCodeConstraint : IRouteConstraint
    {
        private const string AllowedCode = "itiB";

        public bool Match(
            HttpContext? httpContext,
            IRouter? route,
            string routeKey,
            RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            if (!values.TryGetValue(routeKey, out var value) || value == null)
            {
                return false;
            }

            var code = Convert.ToString(value, CultureInfo.InvariantCulture);

            return string.Equals(code, AllowedCode, StringComparison.OrdinalIgnoreCase);
        }
    }
}