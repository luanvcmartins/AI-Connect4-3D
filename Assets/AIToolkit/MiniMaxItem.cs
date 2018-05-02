public class MiniMaxItem<T>
{
    private T item;
    private double utility;

    public MiniMaxItem(T item, double utility)
    {
        this.item = item;
        this.utility = utility;
    }
    public T Item
    {
        get { return item; }
        set { item = value; }
    }


    public double Utility
    {
        get { return utility; }
        set { utility = value; }
    }
}