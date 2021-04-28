using System;
using Mirror;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
    public Character SelectedCharacter => characters[selectedCharacter];

    public event Action<Character> OnCharacterChanged;
    
    [SerializeField] private NetworkAnimator networkAnimator;

    [SerializeField] private Character[] characters;

    private int selectedCharacter = 0;
    
    [System.Serializable]
    public class Character
    {
        public Transform movementRotator;
        public Animator animator;
        public GameObject character;
        public bool useMovementRotations;
    }

    private void Start()
    {
        SelectCharacter(0);
    }

#if UNITY_EDITOR

    private void Update()
    {
        if (!isLocalPlayer) return;
        
        if (Input.GetKeyDown(KeyCode.Keypad0)) SelectCharacter(0);
        else if (Input.GetKeyDown(KeyCode.Keypad1)) SelectCharacter(1);
        else if (Input.GetKeyDown(KeyCode.Keypad2)) SelectCharacter(2);
    }

#endif

    /// <summary>
    /// This should be probably called by a button, 1 for each character.
    /// </summary>
    /// <param name="character"></param>
    public void SelectCharacter(int character)
    {
        selectedCharacter = character;

        UpdateCharacter();
    }

    private void UpdateCharacter()
    {
        for (var i = 0; i < characters.Length; i++)
            characters[i].character.SetActive(i == selectedCharacter);

        networkAnimator.animator = SelectedCharacter.animator;
        
        OnCharacterChanged?.Invoke(SelectedCharacter);
    }
}
