using Orko.ObjectWorks.Abstractions;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Orko.ObjectWorks.Core;

/// <summary>
/// Represents cached net property.
/// </summary>
public class PropertyContext<TObject> : IPropertyContext<TObject> where TObject : class
{
    #region Fields
    private readonly PropertyInfo _propertyInfo;
    private readonly HashSet<Attribute> _attributesSet = new();
    private ReadOnlyCollection<Attribute> _attributeCollection = null!;
    private Func<TObject, object?> _propertyGetter = null!;
    private Action<TObject, object?> _propertySetter = null!;
    #endregion

    #region Constructors
    /// <summary>
    /// Not available outside library.
    /// </summary>
    internal PropertyContext(PropertyInfo propertyInfo)
    {
        // Save arguments.
        _propertyInfo = propertyInfo;

        // Cache everything.
        CachePropertyInfo();
        CachePropertyAttributes();
        CacheGet();
        CacheSet();
    }
    #endregion

    #region Properties
    /// <summary>
    /// Property name.
    /// </summary>
    public string Name { get; private set; } = null!;
    /// <summary>
    /// Property declaring type.
    /// </summary>
    public Type? DeclaringType { get; private set; }
    /// <summary>
    /// Property type.
    /// </summary>
    public Type PropertyType { get; private set; } = null!;
    #endregion

    #region Public methods
    /// <summary>
    /// Gets property value.
    /// </summary>
    public object? GetValue(TObject instance)
    {
        return _propertyInfo!.GetValue(instance);
    }
    /// <summary>
    /// Sets property value
    /// </summary>
    public void SetValue(TObject instance, object? value)
    {
        _propertyInfo.SetValue(instance, new object[] { value! });
    }
    /// <summary>
    /// Gets property value.
    /// </summary>
    public object? GetValueFast(TObject instance)
    {
        return _propertyGetter(instance);
    }
    /// <summary>
    /// Sets property value
    /// </summary>
    public void SetValueFast(TObject instance, object? value)
    {
        _propertySetter(instance, value);
    }
    /// <summary>
    /// Gets object attributes.
    /// </summary>
    public IEnumerable<TAttribute> GetAttributes<TAttribute>() where TAttribute : Attribute
    {
        return _attributeCollection
            .Where(attr => attr is TAttribute)
            .Cast<TAttribute>();
    }
    #endregion

    #region Cache general
    /// <summary>
    /// Caches property information.
    /// </summary>
    private void CachePropertyInfo()
    {
        Name = _propertyInfo.Name;
        DeclaringType = _propertyInfo.DeclaringType;
        PropertyType = _propertyInfo.PropertyType;
    }
    /// <summary>
    /// Caches property attributes.
    /// </summary>
    private void CachePropertyAttributes()
    {
        // Cache attributes to hash set.
        _propertyInfo!
            .GetCustomAttributes()
            .ToList()
            .ForEach(attribute => { _attributesSet.Add(attribute); });

        // Cache attribute to collection.
        _attributeCollection = new ReadOnlyCollection<Attribute>(
           _attributesSet
               .Select(x => x)
               .ToList());
    }
    #endregion

    #region Cache Accessors
    /// <summary>
    /// Caches and compiles getter method for fast property operation.
    /// </summary>
    private void CacheGet()
    {
        // Get GetMethod method info via reflection.
        var _propertyGetMethod = _propertyInfo!.GetGetMethod()!;

        // Create parameter expression.
        ParameterExpression instanceExpression = Expression.Parameter(typeof(TObject));

        // Unary expression for instance conversion.
        UnaryExpression instanceCast = _propertyGetMethod!.DeclaringType!.IsValueType ?
            Expression.Convert(instanceExpression, _propertyGetMethod!.DeclaringType!) :
            Expression.TypeAs(instanceExpression, _propertyGetMethod!.DeclaringType!);

        // Create method call expression.
        MethodCallExpression methodCallExpression = Expression.Call(instanceCast, _propertyGetMethod);

        // Create mehod class with cast expression.
        var methodCastExpression = Expression.TypeAs(methodCallExpression, typeof(object));

        // Compile GetMethod method call to get delegate.
        _propertyGetter = Expression
            .Lambda<Func<TObject, object?>>(methodCastExpression, instanceExpression)
            .Compile();
    }
    /// <summary>
    /// Caches and compiles setter method for fast property operation.
    /// </summary>
    private void CacheSet()
    {
        // Get SetMethod method info via reflection.
        var _propertySetMethod = _propertyInfo!.GetSetMethod(true)!;

        // Create parameter expression for instance.
        ParameterExpression instanceExpression = Expression.Parameter(_propertyInfo!.DeclaringType!);

        // Create parameter expression for value.
        ParameterExpression valueExpression = Expression.Parameter(typeof(object));

        // Unary expression for instance conversion.
        UnaryExpression instanceCast = _propertyInfo!.DeclaringType!.IsValueType ?
            Expression.Convert(instanceExpression, _propertyInfo!.DeclaringType!) :
            Expression.TypeAs(instanceExpression, _propertyInfo!.DeclaringType!);

        // Unary expression for value conversion.
        UnaryExpression valueCast = _propertyInfo.PropertyType.IsValueType ?
            Expression.Convert(valueExpression, _propertyInfo.PropertyType) :
            Expression.TypeAs(valueExpression, _propertyInfo.PropertyType);

        // Create method call expression.
        MethodCallExpression methodCallExpression = Expression.Call(instanceCast, _propertySetMethod!, valueCast);

        // Create arguments for method.
        var parameters = new ParameterExpression[] { instanceExpression, valueExpression };

        // Compile SetMethod method call to set delegate.
        _propertySetter = Expression
            .Lambda<Action<TObject, object?>>(methodCallExpression, parameters)
            .Compile();
    }
    #endregion
}
