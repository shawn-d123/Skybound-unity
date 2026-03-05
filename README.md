# Skybound: Chronicles of Conquest

![Unity](https://img.shields.io/badge/Unity-2022.3%20LTS-black?logo=unity)
![C#](https://img.shields.io/badge/C%23-.NET-blue?logo=csharp)
![URP](https://img.shields.io/badge/Render%20Pipeline-URP-purple)
![Status](https://img.shields.io/badge/Status-Prototype-informational)
![License](https://img.shields.io/badge/License-MIT-green)

A Unity 3D third-person RPG prototype written in C#. The focus of this project was building a clean, modular architecture — covering player movement, class-based abilities, melee and archery combat, enemy AI using finite state machines, RPG-style progression, and a full account and world save system. Less about visual polish, more about getting the systems right.

---

> **Asset Notice:** The repo includes the full Unity project and prefabs, but several prefabs depend on third-party Asset Store packages (models, textures, animations, VFX) that aren't included due to licensing. You'll need to grab those separately — the full list is [below](#asset-store-packages-used).

---

## Table of Contents

- [Features](#features)
- [Player Classes](#player-classes)
- [Player Stats & Progression](#player-stats--progression)
- [Screens & Game Flow](#screens--game-flow)
- [Worlds & Saving](#worlds--saving)
- [Tech Stack](#tech-stack)
- [Getting Started](#getting-started)
- [Controls](#controls)
- [Enemy AI](#enemy-ai)
- [Project Structure](#project-structure)
- [Animator Parameters](#animator-parameters)
- [Configuration & Tuning](#configuration--tuning)
- [Asset Store Packages](#asset-store-packages-used)
- [Tutorials Referenced](#tutorials-referenced)
- [Troubleshooting](#troubleshooting)
- [Author](#author)

---

## Features

### Player Movement
Built on Unity's `CharacterController`. Movement is camera-relative with a full state machine tracking what the player is doing at any given time.

- Sprinting, crouching (toggle), jumping
- Gliding — hold jump while falling to reduce gravity and extend air time
- Dash — short burst with a cooldown managed by a coroutine
- Ground detection via `SphereCast`
- States: `Idle` / `Walking` / `Sprinting` / `Jumping` / `Crouching` / `Gliding` / `Dashing` / `Falling`

### Combat
- Equip/unequip workflow for both melee and bow
- Dedicated aiming mode that switches the active Cinemachine camera
- Arrow instantiation with forward impulse on fire
- Animator parameters wired to aim and shoot states
- Systems are designed around animation events where it makes sense — the animator drives behaviour, not the other way around

### Camera
- Cinemachine FreeLook for general play, a separate aiming camera for bow use
- Switching is handled via priority — no direct camera transitions

### Animation
- Blend tree input smoothing for natural movement feel
- Centralised animation controller keeps state flags consistent across systems
- Clean separation between what triggers an animation and what the animation does

---

## Player Classes

Each world is tied to exactly one class — **Mage** or **Assassin** — chosen at world creation and locked for that playthrough. This is intentional: it keeps builds distinct and gives players a reason to create multiple worlds.

### Assassin

**Q — Invisibility**
Temporarily removes the player from enemy detection. Implemented by toggling the detection conditions in the enemy AI while the ability window is active — enemies mid-chase will disengage and return to patrol.

**C — Backstab** *(Ultimate — unlocks at Level 10)*
Position-dependent finisher. Requires the player to be behind the enemy and within close range. Deals high burst damage and is designed as the Assassin's primary kill tool in most encounters.

### Mage

**Q — Heal**
Instantly restores a portion of the player's health. Has a cooldown to prevent spam — intended as a survivability tool, not a crutch.

**C — Magic Ball** *(Ultimate — unlocks at Level 10)*
Fires a ranged magic projectile toward a target direction. Gives the Mage a reliable mid-range damage option that complements the bow or fills in when enemies are at range.

> Abilities use cooldown timers and state checks — level requirement gates, positioning checks for Backstab — to keep them predictable and balanced without needing complex tuning.

---

## Player Stats & Progression

Stats persist per world and drive the core RPG loop:

| Stat | Notes |
|---|---|
| **Health** | Displayed via UI bar |
| **Stamina** | Displayed via UI bar |
| **XP + Level** | XP bar with level progression |
| **Money / Currency** | Earned through gameplay, used for progression |

Defeating enemies grants XP → XP increases level → level unlocks stronger tools, including ultimate abilities at Level 10.

---

## Screens & Game Flow

The project has a full menu-driven flow from launch through to gameplay:

**Start-Up Screen** — entry point and navigation into authentication.

**Sign-Up Screen** — creates a new local account and stores it for future logins.

**Log-In Screen** — validates credentials and loads the account data.

**World Creation Screen** — player enters a world name and selects a class (Mage or Assassin). Generating the world creates a new save and loads the game scene with that world's data.

**World Selection Screen** — displays all worlds saved under the logged-in account. Selecting one loads that world's class, stats, and progress.

**In-Game Menu (ESC)** — pauses the game and opens a pause menu. Closing it resumes exactly where the player left off.

**Game Over Screen** — triggered on player death. Provides options to return to the menu or restart depending on build state.

---

## Worlds & Saving

The game supports multiple saved worlds under a single account — each world is a fully independent playthrough.

### Creating a World
After logging in, the player creates a new world by entering a name and selecting a class. From that point, the world has its own isolated save file.

### What Gets Saved Per World
- Selected class
- Player level and XP
- Health and stamina progression
- Money / currency

### Loading a World
The World Selection Screen dynamically pulls all worlds linked to the active account. Selecting one loads the correct save file and spawns the player with the right class and stats already applied.

> Persistence is handled using local file-based saving (JSON). Each world's data is kept separate so multiple playthroughs can exist without overwriting each other.

---

## Tech Stack

| | |
|---|---|
| **Engine** | Unity 2022.3 LTS |
| **Language** | C# |
| **Render Pipeline** | Universal Render Pipeline (URP) |
| **Camera** | Cinemachine |
| **Animation** | Animator + Blend Trees |
| **Movement** | CharacterController |
| **Persistence** | Local JSON save files |

---

## Getting Started

### Requirements

- Unity 2022.3 LTS (other versions may work but aren't tested)
- Unity Asset Store account (for required packages)
- Cinemachine — install via Package Manager if not already present

### Setup

**1. Clone the repo**
```bash
git clone https://github.com/shawn-d123/Skybound-unity.git
```

**2. Open in Unity Hub**

Open the cloned folder as an existing project.

**3. Import Asset Store packages**

```
Window → Package Manager → My Assets
```

Download and import everything in the [Asset Store Packages](#asset-store-packages-used) list. Missing these will cause broken prefab references in the scene.

**4. Check Cinemachine is installed**

```
Window → Package Manager → Cinemachine
```

**5. Hit Play**

Open the Start-Up Screen scene and press Play.

---

## Controls

### Movement

| Input | Action |
|---|---|
| `W / A / S / D` | Move |
| `Mouse` | Look / camera rotation |
| `Left Shift` | Sprint |
| `Space` | Jump |
| `Hold Space` *(airborne)* | Glide |
| `C` | Toggle crouch / Backstab *(Assassin ultimate — replaces crouch)* |
| `E` | Dash |

### Combat & Weapons

| Input | Action |
|---|---|
| `1` | Equip melee weapon |
| `2` | Unequip |
| `3` | Equip bow |
| `RMB` | Aim |
| `LMB` *(while aiming)* | Shoot |

### Class Abilities

| Input | Mage | Assassin |
|---|---|---|
| `Q` | Heal | Invisibility |
| `C` | Magic Ball *(Level 10)* | Backstab *(Level 10)* |

---

## Enemy AI

Enemies are driven by finite state machines, keeping each behaviour isolated and easy to extend without touching unrelated logic.

### Core Behaviour

Enemies operate within a defined zone — if the player leaves the zone or the chase runs too long, the enemy disengages and returns to its spawn point. Behaviour is primarily distance-driven:

- **Mid-range:** use ranged ability if available, then begin chasing
- **Close-range:** prioritise melee attacks

The goal was for AI to feel reactive rather than scripted — transitions between roam, chase, and attack happen naturally based on distance and cooldown state rather than fixed triggers.

### Combat & Cooldowns

- A **global cooldown** blocks certain actions for a short window after any attack fires
- Individual attacks also carry their own cooldown timers
- If a ranged attack is on cooldown, the enemy continues chasing to force close-range engagement rather than standing and waiting

### Animation-Driven Attacks

Hit colliders are enabled and disabled at the correct frames by the Animator, keeping the damage timing tied directly to the animation rather than hardcoded delays. Ranged projectiles can be spawned via animation events, which keeps visuals and logic cleanly separated.

### Scaling to Bosses

The FSM structure was designed with this in mind from the start. Stronger enemies and bosses extend the same base with:

- Multiple attack types with different range requirements
- Priority-based selection — pick the best available attack based on distance and cooldown state
- Repositioning logic if the chosen attack requires a specific range

---

## Project Structure

Core scripts and what they're responsible for:

| Script | Role |
|---|---|
| `playerInputHandler.cs` | Captures raw input — axes, sprint, jump, crouch, dash, yaw |
| `movementHandler.cs` | All CharacterController logic, grounded checks, state updates |
| `playerStateManager.cs` | Defines the movement state enum shared across systems |
| `playerAnimationController.cs` | Central animation driver — smoothing, booleans, triggers |
| `combatInputHandler.cs` | Aim/shoot input flags and combat state signalling |
| `combatController.cs` | Equip logic, aiming params, arrow spawning, camera switching |
| `cameraHandler.cs` | Cinemachine priority switching between main and aim cameras |
| `arrowHandler.cs` | Arrow lifecycle — flight, impact, cleanup |

---

## Animator Parameters

### Floats

| Parameter | Purpose |
|---|---|
| `xInput` | Horizontal input (used for blend tree) |
| `yInput` | Vertical input (used for blend tree) |

### Booleans

| Parameter | Purpose |
|---|---|
| `isCrouching` | Crouch state active |
| `isJumping` | Jump initiated |
| `isFalling` | Player is in freefall |
| `isGliding` | Glide active |
| `isDashing` | Dash active |
| `isSprinting` | Sprint active |
| `isAiming` | Bow aim mode on |
| `isShooting` | Shoot animation playing |

### Triggers

Optional attack triggers — depends on your Animator controller setup.

---

## Configuration & Tuning

All of these are exposed in the Inspector:

| Category | Variables |
|---|---|
| **Speed** | `walkSpeed`, `sprintSpeed`, `crouchSpeed` |
| **Jump / Gravity** | `jumpHeight`, `gravity`, `glideGravity` |
| **Dash** | `dashSpeed`, `dashDuration`, `dashCooldownTime` |
| **Ground Detection** | `groundDetectionRadius`, `groundDetectionHeight`, `groundLayer` |
| **Combat** | `shootSpeed` |

---

## Asset Store Packages Used

Import via `Window → Package Manager → My Assets`:

- VFX URP - Fire Package
- AOE Explosions Pack 2
- Dragon for Boss Monster : PBR
- RPG - Stylized Fantasy Environment
- Free Quick Effects Vol. 1
- Status Effects and Auras FREE
- Stylized Fantasy Sword (PBR)
- Lowpoly Magician RIO
- Lowpoly Cowboy RIO V1.1
- FREE Low Poly Human - RPG Character
- Melee Warrior Animations FREE

---

## Tutorials Referenced

A mix of tutorials that helped shape specific systems in this project:

- [Third Person Movement in Unity](https://www.youtube.com/watch?v=4HpC--2iowE&t=313s)
- [Cinemachine Setup](https://www.youtube.com/watch?v=_J8RPIaO2Lc)
- [Animation Blend Trees](https://www.youtube.com/watch?v=muAzcpAg3lg&list=WL&index=27)
- [CharacterController Movement](https://www.youtube.com/watch?v=S_USClc_r5c)
- [Dash & Cooldown](https://www.youtube.com/watch?v=tD4tR7zO8y0)
- [Melee Combat](https://www.youtube.com/watch?v=78J493qWfDI)
- [Bow / Archery System](https://www.youtube.com/watch?v=zc8ac_qUXQY&t=531s)
- [Enemy AI](https://www.youtube.com/watch?v=0KDU_SzrCkA&t=224s)
- [Finite State Machines](https://www.youtube.com/watch?v=XOjd_qU2Ido&t=900s)
- [Animator Controller](https://www.youtube.com/watch?v=BLfNP4Sc_iA&t=496s)
- [URP Setup](https://www.youtube.com/watch?v=xWMQIozp6YE)
- [Cinemachine Camera Switching](https://www.youtube.com/watch?v=b-WZEBLNCik)
- [Glide Mechanic](https://www.youtube.com/watch?v=eR-AGr5nKEU)
- [Player State Management](https://www.youtube.com/watch?v=jnETyJUiCiM)
- [RPG Systems Playlist](https://www.youtube.com/playlist?list=PLllNmP7eq6TSkwDN8OO0E8S6CWybSE_xC)

---

## Troubleshooting

| Issue | Fix |
|---|---|
| Missing prefab references / broken scene objects | Import the Asset Store packages listed above |
| Pink / magenta materials | URP isn't configured correctly — re-import URP packages and check your Graphics settings |
| Cinemachine not switching on aim | Check that both FreeLook cameras are assigned in `cameraHandler` and that priority values are distinct |
| World save not loading correctly | Check that the JSON save files exist in the expected path and that the world name matches exactly |

---

## Author

**Shawn Santan D'Souza**

[![GitHub](https://img.shields.io/badge/GitHub-shawn--d123-181717?logo=github)](https://github.com/shawn-d123)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-Shawn%20D'Souza-0077B5?logo=linkedin)](https://www.linkedin.com/in/shawn-dsouza-08b6273b0)
[![Email](https://img.shields.io/badge/Email-dsouzashawn305%40gmail.com-D14836?logo=gmail)](mailto:dsouzashawn305@gmail.com)
