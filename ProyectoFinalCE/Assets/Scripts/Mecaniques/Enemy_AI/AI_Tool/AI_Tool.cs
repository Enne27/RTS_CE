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
    [MenuItem("Window/UI Toolkit/AI_Tool")]
    public static void ShowExample()
    {
        AI_Tool wnd = GetWindow<AI_Tool>();
        wnd.titleContent = new GUIContent("AI_Tool");
    }

    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    bool isSubscribed = false;
    VisualElement root;
    private int nSide = 10;
    private int nResources = 5;
    private Spectator spectator = Spectator.Impartial;

    
    Dictionary<VisualElement, Enemy_AI> IAs = new Dictionary<VisualElement, Enemy_AI>();
    Dictionary<VisualElement, MapCell> map = new Dictionary<VisualElement, MapCell> ();

    private void OnDisable()
    {
        StopSimulation();
    }

    public void CreateGUI()
    {
        root = rootVisualElement;
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        

        var grid = root.Q<VisualElement>("Map");
        if (grid == null)
        {
            Debug.LogError("Map VisualElement not found in UXML.");
            return;
        }
        GenerateGrid(grid);

        var nSideField = root.Q<IntegerField>("nSideField");
        nSideField.RegisterValueChangedCallback(evt =>
        {
            nSide = evt.newValue;
            map.Clear();
            grid.Clear();
            GenerateGrid(grid);
        });

        var nResourcesField = root.Q<IntegerField>("nResources");
        nResourcesField.RegisterValueChangedCallback(evt =>
        {
            nResources = evt.newValue;
        });

        var GenerateMapBtn = root.Q<Button>("GenerateMap");
        GenerateMapBtn.RegisterCallback<ClickEvent>(evt =>
        {
            generate_map();
        });

        var Select_IA1_Vision = root.Q<Button>("Select_IA1_Vision");
        Select_IA1_Vision.RegisterCallback<ClickEvent>(evt =>
        {
            spectator = Spectator.Ia1_Side;
            Debug.Log("ia1");
        });

        var Select_IA2_Vision = root.Q<Button>("Select_IA2_Vision");
        Select_IA2_Vision.RegisterCallback<ClickEvent>(evt =>
        {
            spectator = Spectator.Ia2_Side;
            Debug.Log("ia2");
        });

        var Select_ImpartialVision = root.Q<Button>("Select_ImpartialVision");
        Select_ImpartialVision.RegisterCallback<ClickEvent>(evt =>
        {
            spectator = Spectator.Impartial;
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

        var ia_1VE = root.Q<VisualElement>("IA_1");
        var ia_2VE = root.Q<VisualElement>("IA_2");
        Enemy_AI ia_1Data = new Enemy_AI("IA_1");
        Enemy_AI ia_2Data = new Enemy_AI("IA_2");

        IAs.Add(ia_1VE, ia_1Data);
        IAs.Add(ia_2VE, ia_2Data);
        ia_setup();
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
                column.RegisterCallback<MouseLeaveEvent>(evt => 
                {
                    switch (spectator)
                    {
                        case Spectator.Ia1_Side: 
                            switch (IAs.Values.ElementAt(0).context.mapAwarness.ElementAt(map.Keys.ToList().IndexOf(column)))
                            {
                                case VisionStatus.OutOfVision: 
                                    break;
                                case VisionStatus.Undiscovered: 
                                    break;
                                case VisionStatus.Discovered:
                                    switch (map[column].cellZoneType)
                                    {
                                        case CellZoneType.Empty:
                                            column.style.backgroundColor = Color.white;
                                            break;
                                        case CellZoneType.Resources:
                                            column.style.backgroundColor = Color.green;
                                            break;

                                        case CellZoneType.AntHill:
                                            column.style.backgroundColor = Color.red;
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case Spectator.Ia2_Side:
                            switch (IAs.Values.ElementAt(1).context.mapAwarness.ElementAt(map.Keys.ToList().IndexOf(column)))
                            {
                                case VisionStatus.OutOfVision:
                                    column.style.backgroundColor = Color.grey;

                                    break;
                                case VisionStatus.Undiscovered:
                                    column.style.backgroundColor = Color.black;

                                    break;
                                case VisionStatus.Discovered:
                                    switch (map[column].cellZoneType)
                                    {
                                        case CellZoneType.Empty:
                                            column.style.backgroundColor = Color.white;
                                            break;
                                        case CellZoneType.Resources:
                                            column.style.backgroundColor = Color.green;
                                            break;

                                        case CellZoneType.AntHill:
                                            column.style.backgroundColor = Color.red;
                                            break;
                                    }
                                    break;
                            }
                            break;

                        default:
                            switch (map[column].cellZoneType)
                            {
                                case CellZoneType.Empty:
                                    column.style.backgroundColor = Color.grey;
                                    break;

                                case CellZoneType.Resources:
                                    column.style.backgroundColor = Color.green;
                                    break;

                                case CellZoneType.AntHill:
                                    column.style.backgroundColor = Color.red;
                                    break;
                            }
                            break;
                    }
                });
                column.AddManipulator(new Clickable(evt =>
                {
                    root.Q<Label>("SelectedElementInfo").text = map[column].index.ToString() + "\n" + map[column].cellZoneType;
                }));
                row.Add(column);
                map.Add(column, new MapCell(new Vector2Int(i,e)));
            }
        }
    }

    private void ia_setup() {
        foreach (var ia in IAs)
        {
            Label label = new Label();
            label.text = ia.Value.name;
            ia.Key.Add(label);
        }
    }

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

        for (int i = 0; i < nResources; i++)
        {
            Vector2Int pos = getRandomCell();
            MapCell cell = GetCellFromIndex(pos);
            cell.cellZoneType = CellZoneType.Resources;
            
        }

        foreach (var ia in IAs.Values)
        {
            Vector2Int pos = getRandomCell();
            MapCell cell = GetCellFromIndex(pos);
            cell.cellZoneType = CellZoneType.AntHill;
        }

        UpdateVisuals();

    }

    private void UpdateVisuals()
    {
        foreach (var kvp in map)
        {
            VisualElement ve = kvp.Key;
            MapCell cell = kvp.Value;

            switch (cell.cellZoneType)
            {
                case CellZoneType.Empty:
                    ve.style.backgroundColor = Color.grey;
                    break;

                case CellZoneType.Resources:
                    ve.style.backgroundColor = Color.green;
                    break;

                case CellZoneType.AntHill:
                    ve.style.backgroundColor = Color.red;
                    break;
            }
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

        EditorApplication.update += OnEditorUpdate;
        isSubscribed = true;
    }


    void StopSimulation()
    {
        if (!isSubscribed) return;

        EditorApplication.update -= OnEditorUpdate;
        isSubscribed = false;
    }
    private void OnEditorUpdate()
    {

    }
}

#endif