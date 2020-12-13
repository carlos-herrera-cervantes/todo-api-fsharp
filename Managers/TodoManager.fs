namespace TodoApi.Managers

open MongoDB.Driver
open MongoDB.Bson
open Microsoft.Extensions.Configuration
open Microsoft.AspNetCore.JsonPatch
open TodoApi.Infrastructure.Contexts
open TodoApi.Models

type TodoManager private () =

    member val _context : IMongoCollection<Todo> = null with get, set
    member val _configuration : IConfiguration = null with get, set

    new (configuration : IConfiguration) as this =
        TodoManager() then
        this._configuration <- configuration
        let client = MongoDBFactory.CrateClient(this._configuration.GetSection("MongoDBSettings").GetSection("ConnectionString").Value)
        let database = client.GetDatabase(this._configuration.GetSection("MongoDBSettings").GetSection("Database").Value)
        this._context <- database.GetCollection<Todo>("todos")

    interface ITodoManager with
        
        member this.CreateAsync(todo: Todo) = this._context.InsertOneAsync todo
        
        member this.DeleteByIdAsync(id: string) = this._context.DeleteOneAsync(fun entity -> entity.Id = BsonObjectId(new ObjectId(id)))
        
        member this.UpdateByIdAsync(id: string)(newTodo: Todo)(currentTodo: JsonPatchDocument<Todo>) =
            currentTodo.ApplyTo(newTodo)
            let filter = Builders<Todo>.Filter.Eq((fun entity -> entity.Id), BsonObjectId(new ObjectId(id)))
            let options = new ReplaceOptions(IsUpsert = true)
            this._context.ReplaceOneAsync(filter, newTodo, options)
