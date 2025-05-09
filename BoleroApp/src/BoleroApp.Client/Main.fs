module BoleroApp.Client.Main

open Elmish
open Bolero
    
type MainModel =
    new() = {}

/// The main page's update messages
type MainMessage =
    | Blank


// type Title = Template<"""<title id="title">${Title}</title>""">



let view model dispatch =
    Node.Text "test"

let initModel = MainModel()

let init _ =
    initModel,
    Cmd.none

