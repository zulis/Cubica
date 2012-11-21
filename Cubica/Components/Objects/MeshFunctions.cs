using System.Collections.Generic;
using System.Linq;
using LuaInterface;
using MTV3D65;
using Cubica.Managers;

namespace Cubica.Components.Objects
{
    partial class Mesh
    {
        [RegisterFunction]
        public void RotateXAroundPoint(TV_3DVECTOR point, float angle)
        {
            TVMesh node = Scene.CreateMeshBuilder();
            node.SetPosition(point.x, point.y, point.z);
            mesh.AttachTo(CONST_TV_NODETYPE.TV_NODETYPE_MESH, node.GetIndex(), -1);
            TV_3DVECTOR nodePos = node.GetPosition();
            TV_3DMATRIX mat = new TV_3DMATRIX();
            TV_3DMATRIX modyMat = node.GetMatrix();
            MathLibrary.TVMatrixRotationX(ref mat, angle);
            node.SetRotationMatrix(mat);
            mesh.AttachTo(CONST_TV_NODETYPE.TV_NODETYPE_NONE, -1, -1);
            node.Destroy();
            mat = mesh.GetMatrix();
            Physics.SetBodyMatrix(PhysicsId, mat);
        }

        [RegisterFunction]
        public void RotateYAroundPoint(TV_3DVECTOR point, float angle)
        {
            TVMesh node = Scene.CreateMeshBuilder();
            node.SetPosition(point.x, point.y, point.z);
            mesh.AttachTo(CONST_TV_NODETYPE.TV_NODETYPE_MESH, node.GetIndex(), -1);
            TV_3DVECTOR nodePos = node.GetPosition();
            TV_3DMATRIX mat = new TV_3DMATRIX();
            TV_3DMATRIX modyMat = node.GetMatrix();
            MathLibrary.TVMatrixRotationY(ref mat, angle);
            node.SetRotationMatrix(mat);
            mesh.AttachTo(CONST_TV_NODETYPE.TV_NODETYPE_NONE, -1, -1);
            node.Destroy();
            mat = mesh.GetMatrix();
            Physics.SetBodyMatrix(PhysicsId, mat);
        }

        [RegisterFunction]
        public void RotateZAroundPoint(TV_3DVECTOR point, float angle)
        {
            TVMesh node = Scene.CreateMeshBuilder();
            node.SetPosition(point.x, point.y, point.z);
            mesh.AttachTo(CONST_TV_NODETYPE.TV_NODETYPE_MESH, node.GetIndex(), -1);
            TV_3DVECTOR nodePos = node.GetPosition();
            TV_3DMATRIX mat = new TV_3DMATRIX();
            TV_3DMATRIX modyMat = node.GetMatrix();
            MathLibrary.TVMatrixRotationZ(ref mat, angle);
            node.SetRotationMatrix(mat);
            mesh.AttachTo(CONST_TV_NODETYPE.TV_NODETYPE_NONE, -1, -1);
            node.Destroy();
            mat = mesh.GetMatrix();
            Physics.SetBodyMatrix(PhysicsId, mat);
        }

        [RegisterFunction]
        public TVMesh GetMesh()
        {
            return mesh;
        }

        [RegisterFunction]
        public TV_3DVECTOR GetMinBBox()
        {
            TV_3DVECTOR minBB = new TV_3DVECTOR();
            TV_3DVECTOR maxBB = new TV_3DVECTOR();
            mesh.GetBoundingBox(ref minBB, ref maxBB);
            return minBB;
        }

        [RegisterFunction]
        public TV_3DVECTOR GetMaxBBox()
        {
            TV_3DVECTOR minBB = new TV_3DVECTOR();
            TV_3DVECTOR maxBB = new TV_3DVECTOR();
            mesh.GetBoundingBox(ref minBB, ref maxBB);
            return maxBB;
        }

        [RegisterFunction]
        public void MoveX(float value)
        {
            TV_3DVECTOR position = Physics.GetBodyPosition(PhysicsId);
            position.x += value;
            Physics.SetBodyPosition(PhysicsId, position.x, position.y, position.z);
        }

