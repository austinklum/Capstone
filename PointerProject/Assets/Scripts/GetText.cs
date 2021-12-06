using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRKeys;

public class GetText : MonoBehaviour
{
    public Keyboard keyboard;
	public VRInputModule VRInputModule;

	private void OnEnable()
	{	
		keyboard.Enable();
		keyboard.SetPlaceholderMessage("Please enter your name");

		keyboard.OnUpdate.AddListener(HandleUpdate);
		keyboard.OnSubmit.AddListener(HandleSubmit);
		keyboard.OnCancel.AddListener(HandleCancel);
	}

	private void OnDisable()
	{
		keyboard.OnUpdate.RemoveListener(HandleUpdate);
		keyboard.OnSubmit.RemoveListener(HandleSubmit);
		keyboard.OnCancel.RemoveListener(HandleCancel);

		keyboard.Disable();
	}

	private void Update()
	{

	}

	/// <summary>
	/// Hide the validation message on update. Connect this to OnUpdate.
	/// </summary>
	public void HandleUpdate(string text)
	{
		keyboard.HideValidationMessage();
	}

	/// <summary>
	/// Validate the email and simulate a form submission. Connect this to OnSubmit.
	/// </summary>
	public void HandleSubmit(string text)
	{
		keyboard.DisableInput();

        VRInputModule.StartWorld(text);

        keyboard.HideSuccessMessage();
		keyboard.SetText("");
		keyboard.Disable();
	}

	public void HandleCancel()
	{
		Debug.Log("Cancelled keyboard input!");
	}
}