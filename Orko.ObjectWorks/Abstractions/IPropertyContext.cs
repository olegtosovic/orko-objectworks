namespace Orko.ObjectWorks.Abstractions;

public interface IPropertyContext<TObject> where TObject : class
{
    object? GetValue(TObject instance);
    object? GetValueFast(TObject instance);
    void SetValue(TObject instance, object? value);
    void SetValueFast(TObject instance, object? value);
    IEnumerable<TAttribute> GetAttributes<TAttribute>() where TAttribute : Attribute;
    string Name { get; }
}