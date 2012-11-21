using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using ComponentFramework.Tools;
using MTV3D65;

namespace ComponentFramework.Core
{
    /// <summary>
    /// The base core class for the component framework.
    /// This class handles the application start, looping and end, and provides API to load and unload components.
    /// It also has access to TV3D singletons, and exclusive access to the <see cref="TVEngine"/> reference.
    /// </summary>
    public partial class Core : ICore, IDisposable
    {
        /// <summary>
        /// The current run mode.
        /// </summary>
        readonly RunMode runMode;
        /// <summary>
        /// The looping manager reference.
        /// </summary>
        Looping looping;
        /// <summary>
        /// The current engine settings.
        /// </summary>
        EngineSettings settings;
        /// <summary>
        /// The currently loaded components.
        /// </summary>
        readonly List<Component> components = new List<Component>();
        /// <summary>
        /// The components added in the current loop and that will be added before the next loop.
        /// </summary>
        readonly List<Component> componentsToAdd = new List<Component>();
        /// <summary>
        /// The components removed in the current loop and that will be removed before the next loop.
        /// </summary>
        readonly List<Component> componentsToRemove = new List<Component>();

        /// <summary>
        /// The currently loaded non-component services.
        /// </summary>
        readonly List<IService> nonComponentServices = new List<IService>();
        /// <summary>
        /// The service interface map, including all base interfaces.
        /// </summary>
        readonly Dictionary<Type, IService> serviceMap = new Dictionary<Type, IService>();

        /// <summary>
        /// The elapsed time stopwatch.
        /// </summary>
        readonly Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// Constructor type arguments for component/service loading.
        /// </summary>
        readonly static Type[] ctorTypes = new[] { typeof(ICore) };
        /// <summary>
        /// Constructor parameters for component/service loading.
        /// </summary>
        readonly object[] ctorParams;
        /// <summary>
        /// Has the Core been initialized yet?
        /// </summary>
        bool initialized;

        /// <summary>
        /// The content root directory
        /// </summary>
        string contentRoot;

        /// <summary>
        /// Screen width.
        /// </summary>
        int screenWidth;

        /// <summary>
        /// Screen height.
        /// </summary>
        int screenHeight;

        /// <summary>
        /// Parameterless constructor.
        /// Initializes references and identifies the run mode.
        /// </summary>
        public Core()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Application.ThreadException += new ThreadExceptionEventHandler(UIThreadException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);
            ctorParams = new object[] { this };

#if DEBUG
            runMode = RunMode.Debug;
#else
            runMode = RunMode.Release;
#endif

            Atmosphere = new TVAtmosphere();
            CameraFactory = new TVCameraFactory();
            Globals = new TVGlobals();
            InputEngine = new TVInputEngine();
            InternalObjects = new TVInternalObjects();
            LightEngine = new TVLightEngine();
            MathLibrary = new TVMathLibrary();
            Physics = new TVPhysics();
            Scene = new TVScene();
            Screen2DImmediate = new TVScreen2DImmediate();
            Screen2DText = new TVScreen2DText();
            TextureFactory = new TVTextureFactory();
            MaterialFactory = new TVMaterialFactory();
            GameControllers = new TVGameControllers();
            DeviceInfo = new TVDeviceInfo();
            GraphicEffect = new TVGraphicEffect();            
        }

        /// <summary>
        /// Schedules the engine initialization and starts the message loop with default settings.
        /// </summary>
        public void Run()
        {
            Run(new EngineSettings());
        }

        /// <summary>
        /// Schedules the engine initialization and starts the message loop.
        /// </summary>
        /// <param name="initializationSettings">The engine settings to use.</param>
        public void Run(EngineSettings initializationSettings)
        {
            settings = initializationSettings;

            Application.Idle += InitializeEngine;
            Application.Run();
        }

