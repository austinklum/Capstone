using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRKeys;

public class GetText : MonoBehaviour
{
    public Keyboard keyboard;

	private void OnEnable()
	{
		
		keyboard.Enable();
		keyboard.SetPlaceholderMessage("Please enter your email address");

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
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (keyboard.disabled)
			{
				keyboard.Enable();
			}
			else
			{
				keyboard.Disable();
			}
		}

		if (keyboard.disabled)
		{
			return;
		}

		if (Input.GetKeyDown(KeyCode.Q))
		{
			keyboard.SetLayout(KeyboardLayout.Qwerty);
		}
		else if (Input.GetKeyDown(KeyCode.F))
		{
			keyboard.SetLayout(KeyboardLayout.French);
		}
		else if (Input.GetKeyDown(KeyCode.D))
		{
			keyboard.SetLayout(KeyboardLayout.Dvorak);
		}
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

	
		keyboard.ShowValidationMessage("Please enter a valid email address");
		keyboard.EnableInput();
			

		keyboard.HideSuccessMessage();
		keyboard.SetText("");
		keyboard.EnableInput();
	}

	public void HandleCancel()
	{
		Debug.Log("Cancelled keyboard input!");
	}
}