using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Invert.Core;
using Invert.Core.GraphDesigner;
using Invert.Data;
using Invert.Json;
using JetBrains.Annotations;
using uFrame.Attributes;
using UnityEngine;

namespace Invert.uFrame.ECS
{
    public interface IContextVariable : IDiagramNodeItem
    {
        ITypedItem Source { get; }
        string VariableName { get; }
        ITypeInfo VariableType { get; }
        string ShortName { get; }
        string ValueExpression { get; }
        IEnumerable<IContextVariable> GetPropertyDescriptions();
    }


    public interface IVariableExpressionItem
    {
        string Expression { get; set; }
    }


    public class ContextVariable : GenericTypedChildItem, IContextVariable
    {

        private string _memberExpression;
        private ITypeInfo _variableType;
        private List<object> _items;

        public override string Identifier
        {
            get { return Node.Identifier + ":" + MemberExpression; }
            set { }
        }

        public ContextVariable(params object[] items)
        {
            Items = items.ToList();
        }

        public virtual List<object> Items
        {
            get { return _items ?? (_items = new List<object>()); }
            set { _items = value; }
        }

        public override string ToString()
        {
            return MemberExpression;
        }

        public virtual string MemberExpression
        {
            get
            {

                return _memberExpression ?? (_memberExpression =

                    string.Join(".", Items.Select(p =>
                        {
                            var cv = p as IDiagramNodeItem;
                            if (cv != null) return cv.Name;
                            return (string)p;
                        }).ToArray()));
            }
        }

        public override string Title
        {
            get { return ShortName; }
        }

        public override string Group
        {
            get
            {
                if (Items.Count < 1)
                    return "Missing";

                var item = Items.Count > 1 ? Items[Items.Count - 2] : Items.Last();
                var cv = item as IDiagramNodeItem;
                if (cv != null)
                {
                    return cv.Name;
                }
                return (string)item;
            }
        }

        public override string SearchTag
        {
            get { return MemberExpression; }
        }

        public override string Name
        {
            get { return VariableName; }
            set { base.Name = value; }
        }

        public string VariableName
        {
            get { return MemberExpression; }
            set { }
        }

        public string AsParameter
        {
            get { return Items.Last().ToString().ToLower(); }
        }

        public bool IsSubVariable { get; set; }

        public ITypeInfo VariableType
        {
            get { return _variableType ?? (_variableType = (Source as ITypeInfo)); }
            set { _variableType = value; }
        }


        public IEnumerable<IContextVariable> GetPropertyDescriptions()
        {
            var sourceNode = VariableType;
            if (sourceNode != null)
            {
                foreach (var item in sourceNode.GetMembers())
                {
                    yield return new ContextVariable(VariableName, item.MemberName)
                    {
                        Source = item as ITypedItem,
                        Repository = Repository,
                        Name = item.MemberName,
                        Node = this.Node,
                        VariableType = item.MemberType
                    };
                }
            }

        }

        public string ShortName
        {
            get { return Items.Last() as string; }
        }

        public string ValueExpression
        {
            get { return VariableName; }

        }

        public ITypedItem Source { get; set; }
        public string[] FirstMembers { get; set; }
    }

    public class HandlerInVariable : ContextVariable
    {
        public HandlerInVariable(params object[] items)
            : base(items)
        {
        }

        public IEnumerable<string> MemberExpressionItems
        {
            get
            {
                if (Input != null)
                {
                    yield return Input.Name;
                }
                if (Component != null)
                {
                    yield return Component.Name;
                }
                else
                {
                    yield return "Item";
                }
                if (Source != null)
                {
                    yield return Source.Name;
                }
                else
                {
                    foreach (var item in Items)
                    {
                        yield return item.ToString();
                    }
                }
            }
        }

        public override string MemberExpression
        {
            get { return string.Join(".", MemberExpressionItems.ToArray()); }
        }

        public HandlerNode HandlerNode { get; set; }
        public HandlerIn Input { get; set; }
        public ComponentNode Component { get; set; }
    }
    public interface IVariableContextProvider : IDiagramNodeItem
    {
        IEnumerable<IContextVariable> GetAllContextVariables();
        IEnumerable<IContextVariable> GetContextVariables();
        IVariableContextProvider Left { get; }
    }
    public interface ICodeOutput : IVariableContextProvider
    {
        void WriteCode(IHandlerNodeVisitor visitor, TemplateContext ctx);
    }

