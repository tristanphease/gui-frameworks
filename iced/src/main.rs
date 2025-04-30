use equation::{simple::{new_simple_equation, SimpleEquation}, Equation};
use iced::{
    Element,
    widget::{button, column, text, text_input},
};

pub mod equation;

fn main() -> iced::Result {
    iced::run("Number Pain", update, view)
}

/// The state of the program
#[derive(Debug, Default)]
struct Model {
    program_state: ProgramState,
}

#[derive(Debug, Default, PartialEq, Eq)]
enum ProgramState {
    #[default]
    MainMenu,
    Equation(EquationModelState),
    AnsweredEquation(bool),
}

/// The state of the equation model
#[derive(Debug, PartialEq, Eq)]
struct EquationModelState {
    simple_equation: SimpleEquation,
    current_answer_text: String,
}

fn new_equation_model_state() -> EquationModelState {
    let simple_equation = new_simple_equation();

    EquationModelState {
        simple_equation,
        current_answer_text: String::new(),
    }
}

fn update(model: &mut Model, message: Message) {
    match message {
        Message::StartGame => {
            if model.program_state == ProgramState::MainMenu {
                model.program_state = ProgramState::Equation(new_equation_model_state());
            }
        }
        Message::ChangeAnswer(new_answer) => {
            if let ProgramState::Equation(equation_state) = &mut model.program_state {
                equation_state.current_answer_text = new_answer;
            }
        }
        Message::SubmitAnswer => {
            if let ProgramState::Equation(equation_state) = &mut model.program_state {
                let value_parsed = equation_state.current_answer_text.parse::<i32>();
                let value_correct = if let Ok(value) = value_parsed {
                    if value == equation_state.simple_equation.calc_value() {
                        true
                    } else {
                        false
                    }
                } else {
                    false
                };

                model.program_state = ProgramState::AnsweredEquation(value_correct);
            }
        },
        Message::ReturnMenu => model.program_state = ProgramState::MainMenu,
    }
}

fn view(model: &Model) -> Element<Message> {
    match &model.program_state {
        ProgramState::MainMenu => {
            column![button(text("Start game")).on_press(Message::StartGame)].into()
        },
        ProgramState::Equation(equation_state) => {
            column![
                text(format!("{}", equation_state.simple_equation)),
                text_input("Answer", &equation_state.current_answer_text)
                    .on_input(Message::ChangeAnswer),
                button(text("Submit")).on_press(Message::SubmitAnswer)
            ]
            .into()
        },
        ProgramState::AnsweredEquation(correct) => {
            let answer_text = if *correct {
                "Correct!"
            } else {
                "Incorrect :("
            };

            column![
                text(answer_text),
                button("Return to menu")
                    .on_press(Message::ReturnMenu)
            ].into()
        },
    }
}

#[derive(Debug, Clone)]
enum Message {
    StartGame,
    ChangeAnswer(String),
    SubmitAnswer,
    ReturnMenu,
}
