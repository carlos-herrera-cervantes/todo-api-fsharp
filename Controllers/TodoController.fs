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
        async {
            let! todos = this._todoRepository.GetAllAsync() |> Async.AwaitTask
            let response = { Status = true; Data = todos; }
            return response |> this.Ok :> IActionResult
        }

    [<HttpGet("{id}")>]
    member this.GetById (id: string) =
        async {
            let! todo = this._todoRepository.GetByIdAsync id |> Async.AwaitTask
            let response = { Status = true; Data = todo; }
            return response |> this.Ok :> IActionResult
        }

    [<HttpPost>]
    member this.Create (todo: Todo) =
        async {
            do! this._todoManager.CreateAsync todo |> Async.AwaitTask
            let response = { Status = true; Data = todo; }
            return this.Created("", response) :> IActionResult
        }

    [<HttpPatch("{id}")>]
    member this.UpdateById (id: string, [<FromBodyAttribute>] replaceTodo: JsonPatchDocument<Todo>) =
        async {
            let! todo = this._todoRepository.GetByIdAsync id |> Async.AwaitTask
            let! result = this._todoManager.UpdateByIdAsync (id)(todo)(replaceTodo) |> Async.AwaitTask
            let response = { Status = true; Data = todo; }
            return response |> this.Ok :> IActionResult
        }

    [<HttpDelete("{id}")>]
    member this.DeleteById (id: string) =
        async {
            let! result = this._todoManager.DeleteByIdAsync id |> Async.AwaitTask
            return this.NoContent() :> IActionResult
        }