using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Singer.Utilities.Utils;

public class JwtAuthorizationFilter : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var authenticateResult = await context.HttpContext.AuthenticateAsync();

        if (!authenticateResult.Succeeded || !authenticateResult.Principal.Identity.IsAuthenticated)
        {
            // El token no es válido, devolvemos Unauthorized
            context.Result = new UnauthorizedResult();
            return;
        }

        // El token es válido, continuar con la solicitud
        await Task.CompletedTask;
    }
}
