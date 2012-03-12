using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OctopusTools.Tests
{
    [TestClass]
    public class uri_extensions
    {
        [TestMethod]
        public void should_append_suffix_if_there_is_no_overlap()
        {
            var result = new Uri("http://www.mysite.com").EnsureEndsWith("suffix");
            Assert.AreEqual(result.ToString(), "http://www.mysite.com/suffix");
        }


        [TestMethod]
        public void should_remove_any_overlap_btween_base_addres_and_suffix()
        {
            var result = new Uri("http://www.mysite.com/virtual").EnsureEndsWith("/virtual/suffix");
            Assert.AreEqual(result.ToString(), "http://www.mysite.com/virtual/suffix");
        }

    }
}
