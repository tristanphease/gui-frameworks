module Util.Months exposing (..)

import Time exposing (Month(..))

getAllMonths : List Month 
getAllMonths =
  [
    Time.Jan,
    Time.Feb,
    Time.Mar,
    Time.Apr,
    Time.May,
    Time.Jun,
    Time.Jul,
    Time.Aug,
    Time.Sep,
    Time.Oct,
    Time.Nov,
    Time.Dec
  ]

toMonthString : Time.Month -> String 
toMonthString month =
  case month of 
    Jan -> "January"
    Feb -> "February"
    Mar -> "March"
    Apr -> "April"
    May -> "May"
    Jun -> "June"
    Jul -> "July"
    Aug -> "August"
    Sep -> "September"
    Oct -> "October"
    Nov -> "November"
    Dec -> "December"

toStringMonth : String -> Time.Month
toStringMonth monthName =
  case monthName of 
    "January" -> Jan
    "February" -> Feb
    "March" -> Mar
    "April" -> Apr
    "May" -> May
    "June" -> Jun
    "July" -> Jul
    "August" -> Aug
    "September" -> Sep
    "October" -> Oct
    "November" -> Nov
    "December" -> Dec
    _ -> Jan

toMonthInt : Month -> Int
toMonthInt month =
  case month of 
    Jan -> 1
    Feb -> 2
    Mar -> 3
    Apr -> 4
    May -> 5
    Jun -> 6
    Jul -> 7
    Aug -> 8
    Sep -> 9
    Oct -> 10
    Nov -> 11
    Dec -> 12

toIntMonth : Int -> Month 
toIntMonth monthNum =
  case monthNum of 
    1 -> Jan
    2 -> Feb
    3 -> Mar
    4 -> Apr
    5 -> May
    6 -> Jun
    7 -> Jul
    8 -> Aug
    9 -> Sep
    10 -> Oct
    11 -> Nov
    12 -> Dec
    _ -> Jan
