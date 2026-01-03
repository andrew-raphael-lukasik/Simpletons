#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using Unity.Mathematics;

using Random = Unity.Mathematics.Random;

namespace Simpleton
{
    public class SimpletonInspectorWindow : EditorWindow
    {
#region fields


        SimpletonStateMachine _stateMachine;
        Dictionary<SimpletonState, SimpletonInspectorNode> _lookupNodes = new();


#endregion
#region create window


        [MenuItem("Window/"+nameof(Simpleton)+"/"+nameof(SimpletonInspectorWindow))]
        public static void CreateWindow()
            => CreateWindow(null);

        public static void CreateWindow(
            SimpletonStateMachine stateMachine
        )
        {
            var window = EditorWindow.GetWindow<SimpletonInspectorWindow>();
            window.titleContent = new GUIContent(nameof(SimpletonInspectorWindow));
            window._stateMachine = stateMachine;
            window.Show();
        }


#endregion
#region unity messages


        void CreateGUI()
        {
            SimpletonState[] states;
            SimpletonInspectorNode[] nodes;
            Edge[] edges;
            string[] logs;

            if (_stateMachine!=null)
                GetGraphData(_stateMachine, out states, out nodes, out edges, out logs);
            else
                GetFakeGraphData(out states, out nodes, out edges, out logs);

            {
                int numNodes = nodes.Length;
                int numStates = states.Length;
                int len = Mathf.Min(numNodes, numStates);

                _lookupNodes.Clear();
                for (int i=0 ; i<len ; i++)
                    _lookupNodes.Add(states[i], nodes[i]);
            }

            rootVisualElement.Clear();

            var mainView = new VisualElement();
            {
                var style = mainView.style;
                style.flexGrow = 1;
                style.flexDirection = FlexDirection.RowReverse;
            }
            rootVisualElement.Add(mainView);

            var graphContainer = new VisualElement();
            {
                var style = graphContainer.style;
                style.flexGrow = 1;
                // style.minHeight = style.minWidth = 300f;
            }
            mainView.Add(graphContainer);

            var graphView = new SimpletonInspectorGraphView();
            {
                //graphView.StretchToParentSize();
                var style = graphView.style;
                style.flexGrow = 1;
            }
            {
                foreach (var node in nodes)
                    graphView.AddElement(node);

                foreach (var edge in edges)
                    graphView.Add(edge);
                    // Debug.Log($"Edge <b>{edge.name}</b> connects <b>{edge.output.portName}</b> and <b>{edge.input.portName}</b> ports");

                foreach (var node in nodes)
                {
                    node.RefreshExpandedState();
                    node.RefreshPorts();
                }
            }
            graphContainer.Add(graphView);

            var logsView = new ListView();
            {
                var style = logsView.style;
                // style.flexGrow = 1;
                // style.minHeight = logsView.itemHeight * 12;
                style.minWidth = 200;
                style.maxWidth = new StyleLength(new Length(50,LengthUnit.Percent));
                // style.position = Position.Absolute;
                // style.left = 10;
                // style.top = 10;
                style.borderTopWidth = style.borderLeftWidth = style.borderRightWidth = style.borderBottomWidth = 1;
                style.borderTopColor = style.borderLeftColor = style.borderRightColor = style.borderBottomColor = new Color{ r=0.1f, g=0.1f, b=0.1f, a=1 };;
            }
            //logsView.headerTitle = $"Last Log Messages | Capacity:";
            //logsView.showFoldoutHeader = true;
            logsView.itemsSource = logs;
            logsView.fixedItemHeight = 16;
            logsView.makeItem =()=> new Label();
            logsView.bindItem = (ve,i) => {
                var label = (ve as Label);
                label.text = (string) logsView.itemsSource[i];
                label.style.backgroundColor = !string.IsNullOrEmpty(label.text) ? Color.HSVToRGB(H:new Random((uint)Mathf.Abs(label.text.GetHashCode())).NextFloat(), S:0.5f, V:0.4f) : new Color{ r=0.2f, g=0.2f, b=0.2f, a=1 };
            };
            logsView.selectionType = SelectionType.Single;
            mainView.Add(logsView);

            var toolbar = new Toolbar();
            {
                var refresh = new ToolbarButton(CreateGUI);
                refresh.text = "Refresh ↻";
                toolbar.Add(refresh);
            }
            rootVisualElement.Add(toolbar);
        }

        void Update()
        {
            foreach (var (state, node) in _lookupNodes)
            {
                var style = node.style;
                if (state!=_stateMachine.Current)
                {
                    style.borderTopWidth = style.borderBottomWidth = 0;
                }
                else
                {
                    style.borderTopWidth = style.borderBottomWidth = 2;
                    style.borderTopLeftRadius = style.borderTopRightRadius = 6;
                    style.borderTopColor = style.borderBottomColor = Color.yellow;
                    style.borderBottomLeftRadius = style.borderBottomRightRadius= 12;
                }
            }
        }


#endregion
#region private methods


        static void GetFakeGraphData(
            out SimpletonState[] states,
            out SimpletonInspectorNode[] nodes,
            out Edge[] edges,
            out string[] logs
        )
        {
            var node1 = SimpletonInspectorNode.Factory("node 1", out var ports1, (Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(SimpletonInspectorNode), "input port"));
            node1.SetPositionCircle(math.PI*2f * 0f/2f);
            var node2 = SimpletonInspectorNode.Factory("node 2", out var ports2, (Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(SimpletonInspectorNode), "output port"));
            node2.SetPositionCircle(math.PI*2f * 1f/2f);
            nodes = new SimpletonInspectorNode[]{node1, node2};

            states = new SimpletonState[]{};
            edges = new Edge[0]{};
            logs = new string[0];
        }

