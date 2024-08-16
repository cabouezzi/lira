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

    public object? GetAt(int distance, String id) => Ancestor(distance)._container.TryGetValue(id, out object? value) ? value : null;

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

    public void AssignAt(int distance, Token identifier, object? value)
    {
        Ancestor(distance)._container.Add(identifier.Lexeme, value);
    }

    private Environment? Ancestor(int distance)
    {
        Environment environment = this;
        for (int i = 0; i < distance; i++)
        {
            environment = environment.Enclosing;
        }

        return environment;
    }
}