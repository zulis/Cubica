using System;
using MTV3D65;
using SlimDX.Direct3D9;

namespace ComponentFramework.Core
{
    /// <summary>
    /// A component with rendering support and render-order awareness.
    /// Provides the basic TV3D singleton services, a reference to the <see cref="ICore"/>
    /// and a callback for initialization and updating.
    /// </summary>
    public abstract class Component : IComparable<Component>
    {
        /// <summary>
        /// Protected parameterized constructor. Initializes the TV3D singletons.
        /// </summary>
        /// <param name="core">The core interface used to load this component.</param>
        protected Component(ICore core) 
        {
            Core = core;
            Device = Device.FromPointer(InternalObjects.GetDevice3D());
            Enabled = true;
        }

        /// <summary>
        /// Raised when the <see cref"Order"/> property is changed
        /// </summary>
        internal event Action OrderChanged;

        /// <summary>
        /// Whether this component is disposed.
        /// </summary>
        public bool Disposed { get; private set; }

        /// <summary>
        /// The render (and update) order of this component among other components.
        /// Its value should not change during the component's lifetime.
        /// The default is 0.
        /// </summary>
        public int Order;

        /// <summary>
        /// The DirectX graphics device reference.
        /// </summary>
        protected Device Device { get; private set; }

        /// <summary>
        /// Determines if this component will be updated and drawn.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>A reference to the <see cref="TVAtmosphere"/> singleton.</summary>
        protected TVAtmosphere Atmosphere { get { return Core.Atmosphere; } }
        /// <summary>A reference to the <see cref="TVCameraFactory"/> singleton.</summary>
        protected TVCameraFactory CameraFactory { get { return Core.CameraFactory; } }
        /// <summary>A reference to the <see cref="TVGlobals"/> singleton.</summary>
        protected TVGlobals Globals { get { return Core.Globals; } }
        /// <summary>A reference to the <see cref="TVInputEngine"/> singleton.</summary>
        protected TVInputEngine InputEngine { get { return Core.InputEngine; } }
        /// <summary>A reference to the <see cref="TVInternalObjects"/> singleton.</summary>
        protected TVInternalObjects InternalObjects { get { return Core.InternalObjects; } }
        /// <summary>A reference to the <see cref="TVLightEngine"/> singleton.</summary>
        protected TVLightEngine LightEngine { get { return Core.LightEngine; } }
        /// <summary>A reference to the <see cref="TVMathLibrary"/> singleton.</summary>
        protected TVMathLibrary MathLibrary { get { return Core.MathLibrary; } }
        /// <summary>A reference to the <see cref="TVPhysics"/> singleton.</summary>
        protected TVPhysics Physics { get { return Core.Physics; } }
        /// <summary>A reference to the <see cref="TVScene"/> singleton.</summary>
        protected TVScene Scene { get { return Core.Scene; } }
        /// <summary>A reference to the <see cref="TVScreen2DImmediate"/> singleton.</summary>
        protected TVScreen2DImmediate Screen2DImmediate { get { return Core.Screen2DImmediate; } }
        /// <summary>A reference to the <see cref="TVScreen2DText"/> singleton.</summary>
        protected TVScreen2DText Screen2DText { get { return Core.Screen2DText; } }
        /// <summary>A reference to the <see cref="TVTextureFactory"/> singleton.</summary>
        protected TVTextureFactory TextureFactory { get { return Core.TextureFactory; } }
        /// <summary>A reference to the <see cref="TVMaterialFactory"/> singleton.</summary>
        protected TVMaterialFactory MaterialFactory { get { return Core.MaterialFactory; } }
        /// <summary>A reference to the <see cref="TVDeviceInfo"/> singleton.</summary>
        protected TVDeviceInfo DeviceInfo { get { return Core.DeviceInfo; } }
        /// <summary>A reference to the <see cref="TVGraphicEffect"/> singleton.</summary>
        protected TVGraphicEffect GraphicEffect { get { return Core.GraphicEffect; } }

        /// <summary>
        /// A reference to the core interface used to loaded this component.
        /// </summary>
        protected ICore Core { get; private set; }

        /// <summary>
        /// Call this method when you change the <see cref"Order"/> property after the 
        /// <see cref"Component.Initialize()"/> method has been called.
        /// </summary>
        protected void OnOrderChanged()
        {
            OrderChanged();
        }

        /// <summary>
        /// Performs pre-drawing operations. 
        /// This method is called before <see cref="TVEngine.Clear()"/>.
        /// </summary>
        public virtual void PreDraw() { }
        /// <summary>
        /// Performs drawing operations. 
        /// This method is called between <see cref="TVEngine.Clear()"/> and <see cref="TVEngine.RenderToScreen()"/>.
        /// </summary>
        public virtual void Draw() { }
        /// <summary>
        /// Performs post-drawing operations. 
        /// This method is called after <see cref="TVEngine.RenderToScreen()"/>.
        /// </summary>
        public virtual void PostDraw() { }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. 
        /// The return value has the following meanings: 
        /// Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.
        /// Zero This object is equal to <paramref name="other" />. 
        /// Greater than zero This object is greater than <paramref name="other" />. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(Component other)
        {
            //return Order - other.Order;
            return Order.CompareTo(other.Order);
        }

        /// <summary>
        /// Initializes this component. The base implementation does nothing and does not
        /// need to be called by overriding implementations.
        /// </summary>
        public virtual void Initialize() 
        {
        }

        /// <summary>
        /// Performs post-initialize operations.
        /// This method is called after Initialize method.
        /// If LoadComponent is used, then PostInitialize is not called. In that case you should call it manually.
        /// </summary>
        public virtual void PostInitialize()
        {
        }

        /// <summary>
        /// Performs update of game logic and non-rendering-related operations.
        /// </summary>
        /// <param name="elapsedTime">The time elapsed since the last update.</param>
        public virtual void Update(TimeSpan elapsedTime) { }

        /// <summary>
        /// Does the actual dispose operation, as per the Dispose .NET pattern.
        /// </summary>
        protected virtual void DisposeInternal()
        {
        }

        ///<summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            Device.Dispose();

            DisposeInternal();
            GC.SuppressFinalize(this);
            Disposed = true;
        }
    }
}