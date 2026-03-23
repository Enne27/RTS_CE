using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Esta clase sigue el patrón Singleton.
/// Maneja todo el comportamiento de la clase abstracta UIView.
/// Este script mantiene un seguimiento de cuál es la vista actual, y el historial de listas que hemos seguido hasta llegar a ella.
/// </summary>
public class ViewManager : MonoBehaviour
{
    #region SINGLETON
    // Esta variable no es accesible directamente. Es la instancia del singleton.
    private static ViewManager instance; 

    /// <summary>
    /// Acceder a la instancia singleton.
    /// </summary>
    public static ViewManager Instance => instance;
    #endregion

    #region VARIABLES

    [Header("FUNCIONAMIENTO INTERNO")]
    /// <summary>
    /// Vista de inicio asignada desde el Inspector (por ejemplo, Main Menu)
    /// </summary>
    [Tooltip("Vista en la que el jugador comienza.")]
    [SerializeField] public View startingView;

    /// <summary>
    /// Array con todas las vistas disponibles en la escena.
    /// </summary>
    [SerializeField, Tooltip("Array de todas las vistas disponibles en la escena.")]
    private View[] views;

    /// <summary>
    /// Diccionario para mapear los GUID (en forma de string) a las vistas. 
    /// Esto permite identificar de forma única cada vista, incluso entre escenas.
    /// </summary>
    private Dictionary<string, KeyValuePair<string, View>> viewDictionary = new Dictionary<string, KeyValuePair<string, View>>();

    /// <summary>
    /// Lista para mantener referencia de otros view managers que se auto añadirán
    /// si ya había un ViewManager, a la espera de ser procesados por cambio de escena.
    /// </summary>
    List<ViewManager> otherViewManagers;

    /// <summary>
    /// Referencia a la corutina que espera que haya algún ViewManager en la lista otherViewManagers para "Procesar" su contenido
    /// </summary>
    Coroutine sceneChangeCoroutine;

    /// <summary>
    /// Vista mostrada actualmente.
    /// </summary>
    [Tooltip("La vista que se muestra actualmente.")]
    [SerializeField] public View currentView;



    [Header("RETROCESO")]
    /// <summary>
    /// Historial de vistas para poder retroceder (por ejemplo, con un botón "Atrás")
    /// </summary>
    private readonly Stack<string> viewsHistory = new Stack<string>();

    /// <summary>
    /// Bool para saber si el cambio de escena está siendo hacia atrás o hacia delante.
    /// </summary>
    bool goingBackwards = false;

    #endregion

