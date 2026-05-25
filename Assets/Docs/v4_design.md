# v4 Design Document — "Weapons and Modifiers"

**Project codename:** Elenor
**Version scope:** v4
**Status:** Spec-complete, ready for refactor and continued implementation

---

## Purpose of this document

This is the canonical design spec for v4. It exists to prevent design-amnesia — when v5-you wants to "just add a max-HP pickup real quick" or revisit a v4 decision, this document is what reminds past-you's reasoning so the v4 pillars don't erode quietly.

This document supersedes the previous v4 design doc (which used an "everything is a modifier" model). The change reflects a design realization that surfaced during Phase 2 implementation: weapons and modifiers are fundamentally different concepts and trying to treat them as the same category created friction in both the gameplay and the architecture.

Read top to bottom for full context. Grep by pickup name when implementing.

---

## Player character

| Field              | Value                                                              |
| ------------------ | ------------------------------------------------------------------ |
| Name               | Elenor                                                             |
| Reference          | Quiet female high schooler, Persona 3 Makoto Yuki energy           |
| Sprite status      | Commissioned (in progress); placeholder generated sprite available |
| Baseline HP        | 6                                                                  |
| Baseline abilities | Movement (WASD), Shoot (mouse/arrows), Dash (Space, with i-frames) |

---

## Locked decisions

These are committed for v4. Changing any of them mid-implementation invalidates downstream work.

### Pickup system model

The pickup system has **two distinct categories**:

**Weapons** — Replace Elenor's current weapon. Define how shooting works. Mutually exclusive (one weapon equipped at a time). Not levelable.

**Modifiers** — Stack on top of the current weapon and/or the player. Multiple can be active simultaneously. Levelable (max level 3). Two subtypes:

- **WeaponModifier:** modifies the equipped weapon's projectiles (e.g., Poison, DualStream). Persists across weapon swaps.
- **PlayerModifier:** modifies the player (e.g., PowerSize, Shield, DashMod). Weapon-agnostic.

WeaponModifier vs PlayerModifier is an architectural distinction in code. **It is not player-facing** — modifier rooms drop either kind from a single pool. The distinction exists because the two kinds of modifiers do fundamentally different things (target the projectile config vs target the player) and separating them in code makes the system cleaner to extend.

### Drop sources

- **Regular cleared rooms:** drop health only. No pickups.
- **Item rooms:** drop a single pickup on a center pedestal after the room is cleared. Item room type is visible from outside (different door color in v4; eventually art/iconography). Two types: Weapon Rooms (drop a weapon) and Modifier Rooms (drop a modifier).

### Pool exclusion rules

- **Maxed modifiers leave the pool for the rest of the run.** Once Poison hits L3, no more Poison pickups can spawn this run. This applies to all modifiers — when all modifiers reach L3, the modifier pool is empty (deal with via reroll-or-skip logic; see Architectural Commitments).
- **Currently equipped weapon is excluded from weapon rooms.** If Elenor has MachineGun equipped, weapon rooms only spawn Starter or ChargeShot. Swapping weapons re-enables the previously equipped one for future rooms.

### Cross-swap behavior

- **Weapons swap on pickup.** Picking up a new weapon replaces the equipped weapon. The replaced weapon drops on the floor of the current room.
- **Dropped weapons persist until floor transition.** The player can change their mind and swap back as long as they're still on the same floor. On floor transition, dropped weapons are destroyed (along with any other room state from the previous floor).
- **Modifiers transfer across weapon swaps.** If Elenor has Poison L2 active and swaps weapons, Poison L2 still applies to the new weapon. Modifiers are weapon-agnostic by design.

### Item room distribution per floor

For v4's section (one section, three floors):

- **F1:** 1 Weapon Room (guaranteed). The player establishes weapon identity early.
- **F2:** 2 Modifier Rooms.
- **F3:** 2 Modifier Rooms.

