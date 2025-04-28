
// simple equation

use std::fmt::Display;

use rand::distr::{Distribution, StandardUniform};

#[derive(Debug)]
pub struct SimpleEquation {
    base_value: i32,
    equation_values: Vec<EquationValue>
}

impl SimpleEquation {
    fn new(base: i32, operator: BasicOperator, second_value: i32) -> Self {
        Self {
            base_value: base,
            equation_values: vec![EquationValue::new(operator, second_value)],
        }
    }
}

impl Display for SimpleEquation {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        write!(f, "{}", self.base_value)?;
        for equation_val in &self.equation_values {
            write!(f, " {} {}", equation_val.operator, equation_val.value)?;
        }
        Ok(())
    }
}

pub fn new_simple_equation() -> SimpleEquation {
    let base_value = rand::random_range(-10..=10);
    let second_value = rand::random_range(-10..=10);

    let operator = rand::random::<BasicOperator>();

    SimpleEquation::new(base_value, operator, second_value)
}

#[derive(Debug)]
struct EquationValue {
    operator: BasicOperator,
    value: i32,
}

impl EquationValue {
    fn new(operator: BasicOperator, value: i32) -> Self {
        Self {
            operator,
            value,
        }
    }
}

#[derive(Debug)]
enum BasicOperator {
    Multiply,
    Add,
    Subtract,
}

impl Display for BasicOperator {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        let operator_char = match self {
            BasicOperator::Multiply => "×",
            BasicOperator::Add => "+",
            BasicOperator::Subtract => "-",
        };
        write!(f, "{}", operator_char)
    }
}

impl Distribution<BasicOperator> for StandardUniform {
    fn sample<R: rand::Rng + ?Sized>(&self, rng: &mut R) -> BasicOperator {
        match rng.random_range(0..3) {
            0 => BasicOperator::Multiply,
            1 => BasicOperator::Add,
            2 => BasicOperator::Subtract,
            _ => panic!("Invalid operator"),
        }
    }
}