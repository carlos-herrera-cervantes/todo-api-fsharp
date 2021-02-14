namespace TodoApi.Models

open System
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

    [<BsonElement("password")>]
    [<JsonProperty("password")>]
    Password : string

    [<BsonElement("createdAt")>]
    [<JsonProperty("createdAt")>]
    [<BsonRepresentation(BsonType.DateTime)>]
    CreatedAt : DateTime

    [<BsonElement("updatedAt")>]
    [<JsonProperty("updatedAt")>]
    [<BsonRepresentation(BsonType.DateTime)>]
    UpdatedAt : DateTime
}

type UserDto = {
    [<JsonProperty("id")>]
    Id : BsonObjectId

    [<JsonProperty("email")>]
    Email : string

    [<JsonProperty("name")>]
    Name : string

    [<JsonProperty("createdAt")>]
    CreatedAt : DateTime

    [<JsonProperty("updatedAt")>]
    UpdatedAt : DateTime
}

module User =
    
    /// <summary>Set the minimum values to User object using smart constructor</summary>
    /// <param name="user">User object</param>
    /// <returns>User object</returns>
    let setValuesToUser (user : User) =
        {
            Id = BsonObjectId(new ObjectId())
            Email = user.Email
            Name = user.Name
            Password = user.Password
            CreatedAt = DateTime.UtcNow
            UpdatedAt = DateTime.UtcNow
        }