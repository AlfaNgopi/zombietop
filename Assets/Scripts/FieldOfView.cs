using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius = 5f;
    [Range(0, 360)] public float viewAngle = 90f;

    public LayerMask targetMask; // Set this to "Enemy" layer
    public LayerMask obstructionMask; // Set this to "Walls" layer

    // debug vars
    public bool showFOV = true;
    public Color fovColor = Color.yellow;

    void Update()
    {

        FindVisibleTargets();
        if (showFOV) DrawFOV();

    }

    void DrawFOV()
    {
        // Optional: Visualize the FOV in the editor
        Vector3 leftBoundary = Quaternion.Euler(0, 0, -viewAngle / 2) * transform.up * viewRadius;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, viewAngle / 2) * transform.up * viewRadius;

        Debug.DrawLine(transform.position, transform.position + leftBoundary, fovColor);
        Debug.DrawLine(transform.position, transform.position + rightBoundary, fovColor);
    }


    void FindVisibleTargets()
    {
        // Find all colliders in radius
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);

        foreach (Collider2D target in targetsInViewRadius)
        {
            Transform targetTransform = target.transform;
            Vector2 dirToTarget = (targetTransform.position - transform.position).normalized;

            // Check if target is within the FOV angle
            if (Vector2.Angle(transform.up, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector2.Distance(transform.position, targetTransform.position);

                // Raycast to check for obstructions (Walls)
                if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstructionMask))
                {
                    // Target is visible! 
                    SetEnemyVisibility(targetTransform, true);
                }
                else { SetEnemyVisibility(targetTransform, false); }
            }
            else { SetEnemyVisibility(targetTransform, false); }
        }
    }

    void SetEnemyVisibility(Transform enemy, bool isVisible)
    {
        var renderer = enemy.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            // Target alpha: 1 if visible, 0 if hidden
            float targetAlpha = isVisible ? 1f : 0f;
            Color curColor = renderer.color;

            // Smoothly fade the enemy in/out
            renderer.color = Color.Lerp(curColor, new Color(curColor.r, curColor.g, curColor.b, targetAlpha), Time.deltaTime * 10f);
        }
    }
}