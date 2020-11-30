namespace TodoApi.Managers

open MongoDB.Driver
open Microsoft.Extensions.Configuration
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
    member this.CreateAsync(user: User) = async { this._context.InsertOneAsync user |> ignore }