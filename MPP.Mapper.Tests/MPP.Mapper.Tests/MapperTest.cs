using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MPP.Mapper.Tests
{
    [TestFixture]
    public class MapperTest
    {



        [Test]
        public void TestMap()
        {
            Source source = new Source() { FirstProperty = 11, SecondProperty = "test"};
            Mapper mapper = new Mapper();
            var dest = mapper.Map<Source, Destination>(source);
            Assert.AreEqual(dest.FirstProperty, source.FirstProperty);
            Assert.AreEqual(dest.SecondProperty, source.SecondProperty);
            Assert.AreEqual(dest.ThirdProperty, 0);
            Assert.AreEqual(dest.FourthProperty, new DateTime());
        }

        [Test]
        public void TestIntLongMap()
        {
            Source source = new Source() { FirstProperty = 11, SecondProperty = "test" };
            Mapper mapper = new Mapper();
            var dest = mapper.Map<Source, DestinationLong>(source);
            Assert.AreEqual(dest.FirstProperty, source.FirstProperty);
            Assert.AreEqual(dest.SecondProperty, source.SecondProperty);
            Assert.AreEqual(dest.ThirdProperty, 0);
            Assert.AreEqual(dest.FourthProperty, new DateTime());
        }

        [Test]
        public void TestSourceIntLongMap()
        {
            SourceLong source = new SourceLong() { FirstProperty = 11, SecondProperty = "test" };
            Mapper mapper = new Mapper();
            var dest = mapper.Map<SourceLong, Destination>(source);
            Assert.AreEqual(dest.FirstProperty, 0);
            Assert.AreEqual(dest.SecondProperty, source.SecondProperty);
            Assert.AreEqual(dest.ThirdProperty, 0);
            Assert.AreEqual(dest.FourthProperty, new DateTime());
        }

        [Test]
        public void TestDifferentTypeWithSameNameMap()
        {
            SourceString source = new SourceString() { FirstProperty = "11", SecondProperty = "test" };
            Mapper mapper = new Mapper();
            var dest = mapper.Map<SourceString, Destination>(source);
            
            Assert.AreEqual(dest.FirstProperty, 0);
            Assert.AreEqual(dest.SecondProperty, source.SecondProperty);
            Assert.AreEqual(dest.ThirdProperty, 0);
            Assert.AreEqual(dest.FourthProperty, new DateTime());

            source.SecondProperty = "test test test";
            dest = mapper.Map<SourceString, Destination>(source);

            Assert.AreEqual(dest.FirstProperty, 0);
            Assert.AreEqual(dest.SecondProperty, source.SecondProperty);
            Assert.AreEqual(dest.ThirdProperty, 0);
            Assert.AreEqual(dest.FourthProperty, new DateTime());
        }

        public sealed class Source
        {
            public int FirstProperty { get; set; }
            public string SecondProperty { get; set; }
            //public double ThirdProperty { get; set; }
            //public short FourthProperty { get; set; }
        }

        public sealed class SourceString
        {
            public string FirstProperty { get; set; }
            public string SecondProperty { get; set; }
            public double ThirdProperty { get; set; }
            public short FourthProperty { get; set; }
        }

        public sealed class SourceLong
        {
            public long FirstProperty { get; set; }
            public string SecondProperty { get; set; }
            public double ThirdProperty { get; set; }
            public short FourthProperty { get; set; }
        }

        public sealed class Destination
        {
            public int FirstProperty { get; set; }
            public string SecondProperty { get; set; }
            public float ThirdProperty { get; set; }
            public DateTime FourthProperty { get; set; }
        }

        public sealed class DestinationLong
        {
            public long FirstProperty { get; set; }
            public string SecondProperty { get; set; }
            public float ThirdProperty { get; set; }
            public DateTime FourthProperty { get; set; }
        }

    }
}
