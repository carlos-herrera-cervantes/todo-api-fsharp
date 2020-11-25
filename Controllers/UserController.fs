namespace TodoApi.Controllers

open Microsoft.AspNetCore.Mvc
open TodoApi.Models

[<Route("api/v1/users")>]
[<ApiController>]
type UserController () = 
    inherit ControllerBase()

    let users = [| 
        { Id = "HFG14"; Email = "carlos@example.com"; Name = "Carlos Herrera";  };
        { Id = "VB771"; Email = "ruth@example.com"; Name = "Ruth Villa";  }
    |]

    [<HttpGet>]
    member this.Get() =
        ActionResult<Response<_>>({ Status = true; Data = users; })

    [<HttpGet("{id}")>]
    member this.GetById(id: string) =
        let user = users |> Seq.filter(fun u -> u.Id = id) |> Seq.head
        ActionResult<Response<_>>({ Status = true; Data = user; })

    [<HttpPost>]
    member this.Create(user: User) =
        let result = Seq.append users [user]
        ActionResult<Response<_>>({ Status = true; Data = user; })

    [<HttpPatch("{id}")>]
    member this.UpdateById(id: string, user: User) =
        let userFinded = users |> Seq.filter(fun u -> u.Id = id) |> Seq.head
        let usersWithoutMatch = users |> Seq.filter(fun u -> u.Id <> id)
        userFinded.Name <- user.Name
        let usersUpdated = Seq.append usersWithoutMatch [userFinded]
        ActionResult<Response<_>>({ Status = true; Data = usersUpdated; })

    [<HttpDelete("{id}")>]
    member this.DeleteById(id: string) =
        let result = users |> Seq.filter(fun u -> u.Id <> id)
        ActionResult<Response<_>>({ Status = true; Data = result; })