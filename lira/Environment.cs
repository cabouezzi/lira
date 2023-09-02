namespace Lira;

public class Environment
{
    private Dictionary<String, object?> _container = new();

    public Environment? Enclosing { get; }

    public Environment(Environment? enclosing = null)
    {
        this.Enclosing = enclosing;
    }

    public void Define(String id, object? value) => _container[id] = value;
    public object? Get(Token id)
    {
        if (_container.TryGetValue(id.Lexeme, out object? value)) return value;

        if (Enclosing is not null) return Enclosing.Get(id);

        throw new RuntimeError(id, $"Undefined variable `{id.Lexeme}`.");
    }

    public void Assign(Token id, object? newValue)
    {
        if (_container.TryGetValue(id.Lexeme, out object? value))
        {
            _container[id.Lexeme] = newValue;
            return;
        }
        else if (Enclosing is not null)
        {
            Enclosing.Assign(id, newValue);
            return;
        }

        throw new RuntimeError(id, $"Undefined variable `{id.Lexeme}`.");
    }
}