    public interface IVariableNameProvider
    {
        string GetNewVariableName(string prefix);
    }
    public class ActionNode : ActionNodeBase, ICodeOutput, IConnectableProvider
    { 
        public override void Validate(List<ErrorInfo> errors)
        {
            base.Validate(errors);
            if (Meta == null)
            {
                errors.AddError(string.Format("Action {0} was not found.", MetaType), this);
            }
        }

        public override bool AllowMultipleOutputs
        {
            get { return false; }
        }
        public override bool AllowMultipleInputs
        {
            get { return false; }
        }

        public override Color Color
        {
            get { return Color.blue; }
        }


        public override IEnumerable<IGraphItem> GraphItems
        {
            get
            {
                foreach (var item in InputVars)
                    yield return item;
                foreach (var item in OutputVars)
                    yield return item;
            }
        }

        public string VarName
        {
            get { return VariableName; }
        }

        public override void WriteCode(IHandlerNodeVisitor visitor, TemplateContext ctx)
        {
            if (this.Meta == null)
            {
                ctx._comment("Skipping {0}", this.Name);
                return;
            }
            ctx._comment("Visit {0}", this.Meta.FullName);
            var methodInfo = this.Meta.Method;
            if (methodInfo != null)
            {
                var codeMethodReferenceExpression = new CodeMethodReferenceExpression(
                    new CodeSnippetExpression(this.Meta.Type.FullName),
                    methodInfo.Name);
                var genericInputVars = this.GraphItems.OfType<TypeSelection>().Where(p => p.ActionFieldInfo.IsGenericArgument && p.Item != null).Select(p => p.Item.Name).ToArray();
                if (genericInputVars.Length > 0)
                {
                    codeMethodReferenceExpression = new CodeMethodReferenceExpression(
                       new CodeSnippetExpression(this.Meta.Type.FullName),
                       string.Format("{0}<{1}>", methodInfo.Name, string.Join(",", genericInputVars)));

                }
                var _currentActionInvoker =
                    new CodeMethodInvokeExpression(
                        codeMethodReferenceExpression);

                foreach (var input in this.InputVars)
                {
                    if (input.ActionFieldInfo.IsGenericArgument)
                    {

                    }
                    else
                    {
                        _currentActionInvoker.Parameters.Add(
                            new CodeSnippetExpression((input.ActionFieldInfo.Type.IsByRef ? "ref " : string.Empty) + string.Format("{0}", input.VariableName)));
                    }

                }
                ActionOut resultOut = null;
                // The outputs that should be assigned to by the method
                foreach (var @out in this.GraphItems.OfType<ActionOut>())
                {
                    if (@out.ActionFieldInfo != null && @out.ActionFieldInfo.IsReturn)
                    {
                        resultOut = @out;
                        continue;
                    }
                    _currentActionInvoker.Parameters.Add(
                        new CodeSnippetExpression(string.Format("out {0}", @out.VariableName)));
                }
                foreach (var @out in this.GraphItems.OfType<ActionBranch>())
                {
                    _currentActionInvoker.Parameters.Add(
                        new CodeSnippetExpression(string.Format("()=> {{ System.StartCoroutine({0}()); }}", @out.VariableName)));
                }
                ctx._("while (this.DebugInfo(\"{0}\", this) == 1) yield return new WaitForEndOfFrame()", this.Identifier);
                if (resultOut == null)
                {
                    ctx.CurrentStatements.Add(_currentActionInvoker);
                }
                else
                {
                    var assignResult = new CodeAssignStatement(
                        new CodeSnippetExpression(resultOut.VariableName), _currentActionInvoker);
                    ctx.CurrentStatements.Add(assignResult);
                }

            }
            else
            {
                var varStatement = ctx.CurrentDeclaration._private_(this.Meta.Type, this.VarName);
                varStatement.InitExpression = new CodeObjectCreateExpression(this.Meta.Type);

                foreach (var item in this.GraphItems.OfType<ActionIn>())
                {
                    var contextVariable = item.Item;
                    if (contextVariable == null) continue;
                    ctx._("{0}.{1} = {2}", varStatement.Name, item.Name, item.VariableName);
                }


                ctx._("{0}.System = System", varStatement.Name);


                foreach (var item in this.GraphItems.OfType<ActionBranch>())
                {
                    var branchOutput = item.OutputTo<SequenceItemNode>();
                    if (branchOutput == null) continue;
                    ctx._("{0}.{1} = ()=> {{ System.StartCoroutine({2}()); }}", varStatement.Name, item.Name, item.VariableName);
                }
                ctx._("while (this.DebugInfo(\"{0}\", this) == 1) yield return new WaitForEndOfFrame()", this.Identifier);
                ctx._("{0}.Execute()", varStatement.Name);

                WriteActionOutputs(ctx);

            }
        }

