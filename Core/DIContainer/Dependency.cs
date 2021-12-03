using System;
using System.Collections.Generic;
using System.Reflection;

namespace Core.DIContainer
{
    internal class Dependency
    {
        private readonly Type type;

        private readonly LifeCycle lifeCycle;

        private readonly ContainerConfig config;

        private readonly ConstructorInfo constructor;

        private object instance;

        public Dependency(Type type, LifeCycle lifeCycle, ContainerConfig config)
        {
            this.type = type;
            this.lifeCycle = lifeCycle;
            this.config = config;
            constructor = GetConstructor();
        }

        public object Instantiate()
        {
            if (lifeCycle == LifeCycle.Instance)
            {
                return GetInstance();
            }

            return instance ??= GetInstance();
        }

        private ConstructorInfo GetConstructor()
        {
            if (type.GetConstructors().Length == 0) throw new InvalidOperationException("No constructors present");
            return type.GetConstructors()[0];
        }

        private object GetInstance()
        {
            return constructor.Invoke(GetArguments(constructor.GetParameters()));
        }

        private object[] GetArguments(IEnumerable<ParameterInfo> parameterInfos)
        {
            var arguments = new List<object>();
            foreach (var parameter in parameterInfos)
            {
                if (config.Dependencies.ContainsKey(parameter.ParameterType))
                {
                    arguments.Add(config.Dependencies[parameter.ParameterType][0].Instantiate());
                }
                else
                {
                    throw new InvalidOperationException("No dependency registered for parameter");
                }
            }

            return arguments.ToArray();
        }
    }
}