namespace TodoApi.Repositories

open MongoDB.Driver
open MongoDB.Bson
open Microsoft.Extensions.Configuration
open TodoApi.Infrastructure.Contexts
open TodoApi.Models

type UserRepository private () =
  
  member val _context : IMongoCollection<User> = null with get, set
  member val _configuration : IConfiguration = null with get, set

  new (configuration : IConfiguration) as this =
        UserRepository() then
        this._configuration <- configuration
        let client = MongoDBFactory.CrateClient(this._configuration.GetSection("MongoDBSettings").GetSection("ConnectionString").Value)
        let database = client.GetDatabase(this._configuration.GetSection("MongoDBSettings").GetSection("Database").Value)
        this._context <- database.GetCollection<User>("users")

  interface IUserRepository with

    /// <summary>Get list of users</summary>
    /// <returns>List of all users</returns>
    member this.GetAllAsync() = this._context.Find(fun entity -> true).ToListAsync()

    /// <summary>Get a user by specific ID</summary>
    /// <param name="id">User ID</param>
    /// <returns>Specific user</returns>
    member this.GetByIdAsync(id: string) = this._context.Find(fun entity -> entity.Id = BsonObjectId(new ObjectId(id))).FirstOrDefaultAsync()

    /// <summary>Get one user by specific filter</summary>
    /// <param name="filter">A filter definition with fields to match with a document</summary>
    /// <returns>Specific user</returns>
    member this.GetOneAsync(filter : FilterDefinition<User>) = this._context.Find(filter).FirstOrDefaultAsync()