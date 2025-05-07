module Base

open Falco
open Falco.Markup

let getTitle (title: string option) : string =
    title |> Option.defaultValue "Cool page"

let basePage (title : string option) (header : string) (bodyHtml : XmlNode) : HttpHandler =
    let html = 
        Elem.html [ Attr.lang "en" ] [
            Elem.head [] [
                Elem.meta [ Attr.charset "utf-8" ];
                Elem.title [] [ 
                    TextNode (getTitle title)
                ];
                Elem.link [ Attr.href "/style/style.css"; Attr.rel "stylesheet" ]
            ];
            Elem.body [] [
                Elem.div [
                    Attr.class' "main-wrapper"
                ] [
                    Elem.div [
                        Attr.class' "main-border min-width-800 centre-text"
                    ] [
                        Elem.h1 [
                            Attr.class' "header-text"
                        ] [ Text.raw header ]
                    ]
                    Elem.div [
                        Attr.class' "secondary-border min-width-800"
                    ] [
                        bodyHtml
                    ]
                ]
            ]
        ]
    Response.ofHtml html
