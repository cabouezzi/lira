namespace Lira;

public interface ILiraCallable
{
    int Arity { get; }
    object? Call(Interpreter interpreter, List<object?> arguments);

    public class LiraFunction : ILiraCallable
    {
        public int Arity => _declaration.Parameters.Count;
        private IStatement.Function _declaration { get; }
        private Environment _closure { get; }

        public LiraFunction(IStatement.Function declaration, Environment closure)
        {
            this._declaration = declaration;
            this._closure = closure;
        }

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            Environment env = new(_closure);
            for (int i = 0; i < _declaration.Parameters.Count; i++)
            {
                env.Define(_declaration.Parameters[i].Lexeme, arguments[i]);
            }

            try
            {
                interpreter.ExecuteBlock(_declaration.Body, env);
            }
            catch (Return returned)
            {
                return returned.Value;
            }
            return null;
        }
    }
}