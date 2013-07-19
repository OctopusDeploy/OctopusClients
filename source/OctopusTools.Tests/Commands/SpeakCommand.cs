//using System;
//using NUnit.Framework;
//using OctopusTools.Commands;
//using OctopusTools.Infrastructure;

//namespace OctopusTools.Tests.Commands
//{
//    public class SpeakCommand : ICommand {
//        public OptionSet Options {
//            get {
//                var options = new OptionSet();
//                options.Add("message=", m => { });
//                return options;
//            }
//        }

//        public virtual void Execute() {
//            Assert.Fail("This should not be called");
//        }
//    }
//}