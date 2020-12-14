namespace TodoApi.Models

open System.Collections.Generic

type Localizer () =
    member val Key : string = null with get, set
    member val LocalizedValue : Dictionary<string, string> = null with get, set