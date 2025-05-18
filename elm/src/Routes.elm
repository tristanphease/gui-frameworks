module Routes exposing (..)

import Url exposing (Url)
import Url.Parser exposing (..)
import Environment exposing (..)

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

mainMatchParser : Parser (Route -> c) c
mainMatchParser = 
  oneOf
    [ 
      map Main top, 
      map Quote (s "quotes")
    ]

matchRoute : Parser (Route -> a) a
matchRoute =
  if String.isEmpty Environment.basePath then mainMatchParser else s Environment.basePath </> mainMatchParser

envPathBase : String
envPathBase =
  if String.isEmpty Environment.basePath then "" else "/" ++ Environment.basePath

pathFor : Route -> String
pathFor route =
  envPathBase ++ case route of
    Quote -> "/quotes"
    NotFound -> "/404"
    Main -> "/"

titleFor : Route -> String
titleFor route =
  case route of 
    Quote -> "Save cool quotes!"
    NotFound -> "404 Page"
    Main -> "Main page"
