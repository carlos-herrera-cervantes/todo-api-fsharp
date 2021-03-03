namespace TodoApi.Managers

open MongoDB.Driver
open MongoDB.Bson
open Microsoft.AspNetCore.JsonPatch
open TodoApi.Models

type UserManager private () =
  
  member val _userManager : IManager<User> = null with get, set

  new (userManager : IManager<User>) as this = UserManager() then
    this._userManager <- userManager

  interface IUserManager with

    /// <summary>Creates a new user document</summary>
    /// <param name="user">User object class</param>
    /// <returns>User</returns>
    member this.CreateAsync(user: User) = this._userManager.CreateAsync user

    /// <summary>Deletes a document</summary>
    /// <param name="id">Document ID</param>
    /// <returns>Removal result</returns>
    member this.DeleteByIdAsync(id: string) =
      this._userManager.DeleteByIdAsync(fun entity -> entity.Id = BsonObjectId(new ObjectId(id)))

    /// <summary>Updates a user</summary>
    /// <param name="id">User ID</param>
    /// <param name="newUser">New user values</param>
    /// <param name="currentUser">Current user values</param>
    /// <returns>User</returns>
    member this.UpdateByIdAsync(id: string)(newUser: User)(currentUser: JsonPatchDocument<User>) =
        currentUser.ApplyTo(newUser)
        let filter = Builders<User>.Filter.Eq((fun entity -> entity.Id), BsonObjectId(new ObjectId(id)))
        let options = new ReplaceOptions(IsUpsert = true)
        this._userManager.UpdateOneAsync(filter)(newUser)(options)