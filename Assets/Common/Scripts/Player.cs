using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace APlusOrFail
{
    public class Player
    {
        public enum Action
        {
            Left,
            Right,
            Up,
            Down,
            Action1,
            Action2
        }


        private static int playerAutoId = 1;
        private static readonly List<Player> playerList = new List<Player>();
        public static readonly ReadOnlyCollection<Player> players = new ReadOnlyCollection<Player>(playerList);


        public int id { get; set; }

        private string _name;
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                onNameChanged?.Invoke(this, value);
            }
        }
        public event EventHandler<Player, string> onNameChanged;

        private Color _color = Color.white;
        public Color color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                onColorChanged?.Invoke(this, value);
            }
        }
        public event EventHandler<Player, Color> onColorChanged;

        private GameObject _characterSprite;
        public GameObject characterSprite
        {
            get
            {
                return _characterSprite;
            }
            set
            {
                _characterSprite = value;
                onCharacterSpriteChanged?.Invoke(this, value);
            }
        }
        public event EventHandler<Player, GameObject> onCharacterSpriteChanged;
        
        private readonly Dictionary<Action, KeyCode> actionMap = new Dictionary<Action, KeyCode>();
        


        public Player()
        {
            id = playerAutoId++;
            name = $"Player {id}";
            playerList.Add(this);
        }

        public bool HasKeyForAction(Action action)
        {
            return actionMap.ContainsKey(action);
        }

        public KeyCode? GetKeyForAction(Action action)
        {
            KeyCode key;
            return actionMap.TryGetValue(action, out key) ? (KeyCode?)key : null;
        }

        public void MapActionToKey(Action action, KeyCode key)
        {
            PlayerInputRegistry.RegisterKey(key, this);
            actionMap[action] = key;
        }

        public void UnmapActionFromKey(Action action)
        {
            KeyCode key;
            if (actionMap.TryGetValue(action, out key))
            {
                PlayerInputRegistry.UnregisterKey(key, this);
                actionMap.Remove(action);
            }
            
        }

        public void UnmapAllActionFromKey()
        {
            foreach (KeyCode key in actionMap.Values)
            {
                PlayerInputRegistry.UnregisterKey(key, this);
            }
            actionMap.Clear();
        }

        public void Delete()
        {
            playerList.Remove(this);
            UnmapAllActionFromKey();
            onDelete?.Invoke(this);
        }
        public event EventHandler<Player> onDelete;
    }
}
