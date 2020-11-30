namespace TodoApi.Infrastructure.Contexts

open MongoDB.Driver

[<AbstractClass; Sealed>]
type MongoDBFactory private () =
    static member CrateClient (connectionString: string) = MongoClient(connectionString)