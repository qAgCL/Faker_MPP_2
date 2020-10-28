using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Facker
{
    public class Faker: IFaker
    {
        private Dictionary<Type, IValueGenerator> generators;
        private Stack<Type> CircleDepend = new Stack<Type>();
        private FakerConfiguration Configuration = null;
        public Faker(FakerConfiguration Configur)
        {
            generators = new Dictionary<Type, IValueGenerator>();
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (IsRequiredType(t, typeof(Generator<>)))
                {
                    if ((t.BaseType.GetGenericArguments().Count() > 0)&&(t.Namespace== "SimpleTypeGenerator"))
                    {
                        generators.Add(t.BaseType.GetGenericArguments()[0], (IValueGenerator)Activator.CreateInstance(t));
                    }
                }
            }
            generators.Add(typeof(List<>), new ListGenerator());
            ScanPlugins(AppDomain.CurrentDomain.BaseDirectory+"\\Plugins\\");
            this.Configuration = Configur;
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

        private void ScanPlugins(string directory)
        {
            
            foreach (var file in Directory.EnumerateFiles(directory, "*.dll", SearchOption.AllDirectories))
            {
                
                    var ass = Assembly.LoadFile(file);
                    foreach (Type type in ass.GetTypes())
                    {
                        if (IsRequiredType(type, typeof(Generator<>)))
                        {
                            if (type.BaseType.GetGenericArguments().Count() > 0)
                            {
                                generators.Add(type.BaseType.GetGenericArguments()[0], (IValueGenerator)Activator.CreateInstance(type));
                            }
                        }
                    }

               
            }

        }
        public object Create(Type type) 
        {
            if (CircleDepend.Where(CircleType => CircleType == type).Count() >= 5)
            {
                Console.WriteLine("Circular Dependency");
                return GetDefaultValue(type);
            }
            CircleDepend.Push(type);
            Faker faker = new Faker(Configuration);
            int seed = (int)DateTime.Now.Ticks & 0x0000FFFF;
            GeneratorContext Context = new GeneratorContext(new Random(seed),type, faker);


            IValueGenerator generator = FindGenerator(type);
          
            if (generator!=null)
            {
                CircleDepend.Pop();
                return generator.Generate(Context);
            }

            var obj = CreateObject(type);
            
            obj = FillObject(obj);
            CircleDepend.Pop();
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
                    if (IsValueSet(field,obj))
                    {
                        ConfigurationRule configurationRule = null;
                        if (Configuration != null)
                        {
                            foreach (ConfigurationRule Rule in Configuration.ConfigurationRules)
                            {
                                if ((Rule.FieldName == field.Name) && (Rule.FieldType == field.FieldType))
                                {
                                    configurationRule = Rule;
                                }
                            }
                        }
                        if (configurationRule == null)
                        {
                            field.SetValue(obj, Create(field.FieldType));
                        }
                        else
                        {
                            int seed = (int)DateTime.Now.Ticks & 0x0000FFFF;
                            Faker faker = new Faker(Configuration);
                            GeneratorContext Context = new GeneratorContext(new Random(seed), field.FieldType, faker);
                            IValueGenerator test = (IValueGenerator)Activator.CreateInstance(configurationRule.GeneratorName);
                            field.SetValue(obj,((IValueGenerator)Activator.CreateInstance(configurationRule.GeneratorName)).Generate(Context));
                        }
                        
                    }
                }

                foreach (PropertyInfo property in Properties)
                {
                    if ((property.CanWrite)&& (IsValueSet(property, obj)))
                    {
                        ConfigurationRule configurationRule = null;
                        if (Configuration != null)
                        {
                            foreach (ConfigurationRule Rule in Configuration.ConfigurationRules)
                            {
                                if ((Rule.FieldName == property.Name) && (Rule.FieldType == property.PropertyType))
                                {
                                    configurationRule = Rule;
                                }
                            }
                        }
                        if (configurationRule == null)
                        {
                            property.SetValue(obj, Create(property.PropertyType));
                        }
                        else
                        {
                            int seed = (int)DateTime.Now.Ticks & 0x0000FFFF;
                            Faker faker = new Faker(Configuration);
                            GeneratorContext Context = new GeneratorContext(new Random(seed), property.PropertyType, faker);
                            IValueGenerator test = (IValueGenerator)Activator.CreateInstance(configurationRule.GeneratorName);
                            property.SetValue(obj, ((IValueGenerator)Activator.CreateInstance(configurationRule.GeneratorName)).Generate(Context));
                        }
                    }
                }
            }
            return obj;
        }
        private bool IsValueSet(MemberInfo member, object obj)
        {
            if (member is FieldInfo field)
            {
                if (GetDefaultValue(field.FieldType) == null) return true;

                if (field.GetValue(obj).Equals(GetDefaultValue(field.FieldType))) return true;
            }
            if (member is PropertyInfo property)
            {
                if (GetDefaultValue(property.PropertyType) == null) return true;
                if (property.GetValue(obj).Equals(GetDefaultValue(property.PropertyType))) return true;
            }
            return false;
        }
        private  object GetDefaultValue(Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);
            else
                return null;
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
                    

                    ConfigurationRule configurationRule = null;
                    if (Configuration != null)
                    {
                        foreach (ConfigurationRule Rule in Configuration.ConfigurationRules)
                        {
                            if ((Rule.FieldName == ParametersInfo[i].Name) && (Rule.FieldType == ParametersInfo[i].ParameterType))
                            {
                                configurationRule = Rule;
                            }
                        }
                    }
                    if (configurationRule == null)
                    {
                        Parametrs[i] = Create(ParametersInfo[i].ParameterType);
                    }
                    else
                    {
                        int seed = (int)DateTime.Now.Ticks & 0x0000FFFF;
                        Faker faker = new Faker(Configuration);
                        GeneratorContext Context = new GeneratorContext(new Random(seed), type, faker);
                        IValueGenerator test = (IValueGenerator)Activator.CreateInstance(configurationRule.GeneratorName);
                        Parametrs[i] = ((IValueGenerator)Activator.CreateInstance(configurationRule.GeneratorName)).Generate(Context);
                    }
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
