using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Api.Bff.Application.Services;
using Api.Bff.Domain.Configuration;
using Api.Bff.Domain.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Api.Bff
{
    public class Program
    {
        private const string AWS_COGNITO_SECTION = "Aws:Cognito";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var cognitoSettings = builder.Configuration.GetSection(AWS_COGNITO_SECTION).Get<CognitoSettings>()
                ?? throw new ArgumentNullException(AWS_COGNITO_SECTION);
            builder.Services.Configure<CognitoSettings>(builder.Configuration.GetSection(AWS_COGNITO_SECTION));

            builder.Services.AddScoped(sp =>
                new AmazonCognitoIdentityProviderClient(RegionEndpoint.GetBySystemName(cognitoSettings.Region)));
            builder.Services.AddScoped(sp =>
                new CognitoUserPool(cognitoSettings.UserPoolId, cognitoSettings.ClientId, sp.GetRequiredService<AmazonCognitoIdentityProviderClient>()));

            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                // Definir o esquema de segurança para JWT
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insira o token JWT no formato: Bearer {token}"
                });

                // Configurar o Swagger para usar o token Bearer nos endpoints protegidos
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://cognito-idp.{cognitoSettings.Region}.amazonaws.com/{cognitoSettings.UserPoolId}";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    //Por algum motivo o token gerado pela AWS não está retornando audience, e por isso foi desabilitado
                    ValidateAudience = false, 
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
