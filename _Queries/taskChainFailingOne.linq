<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
	Task.Run(() => doFirst())
	.ContinueWith(a => doSecond())
	.ContinueWith(a => doThird());
}

void doFirst()
{
	"First".Dump();
}

void doSecond()
{
	throw new Exception("well that fails");
	"Second".Dump();
}

void doThird()
{
"Third".Dump();
}