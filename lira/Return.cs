namespace Lira;

public class Return : Exception
{
    public object? Value { get; }

    public Return(object? Value) : base($"Unhandled return statement with value '{Value}'")
    {
        this.Value = Value;
    }
}