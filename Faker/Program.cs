using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
namespace Test
{
    using  Facker;
    public class TestGenerator : Generator<int>
    {
        public override int ObjectGeneration(Random random)
        {

            return 10;
        }
    }
}
namespace Facker
{
    using Test;
    class Program
    {
        class A
        {
            public B B;
        }
        class B
        {
            public C C;

        }
        class C
        {
            public A A;
        }

        class test1
        {
            public int da;
            public int dasd;
            public int dasdas { get; set; }
            public test1(int dasd)
            {
                this.dasd = dasd;
            }
        }

        
        static void Main(string[] args)
        {
            FakerConfiguration configuration = new FakerConfiguration();
            configuration.Add<test1, int, TestGenerator>(test1 => test1.dasd);
            configuration.Add<test1, int, TestGenerator>(test1 => test1.dasdas);
            Faker faker = new Faker(configuration);


            Console.WriteLine(faker.Create<test1>().dasdas);
            
            Console.ReadLine();
        }
    }
}

