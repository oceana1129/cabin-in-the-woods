using UnityEngine;

[ExecuteAlways] // Allows gizmos to draw in Edit mode
public class ColliderGizmoDrawer : MonoBehaviour
{
    public Color gizmoColor = new Color(0f, 1f, 0f, 0.25f); 
    public bool drawWireframe = false;
    public bool drawFilled = true;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        // Box 
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
        {
            DrawBoxColliderGizmo(box);
        }

        // Sphere 
        SphereCollider sphere = GetComponent<SphereCollider>();
        if (sphere != null)
        {
            DrawSphereColliderGizmo(sphere);
        }

        // Capsule 
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        if (capsule != null)
        {
            DrawCapsuleColliderGizmo(capsule);
        }
    }

    private void DrawBoxColliderGizmo(BoxCollider box)
    {
        Matrix4x4 oldMatrix = Gizmos.matrix;

        Transform t = box.transform;
        Gizmos.matrix = Matrix4x4.TRS(
            t.position,
            t.rotation,
            t.lossyScale
        );

        if (drawFilled)
            Gizmos.DrawCube(box.center, box.size);

        if (drawWireframe)
            Gizmos.DrawWireCube(box.center, box.size);

        Gizmos.matrix = oldMatrix;
    }

    private void DrawSphereColliderGizmo(SphereCollider sphere)
    {
        Vector3 center = sphere.transform.TransformPoint(sphere.center);
        float radius = sphere.radius * MaxAbsComponent(sphere.transform.lossyScale);
        if (drawFilled) Gizmos.DrawSphere(center, radius);
        if (drawWireframe) Gizmos.DrawWireSphere(center, radius);
    }

    private void DrawCapsuleColliderGizmo(CapsuleCollider capsule)
    {
        Vector3 center = capsule.transform.TransformPoint(capsule.center);
        float radius = capsule.radius * MaxAbsComponent(capsule.transform.lossyScale);
        if (drawFilled) Gizmos.DrawSphere(center, radius);
        if (drawWireframe) Gizmos.DrawWireSphere(center, radius);
    }

    private float MaxAbsComponent(Vector3 v)
    {
        return Mathf.Max(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    }
}
