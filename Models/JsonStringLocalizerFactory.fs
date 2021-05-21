namespace TodoApi.Models

open System
open Microsoft.Extensions.Localization

type JsonStringLocalizerFactory () =

    interface IStringLocalizerFactory with

        member this.Create(source: Type): IStringLocalizer = JsonStringLocalizer() :> IStringLocalizer

        member this.Create(baseName: string, location: string): IStringLocalizer =
            JsonStringLocalizer() :> IStringLocalizer