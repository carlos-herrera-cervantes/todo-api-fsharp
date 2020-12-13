namespace TodoApi.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.JsonPatch
open TodoApi.Models
open TodoApi.Managers
open TodoApi.Repositories

[<Route("api/v1/todos")>]
[<ApiController>]
type TodoController private () =
    inherit ControllerBase()

    member val _todoRepository : ITodoRepository = null with get, set
    member val _todoManager : ITodoManager = null with get, set

    new (todoRepository : ITodoRepository, todoManager : ITodoManager) as this =
        TodoController() then
        this._todoRepository <- todoRepository
        this._todoManager <- todoManager

    [<HttpGet>]
    member this.Get () =
        let todos = this._todoRepository.GetAllAsync() |> Async.AwaitTask |> Async.RunSynchronously
        let response = { Status = true; Data = todos; }
        response |> this.Ok :> IActionResult

    [<HttpGet("{id}")>]
    member this.GetById (id: string) =
        let todo = this._todoRepository.GetByIdAsync id |> Async.AwaitTask |> Async.RunSynchronously
        let response = { Status = true; Data = todo; }
        response |> this.Ok :> IActionResult

    [<HttpPost>]
    member this.Create (todo: Todo) =
        this._todoManager.CreateAsync todo |> Async.AwaitTask |> ignore
        let response = { Status = true; Data = todo; }
        this.Created("", response) :> IActionResult

    [<HttpPatch("{id}")>]
    member this.UpdateById (id: string, [<FromBodyAttribute>] replaceTodo: JsonPatchDocument<Todo>) =
        let todo = this._todoRepository.GetByIdAsync id |> Async.AwaitTask |> Async.RunSynchronously
        this._todoManager.UpdateByIdAsync (id)(todo)(replaceTodo) |> Async.AwaitTask |> Async.RunSynchronously |> ignore
        let response = { Status = true; Data = todo; }
        response |> this.Ok :> IActionResult

    [<HttpDelete("{id}")>]
    member this.DeleteById (id: string) =
        this._todoManager.DeleteByIdAsync id |> Async.AwaitTask |> Async.RunSynchronously |> ignore
        this.NoContent() :> IActionResult