namespace TodoApi.Extensions

open System
open System.Text
open Microsoft.AspNetCore.Authentication.JwtBearer
open Microsoft.AspNetCore.Authentication
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.IdentityModel.Tokens

module TokenAuthentication =

    type IServiceCollection with
        member this.AddTokenAuthentication(configuration: IConfiguration) =
            this.AddAuthentication(Action<AuthenticationOptions>(fun x ->
                x.DefaultAuthenticateScheme <- JwtBearerDefaults.AuthenticationScheme
                x.DefaultChallengeScheme <- JwtBearerDefaults.AuthenticationScheme
            )).AddJwtBearer(Action<JwtBearerOptions>(fun x ->
                x.RequireHttpsMetadata <- false
                x.SaveToken <- true
                x.TokenValidationParameters <- TokenValidationParameters(
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(configuration.GetValue<string>("SecretKey"))
                    ),
                    ValidateIssuer = false,
                    ValidateAudience = false
                )
            )) |> ignore
            this