namespace TodoApi.Models

open AutoMapper
open System.Collections.Generic

type AutoMapping() as this =
    inherit Profile()

    do this.CreateMap<User, UserDto>().PreserveReferences() |> ignore