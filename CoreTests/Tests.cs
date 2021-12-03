using System;
using Core.DIContainer;
using NUnit.Framework;
using LifeCycle = Core.DIContainer.LifeCycle;

namespace CoreTests
{
    [TestFixture]
    public class Tests
    {
        interface IA
        {
            int rnd { get; }
        }

        interface IB
        {
            int rnd { get; }
            IA a { get; }
        }

        interface IC<T>
        {
            int rnd { get; }
            T t { get; }
        }

        class CImpl1 : IC<IA>
        {
            private int _rnd = new Random().Next();
            public int rnd => _rnd;
            private IA _a;
            public IA t => _a;
        }
        
        class CImpl2 : IC<IA>
        {
            private int _rnd = new Random().Next();
            public int rnd => _rnd;
            private IA _a;
            public IA t => _a;
        }
        
        class AImpl1 : IA
        {
            private int _rnd = new Random().Next();
            public int rnd => _rnd;
        }

        class BImpl1 : IB
        {
            private IA _a;
            public IA a => _a;
            private int _rnd = new Random().Next();
            public int rnd => _rnd;

            public BImpl1(IA a)
            {
                _a = a;
            }
        }

        abstract class AbstractBImpl : IB
        {
            private IA _a;
            public IA a => _a;
            private int _rnd = new Random().Next();
            public int rnd => _rnd;
        }

        [Test]
        public void Resolve_DifferentValues_Instance()
        {
            var c = new ContainerConfig();
            c.Register<IA, AImpl1>(LifeCycle.Instance);
            var s = new Container(c);
            int firstInstanceRnd = s.Resolve<IA>().rnd;
            int secondInstanceRnd = s.Resolve<IA>().rnd;
            Assert.IsTrue(firstInstanceRnd != secondInstanceRnd);
        }
        
        [Test]
        public void Resolve_SameValues_Singleton()
        {
            var c = new ContainerConfig();
            c.Register<IA, AImpl1>(LifeCycle.Singleton);
            var s = new Container(c);
            int firstInstanceRnd = s.Resolve<IA>().rnd;
            int secondInstanceRnd = s.Resolve<IA>().rnd;
            Assert.IsTrue(firstInstanceRnd == secondInstanceRnd);
        }

        [Test]
        public void Resolve_Instance_Instance()
        {
            var c = new ContainerConfig();
            c.Register<IA, AImpl1>(LifeCycle.Instance);
            c.Register<IB, BImpl1>(LifeCycle.Instance);
            var s = new Container(c);
            IB prev = s.Resolve<IB>();
            IB curr = s.Resolve<IB>();
            Assert.AreNotEqual(prev.rnd, curr.rnd);
            Assert.AreNotEqual(prev.a.rnd, curr.a.rnd);
        }
        
        [Test]
        public void Resolve_Instance_Singleton()
        {
            var c = new ContainerConfig();
            c.Register<IA, AImpl1>(LifeCycle.Instance);
            c.Register<IB, BImpl1>(LifeCycle.Singleton);
            var s = new Container(c);
            int preva = s.Resolve<IA>().rnd;
            IB prev = s.Resolve<IB>();
            int curra = s.Resolve<IA>().rnd;
            IB curr = s.Resolve<IB>();
            Assert.AreNotEqual(preva, curra);
            Assert.AreEqual(prev.rnd, curr.rnd);
            Assert.AreEqual(prev.a.rnd, curr.a.rnd);
        }

        [Test]
        public void Resolve_NoDependency()
        {
            var c = new ContainerConfig();
            c.Register<IB, BImpl1>(LifeCycle.Singleton);
            var s = new Container(c);
            Assert.Throws<InvalidOperationException>(() => s.Resolve<IB>());
        }

        [Test]
        public void Register_Abstract()
        {
            var c = new ContainerConfig();
            Assert.Throws<ArgumentException>(() => c.Register<IB, AbstractBImpl>(LifeCycle.Singleton));
        }

        [Test]
        public void ResolveAll_Count_Correct()
        {
            var c = new ContainerConfig();
            c.Register<IC<IA>, CImpl1>(LifeCycle.Instance);
            c.Register<IC<IA>, CImpl2>(LifeCycle.Instance);
            var s = new Container(c);
            Assert.AreEqual(2, s.ResolveAll<IC<IA>>().Length);
        }
    }
}