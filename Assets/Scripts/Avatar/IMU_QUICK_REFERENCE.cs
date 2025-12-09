/**
 * IMU AVATAR MOVEMENT SYSTEM - QUICK REFERENCE
 * ============================================
 * 
 * SYSTEM ARCHITECTURE
 * -------------------
 * 
 *    [Physical Device Sensors]
 *              ↓
 *    [Input.gyro API - Unity]
 *              ↓
 *    ┌─────────────────────┐
 *    │   IMUProvider       │ ← Reads sensors, calibrates
 *    │  - Attitude         │
 *    │  - Acceleration     │
 *    │  - Gyroscope        │
 *    └─────────────────────┘
 *              ↓
 *    ┌─────────────────────────────┐
 *    │   IMUBodyIntegrator         │ ← Main controller (RECOMMENDED)
 *    │  - Maps IMU to bones        │
 *    │  - Manages calibration      │
 *    │  - Multiple body parts      │
 *    └─────────────────────────────┘
 *         ↓                ↓
 *    ┌──────────┐   ┌───────────────┐
 *    │AvatarCtrl│   │ BodyTracker   │
 *    │- Bones   │   │- Transform    │
 *    │- Visual  │   │- Movement     │
 *    └──────────┘   └───────────────┘
 *         ↓                ↓
 *    [Avatar Mesh]    [Body GameObject]
 * 
 * 
 * QUICK START - 3 STEPS
 * ---------------------
 * 
 * 1. ADD COMPONENT
 *    Hierarchy → Create Empty → Name: "IMU_System"
 *    Add Component → IMUBodyIntegrator
 * 
 * 2. PRESS PLAY
 *    Components auto-connect
 * 
 * 3. CALIBRATE
 *    Stand in neutral pose
 *    Press 'C' key
 *    Done!
 * 
 * 
 * KEY BINDINGS
 * ------------
 * C = Calibrate (set current pose as zero)
 * R = Reset tracking
 * Space = Simulate movement (editor only)
 * Arrow Keys = Rotate body (editor only)
 * 
 * 
 * INSPECTOR SETTINGS
 * ------------------
 * 
 * IMUBodyIntegrator:
 *   enableTracking: true/false
 *   globalSmoothingFactor: 0.15 (default)
 *   bodyPartMappings: Array of bone mappings
 *     - targetBone: Which bone to control
 *     - rotationOffset: Adjust rotation (degrees)
 *     - invertX/Y/Z: Flip axes if needed
 * 
 * IMUProvider:
 *   (No configuration needed - auto setup)
 * 
 * BodyTracker:
 *   trackRotation: true
 *   trackPosition: false (optional, may drift)
 *   rotationSmoothing: 5
 * 
 * 
 * CODE EXAMPLES
 * -------------
 * 
 * // Get the integrator
 * IMUBodyIntegrator integrator = FindObjectOfType<IMUBodyIntegrator>();
 * 
 * // Calibrate
 * integrator.CalibrateAll();
 * 
 * // Enable/disable tracking
 * integrator.SetTrackingEnabled(true);
 * 
 * // Add new bone
 * integrator.AddBodyPartMapping(HumanBodyBones.Head, Vector3.zero);
 * 
 * // Get IMU data directly
 * IMUProvider imu = FindObjectOfType<IMUProvider>();
 * Quaternion rotation = imu.Attitude;
 * Vector3 accel = imu.Acceleration;
 * 
 * 
 * TROUBLESHOOTING
 * ---------------
 * 
 * Problem: Avatar doesn't move
 * Solution: 
 *   - Press 'C' to calibrate
 *   - Check IMUProvider exists
 *   - Check Console for errors
 * 
 * Problem: Wrong direction
 * Solution:
 *   - Adjust rotationOffset in mappings
 *   - Try invertX/Y/Z options
 *   - Recalibrate
 * 
 * Problem: Jittery movement
 * Solution:
 *   - Increase smoothing factor
 *   - Reduce update rate
 * 
 * Problem: Position drifts
 * Solution:
 *   - Normal with IMU only
 *   - Disable position tracking
 *   - Use external tracking
 * 
 * 
 * ALTERNATIVE SETUPS
 * ------------------
 * 
 * Option 1: IMUBodyIntegrator (Recommended)
 *   ✓ Easiest to use
 *   ✓ Multiple bones
 *   ✓ Auto-setup
 *   Use: Add to empty GameObject
 * 
 * Option 2: IMUAvatarController
 *   ✓ Simpler, single bone
 *   ✓ Direct control
 *   Use: Add to Avatar GameObject
 * 
 * Option 3: BodyTracker Only
 *   ✓ Minimal, just transform
 *   ✓ No avatar needed
 *   Use: Add to body GameObject
 * 
 * 
 * PERFORMANCE
 * -----------
 * CPU: ~0.1ms per frame
 * Memory: ~1KB per component
 * Update: 60 Hz
 * Latency: <16ms
 * 
 * 
 * SUPPORTED PLATFORMS
 * -------------------
 * ✓ Android (gyro required)
 * ✓ iOS (gyro required)
 * ✓ Unity Editor (simulation)
 * ✗ Desktop (no gyro)
 * 
 * 
 * FILES REFERENCE
 * ---------------
 * Core:
 *   - IMUProvider.cs (sensor data)
 *   - IMUBodyIntegrator.cs (main controller)
 *   - IMUAvatarController.cs (direct bone control)
 *   - BodyTracker.cs (body transform)
 *   - AvatarController.cs (avatar bones)
 * 
 * Docs:
 *   - IMU_SETUP_GUIDE.md (detailed guide)
 *   - IMPLEMENTATION_SUMMARY.md (overview)
 *   - IMU_QUICK_REFERENCE.cs (this file)
 * 
 * Examples:
 *   - IMUAvatarExample.cs (usage examples)
 * 
 * 
 * COMMON BODY BONES
 * -----------------
 * HumanBodyBones.Hips
 * HumanBodyBones.Spine
 * HumanBodyBones.Chest
 * HumanBodyBones.Head
 * HumanBodyBones.LeftUpperArm
 * HumanBodyBones.RightUpperArm
 * HumanBodyBones.LeftLowerArm
 * HumanBodyBones.RightLowerArm
 * HumanBodyBones.LeftUpperLeg
 * HumanBodyBones.RightUpperLeg
 * HumanBodyBones.LeftLowerLeg
 * HumanBodyBones.RightLowerLeg
 * 
 * 
 * DEBUG INFO
 * ----------
 * All components show on-screen debug info when playing.
 * 
 * IMUProvider (lower right):
 *   - Raw attitude
 *   - Corrected attitude
 * 
 * IMUBodyIntegrator (lower left):
 *   - Tracking status
 *   - Connected components
 *   - Active mappings
 * 
 * IMUAvatarController (left):
 *   - Position
 *   - Rotation
 *   - Velocity
 * 
 * 
 * NEXT STEPS
 * ----------
 * 1. Test in editor (Space key)
 * 2. Deploy to device (Android/iOS)
 * 3. Calibrate on device (C key)
 * 4. Tune rotation offsets
 * 5. Add more body parts
 * 6. Integrate with exercises
 * 
 * 
 * SUPPORT
 * -------
 * Check Console for logs/errors
 * Read IMU_SETUP_GUIDE.md for details
 * See IMUAvatarExample.cs for code samples
 * 
 */

// This file is documentation only - no code to compile
