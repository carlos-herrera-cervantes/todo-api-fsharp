namespace TodoApi.Models

open AutoMapper

type AutoMapping() as this =
    inherit Profile()

    do
        this.CreateMap<User, SingleUserDto>().PreserveReferences() |> ignore
        this.CreateMap<User, CreateUserDto>().PreserveReferences() |> ignore
        this.CreateMap<CreateUserDto, SingleUserDto>() |> ignore
        this.CreateMap<CreateUserDto, User>() |> ignore