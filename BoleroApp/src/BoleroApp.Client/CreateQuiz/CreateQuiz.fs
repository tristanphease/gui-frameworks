module BoleroApp.Client.CreateQuiz

open System
open Elmish
open Bolero
open Quiz
open Microsoft.JSInterop


/// The model for the create quiz page
type Model =
    {
        questions: list<QuizQuestion>;
        currentQuestion: QuizQuestion;
        errorMessage: option<string>;
    }

type ErrorMessage =
    | NotEnoughOptions
    | EmptyQuestion
    | ClearExisting

let errorMessageToString (error: ErrorMessage) : string =
    match error with 
        | NotEnoughOptions -> "Need to have at least two options"        
        | EmptyQuestion -> "Question is empty"
        | ClearExisting -> "Need to clear existing question first"

/// A message for updating the model for the create quiz page
type Message =
    | AddCurrentQuestion
    | SetCurrentQuestion of string
    | AddOption
    | SetOption of index : int * value : string
    | DeleteOption of index : int 
    | SetCorrectOption of index : int
    | SetError of ErrorMessage
    | ClearError
    | EditExistingQuestion of index : int 
    | DeleteExistingQuestion of index : int
    | SaveQuizFile
    | LoadQuizFromFile
    | LoadFileMessage of File.FileMessage

/// Create a new blank question
let newQuestion : QuizQuestion = 
    {
        question = String.Empty;
        options = [String.Empty];
        correctAnswer = 0;
    }

/// Initiate the model for the create quiz page
let initModel : Model = 
    {
        questions = list.Empty;
        currentQuestion = newQuestion;
        errorMessage = None
    }

/// Init model and message
let init _ = 
    initModel,
    Cmd.none

/// Add the current question to the existing questions
let addCurrentQuestion (model: Model) : Model * Cmd<Message>  = 
    if model.currentQuestion.options.Length < 2
    then model, Cmd.ofMsg (SetError NotEnoughOptions)
    else if model.currentQuestion.question = String.Empty 
    then model, Cmd.ofMsg (SetError EmptyQuestion) 
    else { model with questions = model.questions @ [model.currentQuestion]; currentQuestion = newQuestion}, Cmd.ofMsg ClearError

let isQuestionBlank (question: QuizQuestion) : bool =
    question.question = String.Empty 
        && not (List.exists (fun n -> not(n = String.Empty)) question.options)

let setExistingQuestion (index: int) (model: Model) : Model * Cmd<Message> =
    if not (isQuestionBlank model.currentQuestion) then
        model, Cmd.ofMsg (SetError ClearExisting)
    else 
        { model with currentQuestion = List.item index model.questions; questions = Util.deleteIndex index model.questions}, Cmd.none

let newIndex currentIndex deletedIndex =
    if currentIndex = deletedIndex && currentIndex > 0 || currentIndex > deletedIndex
    then currentIndex - 1
    else currentIndex

/// Update for message
let update (jsRuntime: IJSRuntime) (message: Message) (model: Model) : Model * Cmd<Message> =
    match message with 
        | AddCurrentQuestion -> 
            addCurrentQuestion model
        | SetCurrentQuestion newQuestion -> 
            { model with currentQuestion.question = newQuestion }, Cmd.none
        | AddOption -> 
            { model with currentQuestion.options = model.currentQuestion.options @ [String.Empty] }, Cmd.none
        | SetOption (index, newValue) -> 
            { model with currentQuestion.options = model.currentQuestion.options |> Util.modifyIndex index newValue }, Cmd.none        
        | DeleteOption index -> 
            { model with currentQuestion = { model.currentQuestion with options = Util.deleteIndex index model.currentQuestion.options; correctAnswer = newIndex model.currentQuestion.correctAnswer index } }, Cmd.none 
        | SetError error -> 
            { model with errorMessage = Some (errorMessageToString error) }, Cmd.none
        | ClearError -> 
            { model with errorMessage = None}, Cmd.none        
        | EditExistingQuestion index -> 
            setExistingQuestion index model
        | DeleteExistingQuestion index -> 
            { model with questions = Util.deleteIndex index model.questions }, Cmd.none
        | SaveQuizFile -> 
            let quizJson = Quiz.quizToJson model.questions
            File.saveFile jsRuntime quizJson 
            model, Cmd.none
        | SetCorrectOption index -> 
            { model with currentQuestion.correctAnswer = index}, Cmd.none
        | LoadQuizFromFile -> 
            model, Cmd.map LoadFileMessage (File.loadFile jsRuntime)        
        | LoadFileMessage (File.ReturnedFileInfo quizJson) -> 
            let questions = Quiz.quizFromJson quizJson
            { model with questions = questions }, Cmd.none
        | LoadFileMessage File.ErrorLoading ->
            { model with errorMessage = Some "Couldn't load file" }, Cmd.none
            

let title : string =
    "Create Quiz"

type CreateQuiz = Template<"CreateQuiz/createquiz.html">

/// View for a current question option
let viewCurrentQuestionOption (dispatch: Dispatch<Message>) (correctIndex: int) (index: int, option: string)  =
    CreateQuiz.CurrentQuestionOption()
        .Option(option, fun newValue -> dispatch (SetOption(index, newValue)))
        .OptionClass(if correctIndex = index then "correct-background" else String.Empty)
        .OptionObjectClass(if correctIndex = index then "opacity-80" else String.Empty)
        .DeleteOption(fun _ -> dispatch (DeleteOption index))
        .SetCorrect(fun _ -> dispatch (SetCorrectOption index))
        .Elt()

/// View for an existing question option
let viewQuestionOption (option: string) : Node =
    CreateQuiz.QuestionOption()
        .Option(option)
        .Elt()

/// View an existing question
let viewExistingQuestion (dispatch: Dispatch<Message>) (index: int, quizQuestion: QuizQuestion) : Node =
    CreateQuiz.ExistingQuestion()
        .Question(quizQuestion.question)
        .QuestionOptions(Html.forEach quizQuestion.options viewQuestionOption)
        .EditQuestion(fun _ -> dispatch (EditExistingQuestion index))
        .DeleteQuestion(fun _ -> dispatch (DeleteExistingQuestion index))
        .Elt()

let viewAllExistingQuestions dispatch questions = 
    if List.isEmpty questions 
    then Node.Text "No questions"
    else Html.forEach (List.indexed questions) (viewExistingQuestion dispatch)

let view (model: Model) (dispatch: Dispatch<Message>) : Node =
    CreateQuiz()
        .CurrentQuestion(model.currentQuestion.question, fun n -> dispatch (SetCurrentQuestion n))
        .CurrentQuestionOptions(Html.forEach (List.indexed model.currentQuestion.options) (viewCurrentQuestionOption dispatch model.currentQuestion.correctAnswer))
        .AddCurrentQuestion(fun _ -> dispatch AddCurrentQuestion)
        .AddOption(fun _ -> dispatch AddOption)
        .ExistingQuestions(viewAllExistingQuestions dispatch model.questions)
        .ErrorMessage(Option.defaultValue String.Empty model.errorMessage)
        .ErrorMessageClass(if model.errorMessage.IsSome then String.Empty else "hidden")
        .SaveQuizFile(fun _ -> dispatch SaveQuizFile)
        .LoadQuizFile(fun _ -> dispatch LoadQuizFromFile)
        .Elt()

