namespace Audacia.Typescript
{
    public interface IAccessor { }

    public static class Modifier
    {
        public static PublicModifier Public { get; } = new PublicModifier();

        public static ExportModifier Export { get; } = new ExportModifier();

        public static AbstractModifier Abstract { get; } = new AbstractModifier();

        public static ProtectedModifier Protected { get; } = new ProtectedModifier();

        public class PublicModifier : IModifier<Function>, IModifier<Property>, IAccessor
        {
            internal PublicModifier() { }

            public string Name => "public";

            public override string ToString() => Name;
        }

        public class ExportModifier : IModifier<Class>, IModifier<Function>, IModifier<Enum>, IAccessor
        {
            internal ExportModifier() { }

            public string Name => "export";

            public override string ToString() => Name;
        }

        public class AbstractModifier : IModifier<Class>, IModifier<Function>, IModifier<Property>
        {
            internal AbstractModifier() { }

            public string Name => "abstract";

            public override string ToString() => Name;
        }

        public class ProtectedModifier : IModifier<Function>, IModifier<Property>, IAccessor
        {
            public ProtectedModifier() { }

            public string Name => "protected";

            public override string ToString() => Name;
        }

    }
}