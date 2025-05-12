module BoleroApp.Client.Program

open Elmish
open Bolero
open Microsoft.AspNetCore.Components.Web

type Page = 
    | [<EndPoint "/">] Main of PageModel<Main.MainModel>
    | [<EndPoint "/createquiz">] CreateQuiz of PageModel<CreateQuiz.Model>

// model
type Model = 
    {
        page: Page;
        navbar: bool;
        themeModel: Theme.Model;
    }

// main message 
type Message = 
    | SetPage of Page
    | Main of Main.MainMessage 
    | CreateQuiz of CreateQuiz.Message
    | ThemeMessage of Theme.Message
    | ToggleNavBar

let update (message: Message) (model: Model) =
    match message with
        | SetPage page -> 
            { model with page = page }, Cmd.none        
        | Main mainMessage -> 
            match model.page with 
                | Page.Main mainModel -> 
                    let newMainModel, mainCmd = Main.update mainMessage mainModel.Model
                    { model with page = Page.Main { Model = newMainModel }}, Cmd.map Main mainCmd
                | _ -> model, Cmd.none
        | CreateQuiz quizMessage ->
            match model.page with 
                | Page.CreateQuiz quizModel ->
                    let newQuizModel, quizCmd = CreateQuiz.update quizMessage quizModel.Model
                    { model with page = Page.CreateQuiz { Model = newQuizModel }}, Cmd.map CreateQuiz quizCmd
                | _ -> model, Cmd.none
        | ThemeMessage themeMessage -> 
            let newThemeModel, themeCmd = Theme.update themeMessage model.themeModel
            { model with themeModel = newThemeModel }, Cmd.map ThemeMessage themeCmd
        | ToggleNavBar -> { model with navbar = not model.navbar }, Cmd.none

let defaultModel page = 
    match page with
        | Page.Main model -> Router.definePageModel model Main.initModel        
        | Page.CreateQuiz model -> Router.definePageModel model CreateQuiz.initModel

// routing system
let router = Router.inferWithModel SetPage (fun model -> model.page) defaultModel

type Program = Template<"wwwroot/program.html">

let themeElementView (programModel: Model) (dispatch: Dispatch<Message>) : Node =
    Html.ecomp<Theme.ThemeComponent,_,_> 
        programModel.themeModel
        ( fun mess -> dispatch (ThemeMessage mess) )
        { Html.attr.empty() }

let pageTitle page : string =
    match page with
        | Page.Main _ -> Main.title
        | Page.CreateQuiz _ -> CreateQuiz.title

let navBarView navbar dispatch =
    Html.div {
        Html.attr.``class`` "align-self-start text-left"
        Html.button { 
            Html.attr.``class`` "blank-background text-color round-border padding-5 "
            Html.on.click (fun _ -> dispatch ToggleNavBar)
            Html.text "Nav"
        }
        if navbar then
            Html.div {
                Html.a {
                    Html.attr.``class`` "nav-circle first-nav-circle"
                    Html.attr.href "/" 
                    Html.text "Main Page"
                }
                Html.a {
                    Html.attr.``class`` "nav-circle second-nav-circle"
                    Html.attr.href "/createquiz"
                    Html.text "Create a Quiz"
                }
            }
    }


let view (model: Model) (dispatch: Dispatch<Message>) =
    Html.concat {
        Program()
            .NavBar(
                navBarView model.navbar dispatch
            )
            .Body(
                Html.cond model.page <| function
                    | Page.Main x -> Main.view x.Model (dispatch << Message.Main)                
                    | Page.CreateQuiz x -> CreateQuiz.view x.Model (dispatch << Message.CreateQuiz)
            )
            .Title(pageTitle model.page)
            .Theme(
                themeElementView model dispatch
            )
            .Elt()
        Html.comp<PageTitle> { 
            Html.text ("App - " + pageTitle model.page)
        }
    }

let init _ =
    {
        page = Page.Main { Model = Main.initModel };
        navbar = false;
        themeModel = Theme.initModel
    },
    Cmd.none

type BoleroApp() =
    inherit ProgramComponent<Model, Message>()

    override _.Program =
        Program.mkProgram init update view
        |> Program.withRouter router

