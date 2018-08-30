<Query Kind="Expression" />

from x in File.ReadAllLines(@"d:\Tmp\Karpeo-20180829.txt")
//where x.Contains("amandine")
where x.StartsWith("2018-08-29T07:40")
//let timestamp = x.Substring(0, 33)
//let u = 57
//let message = x.Substring(u, x.Length - u)
//select new {timestamp, message}
select new { x, amandine = x.Contains("amandine")}