module Main exposing (..)

import Browser exposing (Document)
import Browser.Navigation as Nav
import Html exposing (..)
import Html.Attributes exposing (..)
import Routes exposing (Route, mainPath, quotesPath, titleFor)
import Url
import Quotes
import Platform.Cmd as Cmd
import NotFound
import Util.Ports

-- MAIN

main : Program (Maybe String) Model Msg 
main =
  Browser.application 
    {
      init = init,
      view = view,
      update = update,
      subscriptions = subscriptions,
      onUrlChange = UrlChanged,
      onUrlRequest = LinkClicked
    }

-- MODEL


type alias Model = 
  {
    route : Route,
    page : Page,
    key : Nav.Key,
    url : Url.Url,
    savedState : Maybe String
  }

type Page
  = NotFoundPage
  | QuotesPage Quotes.Model
  | MainPage

isQuotePage : Page -> Bool
isQuotePage page =
  case page of
    QuotesPage _ -> True
    _ -> False

init : (Maybe String) -> Url.Url -> Nav.Key -> ( Model, Cmd Msg )
init savedState url navKey =
  (
    Model (Routes.parseUrl url) MainPage navKey url savedState,
    Cmd.none
  )
    |> loadCurrentPage

loadCurrentPage : ( Model, Cmd Msg ) -> ( Model, Cmd Msg )
loadCurrentPage ( model, existingCmds ) =
  let 
    ( currentPage, mappedPageCmds ) =
      case model.route of
        Routes.NotFound -> 
          ( NotFoundPage, Cmd.none )
        Routes.Main -> 
          ( MainPage, Cmd.none )
        Routes.Quote ->
          let 
            (pageModel, pageCmds) =
              Quotes.init model.savedState
          in 
            (QuotesPage pageModel, Cmd.map QuotesMsg pageCmds)
  in (
    { model | page = currentPage },
    Cmd.batch [ existingCmds, mappedPageCmds ]
  )

-- UPDATE

type Msg 
  = LinkClicked Browser.UrlRequest
  | UrlChanged Url.Url
  | QuotesMsg Quotes.Msg
  | NotFoundMsg NotFound.Msg
  | ReceiveStoredDate ( Maybe String )

update : Msg -> Model -> (Model, Cmd Msg)
update msg model =
  case msg of 
    LinkClicked urlRequest ->
      case urlRequest of 
        Browser.Internal url ->
          ( model, Nav.pushUrl model.key (Url.toString url ) )
        Browser.External href ->
          ( model, Nav.load href )
    UrlChanged url ->
      let 
        newRoute =
          Routes.parseUrl url 
      in 
      ( 
        { model | route = newRoute },
        Cmd.none
      )
        |> loadCurrentPage
    QuotesMsg quotesMsg -> 
      case model.page of 
        QuotesPage quotesModel -> 
          let 
            ( quotesPageModel, quotesCmd ) 
              = Quotes.update quotesMsg quotesModel 
          in 
            ( 
              { model | page = QuotesPage quotesPageModel },
              Cmd.map QuotesMsg quotesCmd
            )
        _ -> (model, Cmd.none)
    NotFoundMsg _ ->
      (model, Cmd.none)
    ReceiveStoredDate storedData ->
      (
        { model | savedState = storedData },
        Cmd.none
      )

-- VIEW

view : Model -> Document Msg
view model =
  { 
    title = titleFor model.route,
    body = 
      [ 
        viewHeader model.page,
        viewBody model.page
      ]
  }

-- displays the nav header
viewHeader : Page -> Html Msg
viewHeader page =
  section [ class "header", class "flex-row", 
    class "gap-10", class "flex-main-center", class "flex-sub-center",
    class "font-size-30" ]
    [
      headerLink mainPath "Main Page" (page == MainPage),
      headerLink quotesPath "Quotes" (isQuotePage page)
    ]

headerLink : String -> String -> Bool -> Html Msg
headerLink link linkText isSelected =
  a [ href link, class "header-link", classList [ ("highlight", isSelected) ] ] 
    [
      text linkText
    ]

viewMain : Html Msg 
viewMain = 
  div []
    [
      p [] [ text "This is the main page. Click the buttons at the top to navigate around!" ],
      p [] [ text "I don't really know what else to put on this main page." ]
    ]

viewBody : Page -> Html Msg 
viewBody page =
  section [ class "flex-column", class "main-body" ]
    [
      case page of
        MainPage -> viewMain
        NotFoundPage -> 
          NotFound.view 
            |> Html.map NotFoundMsg
        QuotesPage pageModel -> 
          Quotes.view pageModel 
            |> Html.map QuotesMsg
    ]
  

-- SUBSCRIPTIONS

subscriptions : Model -> Sub Msg
subscriptions _ =
  Util.Ports.getStoredQuotes ReceiveStoredDate