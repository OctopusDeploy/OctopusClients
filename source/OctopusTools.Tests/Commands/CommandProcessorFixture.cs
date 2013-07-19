//using System;
//using NSubstitute;
//using NUnit.Framework;
//using OctopusTools.Infrastructure;
//using log4net;

//namespace OctopusTools.Tests.Commands
//{
//    [TestFixture]
//    public class CommandProcessorFixture
//    {
//        [Test]
//        public void ShouldFailWithExtraOptions()
//        {
//            var commandLocator = Substitute.For<ICommandLocator>();
//            commandLocator.Find("speak").Returns(new SpeakCommand());
//            var log = Substitute.For<ILog>();
//            var cp = new CommandProcessor(commandLocator, log);
//            var ae = Assert.Throws<ApplicationException>(() => cp.Process(new[] {"speak", "--foo"}));
//            Assert.NotNull(ae.InnerException);
//            Assert.AreEqual("Extra non-recognized parameters for command: --foo", ae.InnerException.Message);
//        }
//    }
//}