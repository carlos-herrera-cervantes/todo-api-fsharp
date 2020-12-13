namespace TodoApi.Repositories

open MongoDB.Driver
open MongoDB.Bson
open Microsoft.Extensions.Configuration
open TodoApi.Infrastructure.Contexts
open TodoApi.Models

type TodoRepository private () =

    member val _context : IMongoCollection<Todo> = null with get, set
    member val _configuration : IConfiguration = null with get, set

    new (configuration : IConfiguration) as this =
        TodoRepository() then
        this._configuration <- configuration
        let client = MongoDBFactory.CrateClient(this._configuration.GetSection("MongoDBSettings").GetSection("ConnectionString").Value)
        let database = client.GetDatabase(this._configuration.GetSection("MongoDBSettings").GetSection("Database").Value)
        this._context <- database.GetCollection<Todo>("todos")

    interface ITodoRepository with
        member this.GetAllAsync() = this._context.Find(fun entity -> true).ToListAsync()
        member this.GetByIdAsync(id: string) = this._context.Find(fun entity -> entity.Id = BsonObjectId(new ObjectId(id))).FirstOrDefaultAsync()