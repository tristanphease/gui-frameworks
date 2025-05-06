module Base

open Falco
open Falco.Markup

let basePage header bodyHtml : HttpHandler =
    let html = 
        Elem.html [ Attr.lang "en" ] [
            Elem.head [] [
                Elem.meta [ Attr.charset "utf-8" ];
                Elem.title [] [ TextNode "F#" ]
            ]
            Elem.body [] [
                Elem.div [] [
                    Text.h1 header
                ]
                Elem.div [] [
                    bodyHtml
                ]
            ]
        ]
    Response.ofHtml html
