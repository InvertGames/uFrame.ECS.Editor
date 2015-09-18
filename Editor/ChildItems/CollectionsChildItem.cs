namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class CollectionsChildItem : CollectionsChildItemBase, IMemberInfo {
        public override Type Type
        {
            get { return base.Type ?? typeof(int); }
        }

    }
    
    public partial interface ICollectionsConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}