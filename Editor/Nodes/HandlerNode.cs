using Invert.Json;
using System.CodeDom;
using Invert.Data;
using uFrame.Attributes;
using UnityEngine;

namespace Invert.uFrame.ECS
{
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface ISystemGroupProvider
    {
        IEnumerable<IMappingsConnectable> GetSystemGroups();
    }

    public class HandlerNode : HandlerNodeBase, ISetupCodeWriter, ICodeOutput, ISequenceNode, ISystemGroupProvider, IVariableNameProvider
    {
        public override string Title
        {
            get { return Name; }
        }

        public override bool AllowMultipleOutputs
        {
            get { return false; }
        }

        public override bool AllowExternalNodes
        {
            get { return false; }
        }

        public override Color Color
        {
            get { return Color.blue; }
        }

        public virtual string DisplayName
        {
            get
            {

                if (Meta == null)
                {

                    return "Event Not Found";
                }
                return Meta.Title;
            }
        }

        private EntityGroupIn[] _contextInputs;
        private EntityGroupIn _entityGroup;
        private string _eventIdentifier;
        private EventNode _eventNode;
        private IEventMetaInfo _meta;
        private string _metaType;

        public override bool AllowInputs
        {
            get
            {
                return !HandlerInputs.Any();
            }
        }

        public virtual bool CanGenerate
        {
            get { return Meta != null; }
        }

        public IEnumerable<ConnectionData> Connections { get; set; }

        public IMappingsConnectable ContextNode
        {
            get { return this.InputFrom<IMappingsConnectable>(); }
        }

        public EntityGroupIn EntityGroup
        {
            get
            {
                return _entityGroup ?? (_entityGroup = new EntityGroupIn()
                {
                    Repository = Repository,
                    Node = this,
                    Identifier = this.Identifier + ":" + "Group"
                });
            }
            set { _entityGroup = value; }
        }

        public virtual string EventType
        {
            get { return Meta.FullName; }
            set { MetaType = value; }
        }

        public virtual IEnumerable<IFilterInput> FilterInputs
        {
            get
            {
                foreach (var handlerIn in HandlerInputs)
                {
                    if (handlerIn.FilterNode != null)
                        yield return handlerIn;
                }
            }
        }

        public IMappingsConnectable FilterNode
        {
            get { return this.InputFrom<IMappingsConnectable>(); }
        }

        public override IEnumerable<IGraphItem> GraphItems
        {
            get
            {
                foreach (var item in HandlerInputs)
                    yield return item;

                foreach (var item in this.PersistedItems)
                    yield return item;
                //foreach (var item in ContextVariables)
                //{
                //    yield return item;
                //}
            }
        }

        public virtual string HandlerFilterMethodName
        {
            get { return Name + "Filter"; }
        }

        public EntityGroupIn[] HandlerInputs
        {
            get { return _contextInputs ?? (_contextInputs = GetHandlerInputs().ToArray()); }
            set { _contextInputs = value; }
        }

        public virtual string HandlerMethodName
        {
            get { return Name + "Handler"; }
        }

        public string HandlerPropertyName
        {
            get { return "Item"; }
        }

        public virtual bool IsLoop
        {
            get
            {
                if (Meta.Dispatcher) return false;
                return Meta.SystemEvent || HandlerInputs.All(p => p == EntityGroup);
            }
        }

        public bool IsSystemEvent
        {
            get
            {
                return Meta != null && Meta.SystemEvent;
            }
        }

        public string MappingId
        {
            get { return "EntityId"; }
        }

        public IEventMetaInfo Meta
        {
            get
            {
                if (string.IsNullOrEmpty(MetaType))
                    return null;
      

                if (_meta != null) return _meta;

                return _meta = Repository.GetSingle<EventNode>(MetaType) as IEventMetaInfo ?? (uFrameECS.Events.ContainsKey(MetaType) ? uFrameECS.Events[MetaType] : null);
            }
            set
            {
                _meta = value;
                _metaType = value.FullName;
            }
        }

        [JsonProperty]
        public string MetaType
        {
            get { return _metaType; }
            set
            {
                _metaType = value;
            }
        }

        public IEnumerable<IContextVariable> Vars
        {
            get { return GetAllContextVariables(); }
        }

        public virtual int SetupOrder { get { return 0; } }
        public override void Validate(List<ErrorInfo> errors)
        {
            base.Validate(errors);
            if (Repository.All<HandlerNode>().Any(p => p != this && p.Name == Name))
            {
                errors.AddError("This name is already being used", this, () =>
                {
                    Name = Name + Repository.All<HandlerNode>().Count();
                });
            }
        }

