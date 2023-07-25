using System;
using System.Collections;
using UnityEngine;

public static class GameObjectExtensions
{
    private static UnityEventBehaviour _unityEventBehaviour = null;
    private static UnityEventBehaviour unityEventBehaviour
    {
        get
        {
            if (_unityEventBehaviour == null)
                _unityEventBehaviour = new GameObject(nameof(UnityEventBehaviour)).AddComponent<UnityEventBehaviour>();
            return _unityEventBehaviour;
        }
    }

    public static GameObject InstantiateWithDestroyTimer(this GameObject prefab, float destroyTime, Vector3 position, Quaternion rotation)
    {
        var obj = UnityEngine.Object.Instantiate(prefab, position, rotation);
        obj.SetActive(true);
        unityEventBehaviour.StartCoroutine(WaitActionCoroutine(destroyTime, () => UnityEngine.Object.Destroy(obj)));
        return obj;
    }

    public static IEnumerator WaitActionCoroutine(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}

public class UnityEventBehaviour : MonoBehaviour { }