        [RegisterFunction]
        public void MoveY(float value)
        {
            TV_3DVECTOR position = Physics.GetBodyPosition(PhysicsId);
            position.y += value;
            Physics.SetBodyPosition(PhysicsId, position.x, position.y, position.z);
        }

        [RegisterFunction]
        public void MoveZ(float value)
        {
            TV_3DVECTOR position = Physics.GetBodyPosition(PhysicsId);
            position.z += value;
            Physics.SetBodyPosition(PhysicsId, position.x, position.y, position.z);
        }

        [RegisterFunction]
        public void Move(TV_3DVECTOR target)
        {
            Move(target.x, target.y, target.z);
        }

        [RegisterFunction]
        public void Move(float x, float y, float z)
        {
            MoveX(x);
            MoveY(y);
            MoveZ(z);
        }

        [RegisterFunction]
        public void RotateX(float angle)
        {
            TV_3DVECTOR position = Physics.GetBodyPosition(PhysicsId);
            TV_3DMATRIX mat = new TV_3DMATRIX();
            TV_3DMATRIX modyMat = Physics.GetBodyMatrix(PhysicsId);
            MathLibrary.TVMatrixRotationX(ref mat, angle);
            MathLibrary.TVMatrixMultiply(ref mat, modyMat, mat);
            Physics.SetBodyMatrix(PhysicsId, mat);
            Physics.SetBodyPosition(PhysicsId, position.x, position.y, position.z);
        }

        [RegisterFunction]
        public void RotateY(float angle)
        {
            TV_3DVECTOR position = Physics.GetBodyPosition(PhysicsId);
            TV_3DMATRIX mat = new TV_3DMATRIX();
            TV_3DMATRIX modyMat = Physics.GetBodyMatrix(PhysicsId);
            MathLibrary.TVMatrixRotationY(ref mat, angle);
            MathLibrary.TVMatrixMultiply(ref mat, modyMat, mat);
            Physics.SetBodyMatrix(PhysicsId, mat);
            Physics.SetBodyPosition(PhysicsId, position.x, position.y, position.z);
        }

        [RegisterFunction]
        public void RotateZ(float angle)
        {
            TV_3DVECTOR position = Physics.GetBodyPosition(PhysicsId);
            TV_3DMATRIX mat = new TV_3DMATRIX();
            TV_3DMATRIX modyMat = Physics.GetBodyMatrix(PhysicsId);
            MathLibrary.TVMatrixRotationZ(ref mat, angle);
            MathLibrary.TVMatrixMultiply(ref mat, modyMat, mat);
            Physics.SetBodyMatrix(PhysicsId, mat);
            Physics.SetBodyPosition(PhysicsId, position.x, position.y, position.z);

        }

        [RegisterFunction]
        public void Rotate(float x, float y, float z)
        {
            RotateX(x);
            RotateY(y);
            RotateZ(z);
        }

        [RegisterFunction]
        public TV_3DVECTOR GetPosition()
        {
            return Physics.GetBodyPosition(PhysicsId);
        }

        [RegisterFunction]
        public void SetPosition(TV_3DVECTOR target)
        {
            SetPosition(target.x, target.y, target.z);
        }

        [RegisterFunction]
        public void SetPosition(float x, float y, float z)
        {
            Physics.SetBodyPosition(PhysicsId, x, y, z);
        }

        [RegisterFunction]
        public TV_3DVECTOR GetRotation()
        {
            return Physics.GetBodyRotation(PhysicsId);
        }

        [RegisterFunction]
        public void SetRotation(TV_3DVECTOR target)
        {
            SetRotation(target.x, target.y, target.z);
        }

        [RegisterFunction]
        public void SetRotation(float x, float y, float z)
        {
            Rotation = new TV_3DVECTOR(x, y, z);
            Physics.SetBodyRotation(PhysicsId, x, y, z);
        }

