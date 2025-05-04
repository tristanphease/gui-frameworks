use rand::{
    distr::{Distribution, StandardUniform},
    seq::IndexedRandom,
};

use super::Equation;
use std::fmt::Display;

pub fn new_medium_equation() -> EquationValue {
    let depth = rand::random_range(1..2);

    let equation_node = generate_equation_backwards(depth);
    EquationValue {
        node: equation_node,
    }
}

pub fn new_complex_equation() -> EquationValue {
    let depth = rand::random_range(2..4);

    let equation_node = generate_equation_backwards(depth);
    EquationValue {
        node: equation_node,
    }
}

fn generate_equation_backwards(depth: i32) -> Node {
    fn equation_value(depth: i32, end_value: f64) -> Node {
        if depth == 0 {
            Node::LeafNode(LeafNode::new(end_value))
        } else {
            generate_equation_backwards_value(depth - 1, end_value)
        }
    }

    fn generate_equation_backwards_value(depth: i32, end_value: f64) -> Node {
        let operator = rand::random::<DoubleOperator>();

        // construct value

        let (left_value, right_value) = match operator {
            DoubleOperator::Add => {
                let left_value = random_f64();
                let right_value = end_value - left_value;

                (left_value, right_value)
            }
            DoubleOperator::Subtract => {
                let left_value = random_f64();
                let right_value = end_value + left_value;

                (left_value, right_value)
            }
            DoubleOperator::Multiply => {
                // ab=c, we have c
                // need factors of c to get even numbers
                let factors = find_float_factors(end_value, 1);
                let (left_value, right_value) = match factors.choose(&mut rand::rng()) {
                    Some(factor_pair) => factor_pair,
                    None => unreachable!(),
                };

                (*left_value, *right_value)
            }
            DoubleOperator::Divide => {
                // a / b = c
                // need a to be a multiple of b
                // choose small value to make it even
                let right_value = random_f64_choose(50, 1);
                let left_value = right_value * end_value;

                (left_value, right_value)
            }
        };

        let left_node = equation_value(depth, left_value);
        let right_node = equation_value(depth, right_value);

        let double_tree_node = DoubleTreeNode {
            operator,
            left_value: Box::new(left_node),
            right_value: Box::new(right_node),
        };
        Node::TreeNode(TreeNode::DoubleTreeNode(double_tree_node))
    }

    let answer = random_f64();

    generate_equation_backwards_value(depth, answer)
}

fn find_float_factors(num: f64, precision: u32) -> Vec<(f64, f64)> {
    //
    let positive = match num.signum() {
        1.0 => true,
        -1.0 => false,
        _ => unreachable!(),
    };
    let precision_f64 = f64::from(10i32.pow(precision));
    // convert number to absolute number with precision, e.g. -16.4 with precision 1 -> 164
    let num_with_precision = num.abs() * precision_f64;
    // the highest number we need to check for factors is the square root
    let max_num = num_with_precision.sqrt() as i32 + 1;
    let num_with_precision = num_with_precision as i32;
    let mut factors = vec![];
    for value in 1..max_num {
        // check if it's a factor
        if num_with_precision % value == 0 {
            let factor_1 = value;
            let factor_2 = num_with_precision / value;
            let factor_1_precision = f64::from(factor_1) / precision_f64;
            let factor_2_precision = f64::from(factor_2) / precision_f64;

            let (sign_1, sign_2) = match (positive, rand::random()) {
                (true, true) => (1.0, 1.0),
                (true, false) => (-1.0, -1.0),
                (false, true) => (-1.0, 1.0),
                (false, false) => (1.0, -1.0),
            };
            factors.push((factor_1_precision * sign_1, factor_2_precision * sign_2));
        }
    }

    factors
}

// unused, old version of equation
#[allow(dead_code)]
fn generate_equation(depth: i32) -> Node {
    if rand::random_ratio(1, 6) {
        //squared
        Node::LeafNode(LeafNode::new(random_f64()))
    } else {
        let operator = rand::random::<DoubleOperator>();

        let left_child = if depth == 0 || rand::random() {
            Node::LeafNode(LeafNode::new(random_f64()))
        } else {
            generate_equation(depth - 1)
        };

        let right_child = if depth > 0 {
            Node::LeafNode(LeafNode::new(random_f64()))
        } else {
            generate_equation(depth - 1)
        };

        let double_tree_node = DoubleTreeNode {
            operator,
            left_value: Box::new(left_child),
            right_value: Box::new(right_child),
        };
        Node::TreeNode(TreeNode::DoubleTreeNode(double_tree_node))
    }
}

fn random_f64() -> f64 {
    f64::from(rand::random_range(0..1000) - 500) * 0.1
}