        private ActionMetaInfo _meta;
        private string _metaType;
        private IActionIn[] _inputVars;
        private IActionOut[] _outputVars;


        public ActionMetaInfo Meta
        {
            get
            {
                if (string.IsNullOrEmpty(MetaType))
                    return null;
                if (!uFrameECS.Actions.ContainsKey(MetaType))
                {
                    //InvertApplication.LogError(string.Format("{0} action was not found in graph {1}.", MetaType, this.Graph.Name));
                    return null;
                }
                return _meta ?? (_meta = uFrameECS.Actions[MetaType]);
            }
            set
            {
                _meta = value;
                _metaType = value.FullName;
            }
        }

        [JsonProperty]
        public virtual string MetaType
        {
            get { return _metaType; }
            set
            {
                _metaType = value;
            }
        }


        public IActionIn[] InputVars
        {
            get { return _inputVars ?? (_inputVars = GetInputVars().ToArray()); }
            set { _inputVars = value; }
        }

        private IEnumerable<IActionIn> GetInputVars()
        {
            var meta = Meta;
            if (meta != null)
            {

                //// Get the generic contraints
                //foreach (var item in meta.Type.GetGenericArguments())
                //{
                //    //var typeConstraints = item.GetGenericParameterConstraints().Select(p => p.Name);
                //    var variableIn = new TypeSelection();
                //    variableIn.Node = this;
                //    variableIn.Repository = Repository;
                //    variableIn.Identifier = this.Identifier + ":" + item.Name;
                //    variableIn.ActionFieldInfo = new ActionFieldInfo()
                //    {
                //        DisplayType = new FieldDisplayTypeAttribute(item.Name,item.Name,true),
                //        Name = item.Name,
                //        Type = typeof(Type),
                //        MetaAttributes = new ActionAttribute[] { }
                //    };
                //    yield return variableIn;
                //}

                foreach (var item in Meta.ActionFields.Where(p => p.DisplayType is In))
                {
                    IActionIn variableIn;

                    variableIn = item.IsGenericArgument ? (IActionIn)new TypeSelection() : new ActionIn();
                    variableIn.Node = this;
                    variableIn.Repository = Repository;
                    variableIn.ActionFieldInfo = item;
                    variableIn.Identifier = this.Identifier + ":" + meta.Type.Name + ":" + item.Name;
                    yield return variableIn;
                }
            }

        }

        public IActionOut[] OutputVars
        {
            get { return _outputVars ?? (_outputVars = GetOutputVars().ToArray()); }
            set { _outputVars = value; }
        }

        public IEnumerable<IActionOut> GetOutsWithType<TType>()
        {
            return OutputVars.Where(p => p.ActionFieldInfo.Type.IsAssignableFrom(typeof(TType)));
        }

        public IEnumerable<IActionIn> GetInsWithType<TType>()
        {
            return InputVars.Where(p => p.ActionFieldInfo.Type.IsAssignableFrom(typeof(TType)));
        }

        private IEnumerable<IActionOut> GetOutputVars()
        {
            var meta = Meta;
            if (meta != null)
            {
                foreach (var item in Meta.ActionFields.Where(p => p.DisplayType is Out))
                {
                    if (item.Type == typeof(Action))
                    {
                        var variableOut = new ActionBranch()
                        {
                            ActionFieldInfo = item,
                            Node = this,
                            Identifier = this.Identifier + ":" + item.Name,
                            Repository = Repository,

                        };
                        yield return variableOut;
                    }
                    else
                    {
                        var variableOut = new ActionOut()
                        {

                            ActionFieldInfo = item,
                            Node = this,
                            Identifier = this.Identifier + ":" + item.Name,
                            Repository = Repository,
                        };
                        yield return variableOut;
                    }
                }
            }
        }


