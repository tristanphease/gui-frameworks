module Quotes exposing (..)

import Http
import Json.Decode exposing (Decoder, map, map3, field, string, int)
import Html exposing (a, div, text, Html, button)
import Html.Events exposing (onClick)
import Result exposing (Result)
import Browser
import Html.Parser exposing (..)
import Time exposing (Posix)

-- base url for wikiquote actions
apiUrl : String
apiUrl = "https://en.wikiquote.org/w/api.php"

-- MAIN

main : Program () Model Msg
main = 
  Browser.element 
    {
      init = init,
      update = update,
      view = view,
      subscriptions = \_ -> Sub.none
    }

-- MODEL

type QuoteError 
  = LoadingError
  | ParsingError String

type Model 
  = LoadedQuote Quote
  | LoadingQuote
  | NoQuote
  | ErrorLoadingQuote QuoteError

type alias Quote =
  {
    quote : String,
    author : String
  }

-- INIT

init : () -> ( Model, Cmd msg)
init _ = ( NoQuote, Cmd.none )

type alias PageObject = 
  {
    title : String,
    pageid : Int,
    text : String
  }

type alias MainOuterObject =
  {
    parse: PageObject
  }

loadQuoteOfTheDayPage : Cmd Msg
loadQuoteOfTheDayPage = 
  Http.get 
    {
      url = apiUrl ++ "?action=parse&page=Wikiquote%3AQuote_of_the_day%2FApril_2025&prop=text&origin=*&formatversion=2&format=json",
      expect = Http.expectJson PageReceived parseMainDecoder
    } 

parseMainDecoder : Decoder MainOuterObject
parseMainDecoder =
  map MainOuterObject 
    (field "parse" parsePageDecoder)

parsePageDecoder : Decoder PageObject
parsePageDecoder =
  map3 PageObject
    (field "title" string)
    (field "pageid" int)
    (field "text" string)

-- PARSE HTML SECTION

parseQuoteOfTheDay : String -> Result String Quote
parseQuoteOfTheDay pageString =
  case Html.Parser.run pageString of
    Ok (nodes) -> 
      let firstNode = (getFirst nodes)
      in case firstNode of 
        Just node ->
          let quoteValue = findQuoteFromNode (Time.millisToPosix 0) node
          in case quoteValue of 
            FoundQuote quote -> Ok quote 
            LookingForDate -> Err ( "Stuck looking for the right date" )
            LookingForTable -> Err ( "Stuck looking for the table" )
            LookingForSubTable -> Err ( "Stuck looking for the sub table" )
            LookingForTds -> Err ( "Stuck looking for the tds in the table" )
            LookingForAuthor _ -> Err ( "Couldn't find author" )
        Nothing -> Err ("Couldn't find node")
    Err (_) -> Err ( "Couldn't parse html" )

-- looking for a table with an element within that contains the date passed in
findTableWithDate : Time.Posix -> Maybe Node -> Node -> Maybe Node
findTableWithDate time currentTable node =
  case node of 
    Text text -> if text == "April 1" then currentTable else Nothing
    Element elementName _ childrenNodes -> 
      let newTable = getTableFromElement elementName node currentTable
          tableList = List.map (findTableWithDate time newTable) childrenNodes
      in 
        List.foldl (\x a -> if a == Nothing then x else a) Nothing (tableList)
    Comment _ -> Nothing

getFirst : List a -> Maybe a
getFirst a =
  List.foldl compareForFirst Nothing a

compareForFirst : a -> Maybe a -> Maybe a 
compareForFirst a1 a2 =
  case a2 of
    Just _ -> a2
    Nothing -> Just a1

type ParseQuoteState
  = LookingForDate
  | LookingForTable
  | LookingForSubTable
  | LookingForTds
  | LookingForAuthor String
  | FoundQuote Quote

findQuoteFromNode : Time.Posix -> Node -> ParseQuoteState
findQuoteFromNode time node =
  findQuoteFromNodeInternal time node LookingForDate

-- get the state from the element
stateFromElement : Node -> String -> ParseQuoteState -> ParseQuoteState
stateFromElement node elementName currentState 
  = case currentState of 
    -- if looking for date, use current state children first
    LookingForDate -> currentState
    LookingForTable -> if checkTable elementName then LookingForSubTable else LookingForTable
    LookingForSubTable -> if checkTable elementName then LookingForTds else LookingForSubTable
    LookingForTds -> 
      if elementName == "td" && String.length (getTextFromNode node) > 0 then
        LookingForAuthor (getTextFromNode node)
      else 
        LookingForTds
    LookingForAuthor quoteString -> 
      if elementName == "td" && String.length (getTextFromNode node) > 0 then
        FoundQuote (Quote quoteString (getTextFromNode node))
      else 
        currentState
    FoundQuote _ -> currentState



findQuoteFromNodeInternal : Time.Posix -> Node -> ParseQuoteState -> ParseQuoteState
findQuoteFromNodeInternal time node state =
  case node of 
    Text text -> 
      case state of 
        LookingForDate -> if checkDateText time text then LookingForTable else LookingForDate
        _ -> state 
    Element elementName _ childrenNodes ->
      List.foldl (findQuoteFromNodeInternal time) (stateFromElement node elementName state) childrenNodes
    Comment _ -> state

-- just retrieves the text from a node and sub nodes recursively
getTextFromNode : Node -> String
getTextFromNode node =
  case node of 
    Text nodeText -> nodeText 
    Element _ _ childrenNodes -> 
      List.foldr (++) "" (List.map getTextFromNode childrenNodes)
    Comment _ -> ""

checkTable : String -> Bool
checkTable elementName =
  elementName == "table"

-- find the date
checkDateText : Time.Posix -> String -> Bool
checkDateText time text =
  text == "April 1"

-- findTableFollowingDate : Time.Posix -> Maybe Node -> Node -> Maybe Node
-- findTableFollowingDate time currentTable node =
--   case node of 
--     Text text -> if text == "April 1"
    


-- gets the table from an element
getTableFromElement : String -> Node -> Maybe Node -> Maybe Node
getTableFromElement elementName node currentTable =
  if elementName == "table" then Just node else currentTable  


-- UPDATE

type Msg 
  = PageReceived (Result Http.Error MainOuterObject)
  | FetchQuote

update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
  case msg of
    PageReceived (Ok outerPage) -> 
      case parseQuoteOfTheDay outerPage.parse.text of
        Ok ( parsedQuote ) -> ( LoadedQuote parsedQuote, Cmd.none )
        Err ( error ) -> ( ErrorLoadingQuote (ParsingError error), Cmd.none )
    PageReceived (Err _) ->
      ( ErrorLoadingQuote LoadingError, Cmd.none )
    FetchQuote ->
      ( LoadingQuote, loadQuoteOfTheDayPage)


-- VIEW

view : Model -> Html Msg
view model =
  div []
    [
      text "hi",
      button [ onClick FetchQuote ] [ text "button" ],
      viewQuote model
    ]

viewQuote : Model -> Html Msg 
viewQuote model =
  case model of 
    LoadedQuote quote ->
      div [] [text "quote: ", text quote.quote, text quote.author ]
    LoadingQuote ->
      div [] [text "loading"]
    NoQuote ->
      div [] [text "get quote?"]
    ErrorLoadingQuote quoteError -> 
      div [] [ text (getErrorText quoteError) ]

getErrorText : QuoteError -> String
getErrorText quoteError = 
  case quoteError of 
    LoadingError -> "Couldn't load from wikiquote"
    ParsingError errorString -> "Couldn't parse: "++ errorString 
