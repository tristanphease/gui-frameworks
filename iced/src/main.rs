use std::fmt::Display;
// use iced::time::{Duration,Instant};
use wasm_timer::Instant;
use std::time::Duration;

use equation::{
    Equation,
    medium::{new_complex_equation, new_medium_equation},
    simple::new_simple_equation,
};
use iced::{
    Background, Color, Element, Font, Length, Padding, Subscription,
    font::Weight,
    gradient::Linear,
    time,
    widget::{Button, button, column, container, row, text, text_input},
};

pub mod equation;

fn main() -> iced::Result {
    iced::application("Number Pain", update, view)
        .subscription(subscription)
        .run()
}

/// The state of the program
#[derive(Debug, Default)]
struct Model {
    program_state: ProgramState,
    timer: Timer,
}

#[derive(Debug)]
struct Timer {
    time_taken: Duration,
    last_tick: Option<Instant>,
}

impl Default for Timer {
    fn default() -> Self {
        Self {
            time_taken: Default::default(),
            last_tick: Default::default(),
        }
    }
}

impl Timer {
    fn add_instant(&mut self, current_tick: Instant) {
        if let Some(last_tick) = self.last_tick {
            self.time_taken += current_tick - last_tick;
        }
        self.last_tick = Some(current_tick);
    }
}

#[derive(Debug)]
enum ProgramState {
    MainMenu(EquationDifficulty),
    Equation(EquationModelState),
    FinishedEquation(EquationFinishedState),
}

impl Default for ProgramState {
    fn default() -> Self {
        Self::MainMenu(EquationDifficulty::Simple)
    }
}

#[derive(Debug)]
struct EquationFinishedState {
    equation_progress: EquationProgress,
    equation_difficulty: EquationDifficulty,
    ended_early: bool,
    background_colors: Vec<Color>,
}

/// The state of the equation model
#[derive(Debug)]
struct EquationModelState {
    current_equation: Box<dyn Equation>,
    equation_difficulty: EquationDifficulty,
    current_answer_text: String,
    equation_progress: EquationProgress,
}

#[derive(Debug, Clone, Copy)]
struct EquationProgress {
    number_equations: u32,
    equations_success: u32,
    equations_completed: u32,
}

impl EquationProgress {
    fn new(number: u32) -> Self {
        Self {
            number_equations: number,
            equations_success: 0,
            equations_completed: 0,
        }
    }

    fn add_equation(&mut self, success: bool) {
        self.equations_completed += 1;
        if success {
            self.equations_success += 1;
        }
    }

    fn completed(&self) -> bool {
        self.equations_completed >= self.number_equations
    }
}

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
enum EquationDifficulty {
    Simple,
    Medium,
    Complex,
}

impl Display for EquationDifficulty {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        let string = match self {
            EquationDifficulty::Simple => "Simple",
            EquationDifficulty::Medium => "Medium",
            EquationDifficulty::Complex => "Complex",
        };
        write!(f, "{}", string)
    }
}

fn new_equation_model_state(equation_difficulty: EquationDifficulty) -> EquationModelState {
    let equation: Box<dyn Equation> = new_equation(equation_difficulty);

    let num_equations = 10;

    EquationModelState {
        current_equation: equation,
        equation_difficulty,
        current_answer_text: String::new(),
        equation_progress: EquationProgress::new(num_equations),
    }
}

fn new_equation(equation_difficulty: EquationDifficulty) -> Box<dyn Equation> {
    match equation_difficulty {
        EquationDifficulty::Simple => Box::new(new_simple_equation()),
        EquationDifficulty::Medium => Box::new(new_medium_equation()),
        EquationDifficulty::Complex => Box::new(new_complex_equation()),
    }
}

fn update(model: &mut Model, message: Message) {
    match message {
        Message::StartGame => match &model.program_state {
            ProgramState::MainMenu(equation_difficulty) => {
                model.program_state =
                    ProgramState::Equation(new_equation_model_state(*equation_difficulty))
            }
            ProgramState::FinishedEquation(finished_equation_state) => {
                model.program_state = ProgramState::Equation(new_equation_model_state(
                    finished_equation_state.equation_difficulty,
                ))
            }
            _ => {}
        },
        Message::ChangeAnswer(new_answer) => {
            if let ProgramState::Equation(equation_state) = &mut model.program_state {
                equation_state.current_answer_text = new_answer;
            }
        }
        Message::SubmitAnswer => {
            if let ProgramState::Equation(equation_state) = &mut model.program_state {
                let value_parsed = equation_state.current_answer_text.parse::<f64>();
                let value_correct = if let Ok(value) = value_parsed {
                    equation_state.current_equation.compare_value(value)
                } else {
                    false
                };

                equation_state.equation_progress.add_equation(value_correct);
                equation_state.current_answer_text = String::new();

                if equation_state.equation_progress.completed() {
                    let equation_finished = EquationFinishedState {
                        equation_progress: equation_state.equation_progress.clone(),
                        equation_difficulty: equation_state.equation_difficulty,
                        ended_early: false,
                        background_colors: generate_finish_menu_colors(),
                    };
                    model.program_state = ProgramState::FinishedEquation(equation_finished);
                } else {
                    equation_state.current_equation =
                        new_equation(equation_state.equation_difficulty);
                }
            }
        }
        Message::ReturnMenu => {
            model.program_state = ProgramState::MainMenu(EquationDifficulty::Simple)
        }
        Message::ChangeDifficulty(equation_difficulty) => match model.program_state {
            ProgramState::MainMenu(_) => {
                model.program_state = ProgramState::MainMenu(equation_difficulty)
            }
            _ => {}
        },
        Message::CancelEquation => match &model.program_state {
            ProgramState::Equation(equation_model_state) => {
                let equation_finished = EquationFinishedState {
                    equation_progress: equation_model_state.equation_progress.clone(),
                    equation_difficulty: equation_model_state.equation_difficulty,
                    ended_early: true,
                    background_colors: generate_finish_menu_colors(),
                };
                model.program_state = ProgramState::FinishedEquation(equation_finished);
            }
            _ => {}
        },
        Message::Tick(now) => {
            model.timer.add_instant(now);
        }
    }
}

