# v4 Design Document — "Interesting Pickups"

**Project codename:** Elenor
**Version scope:** v4
**Status:** Spec-complete, ready for Phase 1 implementation

---

## Purpose of this document

This is the canonical design spec for v4. It exists to prevent design-amnesia — when v5-you wants to "just add a max-HP pickup real quick," this document is what reminds past-you's reasoning so the v4 pillars don't erode quietly.

Read top to bottom for full context. Grep by pickup name when implementing.

---

## Player character

| Field | Value |
|---|---|
| Name | Elenor |
| Reference | Quiet female high schooler, Persona 3 Makoto Yuki energy |
| Sprite status | TBD (Phase 4 — placeholder pack or hand-drawn) |
| Baseline HP | 6 |
| Baseline abilities | Movement (WASD), Shoot (mouse/arrows), Dash (Space, with i-frames) |

---

## Locked decisions

These are committed for v4. Changing any of them mid-implementation invalidates downstream work.

### Pickup system model

- **Stacking:** Levelable. Picking up a duplicate increases level. Max level 3.
- **Categories:** Behavior modifiers + one stat tradeoff (PowerSize). No pure stat-up pickups.
- **Drop pool:** Per-floor rarity tiers. F1 = Common only. F2 = Common + Rare. F3 = Common + Rare + Legendary.
- **Choice rooms:** Deferred to v5+. v4 uses standard random drops only.
- **Implementation pattern:** Behavior modifier components on the Player GameObject (Option A from architectural discussion). Player systems query their relevant modifier components at runtime.
- **Reset on death:** Automatic — Player GameObject reset destroys the components.
- **Maxed pickups:** When a drop selects an L3 pickup, reroll. Implementation lives in `RoomController.SpawnReward`.

### Active abilities

- **Deferred to v5.** v4 is passive pickups only.
- **Dash remains a default ability** in v4. Modified (not granted) by Pickup_DashMod when held.
- The active-ability *system* is not built in v4. Designing it later, after v4's passive baseline is dialed in, will produce better active-ability designs.

### HP and survivability

- **No max-HP pickup in v4.** Deliberate. Survivability comes from Shield + dash skill + iframe management, not from raising the HP ceiling.
- This is a v4 pillar. If v5+ wants to add max-HP scaling, that's a deliberate change to the survivability model — not a quick fix.

### Damage and i-frames

- **Dash i-frames are unchanged from base** in v4. DashMod does not add or modify i-frame duration. Otherwise Shield + DashMod becomes degenerate.
- **Shield damage interception happens BEFORE i-frame logic.** Shield-break does NOT set the standard `_invulnUntil` window. Otherwise Shield = ~0.5s of effective immortality after each break.
- **Shield-break grants ~0.1s mini-iframes** to prevent same-physics-step pile-on (multiple simultaneous contacts breaking shield AND draining HP in one step). Shorter than normal i-frames — enough to prevent stagger, not enough to extend Shield's value.

### Damage attribution (for DashMod L3 refund)

- **Dash registers itself as an active damage source for its duration.** Kills during the dash window check active sources to determine attribution.
- **Refund triggers on:** dash damage kills, MachineGun bullet kills, ChargeShot kills, any direct shot kill — anything attributable to a player damage source during the dash window.
- **Refund does NOT trigger on:** poison ticks, environmental damage (none in v4), or any indirect damage. Prevents degenerate "poison everything, dash forever" loop.

### PowerSize hitbox implementation

- Hitbox change scales `transform.localScale` on the Player. This scales sprite and collider together with one value. Visual-and-mechanical effect from a single field.

### Pickup acquisition feedback

- **On pickup:** Brief notification appears ("STUCK PEN — Lv2!" or similar). Phase 1 scaffolds the system.
- **HUD update:** Icon glow / level-up flash when a pickup is acquired or leveled up. Phase 3 polishes the visuals.
- Without these, pickups feel ghostly. Critical for the keystone-of-v4 status to land.

---

## Pickup catalog

Seven pickups, ordered by tier.

### Pickup_MachineGun

