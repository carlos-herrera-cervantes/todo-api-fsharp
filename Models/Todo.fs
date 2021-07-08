namespace TodoApi.Models

open System.Collections.Generic
open System.ComponentModel.DataAnnotations
open MongoDB.Bson
open MongoDB.Bson.Serialization.Attributes
open Newtonsoft.Json

[<AllowNullLiteral>]
type Todo () =
    inherit BaseEntity()

    [<BsonElement("title")>]
    [<JsonProperty("title")>]
    [<Required(ErrorMessage = "TitleRequired")>]
    member val Title : string = null with get, set

    [<BsonElement("description")>]
    [<JsonProperty("description")>]
    [<Required(ErrorMessage = "DescriptionRequired")>]
    member val Description : string = null with get, set

    [<BsonElement("done")>]
    [<JsonProperty("done")>]
    member val Done : bool = false with get, set

    [<BsonElement("user")>]
    [<BsonRepresentation(BsonType.ObjectId)>]
    [<JsonProperty("user")>]
    [<Required(ErrorMessage = "UserRequired")>]
    [<RegularExpression(@"^[0-9a-fA-F]{24}$", ErrorMessage = "InvalidObjectId")>]
    member val User : string = null with get, set

type SuccessListTodoResponse () =
    inherit SuccessPagerResponse()

    [<JsonProperty("data")>]
    member val Data : List<Todo> = null with get, set

type SuccessTodoResponse () =
    inherit BaseResponse()

    [<JsonProperty("data")>]
    member val Data : Todo = null with get, set