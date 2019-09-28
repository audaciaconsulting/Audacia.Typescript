using System;
using System.Net.Http;
using System.Threading;

namespace Audacia.Typescript.Transpiler.Examples
{
	public class SomeAttribute : Attribute
	{
		public SomeAttribute() { }
		
		public SomeAttribute(IGenericInterface<AbstractClass> property)
		{
			Property = property;
		}

		public IGenericInterface<AbstractClass> Property { get; set; }
	}
	
	[SomeAttribute]
	public interface IGenericInterface<T>
	{
		IGenericInterface<T> RecursiveProperty { get; set; }

		AbstractClass AbstractClass { get; set; }
	}

	[SomeAttribute]
	public abstract class AbstractClass
	{
		[SomeAttribute]
		public int Number { get; set; }
	}

	[SomeAttribute]
	public class Class : AbstractClass, IGenericInterface<Class>
	{
		[SomeAttribute]
		public Class References { get; set; }
		
		[SomeAttribute]
		public IGenericInterface<Class> RecursiveProperty { get; set; }

		public AbstractClass AbstractClass { get; set; }

		public int StupidProperty
		{
			get
			{ 
				// todo:
				//new SemaphoreSlim(0).Wait();
				return 0;
			}
		}
	}
}