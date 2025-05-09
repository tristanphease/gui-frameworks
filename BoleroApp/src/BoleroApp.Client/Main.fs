module BoleroApp.Client.Main

open System
open System.Net.Http
open System.Net.Http.Json
open Microsoft.AspNetCore.Components
open Elmish
open Bolero
open Bolero.Html

/// Routing endpoints definition
type Page =
    | [<EndPoint "/">] Index

/// The model for the application
type Model =
    {
        page: Page
        pageModel: PageModel
    }

and PageModel =
    | MainModel
    


/// The application's update messages
type Message =
    | SetPage of Page

let update message model =
    match message with
    | SetPage page ->
        { model with page = page }, Cmd.none

/// Connects the routing system to the Elmish application.
let router = Router.infer SetPage (fun model -> model.page)

type Title = Template<"""<title id="title">${Title}</title>""">

type Main = Template<"wwwroot/main.html">

let view model dispatch =
    Main()
        .Elt()

let init _ =
    {
        page = Index
        pageModel = MainModel
    },
    Cmd.none

type BoleroApp() =
    inherit ProgramComponent<Model, Message>()

    override _.Program =
        Program.mkProgram init update view
        |> Program.withRouter router
