namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class DescriptorNode : DescriptorNodeBase, IMappingsConnectable {
        public IEnumerable<ComponentNode> SelectComponents { get { yield break; } }
        public string GetContextItemName(string mappingId)
        {
            return mappingId + "Item";
        }

        public string ContextTypeName
        {
            get { return "I" + Name; }
        }

        public string SystemPropertyName
        {
            get { return Name + "Manager"; }
        }
        public string EnumeratorExpression
        {
            get { return string.Format("{0}.Components", SystemPropertyName); }
        }


        public IEnumerable<IContextVariable> GetVariables(IFilterInput input)
        {
            yield return new ContextVariable(input.HandlerPropertyName, "EntityId")
            {

                Node = this,
                VariableType = new SystemTypeInfo(typeof(int)),
                Repository = this.Repository,
            };
            yield return new ContextVariable(input.HandlerPropertyName, "Entity")
            {

                Node = this,
                VariableType = new SystemTypeInfo(uFrameECS.EntityComponentType),
                Repository = this.Repository,
                //TypeInfo = typeof(MonoBehaviour)
            };
        }

        public string MatchAndSelect(string mappingExpression)
        {
            return string.Format("{0}[{1}]", SystemPropertyName, mappingExpression);
        }

        public string DispatcherTypesExpression()
        {
            return SystemPropertyName + ".SelectTypes";
        }

        public IEnumerable<PropertiesChildItem> GetObservableProperties()
        {
            yield break;
        }
    }
    
    public partial interface IDescriptorConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
