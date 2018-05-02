using UnityEngine;
using System.Collections.Generic;

/**
 * Represents a state in the game.
 */
public class Board
{
    private int[][] board;
    private int[] lastPiece;

    public Board(Board from)
    {
        board = new int[from.board.Length][];
        lastPiece = new int[from.lastPiece.Length];

        for (int x = 0; x < from.board.Length; x++)
        {
            board[x] = new int[from.board[x].Length];
            for (int y = 0; y < from.board[0].Length; y++)
                board[x][y] = from.board[x][y];
        }

        for (int i = 0; i < from.LastPiece.Length; i++)
        {
            lastPiece[i] = from.LastPiece[i];
        }
    }

    public Board()
    {
        board = new int[6][];
        lastPiece = new int[7];
        for (int i = 0; i < board.Length; i++)
        {
            board[i] = new int[7];
            for (int i2 = 0; i2 < board[i].Length; i2++)
                board[i][i2] = 0;
            lastPiece[i] = 0;
        }
    }

    /**
     * Register the player 1 (red) movement on the given column.
     */
    public bool registerPlayer1Movement(int column)
    {
        if (lastPiece[column] >= 6) return false;
        board[lastPiece[column]++][column] = 1;
        return true;
    }
    /**
    * Register the player 2 (yellow) movement on the given column.
    */
    public bool registerPlayer2Movement(int column)
    {
        if (lastPiece[column] >= 6) return false;
        board[lastPiece[column]++][column] = -1;
        return true;
    }

    /**
     * Returns the board as a int matrix where 0 represents empty positions.
     */
    public int[][] Get
    {
        get { return board; }
    }

    public int[] LastPiece
    {
        get { return lastPiece; }
    }

    /**
     * Returns true if the board is currently full and no more moves can be made.
     */
    public bool isBoardCompleted()
    {
        foreach (int item in lastPiece)
            if (item != 7) return false;
        return true;
    }

    /**
     * Returns a text representation of the board.
     */
    public override string ToString()
    {
        string boardString = "";
        foreach (int[] row in board)
        {
            foreach (int column in row)
            {
                if (column == 1) boardString += " R ";
                else if (column == -1) boardString += " Y ";
                else boardString += " _ ";
            }
            boardString += "\r\n";
        }

        boardString += " LAST PIECES: " + string.Join("", new List<int>(lastPiece).ConvertAll(i => i.ToString()).ToArray());

        return boardString;
    }
}