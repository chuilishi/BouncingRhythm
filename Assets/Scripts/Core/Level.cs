using ChartNamespace;
using UI;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Level", menuName = "Level")]
    public class Level : ScriptableObject
    {
        public Chart chart;
        public AudioClip audioClip;
        public IBackground background;
    }
}
