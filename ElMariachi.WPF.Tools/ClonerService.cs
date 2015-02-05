using System.Reflection;

namespace ElMariachi.WPF.Tools
{
    public class ClonerService
    {
        private static readonly ClonerService _singleton = new ClonerService();

        public static ClonerService Instance
        {
            get { return _singleton; }
        }

        public T Clone<T>(T objectToClone) where T : new()
        {
            var clonedObject = new T();

            var tType = typeof (T);

            foreach (var property in tType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (property.CanRead && property.CanWrite && !property.PropertyType.IsByRef)
                {
                    var value = property.GetValue(objectToClone, null);
                    property.SetValue(clonedObject, value, null);
                }
            }

            return clonedObject;
        }
    }
}