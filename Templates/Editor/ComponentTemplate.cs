using System.Collections.Generic;
using Invert.Core.GraphDesigner;
using UniRx;
using UnityEngine;

namespace Invert.uFrame.ECS.Templates
{
    [RequiresNamespace("uFrame.ECS")]
    [RequiresNamespace("UniRx")]
    [NamespacesFromItems]
    public partial class ComponentTemplate
    {

        public static int _ComponentIds = 1;
        [GenerateProperty]
        public int ComponentID
        {
            get
            {
                Ctx._("return {0}", _ComponentIds++);
                return 0;
            }
        }

        [ForEach("Properties"), GenerateProperty, WithLazyField(typeof(Subject<_ITEMTYPE_>), true)]
        public IObservable<_ITEMTYPE_> _Name_Observable { get { return null; } }

        [ForEach("Properties"), GenerateProperty, WithName, WithField(null, typeof(SerializeField),ManualSetter = true)]
        public _ITEMTYPE_ Property
        {
            get { return null; }
            set
            {
                Ctx._("_{0} = value", Ctx.Item.Name);
                Ctx._if("_{0}Observable != null", Ctx.Item.Name).TrueStatements
                    ._("_{0}Observable.OnNext(value)", Ctx.Item.Name);
                
            }
        }

        [ForEach("Collections"), GenerateProperty, WithName, WithLazyField(null,typeof(SerializeField))]
        public List<_ITEMTYPE_> Collection { get; set; }
    }
}