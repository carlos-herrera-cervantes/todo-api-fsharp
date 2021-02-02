namespace TodoApi.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.JsonPatch
open Microsoft.AspNetCore.Authorization
open TodoApi.Models
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

    new (userManager : IUserManager, userRepository : IUserRepository) as this =
        UserController() then
        this._userManager <- userManager
        this._userRepository <- userRepository

    [<HttpGet>]
    member this.Get () =
        async {
            let! users = this._userRepository.GetAllAsync() |> Async.AwaitTask
            let response = { Status = true; Data = users; }
            return response |> this.Ok :> IActionResult
        }

    [<HttpGet("{id}")>]
    [<UserExists>]
    member this.GetById (id: string) =
        async {
            let! user = this._userRepository.GetByIdAsync id |> Async.AwaitTask
            let response = { Status = true; Data = user; }
            return response |> this.Ok :> IActionResult
        }

    [<AllowAnonymous>]
    [<HttpPost>]
    member this.Create (user: User) =
        async {
            do! this._userManager.CreateAsync user |> Async.AwaitTask
            let response = { Status = true; Data = user; }
            return this.Created("", response) :> IActionResult
        }

    [<HttpPatch("{id}")>]
    member this.UpdateById(id: string, [<FromBodyAttribute>] replaceUser: JsonPatchDocument<User>) =
        async {
            let! user = this._userRepository.GetByIdAsync id |> Async.AwaitTask 
            let! result = this._userManager.UpdateByIdAsync (id)(user)(replaceUser) |> Async.AwaitTask
            let response = { Status = true; Data = user; }
            return response |> this.Ok :> IActionResult
        }

    [<HttpDelete("{id}")>]
    member this.DeleteById (id: string) =
        async {
            let! result = this._userManager.DeleteByIdAsync id |> Async.AwaitTask
            return this.NoContent() :> IActionResult
        }