#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using ChartNamespace;

public class ChartToLevel : MonoBehaviour
{
    public Chart chart;
    public LineRenderer lineRenderer;
    public Vector2 initPos = Vector2.zero;
    public GameObject parent;
    
    [Button]
    public void Convert()
    {
        if (lineRenderer == null) return;

        if (parent == null)
        {
            parent = new GameObject("Level");
        }
        
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0,initPos);

        // 第一个key是工具key用于方便初始化
        var firstKey = chart.keys[0];
        firstKey.keyType = KeyType.LeftSingle;
        firstKey.position = initPos;
        firstKey.direction = Vector2.up;
        firstKey.ball = Instantiate(GameSettings.Instance.KeyTypeToPrefab[firstKey.keyType], firstKey.position, Quaternion.identity,parent.transform);
        firstKey.ball.GetComponent<SpriteRenderer>().color = Color.clear;
        
        
        for (int i = 1; i < chart.keys.Count-1; i++)
        {
            var key = chart.keys[i];
            var lastKey = chart.keys[i-1];
            key.position = lastKey.position + lastKey.direction * (key.Time - lastKey.Time) * lastKey.speed;
            key.ball = Instantiate(GameSettings.Instance.KeyTypeToPrefab[key.keyType], key.position, Quaternion.identity,parent.transform);
            if (chart.keys[i-1].keyType is KeyType.LeftDouble or KeyType.RightDouble or KeyType.RightSimultaneous)
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount-1,key.position);
            }
            else
            {
                lineRenderer.SetPosition(lineRenderer.positionCount-1,key.position);
            }
        }
        AssetDatabase.SaveAssets();
    }
}
#endif