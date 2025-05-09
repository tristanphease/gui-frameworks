module BoleroApp.Client.Program

open Elmish
open Bolero

type Page = 
    | [<EndPoint "/">] MainPage of PageModel<Main.MainModel>

// model
type Model = 
    {
        page: Page
    }

// main message 
type Message = 
    | SetPage of Page

let update message model =
    match message with
        | SetPage page -> 
            { model with page = page }, Cmd.none

let defaultModel page = 
    match page with
        | Page.MainPage model -> Router.definePageModel model Main.initModel

// routing system
let router = Router.inferWithModel SetPage (fun model -> model.page) defaultModel

type Program = Template<"wwwroot/program.html">

let view model dispatch =
    Program()
        .Elt()

let init _ =
    {
        page = MainPage { Model = Main.initModel }
    },
    Cmd.none

type BoleroApp() =
    inherit ProgramComponent<Model, Message>()

    override _.Program =
        Program.mkProgram init update view
        |> Program.withRouter router

