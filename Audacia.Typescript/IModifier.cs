﻿namespace Audacia.Typescript
{
    public interface IModifier<T> where T : Element
    {
        string Name { get; }
    }
}