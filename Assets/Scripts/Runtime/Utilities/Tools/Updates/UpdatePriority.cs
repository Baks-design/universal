namespace Universal.Runtime.Utilities.Tools.Updates
{
    public enum UpdatePriority
    {
        Early,      // Input, network, game state
        Normal,     // Core gameplay
        Late,       // Camera, physics reactions
        UI          // Rendering, HUD
    }
}