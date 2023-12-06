from __future__ import annotations
from typing import List, Tuple

# ============================================================= STATE & PIECE FUNCTIONS ============================================================= #
class State:
    def __init__(self, pieces, player_turn, move_count=0):
        # Initialize board state.
        self.pieces = pieces            # List of Piece objects.
        self.player_turn = player_turn  # 'white' or 'black' player.
        self.move_count = move_count    # Total number of moves made.

    def is_game_over(self):
        # Check if any of the Kings have been captured.
        if not self.is_king_on_board('white') or not self.is_king_on_board('black'):
            return True
        # Check for draw conditions (there are only Kings left, or each player has made 50 moves).
        if self.is_only_kings_left() or self.move_count >= 100:
            return True
        return False

    def is_king_on_board(self, color):
        # Check if King of player color is on the board.
        for piece in self.pieces:
            if piece.type == 'King' and piece.color == color:
                return True
        return False

    def is_only_kings_left(self):
        # Check if there are only Kings left on the board (i.e. 'white' King and 'black' King).
        return len(self.pieces) == 2 and all(piece.type == 'King' for piece in self.pieces)
    
    def find_opponent_king_position(self, opponent_color):
        # Helper function to find opponent King's position for the
        # Manhattan Distance to Opponent King Heuristic.
        for piece in self.pieces:
            if piece.type == 'King' and piece.color == opponent_color:
                return piece.position
        return None

    def generate_possible_moves(self):
        # Generate all possible moves for each piece for the current player's turn.
        # Used by Opponent Agents for testing.
        moves = []

        for piece in self.pieces:
            if piece.color == self.player_turn:
                piece_moves = piece.get_possible_moves(self.pieces)  
                moves.extend([(piece.position, move) for move in piece_moves])
        return moves
        
    def generate_possible_moves_heuristic(self):
        # Generate all possible moves for each piece for the current player's turn.
        # Heuristic: Manhattan Distance to the opponent King's position.
        # Since our main goal is to quickly win by capturing the opponent's King, we will prioritize moves towards them.
        # Used by Student Agent.
        moves = []
        opponent_color = 'black' if self.player_turn == 'white' else 'white'
        king_position = self.find_opponent_king_position(opponent_color)

        for piece in self.pieces:
            if piece.color == self.player_turn:
                piece_moves = piece.get_possible_moves(self.pieces)
                moves.extend([(piece.position, move) for move in piece_moves])

        if king_position:
            # Sort by Manhattan Distance to the opposing King as heuristic.
            moves.sort(key=lambda x: self.manhattan_distance(x[1], king_position))
        return moves

    def manhattan_distance(self, pos1, pos2):
        # Calculate Manhattan Distance between two positions.
        return abs(pos1[0] - pos2[0]) + abs(pos1[1] - pos2[1])

    def make_move(self, move):
        # Make the desired move and return a new State with that move registered.

        # Get the starting and destination positions for the move.
        (from_pos, to_pos) = move

        # Create a new list of pieces but exclude the piece at destination and piece to be moved.
        new_pieces = [p for p in self.pieces if p.position != to_pos and p.position != from_pos]

        # Find the piece to be moved and update its position.
        moving_piece = next(p for p in self.pieces if p.position == from_pos)
        updated_moving_piece = Piece(moving_piece.type, moving_piece.color, to_pos)
        new_pieces.append(updated_moving_piece)

        # Switch player turn.
        next_player = 'black' if self.player_turn == 'white' else 'white'

        # Create a new state with the updated pieces and player turn
        return State(new_pieces, next_player, self.move_count + 1)

    def evaluate(self):
        # Evaluation function (referred to Prof's Proj3 consultation briefing).
        
        # High positive and negative values for win/loss scenarios.
        WIN_SCORE = 10000
        LOSS_SCORE = -10000

        # Check for win/loss conditions for 'white' winning and 'black' winning.
        if not self.is_king_on_board('black'):
            return WIN_SCORE
        if not self.is_king_on_board('white'):
            return LOSS_SCORE

        # Use weighted sum of the individual Piece values.
        piece_values = {'King': 0, 'Rook': 5, 'Bishop': 3, 'Knight': 3, 'Squire': 2, 'Combatant': 2}
        white_score, black_score = 0, 0

        for piece in self.pieces:
            value = piece_values.get(piece.type, 0)
            if piece.color == 'white':
                white_score += value
            else:
                black_score += value

        # Account for player's turn.
        score = white_score - black_score if self.player_turn == 'white' else black_score - white_score
        return score

    def is_capture(self, move):
        # Checks if the move is a capture move.
        # Used by Opponent Agents for testing.
        _, to_pos = move
        destination_piece = next((p for p in self.pieces if p.position == to_pos), None)
        return destination_piece is not None and destination_piece.color != self.player_turn

    def get_piece_at_position(self, position):
        # Gets the Piece at a given position.
        # Used by Opponent Agents for testing.
        return next((p for p in self.pieces if p.position == position), None)

