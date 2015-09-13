namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class SetVariableNodeDrawer : GenericNodeDrawer<SetVariableNode,SetVariableNodeViewModel> {
        
        public SetVariableNodeDrawer(SetVariableNodeViewModel viewModel) : 
                base(viewModel) {
        }
    }
}
