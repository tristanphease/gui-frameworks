
pub mod simple;

trait Equation {
    fn display_text<'a>() -> &'a str;
}