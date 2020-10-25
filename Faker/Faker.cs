using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

namespace Facker
{
    class Faker: IFaker
    {
        private Dictionary<Type, IValueGenerator> generators;
        
        public Faker()
        { 
            generators = new Dictionary<Type, IValueGenerator>();
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (IsRequiredType(t, typeof(Generator<>)))
                {
                    if (t.BaseType.GetGenericArguments().Count() > 0)
                    {
                        generators.Add(t.BaseType.GetGenericArguments()[0], (IValueGenerator)Activator.CreateInstance(t));
                    }
                }
            }
            generators.Add(typeof(List<>), new ListGenerator());

        }
        private bool IsRequiredType(Type GeneratorType, Type RequiredType)
        {
            Type LocalType = GeneratorType;
            while ((LocalType != null) && (LocalType != typeof(object)))
            {
                Type buf;
                if (LocalType.IsGenericType)
                {
                    buf = LocalType.GetGenericTypeDefinition();
                }
                else
                {
                    buf = LocalType;
                }
                if (RequiredType == buf) 
                {
                    return true;
                }
                LocalType = LocalType.BaseType;
            }
            return false;
        }
        public T Create<T>() 
        {
            return (T)Create(typeof(T));
        }


        public object Create(Type type) 
        {
            Faker faker=new Faker();

            int seed = (int)DateTime.Now.Ticks & 0x0000FFFF;
            
            GeneratorContext Context = new GeneratorContext(new Random(seed),type, faker);


            IValueGenerator generator = FindGenerator(type);
          
            if (generator!=null)
            {
                return generator.Generate(Context);
            }

            var obj = CreateObject(type);

            obj = FillObject(obj);
            return obj;
        }
        private object FillObject(object obj)
        {
            if (obj != null)
            {
                FieldInfo[] Fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
                PropertyInfo[] Properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

                foreach (FieldInfo field in Fields)
                {
                    field.SetValue(obj, Create(field.FieldType));
                }

                foreach (PropertyInfo property in Properties)
                {
                    if (property.CanWrite)
                    {
                        property.SetValue(obj, Create(property.PropertyType));
                    }
                }
            }
            return obj;
        }
        private object CreateObject(Type type) {

            ConstructorInfo[] BufConstructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

            IEnumerable<ConstructorInfo> Constructors = BufConstructors.OrderByDescending(Constructor => Constructor.GetParameters().Length);
     
            object Object = null;

            foreach (ConstructorInfo Constructor in Constructors)
            {
                ParameterInfo[] ParametersInfo = Constructor.GetParameters();
                object[] Parametrs = new object[ParametersInfo.Length];
                for (int i = 0; i < Parametrs.Length; i++)
                {
                    Parametrs[i] = Create(ParametersInfo[i].ParameterType);
                }
                try
                {
                    Object = Constructor.Invoke(Parametrs);
                    break;
                }
                catch
                {
                    continue;
                }
            }
            if ((Object == null) && (type.IsValueType)){
                Object = Activator.CreateInstance(type);
            }
            return Object;
        } 
        public IValueGenerator FindGenerator(Type type)
        {
            if (type.IsGenericType)
            {
                type = type.GetGenericTypeDefinition();
            }
            if (generators.ContainsKey(type))
            {
                return generators[type];
            }
            else
            {
                return null;
            };
        }
    }
}
