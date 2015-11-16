using Invert.Json;
using UnityEngine;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    using Invert.Data;

    public enum DescriptorNodeType
    {
        ComponentOnly,
        ComponentProperties
    }
    public class DescriptorNode : DescriptorNodeBase, IMappingsConnectable, IFlagItem {
        private NodeColor _flagColor;
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

        [JsonProperty, NodeProperty]
        public NodeColor FlagColor
        {
            get { return _flagColor; }
            set { this.Changed("FlagColor", ref _flagColor, value); }
        }

        public override Color Color
        {
            get { return CachedStyles.GetColor(_flagColor); }
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

        NodeColor IFlagItem.Color
        {
            get { return FlagColor; }
        }
    }
    
    public partial interface IDescriptorConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
