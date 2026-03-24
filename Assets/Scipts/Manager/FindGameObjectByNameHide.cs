
using System.Collections.Generic;
using UnityEngine;

public class FindGameObjectByNameHide
{
    private static Dictionary<string, GameObject> gameObjectCache = new Dictionary<string, GameObject>();

    public static GameObject FindGameObjectByName(string name)
    {
        // Kiểm tra trong cache
        if (gameObjectCache.TryGetValue(name, out GameObject cachedObject))
        {
            // Kiểm tra xem đối tượng còn tồn tại không
            if (cachedObject != null)
            {
                return cachedObject;
            }
            // Nếu đối tượng đã bị hủy, xóa khỏi cache
            gameObjectCache.Remove(name);
        }

        // Tìm đối tượng mới
        Transform[] transforms = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform t in transforms)
        {
            if (t.hideFlags == HideFlags.None && t.name == name)
            {
                // Thêm vào cache
                gameObjectCache[name] = t.gameObject;
                return t.gameObject;
            }
        }

        return null;
    }
}
