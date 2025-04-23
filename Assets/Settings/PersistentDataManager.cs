using UnityEngine;

public class PersistentDataManager : MonoBehaviour
{
    private const string INSTANCE_GO_NAME = "[Persistent]GameDataManager";

    private static PersistentDataManager instance;

    private static PersistentDataManager GetInstance() 
    {
        if (instance == null) 
        {
            GameObject go = new GameObject();
            go.name = INSTANCE_GO_NAME;
            DontDestroyOnLoad(go);
            instance = go.AddComponent<PersistentDataManager>();
        }
        return instance;
    }
}
