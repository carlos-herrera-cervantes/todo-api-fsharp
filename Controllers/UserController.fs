namespace TodoApi.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.JsonPatch
open Microsoft.AspNetCore.Authorization
open System.Linq
open AutoMapper
open TodoApi.Models
open TodoApi.Models.Paginator
open TodoApi.Managers
open TodoApi.Repositories
open TodoApi.Attributes

[<Authorize>]
[<Route("api/v1/users")>]
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
    member this.Get ([<FromQuery>] request : Request) =
        async {
            let! users = this._userRepository.GetAllAsync(request) |> Async.AwaitTask
            let! totalDocs = this._userRepository.CountAsync(request) |> Async.AwaitTask

            let paginator = setObjectPaginator(request)(int(totalDocs))
            let dto = users.Select(fun u -> this._mapper.Map<UserDto>(u))
            let response = { Status = true; Data = dto; Paginator = paginator; }

            return response |> this.Ok :> IActionResult
        }

    [<HttpGet("{id}")>]
    [<UserExists>]
    member this.GetById (id: string, [<FromQuery>] request : Request) =
        async {
            request.Filters <- request.Filters + sprintf "id=%s" id

            let! user = this._userRepository.GetOneAndPopulateAsync request |> Async.AwaitTask
            let dto = this._mapper.Map<UserDto>(user)
            let response = { Status = true; Data = dto; }
            return response |> this.Ok :> IActionResult
        }

    [<AllowAnonymous>]
    [<HttpPost>]
    member this.Create (user: User) =
        async {
            do! this._userManager.CreateAsync user |> Async.AwaitTask
            let dto  = this._mapper.Map<UserDto>(user)
            let response = { Status = true; Data = dto; }
            return this.Created("", response) :> IActionResult
        }

    [<HttpPatch("{id}")>]
    member this.UpdateById(id: string, [<FromBodyAttribute>] replaceUser: JsonPatchDocument<User>) =
        async {
            let! user = this._userRepository.GetByIdAsync id |> Async.AwaitTask 
            let! result = this._userManager.UpdateByIdAsync (id)(user)(replaceUser) |> Async.AwaitTask
            let dto = this._mapper.Map<UserDto>(user)
            let response = { Status = true; Data = dto; }
            return response |> this.Ok :> IActionResult
        }

    [<HttpDelete("{id}")>]
    member this.DeleteById (id: string) =
        async {
            let! result = this._userManager.DeleteByIdAsync id |> Async.AwaitTask
            return this.NoContent() :> IActionResult
        }