2048 3D – Physics Prototype

Unity Technical Assignment for Raccoons Games
---------------------------------------------
 Author
 ------
Oshkoder Anna
Unity Developer

Project Vision
--------------
This project is a physics-based 3D reinterpretation of the classic 2048 game.

The objective was not only to recreate the core mechanics, but to demonstrate:

Clean and scalable architecture
Clear responsibility separation
Event-driven communication between systems
Satisfying and responsive gameplay feel

The prototype was built with a production-oriented mindset rather than a quick demo approach.


Gameplay Overview
-----------------
A cube (75% → 2, 25% → 4) spawns at the start of the board.

Player drags horizontally to position the cube.
On release, the cube launches forward using physics.

Cubes merge only if:

They have equal values
Collision impulse exceeds a defined threshold
Collision direction is valid

Each successful merge grants:
Score = merged value / 4

The player wins upon reaching 200 points, after which a win panel allows restart or exit.

Architecture & Design Approach
------------------------------
The project follows an event-driven structure to reduce tight coupling between systems.

Cubes raise merge events.
The score system reacts to those events.
The game state controller reacts to score updates.
This ensures loose coupling and keeps gameplay logic independent from UI and global state management.
The scene hierarchy and project folders are organized to reflect a scalable structure suitable for further development.

 Technical Highlights
----------------------
Physics-based merge validation (impulse + directional check)
Clean separation of gameplay logic and UI
Mobile and Editor input support
Defensive coding practices
Organized scene hierarchy and asset structure
Android-ready build

 Possible Extensions
----------------------
With additional development time, I would extend the project with:
Object pooling for cubes
ScriptableObject-driven configuration
Addressables integration
Enhanced feedback (particles, squash & stretch, layered audio)

Final Note
-------------
This prototype represents my approach to Unity development:

Structured thinking
Maintainable and scalable architecture
Gameplay-first mindset
Clean and readable code