        public void Accept(IHandlerNodeVisitor visitor)
        {
            foreach (var item in HandlerInputs)
            {
                visitor.Visit(item);
            }
            visitor.Visit(this.Right);
        }

        public virtual string BeginWriteLoop(TemplateContext ctx, IMappingsConnectable connectable)
        {
            // ctx.PushStatements(ctx._if("{0} != null", ContextNode.SystemPropertyName).TrueStatements);

            ctx._("var {0}Items = {1}", connectable.Name, connectable.EnumeratorExpression);

            var iteration = new CodeIterationStatement(
                new CodeSnippetStatement(string.Format("var {0}Index = 0", connectable.Name)),
                new CodeSnippetExpression(string.Format("{0}Index < {0}Items.Count", connectable.Name)),
                new CodeSnippetStatement(string.Format("{0}Index++", connectable.Name))
                );

            ctx.CurrentStatements.Add(iteration);
            ctx.PushStatements(iteration.Statements);
            return string.Format("{0}Items[{0}Index]", connectable.Name);
        }

        public virtual void EndWriteLoop(TemplateContext ctx)
        {
            ctx.PopStatements();
        }

        public override IEnumerable<IContextVariable> GetContextVariables()
        {
            var evtNode = Meta;
            if (evtNode != null && !evtNode.SystemEvent)
            {
                yield return new ContextVariable("Event")
                {
                    Repository = this.Repository,
                    Node = this,
                    VariableType = Meta,

                };

                //foreach (var child in evtNode.Members)
                //{
                //    yield return new ContextVariable("Event", child.Name)
                //    {
                //        Repository = this.Repository,
                //        Node = this,
                //        VariableType = new SystemTypeInfo(child.Type)
                //    };
                //}
            }
            foreach (var input in FilterInputs)
            {
                var filter = input.FilterNode;
                foreach (var item in filter.GetVariables(input))
                {
                    yield return item;
                }
            }
        }

        public virtual IEnumerable<IMappingsConnectable> GetSystemGroups()
        {
            //foreach (var item in Scope)
            //{
            //    yield return item.SourceItem as IMappingsConnectable;
            //}
            foreach (var input in FilterInputs)
            {
                var filter = input.FilterNode;
                if (filter != null)
                {
                    yield return filter;
                }
            }
        }

        public override void WriteCode(IHandlerNodeVisitor visitor, TemplateContext ctx)
        {
            VariableNode.VariableCount = 0;
            var handlerMethod = WriteHandler(ctx);
            var filterMethod = WriteHandlerFilter(ctx, handlerMethod);
            WriteEventSubscription(ctx, filterMethod, handlerMethod);
        }

        public virtual void WriteEventSubscription(TemplateContext ctx, CodeMemberMethod filterMethod, CodeMemberMethod handlerMethod)
        {

            if (!IsSystemEvent)
            {
                ctx._("this.OnEvent<{0}>().Subscribe(_=>{{ {1}(_); }}).DisposeWith(this)", EventType,
                    HandlerFilterMethodName);
            }
            else
            {
                var meta = Meta as EventMetaInfo;
                if (meta != null)
                {
                    ctx.CurrentDeclaration.BaseTypes.Add(EventType);
                    var method = meta.SystemType.MethodFromTypeMethod(Meta.SystemEventMethod, false);
                    method._("{0}()", filterMethod.Name);
                    ctx.CurrentDeclaration.Members.Add(method);
                }

            }

        }

        public virtual CodeMemberMethod WriteHandler(TemplateContext ctx)
        {
            var handlerMethod = ctx.CurrentDeclaration.protected_func(typeof(void), HandlerMethodName);

            if (!IsSystemEvent)
                handlerMethod.Parameters.Add(new CodeParameterDeclarationExpression(
                     EventType,
                     "data"
                 ));

            // Push the context on the code template
            var prevMethod = ctx.CurrentMethod;
            ctx.CurrentMember = handlerMethod;
            ctx.PushStatements(handlerMethod.Statements);
            // Now writing the handler method contents
            var name = "handler";
            var field = ctx.CurrentDeclaration._private_(HandlerMethodName, HandlerMethodName + "Instance");
            field.InitExpression = new CodeSnippetExpression(string.Format("new {0}()", HandlerMethodName));

            ctx._("var {0} = {1}Instance;", name, HandlerMethodName);
            ctx._("{0}.System = this", name);

            WriteHandlerSetup(ctx, name, handlerMethod);
            if (DebugSystem.IsDebugMode)
                ctx._("StartCoroutine({0}.Execute())", name);
            else
                ctx._("{0}.Execute()", name);
            // End handler method contents
            ctx.PopStatements();
            ctx.CurrentMember = prevMethod;
            return handlerMethod;
        }

