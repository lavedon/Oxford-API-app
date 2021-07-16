<Query Kind="Statements">
  <RuntimeVersion>5.0</RuntimeVersion>
</Query>

string export = "1,2,3 10,50,9";
export = export.Replace(" ",",");
string[] numbers = export.Trim().Split(',').Dump();


