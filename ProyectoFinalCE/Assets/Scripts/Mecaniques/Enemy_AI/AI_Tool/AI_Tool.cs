using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public enum Spectator
{
    Ia1_Side,
    Ia2_Side,
    Impartial
}
#if UNITY_EDITOR

public class AI_Tool : EditorWindow
{
    #region Default

    [MenuItem("AiTools/AiTool")]
    public static void OpenWindow()
    {
        AI_Tool wnd = GetWindow<AI_Tool>();
        wnd.titleContent = new GUIContent("AI_Tool");
    }

    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;
    #endregion

    static double lastTime;
    public static double deltaTime;
    double time;

    AntHillView antHillView;
    InspectorView inspectorView_ia1;
    InspectorView inspectorView_ia2;
    IMGUIContainer blackboardView_ia1;
    IMGUIContainer blackboardView_ia2;

    SerializedObject antHillObject_ia1;
    SerializedObject antHillObject_ia2;
    SerializedProperty blackboardProperty_ia1;
    SerializedProperty blackboardProperty_ia2;

    bool isSubscribed = false;
    VisualElement root;
    VisualElement troopLayer;
    private int nSide = 10;
    private int nFoodResources = 5;
    private int nConstructionResources = 5;
    private Spectator spectator = Spectator.Impartial;
    private Anthill spectatorAntHill;


    //Enemy_AI ia_1 = ;
    //Enemy_AI ia_2 = ;
    Enemy_AI[] IAs = new Enemy_AI[2];
    //Dictionary<VisualElement, Enemy_AI> IAs = new Dictionary<VisualElement, Enemy_AI>();
    Dictionary <VisualElement, MapCell> map = new Dictionary<VisualElement, MapCell> ();

    private void OnDisable()
    {
        StopSimulation();
    }

