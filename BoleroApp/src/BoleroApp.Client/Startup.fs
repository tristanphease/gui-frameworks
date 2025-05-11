namespace BoleroApp.Client

open Microsoft.AspNetCore.Components.WebAssembly.Hosting

module Startup =
    open Microsoft.AspNetCore.Components.Web

    [<EntryPoint>]
    let Main args =
        let builder = WebAssemblyHostBuilder.CreateDefault args
        builder.RootComponents.Add<Program.BoleroApp> "#program"
        builder.RootComponents.Add<HeadOutlet> "head::after"
        builder.Build().RunAsync() |> ignore
        0
