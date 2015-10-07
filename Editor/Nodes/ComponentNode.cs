
using Invert.Data;
using Invert.Json;
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

        public string Namespace { get { return ListType.Namespace; } }

        public IEnumerable<IMemberInfo> GetMembers()
        {
            return ListType.GetMembers();
        }

        public bool IsAssignableTo(ITypeInfo info)
        {
            return ListType.IsAssignableTo(info);
        }

        public string Title { get { return TypeName; } }
        public string Group { get { return ListType.Namespace; } }
        public string SearchTag { get{return FullName;} }
        public string Description { get{return FullName; }set{} }
        public string Identifier { get{return FullName;} set{} }
    }
    public class ComponentNode : ComponentNodeBase, IComponentsConnectable, IMappingsConnectable, ITypedItem, IDemoVersionLimit {
        private string _customIcon;


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
        public override void Validate(List<ErrorInfo> errors)
        {
            
            base.Validate(errors);
            foreach (var item in PersistedItems)
            {
                if (string.IsNullOrEmpty(item.Name))
                {
                    errors.AddError("All items must have a name.", this);
                }
            }
            if (Repository.AllOf<ComponentNode>().Any(p => p != this && p.Name == this.Name))
            {
                errors.AddError(string.Format("The name {0} is already taken", this.Name), this);
            }
        }
        
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
                Node = this,
                Source = this,
                VariableType = new SystemTypeInfo(uFrameECS.EcsComponentType, this),
                Repository = this.Repository,
                //TypeInfo =  typeof(MonoBehaviour)
            };
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

            foreach (var item in GetMembers())
            {
                yield return new ContextVariable(input.HandlerPropertyName,item.MemberName)
                {
                  
                    Node = this,
                    Source = item as ITypedItem,
                    VariableType = item.MemberType,
                    Repository = this.Repository,
                };
            }
        }

        

        [InspectorProperty, JsonProperty]
        public virtual string CustomIcon
        {
            get { return _customIcon; }
            set
            {
                this.Changed("CustomIcon",ref _customIcon,value);
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