        static void GetGraphData(
            SimpletonStateMachine stateMachine,
            out SimpletonState[] states,
            out SimpletonInspectorNode[] nodes,
            out Edge[] edges,
            out string[] logs
        )
        {
            logs = stateMachine.DebugLogs.ToArray();
            var stateSet = stateMachine.FindAllStates();
            states = new SimpletonState[ stateSet.Count ];
            nodes = new SimpletonInspectorNode[ stateSet.Count ];;
            var stateNodePorts = new Dictionary<SimpletonState,(SimpletonInspectorNode node,Port[] ports)>();
            {
                int i = 0;
                foreach (var state in stateSet)
                {
                    SimpletonInspectorNode.Factory(state, out var node, out var ports);
                    node.SetPositionCircle(math.PI*2f * ((float)i/(float)stateSet.Count));
                    states[i] = state;
                    nodes[i] = node;
                    stateNodePorts.Add(state, (node, ports));
                    i++;
                }
            }

            // foreach (var kv in stateNodePorts)
            //     Debug.Log($"State <b>{kv.Key.name}</b> created <b>{kv.Value.node.title}</b> node with {kv.Value.ports.Length} ports");

            var edgeList = new List<Edge>(capacity: nodes.Length * 2);
            foreach (var (srcState, srcNodePorts) in stateNodePorts)
            for (int i=0 ; i<srcState.transitions.Length ; i++)
            {
                var srcNodeOutput = srcNodePorts.ports[1+i];
                var srcNodeTransition = srcState.transitions[i];

                var dstState = srcNodeTransition.destination;
                var dstNodeInput = stateNodePorts[dstState].ports[0];

                // Debug.Log($"State <b>{srcState.name}</b> transitions to <b>{dstState.name}</b> via <b>{srcNodeTransition.name}</b> transition");
            
                var edge = srcNodeOutput.ConnectTo(dstNodeInput);
                edge.name = srcNodeTransition.label;
                edgeList.Add(edge);

                // Debug.Log($"\tEdge <b>{edge.name}</b> will connect <b>{edge.output.node.title}.{edge.output.portName}</b> and <b>{edge.input.node.title}.{edge.input.portName}</b> ports");
            }
            edges = edgeList.ToArray();
        }


#endregion
    }
#region other types


    public class SimpletonInspectorGraphView : GraphView
    {
        public SimpletonInspectorGraphView()
        {
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());
            this.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        }
        public override List<Port> GetCompatiblePorts(
            Port startPort,
            NodeAdapter nodeAdapter
        )
        {
            var compatiblePorts = new List<Port>();
            foreach (var port in ports)
                if (startPort!=port && startPort.node!=port.node)
                    compatiblePorts.Add(port);
            return compatiblePorts;
        }
    }

    public class SimpletonInspectorNode : Node
    {
        public static SimpletonInspectorNode Factory(
            string node_title,
            out Port[] ports,
            params (Orientation orientation, Direction direction, Port.Capacity capacity, System.Type type, string label)[] port_templates
        )
        {
            var node = new SimpletonInspectorNode{
                title    = node_title,
            };
            {
                ports = new Port[ port_templates.Length ];
                for (int i=0 ; i<port_templates.Length ; i++)
                {
                    var IN = port_templates[i];
                    var port = node.InstantiatePort(IN.orientation, IN.direction, IN.capacity, IN.type);
                    port.portName = IN.label;
                    if (IN.direction==Direction.Output)
                        port.portColor = Color.HSVToRGB(H:new Random((uint)Mathf.Abs(IN.label.GetHashCode())).NextFloat(), S:0.5f, V:1);
                    node.outputContainer.Add(port);
                    ports[i] = port;
                }
            }
            return node;
        }
        public static SimpletonInspectorNode Factory(
            string node_title,
            out Port[] ports,
            params (bool vertical, bool output, bool multi, System.Type type, string label)[] port_templates
        )
        {
            var arr = new (Orientation orientation, Direction direction, Port.Capacity capacity, System.Type type, string label)[ port_templates.Length ];
            if (port_templates.Length!=0)
            for (int i=0 ; i<port_templates.Length ; i++)
            {
                var next = arr[i];
                var template = port_templates[i];
                next.orientation    = template.vertical ? Orientation.Vertical  : Orientation.Horizontal;
                next.direction      = template.output   ? Direction.Output      : Direction.Input;
                next.capacity       = template.multi    ? Port.Capacity.Multi   : Port.Capacity.Single;
                next.type           = template.type;
                next.label      = template.label;
            }
            return Factory(node_title, out ports, arr);
        }
        public static void Factory(SimpletonState state, out SimpletonInspectorNode node, out Port[] ports)
        {
            var type = typeof(SimpletonStateTransition);
            var portTemplates = new (Orientation,Direction,Port.Capacity,System.Type,string)[ state.transitions.Length + 1 ];
            int i = 0;
            portTemplates[i++] = (Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, type, "> Input");
            foreach (var transition in state.transitions)
            {
                portTemplates[i++] = (Orientation.Horizontal, Direction.Output, Port.Capacity.Single, type, transition.label);
                // Debug.Log($"Transition <b>{transition.name}</b> connects <b>{state.name}</b> and <b>{transition.destination.name}</b> states");
            }
            node = SimpletonInspectorNode.Factory(node_title:state.GetType().Name, out ports, portTemplates);
        }

        public void SetPositionCircle(float radians)
        {
            Vector2 offset = new Vector2{ x=400, y=200 };
            this.SetPosition(new Rect(offset + new Vector2{ x=math.cos(radians), y=math.sin(radians) } * 250, this.GetPosition().size));
        }

    }


#endregion
}
#endif
