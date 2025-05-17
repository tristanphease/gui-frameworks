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

let update (jsRunTime: IJSRuntime) message model = 
    match message with 
        | ToggleTheme -> 
            let newDarkMode = not model.darkMode
            jsRunTime.InvokeVoidAsync("setDarkMode", not model.darkMode).AsTask() |> ignore
            { model with darkMode = newDarkMode }, Cmd.none

let view model dispatch : Node =
    ThemeTemplate()
        .ThemeIcon(
            ThemeIconSvg()
                .MoonClass(if not model.darkMode then "opacity-30" else String.Empty)
                .SunClass(if model.darkMode then "opacity-30" else String.Empty)
                .Elt()
        )
        .ToggleTheme(fun _ -> dispatch ToggleTheme)
        .Elt()
        

    