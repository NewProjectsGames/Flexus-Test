using Flexus.Interfaces;
using UnityEngine;

namespace Flexus.Visual
{
    /// <summary>
    /// Paints a pre-defined decal texture onto a surface hit by a projectile.
    /// Implements <see cref="IDecalService"/> so the <see cref="Flexus.Projectile.Projectile"/>
    /// component only depends on the interface, not this MonoBehaviour.
    ///
    /// <para>
    /// The painting is done by reading the surface's UV at the hit point,
    /// writing the decal pixels over the material's main texture, and uploading
    /// the result back to the GPU via <c>Texture2D.Apply()</c>.
    /// </para>
    /// </summary>
    public sealed class DecalPainter : MonoBehaviour, IDecalService
    {
        // ── Inspector ────────────────────────────────────────────────────

        [Tooltip("Source decal texture; must be Read/Write enabled in Import Settings.")]
        [SerializeField] private Texture2D decalTexture;

        [Tooltip("Width and height of the painted decal region in pixels.")]
        [SerializeField] private Vector2Int decalSizePixels = new(90, 90);

        // ── IDecalService ────────────────────────────────────────────────

        /// <inheritdoc/>
        public void PaintDecal(RaycastHit hit)
        {
            Renderer surfaceRenderer = hit.transform.GetComponent<Renderer>();
            if (surfaceRenderer == null) return;

            Texture2D mainTexture = EnsureReadableTexture(surfaceRenderer.material);
            if (mainTexture == null) return;

            Vector2 uv = hit.textureCoord;
            int originX = Mathf.FloorToInt(uv.x * mainTexture.width);
            int originY = Mathf.FloorToInt(uv.y * mainTexture.height);

            // Bail out if the decal would exceed texture boundaries.
            if (originX + decalSizePixels.x > mainTexture.width  ||
                originY + decalSizePixels.y > mainTexture.height)
                return;

            BlendDecalIntoTexture(mainTexture, originX, originY);
        }

        // ── Private helpers ──────────────────────────────────────────────

        /// <summary>
        /// Returns the material's main texture if it is already a readable
        /// <see cref="Texture2D"/>; otherwise creates a blank white texture and assigns it.
        /// </summary>
        private static Texture2D EnsureReadableTexture(Material material)
        {
            if (material.mainTexture is Texture2D existing)
                return existing;

            // Create a 1024×1024 white fallback and assign it to the material.
            Texture2D blank = new(1024, 1024, TextureFormat.RGBA32, mipChain: false, linear: true);
            Color32[] fill  = new Color32[blank.width * blank.height];
            System.Array.Fill(fill, new Color32(255, 255, 255, 255));
            blank.SetPixels32(fill);
            blank.Apply(false);
            material.mainTexture = blank;

            return material.mainTexture as Texture2D;
        }

        /// <summary>
        /// Samples the decal texture at the correct scale and alpha-blends it
        /// over the region of <paramref name="mainTexture"/> starting at
        /// (<paramref name="originX"/>, <paramref name="originY"/>).
        /// </summary>
        private void BlendDecalIntoTexture(Texture2D mainTexture, int originX, int originY)
        {
            int w = decalSizePixels.x;
            int h = decalSizePixels.y;

            Color[] surface = mainTexture.GetPixels(originX, originY, w, h);
            Color[] decal   = decalTexture.GetPixels();

            for (int row = 0; row < h; row++)
            {
                for (int col = 0; col < w; col++)
                {
                    // Map destination pixel to source decal UV.
                    int srcX  = (int)((float)col / w * decalTexture.width);
                    int srcY  = (int)((float)row / h * decalTexture.height);
                    int srcIdx = srcY * decalTexture.width + srcX;

                    // Only overwrite pixels where the decal is opaque.
                    if (decal[srcIdx].a > 0f)
                        surface[row * w + col] = decal[srcIdx];
                }
            }

            mainTexture.SetPixels(originX, originY, w, h, surface);
            mainTexture.Apply(false);
        }
    }
}
