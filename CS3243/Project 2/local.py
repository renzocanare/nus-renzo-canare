from __future__ import annotations
from typing import List, Dict, Tuple, NamedTuple
import random

class Move(NamedTuple):
    # Class to handle move of a square to a new position and its resulting value.
    square: Square
    new_x: int
    new_y: int
    value: int

class Square:
    def __init__(self, size: int, x: int, y: int):
        self.size = size
        self.x = x
        self.y = y

class Board:
    def __init__(self, width: int, height: int, squares: Dict[int, int]):
        self.width = width
        self.height = height
        self.matrix = [[0 for _ in range(width)] for _ in range(height)]  # Overlap matrix.
        self.initial_squares = squares
        self.squares = sorted(self.initialize_squares(squares), key=lambda x: x.size, reverse=True) # List of squares to place.
        self.unit_squares_count = 0  # Counter for unit squares.
        self.sideways_move_count = 0  # Counter for sideways move.
        self.value_cache = {}  # Cache the value of the board state.
        self.tabu_list = []  # Tabu list (suggestion from P2ST).
        self.best_value = float('inf')
        self.best_configuration = None

    def initialize_squares(self, squares: Dict[int, int]) -> List[Square]:
        # Initializes the squares for the board with random positions.
        square_list = []
        self.unit_squares_count = 0

        for size, count in squares.items():
            for _ in range(count):
                x, y = random.randint(0, self.height - size), random.randint(0, self.width - size)
                if size == 1:
                    # If unit square, don't need to save it. Just increase the count for it.
                    self.unit_squares_count += 1
                else:
                    square_list.append(Square(size, x, y))
        return square_list

    def random_restart(self):
        # Resets the board and randomly places the squares.
        self.matrix = [[0 for _ in range(self.width)] for _ in range(self.height)]
        self.squares = sorted(self.initialize_squares(self.initial_squares), key=lambda x: x.size, reverse=True)

    def _apply_to_matrix(self, matrix, square, x, y, operation):
        # Helper functions to perform add and remove operations on a matrix copy.
        for i in range(x, x + square.size):
            for j in range(y, y + square.size):
                matrix[i][j] += operation
        return matrix
    
    def add_unit_square(self, square: Square):
        # Adds a unit square to the board matrix.
        self.matrix[square.x][square.y] += 1

    def add_square(self, square: Square):
        # Adds a square to the board matrix.
        for i in range(square.x, square.x + square.size):
            for j in range(square.y, square.y + square.size):
                self.matrix[i][j] += 1

    def remove_square(self, square: Square):
        # Removes a square from the board matrix.
        for i in range(square.x, square.x + square.size):
            for j in range(square.y, square.y + square.size):
                self.matrix[i][j] -= 1

    def value(self) -> int:
        # Returns the total value of the board (lower is better).
        key = tuple(tuple(row) for row in self.matrix)
        if key in self.value_cache:
            return self.value_cache[key]
        
        # For positive cell values, decrement by 1. For zero or negative values, increment by 1.
        val = sum(1 + cell - 2 * (cell > 0) for row in self.matrix for cell in row)
        self.value_cache[key] = val
        return val

    def add_square_on_matrix(self, matrix, square, x, y):
        # Adds a square to a copied matrix.
        return self._apply_to_matrix(matrix, square, x, y, 1)

    def remove_square_on_matrix(self, matrix, square, x=None, y=None):
        # "Removes a square from a copied matrix.
        x = x or square.x
        y = y or square.y
        return self._apply_to_matrix(matrix, square, x, y, -1)

    def matrix_value(self, matrix) -> int:
        # Returns the total value of a copied matrix.
        # For positive cell values, decrement by 1. For zero or negative values, increment by 1.
        return sum(1 + cell - 2 * (cell > 0) for row in matrix for cell in row)

    def possible_moves(self, square: Square, current_value: int) -> List[Move]:
        # Get all possible moves, with consideration for the TabuList.

        # List to store potential moves for the square.
        moves = []

        directions = [(0, 1), (0, -1), (-1, 0), (1, 0)]
        random.shuffle(directions)  # Shuffle the directions for randomness in move choice.

        # Record the original position of the square.
        original_x, original_y = square.x, square.y

        # Create a temporary matrix copy to test potential moves without altering the real board.
        temp_matrix = [row.copy() for row in self.matrix]
        self.remove_square_on_matrix(temp_matrix, square)  # Remove the square from the temp matrix.

        # The maximum possible jump a square can take in any direction.
        max_jump = max(self.width, self.height)

        for dX, dY in directions:
            for jump in range(1, max_jump):
                # Calculate the new potential position.
                new_x, new_y = original_x + jump * dX, original_y + jump * dY

                # Check if the move is in the tabu list and if its value is not better than the best known.
                if (square, new_x, new_y) in self.tabu_list and self.matrix_value(temp_matrix) >= self.best_value:
                    continue 

                if 0 <= new_x <= self.height - square.size and 0 <= new_y <= self.width - square.size:   
                    # Add the square to the new position on the temp matrix.
                    self.add_square_on_matrix(temp_matrix, square, new_x, new_y)

                    # Evaluate the value of the temp matrix with the square in the new position.
                    move_value = self.matrix_value(temp_matrix)

                    # If this move worsens the board value compared to the current value, 
                    # remove the square and break out of the current jump loop.
                    if move_value > current_value:
                        self.remove_square_on_matrix(temp_matrix, square, new_x, new_y)
                        break

                    moves.append(Move(square, new_x, new_y, move_value))
                    # Remove the square from the temp matrix for the next iteration.
                    self.remove_square_on_matrix(temp_matrix, square, new_x, new_y)

        # Filter out moves that are in the TabuList or those whose value isn't better than the best known.
        valid_moves = [move for move in moves if (move.square, move.new_x, move.new_y) not in self.tabu_list or move.value < self.best_value]

        return valid_moves

    def conflicts(self, square: Square) -> Tuple[int, int]:
        # Determines the number of conflicts for a given square.
        conflict_count = 0
        for i in range(square.x, square.x + square.size):
            for j in range(square.y, square.y + square.size):
                if self.matrix[i][j] > 1:
                    conflict_count += 1
        return (conflict_count, square.size)

    def print_board(self):
        # Prints the current board configuration.
        for row in self.matrix:
            print(row)

    def print_result_matrix(self, result):
        # Prints the resulting board configuration after placing squares.
        matrix = [[0 for _ in range(self.width)] for _ in range(self.height)]
        for size, row, col in result:
            for i in range(row, row + size):
                for j in range(col, col + size):
                    matrix[i][j] = size
        for row in matrix:
            print(row)