    public void CreateGUI()
    {
        root = rootVisualElement;
        m_VisualTreeAsset.CloneTree(root);


        spectatorAntHill = ScriptableObject.CreateInstance<Anthill>();


        //var ia_1VE = root.Q<VisualElement>("IA_1");
        //var ia_2VE = root.Q<VisualElement>("IA_2");
        //Enemy_AI ia_1Data = new Enemy_AI("IA_1");
        //Enemy_AI ia_2Data = new Enemy_AI("IA_2");

        //IAs.Add(ia_1VE, ia_1Data);
        //IAs.Add(ia_2VE, ia_2Data);
        //ia_setup();
        IAs[0] = new Enemy_AI("IA_1");
        IAs[1] = new Enemy_AI("IA_2");

        var grid = root.Q<VisualElement>("Map");
        GenerateGrid(grid);

        antHillView = root.Q<AntHillView>();
        inspectorView_ia1 = root.Q<InspectorView>("InspectorView_IA1");
        inspectorView_ia2 = root.Q<InspectorView>("InspectorView_IA2");
        antHillView.OnStructureSelected = OnStructureSelectionChanged;


        antHillObject_ia1 = new SerializedObject(IAs[0].anthill);
        blackboardProperty_ia1 = antHillObject_ia1.FindProperty("resources");
        antHillObject_ia2 = new SerializedObject(IAs[1].anthill);
        blackboardProperty_ia2 = antHillObject_ia2.FindProperty("resources");

        blackboardView_ia1 = root.Q<IMGUIContainer>("blackboardView_ia1");
        blackboardView_ia1.onGUIHandler = () =>
        {
            antHillObject_ia1.Update();
            EditorGUILayout.PropertyField(blackboardProperty_ia1);
            antHillObject_ia1.ApplyModifiedProperties();
        };

        blackboardView_ia2 = root.Q<IMGUIContainer>("blackboardView_ia2");
        blackboardView_ia2.onGUIHandler = () =>
        {
            antHillObject_ia2.Update();
            EditorGUILayout.PropertyField(blackboardProperty_ia2);
            antHillObject_ia2.ApplyModifiedProperties();
        };


        troopLayer = new VisualElement();
        troopLayer.name = "TroopLayer";

        troopLayer.style.position = Position.Absolute;
        troopLayer.style.left = 0;
        troopLayer.style.top = 0;
        troopLayer.style.right = 0;
        troopLayer.style.bottom = 0;
        troopLayer.pickingMode = PickingMode.Ignore;
        grid.Add(troopLayer);

        var nSideField = root.Q<IntegerField>("nSideField");
        nSideField.RegisterValueChangedCallback(evt =>
        {
            nSide = evt.newValue;
            map.Clear();
            grid.Clear();
            GenerateGrid(grid);
        });

        var nFoodResourcesField = root.Q<IntegerField>("nFoodResources");
        nFoodResourcesField.RegisterValueChangedCallback(evt =>
        {
            nFoodResources = evt.newValue;
        });
        var nConstructionResourcesField = root.Q<IntegerField>("nConstructionResources");
        nConstructionResourcesField.RegisterValueChangedCallback(evt =>
        {
            nConstructionResources = evt.newValue;
        });

        var GenerateMapBtn = root.Q<Button>("GenerateMap");
        GenerateMapBtn.RegisterCallback<ClickEvent>(evt =>
        {
            ResetMapAwarness();
            generate_map();
            ReaplyColorAll();
        });

        var Select_IA1_Vision = root.Q<Button>("Select_IA1_Vision");
        Select_IA1_Vision.RegisterCallback<ClickEvent>(evt =>
        {
            spectator = Spectator.Ia1_Side;
            ReaplyColorAll();
            antHillView.PopulateView(IAs[0].anthill);
            Debug.Log("ia1");
        });

        var Select_IA2_Vision = root.Q<Button>("Select_IA2_Vision");
        Select_IA2_Vision.RegisterCallback<ClickEvent>(evt =>
        {
            spectator = Spectator.Ia2_Side;
            ReaplyColorAll();
            antHillView.PopulateView(IAs[1].anthill);
            Debug.Log("ia2");
        });

        var Select_ImpartialVision = root.Q<Button>("Select_ImpartialVision");
        Select_ImpartialVision.RegisterCallback<ClickEvent>(evt =>
        {
            spectator = Spectator.Impartial;
            ReaplyColorAll();
            antHillView.PopulateView(spectatorAntHill);
            Debug.Log("impartial");
        });

        var StartSimBtn = root.Q<Button>("StartSimulation");
        StartSimBtn.RegisterCallback<ClickEvent>(evt =>
        {
            StartSimulation();
        });

        var StopSimBtn = root.Q<Button>("StopSimulation");
        StopSimBtn.RegisterCallback<ClickEvent>(evt =>
        {
            StopSimulation();
        });


    }

    void OnStructureSelectionChanged(StructureView structureView)
    {
        switch (spectator)
        {
            case Spectator.Ia1_Side:
                inspectorView_ia1.UpdateSelection(structureView);
                break;
            case Spectator.Ia2_Side:
                inspectorView_ia2.UpdateSelection(structureView);
                break;
        }
    }

    private void GenerateGrid(VisualElement grid)
    {
        for(int i = 0; i < nSide; i++)
        {
            VisualElement row = new VisualElement();
            row.AddToClassList("row");
            grid.Add(row);  
            for (int e = 0; e < nSide; e++)
            {

                VisualElement column = new VisualElement();
                column.AddToClassList("column");
                column.RegisterCallback<MouseEnterEvent>(evt => column.style.backgroundColor = Color.yellow);
                column.RegisterCallback<MouseLeaveEvent>(OnColumnMouseLeave);
                column.AddManipulator(new Clickable(evt =>
                {
                    root.Q<Label>("SelectedElementInfo").text = map[column].index.ToString() + "\n" + map[column].cellZoneType;
                }));
                row.Add(column);
                map.Add(column, new MapCell(new Vector2Int(i,e)));

            }
        }

        foreach (var kvp in map)
        {
            ApplyZoneColor(kvp.Key);
        }

        ResetMapAwarness();


    }

    void ResetMapAwarness()
    {
        foreach (var ia in IAs)
        {
            ia.context.mapAwarness = Enumerable
                .Repeat(VisionStatus.Undiscovered, nSide * nSide)
                .ToList();
        }
    }