fn random_f64_choose(max: i32, precision: u32) -> f64 {
    f64::from(rand::random_range(-max..max)) / f64::from(10i32.pow(precision))
}

#[derive(Debug)]
pub struct EquationValue {
    node: Node,
}

impl Display for EquationValue {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        write!(f, "{}", self.node)
    }
}

impl Equation for EquationValue {
    fn calc_value(&self) -> f64 {
        match &self.node {
            Node::LeafNode(leaf_node) => leaf_node.calc_value(),
            Node::TreeNode(tree_node) => tree_node.calc_value(),
        }
    }
}

#[derive(Debug)]
enum Node {
    LeafNode(LeafNode),
    TreeNode(TreeNode),
}

impl Display for Node {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        match self {
            Node::LeafNode(leaf_node) => write!(f, "{}", leaf_node),
            Node::TreeNode(tree_node) => write!(f, "{}", tree_node),
        }
    }
}

impl Equation for Node {
    fn calc_value(&self) -> f64 {
        match self {
            Node::LeafNode(leaf_node) => leaf_node.calc_value(),
            Node::TreeNode(tree_node) => tree_node.calc_value(),
        }
    }
}

#[derive(Debug)]
struct LeafNode {
    value: f64,
}

impl LeafNode {
    fn new(value: f64) -> Self {
        LeafNode { value }
    }
}

impl Display for LeafNode {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        write!(f, "{:.1}", self.value)
    }
}

impl Equation for LeafNode {
    fn calc_value(&self) -> f64 {
        self.value
    }
}

#[derive(Debug)]
enum TreeNode {
    DoubleTreeNode(DoubleTreeNode),
    SingleTreeNode(SingleTreeNode),
}

impl Display for TreeNode {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        match self {
            TreeNode::DoubleTreeNode(double_tree_node) => write!(f, "{}", double_tree_node),
            TreeNode::SingleTreeNode(single_tree_node) => write!(f, "{}", single_tree_node),
        }
    }
}

impl Equation for TreeNode {
    fn calc_value(&self) -> f64 {
        match self {
            TreeNode::DoubleTreeNode(double_tree_node) => double_tree_node.calc_value(),
            TreeNode::SingleTreeNode(single_tree_node) => single_tree_node.calc_value(),
        }
    }
}

#[derive(Debug)]
struct DoubleTreeNode {
    operator: DoubleOperator,
    left_value: Box<Node>,
    right_value: Box<Node>,
}

impl Display for DoubleTreeNode {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        match self.operator {
            DoubleOperator::Add => write!(f, "{}+{}", self.left_value, self.right_value),
            DoubleOperator::Subtract => write!(f, "{}-{}", self.left_value, self.right_value),
            DoubleOperator::Multiply => write!(f, "{}×{}", self.left_value, self.right_value),
            DoubleOperator::Divide => write!(f, "({})÷({})", self.left_value, self.right_value),
        }
    }
}

impl Equation for DoubleTreeNode {
    fn calc_value(&self) -> f64 {
        let left_value = self.left_value.calc_value();
        let right_value = self.right_value.calc_value();
        match self.operator {
            DoubleOperator::Add => left_value + right_value,
            DoubleOperator::Subtract => left_value - right_value,
            DoubleOperator::Multiply => left_value * right_value,
            DoubleOperator::Divide => left_value / right_value,
        }
    }
}

#[derive(Debug)]
struct SingleTreeNode {
    operator: SingleOperator,
    value: Box<TreeNode>,
}

impl Display for SingleTreeNode {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        match self.operator {
            SingleOperator::Brackets => write!(f, "({})", self.value),
            SingleOperator::Square => write!(f, "{}²", self.value),
        }
    }
}

impl Equation for SingleTreeNode {
    fn calc_value(&self) -> f64 {
        match self.operator {
            SingleOperator::Brackets => self.value.calc_value(),
            SingleOperator::Square => {
                let sub_value = self.value.calc_value();
                sub_value * sub_value
            }
        }
    }
}

#[derive(Debug)]
enum DoubleOperator {
    Add,
    Subtract,
    Multiply,
    Divide,
}

impl Distribution<DoubleOperator> for StandardUniform {
    fn sample<R: rand::Rng + ?Sized>(&self, rng: &mut R) -> DoubleOperator {
        match rng.random_range(0..4) {
            0 => DoubleOperator::Add,
            1 => DoubleOperator::Subtract,
            2 => DoubleOperator::Multiply,
            3 => DoubleOperator::Divide,
            _ => unreachable!(),
        }
    }
}

#[derive(Debug)]
enum SingleOperator {
    Brackets,
    Square,
}