        [RegisterFunction]
        public bool InteractsWith(Trigger target)
        {
            TV_3DVECTOR meshBBoxMin = new TV_3DVECTOR();
            TV_3DVECTOR meshBBoxMax = new TV_3DVECTOR();
            mesh.GetBoundingBox(ref meshBBoxMin, ref meshBBoxMax);
            return (meshBBoxMin.x < target.BoundingBoxMax.x) &&
                (meshBBoxMax.x > target.BoundingBoxMin.x) &&
                (meshBBoxMin.y < target.BoundingBoxMax.y) &&
                (meshBBoxMax.y > target.BoundingBoxMin.y) &&
                (meshBBoxMin.z < target.BoundingBoxMax.z) &&
                (meshBBoxMax.z > target.BoundingBoxMin.z);
        }

        [RegisterFunction]
        public bool InteractsWith(Mesh target)
        {
            TV_3DVECTOR meshBBoxMin = new TV_3DVECTOR();
            TV_3DVECTOR meshBBoxMax = new TV_3DVECTOR();
            TV_3DVECTOR targetBBoxMin = new TV_3DVECTOR();
            TV_3DVECTOR targetBBoxMax = new TV_3DVECTOR();
            mesh.GetBoundingBox(ref meshBBoxMin, ref meshBBoxMax);
            target.GetMesh().GetBoundingBox(ref targetBBoxMin, ref targetBBoxMax);
            return (meshBBoxMin.x < targetBBoxMax.x) &&
                (meshBBoxMax.x > targetBBoxMin.x) &&
                (meshBBoxMin.y < targetBBoxMax.y) &&
                (meshBBoxMax.y > targetBBoxMin.y) &&
                (meshBBoxMin.z < targetBBoxMax.z) &&
                (meshBBoxMax.z > targetBBoxMin.z);
        }

        [RegisterFunction]
        public LuaTable GetInteractingObjects(Mesh target)
        {
            var result = new List<object>();
            var objList = SceneManager.GetGameObjects();

            foreach (var obj in objList)
            {
                if (obj.GetType().Equals(typeof(Mesh)))
                {
                    if (!((Mesh)obj).Equals(target))
                    {
                        if (target.InteractsWith((Mesh)obj))
                        {
                            result.Add(obj);
                        }
                    }
                }
                else if (obj.GetType().Equals(typeof(Trigger)))
                {
                    if (target.InteractsWith((Trigger)obj))
                    {
                        result.Add(obj);
                    }
                }
            }

            return ScriptManager.ListToTable(result);
        }

        [RegisterFunction]
        public LuaTable GetInteractingTriggers(Mesh target)
        {
            var result = new List<object>();
            var triggers = from t in SceneManager.GetGameObjects()
                           where t.GetType().Equals(typeof(Trigger))
                           select t;

            foreach (Trigger trigger in triggers)
            {
                if (target.InteractsWith(trigger))
                {
                    result.Add(trigger);
                }
            }

            return ScriptManager.ListToTable(result);
        }

        //[RegisterFunction]
        //public ObjectBase GetInteractingObject(Mesh target)
        //{
        //    var objList = SceneManager.GetGameObjects();

        //    foreach (var obj in objList)
        //    {
        //        if (obj.GetType().Equals(typeof(Mesh)))
        //        {
        //            if (!((Mesh)obj).Equals(target))
        //            {
        //                if (target.InteractsWith((Mesh)obj))
        //                {
        //                    return (Mesh)obj;
        //                }
        //            }
        //        }
        //        else if (obj.GetType().Equals(typeof(Trigger)))
        //        {
        //            if (target.InteractsWith((Trigger)obj))
        //            {
        //                return (Trigger)obj;
        //            }
        //        }
        //    }

        //    // No interaction.
        //    return null;
        //}

        [RegisterFunction]
        public void Freeze()
        {
            Physics.FreezeBody(PhysicsId);
        }

        [RegisterFunction]
        public void Unfreeze()
        {
            Physics.UnfreezeBody(PhysicsId);
        }

        [RegisterFunction]
        public void SetMovable(bool value)
        {
            Physics.SetBodyMovable(PhysicsId, value);
        }

        [RegisterFunction]
        public void SetMass(float value)
        {
            Physics.SetBodyMass(PhysicsId, value);
        }

        [RegisterFunction]
        public void SetImpulse(TV_3DVECTOR value)
        {
            Physics.AddImpulse(PhysicsId, value);
        }
    }
}