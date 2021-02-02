namespace TodoApi

open System
open System.Collections.Generic
open System.Globalization
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.AspNetCore.Mvc.NewtonsoftJson
open Microsoft.AspNetCore.Localization
open Microsoft.Extensions.Localization
open TodoApi.Managers
open TodoApi.Repositories
open TodoApi.Models
open TodoApi.Extensions.TokenAuthentication

type Startup private () =

    member val Configuration : IConfiguration = null with get, set

    new (configuration: IConfiguration) as this =
        Startup() then
        this.Configuration <- configuration

    member this.ConfigureServices(services: IServiceCollection) =
        services.AddLocalization(fun options -> options.ResourcesPath <- "Resources") |> ignore
        services.AddControllers().AddNewtonsoftJson().AddDataAnnotationsLocalization(fun options -> 
            options.DataAnnotationLocalizerProvider <- Func<Type, IStringLocalizerFactory, IStringLocalizer>(fun _ factory -> factory.Create(typeof<SharedResources>))) |> ignore
        services.AddTokenAuthentication(this.Configuration) |> ignore
        services.AddSingleton<IUserManager, UserManager>() |> ignore
        services.AddSingleton<IConfiguration>(this.Configuration) |> ignore
        services.AddSingleton<IUserRepository, UserRepository>() |> ignore
        services.AddSingleton<ITodoRepository, TodoRepository>() |> ignore
        services.AddSingleton<ITodoManager, TodoManager>() |> ignore
        services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>() |> ignore
        services.AddSingleton<IStringLocalizer, JsonStringLocalizer>() |> ignore

    member this.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        if env.IsDevelopment() then
            app.UseDeveloperExceptionPage() |> ignore

        let cultures = new List<CultureInfo>()
        
        cultures.Add(new CultureInfo("en"))
        cultures.Add(new CultureInfo("es"))

        app.UseRequestLocalization(Action<RequestLocalizationOptions>(fun options ->
            options.DefaultRequestCulture <- new RequestCulture("en")
            options.SupportedCultures <- cultures
            options.SupportedUICultures <- cultures)) |> ignore
        app.UseAuthentication() |> ignore
        app.UseRouting() |> ignore
        app.UseAuthorization() |> ignore
        app.UseEndpoints(fun endpoints -> endpoints.MapControllers() |> ignore) |> ignore