        public virtual IEnumerable<IConnectable> Connectables
        {
            get
            {
                foreach (var item in InputVars) yield return item;
                foreach (var item in OutputVars) yield return item;
            }
        }

        public Breakpoint BreakPoint
        {
            get { return Repository.All<Breakpoint>().FirstOrDefault(p => p.ForIdentifier == this.Identifier); }
        }


        public void WriteActionOutputs(TemplateContext _)
        {
            foreach (var output in this.GraphItems.OfType<ActionOut>())
            {
                WriteActionOutput(_, output);
            }
        }

        private void WriteActionOutput(TemplateContext _, IActionOut output)
        {
            if (output.ActionFieldInfo != null && output.ActionFieldInfo.IsReturn) return;
            _._("{0} = {1}.{2}", output.VariableName, VarName, output.Name);
            var variableReference = output.OutputTo<IContextVariable>();
            if (variableReference != null)
                _.CurrentStatements.Add(new CodeAssignStatement(new CodeSnippetExpression(variableReference.VariableName),
                    new CodeSnippetExpression(output.VariableName)));
            var actionIn = output.OutputTo<IActionIn>();
            if (actionIn != null)
            {
                _.CurrentStatements.Add(new CodeAssignStatement(
                    new CodeSnippetExpression(actionIn.VariableName),
                    new CodeSnippetExpression(output.VariableName)));
            }
        }
    }
    public class Breakpoint : IDataRecord
    {
        public IRepository Repository { get; set; }
        public string Identifier { get; set; }
        public bool Changed { get; set; }

        [JsonProperty]
        public string ForIdentifier { get; set; }

        public ActionNode Action
        {
            get { return Repository.GetSingle<ActionNode>(ForIdentifier); }
        }
    }
    public partial interface IActionConnectable : IDiagramNodeItem, IConnectable
    {
    }

    public interface IActionItem : IDiagramNodeItem
    {
        ActionFieldInfo ActionFieldInfo { get; set; }
        string VariableName { get; }
        ITypeInfo VariableType { get; }

    }
    public interface IActionIn : IActionItem
    {
        IContextVariable Item { get; }

    }

    public interface IActionOut : IActionItem
    {

    }
    public class CustomAction : SequenceItemNode, IConnectableProvider
    {


        public virtual IEnumerable<IActionIn> GetInputs()
        {
            yield break;
        }
        public virtual IEnumerable<IActionOut> GetOutputs()
        {
            yield break;
        }

        public virtual IEnumerable<ActionBranch> GetBranches()
        {
            yield break;
        }

        public override IEnumerable<IGraphItem> GraphItems
        {
            get
            {
                if (Repository == null)
                    yield break;
                foreach (var item in GetInputs())
                {
                    yield return item;
                } 
                foreach (var item in GetBranches())
                {
                    yield return item;
                }
                foreach (var item in GetOutputs())
                {
                    yield return item;
                }
              
            }
        }

        public virtual IEnumerable<IConnectable> Connectables
        {
            get { return GraphItems.OfType<IConnectable>(); }
        }
    }

    [ActionTitle("Enum Switch"), uFrameCategory("Enums", "Conditions")]
    public class EnumSwitch : CustomAction
    {
        private VariableIn _enumIn;
        private ActionBranch[] _branches;

        public VariableIn EnumIn
        {
            get { return GetSlot(ref _enumIn,"Enum",_=>_.DoesAllowInputs = true); }
        }
        
        public override IEnumerable<IGraphItem> GraphItems
        {
            get
            { 
                if (Repository == null)
                    yield break;

                yield return EnumIn;

                if (EnumIn.Item == null)
                {
                    yield break;
                }
                if (Branches != null)
                {
                    foreach (var item in Branches)
                        yield return item;
                }
                
            }
        }
          
        public override IEnumerable<IConnectable> Connectables
        {
            get
            {
                yield return EnumIn;
                if (_branches != null)
                    foreach (var item in Branches) yield return item;
            }
        } 

        public ActionBranch[] Branches
        {
            get
            {
                return _branches ?? (_branches = EnumIn.Item == null ? null : EnumIn.Item.VariableType.GetMembers()
                          .Select(p => CreateSlot<ActionBranch>(p.MemberName))
                          .ToArray());
            }
            set { _branches = value; }
        }

