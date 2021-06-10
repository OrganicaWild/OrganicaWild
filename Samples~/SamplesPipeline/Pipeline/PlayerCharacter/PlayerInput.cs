// GENERATED AUTOMATICALLY FROM 'Packages/ch.i4ds.organicawild/Samples/SamplesPipeline/Pipeline/PlayerCharacter/PlayerInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerInput : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInput"",
    ""maps"": [
        {
            ""name"": ""CharacterControls"",
            ""id"": ""c1af3b6c-cc81-4ec9-a45e-d0bbd3900d64"",
            ""actions"": [
                {
                    ""name"": ""Forward"",
                    ""type"": ""Button"",
                    ""id"": ""25a012ae-db7e-4828-a363-d1126e5afa8a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Left"",
                    ""type"": ""Button"",
                    ""id"": ""210394e2-00be-424e-a980-2e8301ccb021"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Backward"",
                    ""type"": ""Button"",
                    ""id"": ""b8d44fc6-5957-4c7e-a614-129612a98b20"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Right"",
                    ""type"": ""Button"",
                    ""id"": ""971ba25c-c36b-4fa7-ad1e-56a438ed8d38"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Run"",
                    ""type"": ""Button"",
                    ""id"": ""17b6ab44-2185-4530-9552-4a74ca3b328b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""401e3d5b-88d8-4d46-92f2-07e244ba7254"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Forward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ff7478fa-e8d1-4be2-96c0-5d57fae37416"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""88b2b7a6-a776-40f7-bdb5-52bf58b7cb79"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""77cde793-9eb8-4714-b29d-ac64caceb82c"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Backward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ea7b7245-14bb-42e1-8429-e3fb5ee50645"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8bfde31e-9064-4089-aea2-b2ab85810475"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""CameraControls"",
            ""id"": ""dd42a97e-c688-4479-83b4-99a2dadabdba"",
            ""actions"": [
                {
                    ""name"": ""Mouse"",
                    ""type"": ""PassThrough"",
                    ""id"": ""05843a08-497d-49ee-adf0-1b2ceca83e79"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""ef69f369-e84d-441b-b854-4e6752c67618"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Mouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // CharacterControls
        m_CharacterControls = asset.FindActionMap("CharacterControls", throwIfNotFound: true);
        m_CharacterControls_Forward = m_CharacterControls.FindAction("Forward", throwIfNotFound: true);
        m_CharacterControls_Left = m_CharacterControls.FindAction("Left", throwIfNotFound: true);
        m_CharacterControls_Backward = m_CharacterControls.FindAction("Backward", throwIfNotFound: true);
        m_CharacterControls_Right = m_CharacterControls.FindAction("Right", throwIfNotFound: true);
        m_CharacterControls_Run = m_CharacterControls.FindAction("Run", throwIfNotFound: true);
        // CameraControls
        m_CameraControls = asset.FindActionMap("CameraControls", throwIfNotFound: true);
        m_CameraControls_Mouse = m_CameraControls.FindAction("Mouse", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // CharacterControls
    private readonly InputActionMap m_CharacterControls;
    private ICharacterControlsActions m_CharacterControlsActionsCallbackInterface;
    private readonly InputAction m_CharacterControls_Forward;
    private readonly InputAction m_CharacterControls_Left;
    private readonly InputAction m_CharacterControls_Backward;
    private readonly InputAction m_CharacterControls_Right;
    private readonly InputAction m_CharacterControls_Run;
    public struct CharacterControlsActions
    {
        private @PlayerInput m_Wrapper;
        public CharacterControlsActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Forward => m_Wrapper.m_CharacterControls_Forward;
        public InputAction @Left => m_Wrapper.m_CharacterControls_Left;
        public InputAction @Backward => m_Wrapper.m_CharacterControls_Backward;
        public InputAction @Right => m_Wrapper.m_CharacterControls_Right;
        public InputAction @Run => m_Wrapper.m_CharacterControls_Run;
        public InputActionMap Get() { return m_Wrapper.m_CharacterControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CharacterControlsActions set) { return set.Get(); }
        public void SetCallbacks(ICharacterControlsActions instance)
        {
            if (m_Wrapper.m_CharacterControlsActionsCallbackInterface != null)
            {
                @Forward.started -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnForward;
                @Forward.performed -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnForward;
                @Forward.canceled -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnForward;
                @Left.started -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnLeft;
                @Left.performed -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnLeft;
                @Left.canceled -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnLeft;
                @Backward.started -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnBackward;
                @Backward.performed -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnBackward;
                @Backward.canceled -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnBackward;
                @Right.started -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnRight;
                @Right.performed -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnRight;
                @Right.canceled -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnRight;
                @Run.started -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnRun;
                @Run.performed -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnRun;
                @Run.canceled -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnRun;
            }
            m_Wrapper.m_CharacterControlsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Forward.started += instance.OnForward;
                @Forward.performed += instance.OnForward;
                @Forward.canceled += instance.OnForward;
                @Left.started += instance.OnLeft;
                @Left.performed += instance.OnLeft;
                @Left.canceled += instance.OnLeft;
                @Backward.started += instance.OnBackward;
                @Backward.performed += instance.OnBackward;
                @Backward.canceled += instance.OnBackward;
                @Right.started += instance.OnRight;
                @Right.performed += instance.OnRight;
                @Right.canceled += instance.OnRight;
                @Run.started += instance.OnRun;
                @Run.performed += instance.OnRun;
                @Run.canceled += instance.OnRun;
            }
        }
    }
    public CharacterControlsActions @CharacterControls => new CharacterControlsActions(this);

    // CameraControls
    private readonly InputActionMap m_CameraControls;
    private ICameraControlsActions m_CameraControlsActionsCallbackInterface;
    private readonly InputAction m_CameraControls_Mouse;
    public struct CameraControlsActions
    {
        private @PlayerInput m_Wrapper;
        public CameraControlsActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Mouse => m_Wrapper.m_CameraControls_Mouse;
        public InputActionMap Get() { return m_Wrapper.m_CameraControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CameraControlsActions set) { return set.Get(); }
        public void SetCallbacks(ICameraControlsActions instance)
        {
            if (m_Wrapper.m_CameraControlsActionsCallbackInterface != null)
            {
                @Mouse.started -= m_Wrapper.m_CameraControlsActionsCallbackInterface.OnMouse;
                @Mouse.performed -= m_Wrapper.m_CameraControlsActionsCallbackInterface.OnMouse;
                @Mouse.canceled -= m_Wrapper.m_CameraControlsActionsCallbackInterface.OnMouse;
            }
            m_Wrapper.m_CameraControlsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Mouse.started += instance.OnMouse;
                @Mouse.performed += instance.OnMouse;
                @Mouse.canceled += instance.OnMouse;
            }
        }
    }
    public CameraControlsActions @CameraControls => new CameraControlsActions(this);
    public interface ICharacterControlsActions
    {
        void OnForward(InputAction.CallbackContext context);
        void OnLeft(InputAction.CallbackContext context);
        void OnBackward(InputAction.CallbackContext context);
        void OnRight(InputAction.CallbackContext context);
        void OnRun(InputAction.CallbackContext context);
    }
    public interface ICameraControlsActions
    {
        void OnMouse(InputAction.CallbackContext context);
    }
}