    void ReaplyColorAll()
    {
        foreach (var kvp in map)
        {
            var column = kvp.Key;
            switch (spectator)
            {
                case Spectator.Ia1_Side:
                    ApplyVisionColor(column, 0);
                    break;

                case Spectator.Ia2_Side:
                    ApplyVisionColor(column, 1);
                    break;

                default:
                    ApplyZoneColor(column);
                    break;
            }
        }
    }

    private void OnColumnMouseLeave(MouseLeaveEvent evt)
    {
        var column = (VisualElement)evt.currentTarget;

        switch (spectator)
        {
            case Spectator.Ia1_Side:
                ApplyVisionColor(column, 0);
                break;

            case Spectator.Ia2_Side:
                ApplyVisionColor(column, 1);
                break;

            default:
                ApplyZoneColor(column);
                break;
        }
    }

    private void ApplyVisionColor(VisualElement column, int iaIndex)
    {
        int index = map.Keys.ToList().IndexOf(column);
        var vision = IAs[iaIndex].context.mapAwarness.ElementAt(index);

        switch (vision)
        {
            case VisionStatus.OutOfVision:
                column.style.backgroundColor = Color.grey;
                break;

            case VisionStatus.Undiscovered:
                column.style.backgroundColor = Color.gray1;
                break;

            case VisionStatus.Discovered:
                ApplyZoneColor(column);
                break;
        }
    }

    private void ApplyZoneColor(VisualElement column)
    {
        switch (map[column].cellZoneType)
        {
            case CellZoneType.Empty:
                column.style.backgroundColor = Color.white;
                break;

            case CellZoneType.ResourcesFood:
                column.style.backgroundColor = Color.green;
                break;

            case CellZoneType.ResourcesConstruction:
                column.style.backgroundColor = Color.chocolate;
                break;

            case CellZoneType.AntHill_IA1:
                column.style.backgroundColor = Color.red;
                break;
            case CellZoneType.AntHill_IA2:
                column.style.backgroundColor = Color.blue;
                break;
        }
    }

    //private void ia_setup() {
    //    foreach (var ia in IAs)
    //    {
    //        ia.Value.resources = new AntHillResources();

    //        Label iaName = new Label();
    //        iaName.text = ia.Value.name;

    //        Label evolucion = new Label();
    //        evolucion.text = "Evolucion: " + Evoluciones.Brote.ToString();

    //        Label eggCapacity = new Label();
    //        eggCapacity.text = "Egg Capacity: " + ia.Value.resources.eggCapacity;

    //        Label eggs = new Label();
    //        eggs.text = "Eggs: " + ia.Value.resources.eggs;

    //        Label foodCapacity = new Label();
    //        foodCapacity.text = "Food capacity: " + ia.Value.resources.foodCapacity;

    //        Label food = new Label();
    //        food.text = "Food: " + ia.Value.resources.food;

    //        Label upgradePoints = new Label();
    //        upgradePoints.text = "upgradePoints: " + ia.Value.resources.upgradePoints;

    //        ia.Key.Add(iaName);
    //        ia.Key.Add(evolucion);
    //        ia.Key.Add(eggCapacity);
    //        ia.Key.Add(eggs);
    //        ia.Key.Add(foodCapacity);
    //        ia.Key.Add(food);
    //        ia.Key.Add(upgradePoints);

    //    }
    //}

    private void generate_map()
    {
        foreach (var cell in map.Values)
        {
            cell.cellZoneType = CellZoneType.Empty;
        }

        List<Vector2Int> selectedCells = new List<Vector2Int>(); 

        Vector2Int getRandomCell()
        {
            Vector2Int randomCell;
            do
            {
                randomCell = new Vector2Int(Random.Range(0, nSide), Random.Range(0, nSide));

            } while (selectedCells.Contains(randomCell));

            selectedCells.Add(randomCell);

            return randomCell;
        }

        for (int i = 0; i < nConstructionResources; i++)
        {
            Vector2Int pos = getRandomCell();
            MapCell cell = GetCellFromIndex(pos);
            cell.cellZoneType = CellZoneType.ResourcesConstruction;
        }
        for (int i = 0; i < nFoodResources; i++)
        {
            Vector2Int pos = getRandomCell();
            MapCell cell = GetCellFromIndex(pos);
            cell.cellZoneType = CellZoneType.ResourcesFood;
        }

        foreach (var ia in IAs)
        {
            Vector2Int pos = getRandomCell();
            
            MapCell cell = GetCellFromIndex(pos);
            cell.cellZoneType = ia.name == "IA_1" ? CellZoneType.AntHill_IA1 : CellZoneType.AntHill_IA2;
            ia.context.pos_AntHill = pos;
            ia.context.mapAwarness[getIndexFrom2D(pos)] = VisionStatus.Discovered;
        }
    }
    private MapCell GetCellFromIndex(Vector2Int index)
    {
        foreach (var cell in map.Values)
        {
            if (cell.index == index)
                return cell;
        }

        return null;
    }

