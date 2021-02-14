namespace TodoApi.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.JsonPatch
open Microsoft.AspNetCore.Authorization
open System.Linq
open AutoMapper
open TodoApi.Models
open TodoApi.Models.User
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

    new (userManager : IUserManager, userRepository : IUserRepository, mapper : IMapper) as this = UserController() then
        this._userManager <- userManager
        this._userRepository <- userRepository
        this._mapper <- mapper

    [<HttpGet>]
    member this.Get () =
        async {
            let! users = this._userRepository.GetAllAsync() |> Async.AwaitTask
            let dto = users.Select(fun u -> this._mapper.Map<UserDto>(u))
            let response = { Status = true; Data = dto; }
            return response |> this.Ok :> IActionResult
        }

    [<HttpGet("{id}")>]
    [<UserExists>]
    member this.GetById (id: string) =
        async {
            let! user = this._userRepository.GetByIdAsync id |> Async.AwaitTask
            let dto = this._mapper.Map<UserDto>(user)
            let response = { Status = true; Data = dto; }
            return response |> this.Ok :> IActionResult
        }

    [<AllowAnonymous>]
    [<HttpPost>]
    member this.Create (user: User) =
        async {
            let obj = setValuesToUser user
            do! this._userManager.CreateAsync obj |> Async.AwaitTask
            let dto  = this._mapper.Map<UserDto>(obj)
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