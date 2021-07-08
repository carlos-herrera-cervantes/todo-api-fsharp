namespace TodoApi.Hooks

open System
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open TodoApi.Managers
open TodoApi.Repositories
open TodoApi.Models

[<AbstractClass; Sealed>]
type Seeder private () =

    static member Initialize (serviceProvider : IServiceProvider) =
        async {
            use scope = serviceProvider.CreateScope()

            scope.ServiceProvider.GetRequiredService<IManager<User>>() |> ignore
            scope.ServiceProvider.GetRequiredService<IRepository<User>>() |> ignore

            let userManager = scope.ServiceProvider.GetRequiredService<IUserManager>()
            let userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>()
            let logger = scope.ServiceProvider.GetRequiredService<ILogger<Seeder>>()

            let! totalDocs = userRepository.CountAsync() |> Async.AwaitTask

            if (int(totalDocs) = 0) then
                let user = User()
                user.Email <- "super.admin@mytransformation.com"
                user.Name <- "Super User"
                user.Roles <- [| "SuperAdmin" |]
                user.Password <- "super.user"

                do! userManager.CreateAsync(user) |> Async.AwaitTask

                logger.LogInformation("Basic users created")

                return true
            else
                logger.LogInformation("Basic users have already been created")
                return false
        }