using System;
using System.CodeDom;
using System.Collections;
using System.Linq;
using Invert.Core.GraphDesigner;
using Invert.uFrame.ECS;
using uFrame.Actions;
using uFrame.Attributes;
using uFrame.ECS;
using uFrame.Kernel;


namespace Invert.uFrame.ECS.Templates
{
    [ForceBaseType(typeof(EcsSystem))]
    [RequiresNamespace("uFrame.ECS")]
    [RequiresNamespace("UniRx")]
    public partial class SystemTemplate
    {

        //[ForEach("FilterNodes"), GenerateProperty, WithField]
        //public _CONTEXT_ _Name_Context { get; set; }

        [GenerateMethod(TemplateLocation.Both), AsOverride, InsideAll]
        public void Setup()
        {
            Ctx.CurrentMethod.invoke_base();
            if (!Ctx.IsDesignerFile) return;
            foreach (var item in Groups)
            {
                Ctx._("{0}Manager = ComponentSystem.RegisterGroup<{0}Group,{0}>()", item.Name);
            }
            foreach (var item in Components)
            {
                Ctx._("{0}Manager = ComponentSystem.RegisterComponent<{0}>()", item.Name);
            }
            foreach (var item in Ctx.Data.FilterNodes.OfType<ISetupCodeWriter>())
            {
                item.WriteSetupCode(new HandlerCsharpVisitor() {_=Ctx}, Ctx);
            }
        }
        
        [ForEach("Components"), GenerateProperty, WithField]
        public IEcsComponentManagerOf<_ITEMTYPE_> _Name_Manager { get; set; }

        [ForEach("Groups"), GenerateProperty, WithField]
        public IEcsComponentManagerOf<_ITEMTYPE_> _GroupName_Manager { get; set; }
    }


    public partial class CustomActionEditableTemplate
    {
    
    }

    public partial class CustomActionDesignerTemplate
    {
        
    }
    //public IObservable<_ITEMTYPE_> _Name_Observable
    //{
    //    get
    //    {
    //        // return _MaxNavigatorsObservable ?? (_MaxNavigatorsObservable = new Subject<int>());
    //    }
    //}
    //public virtual Int32 MaxNavigators
    //{
    //    get
    //    {
    //        return _MaxNavigators;
    //    }
    //    set
    //    {
    //        _MaxNavigators = value;
    //        if (_MaxNavigatorsObservable != null)
    //        {
    //            _MaxNavigatorsObservable.OnNext(value);
    //        }
    //    }
    //}

    //[RequiresNamespace("uFrame.ECS")]
    //public partial class OnEventTemplate
    //{


    //}

    //[RequiresNamespace("uFrame.ECS")]
    //public partial class PublishTemplate
    //{


    //}
}

