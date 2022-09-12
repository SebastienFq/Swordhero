using UnityEditor;
 
public class CreateDataUtility
{
    [MenuItem("Assets/Create/Data/CameraData")]
    public static void CreateAssetCameraData()
    {
        ScriptableObjectUtility.CreateAsset<CameraData>();
    }
}