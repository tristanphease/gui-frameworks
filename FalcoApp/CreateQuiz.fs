module CreateQuiz

open Falco
open Falco.Markup

let createQuizHtml : XmlNode =
    Elem.div [] [
        Text.h2 "Hey!"
    ]

let createQuizPage : HttpHandler = 
    Base.basePage (Some "Create a Quiz") "Create a Quiz" createQuizHtml

