from __future__ import annotations
from typing import List, Dict, Tuple, Any

class Square:
    def __init__(self, size=0, coord: Tuple[int, int] = (0, 0)):
        self.size = size
        self.x = coord[0]
        self.y = coord[1]

    def set_coordinates(self, x: int, y: int) -> None:
        # Sets the coordinates of the square.
        self.x = x
        self.y = y

    def get_coordinates(self) -> Tuple[int, int]:
        # Returns the coordinates of the square.
        return (self.x, self.y)

class Board:
    def __init__(self, rows: int, cols: int, obstacles: List[Tuple[int, int]]):
        self.width = cols
        self.height = rows
        self.obstacles = obstacles
        self.matrix = self._create_matrix()

    def _create_matrix(self) -> List[List[str]]:
        # Creates a matrix to represent the board. Fills it with "0" and places "X" for obstacles.
        matrix = [["0" for _ in range(self.width)] for _ in range(self.height)]
        for x, y in self.obstacles:
            matrix[x][y] = "X"
        return matrix

def initialize_domains(board: Board, input_squares: Dict[int, int]) -> Dict[Square, List[Tuple[int, int]]]:
    # Initializes domains (list of coordinates to place squares) for each square type. 
    rows, cols = board.height, board.width
    domains = {}
    for size, count in input_squares.items():
        for _ in range(count):
            square = Square(size)
            # List of all valid (x, y) coordinates where a square can be placed on the board without overlapping with any obstacles.
            possible_locations = [(row, col) for row in range(rows - size + 1) for col in range(cols - size + 1) if not any("X" in board.matrix[row + k][col:col + size] for k in range(size))]
            domains[square] = possible_locations
    return domains

def solve_CSP(dct: Dict[str, Any]) -> List[Square]:
    board = Board(dct["rows"], dct["cols"], dct["obstacles"])
    domains = initialize_domains(board, dct["input_squares"])
    assigned = []  # List of squares that have been assigned coordinates.
    undo_stack = []  # Stack to keep track of changes for backtracking.

    def is_consistent(square: Square, x: int, y: int) -> bool:
        # Checks if placing the square at coordinates (x, y) is consistent with the current board state.
        size = square.size
        return not any("1" in board.matrix[row][y:y + size] for row in range(x, x + size))

    def forward_checking(square: Square, x: int, y: int) -> bool:
        # Updates the domains of other squares based on placing the given square at (x, y).
        # Returns False if any domain becomes empty.
        for s, coords in domains.items():
            new_coords = [coord for coord in coords if not (coord[0] + s.size > x and coord[0] < x + square.size and coord[1] + s.size > y and coord[1] < y + square.size)]
            if len(new_coords) < len(coords):
                undo_stack.append((s, list(set(coords) - set(new_coords))))
                domains[s] = new_coords
            if not domains[s]:
                return False
        return True

    def run_csp() -> List[Square] or False:
        # Recursive CSP backtracking algorithm.
        if not domains:
            return assigned

        # Selects the next square to be assigned based on the Minimum Remaining Values heuristic.
        square = min(domains.keys(), key=lambda x: len(domains[x]), default=None) 
        if not square:
            return False
        
        for x, y in domains[square]:
            # Check if can place square without violating constraint.
            if is_consistent(square, x, y):
                # If can, set it as assigned and update matrix.
                square.set_coordinates(x, y)
                assigned.append(square)
                for i in range(x, x + square.size):
                    for j in range(y, y + square.size):
                        board.matrix[i][j] = "1"

                # Store state of undo stack before forward checking.
                stack_size_before = len(undo_stack)
                coords_to_remove = domains[square]
                del domains[square] # Remove the current square's domain since it's now assigned.

                if forward_checking(square, x, y):
                    result = run_csp()
                    if result:
                        return result

                # Backtrack if solution is not found.
                assigned.pop()
                for i in range(x, x + square.size):
                    for j in range(y, y + square.size):
                        board.matrix[i][j] = "0"

                # Restore domains using the undo stack.
                domains[square] = coords_to_remove
                while len(undo_stack) > stack_size_before:
                    s, coords = undo_stack.pop()
                    if s in domains:
                        domains[s].extend(coords)
                    else:
                        domains[s] = coords
        return False
    
    assignments = run_csp()
    return [(square.size, square.x, square.y) for square in assignments]
