namespace TodoApi.Controllers

open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.JsonPatch
open Microsoft.AspNetCore.Authorization
open System.Linq
open AutoMapper
open TodoApi.Models
open TodoApi.Models.Paginator
open TodoApi.Constants
open TodoApi.Managers
open TodoApi.Repositories
open TodoApi.Attributes
open TodoApi.Extensions.HeaderDictionaryExtensions
open TodoApi.Extensions.StringExtensions

[<Authorize>]
[<Route("api/v1/users")>]
[<Consumes("application/json")>]
[<Produces("application/json")>]
[<ApiController>]
type UserController private () = 
    inherit ControllerBase()

    member val _userManager : IUserManager = null with get, set
    member val _userRepository : IUserRepository = null with get, set
    member val _mapper : IMapper = null with get, set

    new (
            userManager : IUserManager,
            userRepository : IUserRepository,
            mapper : IMapper
        ) as this = UserController() then
        this._userManager <- userManager
        this._userRepository <- userRepository
        this._mapper <- mapper

    [<HttpGet>]
    [<ProducesResponseType(StatusCodes.Status200OK, Type = typedefof<SuccessListUserResponse>)>]
    [<ProducesResponseType(StatusCodes.Status403Forbidden)>]
    [<ProducesResponseType(StatusCodes.Status500InternalServerError)>]
    [<Role([| Roles.SuperAdmin |])>]
    member this.Get ([<FromQuery>] request : Request) =
        async {
            let! users = this._userRepository.GetAllAsync(request) |> Async.AwaitTask
            let! totalDocs = this._userRepository.CountAsync(request) |> Async.AwaitTask

            let paginator = setObjectPaginator(request)(int(totalDocs))
            let dto = users.Select(fun u -> this._mapper.Map<SingleUserDto>(u)).ToList()
            let response = SuccessListUserResponse()
            response.Data <- dto
            response.Paginator <- paginator

            return response |> this.Ok :> IActionResult
        }

    [<HttpGet("{id}")>]
    [<ProducesResponseType(StatusCodes.Status200OK, Type = typedefof<SuccessUserResponse>)>]
    [<ProducesResponseType(StatusCodes.Status403Forbidden)>]
    [<ProducesResponseType(StatusCodes.Status404NotFound, Type = typedefof<FailResponse>)>]
    [<ProducesResponseType(StatusCodes.Status500InternalServerError)>]
    [<Role([| Roles.SuperAdmin |])>]
    [<UserExists>]
    member this.GetById ([<FromRouteAttribute>] id: string, [<FromQuery>] request : Request) =
        async {
            request.Filters <- request.Filters + sprintf "id=%s" id

            let! user = this._userRepository.GetOneAndPopulateAsync request |> Async.AwaitTask
            let dto = this._mapper.Map<SingleUserDto>(user)
            let response = SuccessUserResponse()
            response.Data <- dto

            return response |> this.Ok :> IActionResult
        }

    [<HttpGet("me")>]
    [<ProducesResponseType(StatusCodes.Status200OK, Type = typedefof<SuccessUserResponse>)>]
    [<ProducesResponseType(StatusCodes.Status403Forbidden)>]
    [<ProducesResponseType(StatusCodes.Status500InternalServerError)>]
    [<Role([| Roles.All |])>]
    member this.GetMe ([<FromQuery>] request : Request) =
        async {
            let token = this.HttpContext.Request.Headers.ExtractJsonWebToken()
            let id = token.SelectClaim("nameid")

            request.Filters <- request.Filters + sprintf "id=%s" id

            let! user = this._userRepository.GetOneAndPopulateAsync request |> Async.AwaitTask

            let dto = this._mapper.Map<SingleUserDto>(user)
            let response = SuccessUserResponse()
            response.Data <- dto

            return response |> this.Ok :> IActionResult
        }

    [<AllowAnonymous>]
    [<HttpPost>]
    [<ProducesResponseType(StatusCodes.Status200OK, Type = typedefof<SuccessUserResponse>)>]
    [<ProducesResponseType(StatusCodes.Status400BadRequest)>]
    [<ProducesResponseType(StatusCodes.Status500InternalServerError)>]
    member this.Create ([<FromBody>] user: CreateUserDto) =
        async {
            let mapped = this._mapper.Map<User>(user)
            do! this._userManager.CreateAsync(mapped) |> Async.AwaitTask

            let dto  = this._mapper.Map<SingleUserDto>(mapped)
            let response = SuccessUserResponse()
            response.Data <- dto

            return this.Created("", response) :> IActionResult
        }

    [<HttpPatch("{id}")>]
    [<ProducesResponseType(StatusCodes.Status200OK, Type = typedefof<SuccessUserResponse>)>]
    [<ProducesResponseType(StatusCodes.Status403Forbidden)>]
    [<ProducesResponseType(StatusCodes.Status404NotFound, Type = typedefof<FailResponse>)>]
    [<ProducesResponseType(StatusCodes.Status400BadRequest)>]
    [<ProducesResponseType(StatusCodes.Status500InternalServerError)>]
    [<Role([| Roles.All |])>]
    [<UserExists>]
    member this.UpdateById([<FromRoute>] id: string, [<FromBody>] replaceUser: JsonPatchDocument<User>) =
        async {
            let! user = this._userRepository.GetByIdAsync id |> Async.AwaitTask
            let! _ = this._userManager.UpdateByIdAsync (id)(user)(replaceUser) |> Async.AwaitTask

            let dto = this._mapper.Map<SingleUserDto>(user)
            let response = SuccessUserResponse()
            response.Data <- dto

            return response |> this.Ok :> IActionResult
        }

    [<HttpDelete("{id}")>]
    [<ProducesResponseType(StatusCodes.Status204NoContent)>]
    [<ProducesResponseType(StatusCodes.Status403Forbidden)>]
    [<ProducesResponseType(StatusCodes.Status404NotFound, Type = typedefof<FailResponse>)>]
    [<ProducesResponseType(StatusCodes.Status500InternalServerError)>]
    [<Role([| Roles.SuperAdmin |])>]
    [<UserExists>]
    member this.DeleteById ([<FromRoute>] id: string) =
        async {
            let! _ = this._userManager.DeleteByIdAsync id |> Async.AwaitTask
            return this.NoContent() :> IActionResult
        }