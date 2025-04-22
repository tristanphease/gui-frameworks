module NotFound exposing (..)

import Html exposing (..)

type Msg = NoOp

view : Html Msg
view =
  div []
    [
      text "Page not found, navigate elsewhere for what you seek."
    ]
