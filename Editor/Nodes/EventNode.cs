using Invert.Json;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class EventNode : EventNodeBase {

        [InspectorProperty]
        public bool Dispatcher
        {
            get { return this["Dispatcher"]; }
            set { this["Dispatcher"] = value; }
        }

        public override string ClassName
        {
            get {
                if (Dispatcher)
                {
                    return string.Format("{0}Dispatcher",Name);
                }
                return Name;
            }
        }

        [InspectorProperty]
        public bool NeedsMappings
        {
            get { return this["NeedsMappings"]; }
            set { this["NeedsMappings"] = value; }
        }

        [InspectorProperty]
        public bool SystemEvent
        {
            get { return this["SystemEvent"]; }
            set { this["SystemEvent"] = value; }
        }
        [InspectorProperty, JsonProperty]
        public string SystemEventMethod { get; set; }


    }
    
    public partial interface IEventConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
