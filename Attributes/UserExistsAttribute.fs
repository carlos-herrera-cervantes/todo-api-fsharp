namespace TodoApi.Attributes

open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Mvc.Filters
open Microsoft.Extensions.Localization
open TodoApi.Repositories
open TodoApi.Models
open TodoApi

type UserExistsFilter private () =

    member val _userRepository : IUserRepository = null with get, set
    member val _localizer : IStringLocalizer<SharedResources> = null with get, set

    new (userRepository : IUserRepository, localizer : IStringLocalizer<SharedResources>) as this =
        UserExistsFilter() then
        this._userRepository <- userRepository
        this._localizer <- localizer

    interface IAsyncActionFilter with

        member this.OnActionExecutionAsync(context: ActionExecutingContext, next: ActionExecutionDelegate) =
            async {
                try
                    let pair = context.ActionArguments.TryGetValue("id")
                    let id = snd pair
                    let user = this._userRepository.GetByIdAsync(id.ToString()) |> Async.AwaitTask |> Async.RunSynchronously
                    
                    if (box user = null) then
                        context.Result <- { Status = false; Message = this._localizer.Item("UserNotFound").Value; Code = "UserNotFound" } |> NotFoundObjectResult

                    let task = next.Invoke()
                    return task
                with
                | :? System.AggregateException as e ->
                    context.Result <- { Status = false; Message = this._localizer.Item("InvalidObjectId").Value; Code = "InvalidObjectId" } |> BadRequestObjectResult
                    let task = next.Invoke()
                    return task
            }
            |> Async.StartAsTask :> Task
            
type UserExistsAttribute =
    inherit TypeFilterAttribute

    new () = { inherit TypeFilterAttribute(typeof<UserExistsFilter>) }