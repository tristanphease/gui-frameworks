module BoleroApp.Client.Main

open Elmish
open Bolero
    
type MainModel =
    new() = {}

/// The main page's update messages
type MainMessage =
    | Blank


// type Title = Template<"""<title id="title">${Title}</title>""">

let update (message: MainMessage) (model: MainModel) =
    match message with
        | Blank -> model, Cmd.none

let view (model: MainModel) (dispatch: Dispatch<MainMessage>) =
    Node.Text "This is the main page!"

let initModel = MainModel()

let init _ =
    initModel,
    Cmd.none

