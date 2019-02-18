using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Audacia.Typescript.Transpiler.Configuration;
using Audacia.Typescript.Transpiler.Extensions;
using FirstNamespace;
using SecondNamespace;
using Xunit;

namespace Audacia.Typescript.Transpiler.Tests.Extensions
{
    public class EnumerableExtensionsTests
    {
        public class FilterByInputSettingsTests
        {
            private readonly Type[] _types;

            public FilterByInputSettingsTests()
            {
                this._types = GetTypes();
            }
            [Fact]
            public void NoNamespaces_AllReturned()
            {
                var settings = new InputSettings { Namespaces = null };

                var result = this._types.FilterBy(settings);

                Assert.Equal(this._types, result);
            }

            /// <summary>
            /// Can also specify every namespace manually
            /// </summary>
            [Fact]
            public void AllNamespaces_AllReturned()
            {
                var namespaceToUse = nameof(FirstNamespace);
                var settings = new InputSettings { Namespaces = new[] { new NamespaceSettings(namespaceToUse) } };

                var result = this._types.FilterBy(settings);

                Assert.True(result.Any() && result.All(t => t.Namespace == namespaceToUse),
                    $"Should have only returned types within the specified namespace {namespaceToUse}");
            }

            [Fact]
            public void SomeNamespaces_OnlyTypesInNamespacesReturned()
            {
                var namespaceToUse = nameof(FirstNamespace);
                var settings = new InputSettings { Namespaces = new[] { new NamespaceSettings(namespaceToUse) } };

                var result = this._types.FilterBy(settings);

                Assert.True(result.Any() && result.All(t => t.Namespace == namespaceToUse),
                    $"Should have only returned types within the specified namespace {namespaceToUse}");
            }

            [Fact]
            public void TypeNameDoesNotBelongInNamespace_FilteredOut()
            {
                var settings = new InputSettings
                {
                    Namespaces = new []
                    {
                        new NamespaceSettings(nameof(FirstNamespace))//Only ClassA and ClassB are in here
                        {
                            Types = new []
                            {
                                new TypeNameSettings{ Name = nameof(ClassC)}//doesn't belong to `FirstNamespace`
                            }
                        }
                    }
                };

                var result = this._types.FilterBy(settings);

                Assert.Empty(result);
            }
            
            [Fact]
            public void TypeNameSpecified_TypeReturned()
            {
                var settings = new InputSettings
                {
                    Namespaces = new[]
                    {
                        new NamespaceSettings(nameof(FirstNamespace))//Only ClassA and ClassB are in here
                        {
                            Types = new []
                            {
                                new TypeNameSettings{ Name = nameof(ClassA)}
                            }
                        }
                    }
                };

                var result = this._types.FilterBy(settings);

                Assert.Equal(result, new []{ typeof(ClassA)});
            }
            
            [Fact]
            public void AllTypes_AllReturned()
            {
                var settings = new InputSettings
                {
                    Namespaces = new[]
                    {
                        new NamespaceSettings(nameof(FirstNamespace))//Only ClassA and ClassB are in here
                        {
                            Types = new []
                            {
                                new TypeNameSettings{ Name = nameof(ClassA)},
                                new TypeNameSettings{ Name = nameof(ClassB)}
                            }
                        },
                        new NamespaceSettings(nameof(SecondNamespace))//Only ClassA and ClassB are in here
                        {
                            Types = new []
                            {
                                new TypeNameSettings{ Name = nameof(ClassC)},
                                new TypeNameSettings{ Name = nameof(ClassD)}
                            }
                        }
                    }
                };

                var result = this._types.FilterBy(settings);

                Assert.Equal(this._types, result);

            }
            
            [Fact]
            public void MixtureOfNamespacesAndTypes_FiltersMissingTypeOut()
            {
                var settings = new InputSettings
                {
                    Namespaces = new[]
                    {
                        new NamespaceSettings(nameof(FirstNamespace)),
                        new NamespaceSettings(nameof(SecondNamespace))//Only ClassA and ClassB are in here
                        {
                            Types = new []
                            {
                                new TypeNameSettings{ Name = nameof(ClassC)}
                            }
                        }
                    }
                };

                var result = this._types.FilterBy(settings);
                
                Assert.True(result.Length == 3 && !result.Contains(typeof(ClassD)),
                    $"Should have filtered out {nameof(ClassD)} if other types are specified");
            }

            private Type[] GetTypes()
            {
                return new[]
                {
                    typeof(ClassA),
                    typeof(ClassB),
                    typeof(ClassC),
                    typeof(ClassD)
                };
            }
        }
    }
}

//For testing the types filtering
namespace FirstNamespace
{
    internal class ClassA { }
    internal class ClassB { }
}
namespace SecondNamespace
{
    internal class ClassC { }
    internal class ClassD { }
}