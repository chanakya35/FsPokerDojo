﻿(* Style of dojo taken from https://github.com/c4fsharp/Dojo-Markov-Bot

All source material for this dojo can be found at:
https://github.com/chryosolo/FsPokerDojo.git
It's MIT licensed - use it, share it, modify it. *)

(*
Introduction
###############################################################################
The goal of the dojo is to take a valid poker hand and put it in the correct
scoring category.  To achieve this, we will break the task down into manageable
pieces, and then suggest directions to explore further! 

The BEGINNER script is a guide, walking the user step by step towards a basic,
working solution, emphasizing notes about F# syntax and concepts.

The INTERMEDIATE script takes over from there -- the beginner program is simply
given, already working, and steps will work toward the improvement of the
program by giving better error handling using computational expressions and
Railway Oriented Programming, and better testing using FsCheck and xUnit.
*)

// --> delete this error when you understand to look for tasks! :)
failwith( "Any time you see '-->', this is a task for you!" )

(*
Freebie: Simple testing function -- we'll get more in-depth later
###############################################################################
The function testFunc which exists to help us test our parsing routines as
we're writing them.  It will print out a test description, whether or not the
test passed, and give the result, or the error as appropriate.

It takes three parameters:
* desc is a string, and simply prints out to show what test is being performed
* parseFunc is the parsing function to call
* input is what to pass to the parsing function

It does the call to the function inside a try/with, so the parseFunc can
'failwith', and the failure message will be printed out.
*)
let testFunc desc parseFunc input =
   try
      let actual = parseFunc input
      printfn "%s: OK with value of %A" desc actual
   with
   | Failure(message) -> printfn "%s: FAIL with %s" desc message




(*
Chapter 1: Representing a card
###############################################################################
Your goal here is to take a two character string input, like "5C", and return a
strongly-typed card (Five, Clubs), or throw an error on invalid input.

We start with defining types which hopefully will make impossible to hold
incorrect data.

F# has a capability called a Discriminated Union, which define a list of values
and optionally allows each to have dmata of different types.  At its simplest,
DU choices do not have to have any data, and are like an unnumbered enum.

type EmptyDu =
   | Red
   | Green
   | Blue

There are four suits: Clubs, Spades, Hearts, and Diamonds. *)

// --> Fix and complete the Suit DU.
type Suit =
   | Suit1
   | Suit2

(*
Enum syntax is like the empty DU, except each holds an integer value.

type ValuedEnum =
   | Squared1 = 1
   | Squared2 = 4
   | Squared3 = 9

Even though values will be numeric, we will use defined names for easier future
programming. There are thirteen Values: Two, Three, Four, Five, Six, Seven,
Eight, Nine, Ten, Jack, Queen, King, Ace
*)

// --> Fix and complete the Value enum.
type Value =
   | Squared1 = 1
   | Squared2 = 4
   | Squared3 = 9

(*
A card has both a Suit and a Value, never just one or the other.  This is a
perfect time to use a Tuple, which requires all portions to be used together.
A tuple type has portion types separated by an asterisk:

type Date = int * int * int * DayOfWeek

Note the Date has three integer values and a DayOfWeek.  It is up to the
programmer to remember that the first int is a Month, the second is a Day,
and the third is a Year.  This is a weakness of Tuples, but there are ways
to help get around this which we'll see later.

hint: Records are one way around this
hint: Single case DUs help avoid "primitive obsession":
      see http://blog.ploeh.dk/2015/01/19/from-primitive-obsession-to-domain-modelling/
   type Year = Year of int
   type Month = Month of int
   type Day = Day of int
   type Date = Year * Month * Day * DayOfWeek

Our card type will have a Value and a Suit
*)

// --> Fix the Card type definition
type Card = int * float * bool

(*
Now we'll have a function which will parse a character and return the valid
suit, or throw an error.  Pattern matching is a strong feature of F#.  It works
really well for DUs, because it requires us to deal with all cases which
eliminates all bugs about forgetting to handle everything.

let myColorLikes color =
   match color with
   | EmptyDu.Red -> true
   | EmptyDu.Blue -> false
// throws a compile-time error because EmptyDu.Green wasn't handled

Pattern matching also work for enums, but here's the interesting thing... In
F#, you can dynamically create new enum values at runtime, so the list you
give in code cannot be considered "complete".  As such, you can never cover all
possible enum values, so you must include a wildcard to catch unspecified values.
See https://fsharpforfunandprofit.com/posts/enum-types/ for more info.

let myValueLikes value =
   match value with
   | Value.Squared1 -> true
   | Value.Squared2 -> false
   | Value.Squared3 -> false
// gives a warning that you haven't covered all cases, like 0.  WTF?  There
IS NO ZERO enum!!!  Oh, right, someone may MAKE ONE at runtime...
   | _ -> failwith "I haven't handled that enum yet..."

We will use C for clubs, S for spades, H for hearts, and D for diamonds.
*)

// --> fix and complete the function
let parseSuit charSuit =
   match charSuit with
   | 'q'                 // not handling 'q' means it is handled below
   | 'Q' -> Suit.Suit1   // 'Q' (and 'q') are handled here
   | 'W' -> Suit.Suit2
   | _ -> failwith "Invalid suit." // wildcard match throws an error

testFunc "parseSuit success" parseSuit 'c'
testFunc "parseSuit invalid" parseSuit 'Q'

(*
We will use 2-9 for Two..Nine, T for Ten, J for Jack, Q for Queen, K for King,
and A for Ace
*)
// --> fix and complete the parseValue function
let parseValue charValue =
   match charValue with
   | '1' -> Value.Squared3
   | _ -> failwith "Invalid value."

testFunc "parseValue success numeric" parseValue '5'
testFunc "parseValue success face" parseValue 'Q'
testFunc "parseValue invalid" parseValue '1'

(*
We'll put it together here and return a card or throw an error.  We'll use
pattern matching again, this time on the size of an array.  If we're given
two chars, we'll assume they are a value followed by a suit.  Anything else
is invalid.
*)
let parseCard (strCard:string) =
   // --> add a ToUpper so you can simplify above parsing!
   let chars = strCard.   ToCharArray()
   match chars with
   // here pattern matching is seeing if the chars array can be placed into a
   // two-item array.  If it can, it will name them charValue and charSuit,
   // which you can use when it's appropriate!  Handy!
   | [| charValue; charSuit |] ->
      // --> call the parseValue and parseSuit functions
      let value = Value.Squared1
      let suit = Suit.Suit2
      value,suit
   // array isn't two items long
   | _ -> failwith "Invalid card syntax."

testFunc "parseCard success UPPER lower" parseCard "Tc"
testFunc "parseCard success lower UPPER" parseCard "aS"
testFunc "parseCard invalid card syntax" parseCard "123"
testFunc "parseCard invalid value valid suit" parseCard "1s"
testFunc "parseCard valid value invalid suit" parseCard "4K"




(*
Chapter 2: Representing a hand
###############################################################################
In chapter two, we will be taking a string of five cards separated by spaces,
like "2C 3C 4C 5C 6C" and converting it into a Hand, which will be a 5-tuple of
Cards.
*)

// --> make sure your Card type from above matches, and then delete this line
//     and switch all future Code from Card' to just Card
type Card' = Value * Suit
// --> fix the hand type
type Hand = bool * double


(*
F# has several built in collection types including :
- Arrays -- random access by index
- List -- sequential access via a singly linked list
- Sequence -- "like a list" -- but lazily evaluated and doesn't support pattern
              matching and poor performance in certain areas
Note -- see https://fsharpforfunandprofit.com/posts/list-module-functions
for distinction between the collection types, and when you might choose each.

We'll write a few utility functions first.  We will need to be able to tell if
an array of cards contains any duplicates.  We'll use the GROUP BY function
which gives an array of buckets where each bucket is "named" by a key, and the
bucket holds all values that key applies to.  In our case, we're generating the
key to be the card itself, and that key is the same for all identical cards --
duplicate cards will go into the same bucket while unique cards will go into
different buckets.  Given this, if the length of the array of buckets is less
than the input length, there must be a duplicate!
*)

let areDups cardArray =
   let uniqueCards = Array.groupBy id cardArray
   // --> functions return their last expression.  Here, we return true if
   // the uniqueCards length is less than the cardArray length
   true
   
testFunc "areDups all unique" areDups [| (Value.Squared1,Suit.Suit1);
                                         (Value.Squared1,Suit.Suit2) |]
testFunc "areDups duplicates" areDups [| (Value.Squared1,Suit.Suit1);
                                         (Value.Squared1,Suit.Suit1) |]

(*
We can use String.Split to get an array of card tokens, and just send them to
our existing parseCard function.  Since we're expecting a single deck, there
should not be any duplicate cards.  Hand analysis will be easier if we sort
the hand by card.  Note that you get sorting for free in F#, first by Value,
which is just an int, and then by Suit, which is an empty DU, which is turned
into int cases by the compiler, so they're sortable too.

A common collection function is Map, which is a function to take the current
item and turn it into something else.  For instance, we'll be taking a
collection of card tokens and MAPPING each into an actual card.

for example, squaresStr takes a list of the integers 0 through 10 and MAPS each
to the string representation of that number squared.
let squaresStr = [0..10] |> List.map (fun x -> sprintf "%i" (x * x) )
*)
let parseHand (strHand:string) =
   let cardTokens = strHand.Split[| ' ' |]
   // the |> is a pipe, and it passes the result forward to be the final
   // parameter of the next expression.  This example is the equivalent of:
   // let temp1 = Array.map (fun) cardTokens
   // let temp2 = Array.sort temp1
   // --> change the mapping to call our parseCard function
   let cards = cardTokens
               |> Array.map (fun token -> Value.Squared1, Suit.Suit1 )
               |> Array.sort
   // now we'll check for duplicates by
   printfn "Hand is: %A" cards
   13

testFunc "foo" parseHand "2C 4H 3D 6S KC"
