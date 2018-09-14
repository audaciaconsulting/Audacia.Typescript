namespace Audacia.Templating.Typescript
{
    public static class Modifier
    {
        public static PublicModifier Public { get; } = new PublicModifier();

        public static ExportModifier Export { get; } = new ExportModifier();

        public static AbstractModifier Abstract { get; } = new AbstractModifier();

        public static ProtectedModifier Protected { get; } = new ProtectedModifier();
        
        public class PublicModifier : IModifier<Function>, IModifier<Property>
        {
            internal PublicModifier() { }

            public string Name => "public";

            public override string ToString() => Name;
        }

        public class ExportModifier : IModifier<Class>, IModifier<Function>
        {
            internal ExportModifier() { }

            public string Name => "export";

            public override string ToString() => Name;
        }

        public class AbstractModifier : IModifier<Class>, IModifier<Function>
        {
            internal AbstractModifier() { }

            public string Name => "abstract";

            public override string ToString() => Name;
        }

        public class ProtectedModifier : IModifier<Function>, IModifier<Property>
        {
            public ProtectedModifier() { }

            public string Name => "protected";

            public override string ToString() => Name;
        }

    }
}