Total: 1 weapon room + 4 modifier rooms across the section. F2 and F3 do not guarantee weapon rooms in v4. The player can choose to stick with their F1 weapon for the whole run, or wait for a later weapon room (none exist in v4, but the system supports it for v5+).

### Active abilities

- **Deferred to v5.** v4 is passive pickups and weapons only.
- Dash remains a default ability in v4. Modified (not granted) by DashMod when held.

### HP and survivability

- **No max-HP pickup in v4.** Deliberate. Survivability comes from Shield + dash skill + iframe management, not from raising the HP ceiling.
- Health drops can spawn in regular cleared rooms (existing behavior).

### Damage and i-frames

- Dash i-frames unchanged from base in v4. DashMod does not add or modify i-frame duration.
- Shield damage interception happens BEFORE i-frame logic. Shield-break does NOT set the standard `_invulnUntil` window.
- Shield-break grants ~0.1s mini-iframes to prevent same-physics-step pile-on.

### Damage attribution (for DashMod L3 refund)

- Dash registers itself as an active damage source for its duration. Kills during the dash window check active sources to determine attribution.
- Refund triggers on: dash damage kills, any projectile kill (regardless of weapon) within the dash window.
- Refund does NOT trigger on: poison ticks, environmental damage, indirect damage.

### PowerSize hitbox implementation

- Hitbox change scales `transform.localScale` on the Player. Sprite and collider scale together with one value.

### Pickup acquisition feedback

- On pickup: brief notification ("STUCK PEN — Lv2!" or for weapons "WEAPON SWAPPED — Old Lighter").
- HUD update: icon glow / level-up flash for modifiers; weapon icon swap for weapons.
- Phase 1 scaffolds the system; Phase 3 polishes visuals.

---

## Weapons

Three weapons total in v4.

### Weapon_Starter

| Field        | Value                                                           |
| ------------ | --------------------------------------------------------------- |
| Display Name | (Elenor's bare hands / pen / improvised starter — TBD with art) |
| Flavor Text  | TBD                                                             |
| Sprite       | TBD                                                             |

**Mechanical Effect:**
The default weapon Elenor starts every run with. Single-projectile, medium speed, modest damage and fire rate. Functional but not exciting — establishes a baseline that other weapons can clearly improve on. Roughly equivalent to Isaac's base tears.

**Stats (first pass — Phase 5 tuning):**

- Fire rate: 3 shots/second
- Damage per shot: 1
- Projectile speed: medium (matches current placeholder shooter)
- Projectile size: small/medium
- Effective DPS: 3

**Notes:**

- This is a slightly nerfed version of the current placeholder starter. The current starter does too much work on its own; the new starter exists to _be improved upon_.
- All modifiers apply to the starter weapon as normal.
- The starter cannot be lost; if Elenor "drops" a weapon mid-run, she only does so by picking up a different weapon. There's never a state where she has no weapon.

---

### Weapon_MachineGun

| Field        | Value                                              |
| ------------ | -------------------------------------------------- |
| Display Name | "Stuck Pen"                                        |
| Flavor Text  | "It won't stop clicking. Click click click click." |
| Sprite       | TBD                                                |

**Mechanical Effect:**
Replaces the equipped weapon. Fires smaller, faster projectiles at a high rate. High DPS through volume rather than per-shot power. Changes the _texture_ of combat — more bullets on screen, finer aim, less commitment per shot.

**Stats:**

- Fire rate: ~9 shots/second (3x starter)
- Damage per shot: 0.6
- Projectile speed: fast
- Projectile size: small
- Effective DPS: ~5.4 (significantly above starter)

**Notes:**

- This weapon has _its own projectile configuration_ (small fast bullets), not a modified starter projectile.
- Picking up MachineGun while Starter is equipped drops Starter; picking up MachineGun while ChargeShot is equipped drops ChargeShot.
- Cannot be picked up if already equipped (excluded from weapon room pool).

---

### Weapon_ChargeShot

| Field        | Value                             |
| ------------ | --------------------------------- |
| Display Name | "Old Lighter"                     |
| Flavor Text  | "Mom said never to play with it." |
| Sprite       | TBD                               |

**Mechanical Effect:**
Replaces the equipped weapon. Holding the fire input charges a shot; releasing fires it. Charge time determines damage multiplier. Tapping fire (no hold) still fires a normal shot at base damage, so spam-fire works for clearing weaker enemies. Held charge can swap aim direction before release.

**Stats:**

- Tap-fire rate: ~2 shots/second
- Tap-fire damage: 1
- Max charge time: 0.8s
- Fully charged damage: ×4 base = 4
- Fully charged projectile: visibly larger, pierces 1 enemy
- Effective DPS: highly variable based on player skill (charge timing, target selection)

**Notes:**

- ChargeShot has its own projectile configuration for both tap shots and charged shots (charged shots are a distinct projectile).
- Damage scales linearly with charge time — half-charged shots do half the bonus damage.
- Releasing fire mid-charge consumes the input; no "save the charge" mechanic.
- Modifiers apply to _both_ tap shots and charged shots.
- DashMod doesn't break the charge (dash isn't a fire input).
- Phase 5 tuning will likely revisit charge time and multiplier.

