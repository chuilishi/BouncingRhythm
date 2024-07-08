using UnityEngine;

namespace UI
{
    public class Background_Moving_ver : IBackground
    {
        public float speed = 1f;
        public Vector2 direction = Vector2.right;
        private float curOffsetX = 0;
        private float curOffsetY = 0;
        public Shader shader;
        private Material material;
        private static readonly int OffsetX = Shader.PropertyToID("_OffsetX");
        private static readonly int OffsetY = Shader.PropertyToID("_OffsetY");
        private static readonly int RepeatX = Shader.PropertyToID("_RepeatX");
        private static readonly int RepeatY = Shader.PropertyToID("_RepeatY");

        private void Awake()
        {
            material = GetComponent<SpriteRenderer>().material = new Material(shader);
            material.SetFloat(RepeatX,10f);
            material.SetFloat(RepeatY,10f);
        }
        
        public override void OnUpdate()
        {
            curOffsetX += speed / 100f * direction.x * Time.deltaTime;
            curOffsetY += speed / 100f * direction.y * Time.deltaTime;
            material.SetFloat(OffsetX,curOffsetX);
            material.SetFloat(OffsetY,curOffsetY);
        }
    }
}