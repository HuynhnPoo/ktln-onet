using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonBase<T> :MonoBehaviour where T: MonoBehaviour 
{
    private static T instance;
    public static T Instance
    {
        get 
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    instance = obj.GetComponent<T>();
                }
            }
            return instance;
        }
      
    }

    protected virtual void Awake()
    {
        if (instance != null) Destroy(this);
        else
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
    }

}