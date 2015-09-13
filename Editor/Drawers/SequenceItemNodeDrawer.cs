namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class SequenceItemNodeDrawer : GenericNodeDrawer<SequenceItemNode,SequenceItemNodeViewModel> {
        
        public SequenceItemNodeDrawer(SequenceItemNodeViewModel viewModel) : 
                base(viewModel) {
        }
    }
}
