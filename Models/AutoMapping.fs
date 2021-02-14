namespace TodoApi.Models

open AutoMapper

type AutoMapping() as this =
    inherit Profile()

    do this.CreateMap<User, UserDto>().PreserveReferences() |> ignore