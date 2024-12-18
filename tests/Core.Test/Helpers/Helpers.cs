using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mshop.Core.Test.Helpers
{
    public static class Helpers
    {
        public static T CopyObject<T>(T obj)
        {
            if (obj == null)
                return default;

            var objType = obj.GetType();
            var clone = Activator.CreateInstance(objType);

            foreach (var field in objType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var value = field.GetValue(obj);
                field.SetValue(clone, value);
            }

            foreach (var prop in objType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.CanWrite)
                {
                    var value = prop.GetValue(obj);
                    prop.SetValue(clone, value);
                }
            }

            return (T)clone;
        }
    }
}
