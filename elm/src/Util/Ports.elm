port module Util.Ports exposing (..)

port storeQuotes : String -> Cmd msg

port getStoredQuotes : ( Maybe String -> msg ) -> Sub msg