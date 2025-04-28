use iced::{widget::{container, pane_grid::{self, Content}, text}, Border, Color, Element, Theme};

use crate::Message;


#[derive(Debug, Default)]
pub struct NumberPane {
    value: u32,
    color: Color,
}

impl NumberPane {
    pub fn new(size: u32) -> Self {
        Self {
            value: size,
            color: Color::WHITE,
        }
    }

    pub fn view<'a>(&self) -> Element<'a, Message> 
    {
        text(self.value.to_string()).color(Color::BLACK).into()
    }

    pub fn view_content<'a>(&'a self) -> Content<'a, Message> 
    {
        pane_grid::Content::new(self.view())
            .style(|theme: &Theme| {
                container::Style::default()
                    .background(self.color)
                    .border(Border {
                        color: Color::BLACK,
                        ..Default::default()
                    })
            })
    }
}