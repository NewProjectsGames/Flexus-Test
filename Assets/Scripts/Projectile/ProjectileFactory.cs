using UnityEngine;
using Zenject;

namespace Flexus.Projectile
{
    /// <summary>
    /// Factory responsible for constructing projectile <see cref="GameObject"/>s with
    /// procedurally-generated, randomised cube meshes.
    ///
    /// <para>
    /// Applies the <b>Factory Method</b> pattern: callers request a projectile without
    /// knowing how its geometry is built, and the factory owns all mesh creation details.
    /// </para>
    /// </summary>
    public static class ProjectileFactory
    {
        // ── Cube vertex layout (right-hand, Y-up) ────────────────────────
        //
        //    7 ──── 6
        //   /|     /|
        //  4 ──── 5 |
        //  | 3 ── | 2
        //  |/     |/
        //  0 ──── 1
        //
        // Each face is two counter-clockwise triangles when seen from outside.

        private static readonly Vector3[] BaseVertices =
        {
            new(-1, -1, -1), new( 1, -1, -1),
            new( 1,  1, -1), new(-1,  1, -1),
            new(-1, -1,  1), new( 1, -1,  1),
            new( 1,  1,  1), new(-1,  1,  1),
        };

        private static readonly int[] CubeTriangles =
        {
            0, 2, 1,  0, 3, 2,   // back  face
            4, 5, 6,  4, 6, 7,   // front face
            0, 5, 4,  0, 1, 5,   // bottom face
            1, 2, 6,  1, 6, 5,   // right  face
            2, 3, 6,  6, 3, 7,   // top    face
            0, 7, 3,  0, 4, 7,   // left   face
        };

        // ── Public API ───────────────────────────────────────────────────

        /// <summary>
        /// Instantiates <paramref name="prefab"/>, attaches a randomly-jittered cube
        /// mesh and the supplied <paramref name="material"/>, then scales it down by half.
        /// </summary>
        /// <param name="prefab">Base prefab that must already carry a <see cref="Projectile"/> component.</param>
        /// <param name="material">Material applied to the generated <see cref="MeshRenderer"/>.</param>
        /// <param name="jitterAmount">Maximum positional offset applied to each vertex (adds organic look).</param>
        /// <returns>Inactive, configured projectile <see cref="GameObject"/>.</returns>
        public static GameObject Create(DiContainer container, GameObject prefab, Material material, float jitterAmount = 0.2f)
        {
            GameObject go = container != null ? container.InstantiatePrefab(prefab) : Object.Instantiate(prefab);
            go.SetActive(false);

            Mesh mesh = BuildJitteredCube(jitterAmount);

            if (!go.TryGetComponent(out MeshFilter filter))
                filter = go.AddComponent<MeshFilter>();
                
            if (!go.TryGetComponent(out MeshRenderer renderer))
                renderer = go.AddComponent<MeshRenderer>();

            filter.sharedMesh = mesh;
            renderer.sharedMaterial = material;
            go.transform.localScale /= 2f;

            return go;
        }

        // ── Private helpers ──────────────────────────────────────────────

        public static void RandomizeMesh(Mesh mesh, float jitter)
        {
            Vector3[] vertices = new Vector3[BaseVertices.Length];
            
            // Generate random axis modifiers to create elongated or flattened shapes natively in the mesh
            Vector3 shapeModifier = new Vector3(
                Random.Range(0.4f, 1.6f),
                Random.Range(0.4f, 1.6f),
                Random.Range(0.4f, 1.6f)
            );

            for (int i = 0; i < BaseVertices.Length; i++)
            {
                // Scale the base vertex and then apply the random jitter
                vertices[i] = new Vector3(
                    BaseVertices[i].x * shapeModifier.x + Random.Range(-jitter, jitter),
                    BaseVertices[i].y * shapeModifier.y + Random.Range(-jitter, jitter),
                    BaseVertices[i].z * shapeModifier.z + Random.Range(-jitter, jitter)
                );
            }
            
            mesh.vertices = vertices;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }

        /// <summary>
        /// Clones the base cube vertex positions, applies random jitter to each,
        /// and returns a fully-configured <see cref="Mesh"/>.
        /// </summary>
        private static Mesh BuildJitteredCube(float jitter)
        {
            Mesh mesh = new()
            {
                name = "Projectile_Jittered",
                vertices  = BaseVertices,
                triangles = CubeTriangles,
            };

            RandomizeMesh(mesh, jitter);
            return mesh;
        }
    }
}
