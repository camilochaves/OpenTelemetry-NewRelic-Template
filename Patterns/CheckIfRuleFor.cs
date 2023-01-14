namespace opentelemetry_newrelic_template.Patterns;
public class CheckIfRuleFor<T, K>
{
    private T _obj;
    private List<Type> _conditionsToEvaluate = new();

    public CheckIfRuleFor(T obj) => _obj = obj;

    public CheckIfRuleFor<T, K> AddCheckCondition(Type type)
    {
        if (!type.GetType().IsInstanceOfType(typeof(TestCondition<T, K>))) throw new Exception();
        _conditionsToEvaluate.Add(type);
        return this;
    }

    public IEnumerable<TestCondition<T, K>> Build()
    {
        foreach (var _conditionType in _conditionsToEvaluate)
        {
            if ((Activator.CreateInstance(_conditionType, _obj) as TestCondition<T, K>).Rule())
                yield return Activator.CreateInstance(_conditionType, _obj) as TestCondition<T, K>;
        }
    }
}
