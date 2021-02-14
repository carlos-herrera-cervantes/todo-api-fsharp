namespace TodoApi.Extensions

open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open AutoMapper
open TodoApi.Models

module AutoMapperExtensions =

    type IServiceCollection with
        
        member this.AddAutoMapperConfiguration(configuration : IConfiguration) =
            let mapperConfig = new MapperConfiguration(fun mc -> mc.AddProfile(new AutoMapping()))
            let mapper = mapperConfig.CreateMapper()

            this.AddSingleton(mapper) |> ignore
            this