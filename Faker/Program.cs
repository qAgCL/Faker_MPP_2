using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace Facker
{
    class Program
    {
        public class hui
        {
            int dasd;
        }
        static void Main(string[] args)
        {
            Faker faker = new Faker();
            List<int> type = new List<int>();

            foreach(var i in faker.Create<List<List<int>>>())
            {
                foreach (var j in i)
                {
                    Console.WriteLine(j.ToString());
    
                }
                Console.WriteLine();
            }
            Console.ReadLine();
        }
    }
}
