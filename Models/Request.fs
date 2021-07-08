namespace TodoApi.Models

open Microsoft.AspNetCore.Mvc

[<AllowNullLiteral>]
type Request () =

    [<FromQuery(Name = "sort")>]
    member val Sort : string = "" with get, set

    [<FromQuery(Name = "pageSize")>]
    member val PageSize : int = 10 with get, set

    [<FromQuery(Name = "page")>]
    member val Page : int = 0 with get, set

    [<FromQuery(Name = "relation")>]
    member val Entities : string = "" with get, set

    [<FromQuery(Name = "filter")>]
    member val Filters : string = "" with get, set