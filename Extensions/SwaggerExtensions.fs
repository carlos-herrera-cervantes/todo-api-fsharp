namespace TodoApi.Extensions

open System
open Microsoft.Extensions.DependencyInjection
open Microsoft.OpenApi.Models
open Swashbuckle.AspNetCore.SwaggerGen

module SwaggerExtensions =

    type IServiceCollection with

        /// <summary>Setup the title and description for API</summary>
        /// <returns>Service collection</returns>
        member this.ConfigSwagger() =
            let info = OpenApiInfo()
            info.Title <- "Todo API F#"
            info.Description <- "Documentation for Todo API component"

            this.AddSwaggerGen(Action<SwaggerGenOptions>(fun c -> 
                c.SwaggerDoc("v1", info))) |> ignore

            this