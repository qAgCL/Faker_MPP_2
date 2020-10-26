using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facker
{
    public abstract class Generator<T>: IValueGenerator
    {
        public abstract T ObjectGeneration(Random random);
        public object Generate(GeneratorContext generatorContext)
        {

            return ObjectGeneration(generatorContext.Random);
        }
    }

    public class ListGenerator : IValueGenerator
    {
        private GeneratorContext Context;
        public object Generate(GeneratorContext generatorContext)
        {

            var list = (System.Collections.IList)Activator.CreateInstance(generatorContext.TargetType);

            for (int i = 0; i <= generatorContext.Random.Next(1, 10); i++)
            {
                list.Add(generatorContext.Faker.Create(generatorContext.TargetType.GetGenericArguments()[0]));
            }

            return list;
        }


    }
}
