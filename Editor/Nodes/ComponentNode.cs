using UnityEngine;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;

    public class CollectionTypeInfo : ITypeInfo
    {
        public static SystemTypeInfo ListType = new SystemTypeInfo(typeof(IList));
        public CollectionsChildItem ChildItem { get; set; }

        public bool IsArray { get { return false; } }

        public bool IsList
        {
            get { return true; }
        }

        public bool IsEnum
        {
            get { return false; }
        }

        public ITypeInfo InnerType { get { return ChildItem.MemberType; }}
        public string TypeName { get { return string.Format("List<{0}>", ChildItem.MemberType.FullName); }}

        public string FullName
        {
            get { return "System.Collections.Generic." + TypeName; }
        }

        public IEnumerable<IMemberInfo> GetMembers()
        {
            return ListType.GetMembers();
        }

        public bool IsAssignableTo(ITypeInfo info)
        {
            return ListType.IsAssignableTo(info);
        }
    }
    public class ComponentNode : ComponentNodeBase, IComponentsConnectable, IMappingsConnectable, ITypedItem {
 

        public IEnumerable<ComponentNode> WithAnyComponents
        {
            get { yield break; }
        }

        public IEnumerable<ComponentNode> SelectComponents
        {
            get { yield return this; }
        }

        public string GetContextItemName(string mappingId)
        {
            return mappingId + Name;
        }

        public string ContextTypeName
        {
            get { return Name; }
        }

        public override IEnumerable<IMemberInfo> GetMembers()
        {
            foreach (var item in Properties)
            {
                yield return item;
            }
            foreach (var item in Collections)
            {
                yield return new DefaultMemberInfo()
                {
                    MemberName = item.Name,
                    MemberType = new CollectionTypeInfo() { ChildItem = item }
                };
            }
        }

        //public override IEnumerable<IMemberInfo> GetMembers()
        //{
            
        //}
        
        public override bool IsAssignableTo(ITypeInfo info)
        {
            var systemInfo = info as SystemTypeInfo;
            if (systemInfo != null)
            {
                if (systemInfo.SystemType == typeof (MonoBehaviour)) return true;
                if (systemInfo.SystemType.Name == "IEcsComponent") return true;
                if (systemInfo.SystemType.Name == "EcsComponent") return true;
                if (systemInfo.SystemType.Name == "uFrameComponent") return true;
            }
            return base.IsAssignableTo(info);
        }

        public IEnumerable<IContextVariable> GetVariables(IFilterInput input)
        {
            yield return new ContextVariable(input.HandlerPropertyName)
            {
                Repository = this.Repository,
                Node = this,
                Source = this,
                VariableType = this,
                //TypeInfo =  typeof(MonoBehaviour)
            };
            yield return new ContextVariable(input.HandlerPropertyName, "EntityId")
            {
                Repository = this.Repository,
                Node = this,
                VariableType = new SystemTypeInfo(typeof(int)),

            };
            yield return new ContextVariable(input.HandlerPropertyName, "Entity")
            {
                Repository = this.Repository,
                Node = this,
                VariableType = new SystemTypeInfo(typeof(MonoBehaviour)),
                //TypeInfo = typeof(MonoBehaviour)
            };

            foreach (var item in PersistedItems.OfType<IMemberInfo>())
            {
                yield return new ContextVariable(input.HandlerPropertyName,item.MemberName)
                {
                    Repository = this.Repository,
                    Node = this,
                    Source = item as ITypedItem,
                    VariableType = item.MemberType
                };
            }
        }

        public string SystemPropertyName
        {
            get { return this.Name + "Manager"; }
        }

        public string EnumeratorExpression
        {
            get { return string.Format("{0}.Components", SystemPropertyName); }
        }

        public string MatchAndSelect(string mappingExpression)
        {
            return string.Format("{0}[{1}]",SystemPropertyName,mappingExpression);
        }

        public string DispatcherTypesExpression()
        {
            return string.Format("typeof({0})", this.Name);
        }

        public IEnumerable<PropertiesChildItem> GetObservableProperties()
        {
            return Properties;
        }

       
    }
    
    public partial interface IComponentConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
