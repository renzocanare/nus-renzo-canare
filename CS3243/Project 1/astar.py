from typing import Dict, List, Tuple
from heapq import heappop, heappush
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

# Invert the maze and return a list of positions with inverted creep dmg. 
def inversion(creeps_list: List[Tuple[int, int, int]], obstacles_set: set, rows: int, columns: int) -> List[Tuple[int, int, int]]:
    creeps_set = set(creeps_list)
    max_creeps = max(num for x, y, num in creeps_set if (x, y) not in obstacles_set)
    
    # Generate a set of all cells in the maze.
    all_cells = {(x, y) for x in range(rows) for y in range(columns)}
    
    # Cells with no creeps.
    no_creeps_cells = all_cells - {(x, y) for x, y, _ in creeps_set} - obstacles_set
    
    # Create the inverted set for cells with no creeps.
    no_creeps_inverted = {(x, y, max_creeps) for x, y in no_creeps_cells}
    
    # Invert the creeps based on their original order.
    inverted_creeps_list = [(x, y, max_creeps - num) if (x, y, num) in creeps_set else (x, y, num) for x, y, num in creeps_list]
    
    # Add creeps from no_creeps_inverted.
    inverted_creeps_list += list(no_creeps_inverted)

    return inverted_creeps_list
 
# Precompute Manhattan distances into a dictionary.
def manhattan_distances(rows: int, columns: int, goals_set: set) -> Dict[Tuple[int, int], int]:
    distances = {}
    for x in range(rows):
        for y in range(columns):
            pos = (x, y)
            distances[pos] = min([abs(pos[0] - goal[0]) + abs(pos[1] - goal[1]) for goal in goals_set])
    return distances
 
# Manhattan Distance adjusted for creep damage.
def heuristic(pos: Tuple[int, int], min_creep_dmg: int, precomputed_distances: Dict[Tuple[int, int], int]) -> int:
    manhattan_dist =  precomputed_distances[pos] 
    heuristic_cost = manhattan_dist * (1 + min_creep_dmg)
    return heuristic_cost
 
# Calculate move cost for normal or flash moves.
def get_move_cost(curr_pos: Tuple[int, int], direction: Tuple[int, int], obstacles_set: List[Tuple[int, int]], creep_damage_map: Dict[Tuple[int, int], int], rows: int, columns: int, flash = False) -> Tuple[Tuple[int, int], int]:
    x, y = curr_pos
    dx, dy = direction
    hp_lost = 0
    flash_steps_moved = 0
 
    if flash:
        while 0 <= x + dx < rows and 0 <= y + dy < columns and (x + dx, y + dy) not in obstacles_set:
            x += dx
            y += dy
            flash_steps_moved += 1
        hp_lost = (2 * flash_steps_moved) + creep_damage_map.get((x, y), 0)  + 10
    else:
        if 0 <= x + dx < rows and 0 <= y + dy < columns and (x + dx, y + dy) not in obstacles_set:
            x += dx
            y += dy
            hp_lost = 4 + creep_damage_map.get((x, y), 0)
 
    return (x, y), hp_lost

