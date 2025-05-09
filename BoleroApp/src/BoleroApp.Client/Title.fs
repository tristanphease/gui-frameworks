module BoleroApp.Client.Title

open Bolero

type TitleModel = string
type TitleMessage = 
    | ChangeTitle of string

let titleView model = 
    Node.Text model

type TitleController() =
    inherit ElmishComponent<TitleModel, TitleMessage>()
    
    override _.View model dispatch =
        Node.Text model