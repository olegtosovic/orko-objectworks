namespace Orko.ObjectWorks.Abstractions;

public interface IObjectContext<TObject> where TObject : class
{
    bool TryGetProperty(string propertyName, out IPropertyContext<TObject>? propertyContext);

    IPropertyContext<TObject> GetProperty(string propertyName);

    IReadOnlyCollection<IPropertyContext<TObject>> GetProperties();
}
