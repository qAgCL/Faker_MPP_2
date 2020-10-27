using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facker;

namespace StringGenerator
{
    public class StringGenerator: Generator<string>
    {
        public override string ObjectGeneration(Random random)
        {
            byte[] ByteArray = new byte[random.Next(10,20)];

            for (int i = 0; i < ByteArray.Length; i++)
            {
                ByteArray[i] = (byte)random.Next(90,255);
            }
            string result = System.Text.Encoding.UTF8.GetString(ByteArray);
            return result;
        }
    }
}
