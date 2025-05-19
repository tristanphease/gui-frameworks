module BoleroApp.Client.Program

open Elmish
open Bolero
open Microsoft.AspNetCore.Components.Web
open Microsoft.AspNetCore.Components
open Microsoft.JSInterop

type Page = 
    | [<EndPoint "/">] Main
    | [<EndPoint "/createquiz">] CreateQuiz
    | [<EndPoint "/viewquiz">] ViewQuiz

// model
type Model = 
    {
        page: Page;
        mainModel: Main.MainModel;
        createQuizModel: CreateQuiz.Model;
        viewQuizModel: ViewQuiz.Model;
        navbar: bool;
        themeModel: Theme.Model;
    }

// main message 
type Message = 
    | SetPage of Page
    | Main of Main.MainMessage 
    | CreateQuiz of CreateQuiz.Message
    | ViewQuiz of ViewQuiz.Message
    | ThemeMessage of Theme.Message
    | ToggleNavBar

let update (jsRuntime: IJSRuntime) (message: Message) (model: Model) =
    match message with
        | SetPage page -> 
            { model with page = page }, Cmd.none        
        | Main mainMessage -> 
            match model.page with 
                | Page.Main -> 
                    let newMainModel, mainCmd = Main.update mainMessage model.mainModel
                    { model with mainModel = newMainModel }, Cmd.map Main mainCmd
                | _ -> model, Cmd.none
        | CreateQuiz createQuizMessage ->
            match model.page with 
                | Page.CreateQuiz ->
                    let newQuizModel, quizCmd = CreateQuiz.update jsRuntime createQuizMessage model.createQuizModel
                    { model with createQuizModel = newQuizModel }, Cmd.map CreateQuiz quizCmd
                | _ -> model, Cmd.none
        | ViewQuiz viewQuizMessage ->
            match model.page with 
                | Page.ViewQuiz ->
                    let newViewQuizModel, quizCmd = ViewQuiz.update jsRuntime viewQuizMessage model.viewQuizModel
                    { model with viewQuizModel = newViewQuizModel }, Cmd.map ViewQuiz quizCmd
                | _ -> model, Cmd.none
        | ThemeMessage themeMessage -> 
            let newThemeModel, themeCmd = Theme.update jsRuntime themeMessage model.themeModel
            { model with themeModel = newThemeModel }, Cmd.map ThemeMessage themeCmd
        | ToggleNavBar -> { model with navbar = not model.navbar }, Cmd.none

// routing system
let router = Router.infer SetPage (fun model -> model.page)

type Program = Template<"wwwroot/program.html">

let themeElementView (programModel: Model) (dispatch: Dispatch<Message>) : Node =
    Theme.view programModel.themeModel ( fun mess -> dispatch (ThemeMessage mess) )

let pageTitle page : string =
    match page with
        | Page.Main -> Main.title
        | Page.CreateQuiz -> CreateQuiz.title
        | Page.ViewQuiz -> ViewQuiz.title

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
                // make absolute so it doesn't take up space in element
                // and move things about
                Html.attr.``class`` "position-absolute z-index-1"
                Html.a {
                    Html.attr.``class`` "nav-circle first-nav-circle"
                    Html.attr.href (router.Link Page.Main )
                    Html.text "Main Page"
                }
                Html.a {
                    Html.attr.``class`` "nav-circle second-nav-circle"
                    Html.attr.href (router.Link Page.CreateQuiz )
                    Html.text "Create a Quiz"
                }
                Html.a {
                    Html.attr.``class`` "nav-circle third-nav-circle"
                    Html.attr.href (router.Link Page.ViewQuiz )
                    Html.text "View a Quiz"
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
                    | Page.Main -> Main.view model.mainModel (dispatch << Message.Main)                
                    | Page.CreateQuiz -> CreateQuiz.view model.createQuizModel (dispatch << Message.CreateQuiz)                    
                    | Page.ViewQuiz -> ViewQuiz.view model.viewQuizModel (dispatch << Message.ViewQuiz)
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
        page = Page.Main;
        mainModel = Main.initModel;
        createQuizModel = CreateQuiz.initModel;
        viewQuizModel = ViewQuiz.initModel;
        navbar = false;
        themeModel = Theme.initModel
    },
    Cmd.none

type BoleroApp() =
    inherit ProgramComponent<Model, Message>()

    [<Inject>]
    member val JSRuntime = Unchecked.defaultof<IJSRuntime> with get, set

    override this.Program =
        Program.mkProgram init (update this.JSRuntime) view
        |> Program.withRouter router

