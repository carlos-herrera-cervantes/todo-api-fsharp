namespace TodoApi.Models

open System
open MongoDB.Bson
open MongoDB.Bson.Serialization.Attributes
open Newtonsoft.Json

[<AllowNullLiteral>]
type BaseEntity () =

    [<BsonElement("_id")>]
    [<JsonProperty("id")>]
    [<BsonId>]
    [<BsonRepresentation(BsonType.ObjectId)>]
    member val Id : string = null with get, set

    [<BsonElement("createdAt")>]
    [<JsonProperty("createdAt")>]
    [<BsonRepresentation(BsonType.DateTime)>]
    member val CreatedAt : DateTime = DateTime.UtcNow with get, set

    [<BsonElement("updatedAt")>]
    [<JsonProperty("updatedAt")>]
    [<BsonRepresentation(BsonType.DateTime)>]
    member val UpdatedAt : DateTime = DateTime.UtcNow with get, set