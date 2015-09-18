using Invert.Json;
using uFrame.Attributes;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class CustomActionNode : CustomActionNodeBase, IActionMetaInfo {
        private uFrameCategory _category;

        [JsonProperty,InspectorProperty]
        public string ActionTitle { get; set; }

        public IEnumerable<string> CategoryPath
        {
            get { yield return this.Graph.Name; }
        }

        public bool IsEditorClass
        {
            get { return false; }
            set { }
        }

        public uFrameCategory Category
        {
            get { return _category ?? (_category = new uFrameCategory(Graph.Name)); }
            set { _category = value; }
        }
    }
    
    public partial interface ICustomActionConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
