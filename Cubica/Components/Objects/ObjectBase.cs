using System.Collections.Generic;
using ComponentFramework.Core;
using MTV3D65;
using Cubica.Managers;

namespace Cubica.Components.Objects
{
    partial class ObjectBase : Component
    {
        // Common variables.
        public string Name { get; set; }
        public string FileName { get; set; }
        public bool Visible { get; set; }
        public TV_3DVECTOR Position { get; set; }
        public TV_3DVECTOR Rotation { get; set; }
        public TV_3DVECTOR Scale { get; set; }
        public bool ScriptEnabled { get; set; }
        public string Script { get; set; }
        public List<Parameter> Parameters { get; set; }

        public ObjectBase(ICore core) : base(core)
        {
            Parameters = new List<Parameter>();
        }

        [ServiceDependency]
        public new IScriptManagerService ScriptManager { get; set; }
    }

    public class Parameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
