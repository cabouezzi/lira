namespace Lira;

public interface ILiraCallable
{
    int Arity { get; }
    object? Call(Interpreter interpreter, List<object?> arguments);

    public class LiraFunction : ILiraCallable
    {
        public int Arity => _declaration.Parameters.Count;
        private IStatement.Function _declaration { get; }

        public LiraFunction(IStatement.Function declaration)
        {
            this._declaration = declaration;
        }

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            Environment env = new(interpreter.Globals);
            for (int i = 0; i < _declaration.Parameters.Count; i++)
            {
                env.Define(_declaration.Parameters[i].Lexeme, arguments[i]);
            }

            interpreter.ExecuteBlock(_declaration.Body, env);
            return null;
        }
    }
}