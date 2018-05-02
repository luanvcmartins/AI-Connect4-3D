# Connect 4
Simple Connect 4 implementation on the Unity3D Engine. It uses MINIMAX with Alpha-Beta Pruning to play against the player with a depth limitation. 

This project was submitted as a project for an AI class (01-2018). The graphical interface was not required, but the task provided me with a good context to learn more about Unity3D development.

## MINIMAX - Implementing the computer player
The algorithm to play the computer is in the ``Assets\AIToolkit`` folder. It is a recursive implementation of the MINIMAX algorithm with Alpha-Beta Pruning in ``C#``. Because the search space is too large, a depth limit was implemented as well. This implementation is capable of going up to 8 levels of the search tree in a reasonable time for the user to wait. 

The search is composed of the following files, they are a generic implementation of the algorithm and can be easily used to play any other game by extending the ``GameRules`` class to generate and evaluate the rules of the game:
 - ``AIToolkit\GameRules.cs``: abstract class that represents and informs the pattern to be provided to the ``MiniMax`` algorithm. The user may extend this class and implement the required methods to use the search algorithm. There are four main functions to this class:
    - ``GenerateNextStates()``: returns a list of new states of the game that can be accessed from the current one.
    - ``CalculateUtility()``: returns a numeric value that represents how good the current state is for the given player. This function will be called once the MINIMAX Search reach the leaves of the tree, if it does that.
    - ``PredictWinner()``: returns a numeric value that represents how good of a solution the current state is. It will be called when the MINIMAX reach the depth it is allowed to go but didn't reach the end of the tree. This function may just call ``CalculateUtility()`` or you could provide a more complex prediction function.
    - ``IsGameOver()``: returns true if the game has ended. You may pass a parameter by reference in which the winner of the game will be informed.
 - ``AIToolkit\MiniMax.cs``: the implementation of the Minimax algorithm. The user may instantiate this class by passing his own implementation of the ``GameRules`` class on the constructor. To execute the algorithm, call the ``PlayNextMove`` function passing the current state of the game and the search depth and it will return the state with the next move.
 - ``AIToolkit\MiniMaxItem.cs``: holds the current state object (which is a generic type) and its' heuristic. It's used by the minimax algorithm to synchronize a state and its' heuristic.

The following files define the rules of the Connect 4 game and its' states:
 - ``GameAssets\Scripts\AI\Board.cs``: represents a state in the game.
 - ``GameAssets\Scripts\AI\Connect4GameRules.cs``: GameRules extended class that is able to generate and evaluate new states of the game. 

## Heuristic function
When the search tree is too large to be fulled verified as in this case, the importance of a good heuristic function to numeric represent how good the current state is for the given player grows. The heuristic used in this case is very simple:
 - For each piece in the board, we verify its' neighbor in every direction (vertically, horizontally and in both diagonals), counting how many pieces of the same type we have in sequence. 
 - If a piece of the enemy is found, we reset the count for the current direction. 
 - If, for any given direction, we got a trend of more than two pieces, we multiply the current points by a (almost arbitrary) value of 1000 for 4 pieces (a win) and 100 for 3 pieces in sequences (and nothing blocking it). 

The same process is done for pieces of the enemy at the same time, except we subtract them from the current utility. A positive utility for a given state means we have more and better chances of a 4 piece sequence.

## Game Assets
The background music is the guitar only version of [Kindle the Soul](https://www.youtube.com/watch?v=EU06k6pYzyw) by [Alex Roe](https://www.youtube.com/user/RoeTaKa). The other assets have been found randomly on the internet and may be changed or removed if needed.