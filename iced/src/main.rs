use equation::simple::{new_simple_equation, SimpleEquation};
use iced::{
    widget::{button, column, container, pane_grid, text}, Border, Color, Element, Length, Theme
};

mod equation;

fn main() -> iced::Result {
    iced::run("Number Pain", update, view)
}

#[derive(Debug, Default)]
struct Model {
    current_equation: Option<SimpleEquation>,
}

fn update(model: &mut Model, message: Message) {
    match message {
        Message::StartGame => {
            if model.current_equation.is_none() {
                model.current_equation = Some(new_simple_equation());
            }
        }
    }
}

fn view(model: &Model) -> Element<Message> {
    if let Some(current_equation) = &model.current_equation {
        text("todo").into()
    } else {
        column![
            button(text("Start game"))
                .on_press(Message::StartGame)
        ].into()
    }
}

#[derive(Debug, Clone)]
enum Message {
    StartGame,
}

