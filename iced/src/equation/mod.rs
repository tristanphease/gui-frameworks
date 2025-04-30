use std::fmt::Display;

pub mod simple;

pub trait Equation<T>: Display {
    fn calc_value(&self) -> T;
}
