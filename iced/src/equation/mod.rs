use std::fmt::{Debug, Display};

pub mod medium;
pub mod simple;

pub trait Equation: Display + Debug {
    fn calc_value(&self) -> f64;

    fn compare_value(&self, value: f64) -> bool {
        f64::abs(self.calc_value() - value) < 0.001
    }
}
