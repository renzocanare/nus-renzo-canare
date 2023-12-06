from typing import Dict, List, Tuple
import heapq
 
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
    Solve the maze using uniform-cost search.
 
    Write your implementation below
    """
 
    # Get columns and rows.
    columns = dct["cols"]
    rows = dct["rows"]
 
    # Set obstacles.
    obstacles_set = set(tuple(obs) for obs in dct["obstacles"])
    
    # Set starting position.
    start_pos = tuple(dct["start"])
 
    # Set goals.
    goals_set = set(tuple(goal) for goal in dct["goals"])
 
    def run_ucs(start_pos: Tuple[int, int]) -> List[Tuple[int, int]]:
        # Create a priority queue with its (cost, current position).
        pq = [(0, start_pos)]
        
        # Check if start is already the goal.
        if start_pos in goals_set:
            return [start_pos]

        # Initialize visited set to track visited positions.
        visited_pos = set()

        # Create a parents dictionary to track the path.
        parents = {start_pos: None}

        # Possible next steps that can be taken (right, left, down, up).
        directions = [(0, 1), (0, -1), (1, 0), (-1, 0)]
        
        while pq:
            # Using heapq, pops the smallest cost first.
            # If all costs are the same, will pop as a normal queue.
            cost, (cur_row, cur_col) = heapq.heappop(pq)
 
            # Continue if position has already been visited.
            if (cur_row, cur_col) in visited_pos:
                continue
            
            # Goal test when node is popped from pq.
            if (cur_row, cur_col) in goals_set:
                return build_path((cur_row, cur_col), parents)
            
            visited_pos.add((cur_row, cur_col))
            
            for row_change, column_change in directions:
                new_row, new_col = cur_row + row_change, cur_col + column_change
                new_pos = (new_row, new_col)

                if (0 <= new_row < rows and 0 <= new_col < columns 
                    and new_pos not in obstacles_set
                    and new_pos not in visited_pos):
                        
                        # Add 1 to current cost.
                        new_cost = cost + 1

                        # Update parent of the new position.
                        parents[new_pos] = (cur_row, cur_col)

                        heapq.heappush(pq, (new_cost, new_pos))
 
        return []
    
    return run_ucs(start_pos)
 