﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Demo.Infrastructure;
public static class AddKeycloakAuthenticationIServiceCollection
{
    public static IServiceCollection AddKeycloakAuthentication
        (this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"{configuration["Keycloak:BaseUrl"]}/realms/{configuration["Keycloak:Realm"]}",
                    ValidateAudience = true,
                    ValidAudience = "account",
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = false,
                    RoleClaimType = ClaimsIdentity.DefaultRoleClaimType,

                    IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                    {
                        var client = new HttpClient();
                        var keyUri = $"{parameters.ValidIssuer}/protocol/openid-connect/certs";
                        var response = client.GetAsync(keyUri).Result;
                        var keys = new JsonWebKeySet(response.Content.ReadAsStringAsync().Result);

                        return keys.GetSigningKeys();
                    }
                };

                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
            });

        return services;
    }
}
