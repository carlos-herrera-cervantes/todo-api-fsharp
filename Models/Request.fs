namespace TodoApi.Models

open Microsoft.AspNetCore.Mvc

type Request = {
    [<FromQuery(Name = "sort")>]
    Sort : string

    [<FromQuery(Name = "pageSize")>]
    PageSize : int

    [<FromQuery(Name = "page")>]
    Page : int

    [<FromQuery(Name = "paginate")>]
    Paginate : bool

    [<FromQuery(Name = "relation")>]
    Entities : string[]

    [<FromQuery(Name = "filter")>]
    Filters : string[]
}