namespace TodoApi.Attributes

open System
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Mvc.Filters
open Microsoft.Extensions.Localization
open TodoApi.Extensions.HeaderDictionaryExtensions
open TodoApi.Extensions.StringExtensions
open TodoApi.Repositories
open TodoApi.Models
open TodoApi.Constants
open TodoApi

type UserExistsFilter private () =

    member val _userRepository : IUserRepository = null with get, set
    member val _localizer : IStringLocalizer<SharedResources> = null with get, set

    new (
            userRepository : IUserRepository,
            localizer : IStringLocalizer<SharedResources>
        ) as this = UserExistsFilter() then
        this._userRepository <- userRepository
        this._localizer <- localizer

    interface IAsyncActionFilter with

        member this.OnActionExecutionAsync(context: ActionExecutingContext, next: ActionExecutionDelegate) =
            async {
                try
                    let pair = context.ActionArguments.TryGetValue("id")
                    let id = snd pair
                    let! user = this._userRepository.GetByIdAsync(id.ToString()) |> Async.AwaitTask

                    let token = context.HttpContext.Request.Headers.ExtractJsonWebToken()
                    let sub = token.SelectClaim("nameid")
                    let role = token.SelectClaim("role")
                    
                    if (isNull user) then
                        let firstValidation = FailResponse()
                        firstValidation.Status <- false
                        firstValidation.Message <- this._localizer.Item("UserNotFound").Value
                        firstValidation.Code <- "UserNotFound"
                        
                        context.Result <- firstValidation |> NotFoundObjectResult
                    elif (sub <> user.Id && role <> Roles.SuperAdmin) then
                        let thirdValidation = FailResponse()
                        thirdValidation.Status <- false
                        thirdValidation.Message <- this._localizer.Item("InvalidOperation").Value
                        thirdValidation.Code <- "InvalidOperation"
                        
                        context.Result <- thirdValidation |> NotFoundObjectResult
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
            
type UserExistsAttribute() =
    inherit TypeFilterAttribute(typeof<UserExistsFilter>)