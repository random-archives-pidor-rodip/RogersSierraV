using FusionLibrary;
using GTA;
using GTA.Math;
using GTA.Native;

namespace RogersSierra.Extensions
{
    /// <summary>
    /// Other random extensions.
    /// </summary>
    public static class OtherExtensions
    {
        //
        // Summary:
        //     Attaches this GTA.Entity to a different GTA.Entity
        //
        // Parameters:
        //   entity:
        //     The GTA.Entity to attach this GTA.Entity to.
        //
        //   position:
        //     The position relative to the entity to attach this GTA.Entity to.
        //
        //   rotation:
        //     The rotation to apply to this GTA.Entity relative to the entity
        public static void AttachToWithCollision(this Entity entityBase, Entity entity, Vector3 position = default(Vector3), Vector3 rotation = default(Vector3))
        {
            Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, entityBase.Handle, entity.Handle, -1, position.X, position.Y, position.Z, rotation.X, rotation.Y, rotation.Z, 0, 0, 1, 0, 2, 1);
        }
    }
}
