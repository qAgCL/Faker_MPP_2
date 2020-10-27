using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facker;
namespace SimpleTypeGenerator
{
        public class IntGenerator : Generator<int>
        {
            public override int ObjectGeneration(Random random)
            {
                
                int RandomValue = random.Next();
                return RandomValue;
            }
        }
        public class FloatGenerator : Generator<float>
        {
            public override float ObjectGeneration(Random random)
            {
                float RandomValue = (float)(random.NextDouble())*random.Next();
                return RandomValue;
            }
        }
        public class LongGenerator : Generator<long>
        {
            public override long ObjectGeneration(Random random)
            {
                long RandomValue = random.Next()<<32 + random.Next();
                return RandomValue;
            }
        }

        public class DoubleGenerator : Generator<double>
        {
            public override double ObjectGeneration(Random random)
            {
                double RandomValue = (double)(random.NextDouble()) * (random.Next());
                return RandomValue;
            }
        }

        public class CharGenerator : Generator<char>
        {
            public override char ObjectGeneration(Random random)
            {
                char RandomValue = (char)(random.Next(0, 255));
                return RandomValue;
            }
        }

        public class BoolGenerator : Generator<bool>
        {
            public override bool ObjectGeneration(Random random)
            {
                bool RandomValue = Convert.ToBoolean(random.Next(0, 1));
                return RandomValue;
            }
        }
}
