
# Crystal & Thorn

**Crystal & Thorn** — a small single-player hack-and-slash prototype set in a corrupted mining complex. Fight through cave tunnels, crystal caverns and ruined mine halls; clear every hostile to secure each zone and open the way forward.

---

## Table of Contents
- [Overview](#overview)  
- [Storyline](#storyline)  
- [Gameplay & Controls](#gameplay--controls)  
- [Screens & Tutorial](#screens--tutorial)  
- [Features](#features)  
- [Project structure (suggested)](#project-structure-suggested)  
- [Getting started / How to run](#getting-started--how-to-run)  
- [How to play (quick)](#how-to-play-quick)  
- [Roadmap & Notes](#roadmap--notes)  
- [Contributors](#contributors)  
- [License](#license)  

---

## Overview
This repository contains the Crystal & Thorn prototype: a fast, tactile hack-and-slash built around clearing rooms of enemies. The player begins at a cave entrance and fights through branching cave tunnels (including a scenic crystal side route), a mine tunnel and multiple combat arenas (prison, rooftop, main hall, final hall). The environment is atmospheric — skeletons, thornlike growths and crystal formations — but the gameplay focus is on combat and area-clear objectives rather than item collection.

---

## Storyline
Mining at Kaldren stopped when strange crystalline growths began to warp tunnels and reanimate the dead. Villages nearby were threatened by mutated scavengers and stone sentinels. The council hired purge-runners to clear the mine and reopen trade routes.

You are a purge-runner. Enter the cave, push through corrupted tunnels, and clear each area of hostile creatures so the miners’ old routes can be used again. There’s no artifact to retrieve — success is measured by how thoroughly you cleanse each zone.

---

## Gameplay & Controls

### Core loop
Explore → engage enemies → dodge (backstep/roll) → strike → clear area → progress to next arena.  
Progression is driven by clearing all enemies in an area (clearing a room opens doors / triggers the next wave).

### Controls
- **Move:** `W` / `A` / `S` / `D`  
- **Attack:** Left Mouse Button  
- **Backstep:** `Space` (short quick retreat)  
- **Roll:** `Space` + direction (`W`/`A`/`S`/`D`) — directional roll with brief invincibility frames; consumes stamina (or uses cooldown)

### Enemy types
- **Goblin:** fast, low HP, hit-and-run / swarm behaviour  
- **Golem:** slow, high HP, heavy hits, requires timed dodges and repositioning  
- **Skeletons (environmental):** scattered corpses for atmosphere; occasionally reanimated

---

## Screens & Tutorial
- **Intro / Title Screen:** Title, Start, Options (sound, controls), Quit.  
- **Game HUD:** Minimal UI — player health bar and a stamina/dodge meter (if implemented).  
- **Lose / Death Screen:** Defeat messaging and options to retry or return to title.  
- **Tutorial UI:** 8 labeled tutorial buttons and a central area that displays control/mechanics explanations (enemy behaviors, mechanics, control layout).

## Features

The system implements a rich set of features that enhance both gameplay experience and maintainability of the codebase:

- **Fast-paced Hack & Slash Combat**:  
  Responsive melee attacks, dodges, and rolls with clear feedback.
- **Immersive Dungeon Environment**:  
  Multiple connected scenes or one scene with marked areas: cave entrance, crystal cave branch, mine tunnel, prison, roof, main hall, real hall. Placement of environmental props (skeletons, crystals, thorn trees).  
- **Player Mobility**:  
  WASD movement, quick backstep, and directional roll for evasive tactics.  
- **Progression Through Clearing Enemies**:  
  Advancing by defeating all foes in an area rather than collecting artifacts.  
- **Animator & Animation Blending**:  
  Character animator with animation blending and transitions for idle, walk, attack, roll.  
- **Health & UI System**:  
  Player health bar, enemy hidden health values, loss screen and HUD management.  

---

## Project structure (suggested)
> Adapt this to your repository’s actual layout.

```
/CrystalAndThorn/
├─ Assets/
│  ├─ Scenes/
│  │  ├─ CaveEntrance.unity
│  │  ├─ CrystalCave.unity
│  │  ├─ MineTunnel.unity
│  │  ├─ Prison.unity
│  │  ├─ Rooftop.unity
│  │  ├─ MainHall.unity
│  │  └─ FinalHall.unity
│  ├─ Scripts/
│  │  ├─ PlayerController.cs
│  │  ├─ EnemyAI/
│  │  │  ├─ GoblinAI.cs
│  │  │  └─ GolemAI.cs
│  │  ├─ Combat/
│  │  └─ Nav/
│  ├─ Prefabs/
│  ├─ Animations/
│  ├─ Materials/
│  └─ Audio/
├─ ProjectSettings/
├─ README.md
└─ LICENSE
```

---

## Getting started / How to run
> These are general steps for a Unity-based project. Adjust for your engine/version if different.

### Prerequisites
- Unity Editor version 2022.3.31f1 (open the project folder in your installed Unity Editor).  
  *If you use a specific Unity version, note it in the repo (recommended to include `ProjectSettings/ProjectVersion.txt`).*
---

## How to play (quick)
- Start the game from the title screen.  
- Move with WASD, attack with left click. Use `Space` to backstep or `Space + direction` to roll.  
- Clear all enemies in a room to open the path forward. Survive through each arena until the final hall is cleared.

---

## Roadmap & Notes
**Planned / Suggested improvements**
- Add enemy variety and special attacks.  
- Implement checkpoints and better respawn behavior.  
- Polish combat with combos, enemy telegraphs, and stagger windows.  
- Add audio layers and more VFX for crystal/thorn interactions.  
- Balance enemy stats and add difficulty scaling / waves for larger halls.

**Known issues**
- (Add known bugs or placeholders here — e.g., NavMesh edge cases on the rooftop, roll stamina tuning.)

---

## Contributors
- **To Ngoc Hung** — `22125030`  
- **Khuong Nhan Kiet** — `22125043`  
- **Pham Trung Nghia** — `22125066`

---

## License
This project is released under the **MIT License**. See `LICENSE` for details.  
*(If you prefer another license, replace this section accordingly.)*

---

## Credits & Assets
- **Environment** https://assetstore.unity.com/packages/3d/environments/dungeons/polygon-dungeons-low-poly-3d-art-by-synty-102677
- **Player** https://assetstore.unity.com/packages/3d/animations/straight-sword-animation-set-220752
- **Enemies** https://assetstore.unity.com/packages/3d/animations/goblin-anims-253420
- **Codebase** https://youtube.com/playlist?list=PLD_vBJjpCwJvP9F9CeDRiLs08a3ldTpW5&si=ZDvreHsf1Ju59IgB
