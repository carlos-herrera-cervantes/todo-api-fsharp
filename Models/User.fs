namespace TodoApi.Models

open System
open System.Collections.Generic
open System.ComponentModel.DataAnnotations
open MongoDB.Bson
open MongoDB.Bson.Serialization.Attributes
open Newtonsoft.Json

type User () =
    inherit BaseEntity()

    static let relations = new List<Relation>()

    static do
        relations.Add(new Relation(Entity = "todos", LocalKey = "_id", ForeignKey = "user", JustOne = false))

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

    [<BsonIgnoreIfNull>]
    [<JsonProperty("todosEmbedded", DefaultValueHandling = DefaultValueHandling.Ignore)>]
    member val TodosEmbedded : List<Todo> = null with get, set

    [<BsonIgnore>]
    [<JsonIgnore>]
    static member val Relations : List<Relation> = relations with get

type UserDto = {
    [<JsonProperty("id")>]
    Id : BsonObjectId

    [<JsonProperty("email")>]
    Email : string

    [<JsonProperty("name")>]
    Name : string

    [<JsonProperty("todosEmbedded")>]
    TodosEmbedded : List<Todo>

    [<JsonProperty("createdAt")>]
    CreatedAt : DateTime

    [<JsonProperty("updatedAt")>]
    UpdatedAt : DateTime
}