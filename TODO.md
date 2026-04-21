# Project TODO - OpenDAoC SPB (Amtenael)

## High Priority (Completed)
- [x] Enable Cross-Realm grouping logic in `AmtenaelRules`.
- [x] Bypass client-side invitation filters using `CustomDialog`.
- [x] Implement `/gjoin` command for instant grouping bypass.
- [x] Fix visibility issues in the group UI for cross-realm members.

## Medium Priority (Future Improvements)
- [ ] **Confirmation Flow for /gjoin**: Add a `CustomDialog` on the target's side to allow them to accept or decline the direct join request.
- [ ] **Visual Popup for /gjoin**: Use a `CustomDialog` instead of just chat messages to make the grouping process more immersive.
- [ ] **Range/Region Settings**: Consider making the global range of `/gjoin` configurable via server properties.

## Maintenance
- [ ] Optimize server performance (Address `Long TimerService.Tick` warnings).
- [ ] Regular database indexing to speed up player and object lookups.

---
*Created by Antigravity AI - April 2026*
