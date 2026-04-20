namespace DOL;

/// <summary>
/// The different game server types
/// </summary>
public enum EGameServerType
{
    /// <summary>
    /// Normal server
    /// </summary>
    GST_Normal = 0,
    /// <summary>
    /// Test server
    /// </summary>
    GST_Test = 1,
    /// <summary>
    /// Player vs Player
    /// </summary>
    GST_PvP = 2,
    /// <summary>
    /// Player vs Monsters
    /// </summary>
    GST_PvE = 3,
    /// <summary>
    /// Roleplaying server
    /// </summary>
    GST_Roleplay = 4,
    /// <summary>
    /// Casual server
    /// </summary>
    GST_Casual = 5,
    /// <summary>
    /// Unknown server type
    /// </summary>
    GST_Unknown = 6,
    /// <summary>
    /// Guild vs Guild mode
    /// </summary>
    GST_GvG = 8,
    /// <summary>
    /// Amtenael custom mode
    /// </summary>
    GST_Amtenael = 9,
}