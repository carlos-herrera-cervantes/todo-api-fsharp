namespace TodoApi.Models

open MongoDB.Bson
open MongoDB.Bson.Serialization.Attributes
open Newtonsoft.Json

[<CLIMutable>]
type User = {
    [<BsonElement("_id")>]
    [<JsonProperty("id")>]
    Id : BsonObjectId

    [<BsonElement("email")>]
    [<JsonProperty("email")>]
    Email : string

    [<BsonElement("name")>]
    [<JsonProperty("name")>]
    Name : string
}