# Thoughts on using Iced

Quite nice, most of the basic things are pretty simple. Cool for building native apps.

## Good things

It is enjoyable to use Rust, even if this domain isn't really suited to UI in general since there's an extra compile step. I enjoy the elm architecture and it makes a lot of sense for constructing UI, doing the thing where it drives you towards writing well structured code.

## Bad things

Seems to have the issue where things are either trivially easy to make since they're built-in or extremely complicated.

One issue with the elm architecture is there's no way to tie certain messages to model states, which means there are invalid states such as getting a message that's invalid for the current model.

There's no guide for Iced, so had to work things oout mostly by looking at examples and the docs.rs page. Was mostly pretty intuitive though, seems to be similar to html formatting.

| Category | Rating (out of 5) |
| -- | -- |
| Easy to setup | 2 |
| Fun | 3 |
| Effectiveness | 4 |
| Easy to learn | 3 |
| Reliable code | 5 |