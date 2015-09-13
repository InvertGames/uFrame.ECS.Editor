using UnityEngine;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class ActionNodeDrawer : GenericNodeDrawer<ActionNode,ActionNodeViewModel> {
        
        public ActionNodeDrawer(ActionNodeViewModel viewModel) : 
                base(viewModel) {
        }

        public override Vector2 HeaderPadding
        {
            get { return Vector2.zero; }
        }
        public override float MinWidth
        {
            get { return 100f; }
        }

        public override void Draw(IPlatformDrawer platform, float scale)
        {
            base.Draw(platform, scale);
            if (NodeViewModel.IsBreakpoint)
            {
                var adjustedBounds = new Rect(Bounds.x - 9, Bounds.y + 1, Bounds.width + 19, Bounds.height + 9);

                platform.DrawStretchBox(adjustedBounds, CachedStyles.BoxHighlighter6, 20);
            }
           
        }

        //public override bool ShowHeader
        //{
        //    get
        //    {
        //        if (NodeViewModel.Action.Meta.Method == null)
        //            return false;
        //        return true;
        //    }
        //}
    }
}