        public override void WriteCode(IHandlerNodeVisitor visitor, TemplateContext ctx)
        {
            base.WriteCode(visitor, ctx);
        }
    }

    public class ActionIn : SelectionFor<IContextVariable, VariableSelection>, IActionIn
    {
        private string _variableName;

        public override bool CanInputFrom(IConnectable output)
        {
            var outputVariable = output as IContextVariable;
            if (outputVariable != null)
            {
                var varType = outputVariable.VariableType;
                if (varType != null)
                    if (!varType.IsAssignableTo(VariableType))
                    {
                        return false;
                    }
            }
            return true;
        }

        public ActionFieldInfo ActionFieldInfo { get; set; }

        public SequenceItemNode SequenceItem
        {
            get { return this.Node as SequenceItemNode; }
        }
        public string VariableName
        {
            get
            {

                return _variableName ?? (_variableName = SequenceItem.VariableName + "_" + this.Name);
            }
        }

        public ITypeInfo VariableType
        {
            get
            {
                if (ActionFieldInfo != null)
                {
                    return new SystemTypeInfo(ActionFieldInfo.Type);
                }
                return new SystemTypeInfo(typeof(object));
                //var item = Item;
                //if (item == null)
                //    return "object";
                //return Item.VariableType;
            }
        }

        public override string Name
        {
            get { return ActionFieldInfo.Name; }
            set { base.Name = value; }
        }

        public override IEnumerable<IDataRecord> GetAllowed()
        {
            var action = this.Node as IVariableContextProvider;
            if (action != null)
            {

                foreach (var item in action.GetAllContextVariables().Where(p => p.VariableType.IsAssignableTo(VariableType)))
                    yield return item;
            }
            else
            {
                InvertApplication.Log("BS");
            }
        }
    }
    public class PropertyIn : SelectionFor<IContextVariable, VariableSelection>, IActionIn
    {
        public bool DoesAllowInputs;
        public override bool AllowInputs
        {
            get { return DoesAllowInputs; }

        }

        public override string SelectedDisplayName
        {
            get { return base.SelectedDisplayName; }
        }

        public override IContextVariable Item
        {
            get { return base.Item; }
        }

        public IVariableContextProvider Handler
        {
            get { return Node.Filter as IVariableContextProvider; }
        }

        public ActionFieldInfo ActionFieldInfo { get; set; }

        public string VariableName
        {
            get
            {
                var item = Item;
                if (item == null)
                {
                    return "...";
                }
                return item.VariableName;
            }
        }

        public virtual ITypeInfo VariableType
        {
            get
            {
                var item = Item;
                if (item == null)
                    return new SystemTypeInfo(typeof(object));
                return Item.VariableType;
            }
        }

        //public override string Name
        //{
        //    get { return "Property"; }
        //    set { base.Name = value; }
        //}

        public override IEnumerable<IDataRecord> GetAllowed()
        {
            var action = this.Node as IVariableContextProvider;
            if (action != null)
            {
                foreach (var item in action.GetAllContextVariables())
                {
                    if (item.Source is PropertiesChildItem)
                    {
                        yield return item;
                    }
                }
            }
            else
            {
                var hn = Handler;
                if (hn != null)
                {
                    foreach (var item in hn.GetAllContextVariables())
                    {
                        yield return item;
                    }
                }
            }

        }
    }
    public class VariableIn : SelectionFor<IContextVariable, VariableSelection>, IActionIn
    {
        public bool DoesAllowInputs;
        public override bool AllowInputs
        {
            get { return DoesAllowInputs; }

        }

        public override IContextVariable Item
        {
            get { return base.Item; } 
        }

        public IVariableContextProvider Handler
        {
            get { return Node.Filter as IVariableContextProvider; }
        }

        public ActionFieldInfo ActionFieldInfo { get; set; }
        public override string ItemDisplayName(IContextVariable item)
        {
            return base.ItemDisplayName(item);
        }

        public override string SelectedDisplayName
        {
            get
            {

                return base.SelectedDisplayName;
            }
        }

        public string VariableName
        {
            get
            {

                var item = Item;
                if (item == null)
                {
                    return "...";
                }
                return item.VariableName;
            }
        }

        public virtual ITypeInfo VariableType
        {
            get
            {

                var item = Item;
                if (item == null)
                    return new SystemTypeInfo(typeof(object));
                return Item.VariableType;
            }
        }

