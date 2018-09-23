namespace Audacia.Typescript
{
    /// <summary>
    /// Represents an element which can belong as a member of another element,
    /// such as a property on a class.
    /// </summary>
    /// <typeparam name="T">The type of element this element can be a member of.</typeparam>
    public interface IMemberOf<T> : IElement where T : IElement { }
}