using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Connect4GameRules : GameRules<Board>
{

    #region MAIN SEARCH FUNCTIONS (final state, utility, nextstates)
    /**
     * Generates the next states that can be accessed by the current state. 
     */
    public override List<Board> generateNextStates(Board currentState, int player)
    {
        List<Board> boards = new List<Board>();

        if (!currentState.isBoardCompleted())
            for (int column = 0; column < 7; column++)
            {
                Board nBoard = new Board(currentState);
                if (player == -1)
                {
                    bool con = !nBoard.registerPlayer2Movement(column);
                    if (con) { continue; }
                }
                else
                {
                    bool con = !nBoard.registerPlayer1Movement(column);
                    if (con) { continue; }
                }
                boards.Add(nBoard);
            }

        return boards;
    }

    /**
     * Evaluates if the game is over and returns the winner in the "winner" variable that must
      * be passed as a reference. 
     */
    public override bool IsGameOver(Board entry, ref int winner)
    {
        if (entry.isBoardCompleted())
        {
            // If the board is completed, the game has ended with a tie:
            winner = 0;
            return true;
        }
        for (int x = 0; x < entry.Get.Length; x++)
            for (int y = 0; y < entry.Get[x].Length; y++)
            {
                // For each position we will evaluate the next 4 pieces in the board to determinate 
                // if there is a win.
                int player = entry.Get[x][y];
                if (player != 0)
                {
                    winner = player;
                    bool finished = true;
                    for (int pX = x; pX < x + 4; pX++)
                    {
                        if (inBounds(pX, y, entry.Get)) { if (entry.Get[pX][y] != player) { finished = false; break; } }
                        else { finished = false; break; }
                    }
                    if (finished) return true;
                    finished = true;
                    for (int pY = y; pY < y + 4; pY++)
                    {
                        if (inBounds(x, pY, entry.Get)) { if (entry.Get[x][pY] != player) { finished = false; break; } }
                        else { finished = false; break; }
                    }
                    if (finished) return true;
                    finished = true;

                    for (int p = 0; p < 4; p++)
                    {
                        if (inBounds(x + p, y + p, entry.Get)) { if (entry.Get[x + p][y + p] != player) { finished = false; break; } }
                        else { finished = false; break; }
                    }
                    if (finished) return true;
                    finished = true;

                    for (int p = 0; p < 4; p++)
                    {
                        if (inBounds(x + p, y - p, entry.Get)) { if (entry.Get[x + p][y - p] != player) { finished = false; break; } }
                        else { finished = false; break; }
                    }
                    if (finished) return true;
                    finished = true;
                }
            }

        winner = 0;
        return false;
    }

    public override double PredictWinner(Board entry, int player)
    {
        return CalculateUtility(entry, player);
    }

    /**
     * Evaluates how good the current board is for a given player.
     */
    public override double CalculateUtility(Board entry, int player)
    {
        double utility = 0;
        for (int x = 0; x < entry.Get.Length; x++)
            for (int y = 0; y < entry.Get[x].Length; y++)
            {
                if (entry.Get[x][y] == player * -1)
                    utility -= getMax(x, y, entry.Get, player * -1);
                else if (entry.Get[x][y] == player)
                    utility += getMax(x, y, entry.Get, player);
            }
        return utility;
    }

    #endregion

    #region HELPER FUNCTIONS
    private double getMax(int x, int y, int[][] board, int player)
    {
        int[] max = new int[4], trend = new int[4];
        for (int padding = 0; padding < 4; padding++)
        {
            if (inBounds(x, y + padding, board))
            {
                if (board[x][y + padding] == player) trend[0]++;
                else if (board[x][y + padding] == player * -1) { trend[0] = 0; }
            }
            else trend[0] = 0;

            if (inBounds(x + padding, y, board))
            {
                if (board[x + padding][y] == player) trend[1]++;
                else if (board[x + padding][y] == player * -1) { trend[1] = 0; }
            }
            else { trend[1] = 0; max[1] = 0; }

            if (inBounds(x + padding, y + padding, board))
            {
                if (board[x + padding][y + padding] == player) trend[2]++;
                else if (board[x + padding][y + padding] == player * -1) { trend[2] = 0; }
            }
            else { trend[2] = 0; max[2] = 0; }

            if (inBounds(x + padding, y - padding, board))
            {
                if (board[x + padding][y - padding] == player) trend[3]++;
                else if (board[x + padding][y - padding] == player * -1) { trend[3] = 0; }
            }
            else { trend[3] = 0; max[3] = 0; }

            for (int i = 0; i < 4; i++)
                if (max[i] < trend[i]) max[i] = trend[i];
        }
        int maximum = max.Max();

        switch (maximum)
        {
            case 4: maximum *= 1000; break;
            case 3: maximum *= 100; break;
        }

        return trend.Max() * maximum;
    }
    /**
     * Returns true if the given point is within the board's bounds.
     */
    private bool isTrendingInBound(int x, int y, int[][] board, int player)
    {
        if (!inBounds(x, y, board)) return false;
        else return board[x][y] == player;
    }

    private bool inBounds(int x, int y, int[][] board)
    {
        return x >= 0 && x < board.Length && y >= 0 && y < board[x].Length;
    }
    #endregion
}