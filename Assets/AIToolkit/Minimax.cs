using System.Collections.Generic;
using UnityEngine;

public class Minimax<T>
{
    private GameRules<T> gameRules;

    public Minimax(GameRules<T> gm)
    {
        this.gameRules = gm;
    }

    public T PlayNextMove(T currentState, int Depth)
    {
        double alpha = double.MinValue, beta = double.MaxValue;
        MiniMaxItem<T> i = Max(currentState, -1, alpha, beta, Depth);
        Debug.Log("Playing with utility " + i.Utility + " for player yellow:\r\n" + i.Item);
        return i.Item;
    }

    public T PlayNextMove(T currentState, int player, int Depth)
    {
        double alpha = double.MinValue, beta = double.MaxValue;
        MiniMaxItem<T> i = Max(currentState, player, alpha, beta, Depth);
        Debug.Log("Playing with utility " + i.Utility + " for player "+(player == -1 ? "yellow" : "red")+":\r\n" + i.Item);
        return i.Item;
    }

    private MiniMaxItem<T> Max(T Entry, int player, double Alpha, double Beta, int depth)
    {
        if (depth == 0) return new MiniMaxItem<T>(Entry, gameRules.PredictWinner(Entry, player));

        T nextState = default(T);
        List<T> states = gameRules.generateNextStates(Entry, player);
        double maxUtility = double.MinValue;

        if (states.Count == 0)
            return new MiniMaxItem<T>(default(T), gameRules.CalculateUtility(Entry, player));
        else
            foreach (T item in states)
            {
                MiniMaxItem<T> cUtility = Min(item, player * -1, Alpha, Beta, depth - 1);
                double ItemUtility = cUtility.Utility;
                if (ItemUtility >= Beta)
                    return cUtility;

                if (ItemUtility > maxUtility)
                {
                    maxUtility = ItemUtility;
                    nextState = item;
                }
                if (ItemUtility > Alpha) Alpha = ItemUtility;

            }

        return new MiniMaxItem<T>(nextState, maxUtility);
    }

    private MiniMaxItem<T> Min(T Entry, int player, double Alpha, double Beta, int Depth)
    {
        if (Depth == 0) return new MiniMaxItem<T>(Entry, gameRules.PredictWinner(Entry, player));

        T nextState = default(T);
        List<T> entries = gameRules.generateNextStates(Entry, player);
        double minUtility = double.MaxValue;

        // We don't have any more states to cover, let's calculate the utility and go back:
        if (entries.Count == 0)
            return new MiniMaxItem<T>(default(T), gameRules.CalculateUtility(Entry, player));
        else
            // We got more states to cover:
            foreach (T item in entries)
            {
                MiniMaxItem<T> cUtility = Max(item, player * -1, Alpha, Beta, Depth - 1);
                double ItemUtility = cUtility.Utility;

                if (ItemUtility <= Alpha)
                    return cUtility;

                if (ItemUtility < minUtility)
                {
                    // We are considering returning this state:
                    nextState = item;
                    minUtility = ItemUtility;
                }
            }

        return new MiniMaxItem<T>(nextState, minUtility);
    }
}