| Field | Value |
|---|---|
| Display Name | "Stuck Pen" |
| Flavor Text | "It won't stop clicking. Click click click click." |
| Rarity | Common |
| Available From | F1 |
| Sprite | TBD |

**Mechanical Effect:**
Modifies the player's projectile spawning. Each shot input produces multiple smaller, faster projectiles fired in rapid succession instead of one larger projectile. Total damage-per-second increases at higher levels; per-shot damage decreases. Changes the *texture* of combat — more bullets on screen, finer aim required, but less commitment per shot.

**Levels:**
- **L1:** Fire rate ×2, damage per shot ×0.5. Net DPS unchanged. Bullets visibly smaller. (L1 is intentionally a feel-change, not a power-change — tutorial-shaped pickup that introduces the texture.)
- **L2:** Fire rate ×2.5, damage per shot ×0.55. Net DPS slightly up.
- **L3:** Fire rate ×3, damage per shot ×0.6. Net DPS noticeably up. Bullets clearly smaller, denser stream.

**Composes With:**
- **Pickup_Poison:** Many more poison applications per second; refreshes constantly on a hit target.
- **Pickup_DualStream:** Multiplies total bullet output. L3 of both = wall of bullets.
- **Pickup_PowerSize:** Larger hitbox makes higher fire rate easier to compensate for via volume.
- **Pickup_ChargeShot:** Tap-fire = MachineGun spray. Hold-fire = full ChargeShot. Both identities preserved by input shape.
- **Pickup_DashMod:** MachineGun kills during dash window count toward L3 refund (see Damage attribution).

**Edge Cases / Notes:**
- ChargeShot conflict resolution: tap = MachineGun, hold = ChargeShot. Both pickups remain useful when held simultaneously; player chooses tactical mode by input shape.
- Fire rate cap should respect a minimum cooldown so the engine doesn't spawn N projectiles in one frame. Phase 1 sanity check.

---

### Pickup_Poison

| Field | Value |
|---|---|
| Display Name | "Cafeteria Mystery" |
| Flavor Text | "Nobody asked what's in it." |
| Rarity | Common |
| Available From | F1 |
| Sprite | TBD |

**Mechanical Effect:**
Adds a poison status effect to all player projectiles. Enemies hit by a player projectile take damage-over-time for a duration in addition to the projectile's direct damage. Poison refreshes the timer on re-hit; it does not stack DPS. At L3, poisoned enemies that die spawn a small poison cloud at their position, briefly damaging other enemies in range.

**Levels:**
- **L1:** 1 damage per second for 2 seconds. (Total 2 damage per application. Whole-number ticks for readability.)
- **L2:** 2 damage per second for 4 seconds.
- **L3:** 3 damage per second for 5 seconds. Poisoned enemies spawn a small AoE poison cloud on death (radius small, duration ~2s, damage equal to L3 poison DPS).

