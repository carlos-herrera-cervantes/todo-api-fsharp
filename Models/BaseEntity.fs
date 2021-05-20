namespace TodoApi.Models

open System
open MongoDB.Bson
open MongoDB.Bson.Serialization.Attributes
open Newtonsoft.Json

type BaseEntity () =

    [<BsonElement("_id")>]
    [<JsonProperty("id")>]
    member val Id : BsonObjectId = null with get, set

    [<BsonElement("createdAt")>]
    [<JsonProperty("createdAt")>]
    [<BsonRepresentation(BsonType.DateTime)>]
    member val CreatedAt : DateTime = DateTime.UtcNow with get, set

    [<BsonElement("updatedAt")>]
    [<JsonProperty("updatedAt")>]
    [<BsonRepresentation(BsonType.DateTime)>]
    member val UpdatedAt : DateTime = DateTime.UtcNow with get, set