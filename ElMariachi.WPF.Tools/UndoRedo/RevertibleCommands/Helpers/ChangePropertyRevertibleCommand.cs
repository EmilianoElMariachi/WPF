using System;
using System.Reflection;

namespace ElMariachi.WPF.Tools.UndoRedo.RevertibleCommands.Helpers
{
    public class ChangePropertyRevertibleCommand : AbstractRevertibleCommand
    {

        #region Fields & Properties

        private readonly object _propertyOwnerObj;
        private readonly object _newValue;
        private readonly PropertyInfo _propertyInfo;
        private readonly object _oldValue;

        /// <summary>
        /// Obtient le nom de la propriété cible
        /// </summary>
        public string PropertyName
        {
            get { return _propertyInfo.Name; }
        }

        #endregion

        #region Constructors

        public ChangePropertyRevertibleCommand(object propertyOwnerObj, PropertyInfo propertyInfo, object newValue, object oldValue, string description = "")
        {
            if (propertyOwnerObj == null) throw new ArgumentNullException("propertyOwnerObj");
            if (propertyInfo == null) throw new ArgumentNullException("propertyInfo");
            _propertyOwnerObj = propertyOwnerObj;
            _propertyInfo = propertyInfo;
            _newValue = newValue;
            _oldValue = oldValue;
            this.Description = description;
        }

        public ChangePropertyRevertibleCommand(object propertyOwnerObj, PropertyInfo propertyInfo, object newValue, string description = "")
            : this(propertyOwnerObj, propertyInfo, newValue, ReadPropertyValue(propertyOwnerObj, propertyInfo), description)
        {
        }

        public ChangePropertyRevertibleCommand(object propertyOwnerObj, string propertyName, object newValue, string description = "")
            : this(propertyOwnerObj, FindPropertyInfo(propertyOwnerObj, propertyName), newValue, description)
        {
        }

        public ChangePropertyRevertibleCommand(object propertyOwnerObj, string propertyName, object newValue, object oldValue, string description = "")
            : this(propertyOwnerObj, FindPropertyInfo(propertyOwnerObj, propertyName), newValue, oldValue, description)
        {
        }

        #endregion

        #region Methods

        private static object ReadPropertyValue(object propertyOwnerObj, PropertyInfo propertyInfo)
        {
            if (propertyInfo == null) throw new ArgumentNullException("propertyInfo");
            if (propertyOwnerObj == null) throw new ArgumentNullException("propertyOwnerObj");

            return propertyInfo.GetValue(propertyOwnerObj, new object[] { });
        }

        private static PropertyInfo FindPropertyInfo(object propertyOwnerObj, string propertyName)
        {
            return propertyOwnerObj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }

        public override void Do()
        {
            _propertyInfo.SetValue(_propertyOwnerObj, _newValue, new object[] { });
        }

        public override void Undo()
        {
            _propertyInfo.SetValue(_propertyOwnerObj, _oldValue, new object[] { });
        }

        #endregion

    }
}