class Piece:
    def __init__(self, type, color, position):
        # Initialize Piece object.
        self.type = type            # Type of Piece ('King'/'Rook'/'Bishop' etc.)
        self.color = color          # Color of Piece (which player owns it - 'white' or 'black')
        self.position = position    # Position on 8x8 board.

    def get_possible_moves(self, board):
        # Get the possible moves for the current Piece.
        if self.type == 'King':
            return self.get_king_moves(board)
        elif self.type == 'Rook':
            return self.get_rook_moves(board)
        elif self.type == 'Bishop':
            return self.get_bishop_moves(board)
        elif self.type == 'Knight':
            return self.get_knight_moves(board)
        elif self.type == 'Squire':
            return self.get_squire_moves(board)
        elif self.type == 'Combatant':
            return self.get_combatant_moves(board)
        else:
            return []

    def get_king_moves(self, pieces):
        # Get the possible moves for the King piece.
        moves = []
        directions = [(1, 0), (-1, 0), (0, 1), (0, -1), (1, 1), (1, -1), (-1, 1), (-1, -1)] # R, L, D, U, DiRD, DiRU, DiLD, DiLU.
        for d in directions:
            new_pos = (self.position[0] + d[0], self.position[1] + d[1])
            if is_valid_move(new_pos, pieces, self.color):
                moves.append(new_pos)
        return moves

    def get_rook_moves(self, pieces):
        # Get the possible moves for the Rook piece.
        moves = []
        directions = [(1, 0), (-1, 0), (0, 1), (0, -1)]  # R, L, D, U.

        for d in directions:
            for i in range(1, 8):
                new_pos = (self.position[0] + d[0] * i, self.position[1] + d[1] * i)
                if not is_valid_move(new_pos, pieces, self.color):
                    # Stop if move is not valid (includes if move results in capturing of player's own pieces).
                    break  

                if any(p.position == new_pos for p in pieces):
                    if any(p.position == new_pos and p.color != self.color for p in pieces):
                        # Include square if occupied by opponent's piece (capturing).
                        moves.append(new_pos)  
                    break  # Stop after capturing opponent's piece.
                moves.append(new_pos)
        return moves

    def get_bishop_moves(self, pieces):
        # Get the possible moves for the Bishop piece.
        moves = []
        directions = [(1, 1), (1, -1), (-1, 1), (-1, -1)]  # DiRD, DiRU, DiLD, DiLU.

        for d in directions:
            for i in range(1, 8):
                new_pos = (self.position[0] + d[0] * i, self.position[1] + d[1] * i)
                if not is_valid_move(new_pos, pieces, self.color):
                    # Stop if move is not valid (includes if move results in capturing of player's own pieces).
                    break

                if any(p.position == new_pos for p in pieces):
                    if any(p.position == new_pos and p.color != self.color for p in pieces):
                        # Include square if occupied by opponent's piece (capturing).
                        moves.append(new_pos)
                    break  # Stop after capturing opponent's piece.
                moves.append(new_pos)
        return moves

    def get_knight_moves(self, pieces):
        # Get the possible moves for the Knight piece.
        moves = []
        knight_moves = [(2, 1), (2, -1), (-2, 1), (-2, -1), (1, 2), (1, -2), (-1, 2), (-1, -2)]
        for km in knight_moves:
            new_pos = (self.position[0] + km[0], self.position[1] + km[1])
            if is_valid_move(new_pos, pieces, self.color):
                moves.append(new_pos)
        return moves

    def get_squire_moves(self, pieces):
        # Get the possible moves for the Squire piece.
        moves = []
        squire_moves = [(2, 0), (-2, 0), (0, 2), (0, -2), (1, 1), (1, -1), (-1, 1), (-1, -1)]
        for sm in squire_moves:
            new_pos = (self.position[0] + sm[0], self.position[1] + sm[1])
            if is_valid_move(new_pos, pieces, self.color):
                moves.append(new_pos)
        return moves

    def get_combatant_moves(self, pieces):
        # Get the possible moves for the Combatant piece.
        moves = []
        move_directions = [(1, 0), (-1, 0), (0, 1), (0, -1)]  # Orthogonal moves (R, L, D, U).
        capture_directions = [(1, 1), (1, -1), (-1, 1), (-1, -1)]  # Diagonal captures (DiRD, DiRU, DiLD, DiLU).
        for md in move_directions:
            # Can move Orthagonally only if the Combatant is not capturing.
            new_pos = (self.position[0] + md[0], self.position[1] + md[1])
            if is_valid_move_combatant(new_pos, pieces, self.color, False):
                moves.append(new_pos)
        for cd in capture_directions:
            # Can move diagonally only if the Combatant is going to capture.
            new_pos = (self.position[0] + cd[0], self.position[1] + cd[1])
            if is_valid_move_combatant(new_pos, pieces, self.color, True):
                moves.append(new_pos)
        return moves