# Run A* Search algorithm.
def run_a_star(columns: int, rows: int, obstacles_set: set, creeps_list: List[Tuple[int, int, int]], start_pos: Tuple[int, int], goals_set: set, max_flash: int) -> List[int]:
    
    # Create a pq for A* Search.
    pq = ([(0, start_pos, max_flash, False, 0)]) # cost, position, number of flashes left, is_inverted, total path cost.
    start_state = (start_pos, max_flash, False)
    
    # Check if start already is goal.
    if start_pos in goals_set:
        return []
 
    # Initialize visited_state set to track visited states.
    visited_state = set()
    
    # Pre-compute inversion set.
    inverted_creeps_list = inversion(creeps_list, obstacles_set, rows, columns)

    # Pre-compute creep locations.
    creep_damage_map = {(x, y): dmg for x, y, dmg in creeps_list}
    inverted_creep_damage_map = {(x, y): dmg for x, y, dmg in inverted_creeps_list}

    # Pre-compute heuristic components.
    min_creep_dmg = min(num for (_, _, num) in creeps_list)
    min_inverted_creep_dmg = min(num for (_, _, num) in inverted_creeps_list)

    # Pre-compute Manhattan distances.
    precomputed_distances = manhattan_distances(rows, columns, goals_set)
 
    # Dictionary to store parent nodes for backtracking.
    came_from = {start_state: None}
 
    while pq: 
        _, cur_pos, flashes, is_inverted, path_cost = heappop(pq) 
        
        # If visited state has already been explored, skip it.
        curr_state = (cur_pos, flashes, is_inverted)
        if curr_state in visited_state:
            continue
        
        # Add to visited state.
        visited_state.add(curr_state)
 
        # Goal test by back-forming path.
        if cur_pos in goals_set:
            path_new = []
            current = curr_state
            while current != start_state:
                action = came_from[current][1]
                if isinstance(action, tuple):
                    path_new.append(action[1])
                    path_new.append(action[0])
                else:
                    path_new.append(action)
                current = came_from[current][0]
            return path_new[::-1]  
         
        # Get pos and cost for normal moves + flash.
        for action_val, direction in enumerate(DIRECTIONS):
            
            #=== Normal moves: ===#
            if is_inverted:
                new_pos, move_cost = get_move_cost(cur_pos, direction, obstacles_set, inverted_creep_damage_map, rows, columns, flash = False)
                move_heuristic_cost = heuristic(new_pos, min_inverted_creep_dmg, precomputed_distances)
            else:
                new_pos, move_cost = get_move_cost(cur_pos, direction, obstacles_set, creep_damage_map, rows, columns, flash = False)
                move_heuristic_cost = heuristic(new_pos, min_creep_dmg, precomputed_distances)
            new_path_cost = path_cost + move_cost
            f = new_path_cost + move_heuristic_cost   
            if (new_pos, flashes, is_inverted) not in visited_state:
                new_state = (f, new_pos, flashes, is_inverted, new_path_cost)
                heappush(pq, new_state)
                if (new_pos, flashes, is_inverted) not in came_from or new_path_cost < came_from[(new_pos, flashes, is_inverted)][2]:
                    # Only replace duplicates in parent directory if the cost is lower.
                    came_from[(new_pos, flashes, is_inverted)] = (curr_state, (action_val), new_path_cost)
 
            #=== Flash moves: ===#
            if flashes > 0:
                if is_inverted:
                    flash_pos, flash_cost = get_move_cost(cur_pos, direction, obstacles_set, inverted_creep_damage_map, rows, columns, flash = True)
                    flash_heuristic_cost = heuristic(flash_pos, min_inverted_creep_dmg, precomputed_distances)
                else:
                    flash_pos, flash_cost = get_move_cost(cur_pos, direction, obstacles_set, creep_damage_map, rows, columns, flash = True)
                    flash_heuristic_cost = heuristic(flash_pos, min_creep_dmg, precomputed_distances)
                new_path_cost = path_cost + flash_cost
                f = new_path_cost + flash_heuristic_cost
                if (flash_pos, flashes - 1, is_inverted) not in visited_state:
                    new_flash_state = (f, flash_pos, flashes - 1, is_inverted, new_path_cost)
                    heappush(pq, new_flash_state)
                    if (flash_pos, flashes - 1, is_inverted) not in came_from or new_path_cost < came_from[(flash_pos, flashes - 1, is_inverted)][2]:
                        # Only replace duplicates in parent directory if the cost is lower.
                        came_from[(flash_pos, flashes - 1, is_inverted)] = (curr_state, (Action.FLASH.value, action_val), new_path_cost)
        
        #=== Inversion: ===#
        if not is_inverted and (cur_pos, flashes, True) not in visited_state:
            f = path_cost + heuristic(cur_pos, min_inverted_creep_dmg, precomputed_distances)
            heappush(pq, (f, cur_pos, flashes, True, path_cost))
            if (cur_pos, flashes, True) not in came_from or path_cost < came_from[(cur_pos, flashes, True)][2]:
                # Only replace duplicates in parent directory if the cost is lower.
                came_from[(cur_pos, flashes, True)] = (curr_state, (Action.INVERSION.value), path_cost) 
    return []
 
def search(dct: Dict) -> List[int]:
    """
    Solve the maze using A* search.
 
    Write your implementation below
    """
    # Get columns and rows.
    columns = dct["cols"]
    rows = dct["rows"]
 
    # Set obstacles.
    obstacles_set = set(tuple(obs) for obs in dct["obstacles"])
 
    # Get creeps into dictionary to remove duplicates at same position (in order),
    # then convert to list.
    creeps_dict = {(x, y): num for (x, y, num) in dct["creeps"]}
    creeps_list = [(x, y, num) for (x,y), num in creeps_dict.items()]
    
    # Set starting position.
    start_pos = tuple(dct["start"])
 
    # Set goals.
    goals_set = set(tuple(goal) for goal in dct["goals"])
 
    # Get max flash num.
    max_flash = dct["num_flash_left"]
 
    return run_a_star(columns, rows, obstacles_set, creeps_list, start_pos, goals_set, max_flash)
