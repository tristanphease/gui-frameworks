# Thoughts on using Bolero

Wanted to try out F#, although a web framework is a strange thing to do in F#.
Considered [bolero](https://fsbolero.io/), but seemed too similar to Elm, so was trying out [Falco](https://www.falcoframework.com/) instead. However it doesn't have a way to generate a static site which is lame to me. Plus I really want to mess around with WASM more, so switched back to bolero.

Maybe blazor would be easier/better but I don't want to use C#.

## Good things

F#  
The type system is good since it's an ML family language. However sometimes the dotnet stuff leaks through and it isn't as purely functional at times.

Bolero  
The templating system is really nice; HTML is actually a good language for describing UIs when you don't have styling or scripting required, and the template works well for scripting with F#. A lot of the design is clean and quite comfortable to use.

## Bad things

F#
The type inference isn't always helpful when learning since I want the types to constrain what I do and make the 
Using the indexes with the lists wasn't the cleanest thing to do but I had to use lists since arrays are fixed size. Would prefer a different data structure. Wonder if a language has a convenient way of dealing with that kind of thing. The issue is that it can't necessarily be a reference because that can get moved out from under me since I need to mutate the list. Maybe encapsulation is the way to deal with it where you have the logic but as private variables and the outside world can only interact with the public interface...

Bolero   
The javascript interop is a bit of a mess and the docs don't help with much outside the basics, instead need to go trawling for examples for what to do.

| Category | Rating (out of 5) |
| -- | -- |
| Easy to setup | 3 |
| Fun | 3 |
| Effectiveness | 4 |
| Easy to learn | 3 |
| Reliable code | 4 |