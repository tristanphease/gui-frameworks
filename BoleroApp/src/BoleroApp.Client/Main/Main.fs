module BoleroApp.Client.Main

open Elmish
open Bolero
    
type MainModel =
    new() = {}

/// The main page's update messages
type MainMessage =
    | Blank


let update (message: MainMessage) (model: MainModel) =
    match message with
        | Blank -> model, Cmd.none

let title : string =
    "Main Page"

type MainTemplate = Template<"Main/main.html">

let view (model: MainModel) (dispatch: Dispatch<MainMessage>) =
    MainTemplate()
        .Elt()

let initModel = 
    MainModel()

let init _ =
    initModel,
    Cmd.none