---

## Modifiers

Five modifiers total in v4. All are levelable (max L3). All persist across weapon swaps.

### Modifier_Poison (WeaponModifier)

| Field        | Value                        |
| ------------ | ---------------------------- |
| Display Name | "Cafeteria Mystery"          |
| Flavor Text  | "Nobody asked what's in it." |
| Rarity       | Common                       |
| Sprite       | TBD                          |

**Mechanical Effect:**
Adds a poison status to all projectiles fired by Elenor's currently equipped weapon. Enemies hit take damage-over-time in addition to the projectile's direct damage. Poison refreshes the timer on re-hit; it does not stack DPS. At L3, poisoned enemies that die spawn a small poison cloud at their position.

**Levels:**

- **L1:** 1 damage per second for 2 seconds.
- **L2:** 2 damage per second for 4 seconds.
- **L3:** 3 damage per second for 5 seconds. Poisoned enemies spawn a small AoE poison cloud on death (small radius, ~2s duration, damage equal to L3 poison DPS).

**Notes:**

- Implementation: poison is applied via the WeaponModifier modifying the ProjectileConfig before spawn. Whatever weapon fires, its projectiles get the poison flag set.
- Poison damage doesn't trigger DashMod L3 refund (must be direct damage source).
- Cloud doesn't damage Elenor.
- Applies to ChargeShot's tap-fire and charged shots equally.

---

### Modifier_DualStream (WeaponModifier)

| Field        | Value                           |
| ------------ | ------------------------------- |
| Display Name | "Snapped Ruler"                 |
| Flavor Text  | "Now it's two rulers. Sort of." |
| Rarity       | Rare                            |
| Sprite       | TBD                             |

**Mechanical Effect:**
Each shot fires multiple projectiles in opposing directions simultaneously. Whatever the current weapon spawns, DualStream spawns mirrored copies. Changes positioning incentives — being in the center of a room becomes valuable; walls behind Elenor become useful for "wasted" stream coverage.

**Levels:**

- **L1:** 2 streams — forward (aimed) and backward (opposite of aim).
- **L2:** 3 streams — forward, backward, and a third stream in the player's current movement direction (defaulting to up if stationary).
- **L3:** 4 streams — symmetric cross pattern _relative to aim_ (forward, backward, perpendicular-left, perpendicular-right). Cross rotates with aim direction.

**Notes:**

