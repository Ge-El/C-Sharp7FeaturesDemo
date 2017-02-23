using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace CSharp7FeaturesDemo
{
    class Program
    {
        #region Demo Implemenation
        private static readonly Dictionary<int, MethodInfo> AvailableMethods = new Dictionary<int, MethodInfo>();
        private const int ConsoleWaitTime = 500;

        static void Main(string[] args)
        {
            FindAllAvailableMethods();

            while (true)
            {
                ListAllAvailableMethodsToCall();
                var indexOfMethodToInvoke = Console.ReadLine();
                var parsedIndex = int.Parse(indexOfMethodToInvoke);
                InvokeMethod(parsedIndex);

                Thread.Sleep(ConsoleWaitTime);
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
            Thread.Sleep(ConsoleWaitTime);
            methodToInvoke.Invoke(null, null);
        }
        #endregion

        #region Out Variables
        /// <summary>
        /// Tries to parse a <c>Guid</c> and print out result
        /// </summary>
        ///<remarks>
        /// Uses predeclared out variables 
        /// </remarks>
        [Available]
        private static void ParseGuidOld()
        {
            Guid parsedGuid;
            var pareseSucceded = Guid.TryParse("49C49483-A2E8-43B3-B5F5-E5650E35EEF2", out parsedGuid);

            var resultMessage = pareseSucceded ? $"Parse succeded guid: {parsedGuid:D} " : "Parse failed";

            Console.WriteLine(resultMessage);
        }


        /// <summary>
        /// Tries to parse a <c>Guid</c> and print out result
        /// </summary>
        /// <remarks>
        /// Uses C# 7 feature out variables, declaration of variables is right in the parameter list where they are passed
        /// </remarks>
        [Available]
        private static void ParseGuidNew()
        {
            var pareseSucceded = Guid.TryParse("49C49483-A2E8-43B3-B5F5-E5650E35EEF2", out var parsedGuid);

            var resultMessage = pareseSucceded ? $"Parse succeded guid: {parsedGuid:D} " : "Parse failed";

            Console.WriteLine(resultMessage);
        }


        /// <summary>
        /// Prints out the value of x
        /// </summary>
        /// <remarks>
        /// Uses C# 7 feature out variables declared in the parameterlist and wildcard.
        /// </remarks>
        [Available]
        private static void PrintPoint()
        {
            // GetPoints(out var x, out *); // I only care about x

            // Console.WriteLine($"X: {x}");

        }

        [Available]
        private static void GetPoints(out int x, out int y)
        {
            x = 1;
            y = 2;
        }

        #endregion

        #region Pattern Matching

        [Available]
        private static void Old()
        {
            throw new NotImplementedException();
        }

        [Available]
        private static void New()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}