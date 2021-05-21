namespace TodoApi.Models

open System.Collections.Generic
open System.ComponentModel.DataAnnotations
open MongoDB.Bson.Serialization.Attributes
open Newtonsoft.Json
open TodoApi.Constants

[<AllowNullLiteral>]
type User () =
    inherit BaseEntity()

    static let relations = new List<Relation>()

    static do
        relations.Add(Relation(Entity = "todos", LocalKey = "_id", ForeignKey = "user", JustOne = false))

    [<BsonElement("email")>]
    member val Email : string = null with get, set

    [<BsonElement("name")>]
    member val Name : string = null with get, set

    [<BsonElement("password")>]
    member val Password : string = null with get, set

    [<BsonElement("roles")>]
    member val Roles : string[] = [| Roles.Client |] with get, set

    [<BsonIgnoreIfNull>]
    member val TodosEmbedded : List<Todo> = null with get, set

    [<BsonIgnore>]
    static member val Relations : List<Relation> = relations with get

[<AllowNullLiteral>]
type CreateUserDto () =
    inherit BaseEntity()

    [<BsonElement("email")>]
    [<JsonProperty("email")>]
    [<Required(ErrorMessage = "EmailRequired")>]
    [<DataType(DataType.EmailAddress, ErrorMessage = "EmailFormatInvalid")>]
    member val Email : string = null with get, set

    [<BsonElement("name")>]
    [<JsonProperty("name")>]
    [<Required(ErrorMessage = "NameRequired")>]
    member val Name : string = null with get, set

    [<BsonElement("password")>]
    [<JsonProperty("password")>]
    [<Required(ErrorMessage = "PasswordRequired")>]
    member val Password : string = null with get, set

    [<BsonElement("roles")>]
    [<JsonProperty("roles")>]
    member val Roles : string[] = [| Roles.Client |] with get, set

[<AllowNullLiteral>]
type SingleUserDto () =
    inherit BaseEntity()

    [<JsonProperty("email")>]
    member val Email : string = null with get, set

    [<JsonProperty("name")>]
    member val Name : string = null with get, set

    [<JsonProperty("todosEmbedded", DefaultValueHandling = DefaultValueHandling.Ignore)>]
    member val TodosEmbedded : List<Todo> = null with get, set

type SuccessListUserResponse () =
    inherit SuccessPagerResponse()

    [<JsonProperty("data")>]
    member val Data : List<SingleUserDto> = null with get, set

type SuccessUserResponse () =
    inherit BaseResponse()

    [<JsonProperty("data")>]
    member val Data : SingleUserDto = null with get, set