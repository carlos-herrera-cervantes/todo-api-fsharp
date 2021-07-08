namespace TodoApi.Attributes

open System
open Microsoft.Extensions.Localization
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Mvc.Filters
open System.Threading.Tasks
open TodoApi.Repositories
open TodoApi.Extensions.HeaderDictionaryExtensions
open TodoApi.Extensions.StringExtensions
open TodoApi.Models
open TodoApi

type TodoExistsFilter private () =

    member val _todoRepository : ITodoRepository = null with get, set
    member val _localizer : IStringLocalizer<SharedResources> = null with get, set

    new (
            todoRepository : ITodoRepository,
            localizer : IStringLocalizer<SharedResources>
        ) as this = TodoExistsFilter() then
        this._todoRepository <- todoRepository
        this._localizer <- localizer
    
    interface IAsyncActionFilter with

        member this.OnActionExecutionAsync(context: ActionExecutingContext, next: ActionExecutionDelegate) =
            async {
                try
                    let pair = context.ActionArguments.TryGetValue("id")
                    let id = snd pair
                    let! todo = this._todoRepository.GetByIdAsync(id.ToString()) |> Async.AwaitTask

                    let token = context.HttpContext.Request.Headers.ExtractJsonWebToken()
                    let sub = token.SelectClaim("nameid")

                    if (isNull todo) then
                        let firstValidation = FailResponse()
                        firstValidation.Status <- false
                        firstValidation.Message <- this._localizer.Item("TodoNotFound").Value
                        firstValidation.Code <- "TodoNotFound"

                        context.Result <- firstValidation |> NotFoundObjectResult
                    elif (sub <> todo.User) then
                        let thirdValidation = FailResponse()
                        thirdValidation.Status <- false
                        thirdValidation.Message <- this._localizer.Item("InvalidOperation").Value
                        thirdValidation.Code <- "InvalidOperation"

                        context.Result <- thirdValidation |> BadRequestObjectResult
                    else
                        do! next.Invoke() :> Task |> Async.AwaitTask
                with
                | :? AggregateException as e ->
                    let secondValidation = FailResponse()
                    secondValidation.Status <- false
                    secondValidation.Message <- this._localizer.Item("InvalidObjectId").Value
                    secondValidation.Code <- "InvalidObjectId"

                    context.Result <- secondValidation |> BadRequestObjectResult
            }
            |> Async.StartAsTask :> Task

type TodoExistsAttribute () =
    inherit TypeFilterAttribute(typeof<TodoExistsFilter>)