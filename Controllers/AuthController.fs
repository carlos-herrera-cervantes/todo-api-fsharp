namespace TodoApi.Controllers

open System
open System.IdentityModel.Tokens.Jwt
open System.Security.Claims
open System.Text
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Localization
open Microsoft.Extensions.Configuration
open Microsoft.IdentityModel.Tokens
open MongoDB.Driver
open TodoApi.Models
open TodoApi.Repositories
open TodoApi

[<Route("api/v1/auth")>]
[<ApiController>]
type AuthController private () =
    inherit ControllerBase()

    member val _userRepository : IUserRepository = null with get, set
    member val _configuration : IConfiguration = null with get, set
    member val _localizer : IStringLocalizer<SharedResources> = null with get, set

    new (
            userRepository : IUserRepository,
            configuration : IConfiguration,
            localizer : IStringLocalizer<SharedResources>
        ) as this = AuthController() then
        this._userRepository <- userRepository
        this._configuration <- configuration
        this._localizer <- localizer

    [<HttpPost("sign-in")>]
    member this.SignIn (credentials : Credentials) =
        async {
            let filter = Builders<User>.Filter.Where(fun u -> u.Email = credentials.Email)
            let! user = this._userRepository.GetOneAsync filter |> Async.AwaitTask
            
            let result = 
                match box user with
                | null ->
                    let firstValidation = {
                        Status = false
                        Message = this._localizer.Item("UserNotFound").Value
                        Code = "UserNotFound"
                    }
                    firstValidation |> NotFoundObjectResult :> IActionResult
                | _ ->
                    let isSame = credentials.Password = user.Password
                    
                    match isSame with
                    | true ->
                        let token = this.GenerateToken credentials
                        let response = { Status = true; Data = token; }
                        response |> OkObjectResult :> IActionResult
                    | false ->
                        let secondValidation = {
                            Status = false
                            Message = this._localizer.Item("InvalidCredentials").Value
                            Code = "InvalidCredentials"
                        }
                        secondValidation |> BadRequestObjectResult :> IActionResult

            return result
        }

    [<HttpPost("logout")>]
    member this.Logout () =
        this.NoContent() :> IActionResult

    /// <summary>Generate the Json Web Token</summary>
    /// <param name="credentials">User's email and password</param>
    /// <returns>Json Web Token</returns>
    member private this.GenerateToken (credentials : Credentials) =
        let claims = ClaimsIdentity([| Claim(ClaimTypes.Email, credentials.Email) |])
        let tokenDescriptor = 
            SecurityTokenDescriptor(
                Subject = claims,
                SigningCredentials = SigningCredentials(
                    SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(this._configuration.GetValue<string>("SecretKey"))
                    ),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Expires = Nullable(DateTime.UtcNow.AddDays(5.0))
            )
        let tokenHandler = JwtSecurityTokenHandler()
        let createdToken = tokenHandler.CreateToken(tokenDescriptor)

        tokenHandler.WriteToken(createdToken)