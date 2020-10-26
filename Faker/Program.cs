using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
namespace Facker
{
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
            public test2 ds;

        }

        class test2
        {
            public int das;
        }
        static void Main(string[] args)
        {
            Faker faker = new Faker();

            Console.WriteLine(faker.Create<DateTime>());
            Console.ReadLine();
        }
    }
}