    #region UNITY_METHODS

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            instance.otherViewManagers = new List<ViewManager>();
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance.otherViewManagers == null)
                instance.otherViewManagers = new List<ViewManager>();

            // Añadir el nuevo ViewManager correctamente a la lista.
            if (!instance.otherViewManagers.Contains(this))
                instance.otherViewManagers.Add(this);

            InternalStart();
        }
    }

    private void Start()
    {
        InternalStart();

        // Mostramos la vista de inicio si está asignada.
        if (startingView != null)
            Show(startingView, false);
    }
    #endregion

    /// <summary>
    /// Inicio interno del propio ViewManager.
    /// Registra las vistas de escena en el diccionario interno, hace la inicialización de todas las vistas y las oculta al momento.
    /// </summary>
    void InternalStart()
    {
        // Registramos todas las vistas en el diccionario utilizando su GuidComponent.
        // Se espera que cada vista tenga asignado un GuidComponent para generar su ID único.
        foreach (View v in views)
        {
            var guidComp = v.GetComponent<GuidComponent>();
            if (guidComp != null)
            {
                string id = guidComp.GetGuid().ToString();
                if (!instance.viewDictionary.ContainsKey(id))
                {
                    instance.viewDictionary.Add(id, new KeyValuePair<string, View>(v.gameObject.scene.name, v));
                    Debug.Log($"Registrando vista: {v.gameObject.name} con GUID: {id} en la escena {v.gameObject.scene.name}");
                }
                else
                {
                    instance.viewDictionary[id] = new KeyValuePair<string, View>(v.gameObject.scene.name, v);
                    Debug.Log($"Actualizando vista: {v.gameObject.name} con GUID: {id} en la escena {v.gameObject.scene.name}");
                }
            }
            else
            {
                Debug.LogError($"La vista {v.gameObject.name} no tiene un GuidComponent asignado.");
            }
        }


        // Inicializamos y ocultamos todas las vistas.
        foreach (View v in views)
        {
            v.Initialize();
            v.Hide();
        }
    }

    /// <summary>
    /// Subscripción al cargar la escena.
    /// </summary>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (goingBackwards)
        {
            // Si vamos hacia atrás, procesamos el historial
            KeyValuePair<string, View> sceneView = FindViewById(instance.viewsHistory.Pop());
            View aView = sceneView.Value;
            if (sceneChangeCoroutine != null)
            {
                StopCoroutine(sceneChangeCoroutine);
                sceneChangeCoroutine = null;
            }
            sceneChangeCoroutine = StartCoroutine(_ProcessSceneViewManager());
            // Antes de mostrar, actualizamos las vistas de la escena actual
            Show(aView, false);
            // FALTA QUE DEL STACK ME LLEVE HACIA ATRAS Y BORRE LO QUE SE REPITE
        }
        else
        {
            if (sceneChangeCoroutine != null)
            {
                StopCoroutine(sceneChangeCoroutine);
                sceneChangeCoroutine = null;
            }
            sceneChangeCoroutine = StartCoroutine(_ProcessSceneViewManager());
        }

        goingBackwards = false;
        DebugPrintHistory();
    }

    /// <summary>
    /// Debug en consola de cuántas vistas hay en el historial mediante los IDs registrados.
    /// </summary>
    private void DebugPrintHistory()
    {
        Debug.Log("Historial de vistas (Count: " + viewsHistory.Count + "):");
        foreach (string guid in viewsHistory)
        {
            Debug.Log(" - " + guid);
        }
    }

    /// <summary>
    /// Corutina que espera a que haya un otherViewManager para procesarlo. Se iniciará si hemos cambiado de escena "hacia adelante"
    /// </summary>
    /// <returns></returns>
    IEnumerator _ProcessSceneViewManager()
    {
        while (instance.otherViewManagers == null || instance.otherViewManagers.Count == 0) // ESTE BUCLE SE REPETIRÁ DURANTE LA PRIMERA ESCENA, ESPERANDO A QUE PASEMOS A LAS DEMÁS
        {
            yield return null;
        }
        ProcessSceneViewManager();
        sceneChangeCoroutine = null;
    }

    /// <summary>
    /// Procesa el ViewManager local de la escena recién cargada, copiando su configuración al singleton persistente.
    /// Se asigna la startingView del ViewManager local y se copia el array de views, reconstruyéndose el diccionario de vistas.
    /// Finalmente, se destruyen todos los ViewManagers locales para consolidar la configuración en el singleton.
    /// </summary>
    private void ProcessSceneViewManager()
    {
        // Verifica que haya al menos un ViewManager local en la lista.
        if (instance.otherViewManagers != null && instance.otherViewManagers.Count > 0)
        {
            // Obtenemos el primer ViewManager local.
            ViewManager localVM = instance.otherViewManagers[0];

            // Mostramos la startingView del ViewManager local en el singleton.
            // Se asume que la startingView está asignada en el Inspector en la nueva escena.
            if (localVM.startingView != null)
            {
                // Se muestra sin guardar en el historial (remember = false). ESTO LO HE CAMBIADO, YO QUIERO QUE SE GUARDE
                Show(localVM.startingView, true);
            }
            else
            {
                Debug.LogWarning("El ViewManager local no tiene startingView asignada.");
            }

            // Copiamos el array de views del ViewManager local al singleton. Deberíamos copiar la starting view también?
            instance.views = new View[localVM.views.Length];
            localVM.views.CopyTo(instance.views, 0);

            foreach (View v in instance.views)
            {
                if (v != null)
                {
                    var guidComp = v.GetComponent<GuidComponent>();
                    if (guidComp != null)
                    {
                        string id = guidComp.GetGuid().ToString();
                        // Guardamos la vista junto con el nombre de la escena en que se encuentra.
                        if (!instance.viewDictionary.ContainsKey(id))
                        {
                            instance.viewDictionary.Add(id, new KeyValuePair<string, View>(v.gameObject.scene.name, v));
                        }
                        else
                        {
                            instance.viewDictionary[id] = new KeyValuePair<string, View>(v.gameObject.scene.name, v);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("La vista " + v.gameObject.name + " no tiene asignado un GuidComponent.");
                    }
                }
            }

            // Destruye todos los ViewManagers locales ya que sus datos se han copiado al singleton.
            foreach (ViewManager aVM in instance.otherViewManagers)
            {
                Destroy(aVM.gameObject);
            }
            instance.otherViewManagers.Clear();
        }
    }

    /// <summary>
    /// Obtiene una vista a partir de su GUID (en formato string).
    /// </summary>
    /// <param name="guid">El GUID de la vista en formato string.</param>
    /// <returns>La vista asociada o null si no se encuentra.</returns>
    public static KeyValuePair<string, View> GetViewByGuid(string guid)
    {
        if (instance.viewDictionary.TryGetValue(guid, out KeyValuePair<string, View> view))
        {
            return view;
        }
        return new KeyValuePair<string, View>("", null);
    }

    /// <summary>
    /// Muestra una vista del tipo T (que hereda de View) y oculta la vista actual.
    /// Si 'remember' es true, se guarda la vista actual en el historial para poder volver atrás.
    /// </summary>
    /// <typeparam name="T">Tipo de la vista que se desea mostrar.</typeparam>
    /// <param name="remember">Indica si se debe guardar la vista actual en el historial.</param>
    public static void Show<T>(bool remember = true) where T : View
    {
        //Comprobar si la vista ya existe en el stack, pero no está la siguiente
        //Si no existe, funcioinamiento normal
        //Si si existe, llamada al Clear hasta llegar a la vista deseada y hacer Show de esta. Asegurandote de hacer Show con remember a false

        // Recorremos el array de vistas buscando una vista del tipo T.
        for (int i = 0; i < instance.views.Length; i++)
        {
            if (instance.views[i] is T)
            {
                // Si hay una vista actual, la ocultamos y, si 'remember' es true, la guardamos en el historial.
                if (instance.currentView != null)
                {
                    if (remember)
                    {
                        instance.viewsHistory.Push(instance.currentView.GetGuid());
                    }
                    instance.currentView.Hide();
                }
                // Mostramos la vista encontrada y la asignamos como la actual.
                instance.views[i].Show();
                instance.currentView = instance.views[i];
                break;
            }
        }
    }

    /// <summary>
    /// Muestra la vista pasada como parámetro y oculta la vista actual.
    /// Si 'remember' es true, se guarda la vista actual en el historial.
    /// </summary>
    /// <param name="view">La vista a mostrar.</param>
    /// <param name="remember">Si se debe recordar la vista actual en el historial.</param>
    public static void Show(View view, bool remember = true)
    {
        if (instance.currentView != null)
        {
            if (remember)
            {
                instance.viewsHistory.Push(instance.currentView.GetGuid());
            }
            instance.currentView.Hide();
        }
        view.Show();
        instance.currentView = view;
    }

    /// <summary>
    /// Busca en la escena y oculta la vista indicada.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void Hide<T>() where T : View
    {
        T view = FindFirstObjectByType<T>(); // Busca incluso objetos inactivos
        if (view != null)
        {
            view.Hide(); // Llama al método Hide de la vista
        }
    }

    /// <summary>
    /// Retrocede a la vista anterior.
    /// </summary>
    /// <param name="remember"></param>
    private static void ShowLastViewInternal(bool remember = true)
    {
        // Mientras haya entradas en el historial...
        while (instance.viewsHistory.Count > 0)
        {
            string topGuid = instance.viewsHistory.Peek();
            KeyValuePair<string, View> sceneView = FindViewById(topGuid); // FIND VIEW BY ID Y GET VIEW BY ID HACEN LO MISMO????

            if (sceneView.Value != null)
            {
                instance.viewsHistory.Pop();
                Show(sceneView.Value, remember);
                return;
            }
            else
            {
                // Si la vista válida está en otra escena, marcamos que se va hacia atrás y cambiamos la escena.
                instance.goingBackwards = true;
                if (instance.sceneChangeCoroutine != null)
                {
                    instance.StopCoroutine(instance.sceneChangeCoroutine);
                    instance.sceneChangeCoroutine = null;
                }
                FindFirstObjectByType<ScenesManager>().ChangeScene(sceneView.Key, false);
                return;
            }
        }
    }

    /// <summary>
    /// Llama al método para retroceder a la vista anterior.
    /// </summary>
    /// <param name="steps"></param>
    /// <param name="remember"></param>
    public static void ShowLastView(int steps = 1, bool remember = true)
    {
        for (int i = 0; i < steps; i++)
        {
            ShowLastViewInternal(remember);
        }
    }

    /// <summary>
    /// Utilizando el guid como referencia. Se busca una referencia de una View.
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    public static KeyValuePair<string, View> FindViewById(string guid)
    {
        instance.viewDictionary.TryGetValue(guid, out KeyValuePair<string, View> view); // ESTO DA NULL, EL PROBLEMA ESTÁ AQUÍ

        return view;
    }


    /// <summary>
    /// Devuelve la primera vista que hereda de T, buscando en el array de vistas.
    /// </summary>
    /// <typeparam name="T">Tipo de vista que se desea obtener</typeparam>
    /// <returns>Instancia de la vista o null si no se encuentra</returns>
    public static T GetView<T>() where T : View
    {
        for (int i = 0; i < instance.views.Length; i++)
        {
            if (instance.views[i] is T view)
            {
                return view;
            }
        }
        return null;
    }

    /// <summary>
    /// Para el menú de pausa. Indica si una vista está activa o no.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool IsViewActive<T>() where T : View
    {
        T view = FindFirstObjectByType<T>();
        return view != null && view.gameObject.activeInHierarchy;
    }

    /// <summary>
    /// Guarda la vista actual en el stack.
    /// </summary>
    public void RememberCurrentView()
    {
        instance.viewsHistory.Push(instance.currentView.GetGuid());
    }
}
