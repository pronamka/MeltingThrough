using UnityEngine;
using UnityEngine.InputSystem;

public class InteractInputSetup : MonoBehaviour
{
    private void Awake()
    {
        
        SetupInteractAction();
    }

    private void SetupInteractAction()
    {
        var inputActions = InputSystem.actions;

        
        var playerMap = inputActions.FindActionMap("Player");
        if (playerMap == null)
        {
            return;
        }

        
        var interactAction = playerMap.FindAction("Interact");
        if (interactAction == null)
        {
            interactAction = playerMap.AddAction("Interact", type: InputActionType.Button);
            interactAction.AddBinding("<Keyboard>/e");
        }
        interactAction.Enable();
    }
}