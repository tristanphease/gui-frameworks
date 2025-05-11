module BoleroApp.Client.Theme

open System
open Bolero
open Microsoft.JSInterop
open Bolero.Html
open Elmish
open Microsoft.AspNetCore.Components

type ThemeTemplate = Template<"wwwroot/theme.html">
type ThemeIconSvg = Template<"wwwroot/icons/themeicon.svg">

type Model = 
    {
        darkMode: bool;
    }

let initModel =
    {
        darkMode = true;
    }

type Message = 
    | ToggleTheme

let update message model = 
    match message with 
        | ToggleTheme -> { model with darkMode = not model.darkMode }, Cmd.none

type ThemeComponent() =
    inherit ElmishComponent<Model, Message>()

    [<Inject>]
    member val JSRuntime = Unchecked.defaultof<IJSRuntime> with get, set

    override this.View model dispatch : Node =
        ThemeTemplate()
            .ThemeIcon(
                ThemeIconSvg()
                    .MoonClass(if not model.darkMode then "opacity-30" else String.Empty)
                    .SunClass(if model.darkMode then "opacity-30" else String.Empty)
                    .Elt()
            )
            .ToggleTheme(fun _ -> this.JSRuntime.InvokeVoidAsync("setDarkMode", not model.darkMode).AsTask() |> ignore; dispatch ToggleTheme)
            .Elt()
        

    