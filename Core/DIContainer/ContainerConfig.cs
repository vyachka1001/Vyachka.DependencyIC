using System;
using System.Collections.Generic;

namespace Core.DIContainer
{
    public class ContainerConfig
    {
        internal Dictionary<Type, List<Dependency>> Dependencies { get; }

        public ContainerConfig()
        {
            Dependencies = new Dictionary<Type, List<Dependency>>();
        }

        public void Register<TInterface, TImplementation>(LifeCycle lifeCycle) where TInterface : class
            where TImplementation : class, TInterface
        {
            var inter = typeof(TInterface);
            var impl = typeof(TImplementation);
            if (impl.IsAbstract) throw new ArgumentException($"Abstract {impl.Name} classes are not supported");

            if (Dependencies.ContainsKey(inter))
            {
                Dependencies[inter].Add(new Dependency(impl, lifeCycle, this));
            }
            else
            {
                Dependencies.Add(inter, new List<Dependency> { new(impl, lifeCycle, this) });
            }
        }
    }
}