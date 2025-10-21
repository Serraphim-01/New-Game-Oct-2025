# Camera Follow Implementation TODO

- [x] Create CameraFollow.cs script with public fields for player transform, follow speed, camera distance, height offset, and wall layer mask.
- [x] Implement smooth interpolation using Vector3.SmoothDamp to follow the player.
- [x] Add collision avoidance logic using Physics.Raycast to detect walls and adjust camera position to avoid clipping.
- [x] Use LateUpdate to calculate desired position, apply smoothing, check collisions, and set camera position/rotation to look at player.
- [x] Attach script to Main Camera in Unity scene and assign player transform and wall layer.
- [x] Test camera follow in Unity Editor for smooth movement and wall avoidance.

# UI Management TODO

- [x] Create GameUIManager.cs script to handle the main game UI, including the Mission button.
- [x] Create MissionUIManager.cs script to manage the mission UI.
- [x] Ensure MissionUIManager is disabled on game start.
- [x] Implement button click handler in GameUIManager to enable MissionUIManager when Mission button is clicked.
- [x] Attach scripts to appropriate GameObjects in the Unity scene and assign references.
- [x] Test the UI interaction in Unity Editor.

# Cursor Management TODO

- [x] Modify CameraFollow.cs to hide cursor on game start.
- [x] Implement Escape key toggle for cursor visibility and lock state.
- [x] Ensure cursor remains visible after Escape until pressed again, allowing UI interactions.
- [x] Test cursor toggle functionality in Unity Editor.

# UI Styling TODO

- [x] Style Game UI with Mission button on left, zone text in center, and right panel for future use.
- [x] Style Mission UI as right-aligned panel, max 500px width, with space above/below.
- [x] Integrate zone text display into UI Toolkit instead of standalone Text UI element.
- [x] Update ZoneManager.cs to reference GameUIManager instead of Text component.
- [x] Update GameUIManager.cs to handle zone text updates via UI Toolkit Label.
- [x] Test UI styling and zone text display in Unity Editor.
