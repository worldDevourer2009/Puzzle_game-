# Portfolio Game

A Unity 3D game project built with Unity 6000.0.36f1 and modern C# development practices.

## Project Overview

This project demonstrates a modular and maintainable architecture for an interactive Unity game. 

It’s a small puzzle game where gameplay is driven by trigger-based logic defined via level configurations — enabling flexible sequencing of actions and states.

The goal of this project is to showcase clean code structure, scalable systems, and advanced usage of async programming, dependency injection, and custom interaction handling.

## Technical Specifications

- **Unity Version**: 6000.0.36f1
- **Target Framework**: .NET Framework 4.7.1
- **Language**: C# 9.0

## Key Dependencies

- [UniTask](https://github.com/Cysharp/UniTask)
- [Zenject](https://github.com/modesttree/Zenject)
- [DOTween](http://dotween.demigiant.com/)
- [R3](https://github.com/Cysharp/R3))

## Core Features

### Interactive Systems
- Physics-based interactive elements
- Modular activation/deactivation logic
- Collision-based interaction detection
- Smooth animation transitions

### Architecture
- Async programming with UniTask integration
- Component-based modular design
- Dependency injection implementation
- Comprehensive logging system
- Clean separation of concerns

## Development Approach

The project implements modern Unity development standards including async/await patterns for non-blocking operations, modular component architecture for reusability, 
comprehensive error handling, and maintainable code structure.

## Project Structure

Assets/
├── Scripts/
│   ├── Core/                    # Core game systems
│   │   ├── AddressableS/        # Asset loading system
│   │   ├── AsyncManagement/     # Async operation handling
│   │   ├── AudioSystem/         # Audio management
│   │   ├── CameraManagement/    # Camera controls
│   │   ├── Configs/             # Configuration files
│   │   ├── Const/               # Constants and enums
│   │   ├── Data/                # Data structures
│   │   ├── Factories/           # Object creation patterns
│   │   ├── GameLoopSystem/      # Game loop management
│   │   ├── GameManagement/      # Core game state
│   │   ├── InputSystemCustom/   # Input handling
│   │   ├── Interactions/        # Interaction framework
│   │   ├── LevelManagement/     # Level loading/unloading
│   │   ├── Localisation/        # Localization system
│   │   ├── Logs/                # Logging framework
│   │   ├── MainInterfaces/      # Core interfaces
│   │   ├── Player/              # Player core systems
│   │   ├── Pools/               # Object pooling
│   │   ├── RaycastSystem/       # Raycasting utilities
│   │   ├── SaveSystem/          # Save/load functionality
│   │   ├── SceneManagement/     # Scene transitions
│   │   └── Triggers/            # Trigger systems
│
│   ├── Game/                    # Game-specific logic
│   │   ├── ActivatableObjects/  # Objects with activation states
│   │   ├── Animations/          # Game animations
│   │   ├── Camera/              # Game camera behavior
│   │   ├── Interactions/        # Game-specific interactions
│   │   ├── InteractableObjects/ # Interactive game elements
│   │   ├── Interfaces/          # Game interfaces
│   │   ├── Logics/              # Game logic components
│   │   ├── Player/              # Player-specific components
│   │   ├── RaycastSystem/       # Game raycasting
│   │   └── Sounds/              # Sound effects
│
│   ├── UI/                      # User interface
│   │   ├── Extensions/          # UI extension methods
│   │   ├── Models/              # UI data models
│   │   ├── Presenters/          # MVP pattern presenters
│   │   ├── Views/               # UI view components
│   │   ├── UIInitializer.cs     # UI system initialization
│   │   ├── UIRoot.cs            # UI root controller
│   │   └── UISystem.cs          # UI management system
│
│   ├── EntryPoint/              # Application entry points
│   ├── Extensions/              # C# extension methods
│   ├── Installers/              # Dependency injection setup
│   └── Editor/                  # Editor tools and utilities