        public virtual CodeMemberMethod WriteHandlerFilter(TemplateContext ctx, CodeMemberMethod handlerMethod)
        {
            var handlerFilterMethod = ctx.CurrentDeclaration.protected_func(typeof(void), HandlerFilterMethodName);

            if (!IsSystemEvent) // No event data for system events
                handlerFilterMethod.Parameters.Add(new CodeParameterDeclarationExpression(EventType, "data"));

            ctx.PushStatements(handlerFilterMethod.Statements);


            if (!IsLoop)
            {
                var handlerInvoker = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), HandlerMethodName);
                //

                if (!IsSystemEvent)
                    handlerInvoker.Parameters.Add(new CodeSnippetExpression("data"));

                foreach (var item in FilterInputs)
                {
                    var filter = item.FilterNode;
                    if (filter == null) continue;

                    handlerInvoker.Parameters.Add(new CodeSnippetExpression(filter.GetContextItemName(item.Name)));

                    ctx._("var {0} = {1}", filter.GetContextItemName(item.Name),
                        filter.MatchAndSelect("data." + item.MappingId));
                    ctx._if("{0} == null", filter.GetContextItemName(item.Name)).TrueStatements._("return");
                }
                WriteHandlerInvoker(handlerInvoker, handlerFilterMethod);
                ctx.CurrentStatements.Add(handlerInvoker);
            }
            else
            {
                var handlerInvoker = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), HandlerMethodName);
                if (!IsSystemEvent)
                    handlerInvoker.Parameters.Add(new CodeSnippetExpression("data"));
                if (this.EntityGroup.Item != null)
                {
                    var item = this.BeginWriteLoop(ctx, this.EntityGroup.Item);
                    handlerInvoker.Parameters.Add(new CodeSnippetExpression(item));
                    ctx.CurrentStatements.Add(handlerInvoker);
                    this.EndWriteLoop(ctx);
                }
                else
                {
                    ctx.CurrentStatements.Add(handlerInvoker);
                }

            }


            ctx.PopStatements();
            return handlerFilterMethod;
        }

        public virtual void WriteSetupCode(IHandlerNodeVisitor visitor, TemplateContext ctx)
        {
            WriteCode(visitor, ctx);
        }

        protected virtual void WriteHandlerInvoker(CodeMethodInvokeExpression handlerInvoker, CodeMemberMethod handlerFilterMethod)
        {
            // If its a system event then there isn't any event data
        }

        private IEnumerable<EntityGroupIn> GetHandlerInputs()
        {

            bool hasMappings = false;
            var meta = Meta;
            if (meta != null)
            {
                foreach (var item in Meta.GetMembers())
                {
                    if (!item.HasAttribute<uFrameEventMapping>()) continue;
                    var variableIn = new HandlerIn()
                    {
                        Repository = Repository,
                        EventFieldInfo = item,
                        Node = this,
                        Identifier = this.Identifier + ":" + item.MemberName
                    };
                    yield return variableIn;
                    if (item.MemberName != "EntityId")
                        hasMappings = true;
                }
            }
            if (!hasMappings)
            {
                yield return EntityGroup;
            }
        }

        private void WriteEnsureDispatchers(TemplateContext ctx)
        {
            foreach (var item in FilterInputs)
            {
                var filter = item.FilterNode;
                if (filter == null) continue;
                if (Meta.Dispatcher)
                {
                    ctx._("EnsureDispatcherOnComponents<{0}>( {1} )", Meta.TypeName, filter.DispatcherTypesExpression());
                }
            }
        }

        private void WriteHandlerSetup(TemplateContext ctx, string name, CodeMemberMethod handlerMethod)
        {
            if (!IsSystemEvent)
            {
                ctx._("{0}.Event = data", name);
            }
            foreach (var item in this.FilterInputs)
            {
                var filter = item.FilterNode;
                if (filter == null) continue;
                ctx._("{0}.{1} = {2}", name, item.HandlerPropertyName, item.HandlerPropertyName.ToLower());
                handlerMethod.Parameters.Add(new CodeParameterDeclarationExpression(filter.ContextTypeName,
                    item.HandlerPropertyName.ToLower()));
            }
        }


        private int _variableCount;


        [JsonProperty]
        public int VariableCount
        {
            get { return _variableCount; }
            set { this.Changed("VariableCount", ref _variableCount, value); }
        }

        public string GetNewVariableName(string prefix)
        {
            InvertApplication.Log("YUP");
            return string.Format("{0}{1}", prefix, VariableCount++);
        }
    }
}