using System;
using System.Linq;
using uFrame.Attributes;

namespace Invert.uFrame.ECS
{
    public class ActionFieldInfo
    {
        public string Name
        {
            get
            {
                if (DisplayType == null) return _name;
                return DisplayType.DisplayName ?? _name;
            }
            set { _name = value; }
        }
        private FieldDisplayTypeAttribute _displayType;
        private string _name;
        public Type Type { get; set; }
        public ActionAttribute[] MetaAttributes { get; set; }

        public FieldDisplayTypeAttribute DisplayType
        {
            get { return _displayType ?? (_displayType = MetaAttributes.OfType<FieldDisplayTypeAttribute>().FirstOrDefault()); }
            set { _displayType = value; }
        }

        public bool IsGenericArgument { get; set; }
        public bool IsReturn { get; set; }
    }
}