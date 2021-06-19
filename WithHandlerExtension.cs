using System;
using System.Reflection;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace OxfordV2
{
        public static class WithHandlerExtension
        {
            public static Command WithHandler(this Command command, string name)
            {
                var flags = BindingFlags.NonPublic | BindingFlags.Static;
                var method = typeof(Program).GetMethod(name, flags);

                // method! 
                var handler = CommandHandler.Create(method!);
                command.Handler = handler;
                return command;
            }
        }
}