namespace TodoApi.Controllers

open Microsoft.AspNetCore.Mvc
open TodoApi.Models
open TodoApi.Managers

[<Route("api/v1/users")>]
[<ApiController>]
type UserController private () = 
    inherit ControllerBase()

    member val _userManager : IUserManager = null with get, set

    new (userManager : IUserManager) as this =
        UserController() then
        this._userManager <- userManager

    [<HttpGet>]
    member this.Get () =
        ActionResult<Response<_>>({ Status = true; Data = []; })

    [<HttpGet("{id}")>]
    member this.GetById(id: string) =
        ActionResult<Response<_>>({ Status = true; Data = []; })

    [<HttpPost>]
    member this.Create (user: User) : IActionResult =
        this._userManager.CreateAsync user |> Async.StartAsTask |> Async.AwaitTask |> ignore
        let response = { Status = true; Data = user; }
        response |> this.Ok :> IActionResult
        
    // [<HttpPatch("{id}")>]
    // member this.UpdateById(id: string, user: User) =
    //     let userFinded = users |> Seq.filter(fun u -> u.Id = id) |> Seq.head
    //     let usersWithoutMatch = users |> Seq.filter(fun u -> u.Id <> id)
    //     userFinded.Name <- user.Name
    //     let usersUpdated = Seq.append usersWithoutMatch [userFinded]
    //     ActionResult<Response<_>>({ Status = true; Data = usersUpdated; })

    // [<HttpDelete("{id}")>]
    // member this.DeleteById(id: string) =
    //     let result = users |> Seq.filter(fun u -> u.Id <> id)
    //     ActionResult<Response<_>>({ Status = true; Data = result; })