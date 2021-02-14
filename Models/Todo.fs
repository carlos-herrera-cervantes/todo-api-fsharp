namespace TodoApi.Models

open System
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

    [<BsonElement("createdAt")>]
    [<JsonProperty("createdAt")>]
    [<BsonRepresentation(BsonType.DateTime)>]
    CreatedAt : DateTime

    [<BsonElement("updatedAt")>]
    [<JsonProperty("updatedAt")>]
    [<BsonRepresentation(BsonType.DateTime)>]
    UpdatedAt : DateTime
}

module Todo =

    /// <summary>Set the minimum values to ToDo object using smart constructor</summary>
    /// <param name="todo">ToDo object</param>
    /// <returns>ToDo object</returns>
    let setValuesToTodo todo =
        {
            Id = BsonObjectId(new ObjectId())
            Title = todo.Title
            Description = todo.Description
            Done = false
            CreatedAt = DateTime.UtcNow
            UpdatedAt = DateTime.UtcNow
        }