using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
namespace mvcIpsa
{
    public abstract class BusinessLogicInitializer<T> where T : class
    {
        internal abstract void Initialize();
        internal void CopyFrom(T source, Expression<Func<T, object>> expression)
        {
            NewExpression body = (NewExpression)expression.Body;

            foreach (MemberExpression arg in body.Arguments)
            {
                PropertyInfo property = (PropertyInfo)arg.Member;
                var type = property.PropertyType;

                if (!property.CanWrite)
                    continue;

                object value = property.GetValue(source);

                property.SetValue(this, value);
            }
        }
        internal void CopyFromExcept(T source, Expression<Func<T, object>> expression)
        {
            NewExpression body = (NewExpression)expression.Body;
            var exclude = body.Arguments.Cast<MemberExpression>().Select(x => x.Member.Name);
            CopyFromExcept(source, true, exclude.ToArray());

        }
        internal void CopyFromExcept(T source, bool includeDefualt, params string[] exclude)
        {
            var type = typeof(T);
            var properties = type.GetProperties().Where(x => x.CanWrite);
            foreach (var p in properties)
            {
                if (exclude.Contains(p.Name))
                    continue;

                object value = p.GetValue(source);

                p.SetValue(this, value);
            }
        }
    }
}
