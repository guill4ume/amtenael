# Technical Documentation: Cross-Realm Grouping Bypass

This feature was implemented to allow players of different realms to form groups on the Amtenael/OpenDAoC-SPB server, specifically targeting compatibility with Client version 1.127.

## Problem Statement
The standard DAoC client (especially 1.127) blocks grouping with "enemies" (different realms) at several levels:
1.  **Incoming Invitation**: The client filters standard group invitation packets (0x1F) and won't display them if the sender is an enemy.
2.  **Invitation Response**: Even if shown, clicking "Accept" on a cross-realm prompt might be blocked or ignored by the client's internal security logic.

## Implementation Details

### 1. Custom Dialog Bypass
Instead of the native Group Invite packet, we use a `CustomDialog` (Packet 0x06).
- **Packet Lib**: `PacketLib168.SendCustomDialog`.
- **Mechanism**: This packet bypasses the client's realm-filter because it is treated as a generic server dialog rather than a specific "social" action like grouping.
- **Session Mapping**: We send the *target's* own SessionID in the `data1` field of the packet. In 1.127, this ensures the client thinks the dialog relates to itself, further reducing security filtering.

### 2. Group Persistence Rules
In `AmtenaelRules.cs` (or any `PvPServerRules`), we override `IsAllowedToGroup`.
- **Condition**: Logic was added to allow grouping regardless of realm, EXCEPT when in RvR regions (Frontiers) to prevent cross-realm grouping in competitive PvP areas.

### 3. Visual Synchronization
Modified `Group.cs` and `PacketLib168.SendGroupWindowUpdate`.
- **Anonymity**: Group members from different realms are visible in the UI, but they may appear as anonymous or maintain their respective realm visual indicators depending on the ruleset.

### 4. Direct Join Command: `/gjoin`
Since the UI response (0x0F) can still be unreliable due to lag or client-side timing out, a programmatic fallback was added.
- **Command**: `/gjoin <TargetPlayerName>`
- **Logic**: Directly calls `AmtenaelRules.ForceJoin`. It performs security checks (grouping allowed by ruleset) and then manually adds the caller to the target's group without requiring a dialog.
- **Range**: **Global**. No distance or region restriction (except the RvR grouping ruleset).
- **Reference**: `GameServer\scripts\commands\gjoin.cs`.

## Maintenance
- **Files involved**:
    - `AmtenaelRules.cs`: Ruleset and `ForceJoin` logic.
    - `gjoin.cs`: Command handler.
    - `PacketProcessor.cs`: Received packet entry point.
    - `DialogResponseHandler.cs`: Response decoder.

---
*Created by Antigravity AI - April 2026*
