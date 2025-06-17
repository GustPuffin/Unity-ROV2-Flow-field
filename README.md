# Unity-RVO2-Flow-Field

## Introduction

This project is developed using Unity 6000.0.49f1, integrating FlowField Pathfinding, dynamic obstacle avoidance, dynamic obstacle management, and multi-target control. It is suitable for tower defense or large-scale unit movement scenarios. The project utilizes the following technologies and frameworks:

- **Unity 6000.0.49f1**
- **Zenject** for scene dependency injection
- **Flow Field Pathfinding** for efficient multi-agent navigation
- **RVO2 Library (modified version)** for dynamic obstacle avoidance and unit steering

The system supports smooth movement for large-scale units, real-time obstacle placement, complex map navigation, and provides rich debugging and visualization tools to assist development and tuning.

---

## Feature Modules

### Grid System

- Dynamic grid partitioning and coordinate transformation, supporting floating-point precision and center alignment.
- Adjustable map size and grid resolution for different levels.

### Flow Field System

- BFS search to build the global cost field, with shared path information for all units.
- Asynchronous construction and partial field refresh for performance optimization.

### Unit Movement Control

- Real-time movement based on flow field directions, with adaptive directional fusion based on unit size:
  - Small units: direct direction reading
  - Medium units: bilinear interpolation (under development)
  - Large units: multi-cell weighted blending (under development)
- Smooth movement, dynamic speed adjustment, and smooth rotation (currently under debugging).

### Dynamic Obstacle Management

- Runtime obstacle add/remove support.
- Partial flow field rebuild to avoid full recalculation.
- Accurate obstacle mapping and debugging visualization support.

### Multi-Target Pathfinding Support

- Supports multiple target registrations and flow field caching.
- Units dynamically select target flow fields based on type and logic.
- Target switching does not require full flow field reconstruction.

---

## Obstacle Avoidance Modifications (Based on RVO2)

The obstacle avoidance logic in this project is based on [RVO2-CS](https://github.com/snape/RVO2-CS), with the following modifications to adapt to Unity environment and project requirements:

- Integrated with Unity JobSystem to improve performance
- Runtime agent and obstacle add/remove support
- Adjustments and simplifications to API interfaces
- Support for multiple simulator instances running in parallel
- New sample scenes for better understanding and application

Since the original library uses the Apache 2.0 license, all modifications have been publicly released in this project following the original license terms.

---

## License

This project code is licensed under MIT License.

The avoidance part is based on [RVO2-CS](https://github.com/snape/RVO2-CS) under Apache 2.0 license.
