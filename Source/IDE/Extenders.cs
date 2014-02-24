using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Diagnostics;

namespace IDE
{
    public static class Extenders
    {
        public static string PropertyName<TResult>(this object value, Expression<System.Func<TResult>> propertyReturnExpression)
        {
            return ((MemberExpression)propertyReturnExpression.Body).Member.Name;
        }

        public static T PropertyValue<T>(this object value, string propertyName)
        {
            string[] properties = propertyName.Split('.');
            foreach (string property in properties)
            {
                var propertyInfo = value.GetType().GetProperty(property);
                if (propertyInfo != null)
                {
                    value = propertyInfo.GetValue(value, null);

                    if (value == null)
                    {
                        return default(T);
                    }
                }
                else
                {
                    // we have object invalid property
                    Debug.WriteLine("Entity Property " + property + " was not resolved");
                    return default(T);
                }
            }

#if DEBUG
            try
            {
                T temp = (T)value;
            }
            catch
            {
                Debugger.Break();
            }
#endif

            return (T)value;
        }
    }
}
