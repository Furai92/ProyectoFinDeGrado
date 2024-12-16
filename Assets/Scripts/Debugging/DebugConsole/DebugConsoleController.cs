using UnityEngine;
using UnityEngine.UIElements;

public class DebugConsoleController : UIMenu
{
    private Label _consoleText;
    private Button _sendButton;
    private TextField _consoleTextField;
	protected override string MainParentName { get { return "debug-console-menu-container"; } }

	public override void InitializeUI()
	{
        _consoleText = RootVisualElement.Q<Label>("console-text");
        _sendButton = RootVisualElement.Q<Button>("send-button");
        _consoleTextField = RootVisualElement.Q<TextField>("console-text-field");

        _sendButton.clicked += OnSendButtonClicked;
        _consoleTextField.RegisterCallback<KeyDownEvent>(OnTextFieldKeyDown);
        _consoleText.RegisterValueChangedCallback(OnConsoleTextChanged);
    }

    private void OnConsoleTextChanged(ChangeEvent<string> evt)
    {
        
    }

    private void OnTextFieldKeyDown(KeyDownEvent evt)
    {
        if (evt.keyCode == KeyCode.Return)
        {
            OnSendButtonClicked();
        }
    }

    private void OnScrollerValueChanged(float value)
    {
        
        // Calculate the number of lines in the text
        int lineCount = _consoleText.text.Split('\n').Length;
    }

    private void OnSendButtonClicked()
    {
        if (string.IsNullOrEmpty(_consoleTextField.text)) return;
        _consoleText.text = _consoleText.text + "\n" + _consoleTextField.text;
        _consoleTextField.value = "";
        _consoleTextField.Focus();
    }


}
