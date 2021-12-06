using System;
using static System.Console;

namespace Project
{
    class Program
    {
        static void Main(string[] args)
        {
            FEM task = new("file/test2/", 2);
            task.solve();
            //WriteLine(task.dataAll());

        }
    }
}
