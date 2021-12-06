using System;
using static System.Console;

namespace Project
{
    class Program
    {
        static void Main(string[] args)
        {            
            FEM task = new("file/test1/", 1);
            task.solve();
            //WriteLine(task.dataAll());
        }
    }
}