- Implementation: DualStream modifies the spawn step. After the weapon decides to fire, DualStream spawns N projectiles based on its level instead of 1.
- All streams use the equipped weapon's projectile config (so MachineGun + DualStream = 2-4 streams of small fast bullets; ChargeShot + DualStream = 2-4 streams of slow charging shots).
- L2 third-stream direction follows `PlayerMovement.LastNonZeroInput`. Defaults to up when stationary.
- L3 cross is aim-relative, not world-relative.
- Modifiers like Poison apply to all streams equally.
- Charged shots fire from all stream directions, each at full multiplier (not divided).

---

### Modifier_PowerSize (PlayerModifier)

| Field        | Value                                              |
| ------------ | -------------------------------------------------- |
| Display Name | "Energy Drink"                                     |
| Flavor Text  | "She'd never finished one before. Felt different." |
| Rarity       | Rare                                               |
| Sprite       | TBD                                                |

**Mechanical Effect:**
Modifies player stats with a tradeoff: increases damage output and movement speed, but also increases the player's hitbox size (sprite + collider, scaled together). Pure tradeoff design — more offensive capability, easier to get hit.

**Levels:**

- **L1:** +25% damage, +15% speed, +20% hitbox size.
- **L2:** +50% damage, +25% speed, +35% hitbox size.
- **L3:** +85% damage, +40% speed, +55% hitbox size.

**Notes:**

- Implementation: scales `transform.localScale`. Affects projectile damage globally (via player stat), movement speed, and collider/sprite size in one operation.
- Hitbox change is visual _and_ mechanical — Elenor's sprite scales so the player can read their own hitbox size.
- Speed bonus applies to base movement only, not to dash distance.
- L3 hitbox at +55% is starting tuning. Phase 5 playtest at 55%, 40%, 70% to find sweet spot.

---

### Modifier_DashMod (PlayerModifier)

| Field        | Value                                    |
| ------------ | ---------------------------------------- |
| Display Name | "Track Shoes"                            |
| Flavor Text  | "From the lost and found. Someone fast." |
| Rarity       | Rare                                     |
| Sprite       | TBD                                      |

**Mechanical Effect:**
Modifies the default dash ability. Dash itself remains the same input (Space) and base cooldown — this pickup adds capabilities. At higher levels, dash becomes an offensive tool, not just movement/i-frame.

**Levels:**

- **L1:** Dash deals damage to enemies passed through. Damage = 4× base shot damage.
- **L2:** L1 retained. Dash distance increased by ~40%.
- **L3:** L1+L2 retained. If an enemy dies during the dash window from any direct player damage source, dash cooldown refunds fully.

**Notes:**

- Poison ticks killing during dash window do NOT trigger L3 refund.
- Dash damage is NOT a projectile — doesn't apply poison, doesn't get DualStream'd, isn't multiplied by ChargeShot charge.
- Dash i-frames unchanged from base. DashMod adds no i-frames.
- Implementation: dash registers as active damage source for its duration; kills check active sources.

---

### Modifier_Shield (PlayerModifier)

| Field        | Value                                |
| ------------ | ------------------------------------ |
| Display Name | "Lucky Charm"                        |
| Flavor Text  | "She kept it in her pocket. Always." |
| Rarity       | Legendary                            |
| Sprite       | TBD                                  |

**Mechanical Effect:**
Player gains a regenerating shield that absorbs incoming damage. When hit, the shield breaks; Elenor takes no HP damage. After breaking, regenerates after a fixed cooldown. Shield state is visually obvious — sprite around Elenor when active, absent when broken.

**Levels:**

- **L1:** Absorbs 1 hit. Regenerates 12 seconds after breaking.
- **L2:** Absorbs 1 hit. Regenerates 8 seconds after breaking.
- **L3:** Absorbs 2 hits before fully breaking. Regenerates 8 seconds after fully breaking.

**Notes:**

- Shield damage interception happens BEFORE i-frame logic. Shield-break does NOT set `_invulnUntil`.
- Shield-break grants ~0.1s mini-iframes to prevent same-physics-step pile-on.
- Shield regen timer pauses during room transitions and pause menu.
- Shield does not prevent knockback or hit reactions, only HP loss.
- Shield hit triggers hit-flicker for player feedback.

