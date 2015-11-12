// Add references to the following assemblies:
//    System.Windows.Forms
//    UIAutomationClient
//    UIAutomationTypes

using System.Windows.Automation;
using System.Windows.Forms;

// Portions of code adapted from http://www.mathpirate.net/log/2009/09/27/swa-straight-outta-redmond/
public static void HandleIeAuthenticationDialog(string userName, string password)
{
	if(String.IsNullOrWhiteSpace(userName))
	{
		throw new ArgumentNullException(userName, "Must contain a value");
	}

	if(String.IsNullOrWhiteSpace(password))
	{
		throw new ArgumentNullException(password, "Must contain a value");
	}

	// Condition for finding all "pane" elements
	Condition paneCondition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Pane);

	// Conditions for finding windows with a class of type dialog that's labeled Windows Security
	Condition windowsSecurityCondition = new AndCondition(
						new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window),
						new PropertyCondition(AutomationElement.ClassNameProperty, "#32770"),
						new PropertyCondition(AutomationElement.NameProperty, "Windows Security"));

	// Conditions for finding list elements with an AutomationId of "UserList"
	Condition userListCondition = new AndCondition(
					new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.List),
					new PropertyCondition(AutomationElement.AutomationIdProperty, "UserList"));

	// Conditions for finding the account listitem element
	Condition userTileCondition = new AndCondition(
					new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem),
					new PropertyCondition(AutomationElement.ClassNameProperty, "CredProvUserTile"),
					new PropertyCondition(AutomationElement.NameProperty, "Use another account"));

	// Conditions for finding the OK button
	Condition submitButtonCondition = new AndCondition(
						new PropertyCondition(AutomationElement.IsEnabledProperty, true),
						new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button),
						new PropertyCondition(AutomationElement.AutomationIdProperty, "SubmitButton"));

	// Conditions for finding the edit controls
	Condition editCondition = new AndCondition(
					new PropertyCondition(AutomationElement.IsEnabledProperty, true),
					new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));

	Console.Write("Looking for credentials dialog...");

	// Find all "pane" elements that are children of the desktop
	AutomationElementCollection panes = AutomationElement.RootElement.FindAll(TreeScope.Children, paneCondition);

	bool foundSecurityDialog = false;

	// Iterate through the collection of "panes"
	foreach(AutomationElement pane in panes)
	{
		// Check to see if the current pane is labeled as IE
		if(pane.Current.Name.Contains("Windows Internet Explorer"))
		{
			// Ok, we found IE. Now find all children of the IE pane that meets the windowSecurityCondition defined above
			AutomationElement windowsSecurityDialog = pane.FindFirst(TreeScope.Children, windowsSecurityCondition);

			if (windowsSecurityDialog != null)
			{
				// Great, we found the dialog
				Console.WriteLine("found security dialog");

				foundSecurityDialog = true;

				// Grab the first child of the dialog that is a UserList
				AutomationElement userList = windowsSecurityDialog.FindFirst(TreeScope.Children, userListCondition);

				// Grab the first child of the UserList that is a UserTile
				AutomationElement userTile = userList.FindFirst(TreeScope.Children, userTileCondition);

				// Make sure the UserTile has focus so that we can see the UserName and Password edit boxes
				userTile.SetFocus();

				// Get all children of the UserTile that are edit controls
				AutomationElementCollection edits = userTile.FindAll(TreeScope.Children, editCondition);

				// Iterate thru the edit controls
				foreach(AutomationElement edit in edits)
				{
					if(edit.Current.AutomationId == "CredProvClearTextEdit")
					{
						// We found the username edit control. Let's set the contents of the box to the username.
						Console.WriteLine("Entering username");
						ValuePattern userNamePattern = (ValuePattern)edit.GetCurrentPattern(ValuePattern.Pattern);
						userNamePattern.SetValue(userName);
					}
					if(edit.Current.AutomationId == "CredProvPasswordEdit")
					{
						// We found the password edit control. Let's set the contents of the box to the password.
						Console.WriteLine("Entering password");
						ValuePattern userNamePattern = (ValuePattern)edit.GetCurrentPattern(ValuePattern.Pattern);
						userNamePattern.SetValue(password);
					}
				}

				// Find the first child of the security dialog that meets the submitButtonCondition defined above
				AutomationElement submitButton = windowsSecurityDialog.FindFirst(TreeScope.Children, submitButtonCondition);

				// Now press the button
				InvokePattern buttonPattern = (InvokePattern)submitButton.GetCurrentPattern(InvokePattern.Pattern);
				buttonPattern.Invoke();

				break;
			}
		}
	}

	if(!foundSecurityDialog)
	{
		Console.WriteLine("no security dialogs found.");
	}
}