def is_on_board(pos):
    # Checks if the position (to move to) is within the 8x8 board.
    return 0 <= pos[0] < 8 and 0 <= pos[1] < 8

def is_valid_move(pos, pieces, color):
    # Checks if the position (to move to) is a valid move.

    if not is_on_board(pos):
        return False
    
    # Get piece at destination if any.
    destination_piece = next((p for p in pieces if p.position == pos), None)

    if destination_piece is None:
        # If there is no piece at the destination, move to it.
        return True
    else:
        # If there is a piece at the destination, capture it only if it's an opponent's piece.
        return destination_piece.color != color
    
def is_valid_move_combatant(pos, pieces, color, is_capturing):
    # Checks if the position (to move to) is a valid move for the Combatant piece.

    if not is_on_board(pos):
        return False
    
   # Get piece at destination if any.    
    destination_piece = next((p for p in pieces if p.position == pos), None)

    if not is_capturing:
        # If the Combatant is not capturing, return True only if the destination is empty.
        return True if destination_piece is None else False
    else:
        # If the Combatant is capturing, return True only if the destination is an opponent's piece.
        return destination_piece.color != color if destination_piece is not None else False
        
# ============================================================= GAME/STUDENT FUNCTIONS ============================================================= #
def setUpBoard(gameboard):
    # Convert the gameboard input into a board with Piece instances.
    board = []
    for piece_type, color, position in gameboard:
        board.append(Piece(piece_type, color, position))
    return board

def studentAgent(gameboard : List[Tuple[str, str, Tuple[int, int]]], player='white') -> Tuple[Tuple[int, int], Tuple[int, int]]:
    # studentAgent function to run minimax with Alpha-Beta Pruning.
    # Initialize board and A-B pruning.
    list_of_pieces = setUpBoard(gameboard)
    state = State(list_of_pieces, player)
    best_move = None
    best_value = float('-inf')
    alpha = float('-inf')
    beta = float('inf')
    
    for move in state.generate_possible_moves_heuristic():
        new_state = state.make_move(move)
        move_value = ab(new_state, 4, alpha, beta, False) # False since this is 'black's move now.
        
        if move_value > best_value:
            best_value = move_value  
            best_move = move  
    return best_move

def ab(state, depth, alpha, beta, maximizing_player):
    # If reached the desired depth or game over, evaluate the state.
    if depth == 0 or state.is_game_over():
        return state.evaluate()

    # Maximizing ('white's turn).
    if maximizing_player:
        max_eval = float('-inf') 
        for move in state.generate_possible_moves_heuristic():
            new_state = state.make_move(move)
            eval = ab(new_state, depth - 1, alpha, beta, False)
            max_eval = max(max_eval, eval)
            alpha = max(alpha, eval)
            if beta <= alpha:
                break
        return max_eval  

    # Minimizing ('black's turn).
    else:
        min_eval = float('inf')  
        for move in state.generate_possible_moves_heuristic():
            new_state = state.make_move(move)
            eval = ab(new_state, depth - 1, alpha, beta, True)
            min_eval = min(min_eval, eval)
            beta = min(beta, eval)
            if beta <= alpha:
                break
        return min_eval  
