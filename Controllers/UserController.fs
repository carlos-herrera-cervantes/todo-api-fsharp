namespace TodoApi.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.JsonPatch
open TodoApi.Models
open TodoApi.Managers
open TodoApi.Repositories
open TodoApi.Attributes

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
        let users = this._userRepository.GetAllAsync() |> Async.AwaitTask |> Async.RunSynchronously
        let response = { Status = true; Data = users; }
        response |> this.Ok :> IActionResult

    [<HttpGet("{id}")>]
    [<UserExists>]
    member this.GetById (id: string) =
        let user = this._userRepository.GetByIdAsync id |> Async.AwaitTask |> Async.RunSynchronously
        let response = { Status = true; Data = user; }
        response |> this.Ok :> IActionResult

    [<HttpPost>]
    member this.Create (user: User) : IActionResult =
        this._userManager.CreateAsync user |> Async.AwaitTask |> ignore
        let response = { Status = true; Data = user; }
        this.Created("", response) :> IActionResult

    [<HttpPatch("{id}")>]
    member this.UpdateById(id: string, [<FromBodyAttribute>] replaceUser: JsonPatchDocument<User>) =
        let user = this._userRepository.GetByIdAsync id |> Async.AwaitTask |> Async.RunSynchronously
        this._userManager.UpdateByIdAsync (id)(user)(replaceUser) |> Async.AwaitTask |> Async.RunSynchronously |> ignore
        let response = { Status = true; Data = user; }
        response |> this.Ok :> IActionResult

    [<HttpDelete("{id}")>]
    member this.DeleteById (id: string) =
        this._userManager.DeleteByIdAsync id |> Async.AwaitTask |> Async.RunSynchronously |> ignore
        this.NoContent() :> IActionResult