using System.Linq;
using Lillisp.Core;
using Xunit;

namespace Lillisp.Tests
{
    public class PairTests
    {
        [Fact]
        public void Pair_IsList_ProperList()
        {
            var pair = new Pair(new Atom(AtomType.Number, 1), 
                new Pair(new Atom(AtomType.Number, 2), 
                    new Pair(new Atom(AtomType.Number, 3), Nil.Value)));

            Assert.True(pair.IsList);
        }

        [Fact]
        public void Pair_ToString_ProperList()
        {
            var pair = new Pair(new Atom(AtomType.Number, 1),
                new Pair(new Atom(AtomType.Number, 2),
                    new Pair(new Atom(AtomType.Number, 3), Nil.Value)));

            Assert.Equal("(1 2 3)", pair.ToString());
        }

        [Fact]
        public void Pair_IsList_ConsPair()
        {
            var pair = new Pair(new Atom(AtomType.Number, 1), new Atom(AtomType.Number, 2));

            Assert.False(pair.IsList);
        }

        [Fact]
        public void Pair_ToString_ConsPair()
        {
            var pair = new Pair(new Atom(AtomType.Number, 1), new Atom(AtomType.Number, 2));

            Assert.Equal("(1 . 2)", pair.ToString());
        }

        [Fact]
        public void Pair_Enumerator_ProperList()
        {
            var pair = new Pair(new Atom(AtomType.Number, 1),
                new Pair(new Atom(AtomType.Number, 2),
                    new Pair(new Atom(AtomType.Number, 3), Nil.Value)));

            var list = pair.ToList();

            Assert.Equal(3, list.Count);
            Assert.Equal(1, list.OfType<Atom>().ElementAt(0).Value);
            Assert.Equal(2, list.OfType<Atom>().ElementAt(1).Value);
            Assert.Equal(3, list.OfType<Atom>().ElementAt(2).Value);
        }

        [Fact]
        public void Pair_Enumerator_ImproperList()
        {
            var pair = new Pair(new Atom(AtomType.Number, 1), new Pair(new Atom(AtomType.Number, 2), new Atom(AtomType.Number, 3)));

            var list = pair.ToList();

            Assert.Equal(3, list.Count);
            Assert.Equal(1, list.OfType<Atom>().ElementAt(0).Value);
            Assert.Equal(2, list.OfType<Atom>().ElementAt(1).Value);
            Assert.Equal(3, list.OfType<Atom>().ElementAt(2).Value);
        }

        [Fact]
        public void Pair_Enumerator_ConsPair()
        {
            var pair = new Pair(new Atom(AtomType.Number, 1), new Atom(AtomType.Number, 2));

            var list = pair.ToList();

            Assert.Equal(2, list.Count);
            Assert.Equal(1, list.OfType<Atom>().ElementAt(0).Value);
            Assert.Equal(2, list.OfType<Atom>().ElementAt(1).Value);
        }
    }
}