---

## Modifier interaction notes

Modifiers stack and apply to whatever weapon is equipped. Most interactions are emergent rather than explicit.

A few combinations worth noting because they produce notable build identities:

- **Poison + DualStream:** Multiple streams each apply poison, useful for poisoning crowds quickly.
- **Poison + MachineGun (as weapon):** Many projectiles per second = poison applied very frequently.
- **DualStream + ChargeShot:** Charged shots fire from all stream directions at full multiplier. Single charged input becomes a multi-direction nuke.
- **PowerSize + Shield:** Shield mitigates the bigger-hitbox downside. Canonical "aggressive build" pair.
- **DashMod + offensive modifiers:** More damage output during the dash window = more L3 refund procs.
- **Shield + anything:** Shield enables aggressive play across the board.

There are no mutually exclusive modifiers in v4. All five can be active simultaneously (in theory).

---

## Item Rooms

### Visual identification

- Item rooms have a **distinctly colored door** visible from the previous room. Placeholder for v4:
  - Weapon Room door color: TBD (suggest a warm-leaning color from the palette — e.g., the bright cyan `#0ce6f2` to stand out from regular doors)
  - Modifier Room door color: TBD (suggest a different value — e.g., pure white `#ffffff`)
- Eventually replaced with icons or art assets, but color-coding is sufficient for v4.

### Room layout

- Item rooms are visually similar to regular rooms but contain a **central pedestal**.
- After clearing the room (defeating all enemies), the pickup spawns on the pedestal.
- The player picks it up by walking over the pedestal (same trigger logic as regular pickups).

### Difficulty

- Item rooms have **more enemies** than regular rooms (rough target: 1.5-2x normal density).
- v4 does not differentiate further (no elite enemies, no special hazards, no timer). Just more of the existing enemies. Future versions can add complexity.

### Drop rules

- **Weapon Room:** spawns one weapon pickup. Pool excludes the currently equipped weapon. F1's weapon room: pool is {MachineGun, ChargeShot} since Elenor enters with Starter.
- **Modifier Room:** spawns one modifier pickup. Pool excludes modifiers already at L3. Pool includes both WeaponModifiers and PlayerModifiers (no distinction surfaced to the player).
- If the pool is empty (all weapons owned / all modifiers maxed), the room drops a health pickup or nothing — TBD in implementation. Empty-pool case should be rare in v4.

### Persistence

- If the player leaves an item room without collecting the pedestal pickup, **the pickup remains on the pedestal until floor transition**. They can come back.
- On floor transition, the pickup is destroyed.

---

## Architectural commitments

These are implementation patterns that need to be locked when the refactor begins.

### Core type structure

```
PickupSO (abstract base)
├── WeaponSO            — defines a weapon (firing behavior + projectile config)
├── ModifierSO (abstract base for modifiers)
│   ├── WeaponModifierSO  — modifies the equipped weapon's projectiles
│   └── PlayerModifierSO  — modifies the player
```

### WeaponSO

