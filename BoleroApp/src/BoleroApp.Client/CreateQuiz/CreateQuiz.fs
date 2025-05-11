module CreateQuiz

open System
open Elmish
open Bolero

/// A quiz question
type QuizQuestion = 
    {
        question: string;
        options: list<string>
    }

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
    | SetError of ErrorMessage
    | ClearError
    | EditExistingQuestion of index : int 
    | DeleteExistingQuestion of index : int
    | SaveQuizFile

/// Create a new blank question
let newQuestion : QuizQuestion = 
    {
        question = String.Empty;
        options = [String.Empty]
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

/// Function that takes in an index and new value and returns the list with the new value at the index
let modifyIndex (index: int) (newValue: 'a) : (list<'a> -> list<'a>) =
    List.mapi (fun i x -> if i = index then newValue else x) 

/// Function that takes in an index and a list and will return a new list without the element at that index
let deleteIndex (index: int) : (list<'a> -> list<'a>) =
    List.mapi (fun i x -> if i = index then None else Some x)
        >> List.choose id

/// Add the current question to the existing questions
let addCurrentQuestion (model: Model) : Model * Cmd<Message>  = 
    if model.currentQuestion.options.Length < 2
    then model, Cmd.ofMsg (SetError NotEnoughOptions)
    else if model.currentQuestion.question = String.Empty 
    then model, Cmd.ofMsg (SetError EmptyQuestion) 
    else { model with questions = model.currentQuestion :: model.questions; currentQuestion = newQuestion}, Cmd.ofMsg ClearError

let isQuestionBlank (question: QuizQuestion) : bool =
    question.question = String.Empty 
        && not (List.exists (fun n -> not(n = String.Empty)) question.options)

let setExistingQuestion (index: int) (model: Model) : Model * Cmd<Message> =
    if not (isQuestionBlank model.currentQuestion) then
        model, Cmd.ofMsg (SetError ClearExisting)
    else 
        { model with currentQuestion = List.item index model.questions; questions = deleteIndex index model.questions}, Cmd.none


/// Update for message
let update (message: Message) (model: Model) : Model * Cmd<Message> =
    match message with 
        | AddCurrentQuestion -> addCurrentQuestion model
        | SetCurrentQuestion newQuestion -> { model with currentQuestion.question = newQuestion }, Cmd.none
        | AddOption -> { model with currentQuestion.options = model.currentQuestion.options @ [String.Empty] }, Cmd.none
        | SetOption (index, newValue) -> { model with currentQuestion.options = model.currentQuestion.options |> modifyIndex index newValue }, Cmd.none        
        | DeleteOption index -> { model with currentQuestion.options = deleteIndex index model.currentQuestion.options }, Cmd.none 
        | SetError error -> { model with errorMessage = Some (errorMessageToString error) }, Cmd.none
        | ClearError -> { model with errorMessage = None}, Cmd.none        
        | EditExistingQuestion index -> setExistingQuestion index model
        | DeleteExistingQuestion index -> { model with questions = deleteIndex index model.questions }, Cmd.none
        | SaveQuizFile -> failwith "unimplemented"

let title : string =
    "Create Quiz"

type CreateQuiz = Template<"CreateQuiz/createquiz.html">

/// View for a current question option
let viewCurrentQuestionOption (dispatch: Dispatch<Message>) (index: int, option: string) =
    CreateQuiz.CurrentQuestionOption()
        .Option(option, fun newValue -> dispatch (SetOption(index, newValue)))
        .DeleteOption(fun _ -> dispatch (DeleteOption index))
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

let view (model: Model) (dispatch: Dispatch<Message>) : Node =
    CreateQuiz()
        .CurrentQuestion(model.currentQuestion.question, fun n -> dispatch (SetCurrentQuestion n))
        .CurrentQuestionOptions(Html.forEach (List.indexed model.currentQuestion.options) (viewCurrentQuestionOption dispatch))
        .AddCurrentQuestion(fun _ -> dispatch AddCurrentQuestion)
        .AddOption(fun _ -> dispatch AddOption)
        .ExistingQuestions(Html.forEach (List.indexed model.questions) (viewExistingQuestion dispatch))
        .ErrorMessage(Option.defaultValue String.Empty model.errorMessage)
        .ErrorMessageClass(if model.errorMessage.IsSome then String.Empty else "hidden")
        .SaveQuizFile(fun _ -> dispatch SaveQuizFile)
        .Elt()

