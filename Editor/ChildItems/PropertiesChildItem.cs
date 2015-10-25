using Invert.Json;
using uFrame.Attributes;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class PropertiesChildItem : PropertiesChildItemBase {
        private string _friendlyName;


        public override string OutputDescription
        {
            get { return "Connect to any other node which represents a Type. This will set corresponding type of the property."; }
        }

        public override string RelatedTypeName
        {
            get
            {
                if (Type == uFrameECS.EntityComponentType)
                {
                    return typeof (int).Name;
                }
                return base.RelatedTypeName;
            }
        }

        public override Type Type
        {
            get
            {
                return base.Type ?? typeof(int);
            }
        }

        [JsonProperty]
        public string FriendlyName
        {
            get
            {
                if (string.IsNullOrEmpty(_friendlyName))
                    return Name;
                return _friendlyName;
            }
            set { _friendlyName = value; }
        }


        public override string DefaultTypeName
        {
            get { return typeof(int).Name; }
        }

        [InspectorProperty]
        public bool Mapping
        {
            get
            {

                return this["Mapping"] || this.Type == uFrameECS.EntityComponentType;
            }
            set { this["Mapping"] = value; }
        }


        [InspectorProperty]
        public bool HideInUnityInspector
        {
            get { return this["HideInUnityInspector"]; }
            set { this["HideInUnityInspector"] = value; }
        }

        public override IEnumerable<Attribute> GetAttributes()
        {
            if (Mapping)
            {
                yield return new uFrameEventMapping(this.Name);
            }
          

        }
    }
    
    public partial interface IPropertiesConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
