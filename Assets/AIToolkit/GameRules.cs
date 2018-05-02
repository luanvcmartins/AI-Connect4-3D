using System.Collections.Generic;

public abstract class GameRules<T>
{
    public abstract List<T> generateNextStates(T currentState, int player);

    public abstract double CalculateUtility(T entry, int player);

    public abstract double PredictWinner(T entry, int player);

    public abstract bool IsGameOver(T entry, ref int winner);
}