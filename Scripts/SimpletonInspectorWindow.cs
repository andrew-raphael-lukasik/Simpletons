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

        [MenuItem("Window/"+nameof(Simpleton)+"/"+nameof(SimpletonInspectorWindow))]
        public static void CreateWindow()
        {
            (SimpletonState[],SimpletonInspectorNode[],Edge[],string[] logs) rebuild()
            {
                var node1 = SimpletonInspectorNode.Factory("node 1", out var ports1, (Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(SimpletonInspectorNode), "input port"));
                node1.SetPositionCircle(math.PI*2f * 0f/2f);

                var node2 = SimpletonInspectorNode.Factory("node 2", out var ports2, (Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(SimpletonInspectorNode), "output port"));
                node2.SetPositionCircle(math.PI*2f * 1f/2f);

                return (new SimpletonState[]{}, new SimpletonInspectorNode[]{ node1, node2 }, new Edge[0]{}, new string[0]);
            };
            CreateWindow(rebuild, (map)=>{});
        }
        public static void CreateWindow(
            System.Func<(SimpletonState[], SimpletonInspectorNode[], Edge[], string[] logs)> on_rebuild,
            System.Action<Dictionary<SimpletonState,SimpletonInspectorNode>> on_update
        )
        {
            var window = EditorWindow.GetWindow<SimpletonInspectorWindow>();
            window.titleContent = new GUIContent(nameof(SimpletonInspectorWindow));
            window.onRebuild = on_rebuild;
            window.onUpdate = on_update;
            window.Rebuild();
            window.Show();
        }

        event System.Func<(SimpletonState[] states,SimpletonInspectorNode[] nodes,Edge[] edges, string[] logs)> onRebuild;
        event System.Action<Dictionary<SimpletonState,SimpletonInspectorNode>> onUpdate;
        Dictionary<SimpletonState,SimpletonInspectorNode> _map = new();

        // void OnEnable() => Rebuild();

        void Update()
        {
            if (onUpdate!=null && _map!=null)
                onUpdate(_map);
        }

        void Rebuild()
        {
            rootVisualElement.Clear();
            if (onRebuild!=null)
            {
                var dat = onRebuild();
                var states = dat.states;
                var nodes = dat.nodes;

                int numNodes = nodes.Length;
                int numStates = states.Length;
                int len = Mathf.Min(numNodes, numStates);
                
                _map.Clear();
                for (int i=0 ; i<len ; i++)
                    _map.Add(dat.states[i], dat.nodes[i]);

                Rebuild(nodes, dat.edges, dat.logs);
            }
            else
            {
                _map.Clear();
                Rebuild(new SimpletonInspectorNode[0]{}, new Edge[0]{}, new string[0]);
            }
        }

        void Rebuild(
            SimpletonInspectorNode[] nodes,
            Edge[] edges,
            string[] logs
        )
        {
            var MAIN_VIEW = new VisualElement();
            {
                var style = MAIN_VIEW.style;
                style.flexGrow = 1;
                style.flexDirection = FlexDirection.RowReverse;
            }
            rootVisualElement.Add(MAIN_VIEW);

            var GRAPH_CONTAINER = new VisualElement();
            {
                GRAPH_CONTAINER.style.flexGrow = 1;
                // BOX.style.minHeight = BOX.style.minWidth = 300f;
            }
            MAIN_VIEW.Add(GRAPH_CONTAINER);

            var GRAPH = new SimpletonInspectorGraphView();
            {
                //GRAPH.StretchToParentSize();
                var style = GRAPH.style;
                style.flexGrow = 1;
            }
            {
                foreach (var node in nodes)
                    GRAPH.AddElement(node);
                
                foreach (var edge in edges)
                    GRAPH.Add(edge);
                    // Debug.Log($"Edge <b>{edge.name}</b> connects <b>{edge.output.portName}</b> and <b>{edge.input.portName}</b> ports");
                
                foreach (var node in nodes)
                {
                    node.RefreshExpandedState();
                    node.RefreshPorts();
                }
            }
            GRAPH_CONTAINER.Add(GRAPH);

            var LOGS_LIST = new ListView();
            {
                var style = LOGS_LIST.style;
                // style.flexGrow = 1;
                // style.minHeight = LOGS_LIST.itemHeight * 12;
                style.minWidth = 200;
                style.maxWidth = new StyleLength(new Length(50,LengthUnit.Percent));
                // style.position = Position.Absolute;
                // style.left = 10;
                // style.top = 10;
                style.borderTopWidth = style.borderLeftWidth = style.borderRightWidth = style.borderBottomWidth = 1;
                style.borderTopColor = style.borderLeftColor = style.borderRightColor = style.borderBottomColor = new Color{ r=0.1f, g=0.1f, b=0.1f, a=1 };;
            }
            //LOGS_LIST.headerTitle = $"Last Log Messages | Capacity:";
            //LOGS_LIST.showFoldoutHeader = true;
            LOGS_LIST.itemsSource = logs;
            LOGS_LIST.fixedItemHeight = 16;
            LOGS_LIST.makeItem =()=> new Label();
            LOGS_LIST.bindItem = (ve,i) => {
                var label = (ve as Label);
                label.text = (string) LOGS_LIST.itemsSource[i];
                label.style.backgroundColor = !string.IsNullOrEmpty(label.text) ? Color.HSVToRGB(H:new Random((uint)Mathf.Abs(label.text.GetHashCode())).NextFloat(), S:0.5f, V:0.4f) : new Color{ r=0.2f, g=0.2f, b=0.2f, a=1 };
            };
            LOGS_LIST.selectionType = SelectionType.Single;
            MAIN_VIEW.Add(LOGS_LIST);

            var TOOLBAR = new Toolbar();
            {
                var REFRESH = new ToolbarButton(Rebuild);
                REFRESH.text = "Refresh ↻";
                TOOLBAR.Add(REFRESH);
            }
            rootVisualElement.Add(TOOLBAR);
        }

        public static void InspectAI(SimpletonStateMachine stateMachine)
        {
            if (stateMachine==null)
            {
                Debug.LogError($"{nameof(stateMachine)} is null");
                return;
            }

            CreateWindow(
                on_rebuild:() =>
                {
                    var stateSet = stateMachine.FindAllStates();
                    var states = new SimpletonState[ stateSet.Count ];
                    var nodes = new SimpletonInspectorNode[ stateSet.Count ];;
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

                    var edges = new List<Edge>(capacity: nodes.Length * 2);
                    foreach (var srcState in stateNodePorts.Keys)
                    {
                        var srcNodePorts = stateNodePorts[srcState];
                        for (int i=0 ; i<srcState.transitions.Length ; i++)
                        {
                            var srcNodeOutput = srcNodePorts.ports[1+i];
                            var srcNodeTransition = srcState.transitions[i];

                            var dstState = srcNodeTransition.destination;
                            var dstNodeInput = stateNodePorts[dstState].ports[0];

                            // Debug.Log($"State <b>{srcState.name}</b> transitions to <b>{dstState.name}</b> via <b>{srcNodeTransition.name}</b> transition");
                        
                            var edge = srcNodeOutput.ConnectTo(dstNodeInput);
                            edge.name = srcNodeTransition.label;
                            edges.Add(edge);

                            // Debug.Log($"\tEdge <b>{edge.name}</b> will connect <b>{edge.output.node.title}.{edge.output.portName}</b> and <b>{edge.input.node.title}.{edge.input.portName}</b> ports");
                        }
                    }
                
                    return (states, nodes, edges.ToArray(), stateMachine.DebugLogs.ToArray());
                } ,
                on_update: (stateNodes) =>
                {
                    foreach (var kv in stateNodes)
                    {
                        var state = kv.Key;
                        var node = kv.Value;
                        var style = node.style;
                        if (state!=stateMachine.Current)
                        {
                            style.borderTopWidth = 0;
                            style.borderBottomWidth = 0;
                        }
                        else
                        {
                            var col = Color.yellow;
                            style.borderTopWidth = 2;
                            style.borderTopLeftRadius = 6;
                            style.borderTopRightRadius = 6;
                            style.borderTopColor = col;
                            style.borderBottomWidth = 2;
                            style.borderBottomLeftRadius = 12;
                            style.borderBottomRightRadius = 12;
                            style.borderBottomColor = col;
                        }
                    }
                }
            );
        }
    }

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
            string node_title ,
            out Port[] ports ,
            params (Orientation orientation, Direction direction, Port.Capacity capacity, System.Type type, string port_name)[] port_templates
        )
        {
            var node = new SimpletonInspectorNode{
                title    = node_title ,
            };
            {
                ports = new Port[ port_templates.Length ];
                for (int i=0 ; i<port_templates.Length ; i++)
                {
                    var IN = port_templates[i];
                    var port = node.InstantiatePort(IN.orientation, IN.direction, IN.capacity, IN.type);
                    port.portName = IN.port_name;
                    if (IN.direction==Direction.Output)
                        port.portColor = Color.HSVToRGB(H:new Random((uint)Mathf.Abs(IN.port_name.GetHashCode())).NextFloat(), S:0.5f, V:1);
                    node.outputContainer.Add(port);
                    ports[i] = port;
                }
            }
            return node;
        }
        public static SimpletonInspectorNode Factory(
            string node_title ,
            out Port[] ports ,
            params (bool vertical, bool output, bool multi, System.Type type, string port_name)[] port_templates
        )
        {
            var arr = new (Orientation orientation, Direction direction, Port.Capacity capacity, System.Type type, string port_name)[ port_templates.Length ];
            if (port_templates.Length!=0)
            for (int i=0 ; i<port_templates.Length ; i++)
            {
                var next = arr[i];
                var template = port_templates[i];
                next.orientation    = template.vertical ? Orientation.Vertical  : Orientation.Horizontal;
                next.direction      = template.output   ? Direction.Output      : Direction.Input;
                next.capacity       = template.multi    ? Port.Capacity.Multi   : Port.Capacity.Single;
                next.type           = template.type;
                next.port_name      = template.port_name;
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

    // public class SimpletonInspectorPort : Port
    // {
    //     public SimpletonInspectorPort(
    //         Orientation portOrientation,
    //         Direction portDirection,
    //         Capacity portCapacity,
    //         System.Type type
    //     )
    //         : base(
    //             portOrientation,
    //             portDirection,
    //             portCapacity,
    //             type
    //         )
    //     {}
    // }

}
#endif
