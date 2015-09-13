namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    
    
    public class PropertyNodeViewModel : PropertyNodeViewModelBase {
        private string _name;

        public PropertyNodeViewModel(PropertyNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }

        public override IEnumerable<string> Tags
        {
            get { yield break; }
        }

        protected override void DataObjectChanged()
        {
            base.DataObjectChanged();
            _name = GraphItem.Title;
        }

        public override string Name
        {
            get { return _name; }
            set { base.Name = value; }
        }
    }
}
