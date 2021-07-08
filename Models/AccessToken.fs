namespace TodoApi.Models

open MongoDB.Bson.Serialization.Attributes
open MongoDB.Bson
open System.ComponentModel.DataAnnotations
open Newtonsoft.Json

[<AllowNullLiteral>]
type AccessToken () =
    inherit BaseEntity()

    [<BsonElement("token")>]
    [<Required(ErrorMessage = "TokenRequired")>]
    member val Token : string = null with get, set

    [<BsonElement("user")>]
    [<BsonRepresentation(BsonType.ObjectId)>]
    [<Required(ErrorMessage = "UserRequired")>]
    member val User : string = null with get, set

    [<BsonElement("email")>]
    [<Required(ErrorMessage = "EmailRequired")>]
    [<DataType(DataType.EmailAddress, ErrorMessage = "EmailFormatInvalid")>]
    member val Email : string = null with get, set

    [<BsonElement("roles")>]
    member val Roles : string[] = Array.empty with get, set

    [<BsonIgnoreIfNull>]
    member val UserEmbedded : User = null with get, set

type SuccessAuthResponse () =
    inherit BaseResponse()

    [<JsonProperty("data")>]
    member val Data : string = null with get, set