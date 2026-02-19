# 2048 3D - Physical Arcade

A 3D physical representation of the popular 2048 puzzle game, developed as a practical assignment for Raccoons Games. 

## 🎮 Gameplay Overview
The player controls numbered cubes on a rectangular board limited by walls. The core mechanic is to aim, shoot, and merge cubes to earn the highest score possible.

* **Controls:** Touch and drag horizontally to position the active cube. Release to physically launch the cube forward.
* **Spawn Logic:** A new cube spawns automatically with a value of 2 (75% probability) or 4 (25% probability).
* **Merge Mechanics:** Cubes merge into a single higher-value cube (Power-of-2) if they have the identical value **and** collide with sufficient physical impulse. 
* **Scoring:** Every successful merge awards points equal to `Merged Value / 2` (e.g., merging two 4s creates an 8 and awards 4 points).

## 🛠️ Architecture & Technical Details
The project was developed with a strong emphasis on clean, maintainable, and scalable code.

* **Platform:** Android
* **Code Architecture:** Designed following **SOLID principles** and standard design patterns. The logic is cleanly decoupled (Input handling, Spawning, Game Logic, Physics) to ensure flexibility for future features.
* **Physics System:** Utilizes Unity's Rigidbody and physics engine to create satisfying, natural collisions, bouncing, and sliding effects.
* **Optimization:** The source code avoids unnecessary allocations during gameplay to ensure a stable framerate on mobile devices. Unified code style and clear naming conventions are maintained throughout the project.

## 🚀 How to Run
1. Clone this repository.
2. Open the project in Unity.
3. Switch the build platform to **Android**.
4. Open the main gameplay scene and press Play, or build the `.apk` directly to your Android device.

---
*Note: A video demonstration of the gameplay and a playable `.apk` build have been provided separately.*
