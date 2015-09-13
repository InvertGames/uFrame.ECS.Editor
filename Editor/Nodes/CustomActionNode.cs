using Invert.Json;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class CustomActionNode : CustomActionNodeBase {
        [JsonProperty,InspectorProperty]
        public string ActionTitle { get; set; }
    }
    
    public partial interface ICustomActionConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
