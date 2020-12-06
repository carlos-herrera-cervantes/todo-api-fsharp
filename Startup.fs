namespace TodoApi

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.AspNetCore.Mvc.NewtonsoftJson
open TodoApi.Managers
open TodoApi.Repositories

type Startup private () =

    member val Configuration : IConfiguration = null with get, set

    new (configuration: IConfiguration) as this =
        Startup() then
        this.Configuration <- configuration

    member this.ConfigureServices(services: IServiceCollection) =
        services.AddControllers().AddNewtonsoftJson() |> ignore
        services.AddTransient<IUserManager, UserManager>() |> ignore
        services.AddSingleton<IConfiguration>(this.Configuration) |> ignore
        services.AddSingleton<IUserRepository, UserRepository>() |> ignore

    member this.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        if env.IsDevelopment() then
            app.UseDeveloperExceptionPage() |> ignore

        app.UseRouting() |> ignore

        app.UseEndpoints(fun endpoints -> endpoints.MapControllers() |> ignore) |> ignore