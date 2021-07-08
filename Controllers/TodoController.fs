namespace TodoApi.Controllers

open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.JsonPatch
open Microsoft.AspNetCore.Authorization
open TodoApi.Models
open TodoApi.Models.Paginator
open TodoApi.Managers
open TodoApi.Repositories
open TodoApi.Attributes
open TodoApi.Constants
open TodoApi.Extensions.HeaderDictionaryExtensions
open TodoApi.Extensions.StringExtensions

[<Authorize>]
[<Route("api/v1/todos")>]
[<Consumes("application/json")>]
[<Produces("application/json")>]
[<ApiController>]
type TodoController private () =
    inherit ControllerBase()

    member val _todoRepository : ITodoRepository = null with get, set
    member val _todoManager : ITodoManager = null with get, set

    new (todoRepository : ITodoRepository, todoManager : ITodoManager) as this = TodoController() then
        this._todoRepository <- todoRepository
        this._todoManager <- todoManager

    [<HttpGet>]
    [<ProducesResponseType(StatusCodes.Status200OK, Type = typedefof<SuccessListTodoResponse>)>]
    [<ProducesResponseType(StatusCodes.Status403Forbidden)>]
    [<ProducesResponseType(StatusCodes.Status500InternalServerError)>]
    [<Role([| Roles.SuperAdmin |])>]
    member this.Get ([<FromQuery>] request : Request) =
        async {
            let! todos = this._todoRepository.GetAllAsync(request) |> Async.AwaitTask
            let! totalDocs = this._todoRepository.CountAsync(request) |> Async.AwaitTask

            let paginator = setObjectPaginator(request)(int(totalDocs))
            let response = SuccessListTodoResponse()
            response.Data <- todos
            response.Paginator <- paginator

            return response |> this.Ok :> IActionResult
        }

    [<HttpGet("{id}")>]
    [<ProducesResponseType(StatusCodes.Status200OK, Type = typedefof<SuccessTodoResponse>)>]
    [<ProducesResponseType(StatusCodes.Status403Forbidden)>]
    [<ProducesResponseType(StatusCodes.Status404NotFound, Type = typedefof<FailResponse>)>]
    [<ProducesResponseType(StatusCodes.Status500InternalServerError)>]
    [<Role([| Roles.All |])>]
    [<TodoExists>]
    member this.GetById ([<FromRoute>] id: string) =
        async {
            let! todo = this._todoRepository.GetByIdAsync id |> Async.AwaitTask
            let response = SuccessTodoResponse()
            response.Data <- todo

            return response |> this.Ok :> IActionResult
        }

    [<HttpGet("me")>]
    [<ProducesResponseType(StatusCodes.Status200OK, Type = typedefof<SuccessListTodoResponse>)>]
    [<ProducesResponseType(StatusCodes.Status403Forbidden)>]
    [<ProducesResponseType(StatusCodes.Status500InternalServerError)>]
    [<Role([| Roles.All |])>]
    member this.GetMe ([<FromQuery>] request : Request) =
        async {
            let token = this.HttpContext.Request.Headers.ExtractJsonWebToken()
            let id = token.SelectClaim("nameid")

            request.Filters <- if request.Filters.Length = 0 then "user=" + id else request.Filters + ",user=" + id

            let! todos = this._todoRepository.GetAllAsync(request) |> Async.AwaitTask
            let! totalDocs = this._todoRepository.CountAsync(request) |> Async.AwaitTask

            let paginator = setObjectPaginator(request)(int(totalDocs))
            let response = SuccessListTodoResponse()
            response.Data <- todos
            response.Paginator <- paginator

            return response |> this.Ok :> IActionResult
        }

    [<HttpPost>]
    [<ProducesResponseType(StatusCodes.Status200OK, Type = typedefof<SuccessTodoResponse>)>]
    [<ProducesResponseType(StatusCodes.Status403Forbidden)>]
    [<ProducesResponseType(StatusCodes.Status400BadRequest)>]
    [<ProducesResponseType(StatusCodes.Status500InternalServerError)>]
    [<Role([| Roles.SuperAdmin |])>]
    member this.Create ([<FromBody>] todo : Todo) =
        async {
            do! this._todoManager.CreateAsync todo |> Async.AwaitTask
            let response = SuccessTodoResponse()
            response.Data <- todo

            return this.Created("", response) :> IActionResult
        }

    [<HttpPost("me")>]
    [<ProducesResponseType(StatusCodes.Status200OK, Type = typedefof<SuccessTodoResponse>)>]
    [<ProducesResponseType(StatusCodes.Status403Forbidden)>]
    [<ProducesResponseType(StatusCodes.Status400BadRequest)>]
    [<ProducesResponseType(StatusCodes.Status500InternalServerError)>]
    [<Role([| Roles.All |])>]
    member this.CreateMe ([<FromBody>] todo : Todo) =
        async {
            let token = this.HttpContext.Request.Headers.ExtractJsonWebToken()
            let id = token.SelectClaim("nameid")

            todo.User <- id

            do! this._todoManager.CreateAsync todo |> Async.AwaitTask
            let response = SuccessTodoResponse()
            response.Data <- todo

            return this.Created("", response) :> IActionResult
        }

    [<HttpPatch("{id}")>]
    [<ProducesResponseType(StatusCodes.Status200OK, Type = typedefof<SuccessTodoResponse>)>]
    [<ProducesResponseType(StatusCodes.Status403Forbidden)>]
    [<ProducesResponseType(StatusCodes.Status404NotFound, Type = typedefof<FailResponse>)>]
    [<ProducesResponseType(StatusCodes.Status400BadRequest)>]
    [<ProducesResponseType(StatusCodes.Status500InternalServerError)>]
    [<Role([| Roles.All |])>]
    [<TodoExists>]
    member this.UpdateById ([<FromRoute>] id: string, [<FromBody>] replaceTodo: JsonPatchDocument<Todo>) =
        async {
            let! todo = this._todoRepository.GetByIdAsync id |> Async.AwaitTask
            let! _ = this._todoManager.UpdateByIdAsync (id)(todo)(replaceTodo) |> Async.AwaitTask
            let response = SuccessTodoResponse()
            response.Data <- todo
            
            return response |> this.Ok :> IActionResult
        }

    [<HttpDelete("{id}")>]
    [<ProducesResponseType(StatusCodes.Status204NoContent)>]
    [<ProducesResponseType(StatusCodes.Status403Forbidden)>]
    [<ProducesResponseType(StatusCodes.Status404NotFound, Type = typedefof<FailResponse>)>]
    [<ProducesResponseType(StatusCodes.Status500InternalServerError)>]
    [<Role([| Roles.All |])>]
    [<TodoExists>]
    member this.DeleteById ([<FromRoute>] id: string) =
        async {
            let! _ = this._todoManager.DeleteByIdAsync id |> Async.AwaitTask
            return this.NoContent() :> IActionResult
        }