        public override IEnumerable<IDataRecord> GetAllowed()
        {
            //var action = this.Node as IVariableContextProvider;
            //if (action != null)
            //{
            //    foreach (var item in action.GetContextVariables())
            //    {
            //        yield return item;
            //    }
            //}
            //else
            //{
            var hn = Handler;
            if (hn != null)
            {
                foreach (var item in hn.GetContextVariables())
                {
                    yield return item;
                }
            }
            //}

        }
    }
    public class GroupSelection : InputSelectionValue
    {

    }
    public class VariableSelection : InputSelectionValue
    {

    }

    public class ActionOut : MultiOutputSlot<IContextVariable>, IActionOut, IContextVariable
    {
        private string _variableName;
        private ITypeInfo _variableType;
        public SequenceItemNode SequenceItem
        {
            get { return this.Node as SequenceItemNode; }
        }
        public override bool AllowMultipleOutputs
        {
            get { return true; }
        }

        public override bool CanOutputTo(IConnectable input)
        {

            return true;
            return base.CanOutputTo(input);
        }

        public ActionFieldInfo ActionFieldInfo { get; set; }
        public override string Name
        {
            get
            {
                if (ActionFieldInfo == null) return base.Name;
                return ActionFieldInfo.Name;
            }
            set { base.Name = value; }
        }
        public string VariableName
        {
            get
            {
                return _variableName ?? (_variableName = SequenceItem.VariableName + "_" + this.Name);
            }
        }
        public string ShortName
        {
            get { return VariableName; }

        }

        public string ValueExpression
        {
            get { return VariableName; }
        }

        public ITypedItem Source { get; set; }

        public string AsParameter
        {
            get { return Name.ToLower(); }
        }

        public bool IsSubVariable { get; set; }

        public ITypeInfo VariableType
        {
            get
            {
                if (_variableType != null)
                    return _variableType;
                if (ActionFieldInfo.Type.IsGenericParameter)
                {
                    var typeSelection = this.Node.GraphItems.OfType<TypeSelection>()
                        .FirstOrDefault(
                            p =>
                                p.ActionFieldInfo.IsGenericArgument &&
                                p.ActionFieldInfo.Name == ActionFieldInfo.Type.Name);
                    if (typeSelection != null)
                    {
                        var item = typeSelection.Item as ITypeInfo;
                        if (item != null)
                        {
                            return _variableType = item;
                        }
                    }
                }
                return _variableType = new SystemTypeInfo(ActionFieldInfo.Type);

            }
            set { _variableType = value; }
        }

        public ActionNode ActionNode
        {
            get { return this.Node as ActionNode; }
        }
        public IEnumerable<IContextVariable> GetPropertyDescriptions()
        {

            return VariableType.GetPropertyDescriptions(this);
        }
    }

    public static class EcsReflectionExtensions
    {
        public static IEnumerable<IContextVariable> GetPropertyDescriptions(this ITypeInfo type, IContextVariable parent)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            foreach (var item in type.GetMembers())
            {
                yield return new ContextVariable(parent.VariableName, item.MemberName)
                {
                    Repository = parent.Repository,
                    Node = parent.Node,
                    VariableType = item.MemberType
                };
            }
        }
    }
    public class ActionBranch : SingleOutputSlot<ActionNode>, IActionOut, IVariableContextProvider
    {
        private string _varName;

        public override Color Color
        {
            get { return Color.blue; }
        }
        public SequenceItemNode SequenceItem
        {
            get { return this.Node as SequenceItemNode; }
        }
        public string VariableName
        {
            get
            {
                return _varName ?? (_varName = SequenceItem.VariableName + "_" + this.Name);
            }
        }
        public ActionFieldInfo ActionFieldInfo { get; set; }
        public override string Name
        {
            get
            {
                if (ActionFieldInfo != null) return ActionFieldInfo.Name;
                return base.Name;
            }
            set { base.Name = value; }
        }

        public ITypeInfo VariableType { get; set; }

        public IEnumerable<IContextVariable> GetAllContextVariables()
        {
            if (Left == null)
            {
                yield break;
            }
            foreach (var item in Left.GetAllContextVariables())
                yield return item;
        }

        public IEnumerable<IContextVariable> GetContextVariables()
        {
            yield break;
        }

        public IVariableContextProvider Left
        {
            get { return this.Node as SequenceItemNode; }
        }
    }




}
