namespace opentelemetry_newrelic_template.Patterns;
public abstract class TestCondition<T, K>
{
    protected T _input;

    protected TestCondition(T input) => _input = input;

    public abstract bool Rule();

    public abstract K? Handle(params object[] args);
}
