using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

[RequireComponent(typeof(InputActionManager))]
public class InputProvider : MonoBehaviour
{
    [SerializeField] InputActionReference _rightHandButton;
    [SerializeField] InputActionReference _rightHandStick;
    [SerializeField] InputActionReference _rightHandStickButton;
    [SerializeField] InputActionReference _leftHandButton;
    [SerializeField] InputActionReference _leftHandStick;
    [SerializeField] InputActionReference _leftHandStickButton;

    [Header("For Inspecting")]
    public bool IsLeftButtonPressed;
    public bool IsRightButtonPressed;
    public bool IsLeftStickPressed;
    public bool IsRightStickPressed;
    public Vector2 LeftStickValue;
    public Vector2 RightStickValue;

    public static InputProvider Instance { get; private set; }

    private void Initialize()
    {
        // 以下のコードはノリで書いてた
        // ちょっと考えると？
        // A. 一番最初に生成されたインスタンスだけを保持
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Awake()
    {
        Initialize();
    }

    private void Update()
    {
        IsLeftButtonPressed = _leftHandButton.action.IsPressed();
        IsRightButtonPressed = _rightHandButton.action.IsPressed();
        IsLeftStickPressed = _leftHandStickButton.action.IsPressed();
        IsRightStickPressed = _rightHandStickButton.action.IsPressed();

        // ??は左がnullの場合右が渡され、非nullだと左が渡される
        // https://learn.microsoft.com/ja-jp/dotnet/csharp/language-reference/operators/null-coalescing-operator
        LeftStickValue = _leftHandStick.action?.ReadValue<Vector2>() ?? Vector2.zero;
        RightStickValue = _rightHandStick.action?.ReadValue<Vector2>() ?? Vector2.zero;
    }
}
