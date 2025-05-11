namespace BoleroApp.Client

open Microsoft.AspNetCore.Components.WebAssembly.Hosting
open Microsoft.Extensions.DependencyInjection
open System
open System.Net.Http

module Startup =

    [<EntryPoint>]
    let Main args =
        let builder = WebAssemblyHostBuilder.CreateDefault args
        builder.RootComponents.Add<Program.BoleroApp> "#program"
        builder.Build().RunAsync() |> ignore
        0
