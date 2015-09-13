using Invert.Core.GraphDesigner;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    
    
    public class SequenceItemNodeViewModel : SequenceItemNodeViewModelBase {
        
        public SequenceItemNodeViewModel(SequenceItemNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }
        public SequenceItemNode SequenceNode
        {
            get { return GraphItem as SequenceItemNode; }
        }
        protected override void CreateContent()
        {
            foreach (var item in SequenceNode.GraphItems.OfType<IActionIn>())
            {
                var vm = new InputOutputViewModel()
                {
                    Name = item.Name,
                    IsOutput = false,
                    IsInput = true,
                    DataObject = item,
                    IsNewLine = item.ActionFieldInfo == null ? true : item.ActionFieldInfo.DisplayType.IsNewLine,
                    DiagramViewModel = DiagramViewModel
                };
                ContentItems.Add(vm);
                if (vm.InputConnector != null)
                {
                    vm.InputConnector.Style = ConnectorStyle.Circle;
                    vm.InputConnector.TintColor = UnityEngine.Color.green;
                }

            }
            foreach (var item in SequenceNode.GraphItems.OfType<IActionOut>())
            {
                var vm = new InputOutputViewModel()
                {
                    Name = item.Name,
                    DataObject = item,
                    IsOutput = true,
                    IsNewLine = item.ActionFieldInfo == null ? true : item.ActionFieldInfo.DisplayType.IsNewLine,
                    DiagramViewModel = DiagramViewModel
                };
                ContentItems.Add(vm);

                if (!(item is ActionBranch))
                {
                    vm.OutputConnector.Style = ConnectorStyle.Circle;
                    vm.OutputConnector.TintColor = UnityEngine.Color.green;
                }


            }
            base.CreateContent();


        }
    }
}
