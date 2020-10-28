using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facker
{
    public class FakerConfiguration
    {
        public List<ConfigurationRule> ConfigurationRules { get; }
        public FakerConfiguration()
        {
            ConfigurationRules = new List<ConfigurationRule>();
        }
      
       public void Add<T1,T2,T3>(System.Linq.Expressions.Expression<Func<T1, T2>> Expression) where T3 : Generator<T2>
        {
            Type ClassName = typeof(T1);
            Type FieldType = typeof(T2);
            Type GeneratorName = typeof(T3);
            string FieldName = ((System.Linq.Expressions.MemberExpression)(Expression.Body)).Member.Name;
            ConfigurationRule configurationRule = new ConfigurationRule(ClassName, FieldType, GeneratorName, FieldName);
            ConfigurationRules.Add(configurationRule);
       }
    }
    public class ConfigurationRule
    {
        public Type ClassName { get; }
        public Type FieldType { get; }
        public Type GeneratorName { get; }
        public string FieldName { get; }
        public ConfigurationRule(Type ClassName, Type FieldType, Type GeneratorName, string FieldName)
        {
            this.ClassName = ClassName;
            this.FieldType = FieldType;
            this.GeneratorName = GeneratorName;
            this.FieldName = FieldName;
        }
    }
}
