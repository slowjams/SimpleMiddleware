using System;

namespace MiddlewareProject
{
    class Program
    {
        static void Main(string[] args) {

            Try(LambdaFirst);
            Wrap(LambdaSecond);
            //Wrap(First);
            //Wrap(Second);

        }

        public static void First() {
            Console.WriteLine("executing first function");
        }

        public static void Second() {
            Console.WriteLine("executing second function");
        }

        public static void LambdaFirst() {
            Wrap(First);
        }

        public static void LambdaSecond() {
            Try(Second);
        }

        public static void Wrap(Action function) {
            Console.WriteLine("start");
            function();
            Console.WriteLine("end");
        }

        public static void Try(Action function) {
            try {
                Console.WriteLine("trying");
                function();
            } catch(Exception)
            {

            }
        }
    }
}
