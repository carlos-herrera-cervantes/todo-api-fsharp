namespace TodoApi.Models

open MongoDB.Bson
open MongoDB.Bson.Serialization.Attributes
open Newtonsoft.Json

[<CLIMutable>]
type Todo = {
    [<BsonElement("_id")>]
    [<JsonProperty("id")>]
    Id : BsonObjectId

    [<BsonElement("title")>]
    [<JsonProperty("title")>]
    Title : string

    [<BsonElement("description")>]
    [<JsonProperty("description")>]
    Description : string

    [<BsonElement("done")>]
    [<JsonProperty("done")>]
    Done : bool
}