- One SO type per weapon family. Weapons differ along configurable axes (fire rate, damage, projectile config reference, charge behavior, etc.).
- For weapons that need fundamentally novel firing logic (e.g., ChargeShot's charge-and-release input), the WeaponSO references a `WeaponBehaviorSO` or similar opt-in extension. The default firing behavior covers Starter and MachineGun without needing custom code.
- Adding a new weapon that fits the default firing model: create a new `WeaponSO` `.asset`, fill in fields, done. No new code required.
- Adding a new weapon that needs novel behavior: create a new `WeaponBehaviorSO` subclass and reference it. The weapon's `.asset` references the behavior.

### ProjectileConfig

- Projectile _configuration_ (speed, damage, scale, sprite, on-hit effects like poison/pierce/bounce) lives in a `ProjectileConfigSO` or `ProjectileConfig` struct.
- Weapons reference a ProjectileConfig for their projectiles. ChargeShot references two configs (one for tap, one for charged shot).
- WeaponModifiers wrap or modify the ProjectileConfig _before_ the projectile is spawned. Poison sets a poison flag on the config. DualStream causes multiple projectiles to be spawned with the same config.
- The `Projectile` MonoBehaviour stays lean — it just applies whatever config it was spawned with. Adding a new projectile _type_ is a new ProjectileConfig, not a new MonoBehaviour.

### Modifier architecture

- WeaponModifiers and PlayerModifiers share a base interface for pickup acquisition (display, rarity, leveling), but diverge on what they configure.
- WeaponModifiers register themselves with the player's projectile-spawn pipeline. When the player fires, the pipeline asks all active WeaponModifiers to modify the outgoing ProjectileConfig (Poison sets poison flag; DualStream spawns multiple).
- PlayerModifiers register themselves with the relevant player system. PowerSize modifies PlayerStats. DashMod modifies PlayerDash behavior. Shield adds a component that intercepts damage.
- Both types are owned by `PlayerPickupInventory`. The inventory tracks level state for each modifier.

### Pool exclusion

- The pickup registry exposes the full pool but the **selection logic** at item-room drop time filters out:
  - Currently equipped weapon (for weapon rooms)
  - Modifiers already at L3 (for modifier rooms)
- This logic lives in the item room drop pipeline, not in the registry SO itself.

### Cross-swap behavior

- When a new weapon is equipped, the previous weapon is dropped at the current position as a pickup.
- Dropped weapons are normal pickup entities that can be picked up again.
- On floor transition, room state (including dropped weapons) is destroyed.

### Item rooms

- Item rooms are a new `RoomController` configuration or subclass — TBD which is cleaner in the refactor. The room has a pedestal spawn point and an "item room type" field (Weapon or Modifier).
- The floor layout system needs to support marking specific rooms as item rooms. FloorSO gains a per-room `RoomType` field (Regular / Weapon / Modifier).

### Pickup acquisition feedback

- On pickup: brief notification ("STUCK PEN — Lv2!" for modifier levels, "WEAPON SWAPPED — Old Lighter" for weapons).
- HUD shows currently equipped weapon (icon + name).
- HUD shows owned modifiers with current levels.
- Phase 1 scaffolds the system; Phase 3 polishes visuals.

---

## Refactor scope

The current implementation uses the old "everything is a modifier" model with `MachineGunPickupSO` as a modifier-style pickup. The refactor restructures this. Order of work:

1. **Update CONVENTIONS.md** if any new patterns emerge from the redesign.
2. **Introduce ProjectileConfigSO** and refactor `Projectile` to be config-driven. Existing projectile behavior moves into a default config used by the starter weapon.
3. **Introduce WeaponSO** and refactor `PlayerShooter` to be weapon-driven. The current starter behavior becomes `Weapon_Starter`.
4. **Restructure pickup base classes** — `PickupSO` becomes abstract base for `WeaponSO`, `WeaponModifierSO`, `PlayerModifierSO`.
5. **Migrate MachineGun** from modifier to weapon (`Weapon_MachineGun`).
6. **Migrate Poison** to new WeaponModifier base; reimplement as ProjectileConfig modifier rather than as a component on the player.
7. **Implement weapon swap behavior** — drop old weapon as pickup on floor.
8. **Implement item room concept** — new room type, pedestal spawn, item-room drop pipeline.
9. **Implement pool exclusion** — equipped weapon and L3 modifiers filter out of selection.
10. **Update FloorSO + room generation** to support marking rooms as item rooms (1 Weapon Room on F1, 2 Modifier Rooms each on F2/F3).
11. **HUD updates** for equipped weapon and modifier inventory.
12. **Continue building remaining pickups** in the new model: ChargeShot (weapon), DualStream, PowerSize, DashMod, Shield (modifiers).

Estimated effort: 4-6 evenings for steps 1-11, then per-pickup work for step 12. This is significantly larger than the original v4 scope; the project's overall v4 estimate goes up accordingly.

---

## Composition density (post-redesign)

The composition matrix is thinner than the original v4 doc because weapon-weapon interactions don't exist (weapons are mutually exclusive). What remains is modifier-modifier interactions and modifier-with-weapon flavor.

|                | Starter | MG     | Charge | Poison | Dual   | Power | Dash   | Shield |
| -------------- | ------- | ------ | ------ | ------ | ------ | ----- | ------ | ------ |
| **Starter**    | —       | swap   | swap   | ✓      | ✓      | ✓     | ✓      | ✓      |
| **MachineGun** | swap    | —      | swap   | ✓      | ✓      | ✓     | ✓      | ✓      |
| **ChargeShot** | swap    | swap   | —      | ✓      | ✓      | ✓     | ✓      | ✓      |
| **Poison**     | ✓       | ✓      | ✓      | —      | ✓      | (—)   | flavor | (impl) |
| **DualStream** | ✓       | ✓      | ✓      | ✓      | —      | ✓     | (—)    | (impl) |
| **PowerSize**  | ✓       | ✓      | ✓      | (—)    | ✓      | —     | ✓      | ✓      |
| **DashMod**    | ✓       | ✓      | ✓      | flavor | (—)    | ✓     | —      | (impl) |
| **Shield**     | (impl)  | (impl) | (impl) | (impl) | (impl) | ✓     | (impl) | —      |

✓ = compatible / interesting combination. swap = mutually exclusive (weapon swap). (—) = no notable interaction. (impl) = implicit interaction (e.g., Shield enables aggressive play with X). flavor = mechanically distinct but worth noting.

---

## Deferred to v5+

- **Active abilities** (charge meter UI, ability input binding, cooldown logic, ability trigger pipeline)
- **Boss fights / section-end mechanics**
- **Procedural floor generation**
- **New enemy types or enemy art pass**
- **Real audio system + SFX**
- **Save state, hub area, meta-progression**
- **Modifier-stack refactor for stats** (paired with enemy stat scaling)
- **Camera/minimap polish**
- **Choice rooms** ("pick 1 of 2" pickup rooms — different concept from item rooms)
- **Max-HP scaling pickups** (deliberate v4 exclusion)
- **Multiple sections per run** (v4 has one section; future versions add more)
- **Weapon room guarantee in F1 of every section** (rule defined here but only applies to one section in v4)
- **More weapons** (v4 ships 3; v5+ will want 4-6+ for real weapon variety)
- **More modifiers** (v4 ships 5; v5+ adds more)
- **Item room difficulty beyond "more enemies"** (elites, hazards, timers)
- **Sophisticated drop-rate weighting** (rarity tiers within categories, per-floor pool curation)

---

## Open items to resolve in implementation

- [ ] Sprite assets for all 8 pickups + Elenor (commissioned, in progress)
- [ ] Player sprite + animation set (Phase 4)
- [ ] Item room door colors (placeholder palette assignment)
- [ ] Starter weapon final stats (Phase 5 playtest)
- [ ] ChargeShot damage scaling validation (Phase 5 playtest)
- [ ] PowerSize L3 hitbox sweet spot (Phase 5 playtest at 55%, 40%, 70%)
- [ ] Empty-pool fallback for item rooms (when all weapons owned or all modifiers maxed)
- [ ] Acquisition feedback UI polish (Phase 3)
- [ ] HUD layout: equipped weapon + modifier icons
- [ ] Confirm whether item rooms are a RoomController subclass or a configuration on the existing RoomController

---

_Document supersedes the original v4 design. Hand off to refactor work._
