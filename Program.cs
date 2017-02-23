using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace CSharp7FeaturesDemo
{
    class Program
    {
        private static readonly Dictionary<int, MethodInfo> AvailableMethods = new Dictionary<int, MethodInfo>();

        static void Main(string[] args)
        {
            FindAllAvailableMethods();

            while (true)
            {
                ListAllAvailableMethodsToCall();
                var indexOfMethodToInvoke = Console.ReadLine();
                var parsedIndex = int.Parse(indexOfMethodToInvoke);
                InvokeMethod(parsedIndex);

                Thread.Sleep(1000);
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        private static void FindAllAvailableMethods()
        {
            var programType = typeof(Program);
            var methodsInClass = programType.GetMethods(BindingFlags.NonPublic | BindingFlags.Static);

            var index = 0;
            foreach (var method in methodsInClass)
            {
                if (method.CustomAttributes.Any(customAttributes => customAttributes.AttributeType == typeof(Available)))
                {
                    AvailableMethods.Add(index, method);
                    index++;
                }
            }
        }

        private static void ListAllAvailableMethodsToCall()
        {
            Console.WriteLine("Available methods to call:");

            foreach (var method in AvailableMethods)
            {
                Console.WriteLine($"{method.Key}: {method.Value.Name}");
            }

            Console.WriteLine("Write the number of the method you would like to envoke and press enter:");
        }

        private static void InvokeMethod(int index)
        {
            var methodToInvoke = AvailableMethods[index];
            Console.WriteLine($"Invoking method: {methodToInvoke.Name}");
            Thread.Sleep(1000);
            methodToInvoke.Invoke(null, null);
        }
    }
}