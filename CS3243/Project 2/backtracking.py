from __future__ import annotations
from typing import List, Dict, Set, Tuple, Any

class Assignment:
    def __init__(self, domains):
        # Store all the domains as a dictionary.
        self.domains = domains
        # Store variable assignments as a dictionary.
        self.assigned_var = {}
        # Create stack to track backtracking.
        self.undo_stack = []    # (Xj, neighbour_val)

def select_unassigned_variable(assignment: Assignment) -> str:
    # Select the unassigned variable with the minimum remaining values (MRV).
    unassigned = {var: len(domain) for var, domain in assignment.domains.items() if var not in assignment.assigned_var}
    return min(unassigned, key=unassigned.get)

def order_domain_values(constraints: Dict[Tuple[str, str], Any], var: str, assignment: Assignment) -> List:
    # Order the values in the domains based on the least constraining value heuristic.
 
    def count_constrained(value: int) -> int:
        count = 0
        # Iterate through each constraint.
        for (Xi, Xj), constraint in constraints.items():
            if Xi == var:
                for val in assignment.domains[Xj]:
                    # Check if the constraint is violated.
                    if not constraint(value, val):
                        count += 1  # If violated, increase the count.
        return count

    # Sort the domain values based on the least constraining values.
    return sorted(assignment.domains[var], key=count_constrained)

def is_consistent(var: str, value: int, assignment: Assignment, constraints: Dict[Tuple[str, str], Any]) -> bool:
    # Check if assigning the value to the variable is consistent with current assignments.
    for assigned_var, assigned_value in assignment.assigned_var.items():
        constr = constraints.get((var, assigned_var)) or constraints.get((assigned_var, var))
        if constr:
            # If it does not match constraint, return False.
            if (var, assigned_var) in constraints and not constr(value, assigned_value) or \
                (assigned_var, var) in constraints and not constr(assigned_value, value):
                return False
    return True
 
def forward_checking(var: str, value: int, assignment: Assignment, constraints: Dict[Tuple[str, str], Any]) -> bool:
    for (Xi, Xj), constraint in constraints.items():
        if Xi == var and Xj not in assignment.assigned_var:
            # Check each value in the domain of the unassigned variable.
            for neighbor_value in assignment.domains[Xj][:]:
                if not constraint(value, neighbor_value):
                    # Remove the constrained value from the domain.
                    assignment.domains[Xj].remove(neighbor_value)
                    # Add to undo stack.
                    assignment.undo_stack.append((Xj, neighbor_value))
            # If the domain of the unassigned variable is now empty, return False.
            if not assignment.domains[Xj]:
                return False
    return True

def backtrack(assignment: Assignment, constraints: Dict[Tuple[str, str], Any]) -> Dict[str, int] | None:
    # Backtracking algorithm, taken from lecture notes.
    # In order to do forward-checking, a stack is used instead of a deepcopy/normal iteration since it is more efficient and less expensive recursively.

    if len(assignment.assigned_var) == len(assignment.domains):
        return assignment.assigned_var
    var = select_unassigned_variable(assignment)
    for value in order_domain_values(constraints, var, assignment):
        if is_consistent(var, value, assignment, constraints):
            assignment.assigned_var[var] = value
            
            # Track of the size of the undo stack before forward checking.
            stack_size_before = len(assignment.undo_stack)

            if forward_checking(var, value, assignment, constraints):
                result = backtrack(assignment, constraints)
                if result:
                    return result

            # Before backtracking, restore the domain using the undo stack.
            while len(assignment.undo_stack) > stack_size_before:
                removed_var, removed_value = assignment.undo_stack.pop()
                assignment.domains[removed_var].append(removed_value)
            assignment.assigned_var.pop(var)

    return None
 
def solve_CSP(dct: Dict[str, Any]) -> Dict[str, int]:
    # Initialize assignment with domains.
    assignment = Assignment(dct['domains'])
    constraints = dct['constraints']
 
    # Start the backtracking search.
    solutions = backtrack(assignment, constraints)
 
    # Sort the dictionary and return it.
    if solutions is not None:
        solution_keys = list(solutions.keys())
        solution_keys.sort()
        sorted_solutions = {key: solutions.get(key) for key in solution_keys}
        return sorted_solutions
    else: 
        return None
    