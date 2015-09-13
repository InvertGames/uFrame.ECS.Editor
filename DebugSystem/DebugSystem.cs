using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Invert.Common.UI;
using Invert.Data;
using Invert.Json;
using Invert.uFrame.ECS;
using uFrame.Kernel;
using UnityEditor;
using UnityEngine;

namespace Invert.Core.GraphDesigner
{
    public class DebugEvent : Command
    {
        public string ActionId { get; set; }
        public object[] Variables { get; set; }
        public int Result { get; set; }
    }
    public interface IBreakpointHit
    {
        void BreakpointHit();
    }
    public class DebugSystem : DiagramPlugin, 
        IExecuteCommand<DebugEvent>,
        IExecuteCommand<ContinueCommand>,
        IExecuteCommand<StepCommand>,
        IExecuteCommand<ToggleBreakpointCommand>,
        IContextMenuQuery,
        IToolbarQuery,
        IDrawInspector
    {
        private Dictionary<string, Breakpoint> _breakpoints;

        public Dictionary<string, Breakpoint> Breakpoints
        {
            get { return _breakpoints ?? (Container.Resolve<IRepository>().All<Breakpoint>().ToDictionary(p => p.ForIdentifier)); }
            set { _breakpoints = value; }
        }

        public void Execute(ToggleBreakpointCommand command)
        {
            if (command.Action.BreakPoint == null)
                {
                    var breakPoint =Container.Resolve<IRepository>().Create<Breakpoint>();
                    breakPoint.ForIdentifier = command.Action.Identifier;
                }
                else
                {
                    Container.Resolve<IRepository>().Remove(command.Action.BreakPoint);
                }
            _breakpoints = null;
        }

        public void QueryContextMenu(ContextMenuUI ui, MouseEvent evt, object obj)
        {
            var actionVM = obj as ActionNodeViewModel;
            if (actionVM != null)
            {
                ui.AddCommand(new ContextMenuItem()
                {
                    Title = "Breakpoint",
                    Checked = actionVM.Action.BreakPoint != null,
                    Command = new ToggleBreakpointCommand()
                    {
                        Action = actionVM.Action,
                        
                    }
                });
            }
        }

        public ActionNode CurrentBreakpoint { get; set; }
        public bool ShouldStep { get; set; }
        public void Execute(DebugEvent command)
        {
            LastDebugEvent = command;
            if (CurrentBreakpoint != null && CurrentBreakpoint.Identifier == command.ActionId)
            {
                if (ShouldContinue == true)
                {
                    command.Result = 0;
                    CurrentBreakpoint.IsSelected = false;
                    ShouldContinue = false;
                    CurrentBreakpoint = null;
                    return;
                }
                command.Result = 1;
                return;
            }

            if (Breakpoints.ContainsKey(command.ActionId))
            {
                command.Result = 1;
                Signal<IBreakpointHit>(_ => _.BreakpointHit());
                CurrentBreakpoint = Breakpoints[command.ActionId].Action;
                Execute(new NavigateToNodeCommand()
                {
                    Node = CurrentBreakpoint
                });

            }
            else if (ShouldStep)
            {
                CurrentBreakpoint = Container.Resolve<IRepository>().GetById<ActionNode>(command.ActionId);
                command.Result = 1;
                Execute(new NavigateToNodeCommand()
                {
                    Node = CurrentBreakpoint
                });
                ShouldStep = false;
            }
        }

        public DebugEvent LastDebugEvent { get; set; }

        public void QueryToolbarCommands(ToolbarUI ui)
        {
            if (CurrentBreakpoint != null)
            {
                ui.AddCommand(new ToolbarItem()
                {
                    Command = new ContinueCommand(),
                    Title = "Continue"
                });
                ui.AddCommand(new ToolbarItem()
                {
                    Command = new StepCommand(),
                    Title = "Step"
                });
            }
            
        }

        public bool ShouldContinue;
        public void Execute(ContinueCommand command)
        {
            ShouldContinue = true;
        }

        public void Execute(StepCommand command)
        {
            ShouldContinue = true;
            ShouldStep = true;
        }

        public void DrawInspector(Rect rect)
        {
            if (LastDebugEvent != null)
            {
                foreach (var obj in LastDebugEvent.Variables)
                {
                    if (GUIHelpers.DoToolbarEx(obj.GetType().ToString()))
                    {
                        var properties = obj.GetType().GetFields(BindingFlags.Default | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                        foreach (var property in properties)
                        {
                            var val = property.GetValue(obj);
                            if (val != null)
                            {
                                EditorGUILayout.LabelField(property.Name, val.ToString());
                            }
                        }
                    }
                   
                }
            }
        }
    }

    public class ToggleBreakpointCommand : Command
    {
        public ActionNode Action { get; set; }
    }
  
}
