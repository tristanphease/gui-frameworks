module Main

open Falco
open Falco.Markup

let mainHtml : XmlNode =
    Elem.div [] [
        Text.p "hi"
    ]

let mainPage : HttpHandler = 
    Base.basePage "header" mainHtml

