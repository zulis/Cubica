using System;
using System.Windows.Forms;
using SlimDX.Windows;

namespace ComponentFramework.Core
{
    /// <summary>
    /// Handles the message-based looping mechanisms.
    /// See : http://blogs.msdn.com/tmiller/archive/2005/05/05/415008.aspx
    /// and http://pinvoke.net/default.aspx/user32/PeekMessage.html
    /// </summary>
    class Looping
    {
        /// <summary>
        /// A direct reference to the core.
        /// </summary>
        readonly Core core;

        /// <summary>
        /// Whether looping is currently paused.
        /// </summary>
        bool paused;

        const float FIXEDHERTZ = 1f / 60f;
        float elapsed = 0f;

        /// <summary>
        /// Parameterized constructor. Hooks itself onto the <see cref="Application.Idle"/> event.
        /// </summary>
        /// <param name="core"></param>
        public Looping(Core core)
        {
            this.core = core;
            Application.Idle += Mainloop;
        }

        /// <summary>
        /// The main loop handler.
        /// </summary>
        /// <param name="sender">Ignored</param>
        /// <param name="e">Ignored</param>
        void Mainloop(object sender, EventArgs e)
        {
            while (!paused && MessagePump.IsApplicationIdle)
            {
#if DEBUG
                try
                {
#endif
                    elapsed += core.Engine.AccurateTimeElapsed() / 1000f;

                    if (elapsed > core.Settings.UpdateFrequency)
                    {
                        core.Physics.Simulate(core.Engine.AccurateTimeElapsed() / 750f /*
                            * (core.Settings.UpdateFrequency * 1 / core.Settings.UpdateFrequency)*/);
                        core.UpdateInternal();
                        elapsed -= core.Settings.UpdateFrequency;
                    }

                    core.Draw();
#if DEBUG
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + ex.StackTrace, "Error at loop");
                }
#endif
            }
        }

        /// <summary>
        /// Stops looping and exits the application.
        /// </summary>
        public void StopLoop()
        {
            Application.Idle -= Mainloop;
            Application.Exit();
        }

        /// <summary>
        /// Pauses looping until the <see cref="ResumeLoop"/> method is called.
        /// </summary>
        public void PauseLoop()
        {
            if (!paused)
            {
                Application.Idle -= Mainloop;
                paused = true;
            }
        }

        /// <summary>
        /// Resumes looping from a <see cref="PauseLoop"/> call.
        /// </summary>
        public void ResumeLoop()
        {
            if (paused)
            {
                Application.Idle += Mainloop;
                paused = false;
            }
        }
    }
}
