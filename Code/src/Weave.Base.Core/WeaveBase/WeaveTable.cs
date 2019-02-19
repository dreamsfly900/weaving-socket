using System;
using System.Collections.Generic;
using System.Reflection;

namespace Weave.Base
{
    public class WeaveTable
    {
        private List<WeaveTable> temps = new List<WeaveTable>();

        public string Key
        {
            get; set;
        }

        public object Value
        {
            get; set;
        }

        public object Clone()
        {
            return Copy(Value);
        }

        private object Copy(object obj)
        {
            object targetDeepCopyObj;
            Type targetType = obj.GetType();
            //值类型  
            if (targetType.IsValueType == true)
            {
                targetDeepCopyObj = obj;
            }
            //引用类型   
            else
            {
                targetDeepCopyObj = Activator.CreateInstance(targetType);   //创建引用对象   
                MemberInfo[] memberCollection = obj.GetType().GetMembers();
                foreach (MemberInfo member in memberCollection)
                {
                    if (member.MemberType == MemberTypes.Field)
                    {
                        FieldInfo field = (FieldInfo)member;
                        object fieldValue = field.GetValue(obj);
                        if (fieldValue is ICloneable)
                        {
                            field.SetValue(targetDeepCopyObj, (fieldValue as ICloneable).Clone());
                        }
                        else
                        {
                            field.SetValue(targetDeepCopyObj, Copy(fieldValue));
                        }
                    }
                    else if (member.MemberType == MemberTypes.Property)
                    {
                        PropertyInfo myProperty = (PropertyInfo)member;
                        MethodInfo info = myProperty.GetSetMethod(false);
                        if (info != null)
                        {
                            object propertyValue = myProperty.GetValue(obj, null);
                            if (propertyValue is ICloneable)
                            {
                                myProperty.SetValue(targetDeepCopyObj, (propertyValue as ICloneable).Clone(), null);
                            }
                            else
                            {
                                myProperty.SetValue(targetDeepCopyObj, Copy(propertyValue), null);
                            }
                        }
                    }
                }
            }
            return targetDeepCopyObj;
        }

        public bool Add(string key, object value)
        {
            try
            {
                WeaveTable t = new WeaveTable();
                t.Key = key;
                t.Value = value;
                temps.Add(t);
                return true;
            }
            catch { return false; }
        }

        public bool Remove(string key)
        {
            WeaveTable temp = temps.Find(x => x.Key == key);
            temps.Remove(temp);
            return true;
        }

        public int Length
        {
            get { return temps.Count; }
        }

        public object this[int index]
        {
            get
            {
                return temps[index].Value;
            }
            set
            {
                temps[index].Value = value;
            }
        }

        public object GetValueClonebyIndex(int index)
        {
            return temps[index];
        }

        public object GetValueClonebyKey(string key)
        {
            return temps.Find(x => x.Key == key);
        }

        public object this[string key]
        {
            get
            {
                return temps.Find(x => x.Key == key).Value;
            }
            set
            {
                temps.Find(x => x.Key == key).Value = value;
            }
        }
    }
}
