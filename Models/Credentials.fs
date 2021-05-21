namespace TodoApi.Models

open Newtonsoft.Json
open System.ComponentModel.DataAnnotations

type Credentials () =

    [<JsonProperty("email")>]
    [<Required(ErrorMessage = "EmailRequired")>]
    [<DataType(DataType.EmailAddress, ErrorMessage = "EmailFormatInvalid")>]
    member val Email : string = null with get, set

    [<JsonProperty("password")>]
    [<Required(ErrorMessage = "PasswordRequired")>]
    member val Password : string = null with get, set