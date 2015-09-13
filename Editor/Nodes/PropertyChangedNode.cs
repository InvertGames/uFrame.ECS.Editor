using System.CodeDom;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class PropertyChangedNode : PropertyChangedNodeBase, ISequenceNode, ISetupCodeWriter {
        private PropertyIn _PropertyIn;
        private string _PropertyInId;

        public override bool CanGenerate { get { return true; } }
        //public override string Name
        //{
        //    get
        //    {
                
        //        return "PropertyChanged"; 
        //    }
        //    set { base.Name = value; }
        //}

        public IContextVariable SourceProperty
        {
            get { return  PropertyIn.Item; }
        }

        [Invert.Json.JsonProperty()]
        public virtual string PropertyInId
        {
            get
            {
                if (_PropertyInId == null)
                {
                    _PropertyInId = Guid.NewGuid().ToString();
                }
                return _PropertyInId;
            }
            set
            {
                _PropertyInId = value;
            }
        }
        public PropertyIn PropertyIn
        {
            get
            {
                if (Repository == null)
                {
                    return null;
                }
                if (_PropertyIn != null)
                {
                    return _PropertyIn;
                }
                return _PropertyIn ?? (_PropertyIn = new PropertyIn() { Repository = Repository, Node = this, Identifier = PropertyInId });
            }
        }

        public override string DisplayName
        {
            get
            {
                if (Repository != null && !string.IsNullOrEmpty(this.PropertyInId) && PropertyIn != null && SourceProperty != null)
                    return string.Format("{0} {1} Property Changed", SourceProperty.Source.Node.Name, SourceProperty.Source.Name);
                return "PropertyChanged";
            }
        }
        public override string HandlerMethodName
        {
            get
            {
                if (Repository != null && !string.IsNullOrEmpty(this.PropertyInId) && PropertyIn != null && SourceProperty != null)
                    return string.Format("{0}{1}PropertyChanged", SourceProperty.Source.Node.Name, SourceProperty.Source.Name);
                return Graph.CurrentFilter.Name + "PropertyChanged";
            }
        }
        public override string HandlerFilterMethodName
        {
            get
            {
                if (Repository != null && !string.IsNullOrEmpty(this.PropertyInId) && PropertyIn != null && SourceProperty != null)
                    return string.Format("{0}{1}PropertyChangedFilter", SourceProperty.Source.Node.Name, SourceProperty.Source.Name);
                return Graph.CurrentFilter.Name + "PropertyChangedFilter";
            }
        }
         
        public override string EventType
        {
            get
            {
                if (SourceProperty == null) return "...";
                return this.SourceProperty.Node.Name;
                //return SourceInputSlot.InputFrom<IMappingsConnectable>().Name;
            }
            set
            {
                
            }
        }

        public override void Validate(List<ErrorInfo> errors)
        {
            base.Validate(errors);
            if (SourceProperty == null) 
                errors.AddError("Source Property not set",this.Node);
        }

        protected override void WriteHandlerInvoker(CodeMethodInvokeExpression handlerInvoker, CodeMemberMethod handlerFilterMethod)
        {
            base.WriteHandlerInvoker(handlerInvoker, handlerFilterMethod);
            handlerInvoker.Parameters.Add(new CodeSnippetExpression("value"));
        }

        public override void WriteEventSubscription(TemplateContext ctx, CodeMemberMethod filterMethod, CodeMemberMethod handlerMethod)
        {
            //base.WriteEventSubscription(ctx, filterMethod, handlerMethod);
            var relatedTypeProperty = SourceProperty.Source;
            filterMethod.Parameters.Add(new CodeParameterDeclarationExpression(relatedTypeProperty.RelatedTypeName, "value"));
            handlerMethod.Parameters.Add(new CodeParameterDeclarationExpression(relatedTypeProperty.RelatedTypeName, "value"));

            ctx._("this.PropertyChanged<{0},{1}>(Group=>{2}Observable, {3})", EventType, relatedTypeProperty.RelatedTypeName, SourceProperty.Name, filterMethod.Name);
        }

        public override bool IsLoop
        {
            get { return false; }
        }

        public IEnumerable GetObservableProperties()
        {
            foreach (var item in FilterInputs)
            {
                foreach (var p in item.InputFrom<IMappingsConnectable>().GetObservableProperties())
                {
                    yield return p;
                }
            }
        }
    }
    
    public partial interface IPropertyChangedConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
