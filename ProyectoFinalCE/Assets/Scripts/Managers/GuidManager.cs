using System;
using System.Collections.Generic;

/// <summary>
/// Gestor de GUIDs para registrar y eliminar GuidComponents.
/// Le asigna a cada objeto un identificador único.
/// Esto se utiliza para gestionar el stack de la UI.
/// </summary>
public static class GuidManager
{
    /// <summary>
    /// Este diccionario mapea GUIDs a los componentes registrados.
    /// </summary>
    private static Dictionary<Guid, GuidComponent> guidDictionary = new Dictionary<Guid, GuidComponent>();

    /// <summary>
    /// Intenta añadir el componente a la gestión de GUIDs.
    /// Devuelve true si se ha añadido correctamente, o false si ya existe el GUID.
    /// </summary>
    /// <param name="comp">El GuidComponent que quieres registrar.</param>
    /// <returns>True si se registró correctamente, false en caso contrario.</returns>
    public static bool Add(GuidComponent comp)
    {
        Guid guid = comp.GetGuid();
        if (guid == Guid.Empty)
            return false;

        if (guidDictionary.ContainsKey(guid))
            return false;

        guidDictionary.Add(guid, comp);
        return true;
    }

    /// <summary>
    /// Elimina el GUID que le pasamos del diccionario.
    /// </summary>
    /// <param name="guid">El GUID que queremos eliminar.</param>
    public static void Remove(Guid guid)
    {
        if (guidDictionary.ContainsKey(guid))
            guidDictionary.Remove(guid);
    }
}