**Composes With:**
- **Pickup_MachineGun:** Many more poison applications per second; refreshes constantly on a hit target.
- **Pickup_DualStream:** All streams apply poison; useful for poisoning multiple enemies at once.
- **Pickup_ChargeShot:** Charged shots apply poison normally — but with a much bigger projectile and pierce, a single charged shot can poison many enemies at once.
- **Pickup_PowerSize:** PowerSize's bigger projectiles cover more area per shot, so each shot poisons more clustered enemies.
- **Pickup_DashMod:** Dash damage does NOT apply poison (dash isn't a projectile). Worth noting because players will expect it to.

**Edge Cases / Notes:**
- Poison damage doesn't trigger DashMod L3 refund (kill must come from a direct player damage source, not a poison tick).
- L3 poison cloud only spawns from poisoned-enemy death, not from any death. Cloud doesn't damage Elenor.

---

### Pickup_DualStream

| Field | Value |
|---|---|
| Display Name | "Snapped Ruler" |
| Flavor Text | "Now it's two rulers. Sort of." |
| Rarity | Rare |
| Available From | F2 |
| Sprite | TBD |

**Mechanical Effect:**
Modifies projectile spawning. Each shot fires multiple projectiles in opposing directions simultaneously. All projectiles deal full damage. Changes positioning incentives — being in the center of a room becomes valuable, walls behind Elenor become useful for "wasted" stream coverage.

**Levels:**
- **L1:** 2 streams — forward (aimed) and backward (opposite of aim).
- **L2:** 3 streams — forward, backward, and a third stream pointing in the player's current movement direction. Defaults to up if stationary.
- **L3:** 4 streams — symmetric cross pattern *relative to aim* (forward, backward, perpendicular-left, perpendicular-right). Cross rotates with aim direction.

**Composes With:**
- **Pickup_MachineGun:** Multiplies total bullet output. L3 of both = wall of bullets.
- **Pickup_Poison:** Multiple streams = multiple poison vectors; useful in crowded rooms.
- **Pickup_ChargeShot:** Charged shots fire from all stream directions simultaneously. Each stream fires a full-multiplier charged shot (multiplier is NOT divided across streams).
- **Pickup_PowerSize:** PowerSize's bigger projectiles compound with multiple streams — more coverage per shot. L3 + L3 cross pattern becomes a wide AoE spread.

**Edge Cases / Notes:**
- L2 third-stream direction follows `PlayerMovement.LastNonZeroInput`. Defaults to up when stationary.
- L3 cross is aim-relative, not world-relative. Rotates with aim.
- All streams respect projectile modifiers identically (poison applies to all, charge multiplier applies to all per stream).
- Streams that hit nothing don't deal damage — no friendly fire concern, but they exist in world.

---

### Pickup_ChargeShot

| Field | Value |
|---|---|
| Display Name | "Old Lighter" |
| Flavor Text | "Mom said never to play with it." |
| Rarity | Rare |
| Available From | F2 |
| Sprite | TBD |

**Mechanical Effect:**
Modifies the player's shooting behavior. Holding the fire input charges a shot; releasing fires it. Charge time determines damage multiplier — fully charged shots are visibly larger and deal much more damage. Tapping fire (no hold) still fires a normal shot at base damage, so spam-fire still works. Held charge can swap aim direction before release without losing charge.

**Levels:**
- **L1:** Max charge time 1.0s. Fully charged shot: ×3 damage, larger projectile.
- **L2:** Max charge time 0.8s. Fully charged shot: ×4 damage, larger projectile, pierces 1 enemy.
- **L3:** Max charge time 0.6s. Fully charged shot: ×5 damage, larger projectile, pierces 2 enemies.

**Composes With:**
- **Pickup_Poison:** Charged shots apply poison normally; bigger projectile + pierce means a single charged shot can poison many enemies.
- **Pickup_DualStream:** Charged shot fires from all stream directions, full multiplier per stream.
- **Pickup_MachineGun:** Tap-fire = MachineGun spray. Hold-fire = full ChargeShot. Input shape determines tactical mode.
- **Pickup_PowerSize:** Damage multiplier compounds with PowerSize damage bonus; charged + PowerSize L3 = nuke.

**Edge Cases / Notes:**
- Damage scales linearly with charge time — half-charged shots do half the bonus damage. Not a flat threshold.
- Releasing fire while not at max charge still consumes the input — no "save the charge" mechanic.
- Pickup_DashMod is not a fire input, so dashing while charging doesn't break the charge. Phase 1 confirm.
- Damage tuning here is most likely to need adjustment in Phase 5.

---

### Pickup_PowerSize

| Field | Value |
|---|---|
| Display Name | "Energy Drink" |
| Flavor Text | "She'd never finished one before. Felt different." |
| Rarity | Rare |
| Available From | F2 |
| Sprite | TBD |

**Mechanical Effect:**
Modifies player stats with a tradeoff: increases damage output and movement speed, but also increases the player's hitbox size (which is sprite + collider, scaled together via `transform.localScale`). Pure tradeoff design — more offensive capability, easier to get hit.

**Levels:**
- **L1:** +25% damage, +15% speed, +20% hitbox size.
- **L2:** +50% damage, +25% speed, +35% hitbox size.
- **L3:** +85% damage, +40% speed, +55% hitbox size.

**Composes With:**
- **Pickup_Shield:** Shield mitigates the bigger-hitbox downside by giving a free hit each cycle. Shield + PowerSize is the canonical "aggressive build" pair.
- **Pickup_ChargeShot:** Damage bonus compounds with charge multiplier.
- **Pickup_DashMod:** Speed bonus + dash distance = high mobility, partially offsetting bigger hitbox.
- **Pickup_MachineGun:** Bigger hitbox is offset by higher rate-of-fire; volume compensates for vulnerability.
- **Pickup_Poison:** Bigger projectiles cover more area, so each shot poisons more clustered enemies.
- **Pickup_DualStream:** Bigger projectiles on multiple streams = significantly more coverage per shot.

**Edge Cases / Notes:**
- Hitbox change is visual *and* mechanical — Elenor's sprite scales so the player can read their own hitbox size.
- Speed bonus applies to base movement only, not to dash distance.
- Damage bonus applies multiplicatively to all damage sources (shots, dash damage, etc.).
- L3 hitbox at +55% is the starting tuning value. Phase 5 playtest at 55%, 40%, 70% to find the sweet spot.

---

### Pickup_DashMod

| Field | Value |
|---|---|
| Display Name | "Track Shoes" |
| Flavor Text | "From the lost and found. Someone fast." |
| Rarity | Rare |
| Available From | F2 |
| Sprite | TBD |

**Mechanical Effect:**
Modifies the default dash ability. Dash itself remains the same input (Space) and base cooldown — this pickup adds capabilities to it. At higher levels, dash becomes an offensive tool, not just a movement/i-frame tool.

**Levels:**
- **L1:** Dash deals damage to enemies passed through. Damage = 4× base shot damage.
- **L2:** L1 effect retained. Dash distance increased by ~40%.
- **L3:** L1 + L2 effects retained. If an enemy dies during the dash window from any direct player damage source (dash damage, shot kill, charged shot, etc.), dash cooldown refunds fully — chained dashes possible if you can keep killing.

**Composes With:**
- **Pickup_PowerSize:** Damage bonus applies to dash damage; speed + dash distance = very high mobility.
- **Pickup_MachineGun:** More kills per second = more L3 refund procs. MachineGun shot kills during dash window count toward refund.
- **Pickup_ChargeShot:** Charged shot kills during dash window count toward refund.

**Edge Cases / Notes:**
- Poison ticks killing enemies during dash window do NOT trigger L3 refund (only direct kills via player damage sources).
- Dash damage is NOT a projectile — doesn't apply poison, doesn't get DualStream'd, doesn't get charge-multiplied.
- Dash i-frames unchanged from base. DashMod adds no i-frames.
- L2 dash distance bonus is only active while DashMod L2+ is held.
- Implementation: dash registers as an active damage source for its duration; kills check active sources for attribution.

---

### Pickup_Shield

| Field | Value |
|---|---|
| Display Name | "Lucky Charm" |
| Flavor Text | "She kept it in her pocket. Always." |
| Rarity | Legendary |
| Available From | F3 |
| Sprite | TBD |

**Mechanical Effect:**
Player gains a regenerating shield that absorbs incoming damage. When hit, the shield breaks and Elenor takes no HP damage. After breaking, the shield regenerates after a fixed cooldown. Shield state is visually obvious — a sprite around Elenor when active, absent when broken.

**Levels:**
- **L1:** Absorbs 1 hit. Regenerates 12 seconds after breaking.
- **L2:** Absorbs 1 hit. Regenerates 8 seconds after breaking.
- **L3:** Absorbs 2 hits before fully breaking (visible damage to shield sprite after first hit). Regenerates 8 seconds after fully breaking.

**Composes With:**
- **Pickup_PowerSize:** Mitigates the bigger-hitbox downside; turns PowerSize from risky into nearly free. Canonical "aggressive build" pair.
- **All offensive pickups:** Survival floor lets Elenor play more aggressively without being punished for mistakes.

**Edge Cases / Notes:**
- Shield damage interception happens BEFORE i-frame logic. Shield-break does NOT set the standard `_invulnUntil` window.
- Shield-break grants ~0.1s mini-iframes to prevent same-physics-step pile-on. Shorter than normal i-frames.
- Shield does NOT protect against environmental damage (deferred — no env damage in v4).
- Shield regeneration timer pauses during room transitions and pause menu.
- Shield does not prevent knockback or hit reactions, only HP loss.
- Shield hit triggers hit-flicker animation for player feedback.

---

## Composition density matrix

Rows compose with columns. ✓ = explicit interaction documented. (—) = no notable interaction.

|  | MG | Poison | Dual | Charge | Power | Dash | Shield |
|---|---|---|---|---|---|---|---|
| **MachineGun** | — | ✓ | ✓ | ✓ | ✓ | ✓ | (implicit) |
| **Poison** | ✓ | — | ✓ | ✓ | ✓ | ✓ | (implicit) |
| **DualStream** | ✓ | ✓ | — | ✓ | ✓ | (—) | (implicit) |
| **ChargeShot** | ✓ | ✓ | ✓ | — | ✓ | (—) | (implicit) |
| **PowerSize** | ✓ | ✓ | ✓ | ✓ | — | ✓ | ✓ |
| **DashMod** | ✓ | ✓ | (—) | (—) | ✓ | — | (implicit) |
| **Shield** | (impl) | (impl) | (impl) | (impl) | ✓ | (impl) | — |

Density: ~70% of pairs have explicit interactions; remaining are implicit ("Shield enables aggressive play with X"). Strong build identity emerges from any 3+ pickup combination.

---

## Architectural commitments for Phase 1

These are implementation patterns that need to be locked when Phase 1 begins.

1. **Behavior modifier components on Player.** Pickup-on-acquisition adds a component to the Player GameObject. Player systems (PlayerShooter, PlayerDash, PlayerHealth) query for relevant components at runtime. Inspector-visible at runtime for debugging.
2. **Reset on death is automatic.** Player GameObject reset destroys components. No global registry to maintain.
3. **Maxed pickup reroll.** `RoomController.SpawnReward` rerolls when the selected pickup is at L3 for the player. Implementation: filter the available pool by current player level state before rolling.
4. **PickupSO carries level data.** Each pickup SO defines the L1/L2/L3 effect values. Player tracks current level per pickup.
5. **Damage attribution via active sources.** Dash registers as an active damage source for its duration. Kills check active sources to determine attribution for L3 refund.
6. **Shield damage interception path.** PlayerHealth checks for an active Shield component before applying HP damage AND before standard i-frame logic.
7. **Per-floor rarity tier system.** RoomContentsSO / FloorSO supports a rarity-weighted pool. F1 = Common only. F2 = Common + Rare. F3 = Common + Rare + Legendary.
8. **PowerSize via transform.localScale.** Single field scales sprite and collider together.
9. **Acquisition feedback scaffold.** Phase 1 builds the notification system + HUD icon row. Phase 3 polishes visuals.

---

## Deferred to v5+

Listed here so v4 doesn't quietly absorb them.

- Active abilities (charge meter UI, ability input binding, cooldown logic, ability trigger pipeline)
- Boss fights / section-end mechanics
- Procedural floor generation
- New enemy types or enemy art pass
- Real audio system + SFX
- Save state, hub area, meta-progression
- Modifier-stack refactor for stats (paired with enemy stat scaling)
- Room/environment art pass beyond placeholder
- Camera/minimap polish
- Choice rooms ("pick 1 of 2" pickup rooms)
- Max-HP scaling pickups (deliberate v4 exclusion)

---

## Open items to resolve in implementation

- [ ] Sprite assets for all 7 pickups (Phase 3)
- [ ] Player sprite + animation set (Phase 4)
- [ ] Final tuning numbers (Phase 5)
- [ ] PowerSize L3 hitbox sweet spot (Phase 5 playtest at 55%, 40%, 70%)
- [ ] ChargeShot damage scaling validation in playtest
- [ ] Acquisition feedback UI polish (Phase 3)

---

*Document complete. Hand off to Phase 1.*
