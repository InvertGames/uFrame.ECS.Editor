using System.CodeDom;

namespace Invert.uFrame.ECS {
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
            get { return GetSlot(ref _list, "List", _ =>
            {
                _.DoesAllowInputs = true;
                
            }); }
        }
      
        public ActionBranch Next
        {
            get { return GetSlot(ref _next, "Next"); }
        }

        public ActionOut Item
        {
            get { 
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
                new CodeSnippetStatement(string.Format("var {0}Index = 0",  Item.VariableName)),
                new CodeSnippetExpression(string.Format("{0}Index < {1}.Count", Item.VariableName, List.VariableName)),
                new CodeSnippetStatement(string.Format("{0}Index++", Item.VariableName))
                );

            loop.Statements._("{0} = {1}[{0}Index]", Item.VariableName, List.VariableName);
            loop.Statements._("System.StartCoroutine({0}())", Next.VariableName);
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



    public partial interface ILoopCollectionConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
