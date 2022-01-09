using VRC.Core;

namespace MintMod.Reflections {
    public static class WorldReflect {
        public static bool IsInWorld() => GetWorld() != null || GetWorldInstance() != null;

        public static ApiWorld GetWorld() => RoomManager.field_Internal_Static_ApiWorld_0;

        public static ApiWorldInstance GetWorldInstance() => RoomManager.field_Internal_Static_ApiWorldInstance_0;

        public enum SDKType {
            NONE,
            SDK2,
            SDK3
        }
    }
}