# Tank Destroyer

A 3D top-down tank survival game built with Unity where you control a tank and must survive against endless waves of enemies.

## Features

- **Third-person tank combat** - Control a tank with smooth movement and camera following via Cinemachine
- **Auto-aim turret** - Tank turret automatically tracks and aims at the nearest enemy
- **Progressive difficulty** - Enemy spawn rate increases over time, making survival increasingly challenging
- **Multiple enemy types**:
    - _Easy enemies_ - Use evasion tactics, moving away from the player
    - _Hard enemies_ - Aggressively chase the player
- **Health system** - Both player and enemies have health with visual health bars
- **Audio feedback** - Sound effects for shooting and taking damage
- **Particle effects** - Muzzle flash effects when firing

## Requirements

- **Unity 6000.3.7f1** (Unity 6)
- Windows/macOS/Linux

## Installation

1. Clone the repository:
    ```bash
    git clone https://github.com/yourusername/Tank-Destroyer.git
    ```
2. Open the project in Unity Hub
3. Open `Assets/Scenes/Menu.unity` or `Assets/Scenes/Game Scene.unity`
4. Press Play to run the game

## Controls

| Action        | Key   |
| ------------- | ----- |
| Move Forward  | W     |
| Move Backward | S     |
| Move Left     | A     |
| Move Right    | D     |
| Shoot         | Space |

## Project Structure

```
Assets/
├── Animations/      # Character and tank animations
├── Audio/           # Sound effects and music
├── Imported Assets/ # Third-party assets
├── Materials/       # Material definitions
├── Models/          # 3D models
├── Prefabs/         # Reusable game objects
├── Scenes/          # Game scenes (Menu, Game Scene)
├── Scripts/         # C# source code
│   ├── Bullets/     # Bullet behavior
│   ├── Enemy/       # Enemy AI and spawning
│   ├── Game/        # Core game systems
│   ├── Player/      # Player controls
│   └── UI/          # User interface
└── Sprites/         # 2D sprites and textures
```

## Architecture

The project uses several design patterns for clean, maintainable code:

### Factory Pattern

Enemy creation is handled through an abstract factory:

- `IEnemyFactory` - Interface for enemy factories
- `EasyEnemyFactory` - Creates enemies with evasion behavior
- `HardEnemyFactory` - Creates enemies that chase the player

### Strategy Pattern

Enemy behavior is implemented using the Strategy pattern:

- `IEnemyBehaviorStrategy` - Interface for enemy behaviors
- `ChasePlayerStrategy` - Moves toward the player
- `EvadeStrategy` - Moves away from the player

### Singleton Pattern

`GameManager` uses singleton pattern for global game state management

### Component Pattern

Health system uses `HealthComponent` implementing `IDamageable` interface for reusable damage handling

## Key Packages

- **Cinemachine 3.1.6** - Advanced camera system
- **Timeline 1.8.10** - Sequencing and cinematics
- **Unity UI 2.0.0** - User interface

## Scripts Overview

| Script            | Description                                   |
| ----------------- | --------------------------------------------- |
| `GameManager`     | Handles enemy spawning and difficulty scaling |
| `PlayerMovement`  | WASD movement with camera-relative direction  |
| `PlayerShooting`  | Auto-aim and firing mechanics                 |
| `EnemyLogic`      | Enemy behavior execution                      |
| `EnemyFactory`    | Factory classes for enemy instantiation       |
| `EnemyStrategy`   | Behavior strategies                           |
| `HealthComponent` | Reusable health and damage system             |
| `Bullet`          | Projectile behavior                           |
