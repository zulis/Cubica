using System;
using System.Collections.Generic;
using System.Reflection;
using ComponentFramework.Components;
using ComponentFramework.Core;
using LuaInterface;
using System.Globalization;

namespace Cubica.Managers
{
    [AutoLoad]
    partial class ScriptManager : Component, IScriptManagerService
    {
        Lua lua;

        public ScriptManager(ICore core) : base(core) { Order = int.MinValue; }

        public override void Initialize()
        {
            lua = new Lua();
            RegisterCustomFunctions(this);
        }

        public override void Dispose()
        {
            lua.Close();
        }

        public void DoFile(string target)
        {
            try
            {
                lua.DoFile(target);
            }
            catch (Exception e)
            {
                DebuggingBag.Put("ScriptManager.DoFile error:", e.Message);
            }
        }

        public void DoString(string value)
        {
            try
            {
                lua.DoString(value);
            }
            catch (Exception e)
            {
                DebuggingBag.Put("DoString error:", e.Message);
            }
        }

        public LuaFunction FindFunc(string name)
        {
            try
            {
                LuaFunction retfunc = lua.GetFunction(name);
                return retfunc;
            }
            catch (Exception e)
            {
                DebuggingBag.Put("FindFunc error:", e.Message);
                return null;
            }
        }

        public void CallFunction(string name)
        {
            if (FindFunc(name) == null)
            {
                return;
            }
            try
            {
                LuaFunction retfunc = lua.GetFunction(name);
                retfunc.Call();

            }
            catch (Exception e)
            {
                DebuggingBag.Put(string.Format(CultureInfo.InvariantCulture, "{0} error:", name), e.Message);
            }
        }

        public void RegisterFunction(string path, object target, MethodBase function)
        {
            try
            {
                lua.RegisterFunction(path, target, function);
            }
            catch (Exception e)
            {
                DebuggingBag.Put("RegisterFunction error:", e.Message);
            }
        }

        public void SetGlobal(string name, object target)
        {
            lua[name] = target;
        }

        public void RegisterCustomFunctions(object target)
        {
            foreach (var info in target.GetType().GetMethods())
            {
                foreach (var attr in Attribute.GetCustomAttributes(info))
                {
                    if (attr.GetType() == typeof(RegisterFunction))
                    {
                        var funcName = ((RegisterFunction)attr).getFuncName();

                        if (funcName == null)
                        {
                            funcName = info.Name;
                        }

                        RegisterFunction(funcName, target, info);
                    }
                }
            }
        }

        public void RegisterFunctions(object target)
        {
            foreach (var info in target.GetType().GetMethods())
            {
                RegisterFunction(info.Name, target, info);
            }
        }

        public LuaTable CreateTable()
        {
            var tmpName = Guid.NewGuid().ToString();
            lua.NewTable(tmpName);
            return lua.GetTable(tmpName);
        }

        public LuaTable ListToTable(List<object> target)
        {
            var tmpTable = CreateTable();
            var idx = 0;
            foreach (var obj in target)
            {
                tmpTable[idx++] = obj;
            }
            return tmpTable;
        }

        [ServiceDependency]
        public new IDebuggingBagService DebuggingBag { private get; set; }
    }

    public interface IScriptManagerService : IService
    {
        void DoFile(string target);
        void DoString(string value);
        void CallFunction(string name);
        void SetGlobal(string name, object target);
        void RegisterCustomFunctions(object target);
        void RegisterFunctions(object target);
        LuaTable CreateTable();
        LuaTable ListToTable(List<object> target);
    }

    public class RegisterFunction : Attribute
    {
        private string functionName;

        public RegisterFunction() { }

        public RegisterFunction(string functionName)
        {
            this.functionName = functionName;
        }

        public string getFuncName()
        {
            return functionName;
        }
    }
}