    void StartSimulation()
    {
        if (isSubscribed) return;
        lastTime = EditorApplication.timeSinceStartup;
        EditorApplication.update += OnEditorUpdate;
        isSubscribed = true;
    }

    int getIndexFrom2D(Vector2Int pos)
    {
        return pos.x * nSide + pos.y;
    }


    void StopSimulation()
    {
        if (!isSubscribed) return;

        EditorApplication.update -= OnEditorUpdate;
        isSubscribed = false;
    }



    int troops = 0;
    private HashSet<System.Type> createdStructures = new HashSet<System.Type>();

    private void OnEditorUpdate()
    {
        double currentTime = EditorApplication.timeSinceStartup;
        deltaTime = currentTime - lastTime;
        lastTime = currentTime;

        time += deltaTime;

        foreach (var ia in IAs)
        {
            ia.Process(() => antHillView.PopulateView(ia.anthill));

            ia.anthill.structures.ForEach(s =>
            {
                switch (s.structureState)
                {
                    case Structure.state.OnConstruction:
                        s.OnConstruction();
                        break;
                    case Structure.state.OnUpdate:
                        if (time >= 1) s.update();
                        break;
                    case Structure.state.Idle:
                        break;
                }
            });
        }

        if (time >= 1) time = 0;




        //int maxTroops = 10;
        //if (troops < maxTroops)
        //{
        //    troops++;

        //    troopLayer.Add(new Unit(IAs.Values.ElementAt(0), new Vector2(Random.Range(0, troopLayer.resolvedStyle.width), Random.Range(0, troopLayer.resolvedStyle.height))).visualElement);
        //    troopLayer.Add(new Unit(IAs.Values.ElementAt(1), new Vector2(Random.Range(0, troopLayer.resolvedStyle.width), Random.Range(0, troopLayer.resolvedStyle.height))).visualElement);

        //}


        //var types = TypeCache.GetTypesDerivedFrom<Structure>();
        //float startX = 300f;
        //float startY = 50f;
        //float xSpacing = 200f;
        //float ySpacing = 120f;

        //int index = 0;

        //foreach (var type in types)
        //{
        //    if (createdStructures.Contains(type))
        //        continue;
        //    int depth = Mathf.FloorToInt(Mathf.Log(index + 1, 2)); // tree depth
        //    int positionInLevel = index - (int)Mathf.Pow(2, depth) + 1;

        //    float x = startX + (positionInLevel - Mathf.Pow(2, depth) / 2f) * xSpacing;
        //    float y = startY + depth * ySpacing;

        //    //antHillView.PopulateView(IAs[0].anthill);
        //    IAs[0].anthill.CreateStructure(type, new Rect(x, y, 150, 80));
        //    //antHillView.CreateStructure(type, new Rect(x, y, 150, 80));
        //    //antHillView.PopulateView(IAs[0].anthill);

        //    index++;
        //}
    }
}

class Unit
{
    public Enemy_AI owner;
    public VisualElement visualElement;
    public Vector2 position;
    public Vector2 velocity;
    //TipoHormiga

    public Unit(Enemy_AI _owner, Vector2 position)
    {
        owner = _owner;
        visualElement = new VisualElement();
        visualElement.style.position = Position.Absolute;
        visualElement.style.width = 10;
        visualElement.style.height = 10;

        visualElement.style.backgroundColor = owner.name == "IA_1" ? Color.red : Color.blue;
        visualElement.style.left = position.x;
        visualElement.style.top = position.y;
    }
}

#endif