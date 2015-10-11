using Invert.Core.GraphDesigner;

namespace Invert.uFrame.ECS
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;


    public class SequenceItemNodeViewModel : SequenceItemNodeViewModelBase
    {

        public SequenceItemNodeViewModel(SequenceItemNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) :
                base(graphItemObject, diagramViewModel)
        {
        }
        public SequenceItemNode SequenceNode
        {
            get { return GraphItem as SequenceItemNode; }
        }


        public override void DataObjectChanged()
        {
            base.DataObjectChanged();
            IsBreakpoint = SequenceNode.BreakPoint != null;
        }
        public bool IsBreakpoint { get; set; }


        public virtual string SecondTitle
        {
            get { return SequenceNode.SecondTitle; }
        }


        public override IEnumerable<string> Tags
        {
            get
            {
                if (!string.IsNullOrEmpty(SecondTitle)) yield return Name;
                yield break;
            }
        }

        protected override void CreateContent()
        {
            InputConnectorType = NodeConfig.SourceType;
            OutputConnectorType = NodeConfig.SourceType;
            if (AutoAddProperties)
                AddPropertyFields();
            CreateContentByConfiguration(NodeConfig.GraphItemConfigurations, GraphItem);

            foreach (var item in SequenceNode.GraphItems.OfType<IActionIn>())
            {
                var vm = new InputOutputViewModel()
                {
                    Name = item.Name,
                    IsOutput = false,
                    IsInput = true,
                    DataObject = item,
                    IsNewLine = item.ActionFieldInfo == null || item.ActionFieldInfo.DisplayType == null ? true : item.ActionFieldInfo.DisplayType.IsNewLine,
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
                    IsNewLine = item.ActionFieldInfo == null || item.ActionFieldInfo.DisplayType == null ? true : item.ActionFieldInfo.DisplayType.IsNewLine,
                    DiagramViewModel = DiagramViewModel
                };
                ContentItems.Add(vm);

                if (!(item is ActionBranch))
                {
                    vm.OutputConnector.Style = ConnectorStyle.Circle;
                    vm.OutputConnector.TintColor = UnityEngine.Color.green;
                }


            }



        }

        public virtual bool AutoAddProperties
        {
            get { return true; }
        }
    }
}
