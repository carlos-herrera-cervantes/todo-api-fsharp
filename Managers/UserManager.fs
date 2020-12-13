namespace TodoApi.Managers

open MongoDB.Driver
open MongoDB.Bson
open Microsoft.Extensions.Configuration
open Microsoft.AspNetCore.JsonPatch
open TodoApi.Infrastructure.Contexts
open TodoApi.Models

type UserManager private () =
  
  member val _context : IMongoCollection<User> = null with get, set
  member val _configuration : IConfiguration = null with get, set

  new (configuration : IConfiguration) as this =
        UserManager() then
        this._configuration <- configuration
        let client = MongoDBFactory.CrateClient(this._configuration.GetSection("MongoDBSettings").GetSection("ConnectionString").Value)
        let database = client.GetDatabase(this._configuration.GetSection("MongoDBSettings").GetSection("Database").Value)
        this._context <- database.GetCollection<User>("users")

  interface IUserManager with
    
    member this.CreateAsync(user: User) = this._context.InsertOneAsync user
    
    member this.DeleteByIdAsync(id: string) = this._context.DeleteOneAsync(fun entity -> entity.Id = BsonObjectId(new ObjectId(id)))
    
    member this.UpdateByIdAsync(id: string)(newUser: User)(currentUser: JsonPatchDocument<User>) =
        currentUser.ApplyTo(newUser)
        let filter = Builders<User>.Filter.Eq((fun entity -> entity.Id), BsonObjectId(new ObjectId(id)))
        let options = new ReplaceOptions(IsUpsert = true)
        this._context.ReplaceOneAsync(filter, newUser, options)