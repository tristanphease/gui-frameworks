module BoleroApp.Client.ViewQuiz

open Bolero
open Elmish
open Quiz
open Microsoft.JSInterop
open System;

type QuizQuestionWithAnswer =
    {
        question: string;
        options: string list;
        correctAnswer: int;
        answerGiven: int option;
    }

type QuizInfo =
    | QuizNotLoaded
    | QuizLoaded of quiz: QuizQuestionWithAnswer list
    | QuizCompleted of quiz: QuizQuestionWithAnswer list

type Model = 
    {
        quizInfo: QuizInfo;
        errorMessage: string option;
    }

type Message = 
    | LoadQuizFile 
    | CompleteQuiz
    | LoadFileMessage of File.FileMessage
    | SelectOption of questionIndex: int * optionIndex: int

let title : string =
    "View Quiz"

let initModel : Model =
    {
        quizInfo = QuizNotLoaded;
        errorMessage = None;
    }

let newQuestionWithAnswer (quizQuestion : Quiz.QuizQuestion) : QuizQuestionWithAnswer  =
    {
        question = quizQuestion.question;
        options = quizQuestion.options;
        correctAnswer = quizQuestion.correctAnswer;
        answerGiven = None;
    }

let loadQuizWithAnswer quizJson =
    let quiz = Quiz.quizFromJson quizJson
    List.map newQuestionWithAnswer quiz

let questionWithAnswer question answerIndex =
    { question with answerGiven = Some answerIndex }

let selectOption (questionIndex: int) (optionIndex: int) (questions: QuizQuestionWithAnswer list) =
    questions 
        |> Util.modifyIndex questionIndex (questionWithAnswer (questions.Item questionIndex) optionIndex ) 

let update (jsRuntime: IJSRuntime) (message: Message) (model: Model) : Model * Cmd<Message> =
    match message with      
        | LoadQuizFile -> 
            model, Cmd.map LoadFileMessage (File.loadFile jsRuntime)        
        | CompleteQuiz -> 
            match model.quizInfo with 
                QuizLoaded quizQuestions -> 
                    { model with quizInfo = QuizCompleted quizQuestions }, Cmd.none
                | _ -> model, Cmd.none   
        | LoadFileMessage (File.ReturnedFileInfo quizJson) -> 
            { model with quizInfo = QuizLoaded (loadQuizWithAnswer quizJson) }, Cmd.none
        | LoadFileMessage File.ErrorLoading ->
            { model with errorMessage = Some "Didn't load file" }, Cmd.none
        | SelectOption (questionIndex, optionIndex) -> 
            match model.quizInfo with 
                | QuizLoaded quizQuestions ->
                    { model with quizInfo = QuizLoaded (selectOption questionIndex optionIndex quizQuestions) }, Cmd.none                
                | _ -> model, Cmd.none
                
            

type ViewQuiz = Template<"ViewQuiz/viewquiz.html">

let viewQuizOptions (dispatch: Dispatch<Message>) (questionIndex: int) (optionIndex: int, quizOption: string) =
    ViewQuiz.QuestionOption()
        .OptionName(quizOption)
        .QuestionIndex(questionIndex.ToString())
        .OptionIndex(optionIndex.ToString())
        .RadioSelected(fun _ -> dispatch (SelectOption (questionIndex, optionIndex)))
        .Elt()

let viewQuizQuestions (dispatch: Dispatch<Message>) (questionIndex: int, quizQuestion: QuizQuestionWithAnswer) =
    ViewQuiz.Question()
        .QuestionOptions(
            viewQuizOptions dispatch questionIndex
                |> Html.forEach (List.indexed quizQuestion.options)
        )
        .QuizQuestion(quizQuestion.question)
        .Elt()

let correctClass = "correct-background"
let incorrectClass = "incorrect-background"

/// Gets which css classes to add to the question option
/// isAnswer -> whether the answer provided is the current option
/// isCorrect -> whether the correct answer is the current option
let questionOptionClass isAnswer isCorrect =
    match isAnswer, isCorrect with 
        | true, true -> correctClass
        | true, false -> incorrectClass
        | false, true -> correctClass
        | false, false -> String.Empty


let viewQuizQuestionWithAnswer (answerIndex: int option) (correctIndex: int) (optionIndex, optionName: string)  =
    ViewQuiz.QuestionOptionWithAnswer()
        .OptionName(optionName)
        .QuestionOptionClass(questionOptionClass (Util.isEqual optionIndex answerIndex) (optionIndex = correctIndex) )
        .Elt()


let viewQuizQuestionsWithAnswers (quizQuestion: QuizQuestionWithAnswer) =
    ViewQuiz.QuestionWithAnswerTemplate()
        .QuestionOptionWithAnswer(
            viewQuizQuestionWithAnswer quizQuestion.answerGiven quizQuestion.correctAnswer
                |> Html.forEach (List.indexed quizQuestion.options)
        )
        .Elt()

let isCorrectAnswerQuestion quizQuestion = 
    Util.isEqual quizQuestion.correctAnswer quizQuestion.answerGiven

let calculateCorrectScore (quizQuestions: QuizQuestionWithAnswer list) =
    quizQuestions 
        |> List.filter isCorrectAnswerQuestion
        |> List.length

let viewQuiz (dispatch: Dispatch<Message>) (quizInfo: QuizInfo) : Node =
    match quizInfo with 
        | QuizNotLoaded -> 
            ViewQuiz.QuizNotLoadedTemplate()
                .Elt()
        | QuizLoaded quiz -> 
            ViewQuiz.QuizViewTemplate()
                .QuizQuestions(Html.forEach (List.indexed quiz) (viewQuizQuestions dispatch))
                .CompleteQuiz(fun _ -> dispatch CompleteQuiz)
                .Elt()         
        | QuizCompleted quiz -> 
            ViewQuiz.QuizCompletedTemplate()
                .QuestionWithAnswer(Html.forEach quiz viewQuizQuestionsWithAnswers)
                .CorrectScore(
                    (calculateCorrectScore quiz).ToString()
                )
                .MaxScore((List.length quiz).ToString())
                .Elt()

let view (model: Model) (dispatch: Dispatch<Message>) : Node =
    ViewQuiz()
        .LoadQuizFile(fun _ -> dispatch LoadQuizFile)
        .QuizView(viewQuiz dispatch model.quizInfo)
        .Elt()

