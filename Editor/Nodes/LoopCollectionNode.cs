using System.CodeDom;
using uFrame.Attributes;

namespace Invert.uFrame.ECS
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;

    public class DynamicTypeInfo : ITypeInfo
    {
        public static SystemTypeInfo ObjectInfo = new SystemTypeInfo(typeof(object));

        public Func<ITypeInfo> GetInfo;

        protected ITypeInfo Info
        {
            get { return GetInfo() ?? ObjectInfo; }
        }

        public IEnumerable<IMemberInfo> GetMembers()
        {
            return Info.GetMembers();
        }

        public bool IsAssignableTo(ITypeInfo info)
        {
            return Info.IsAssignableTo(info);
        }

        public bool IsArray { get { return Info.IsArray; } }

        public bool IsList
        {
            get { return Info.IsList; }
        }

        public bool IsEnum
        {
            get { return Info.IsEnum; }
        }

        public ITypeInfo InnerType
        {
            get { return Info.InnerType; }
        }

        public string TypeName
        {
            get { return Info.TypeName; }
        }

        public string FullName { get { return Info.FullName; } }
        public string Namespace { get { return Info.Namespace; } }
        public string Title { get { return Info.TypeName; } }
        public string Group { get { return Info.Namespace; } }
        public string SearchTag { get { return FullName; } }
        public string Description { get; set; }
        public string Identifier { get { return FullName; } set { } }
    }
    public class LoopCollectionNode : LoopCollectionNodeBase, IConnectableProvider
    {

        private VariableIn _list;
        private ActionBranch _next;
        private ActionOut _item;
        public override IEnumerable<IContextVariable> GetContextVariables()
        {
            yield return Item;
        }

        public override string Name
        {
            get { return "Loop Collection"; }
            set { base.Name = value; }
        }

        public VariableIn List
        {
            get
            {
                return GetSlot(ref _list, "List", _ =>
                    {
                        _.DoesAllowInputs = true;

                    });
            }
        }

        public ActionBranch Next
        {
            get { return GetSlot(ref _next, "Next"); }
        }

        public ActionOut Item
        {
            get
            {
                return GetSlot(ref _item, "Item", _ =>
                {
                    _.VariableType = new DynamicTypeInfo()
                    {
                        GetInfo = () =>
                        {
                            return List.Item == null || List.Item.VariableType == null ? null : List.Item.VariableType.InnerType;
                        }
                    };
                });
            }
        }

        public override void WriteCode(IHandlerNodeVisitor visitor, TemplateContext ctx)
        {
            base.WriteCode(visitor, ctx);

            var loop = new CodeIterationStatement(
                new CodeSnippetStatement(string.Format("var {0}Index = 0", Item.VariableName)),
                new CodeSnippetExpression(string.Format("{0}Index < {1}.Count", Item.VariableName, List.VariableName)),
                new CodeSnippetStatement(string.Format("{0}Index++", Item.VariableName))
                );

            loop.Statements._("{0} = {1}[{0}Index]", Item.VariableName, List.VariableName);
            ctx.PushStatements(loop.Statements);
            Next.WriteInvoke(ctx);
            ctx.PopStatements();
            ctx.CurrentStatements.Add(loop);
        }

        public override IEnumerable<IGraphItem> GraphItems
        {
            get
            {
                yield return List;
                if (List.Item != null)
                {
                    yield return Next;
                    yield return Item;
                }
            }
        }

        public IEnumerable<IConnectable> Connectables
        {
            get
            {
                if (Repository == null)
                {
                    yield break;
                }
                yield return List;
                yield return Item;

            }
        }
    }

    [ActionTitle("Loop Group Items"), uFrameCategory("Loops")]
    public class LoopGroupNode : CustomAction
    {

        private TypeSelection _list;
        private ActionBranch _next;
        private ActionOut _item;

        [In]
        public TypeSelection List
        {
            get
            {
                return GetSlot(ref _list, "Group", _ =>
                {
                    _.Filter = info => info is ComponentNode || info is GroupNode;
                    
                });
            }
        }

        [Out]
        public ActionBranch Next
        {
            get { return GetSlot(ref _next, "Next"); }
        }

        [In]
        public ActionOut Item
        {
            get
            {
                return GetSlot(ref _item, "Item", _ =>
                {
                    _.VariableType = new DynamicTypeInfo()
                    {
                        GetInfo = () =>
                        {
                            return List.Item;
                        }
                    };
                });
            }
        }

        public override void WriteCode(IHandlerNodeVisitor visitor, TemplateContext ctx)
        {
            base.WriteCode(visitor, ctx);
            ctx._("var {0}Components = System.ComponentSystem.RegisterComponent<{1}>().Components",List.VariableName, Item.VariableType.FullName);

            var loop = new CodeIterationStatement(
                new CodeSnippetStatement(string.Format("var {0}Index = 0", Item.VariableName)),
                new CodeSnippetExpression(string.Format("{0}Index < {1}Components.Count", Item.VariableName, List.VariableName)),
                new CodeSnippetStatement(string.Format("{0}Index++", Item.VariableName))
                );

            loop.Statements._("{0} = {1}Components[{0}Index]", Item.VariableName, List.VariableName);
            ctx.PushStatements(loop.Statements);
            Next.WriteInvoke(ctx);
            ctx.PopStatements();
            ctx.CurrentStatements.Add(loop);
        }

  
    }

    public class ListAction : CustomAction
    {

        private VariableIn _list;
        [In]
        public virtual VariableIn List
        {
            get
            {
                return GetSlot(ref _list, "List", _ =>
                {
                    _.DoesAllowInputs = true;

                });
            }
        }


        public override void Validate(List<ErrorInfo> errors)
        {
            base.Validate(errors);
            if (List.Item == null)
            {
                errors.AddError("List is required.", this);
            }
        }

    }

    public class ListActionWithItem : ListAction
    {
        [In]
        public override VariableIn List
        {
            get { return base.List; }
        }
        private VariableIn _item;

        [In]
        public VariableIn Item
        {
            get
            {
                return GetSlot(ref _item, "Item", _ =>
                {
                    _.DoesAllowInputs = true;
                });
            }
        }

        public override void Validate(List<ErrorInfo> errors)
        {
            base.Validate(errors);
            if (Item.Item == null)
            {
                errors.AddError("Item is required.", this);
            }
        }
    }
    [ActionTitle("Add To List"), uFrameCategory("Lists", "Collections")]
    public class ListAdd : ListActionWithItem
    {

        public override void WriteCode(IHandlerNodeVisitor visitor, TemplateContext ctx)
        {
            base.WriteCode(visitor, ctx);
            ctx._("{0}.Add({1})", List.VariableName, Item.VariableName);
        }

    }
    [ActionTitle("Remove From List"), uFrameCategory("Lists", "Collections")]
    public class ListRemove : ListActionWithItem
    {
        public override void WriteCode(IHandlerNodeVisitor visitor, TemplateContext ctx)
        {
            base.WriteCode(visitor, ctx);
            ctx._("{0}.Remove({1})", List.VariableName, Item.VariableName);
        }
    }

    [ActionTitle("Get List Item"), uFrameCategory("Lists", "Collections")]
    public class GetListItem : ListAction
    {
        private VariableIn _indexVariable;
        private VariableOut _result;

        [In]
        public VariableIn IndexVariable
        {
            get { return GetSlot(ref _indexVariable, "Index"); }
        }

        [Out]
        public VariableOut Result
        {
            get
            {
                return GetSlot(ref _result, "Result", _ =>
                {
                    _.VariableType = new DynamicTypeInfo()
                    {
                        GetInfo = () => List.Item == null || List.Item.VariableType == null ? null : List.Item.VariableType.InnerType
                    };
                });
            }
        }

        public override void WriteCode(IHandlerNodeVisitor visitor, TemplateContext ctx)
        {
            base.WriteCode(visitor, ctx);
            ctx._("{0} = {1}[{2}]", Result.VariableName, List.VariableName, IndexVariable.VariableName);
        }
    }

    public partial interface ILoopCollectionConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable
    {
    }

    [ActionTitle("Get Random List Item"), uFrameCategory("Lists", "Collections")]
    public class GetRandomListItem : ListAction
    {
        private VariableOut _result;

        [Out]
        public VariableOut Result
        {
            get
            {
                return GetSlot(ref _result, "Result", _ =>
                {
                    _.VariableType = new DynamicTypeInfo()
                    {
                        GetInfo = () => List.Item == null || List.Item.VariableType == null ? null : List.Item.VariableType.InnerType
                    };
                });
            }
        }

        public override void WriteCode(IHandlerNodeVisitor visitor, TemplateContext ctx)
        {
            base.WriteCode(visitor, ctx);

            ctx._("{0} = {1}[UnityEngine.Random.Range(0, {1}.Count)]", Result.VariableName, List.VariableName);
        }
    }
}
