 using System;
 
 interface InterfaceWithClassConstraints
 {
     class int Zero { get; }
 }

class Program
{
    static void Main()
    {
        foreach (var member in typeof(object).GetMembers())
        {
            Console.WriteLine(member);
        }
    }
}