def run_local(dct) -> List[Tuple[int, int, int]]:

    # Initialize board.
    width = dct['width']
    height = dct['height']
    input_squares = dct['input_squares']

    board = Board(width, height, input_squares)
    for square in board.squares:
        board.add_square(square)  

    # Calculate the current value (or score) of the board.
    current_value = board.value()

    # Set parameters for the search algorithm.
    max_sideways_moves = 10  # Maximum number of allowed sideways moves.
    max_tabu_size = 310  # TabuList size limit.

    while current_value > 0:
        improved = False  # Flag to track if an improvement was made in the current iteration.

        # Sort squares by number of conflicts. Those with higher conflicts are prioritized.
        squares_sorted_by_conflicts = sorted(board.squares, key=lambda sq: (-board.conflicts(sq)[0], -board.conflicts(sq)[1]))

        for square in squares_sorted_by_conflicts:
            # Find the best move for the current square.
            best_move = min(board.possible_moves(square, current_value), key=lambda m: m.value, default=None)

            # Check if the move is an improvement.
            if best_move and best_move.value < current_value:
                # Make the move.
                board.remove_square(square)
                square.x, square.y = best_move.new_x, best_move.new_y
                board.add_square(square)
                current_value = best_move.value
                improved = True

                # Add this move to the tabu list to prevent revisiting it (from P2ST).
                board.tabu_list.append((square, best_move.new_x, best_move.new_y))
                if len(board.tabu_list) > max_tabu_size:
                    board.tabu_list.pop(0)  # Remove the oldest entry if tabu list exceeds max size.

                # Update the best solution if this is a new best.
                if current_value < board.best_value:
                    board.best_value = current_value
                    board.best_configuration = [(sq.size, sq.x, sq.y) for sq in board.squares]
                break

            # If the move is a sideways move.
            elif best_move and best_move.value == current_value and current_value < 5:
                board.sideways_move_count += 1  # Increment the sideways move counter.
                board.remove_square(square)
                square.x, square.y = best_move.new_x, best_move.new_y
                board.add_square(square)
                improved = True
                break

        # Check if there were no improvements or if max sideways moves are reached.
        if not improved or board.sideways_move_count > max_sideways_moves:
            # If only unit squares are left to be placed:
            if board.unit_squares_count == current_value:
                # Place all the remaining unit squares in the empty slots.
                for _ in range(board.unit_squares_count):
                    for i in range(board.height):
                        for j in range(board.width):
                            if board.matrix[i][j] == 0:
                                unit_square = Square(1, i, j)
                                board.add_unit_square(unit_square)
                                board.squares.append(unit_square)
                current_value = board.value()  
            # If there are larger squares left:
            else:
                board.random_restart()  # Restart the board in a random configuration.
                squares_sorted_by_conflicts = sorted(board.squares, key=board.conflicts, reverse=True)
                for square in squares_sorted_by_conflicts:
                    board.add_square(square)
                current_value = board.value()  # Recalculate the board value.
                board.sideways_move_count = 0  # Reset sideways move count.

    result = [(square.size, square.x, square.y) for square in board.squares]

    return result
