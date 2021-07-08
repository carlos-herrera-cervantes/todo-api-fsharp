namespace TodoApi.Controllers

open System
open System.Linq
open System.IdentityModel.Tokens.Jwt
open System.Security.Claims
open System.Text
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Localization
open Microsoft.Extensions.Configuration
open Microsoft.AspNetCore.Authorization
open Microsoft.IdentityModel.Tokens
open MongoDB.Driver
open TodoApi.Models
open TodoApi.Repositories
open TodoApi.Managers
open TodoApi.Extensions.HeaderDictionaryExtensions
open TodoApi

[<Route("api/v1/auth")>]
[<Consumes("application/json")>]
[<Produces("application/json")>]
[<ApiController>]
type AuthController private () =
    inherit ControllerBase()

    member val _userRepository : IUserRepository = null with get, set
    member val _configuration : IConfiguration = null with get, set
    member val _accessTokenManager : IAccessTokenManager = null with get, set
    member val _localizer : IStringLocalizer<SharedResources> = null with get, set

    new (
            userRepository : IUserRepository,
            configuration : IConfiguration,
            accessTokenManager : IAccessTokenManager,
            localizer : IStringLocalizer<SharedResources>
        ) as this = AuthController() then
        this._userRepository <- userRepository
        this._configuration <- configuration
        this._accessTokenManager <- accessTokenManager
        this._localizer <- localizer

    [<HttpPost("sign-in")>]
    [<ProducesResponseType(StatusCodes.Status200OK, Type = typedefof<SuccessAuthResponse>)>]
    [<ProducesResponseType(StatusCodes.Status400BadRequest, Type = typedefof<FailResponse>)>]
    [<ProducesResponseType(StatusCodes.Status500InternalServerError)>]
    member this.SignIn ([<FromBody>] credentials : Credentials) =
        async {
            let filter = Builders<User>.Filter.Where(fun u -> u.Email = credentials.Email)
            let! user = this._userRepository.GetOneAsync filter |> Async.AwaitTask
            
            let result = 
                match box user with
                | null ->
                    let firstValidation = FailResponse()

                    firstValidation.Status <- false
                    firstValidation.Message <- this._localizer.Item("UserNotFound").Value
                    firstValidation.Code <- "UserNotFound"
                    
                    firstValidation |> BadRequestObjectResult :> IActionResult
                | _ ->
                    let removalFilter = Builders<AccessToken>.Filter.Where(fun t -> t.User = user.Id)
                    this._accessTokenManager.DeleteManyAsync removalFilter |> Async.AwaitTask |> Async.RunSynchronously |> ignore

                    let isSame = credentials.Password = user.Password
                    
                    match isSame with
                    | true ->
                        let token = this.GenerateToken(credentials)(user)
                        let response = SuccessAuthResponse()
                        response.Data <- token
                        
                        let accessToken = AccessToken(
                                            Token = token,
                                            User = user.Id.ToString(),
                                            Email = user.Email,
                                            Roles = user.Roles)

                        this._accessTokenManager.CreateAsync accessToken |> Async.AwaitTask |> Async.RunSynchronously
                        response |> OkObjectResult :> IActionResult
                    | false ->
                        let secondValidation = FailResponse()

                        secondValidation.Status <- false
                        secondValidation.Message <- this._localizer.Item("InvalidCredentials").Value
                        secondValidation.Code <- "InvalidCredentials"

                        secondValidation |> BadRequestObjectResult :> IActionResult

            return result
        }

    [<HttpPost("logout")>]
    [<Authorize>]
    [<ProducesResponseType(StatusCodes.Status204NoContent)>]
    [<ProducesResponseType(StatusCodes.Status403Forbidden)>]
    [<ProducesResponseType(StatusCodes.Status500InternalServerError)>]
    member this.Logout () =
        async {
            let token = this.HttpContext.Request.Headers.ExtractJsonWebToken()
            let handler = JwtSecurityTokenHandler()
            let decoded = handler.ReadJwtToken(token)
            let id = decoded.Claims.ToList()
                        .Where(fun claim -> claim.Type = "nameid")
                        .Select(fun claim -> claim.Value)
                        .SingleOrDefault()

            let removalFilter = Builders<AccessToken>.Filter.Where(fun t -> t.User = id)
            let! _ = this._accessTokenManager.DeleteManyAsync removalFilter |> Async.AwaitTask

            return this.NoContent() :> IActionResult
        }

    /// <summary>Generate the Json Web Token</summary>
    /// <param name="credentials">User's email and password</param>
    /// <param name="user">User object model</param>
    /// <returns>Json Web Token</returns>
    member private this.GenerateToken (credentials : Credentials) (user : User) =
        let claims = ClaimsIdentity([|
                Claim(ClaimTypes.Email, credentials.Email)
                Claim(ClaimTypes.Role, user.Roles.[0])
                Claim(ClaimTypes.NameIdentifier, user.Id)
            |])
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