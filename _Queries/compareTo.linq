<Query Kind="Program" />

void Main()
{
	Foo
		x = new Foo { MyProperty = 8, MyProperty2 = "CCC", MyProperty3 = 8, MyProperty4 = new Bar {} },
		y = new Foo { MyProperty = 4, MyProperty2 = "DDD", MyProperty3 = 0 };

	x.CompareWith(y).Dump();
}

public class Foo
{
	public int MyProperty { get; set; }
	public string MyProperty2 { get; set; }
	public int MyProperty3 { get; set; }
	public Bar MyProperty4 { get; set; }
	public string MyProperty5 => "petitpanda";
}

public class Bar { }

public static class CompareExtension
{
	public static object CompareWith<T>(this T me, Foo with)
	{
		var collection =
			from x in me.GetType().GetProperties()
			where x.CanRead && x.CanWrite
			
			let LfromValue = x.GetValue(me)
			let LtoValue = x.GetValue(with)
			let isEqual = LfromValue.Equals(LtoValue)
			where !isEqual
			where x.PropertyType.IsValueType || x.PropertyType == typeof(string)
			select new CompareWithItem
			{
				Name = x.Name,
				thisValue = LfromValue,
				withValue = LtoValue,				
			};
		return collection;
	}

	public class CompareWithItem
	{
		public object x {get;set;}
		public string Name { get; set; }
		public object thisValue { get; set; }
		public object withValue { get; set; }
	}
}
