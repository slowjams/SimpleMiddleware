using System;
using System.Collections.Generic;
using System.Reflection;

namespace MiddlewareProject
{
   
    class Program
    {
        static void Main(string[] args) {
            var pipe = new PipeBuilder(First).AddPipe(typeof(Try)).AddPipe(typeof(Wrap)).AddPipe(typeof(ThreeMiddleware)).Build();
            pipe("Hello\n");
        }

        public static void First(string msg) {
            Console.WriteLine($"executing {msg} first function");
        }

        public static void Second(string msg) {
            Console.WriteLine($"executing {msg} second function");
        }

        public class PipeBuilder
        {
            Action<string> _mainAction;
            List<Type> _pipeTypes;
            public PipeBuilder(Action<string> mainAction) {
                _mainAction = mainAction;
                _pipeTypes = new List<Type>();
            }

            public PipeBuilder AddPipe(Type pipeType) {
                _pipeTypes.Add(pipeType);
                return this;
            }

            private Action<string> CreatePipe(int index) {
                if (index < _pipeTypes.Count - 1) {
                    var childPipeHandle = CreatePipe(index + 1);
                    var pipe = (Pipe)Activator.CreateInstance(_pipeTypes[index], childPipeHandle);
                    return pipe.Handle;
                } else {
                    var finalPipe = (Pipe)Activator.CreateInstance(_pipeTypes[index], _mainAction);
                    return finalPipe.Handle;
                }
            }

            public Action<string> Build() {
                return CreatePipe(0);
            }
        }

        public abstract class Pipe
        {
            protected Action<string> _action;
            public Pipe(Action<string> action) {
                _action = action;
            }
            public abstract void Handle(string msg);
        }

        public class Wrap : Pipe
        {
            public Wrap(Action<string> action) : base(action) { }
            public override void Handle(string msg) {
                Console.WriteLine($"{msg} Wrap starting\n");
                _action(msg);
                Console.WriteLine("Wrap end\n");
            }
        }

        public class Try : Pipe
        {
            public Try(Action<string> action) : base(action) { }
            public override void Handle(string msg) {
                try {
                    Console.WriteLine($"{msg} Try starting\n");
                    _action(msg);
                    Console.WriteLine($"Try end\n");
                }
                catch (Exception) {

                }
            }
        }

        public class ThreeMiddleware : Pipe
        {
            public ThreeMiddleware(Action<string> action) : base(action) { }
            public override void Handle(string msg) {
                try {
                    Console.WriteLine($"{msg} the third starting\n");
                    _action(msg);
                    Console.WriteLine($"the third end\n");
                }
                catch (Exception) {

                }
            }
        }

        #region
        //public static void LambdaFirst() {
        //    Wrap(First);
        //}

        //public static void LambdaSecond() {
        //    Try(Second);
        //}
        #endregion

        //public static void Wrap(string msg, Action<string> function) {
        //    Console.WriteLine($"{msg} starting");
        //    function(msg);
        //    Console.WriteLine("end");
        //}

        //public static void Try(string msg, Action<string> function) {
        //    try {
        //        Console.WriteLine($"{msg} trying");
        //        function(msg);
        //    } catch(Exception)
        //    {

        //    }
        //}
    }
}
