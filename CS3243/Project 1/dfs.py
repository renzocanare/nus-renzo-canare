from typing import Dict, List, Tuple
from collections import deque

# Helper function to build the path from the goal to the start.
# Helps to prevent updating the full path at every step, increasing efficiency for big mazes.
def build_path(end_pos, parents):
    path = []
    while end_pos is not None:
        path.append(end_pos)
        end_pos = parents[end_pos]
    return path[::-1]  # Reverse it to get path from start to end.

def search(dct: Dict) -> List[Tuple[int, int]]:
    """
    Solve the maze using depth-first search.

    Write your implementation below
    """

    ### 1. Define the maze structure: ###
    columns = dct["cols"]
    rows = dct["rows"]

    # Set obstacles.
    obstacles_set = set(tuple(obs) for obs in dct["obstacles"])

    # Set starting position.
    start_pos = tuple(dct["start"])

    # Set goals.
    goals_set = set(tuple(goal) for goal in dct["goals"])
    
    def run_dfs(start_pos: Tuple[int, int]) -> List[Tuple[int, int]]:
        
        ### 3. Create a stack: ###
        stack = deque([start_pos])
        
        # Check if start is already the goal.
        if start_pos in goals_set:
            return [start_pos]
        
        # Initialize visited set to track visited positions.
        visited_pos = set()

        ### 4. Mark the starting position as visited: ###
        visited_pos.add(start_pos)

        # Create a parents dictionary to track the path.
        parents = {start_pos: None}
        
        # Possible next steps that can be taken (right, left, down, up).
        directions = [(0, 1), (0, -1), (1, 0), (-1, 0)]

        while stack:
            (cur_row, cur_col) = stack[-1]  # Peek into stack.
    
            found_next = False
            
            ### 5. Explore adjacent cells: ###
            for row_change, column_change in directions:
                new_row, new_col = cur_row + row_change, cur_col + column_change
                new_pos = (new_row, new_col)

                ### 6. Check for obstacles and boundaries: ###
                if (0 <= new_row < rows and 0 <= new_col < columns 
                    and new_pos not in obstacles_set
                    and new_pos not in visited_pos):
                        visited_pos.add(new_pos)

                        ### 7. Track the path: ###   
                        # Update parent of the new position.
                        parents[new_pos] = (cur_row, cur_col)

                        ### 10. Return the path: Early goal test. ###
                        if new_pos in goals_set:
                            return build_path(new_pos, parents)
                        
                        ### 9. Backtrack if necessary: ###
                        # Next step found, add to stack and set flag to true to continue
                        # traversing recursively by not popping (backtracking).
                        stack.append(new_pos)
                        found_next = True
                        break
                
            if not found_next:
                stack.pop()

        return []
    
    return run_dfs(start_pos)
