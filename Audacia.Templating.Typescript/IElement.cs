﻿namespace Audacia.Templating.Typescript
{
    public interface IElement
    {
        TypescriptBuilder Build(TypescriptBuilder builder, IElement parent);
    }
}