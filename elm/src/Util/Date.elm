module Util.Date exposing (..)

import Time exposing (..)
import Util.Months exposing ( toMonthInt, toIntMonth, toMonthString )

type alias Date =
  {
    year : Int,
    month : Time.Month,
    day : Int
  }

blankDate : Date 
blankDate =
  Date 0 Time.Jan 0

dateToReadableString : Date -> String 
dateToReadableString date =
  String.fromInt date.day ++ " " ++ 
  toMonthString date.month ++ " " ++ 
  String.fromInt date.year

dateToString : Date -> String 
dateToString date =
  ( padLength 4 ( String.fromInt date.year ) ) ++
  "-" ++
  ( padLength 2 ( String.fromInt ( toMonthInt date.month ) ) ) ++
  "-" ++
  ( padLength 2 ( String.fromInt date.day ) )

stringToDate : String -> Date 
stringToDate dateString =
  dateString
    |> String.split "-" --split on -
    |> List.filterMap String.toInt -- convert to int
    |> List.indexedMap Tuple.pair
    |> List.foldl dateFromStringPart blankDate

dateFromStringPart : ( Int, Int ) -> Date -> Date
dateFromStringPart ( datePart, dateValue ) date =
  case datePart of 
    0 -> { date | year = dateValue }
    1 -> { date | month = toIntMonth dateValue }
    2 -> { date | day = dateValue }
    _ -> date

padLength : Int -> String -> String 
padLength length string =  
  (String.repeat ( max 0 ( length - String.length string ) ) "0") ++ string

checkValidDate : Date -> Bool
checkValidDate date =
  validYear date && validDay date

validYear : Date -> Bool 
validYear date =
  date.year >= 2003

validDay : Date -> Bool
validDay date =
  date.day <= maxDay date.month date.year

maxDay : Month -> Int -> Int
maxDay month year =
  case month of 
    Time.Jan -> 31
    Time.Feb -> if isLeapYear year then 29 else 28
    Time.Mar -> 31
    Time.Apr -> 30
    Time.May -> 31
    Time.Jun -> 30
    Time.Jul -> 31
    Time.Aug -> 31
    Time.Sep -> 30
    Time.Oct -> 31
    Time.Nov -> 30
    Time.Dec -> 31

-- leap year if divisible by 4 unless divisible by 100 unless divisible by 400
isLeapYear : Int -> Bool
isLeapYear year =
  modBy year 4 == 0 &&
  ( modBy year 100 /= 0 || modBy year 400 == 0 )
