module Main exposing (..)

import Browser exposing (Document)
import Html exposing (text)
import Maybe exposing (Maybe)
import Html exposing (button)
import Html.Events exposing (onClick)

-- MAIN

main : Program () Model Msg 
main =
    Browser.document 
      {
        init = init,
        view = view,
        update = update,
        subscriptions = subscriptions
      }

-- MODEL


type alias Model = 
  {
    
  }

init : () -> ( Model, Cmd Msg )
init _ =
  (
    {
      
    },
    Cmd.none
  )

-- UPDATE

type Msg = NoOp

update : Msg -> Model -> (Model, Cmd Msg)
update msg model =
  ( model, Cmd.none )

-- VIEW

view : Model -> Document Msg
view model =
  { 
    title = "test",
    body = 
      [ 
        text "hello world",
        button [ ] [ text "test button" ]
      ]
  }


-- SUBSCRIPTIONS

subscriptions : Model -> Sub Msg
subscriptions _ =
    Sub.none