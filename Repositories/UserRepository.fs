namespace TodoApi.Repositories

open MongoDB.Driver
open TodoApi.Models

type UserRepository private () =
  
  member val _userRepository: IRepository<User> = null with get, set

  new (userRepository : IRepository<User>) as this = UserRepository() then
    this._userRepository <- userRepository

  interface IUserRepository with

    /// <summary>Get list of users</summary>
    /// <param name="request">Request object model</param>
    /// <returns>List of all users</returns>
    member this.GetAllAsync(request : Request) = this._userRepository.GetAllAsync(request)(User.Relations)

    /// <summary>Get a user by specific ID</summary>
    /// <param name="id">User ID</param>
    /// <returns>Specific user</returns>
    member this.GetByIdAsync(id: string) =
      this._userRepository.GetByIdAsync(fun entity -> entity.Id = id)

    /// <summary>Get a user by specific filter and their references</summary>
    /// <param name="request">Request object model</param>
    /// <returns>Specific user</returns>
    member this.GetOneAndPopulateAsync(request : Request) =
      this._userRepository.GetOneAndPopulateAsync(request)(User.Relations)

    /// <summary>Get one user by specific filter</summary>
    /// <param name="filter">A filter definition with fields to match with a document</param>
    /// <returns>Specific user</returns>
    member this.GetOneAsync(filter : FilterDefinition<User>) = this._userRepository.GetOneAsync(filter)

    /// <summary>Returns the number of documents in users collection</summary>
    /// <param name="request">Request object model</param>
    /// <returns>Number of documents</returns>
    member this.CountAsync(request : Request) = this._userRepository.CountAsync(request)

    /// <summary>Returns the number of documents in users collection</summary>
    /// <returns>Number of documents</returns>
    member this.CountAsync() = this._userRepository.CountAsync()