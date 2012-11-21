using System;
using System.Collections.Generic;
using MTV3D65;

namespace ComponentFramework.Core
{
    /// <summary>
    /// The public interface for the <see cref="Core"/> class.
    /// </summary>
    public interface ICore
    {
        /// <summary>
        /// Switches the rendering from windowed mode to fullscreen mode.
        /// </summary>
        void SwitchWindowed();

        /// <summary>
        /// Switches the rendering from fullscreen mode to windowed mode.
        /// </summary>
        void SwitchFullscreen();

        /// <summary>
        /// Gets the singleton service reference from an interface type.
        /// </summary>
        /// <typeparam name="T">The interface type to search for</typeparam>
        /// <returns>The matching service implementation</returns>
        /// <exception cref="KeyNotFoundException">If the service cannot be found</exception>
        T GetService<T>() where T : IService;

        /// <summary>
        /// Injects service dependencies into any class that has service dependencies.
        /// </summary>
        /// <param name="instance">The class instance to process</param>
        void InjectServices(object instance);

        /// <summary>
        /// Creates a component from a type and loads it.
        /// </summary>
        /// <typeparam name="T">The component type to create</typeparam>
        /// <returns>The created, loaded but not yet initialized component</returns>
        T CreateAndLoadComponent<T>() where T : Component;

        /// <summary>
        /// Creates a component from a type and loads it.
        /// </summary>
        /// <param name="componentType">The component type to create</param>
        /// <returns>The created, loaded and initialized component</returns>
        T CreateAndLoadComponent<T>(Type componentType) where T : Component;

        /// <summary>
        /// Loads an already-instantiated component. 
        /// The initialization is deferred to the next loop.
        /// </summary>
        /// <param name="component">The component to load</param>
        /// <returns>The created, loaded and initialized component</returns>
        T LoadComponent<T>(T component) where T : Component;

        /// <summary>
        /// Unloads a component, which schedules its removal and dispose.
        /// </summary>
        /// <param name="component">The component to remove</param>
        void UnloadComponent(Component component);

        /// <summary>
        /// Re-calls the <see cref="Component.Draw()"/> method of all loaded components.
        /// </summary>
        void ReDraw();

        /// <summary>
        /// Stops looping and exits the application.
        /// </summary>
        void Exit();

        /// <summary>
        /// Provides access to the only <see cref="TVEngine"/> reference.
        /// </summary>
        TVEngine Engine { get; }

        /// <summary>A reference to the <see cref="TVAtmosphere"/> singleton.</summary>
        TVAtmosphere Atmosphere { get; }
        /// <summary>A reference to the <see cref="TVCameraFactory"/> singleton.</summary>
        TVCameraFactory CameraFactory { get; }
        /// <summary>A reference to the <see cref="TVGlobals"/> singleton.</summary>
        TVGlobals Globals { get; }
        /// <summary>A reference to the <see cref="TVInputEngine"/> singleton.</summary>
        TVInputEngine InputEngine { get; }
        /// <summary>A reference to the <see cref="TVInternalObjects"/> singleton.</summary>
        TVInternalObjects InternalObjects { get; }
        /// <summary>A reference to the <see cref="TVLightEngine"/> singleton.</summary>
        TVLightEngine LightEngine { get; }
        /// <summary>A reference to the <see cref="TVMathLibrary"/> singleton.</summary>
        TVMathLibrary MathLibrary { get; }
        /// <summary>A reference to the <see cref="TVPhysics"/> singleton.</summary>
        TVPhysics Physics { get; }
        /// <summary>A reference to the <see cref="TVScene"/> singleton.</summary>
        TVScene Scene { get; }
        /// <summary>A reference to the <see cref="TVScreen2DImmediate"/> singleton.</summary>
        TVScreen2DImmediate Screen2DImmediate { get; }
        /// <summary>A reference to the <see cref="TVScreen2DText"/> singleton.</summary>
        TVScreen2DText Screen2DText { get; }
        /// <summary>A reference to the <see cref="TVTextureFactory"/> singleton.</summary>
        TVTextureFactory TextureFactory { get; }
        /// <summary>A reference to the <see cref="TVMaterialFactory"/> singleton.</summary>
        TVMaterialFactory MaterialFactory { get; }
        /// <summary>A reference to the <see cref="TVGameControllers"/> singleton.</summary>
        TVGameControllers GameControllers { get; }
        /// <summary>A reference to the <see cref="TVDeviceInfo"/> singleton.</summary>
        TVDeviceInfo DeviceInfo { get; }
        /// <summary>A reference to the <see cref="TVGraphicEffect"/> singleton.</summary>
        TVGraphicEffect GraphicEffect { get; }

        /// <summary>
        /// Provides access to the engine initialization settings.
        /// </summary>
        EngineSettings Settings { get; }

        /// <summary>
        /// Provides access to the content root directory.
        /// </summary>
        string ContentRoot { get; }

        /// <summary>
        /// Provides access to the engine initialization settings.
        /// </summary>
        //Form RenderForm { get; }
    }
}
