using Invert.Json;
using uFrame.Attributes;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    using Invert.Data;

    public class CustomActionNode : CustomActionNodeBase, IActionMetaInfo, IDemoVersionLimit
    {
        public override bool AllowOutputs
        {
            get { return false; }
        }
        public override bool AllowInputs
        {
            get { return false; }
        }
    
        private uFrameCategory _category;
        private bool _isAsync;
        private ActionDescription _descriptionAttribute;

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

        public virtual ActionDescription DescriptionAttribute
        {
            get { return _descriptionAttribute ?? (_descriptionAttribute = new ActionDescription("")); }
            set { _descriptionAttribute = value; }
        }


        [InspectorProperty, JsonProperty]
        public bool IsAsync
        {
            get { return _isAsync; }
            set { this.Changed("IsAsync", ref _isAsync, value); }
        }
    }
    
    public partial interface ICustomActionConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
