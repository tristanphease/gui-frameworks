// simple equation

use std::fmt::Display;

use rand::distr::{Distribution, StandardUniform};

use super::Equation;

#[derive(Debug, PartialEq, Eq)]
pub struct SimpleEquation {
    equation_value: EquationValue,
}

impl SimpleEquation {
    fn new(operator: BasicOperator, left: i32, right: i32) -> Self {
        Self {
            equation_value: EquationValue::new(operator, left, right),
        }
    }
}

impl Equation<i32> for SimpleEquation {
    fn calc_value(&self) -> i32 {
        self.equation_value.calc_value()
    }
}

impl Display for SimpleEquation {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        write!(f, "{}", self.equation_value)
    }
}

pub fn new_simple_equation() -> SimpleEquation {
    let left = rand::random_range(-10..=10);
    let right = rand::random_range(-10..=10);

    let operator = rand::random::<BasicOperator>();

    SimpleEquation::new(operator, left, right)
}

#[derive(Debug, PartialEq, Eq)]
struct EquationValue {
    operator: BasicOperator,
    value_left: i32,
    value_right: i32,
}

impl EquationValue {
    fn new(operator: BasicOperator, value_left: i32, value_right: i32) -> Self {
        Self {
            operator,
            value_left,
            value_right,
        }
    }
}

impl Equation<i32> for EquationValue {
    fn calc_value(&self) -> i32 {
        self.operator.calc(self.value_left, self.value_right)
    }
}

impl Display for EquationValue {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        write!(
            f,
            "{} {} {}",
            self.value_left, self.operator, self.value_right
        )
    }
}

#[derive(Debug, PartialEq, Eq)]
enum BasicOperator {
    Multiply,
    Add,
    Subtract,
}

impl BasicOperator {
    fn operation_order(&self) -> i32 {
        match self {
            BasicOperator::Add => 0,
            BasicOperator::Subtract => 0,
            BasicOperator::Multiply => 1,
        }
    }

    fn calc(&self, left: i32, right: i32) -> i32 {
        match self {
            BasicOperator::Multiply => left * right,
            BasicOperator::Add => left + right,
            BasicOperator::Subtract => left - right,
        }
    }
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
