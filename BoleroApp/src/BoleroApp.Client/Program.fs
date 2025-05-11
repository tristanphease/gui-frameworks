module BoleroApp.Client.Program

open Elmish
open Bolero

type Page = 
    | [<EndPoint "/">] Main of PageModel<Main.MainModel>
    | [<EndPoint "/createquiz">] CreateQuiz of PageModel<CreateQuiz.Model>

// model
type Model = 
    {
        page: Page;
        navbar: bool
    }

// main message 
type Message = 
    | SetPage of Page
    | Main of Main.MainMessage 
    | CreateQuiz of CreateQuiz.Message

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

let defaultModel page = 
    match page with
        | Page.Main model -> Router.definePageModel model Main.initModel        
        | Page.CreateQuiz model -> Router.definePageModel model CreateQuiz.initModel

// routing system
let router = Router.inferWithModel SetPage (fun model -> model.page) defaultModel

type Program = Template<"wwwroot/program.html">

let view (model: Model) (dispatch: Dispatch<Message>) =
    Program()
        .Body(
            Html.cond model.page <| function
                | Page.Main x -> Main.view x.Model (dispatch << Message.Main)                
                | Page.CreateQuiz x -> CreateQuiz.view x.Model (dispatch << Message.CreateQuiz)
        )
        .Elt()

let init _ =
    {
        page = Page.Main { Model = Main.initModel };
        navbar = false
    },
    Cmd.none

type BoleroApp() =
    inherit ProgramComponent<Model, Message>()

    override _.Program =
        Program.mkProgram init update view
        |> Program.withRouter router

