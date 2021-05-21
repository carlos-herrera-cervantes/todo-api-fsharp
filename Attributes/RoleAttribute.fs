namespace TodoApi.Attributes

open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Localization
open Microsoft.AspNetCore.Mvc.Filters
open TodoApi.Repositories
open TodoApi.Models
open TodoApi.Constants
open TodoApi

type RoleFilter private () =

    member val _accessTokenRepository : IAccessTokenRepository = null with get, set
    member val _localizer : IStringLocalizer<SharedResources> = null with get, set
    member val _roles : string[] = null with get, set

    new (
            accessTokenRepository : IAccessTokenRepository,
            localizer : IStringLocalizer<SharedResources>,
            roles : string[]
        ) as this = RoleFilter() then
        this._accessTokenRepository <- accessTokenRepository
        this._localizer <- localizer
        this._roles <- roles

    interface IAsyncActionFilter with

        member this.OnActionExecutionAsync(context: ActionExecutingContext, next: ActionExecutionDelegate) =
            async {
                let freePass = this._roles.Where(fun outer -> outer = Roles.All).FirstOrDefault()

                match freePass with
                    | null ->
                        let authorizationHeader = context.HttpContext.Request.Headers.["Authorization"].ToString()
                        let token = authorizationHeader.Split(" ").Last()
                        let filter = Request(Filters = sprintf "token=%s" token)                
                        let! sessionToken = this._accessTokenRepository.GetOneAndPopulateAsync filter |> Async.AwaitTask

                        match sessionToken with
                            | null ->
                                context.Result <- ForbidResult()
                            | _ ->
                                let validRole = this._roles.Any(fun outer -> sessionToken.Roles.Any(fun inner -> inner = outer))

                                match validRole with
                                    | true ->
                                        do! next.Invoke() :> Task |> Async.AwaitTask
                                    | false ->
                                        context.Result <- ForbidResult()
                        
                    | _ ->
                        do! next.Invoke() :> Task |> Async.AwaitTask
                        
            }
            |> Async.StartAsTask :> Task

type RoleAttribute(roles : string[]) =
    inherit TypeFilterAttribute(typeof<RoleFilter>)

    let inner : obj[] = [| roles |]
    do base.Arguments <- inner
