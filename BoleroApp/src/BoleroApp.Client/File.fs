module BoleroApp.Client.File

open Microsoft.JSInterop
open Elmish
open Bolero
open Bolero.Remoting.Client

let saveFile (jsRunTime: IJSRuntime) (fileString: string) =
    jsRunTime.InvokeVoidAsync("saveFile", fileString, "quiz.json" ) |> ignore


type FileMessage = 
    | ReturnedFileInfo of string
    | ErrorLoading


let loadFile (jsRunTime: IJSRuntime) = 
    Cmd.OfJS.either jsRunTime "loadFile" [| |] (fun file -> ReturnedFileInfo file) (fun _ -> ErrorLoading)

    

    
