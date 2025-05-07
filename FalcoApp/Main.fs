module Main

open Falco
open Falco.Markup

let mainHtml : XmlNode =
    Elem.div [] [
        Text.h2 "Hey!"
    ]

let mainPage : HttpHandler = 
    Base.basePage (Some "Main Page") "Main Page" mainHtml

