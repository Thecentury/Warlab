
namespace ScientificStudio.Charting
{
    public interface IGraphicalObject
    {
		/// <summary>
		/// Sets the viewport.
		/// </summary>
		/// <param name="viewport">The viewport.</param>
        void SetViewport(Viewport2D viewport);
		/// <summary>
		/// Detaches the viewport.
		/// </summary>
        void DetachViewport();
    }
}
