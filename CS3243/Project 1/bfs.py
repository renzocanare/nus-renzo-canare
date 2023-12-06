from typing import Dict, List, Tuple
from collections import deque
from enum import Enum

DIRECTIONS = [
    (-1, 0), # UP
    (1, 0),  # DOWN
    (0, -1), # LEFT
    (0, 1)  # RIGHT
]

class Action(Enum):
    UP = 0
    DOWN = 1
    LEFT = 2
    RIGHT = 3
    FLASH = 4
    INVERSION = 5

def search(dct: Dict) -> List[int]:
    """
    Solve the maze using breadth-first search.

    Write your implementation below
    """
    # Creeps and flash can be ignored since no cost involved in BFS.
    # Can traverse tree without considering creep cost, just need to find
    # a path.

    # Get columns and rows.
    columns = dct["cols"]
    rows = dct["rows"]
 
    # Set obstacles.
    obstacles_set = set(tuple(obs) for obs in dct["obstacles"])

    # Creeps can be ignored.
    
    # Set starting position.
    start_pos = tuple(dct["start"])
 
    # Set goals.
    goals_set = set(tuple(goal) for goal in dct["goals"])

    # Flashes can be ignored.

    def run_bfs() -> List[int]:
        
        # Create a queue for BFS.
        queue = deque([(start_pos, [])]) # position, path taken

        # Check if start already is goal.
        if start_pos in goals_set:
            return []

        # Initialize visited set to track visited positions.
        visited_pos = set()

        # Mark the starting position as visited.
        visited_pos.add(start_pos)

        while queue:
            cur_pos, path = queue.popleft()

            for action_val, direction in enumerate(DIRECTIONS):
                # Get directional changes.
                row_change, col_change = direction
                
                # Get current x and y positions.
                cur_row, cur_col = cur_pos

                # Calculate new position.
                new_row, new_col = cur_row + row_change, cur_col + col_change
                new_pos = (new_row, new_col)

                # Check for obstacles and boundaries.
                if (0 <= new_row < rows and 0 <= new_col < columns 
                    and new_pos not in obstacles_set
                    and new_pos not in visited_pos):
                        new_path = path + [action_val]  

                        # Goal test when child node generated.
                        if new_pos in goals_set:
                             return new_path

                        queue.append((new_pos, new_path))
                        visited_pos.add(new_pos)
        return []
    
    return run_bfs()
