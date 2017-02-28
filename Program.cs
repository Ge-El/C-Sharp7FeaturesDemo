using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Sockets;
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

        #region Binary Literals
        [Available]
        static void BinaryLiteralsOld()
        {
            int[] numbers = { 1, 2, 4, 8, 16, 32 };

            foreach (var number in numbers)
            {
                Console.WriteLine(number);
            }
        }

        [Available]
        static void BinaryLiteralsNew()
        {
            int[] numbers = { 0b1, 0b10, 0b100, 0b1000, 0b10000, 0b100000 };

            foreach (var number in numbers)
            {
                Console.WriteLine(number);
            }
        }

        [Available]
        static void BinaryLiteralsNewWithDigitSeperator()
        {
            int[] numbers = { 0b1, 0b10, 0b100, 0b1_000, 0b10_000, 0b100_000 };

            foreach (var number in numbers)
            {
                Console.WriteLine(number);
            }
        }
        #endregion

        #region Tuples / Deconstruction / Local Functions
        static readonly int[] Numbers = { 0b1, 0b10, 0b100, 0b1_000, 0b10_000, 0b100_000 };

        [Available]
        static void TuplesOld()
        {
            var result = Tally(Numbers);

            Console.WriteLine( $" Count:{result.Item1}. result:{result.Item2}");
        }

        static Tuple<int, int> Tally(int[] values)
        {
            var sum = 0;
            var count = 0;

            foreach (var value in values)
            {
                sum += value;
                count++;
            }

            return new Tuple<int, int>(count, sum);
        }

        [Available]
        static void TuplesNew()
        {
            var result = TallyNew(Numbers);

            Console.WriteLine($" Count:{result.count}. result:{result.sum}");
        }

        static (int count, int sum) TallyNew(int[] values)
        {
            var result = (count: 0, sum: 0);

            foreach (var value in values)
            {
                result.sum += value;
                result.count++;
            }

            return result;
        }

        [Available]
        static void TuplesNewDeconstruction()
        {
             (var count, var sum) = TallyNew(Numbers);

            Console.WriteLine($" Count:{count}. result:{sum}.");
        }

        [Available]
        static void TuplesNewLocalFunction()
        {
            (var count, var sum) = TallyNewLocalFunction(Numbers);

            Console.WriteLine($" Count:{count}. result:{sum}.");
        }

        static (int count, int sum) TallyNewLocalFunction(int[] numbers)
        {
            var result = (count: 0, sum: 0);

            void Add(int sum, int count)
            {
                result.sum += sum;
                result.count += count;
            }

            foreach (var number in numbers)
            {
                Add(number, 1);
            }
            return result;
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
        private static void OldPatternMatching()
        {
            var obj = new object();

            if (obj == null) return;
            if (obj is int) return;

            Console.WriteLine($"'*'{obj}");
        }

        [Available]
        private static void NewPatternMatching()
        {
            var obj = new object();
            if (obj is null) Console.WriteLine("obj is null"); // Constant pattern
            if (!(obj is int i)) return; // Type pattern

            Console.WriteLine($"'*'{i}");
        }

        class Person
        {
            public string Firstname { get; set; }
        }

        class Student: Person
        {
            public decimal GradePointAverage { get; set; }
        }

         class Teacher: Person
        {
            public string MainSubject { get; set; }
        }

        static Teacher _teacher = new Teacher
        {
            Firstname = "Bill",
            MainSubject = "Math"
        };

        static Student _goodStudent = new Student
        {
            Firstname = "Erik",
            GradePointAverage = 4.8m
        };

        static  Student _badStudent = new Student
        {
            Firstname = "Sarah",
            GradePointAverage = 2.3m
        };

        [Available]
        private static void SwitchStatmentWithPatternMatchingOld()
        {
            CommentPersonOld(_teacher);
            CommentPersonOld(_goodStudent);
            CommentPersonOld(_badStudent);
       
        }

        [Available]
        private static void SwitchStatmentWithPatternMatchingNew()
        {
            CommentPersonNew(_teacher);
            CommentPersonNew(_goodStudent);
            CommentPersonNew(_badStudent);
        }

        static void CommentPersonOld(Person person)
        {
            if (person is Student && ((Student)person).GradePointAverage >= 4m)
            {
                Console.WriteLine($"{person.Firstname} is a good student");
            }
            else if (person is Student && ((Student)person).GradePointAverage < 4m)
            {
                Console.WriteLine($"{person.Firstname} is not a good student");
            }
            else if (person is Teacher)
            {
                Console.WriteLine($"{person.Firstname} main subject is {((Teacher)person).MainSubject}");
            }
            else
            {
                Console.WriteLine($"Persons name is {person.Firstname}");
            }
        }

        static void CommentPersonNew(Person person)
        {
            switch (person)
            {
                case Student gs when (gs.GradePointAverage >= 4m):
                    Console.WriteLine($"{gs.Firstname} is a good student");
                    break;
                case Student bs when (bs.GradePointAverage < 4m):
                    Console.WriteLine($"{bs.Firstname} is not a good student");
                    break;
                case Teacher t:
                    Console.WriteLine($"{t.Firstname} main subject is {t.MainSubject}");
                    break;
                default:
                    Console.WriteLine($"Persons name is {person.Firstname}");
                    break;
                case null:
                    throw new ArgumentNullException(nameof(person));
            }
        }
        #endregion
    }
}