using System;
using System.Collections.Generic;

namespace Theframeworkshopskeleton
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Point point = new Point("center", 0, 0, 0);
            Console.WriteLine(point.Name);
        }
    }
}