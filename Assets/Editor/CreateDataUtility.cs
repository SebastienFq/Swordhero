using UnityEditor;
 
public class CreateDataUtility
{
    [MenuItem("Assets/Create/Data/CameraData")]
    public static void CreateAssetCameraData()
    {
        ScriptableObjectUtility.CreateAsset<CameraData>();
    }

    [MenuItem("Assets/Create/Data/WeaponData")]
    public static void CreateAssetWeaponData()
    {
        ScriptableObjectUtility.CreateAsset<WeaponData>();
    }

    [MenuItem("Assets/Create/Data/WeightedItemData")]
    public static void CreateAssetWeightedItemData()
    {
        ScriptableObjectUtility.CreateAsset<WeightedItemData>();
    }

}