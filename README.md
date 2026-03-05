# Skybound: Chronicles of Conquest (Unity 3D RPG Prototype)

![Unity](https://img.shields.io/badge/Unity-2022.3%20LTS-black)
![C#](https://img.shields.io/badge/C%23-.NET-blue)
![URP](https://img.shields.io/badge/Render%20Pipeline-URP-purple)
![Status](https://img.shields.io/badge/Status-Prototype-informational)

A Unity 3D third-person open-world RPG prototype built in C# with a modular gameplay architecture. The project focuses on responsive player movement, melee + archery combat, Cinemachine camera control, animation-driven gameplay, and complex enemy AI implemented using finite state machines.

> Asset dependency notice: This repository includes the full Unity project and prefabs, but some prefabs reference third-party Unity Asset Store packages (models/materials/textures/animations/VFX). Due to licensing, those Asset Store packages must be acquired and imported separately after cloning.

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
- Centralised animation updates for consistent behaviour

### Enemy AI (Finite State Machines)
- AI behaviours structured using state machines for clarity and scalability
- Supports layered combat flow design (chase -> range check -> attack selection -> cooldown handling)
- Designed to scale from base enemies to more advanced enemies/bosses

---

## Tech Stack
- Unity: 2022.3 LTS (recommended)
- Language: C#
- Render Pipeline: URP (Universal Render Pipeline)
- Camera: Cinemachine
- Animation: Animator / Blend Trees
- Movement: CharacterController

---

## Getting Started

### Requirements
- Unity 2022.3 LTS (or close equivalent)
- Unity Asset Store access (to import required packages)
- Cinemachine installed (Unity Package Manager)

### Setup
1. Clone the repository:
   ```bash
   git clone https://github.com/shawn-d123/Skybound-unity.git

Open the project in Unity Hub.

Import the required Asset Store packages:

Unity Editor -> Window -> Package Manager -> My Assets

Download and Import each package listed in "Asset Store Packages Used"

Install/confirm Cinemachine:

Window -> Package Manager -> Cinemachine

Open the scene for the Start-Up Screen and press Play.

Controls (Default)
Movement

W/A/S/D - Move

Mouse - Look / rotate camera

Left Shift - Sprint

Space - Jump

Hold Space (while falling) - Glide

C - Toggle crouch

E - Dash (cooldown applied)

Combat / Weapons

1 - Equip melee weapon

2 - Unequip current weapon

3 - Equip bow

Right Mouse Button - Aim

Left Mouse Button (while aiming) - Shoot

Project Structure (Core Scripts)

playerInputHandler.cs - Reads player input (movement axes, sprint/jump/crouch/dash, camera yaw).

movementHandler.cs - CharacterController movement logic, grounded checks, movement state updates.

playerStateManager.cs - Defines movement state enum used across systems.

playerAnimationController.cs - Central animation driver (blend smoothing + state booleans + optional triggers).

combatInputHandler.cs - Input flags for aim/shoot and combat state signalling.

combatController.cs - Weapon equip/unequip, aiming params, arrow spawning + force, camera switching integration.

cameraHandler.cs - Cinemachine FreeLook priority switching (main vs aiming).

arrowHandler.cs - Basic arrow lifecycle (cleanup + impact behaviour).

Animator Parameters (Expected)
Floats

xInput

yInput

Bools

isCrouching

isJumping

isFalling

isGliding

isDashing

isSprinting

isAiming

isShooting

Triggers

Optional attack triggers (if used by your controller)

Configuration / Tuning

Common values to tweak in the Inspector:

walkSpeed, sprintSpeed, crouchSpeed

jumpHeight, gravity, glideGravity

dashSpeed, dashDuration, dashCooldownTime

groundDetectionRadius, groundDetectionHeight, groundLayer

shootSpeed

Asset Store Packages Used

The following Asset Store packages are used by this project and must be acquired/imported separately:

VFX URP - Fire Package

AOE Explosions Pack 2

Dragon for Boss Monster : PBR

RPG - Stylized Fantasy Environment

Free Quick Effects Vol. 1

Status Effects and Auras FREE

Stylized Fantasy Sword (PBR)

Lowpoly Magician RIO

Lowpoly Cowboy RIO V1.1

FREE Low Poly Human - RPG Character

Melee Warrior Animations FREE

Tutorials Referenced

https://www.youtube.com/watch?v=4HpC--2iowE&t=313s

https://www.youtube.com/watch?v=_J8RPIaO2Lc

https://www.youtube.com/watch?v=muAzcpAg3lg&list=WL&index=27

https://www.youtube.com/watch?v=S_USClc_r5c

https://www.youtube.com/watch?v=tD4tR7zO8y0

https://www.youtube.com/watch?v=78J493qWfDI

https://www.youtube.com/watch?v=zc8ac_qUXQY&t=531s

https://www.youtube.com/watch?v=0KDU_SzrCkA&t=224s

https://www.youtube.com/watch?v=XOjd_qU2Ido&t=900s

https://www.youtube.com/watch?v=BLfNP4Sc_iA&t=496s

https://www.youtube.com/watch?v=xWMQIozp6YE

https://www.youtube.com/watch?v=b-WZEBLNCik

https://www.youtube.com/watch?v=eR-AGr5nKEU

https://www.youtube.com/watch?v=jnETyJUiCiM

https://www.youtube.com/playlist?list=PLllNmP7eq6TSkwDN8OO0E8S6CWybSE_xC

Troubleshooting

Missing prefabs / missing references: Import the Asset Store packages listed above (Window -> Package Manager -> My Assets).

Pink materials / shaders: Confirm URP is set up correctly and re-import URP-related packages if needed.

Cinemachine camera not switching: Ensure both FreeLook cameras are assigned in cameraHandler and priorities are set correctly.

License

No license is applied by default. If you want others to reuse your code, consider adding an MIT License.

Author / Contact

Shawn Santan D’Souza
GitHub: https://github.com/shawn-d123

LinkedIn: https://www.linkedin.com/in/shawn-dsouza-08b6273b0

Email: dsouzashawn305@gmail.com


### If bullets still don’t paste correctly
That’s usually because you’re pasting into something like Word/Notes first, or your browser is converting characters.

**Fix:** Paste directly into GitHub’s README editor, or into VS Code, then into GitHub.

If you tell me **where** you’re pasting it (GitHub editor / VS Code / Notepad / Word), I’ll give you the exact fix for that one.
