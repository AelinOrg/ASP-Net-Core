using System.Reflection;

namespace API.Helpers;

public abstract class AssemblyHelper
{
    private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();

    /// <summary>
    /// A partir de um atributo, retorna uma lista de tipos
    /// </summary>
    /// <typeparam name="TAttribute"></typeparam>
    /// <returns></returns>
    public static IEnumerable<Type> GetTypesWithAttribute<TAttribute>()
        where TAttribute : Attribute
    {
        return _assembly.GetTypes().Where(type => type.GetCustomAttributes(typeof(TAttribute), true).Length > 0);
    }
}
