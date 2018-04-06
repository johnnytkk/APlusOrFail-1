using System;
using System.Collections.Generic;
using UnityEngine;

namespace APlusOrFail
{
    public static class PlayerInputRegistry
    {
        private static readonly Dictionary<KeyCode, Player> registry = new Dictionary<KeyCode, Player>();

        public static bool HasRegistered(KeyCode key)
        {
            return registry.ContainsKey(key);
        }

        public static bool HasRegisteredByOther(KeyCode key, Player exceptPlayer)
        {
            Player associatedPlayer = GetAssociatedPlayer(key);
            return associatedPlayer != null && associatedPlayer != exceptPlayer;
        }

        public static Player GetAssociatedPlayer(KeyCode key)
        {
            Player player;
            return registry.TryGetValue(key, out player) ? player : null;
        }

        public static void RegisterKey(KeyCode key, Player player)
        {
            Player associatedPlayer = GetAssociatedPlayer(key);
            if (associatedPlayer == null)
            {
                registry[key] = player;
            }
            else if (associatedPlayer != player)
            {
                throw new ArgumentException($"Key \"{Enum.GetName(typeof(KeyCode), key)}\" has already been associated with Player {associatedPlayer.id}");
            }
        }

        public static void UnregisterKey(KeyCode key, Player player)
        {
            if (GetAssociatedPlayer(key) == player)
            {
                registry.Remove(key);
            }
        }
    }
}
