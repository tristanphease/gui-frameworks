module Quotes exposing (..)

import Http
import Json.Decode exposing (Decoder, map3, field, string, int)
import Html exposing (..)
import Html.Events exposing (onClick)
import Result exposing (Result)
import Browser
import Html.Parser exposing (..)
import Time exposing (..)
import Html.Attributes exposing (..)
import Task exposing (Task)
import Util.Months exposing (..)
import Util.Date exposing (..)

-- base url for wikiquote actions
apiUrl : String
apiUrl = "https://en.wikiquote.org/w/api.php"

-- MAIN
-- this main is just so this page works when using elm.reactor
-- otherwise not needed

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

type QuoteLoading 
  = LoadedQOTD Date Quote
  | LoadingQOTD Date
  | ErrorLoadingQOTD Date QuoteError
  | GettingDate
  | NoQuote


type DateType 
  = ValidDate Date 
  | InvalidDate Date
  | NoDate

type alias Model =
  {
    savedQuotes: List Quote,
    date: DateType,
    -- can only load one quote at a time
    currentQOTDLoading : QuoteLoading
  }

type QuoteOrigin
  = QuoteOfTheDay Date
  
type alias Quote =
  {
    quote : String,
    author : String
  }

-- INIT

init : () -> ( Model, Cmd Msg)
init _ = 
  ( 
    {
      savedQuotes = [],
      date = NoDate,
      currentQOTDLoading = NoQuote
    },
    Task.perform SetCurrentDate getDate
  )

getDate : Task x Date
getDate =
  Task.map2 getDateFromTime Time.now Time.here 

getDateFromTime : Time.Posix -> Time.Zone -> Date 
getDateFromTime time zone =
  Date
    ( Time.toYear zone time )
    ( Time.toMonth zone time )
    ( Time.toDay zone time )

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

loadQuoteOfTheDayPage : Date -> Cmd Msg
loadQuoteOfTheDayPage date = 
  Http.get 
    {
      url = getApiUrl date,
      expect = Http.expectJson ( PageReceived date ) parseMainDecoder
    } 

getApiUrl : Date -> String 
getApiUrl date =
  apiUrl 
    ++ "?action=parse&page=Wikiquote%3AQuote_of_the_day%2F" 
    ++ (toMonthString date.month) 
    ++ "_"
    ++ (String.fromInt date.day) 
    ++ ",_"
    ++ (String.fromInt date.year)
    ++ "&prop=text&origin=*&formatversion=2&format=json"

parseMainDecoder : Decoder MainOuterObject
parseMainDecoder =
  Json.Decode.map MainOuterObject 
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
          let quoteValue = findQuoteFromNode node
          in case quoteValue of 
            FoundQuote quote -> Ok quote 
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
  = LookingForTable
  | LookingForSubTable
  | LookingForTds
  | LookingForAuthor String
  | FoundQuote Quote

findQuoteFromNode : Node -> ParseQuoteState
findQuoteFromNode node =
  findQuoteFromNodeInternal node LookingForTable

-- get the state from the element
stateFromElement : Node -> String -> ParseQuoteState -> ParseQuoteState
stateFromElement node elementName currentState 
  = case currentState of 
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



findQuoteFromNodeInternal : Node -> ParseQuoteState -> ParseQuoteState
findQuoteFromNodeInternal node state =
  case node of 
    Text _ -> state
    Element elementName _ childrenNodes ->
      List.foldl findQuoteFromNodeInternal (stateFromElement node elementName state) childrenNodes
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
    


-- gets the table from an element
getTableFromElement : String -> Node -> Maybe Node -> Maybe Node
getTableFromElement elementName node currentTable =
  if elementName == "table" then Just node else currentTable  


-- UPDATE

type Msg 
  = PageReceived Date (Result Http.Error MainOuterObject)
  | FetchQuote Date
  | SetCurrentDate Date

update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
  case msg of
    PageReceived date (Ok outerPage) -> 
      case parseQuoteOfTheDay outerPage.parse.text of
        Ok ( parsedQuote ) -> 
          ( 
            { model | currentQOTDLoading = LoadedQOTD date parsedQuote },
            Cmd.none
          )
        Err ( error ) -> 
          ( 
            { model | currentQOTDLoading = ErrorLoadingQOTD date (ParsingError error) }
            , Cmd.none 
          )
    PageReceived date (Err _) ->
      ( 
        { model | currentQOTDLoading = ErrorLoadingQOTD date LoadingError }, 
        Cmd.none
      )
    FetchQuote date ->
      ( 
        { model | currentQOTDLoading = LoadingQOTD date }, 
        loadQuoteOfTheDayPage date
      )
    SetCurrentDate date ->
      ( 
        { model | date = if checkValidDate date then ValidDate date else InvalidDate date }, 
        Cmd.none 
      )

-- VIEW

view : Model -> Html Msg
view model =
  div []
    [
      h1 [] [ text "Get and save quotes" ],
      viewRetrieveQuote model,
      viewCurrentQuote model
    ]

viewRetrieveQuote : Model -> Html Msg 
viewRetrieveQuote model =
  div [ class "flex-row", class "gap-10" ] 
    [
      viewCalendar model.date,
      viewGetQuote model
    ]

viewGetQuote : Model -> Html Msg
viewGetQuote model =
  case model.date of 
    ValidDate date -> button [ onClick ( FetchQuote date ) ] [ text getQuoteButtonText ]
    InvalidDate _ -> button [ disabled True ] [ text getQuoteButtonText ]
    NoDate -> button [ disabled True ] [ text getQuoteButtonText ]

getQuoteButtonText : String 
getQuoteButtonText = "Get Quote of the Day"

viewCurrentQuote : Model -> Html Msg 
viewCurrentQuote model =
  div [ class "flex-column", class "gap-10" ] 
    [
      viewQuote model
    ]

viewQuote : Model -> Html Msg 
viewQuote model =
  
  case model.currentQOTDLoading of 
    LoadedQOTD date quote ->
      div [ class "flex-column", class "gap-10" ]
        [
          text ( "Loaded quote of the day for " ++ dateToReadableString date ++ ":" ),
          textarea [ disabled True, rows 5, cols 10 ] 
            [
              text quote.quote
            ],
          text quote.author,
          button [] [ text "Save quote" ] 
        ]
    LoadingQOTD _ -> viewLoading
    GettingDate -> viewLoading
    NoQuote ->
      div [] [text "get quote?"]
    ErrorLoadingQOTD date quoteError -> 
      div [] [ text ("Couldn't load quote: " ++ getErrorText quoteError) ]
viewLoading : Html Msg
viewLoading =
  div [] [text "Loading Quote"]

viewCalendar : DateType -> Html Msg
viewCalendar dateType =
  div []
    [
      (
        case dateType of 
          ValidDate date -> dateInput date
          InvalidDate invalidDate -> text "invalid date"
          NoDate -> text "don't have date loaded"
      )
    ]

dateInput : Date -> Html Msg 
dateInput date =
  span []
    [
      input [ type_ "date", value ( dateToString date ) ] []
    ]



getErrorText : QuoteError -> String
getErrorText quoteError = 
  case quoteError of 
    LoadingError -> "Couldn't load from wikiquote"
    ParsingError errorString -> "Couldn't parse: "++ errorString 
