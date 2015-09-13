using Invert.Core.GraphDesigner;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    
    
    public class HandlerNodeViewModel : HandlerNodeViewModelBase {
        
        public HandlerNodeViewModel(HandlerNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }

        public HandlerNode Handler
        {
            get { return GraphItem as HandlerNode; }
        }

        public override IEnumerable<string> Tags
        {
            get
            {
                yield return Handler.DisplayName;
             
            }
        }

        public HandlerNode HandlerNode
        {
            get { return GraphItem as HandlerNode; }
        }

        protected override void CreateContent()
        {
           
            if (IsVisible(SectionVisibility.WhenNodeIsNotFilter))
            {
                var inputs = Handler.HandlerInputs;
                //if (inputs.Length > 0)
                //    ContentItems.Add(new GenericItemHeaderViewModel()
                //    {
                //        Name = "Mappings",
                //        DiagramViewModel = DiagramViewModel,
                //        IsNewLine = true,
                //    });
           
         
                
                foreach (var item in inputs)
                {
                    var vm = new InputOutputViewModel()
                    {
                        DataObject = item,
                        Name = item.Title,
                        IsInput = true,
                        IsOutput = false,
                        IsNewLine = true,
                        AllowSelection = true
                    };
                    ContentItems.Add(vm);
                }
            }
            base.CreateContent();
        }
    }
}