fn view(model: &Model) -> Element<Message> {
    match &model.program_state {
        ProgramState::MainMenu(equation_difficulty) => container(column![
            container(column![
                text("Number Pain").size(50),
                text("Test your numerical calculation skills!").size(20)
            ])
            .center_x(Length::Fill),
            container(row![
                difficulty_button(EquationDifficulty::Simple, *equation_difficulty),
                difficulty_button(EquationDifficulty::Medium, *equation_difficulty),
                difficulty_button(EquationDifficulty::Complex, *equation_difficulty),
            ])
            .center_x(Length::Fill)
            .padding(Padding::from(10)),
            container(button(text("Start game")).on_press(Message::StartGame))
                .center_x(Length::Fill),
        ])
        .style(|_theme| {
            container::Style::default()
                .background(start_menu_background(model.timer.time_taken.as_secs_f32()))
        })
        .center_x(Length::Fill)
        .center_y(Length::Fill)
        .into(),
        ProgramState::Equation(equation_state) => container(column![
            // main
            container(
                container(
                    column![
                        container(text("What's the answer?").size(30)).center_x(Length::Fill),
                        container(text(format!("{}", equation_state.current_equation)).size(25))
                            .center_x(Length::Fill),
                        container(
                            text_input("Answer", &equation_state.current_answer_text)
                                .on_input(Message::ChangeAnswer)
                                .on_submit(Message::SubmitAnswer)
                                .size(20)
                                .padding(5)
                        )
                        .center_x(Length::Shrink),
                        container(button(text("Submit")).on_press(Message::SubmitAnswer))
                            .center_x(Length::Fill)
                    ]
                    .spacing(20)
                )
                .max_width(400)
            )
            .center_x(Length::Fill)
            .center_y(Length::Fill),
            container(column![
                text(format!(
                    "Completed {}/{}",
                    equation_state.equation_progress.equations_completed,
                    equation_state.equation_progress.number_equations
                )),
                container(button("End early").on_press(Message::CancelEquation))
            ])
            .align_right(Length::Fill)
        ])
        .into(),
        ProgramState::FinishedEquation(finished_state) => {
            let header_text = if !finished_state.ended_early {
                "Finished!"
            } else {
                "Ended"
            };
            let score_text = format!(
                "Scored {}/{}",
                finished_state.equation_progress.equations_success,
                finished_state.equation_progress.equations_completed
            );
            container(
                container(
                    column![
                        text(header_text).size(35),
                        text(score_text).size(20),
                        row![
                            button("Start again").on_press(Message::StartGame),
                            button("Return to start menu").on_press(Message::ReturnMenu)
                        ]
                        .spacing(20)
                    ]
                    .spacing(10),
                )
                .max_width(400),
            )
            .style(|_theme| {
                container::Style::default().background(finish_menu_background(
                    model.timer.time_taken.as_secs_f32(),
                    &finished_state.background_colors,
                ))
            })
            .center(Length::Fill)
            .into()
        }
    }
}

fn start_menu_background(time_taken: f32) -> Background {
    Background::Gradient(iced::Gradient::Linear(
        Linear::new(time_taken * 0.1)
            .add_stop(0.0, Color::from_rgb(0.2, 0.2, 0.3))
            .add_stop(0.5, Color::from_rgb(0.1, 0.1, 0.2))
            .add_stop(1.0, Color::from_rgb(0.2, 0.2, 0.3)),
    ))
}

fn finish_menu_background(_time_taken: f32, colors: &Vec<Color>) -> Background {
    let mut linear_gradient = Linear::new(std::f32::consts::FRAC_PI_3);
    let colors_len = (colors.len() - 1) as f32;
    for (index, color) in colors.iter().enumerate() {
        let index_f32 = index as f32;
        linear_gradient = linear_gradient.add_stop(index_f32 / colors_len, *color);
    }
    Background::Gradient(iced::Gradient::Linear(linear_gradient))
}

fn generate_finish_menu_colors() -> Vec<Color> {
    let mut colors = vec![Color::default(); 8];
    for color in colors.iter_mut() {
        *color = random_color();
    }
    colors
}

fn random_color() -> Color {
    let rand_dark = || rand::random::<f32>() * 0.3;
    Color::from_rgb(rand_dark(), rand_dark(), rand_dark())
}

fn difficulty_button<'a>(
    difficulty: EquationDifficulty,
    current_difficulty: EquationDifficulty,
) -> Button<'a, Message> {
    let font = Font {
        weight: Weight::Bold,
        ..Default::default()
    };
    button(text(format!("{}", difficulty)).font(font))
        .on_press(Message::ChangeDifficulty(difficulty))
        .style(move |_theme, _status| {
            button::Style::default().with_background(if difficulty == current_difficulty {
                Color::from_rgb(0.8, 0.2, 0.2)
            } else {
                Color::from_rgb(0.5, 0.2, 0.2)
            })
        })
}

fn subscription(_model: &Model) -> Subscription<Message> {
    time::every(time::Duration::from_millis(10)).map(Message::Tick)
}

#[derive(Debug, Clone)]
enum Message {
    StartGame,
    ChangeAnswer(String),
    SubmitAnswer,
    CancelEquation,
    ChangeDifficulty(EquationDifficulty),
    ReturnMenu,
    Tick(Instant),
}
