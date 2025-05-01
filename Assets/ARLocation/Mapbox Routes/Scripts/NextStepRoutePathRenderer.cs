using System.Collections.Generic;
using UnityEngine;

namespace ARLocation.MapboxRoutes
{
    public class NextStepRoutePathRenderer : AbstractRouteRenderer
    {
        [System.Serializable]
        public class SettingsData
        {
            [Tooltip("The material used to render the line.")]
            public Material LineMaterial;

            [Tooltip("A texture offset factor used to move the line's texture as to appear that a pattern is moving. If you don't need this just set it to '0'.")]
            public float TextureOffsetFactor = -4.0f;

            [Tooltip("The prefab used to render an arrow pointing from the user to the next step.")]
            public GameObject ArrowPrefab;

            [Tooltip("If true, draws a connecting line in addition to arrows.")]
            public bool DrawLine = false;

            [Tooltip("Maximum number of arrows to keep in the pool.")]
            public int MaxPoolSize = 50;

            [Tooltip("How quickly arrows interpolate to their new positions.")]
            public float SmoothSpeed = 5f;

            [Tooltip("Distance between arrows in meters.")]
            public float ArrowSpacing = 2.0f;

            [Tooltip("Vertical offset to prevent clipping into the ground.")]
            public float ArrowYOffset = 0.05f;
        }

        public SettingsData Settings = new SettingsData();

        // Internal state
        private class State
        {
            public GameObject Parent;
            public LineRenderer LineRenderer;
        }
        private State state = new State();

        // Arrow‐pool & target positions
        private List<GameObject> arrowPool = new List<GameObject>();
        private Vector3[] targetPositions;
        private Vector3 flatDirection;
        private Transform camTransform;

        public override void Init(RoutePathRendererArgs args) { /* no special init needed */ }

        private void OnEnable()
        {
            // Cache camera transform (avoids repeated Camera.main lookups)
            camTransform = Camera.main != null ? Camera.main.transform : transform;

            // Create parent GameObject + LineRenderer
            state.Parent = new GameObject("[NextStepRoutePathRenderer]");
            state.LineRenderer = state.Parent.AddComponent<LineRenderer>();
            state.LineRenderer.startWidth = 0.25f;
            state.LineRenderer.useWorldSpace = true;
            state.LineRenderer.alignment = LineAlignment.View;
            state.LineRenderer.textureMode = LineTextureMode.Tile;
            state.LineRenderer.numCornerVertices = 2;
            state.LineRenderer.sharedMaterial = Settings.LineMaterial;

            // Prepare empty pool
            arrowPool.Clear();
        }

        private void OnDisable()
        {
            if (state.Parent != null)
                Destroy(state.Parent);
            state.Parent = null;

            arrowPool.Clear();
            targetPositions = null;
        }

        private void OnDestroy() => OnDisable();

        public override void OnRouteUpdate(RoutePathRendererArgs args)
        {
            // Compute horizontal‐plane positions at camera‐height minus ground
            float groundH = args.Route.Settings.GroundHeight;
            float y = camTransform.position.y - groundH;
            Vector3 userPos   = MathUtils.SetY(args.UserPos,   y);
            Vector3 targetPos = MathUtils.SetY(args.TargetPos, y);

            // Flatten direction onto XZ plane
            Vector3 dir = (targetPos - userPos).normalized;
            flatDirection = new Vector3(dir.x, 0f, dir.z).normalized;

            // Determine how many arrows
            float dist = Vector3.Distance(userPos, targetPos);
            int count = Mathf.FloorToInt(dist / Settings.ArrowSpacing);

            // Expand pool on demand (but never exceed MaxPoolSize)
            for (int i = arrowPool.Count; i < count && arrowPool.Count < Settings.MaxPoolSize; i++)
            {
                var a = Instantiate(Settings.ArrowPrefab, state.Parent.transform);
                a.name = "[RouteArrow]";
                a.SetActive(false);
                arrowPool.Add(a);
            }

            // Compute all target positions (starting at 1*spacing to avoid feet)
            targetPositions = new Vector3[count];
            for (int i = 0; i < count; i++)
            {
                Vector3 p = userPos + flatDirection * Settings.ArrowSpacing * (i + 1);
                p.y += Settings.ArrowYOffset;
                targetPositions[i] = p;
            }

            // Activate/mask pool entries
            for (int i = 0; i < arrowPool.Count; i++)
            {
                arrowPool[i].SetActive(i < count);
            }

            // Update (or hide) the line
            if (Settings.DrawLine)
            {
                state.LineRenderer.enabled = true;
                state.LineRenderer.positionCount = 2;
                state.LineRenderer.SetPosition(0, userPos);
                state.LineRenderer.SetPosition(1, targetPos);
                state.LineRenderer.material.SetTextureOffset(
                    "_MainTex",
                    new Vector2(Settings.TextureOffsetFactor * args.Distance, 0f)
                );
            }
            else if (state.LineRenderer.enabled)
            {
                state.LineRenderer.enabled = false;
            }
        }

// somewhere in your class
private readonly Quaternion arrowOffset = Quaternion.Euler(0, 90, 0);

     private void Update()
{
    // Smoothly lerp each active arrow toward its latest target
    if (targetPositions != null)
    {
        int len = Mathf.Min(targetPositions.Length, arrowPool.Count);
        for (int i = 0; i < len; i++)
        {
            var arrow = arrowPool[i];
            if (!arrow.activeSelf) continue;

            // Position interpolation
            arrow.transform.position = Vector3.Lerp(
                arrow.transform.position,
                targetPositions[i],
                Time.deltaTime * Settings.SmoothSpeed
            );

            // Compute the base look rotation once
            Quaternion look = Quaternion.LookRotation(flatDirection, Vector3.up);

            // Slerp toward look + your model offset
            arrow.transform.rotation = Quaternion.Slerp(
                arrow.transform.rotation,
                look * arrowOffset,
                Time.deltaTime * Settings.SmoothSpeed
            );
        }
    }

    // Optional: continuous texture scroll for the line
    if (Settings.DrawLine && state.LineRenderer.enabled)
    {
        float offset = Time.time * -Settings.TextureOffsetFactor;
        state.LineRenderer.material.SetTextureOffset("_MainTex", new Vector2(offset, 0f));
    }
}

    }
}
