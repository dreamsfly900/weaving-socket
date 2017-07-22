using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
namespace WeaveBase
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
            return Copy(this.Value);
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
                targetDeepCopyObj = System.Activator.CreateInstance(targetType);   //创建引用对象   
                System.Reflection.MemberInfo[] memberCollection = obj.GetType().GetMembers();
                foreach (MemberInfo member in memberCollection)
                {
                    if (member.MemberType == System.Reflection.MemberTypes.Field)
                    {
                        System.Reflection.FieldInfo field = (System.Reflection.FieldInfo)member;
                        Object fieldValue = field.GetValue(obj);
                        if (fieldValue is ICloneable)
                        {
                            field.SetValue(targetDeepCopyObj, (fieldValue as ICloneable).Clone());
                        }
                        else
                        {
                            field.SetValue(targetDeepCopyObj, Copy(fieldValue));
                            //field.SetValue(targetDeepCopyObj, (fieldValue));
                        }
                    }
                    else if (member.MemberType == System.Reflection.MemberTypes.Property)
                    {
                        System.Reflection.PropertyInfo myProperty = (System.Reflection.PropertyInfo)member;
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