        /// <summary>
        /// Initializes the engine, services and components.
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        void InitializeEngine(object sender, EventArgs e)
        {
            // Attach the looping events
            looping = new Looping(this);

            Engine = new TVEngine();
#if DEBUG
            Engine.SetDebugMode(true, true, true, false);
            Engine.SetDebugFile(settings.DebugFile.FullName);
#else
            Engine.SetDebugMode(false, false, false, false);
            Engine.DisplayFPS(false);
#endif
            Engine.AllowMultithreading(settings.MultiThreading);

            if (File.Exists(GetType().Assembly.GetName().Name + ".lic"))
                Engine.SetLicenseFile(GetType().Assembly.GetName().Name + ".lic");

            // When we use doubles, we need double-precision.
            Engine.SetFPUPrecision(true);

            // Set default component update frequency value.
            if (settings.UpdateFrequency.Equals(0f))
                settings.UpdateFrequency = 1f / 60f;

            SetMultisample();

            // Create render form.
            settings.RenderForm = new RenderForm();

            // Apply v sync option.
            Engine.SetVSync(settings.VSync);

            // Use current settings by default if none are given.
            screenWidth = Screen.PrimaryScreen.Bounds.Width;
            screenHeight = Screen.PrimaryScreen.Bounds.Height;

            if (settings.ScreenMode.Width == 0 || settings.ScreenMode.Height == 0)
            {
                settings.ScreenMode.Width = screenWidth;
                settings.ScreenMode.Height = Screen.PrimaryScreen.Bounds.Height;
            }
            if (settings.ScreenMode.Format == 0)
                settings.ScreenMode.Format = screenHeight;

            bool initSuccess = false;

            if (settings.Fullscreen)
            {
                try
                {
                    initSuccess = Engine.Init3DFullscreen(settings.ScreenMode.Width, settings.ScreenMode.Height,
                        settings.ScreenMode.Format, true, settings.VSync,
                        CONST_TV_DEPTHBUFFERFORMAT.TV_DEPTHBUFFER_BESTBUFFER, 1, settings.RenderForm.Handle);
                }
                catch (AccessViolationException) { /* License failure */ }
            }
            else
            {
                try
                {
                    // Resize windowed form and move to center screen.
                    settings.RenderForm.Size = new System.Drawing.Size(settings.ScreenMode.Width, settings.ScreenMode.Height);
                    settings.RenderForm.Top = (screenHeight - settings.ScreenMode.Height) / 2;
                    settings.RenderForm.Left = (screenWidth - settings.ScreenMode.Width) / 2;

                    initSuccess = Engine.Init3DWindowed(settings.RenderForm.Handle, true);
                }
                catch (AccessViolationException) { /* License failure */ }
            }

            // Apply auto resize.
            Engine.GetViewport().SetAutoResize(true);

            // Probably a license failure
            if (!initSuccess)
            {
                MessageBox.Show("Couldn't initialize engine! Exiting.", "Critical error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Idle -= InitializeEngine;
                Dispose();
                Application.Exit();
                return;
            }

            // Initialize physics
            Physics.Initialize();
            Physics.SetSolverModel(CONST_TV_PHYSICS_SOLVER.TV_SOLVER_ADAPTIVE);
            Physics.SetFrictionModel(CONST_TV_PHYSICS_FRICTION.TV_FRICTION_ADAPTIVE);
            Physics.SetGlobalGravity(new TV_3DVECTOR(0, -14.8f, 0));
            Physics.EnableCPUOptimizations(true);

            // Initialize input
            InputEngine.Initialize(true, true);

            // If nothing is found, just don't hook the events
            if (settings.RenderForm != null)
            {
                settings.RenderForm.FormClosing += (_, __) => Exit();
                //settings.RenderForm.Deactivate += (_, __) => looping.PauseLoop();
                //settings.RenderForm.Activated += delegate
                //{
                //    // Silly workaround
                //    InputEngine.ClearKeyBuffer();
                //    InputEngine.ForceUpdate();
                //    stopwatch.Reset();
                //    looping.ResumeLoop();
                //};
            }

            Initialize();

            if (!settings.Fullscreen)
                settings.RenderForm.Show();

            Application.Idle -= InitializeEngine;
        }

        /// <summary>
        /// Switches the rendering from windowed mode to full screen mode.
        /// </summary>
        public void SwitchWindowed()
        {
            settings.Fullscreen = false;
            SetMultisample();
            Engine.SwitchWindowed(settings.RenderForm.Handle);
            settings.RenderForm.WindowState = FormWindowState.Normal;
            settings.RenderForm.Size = new System.Drawing.Size(settings.ScreenMode.Width, settings.ScreenMode.Height);
            settings.RenderForm.Top = (screenHeight - settings.ScreenMode.Height) / 2;
            settings.RenderForm.Left = (screenWidth - settings.ScreenMode.Width) / 2;
        }

        /// <summary>
        /// Switches the rendering from windowed mode to full screen mode.
        /// </summary>
        public void SwitchFullscreen()
        {
            settings.Fullscreen = true;
            SetMultisample();
            Engine.SwitchFullscreen(settings.ScreenMode.Width,
                settings.ScreenMode.Height,
                settings.ScreenMode.Format,
                CONST_TV_DEPTHBUFFERFORMAT.TV_DEPTHBUFFER_BESTBUFFER,
                Settings.RenderForm.Handle);
        }

        /// <summary>
        /// Sets the multi sample mode.
        /// </summary>
        private void SetMultisample()
        {
            if (settings.MultisampleType != CONST_TV_MULTISAMPLE_TYPE.TV_MULTISAMPLE_NONE)
                Engine.SetAntialiasing(true, settings.MultisampleType);
            else
                Engine.SetAntialiasing(false, CONST_TV_MULTISAMPLE_TYPE.TV_MULTISAMPLE_NONE);
        }

        /// <summary>
        /// Performs additional initialization before auto-loading services and components.
        /// The base implementation MUST BE CALLED after specific initialization is performed, 
        /// as it initializes all components and services.
        /// </summary>
        protected virtual void Initialize()
        {
            InitializeComponentsAndServices();
            initialized = true;
        }

        /// <summary>
        /// Stops looping and exits the application.
        /// </summary>
        public void Exit()
        {
            looping.StopLoop();
            Dispose();
        }

        /// <summary>
        /// Finds all auto-loaded components and enabled services in the core implementation assembly and loads them.
        /// If the <see cref="Core"/> class is directly used and not overridden, nothing will be loaded!
        /// </summary>
        void InitializeComponentsAndServices()
        {
            // This assembly and the host assembly are included
            foreach (Type type in GetType().Assembly.GetTypes().Where(t => t.IsClass).Union(typeof(Core).Assembly.GetTypes().Where(t => t.IsClass)))
            {
                object instance = null;

                bool isComponent = typeof(Component).IsAssignableFrom(type);
                bool isAutoLoaded = false;
                if (isComponent)
                {
                    var attribute = ReflectionHelper.GetAttribute<AutoLoadAttribute>(type);
                    isAutoLoaded = attribute != null;
                    if (isAutoLoaded && (attribute.RunModeRestriction != RunMode.Always && attribute.RunModeRestriction != runMode))
                        continue;
                }

                if (isComponent && isAutoLoaded)
                {
                    instance = type.GetConstructor(ctorTypes).Invoke(ctorParams);
                    components.Add(instance as Component);
                }

                if (!isComponent || isAutoLoaded)
                    RegisterServices(type, instance);
            }

            foreach (var service in nonComponentServices)
                InjectServices(service);

            InjectServices(this);


            components.Sort();
            foreach (var component in components)
            {
                InjectServices(component);
                component.Initialize();
            }

            foreach (var component in components)
            {
                InjectServices(component);
                component.PostInitialize();
            }
        }

        void RegisterServices(Type type, object instance)
        {
            // Can't add services after the core has been initialized (services are singleton, app-global)
            if (initialized)
                return;

            // Make sure it is a service
            bool isService = typeof(IService).IsAssignableFrom(type);
            if (!isService) return;

            if (instance == null)
                try { instance = ReflectionHelper.Instantiate(type); }
                catch (MissingMethodException)
                {
                    throw new InvalidOperationException("Service of type " + type.Name +
                                                        " could not be instantiated; make sure it has a parameterless constructor.");
                }

            var service = instance as IService;
            if (!(service is Component))
                nonComponentServices.Add(service);

            foreach (Type serviceInterface in from i in type.GetInterfaces()
                                              where typeof(IService).IsAssignableFrom(i) && i != typeof(IService)
                                              select i)
            {
                if (ReflectionHelper.GetAttribute<DisableAttribute>(serviceInterface) == null)
                    serviceMap.Add(serviceInterface, service);
            }
        }

        /// <summary>
        /// Injects service dependencies into any class that has service dependencies.
        /// </summary>
        /// <param name="instance">The class instance to process</param>
        public void InjectServices(object instance)
        {
            const BindingFlags NonPublicFlattenedInstanceMembers =
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

            var type = instance.GetType();

            foreach (PropertyInfo property in type.GetProperties(NonPublicFlattenedInstanceMembers))
            {
                var attributes = property.GetCustomAttributes(typeof(ServiceDependencyAttribute), true);
                if (attributes.Length == 0) continue;

                var dependentServiceType = property.PropertyType;
                IService dependentSevice;
                bool serviceFound = serviceMap.TryGetValue(dependentServiceType, out dependentSevice);
                var attribute = attributes[0] as ServiceDependencyAttribute;

                if (!serviceFound)
                {
                    if (!attribute.Optional)
                        throw new MissingServiceException(type, dependentServiceType);
                }
                else
                {
                    var setter = property.GetSetMethod(true);
                    if (setter == null)
                        throw new MissingSetterException(type, dependentServiceType);

                    setter.Invoke(instance, new[] { dependentSevice });
                }
            }
        }

        /// <summary>
        /// Gets the singleton service reference from an interface type.
        /// </summary>
        /// <typeparam name="T">The interface type to search for</typeparam>
        /// <returns>The matching service implementation</returns>
        /// <exception cref="KeyNotFoundException">If the service cannot be found</exception>
        public T GetService<T>() where T : IService
        {
            var service = (T)serviceMap[typeof(T)];
            return service;
        }

        /// <summary>
        /// Creates a component from a type and loads it.
        /// </summary>
        /// <typeparam name="T">The component type to create</typeparam>
        /// <returns>The created, loaded but not yet initialized component</returns>
        public T CreateAndLoadComponent<T>() where T : Component
        {
            var component = typeof(T).GetConstructor(ctorTypes).Invoke(ctorParams) as T;
            LoadComponent(component);
            return component;
        }

        /// <summary>
        /// Creates a component from a type and loads it.
        /// </summary>
        /// <param name="componentType">The component type to create</param>
        /// <returns>The created, loaded but not yet initialized component</returns>
        public T CreateAndLoadComponent<T>(Type componentType) where T : Component
        {
            var component = componentType.GetConstructor(ctorTypes).Invoke(ctorParams) as T;
            LoadComponent(component);
            return component;
        }

        /// <summary>
        /// Loads an already-instantiated component. 
        /// </summary>
        /// <param name="component">The component to load</param>
        public T LoadComponent<T>(T component) where T : Component
        {
            componentsToAdd.Add(component);
            RegisterServices(component.GetType(), component);
            InjectServices(component);
            component.Initialize();
            //component.PostInitialize();
            return component;
        }

        /// <summary>
        /// Unloads a component, which schedules its removal and disposal.
        /// </summary>
        /// <param name="component">The component to remove</param>
        public void UnloadComponent(Component component)
        {
            componentsToRemove.Add(component);
        }

        /// <summary>
        /// Sorts the component list.
        /// </summary>
        void SortComponents()
        {
            components.Sort();
        }

        /// <summary>
        /// Synchronizes the component addition/removal lists.
        /// </summary>
        void SynchronizeLists()
        {
            foreach (Component component in componentsToAdd)
            {
                components.Add(component);
                component.OrderChanged += ShouldSort;
            }
            componentsToAdd.Clear();

            foreach (Component component in componentsToRemove.ToArray())
            {
                component.Dispose();
                component.OrderChanged -= ShouldSort;
                components.Remove(component);
            }
            componentsToRemove.Clear();

            shouldSort = true;
        }

        bool shouldSort;
        void ShouldSort()
        {
            shouldSort = true;
        }

        /// <summary>
        /// Does the game logic update of a single loop.
        /// </summary>
        internal void UpdateInternal()
        {
            stopwatch.Stop();
            TimeSpan elapsed = stopwatch.Elapsed;
            stopwatch.Reset();
            stopwatch.Start();

            if (componentsToAdd.Count > 0 || componentsToRemove.Count > 0)
                SynchronizeLists();

            foreach (Component component in components)
            {
                if (component.Enabled)
                    component.Update(elapsed);

                if (Disposed)
                    break;
            }

            if (shouldSort)
            {
                SortComponents();
                shouldSort = false;
            }

            Update();
        }

        /// <summary>
        /// Performs additional global update logic.
        /// The base implementation is empty.
        /// </summary>
        protected virtual void Update()
        {
        }

        /// <summary>
        /// Re-calls the <see cref="Component.Draw()"/> method of all loaded components.
        /// </summary>
        public void ReDraw()
        {
            foreach (Component component in components)
                if (component.Enabled)
                    component.Draw();
        }

        /// <summary>
        /// Does the drawing logic of a single loop.
        /// </summary>
        internal void Draw()
        {
            if (Disposed)
                return;

            foreach (Component component in components)
                if (component.Enabled)
                    component.PreDraw();

            Engine.Clear();

            foreach (Component component in components)
                if (component.Enabled)
                    component.Draw();
            foreach (Component component in components)
                if (component.Enabled)
                    component.PostDraw();

            Engine.RenderToScreen();
        }

        /// <summary>
        /// Provides access to the current engine settings.
        /// </summary>
        public EngineSettings Settings
        {
            get
            {
                return settings == null ? new EngineSettings(settings) : settings;
            }
        }

        /// <summary>
        /// Provides access to the only <see cref="TVEngine"/> reference.
        /// </summary>
        public TVEngine Engine { get; private set; }

        /// <summary>A reference to the <see cref="TVAtmosphere"/> singleton.</summary>
        public TVAtmosphere Atmosphere { get; private set; }
        /// <summary>A reference to the <see cref="TVCameraFactory"/> singleton.</summary>
        public TVCameraFactory CameraFactory { get; private set; }
        /// <summary>A reference to the <see cref="TVGlobals"/> singleton.</summary>
        public TVGlobals Globals { get; private set; }
        /// <summary>A reference to the <see cref="TVInputEngine"/> singleton.</summary>
        public TVInputEngine InputEngine { get; private set; }
        /// <summary>A reference to the <see cref="TVInternalObjects"/> singleton.</summary>
        public TVInternalObjects InternalObjects { get; private set; }
        /// <summary>A reference to the <see cref="TVLightEngine"/> singleton.</summary>
        public TVLightEngine LightEngine { get; private set; }
        /// <summary>A reference to the <see cref="TVMathLibrary"/> singleton.</summary>
        public TVMathLibrary MathLibrary { get; private set; }
        /// <summary>A reference to the <see cref="TVPhysics"/> singleton.</summary>
        public TVPhysics Physics { get; private set; }
        /// <summary>A reference to the <see cref="TVScene"/> singleton.</summary>
        public TVScene Scene { get; private set; }
        /// <summary>A reference to the <see cref="TVScreen2DImmediate"/> singleton.</summary>
        public TVScreen2DImmediate Screen2DImmediate { get; private set; }
        /// <summary>A reference to the <see cref="TVScreen2DText"/> singleton.</summary>
        public TVScreen2DText Screen2DText { get; private set; }
        /// <summary>A reference to the <see cref="TVTextureFactory"/> singleton.</summary>
        public TVTextureFactory TextureFactory { get; private set; }
        /// <summary>A reference to the <see cref="TVMaterialFactory"/> singleton.</summary>
        public TVMaterialFactory MaterialFactory { get; private set; }
        /// <summary>A reference to the <see cref="TVGameControllers"/> singleton.</summary>
        public TVGameControllers GameControllers { get; private set; }
        /// <summary>A reference to the <see cref="TVDeviceInfo"/> singleton.</summary>
        public TVDeviceInfo DeviceInfo { get; private set; }
        /// <summary>A reference to the <see cref="TVGraphicEffect"/> singleton.</summary>
        public TVGraphicEffect GraphicEffect { get; private set; }

        /// <summary>
        /// Does the actual dispose operation, as per the Dispose .NET pattern.
        /// </summary>
        protected virtual void DisposeInternal()
        {
            Engine.ReleaseAll();
            Engine = null;
        }

        ///<summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (Disposed) return;

            componentsToRemove.AddRange(components);
            SynchronizeLists();

            DisposeInternal();
            GC.SuppressFinalize(this);
            Disposed = true;
        }

        /// <summary>
        /// Whether this component is disposed.
        /// </summary>
        public bool Disposed { get; private set; }

        /// <summary>
        /// Gets or sets the root content directory
        /// </summary>
        public string ContentRoot
        {
            get { return contentRoot; }
            set
            {
                contentRoot = value;
                Engine.AddSearchDirectory(contentRoot);
                foreach (string directory in Directory.GetDirectories(contentRoot))
                {
                    Engine.AddSearchDirectory(directory);
                }
            }
        }

        /// <summary>
        /// Gets or sets the windowed form's title
        /// </summary>
        public string WindowTitle
        {
            get { return Settings.RenderForm.Text; }
            set { Settings.RenderForm.Text = value; }
        }
    }
}
