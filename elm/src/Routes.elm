module Routes exposing (..)

import Url exposing (Url)
import Url.Parser exposing (..)


type Route
  = NotFound
  | Main
  | Quote

quotesPath : String
quotesPath = 
  pathFor Quote

mainPath : String
mainPath = 
  pathFor Main

parseUrl : Url -> Route
parseUrl url =
  case parse matchRoute url of
    Just route ->
      route
    Nothing ->
      NotFound


matchRoute : Parser (Route -> a) a
matchRoute =
  oneOf
    [ 
      map Main top, 
      map Quote (s "quotes")
    ]

pathFor : Route -> String
pathFor route =
  case route of
    Quote -> "/quotes"
    NotFound -> "/404"
    Main -> "/"

titleFor : Route -> String
titleFor route =
  case route of 
    Quote -> "Save cool quotes!"
    NotFound -> "404 Page"
    Main -> "Main page"
