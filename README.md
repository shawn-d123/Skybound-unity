# Skybound: Chronicles of Conquest (Unity 3D RPG Prototype)

A Unity 3D third-person open-world RPG prototype built in C# with a modular gameplay architecture. The project focuses on responsive player movement, melee + archery combat, Cinemachine camera control, animation-driven gameplay, and complex enemy AI built using finite state machines.

> **Repo focus:** This repository is intended to showcase **engineering + gameplay systems** (scripts/architecture). Large third-party assets (models, animations, environments, SFX/VFX packs) may be excluded to keep the repo lightweight.

---

## Features

### Player Movement (CharacterController)
- Camera-relative 3D movement
- Sprinting, crouching (toggle), jumping
- Gliding (reduced fall gravity when holding jump)
- Dash with cooldown + coroutine-driven burst
- Ground detection via SphereCast
- Movement state tracking (Idle / Walking / Sprinting / Jumping / Crouching / Gliding / Dashing / Falling)

### Combat
- Weapon equipping workflow (melee + bow)
- Aiming mode with camera switching
- Archery: arrow instantiation + forward impulse
- Animator parameter control for aiming/shooting states
- Animation-centric approach (systems designed to be driven by animation events where appropriate)

### Camera (Cinemachine)
- Cinemachine FreeLook main camera + aiming camera
- Smooth switching using priority control

### Animation System
- Blend tree input smoothing for natural transitions
- Clear animation state flags (jump, fall, glide, dash, sprint, crouch, aim, shoot)
- Designed to keep animation updates centralized and consistent

### Enemy AI (Finite State Machines)
- AI behaviours structured using state machines for clarity and performance
- Supports layered combat flow design (e.g., chase → range-check → attack selection → cooldown handling)
- Architecture built to scale from base enemies to more complex enemies/bosses

---

## Tech Stack
- **Unity** (2022.x recommended)
- **C#**
- **Cinemachine**
- **Animator / Blend Trees**
- **CharacterController**

---

## Getting Started

### Requirements
- Unity 2022.x (or close equivalent)
- Cinemachine package installed via Unity Package Manager

### Setup (High Level)
1. Clone this repository.
2. Open the Unity project (or copy scripts into your own Unity project under `Assets/Scripts/`).
3. Install **Cinemachine**:
   - `Window → Package Manager → Cinemachine`
4. Create and assign scene references in the Inspector (Player object, cameras, Animator, prefabs).

> If you’re missing art assets/prefabs from this repo, import your own equivalents and wire them up in the Inspector.

---

## Controls (Default)

### Movement
- **W/A/S/D** — Move
- **Mouse** — Look / rotate camera
- **Left Shift** — Sprint
- **Space** — Jump
- **Hold Space (while falling)** — Glide
- **C** — Toggle crouch
- **E** — Dash (cooldown applied)

### Combat / Weapons
- **1** — Equip melee weapon
- **2** — Unequip current weapon
- **3** — Equip bow
- **Right Mouse Button** — Aim
- **Left Mouse Button (while aiming)** — Shoot

---

## Project Structure (Core Scripts)

> Names may vary slightly depending on your folder structure.

- `playerInputHandler.cs`  
  Reads player input (movement axes, sprint/jump/crouch/dash, camera yaw).

- `movementHandler.cs`  
  CharacterController movement logic (walk/sprint/crouch/jump/glide/dash), grounded checks, movement state updates.

- `playerStateManager.cs`  
  Defines movement state enum used throughout the project.

- `playerAnimationController.cs`  
  Central animation driver: blend tree smoothing + state booleans + optional triggers.

- `combatInputHandler.cs`  
  Input flags for aim/shoot and combat state signalling.

- `combatController.cs`  
  Weapon equip/unequip, aiming animation parameters, arrow spawning + force application, and camera switching integration.

- `cameraHandler.cs`  
  Cinemachine FreeLook priority switching between main and aiming cameras.

- `arrowHandler.cs`  
  Basic arrow lifecycle logic (cleanup + impact behaviour).

---

## Animator Parameters (Expected)

Ensure your Animator includes parameters matching the gameplay scripts:

**Floats**
- `xInput`
- `yInput`

**Bools**
- `isCrouching`
- `isJumping`
- `isFalling`
- `isGliding`
- `isDashing`
- `isSprinting`
- `isAiming`
- `isShooting`

**Triggers**
- Optional attack triggers (if used by your controller)

---

## Configuration / Tuning

Common values you may want to adjust in the Inspector:
- `walkSpeed`, `sprintSpeed`, `crouchSpeed`
- `jumpHeight`, `gravity`, `glideGravity`
- `dashSpeed`, `dashDuration`, `dashCooldownTime`
- Ground detection: `groundDetectionRadius`, `groundDetectionHeight`, `groundLayer`
- Bow projectile: `shootSpeed`

---

## Design Notes (Why it’s built this way)
- **Modular responsibilities:** input → movement/combat controllers → animation driver.
- **Inspector-driven wiring:** reduces runtime lookups and keeps behaviour explicit.
- **State machines for AI:** improves readability, debugging, and extensibility versus large nested condition blocks.
- **Animation-first combat:** supports industry-style workflows (hitboxes/projectiles triggered by animation events).

---

## Known Limitations / TODO
- Add full melee combo input handling + animation-event hit detection (if not already wired for your build)
- Expand damage pipeline (health, stagger, hit reactions)
- Extend AI states/attacks and polish attack telegraphing
- Add persistence (saving/loading), inventory, and UI polish as needed
- Add performance profiling and optimisation passes for larger scenes

---

## Third-Party Assets
This project may rely on external assets (characters, animations, environment packs, VFX/SFX).  
Those assets are **not automatically licensed** under any code license you apply—always follow the original asset license terms.

---

## License
No license is applied by default.  
If you make the repository public and want others to reuse your **code**, consider adding the **MIT License** (simple, permissive).  
If you prefer others not to reuse your code, keep it unlicensed (default “all rights reserved”).

---

## Screenshots / Demo
Add your GIFs or screenshots here:
- `docs/screenshots/`
- `docs/demo.gif`

(You can embed them with standard Markdown image links.)

---

## Contact
If you want people to reach you, add:
- GitHub / LinkedIn
- Email (optional)
