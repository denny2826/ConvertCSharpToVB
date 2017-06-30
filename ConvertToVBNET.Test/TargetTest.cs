using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ConvertToVBNET;

namespace ConvertToVBNET.Test
{
    [TestFixture]
    public class TargetTest
    {
        [Test]
        public void DoSomethingTest()
        {
            string[] file = { "Program.cs", "ShipClient.cs" };
            Program.Main(file);
        }
    }
}
