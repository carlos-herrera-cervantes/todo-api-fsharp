namespace TodoApi.Models

open Newtonsoft.Json

type BaseResponse () =

    [<JsonProperty("status")>]
    member val Status : bool = true with get, set

type SuccessPagerResponse () =
    inherit BaseResponse()

    [<JsonProperty("paginator")>]
    member val Paginator : Paginator = null with get, set

type FailResponse () =
    inherit BaseResponse()

    [<JsonProperty("message")>]
    member val Message : string = null with get, set

    [<JsonProperty("code")>]
    member val Code : string = null with get, set