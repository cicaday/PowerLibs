using System;
using System.Text;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;

public static class UiaExtension
{
    public static AutomationElement FindChildByProcessId(this AutomationElement element, int processId)
    {
        return element.FindChildBy(
            new PropertyCondition(AutomationElement.ProcessIdProperty, processId));
    }

    public static AutomationElement FindChildById(this AutomationElement e, string automationId)
    {
        Condition c = new PropertyCondition(AutomationElement.AutomationIdProperty, automationId);
        return e.FindChildBy(c);
    }

    public static AutomationElement FindChildByName(this AutomationElement e, string name)
    {
        Condition c = new PropertyCondition(AutomationElement.NameProperty, name);
        return e.FindChildBy(c);
    }

    public static AutomationElement FindChildByName(this AutomationElement e, string name, ControlType type)
    {
        Condition c = new AndCondition(new PropertyCondition(AutomationElement.NameProperty, name),
            new PropertyCondition(AutomationElement.ControlTypeProperty, type));
        return e.FindChildBy(c);
    }

    public static AutomationElement FindChildByControlType(this AutomationElement e, ControlType type)
    {
        Condition c = new PropertyCondition(AutomationElement.ControlTypeProperty, type);
        return e.FindChildBy(c);
    }

    public static AutomationElement FindChildBy(this AutomationElement e, string automationId, ControlType type)
    {
        Condition c = new AndCondition(new PropertyCondition(AutomationElement.ControlTypeProperty, type),
            new PropertyCondition(AutomationElement.AutomationIdProperty, automationId));
        return e.FindChildBy(c);
    }

    public static AutomationElement FindChildBy(this AutomationElement e, Condition condition)
    {
        return e.FindFirst(TreeScope.Children, condition);
    }

    public static AutomationElementCollection FindChildren(this AutomationElement e, Condition condition)
    {
        return e.FindAll(TreeScope.Children, condition);
    }

    public static AutomationElementCollection FindChildrenByControlType(this AutomationElement e, ControlType type)
    {
        Condition c = new PropertyCondition(AutomationElement.ControlTypeProperty, type);
        return e.FindAll(TreeScope.Children, c);
    }

    public static AutomationElement FindChildByIndex(this AutomationElement e, int index, Condition condition)
    {
        return e.FindChildren(condition)[index];
    }

    public static AutomationElement FindChildByIndex(this AutomationElement e, int index)
    {
        return e.FindChildren(Condition.TrueCondition)[index];
    }

    public static AutomationElement FindChildByIndex(this AutomationElement e, int index, ControlType type)
    {
        return e.FindChildrenByControlType(type)[index];
    }

    public static AutomationElement FindDesantantsById(this AutomationElement e, string automationId)
    {
        Condition c = new PropertyCondition(
                     AutomationElement.AutomationIdProperty,
                     automationId);

        return e.FindDescendantsBy(c);
    }

    public static AutomationElement FindDesantantsByName(this AutomationElement e, string name)
    {
        Condition c = new PropertyCondition(AutomationElement.NameProperty, name);
        return e.FindDescendantsBy(c);
    }

    public static AutomationElement FindDesantantsByName(this AutomationElement e, string name, ControlType type)
    {
        Condition c = new AndCondition(new PropertyCondition(AutomationElement.NameProperty, name),
            new PropertyCondition(AutomationElement.ControlTypeProperty, type));
        return e.FindDescendantsBy(c);
    }

    public static AutomationElement FindDesantantsByControlType(this AutomationElement e, ControlType type)
    {
        Condition c = new PropertyCondition(AutomationElement.ControlTypeProperty, type);
        return e.FindDescendantsBy(c);
    }

    public static AutomationElement FindDesantantsBy(this AutomationElement e, string automationId, ControlType type)
    {
        Condition c = new AndCondition(new PropertyCondition(AutomationElement.ControlTypeProperty, type),
            new PropertyCondition(AutomationElement.AutomationIdProperty, automationId));
        return e.FindDescendantsBy(c);
    }

    public static AutomationElement FindDescendantsBy(this AutomationElement e, Condition condition)
    {
        return e.FindFirst(TreeScope.Descendants, condition);
    }

    public static string GetValue(this AutomationElement e)
    {
        var p = e.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
        return p.Current.Value;
    }

    public static void Click(this AutomationElement e)
    {
        try
        {
            var p = e.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            p.Invoke();
        }
        catch (InvalidOperationException)
        {
            var p = e.GetCurrentPattern(TogglePattern.Pattern) as TogglePattern;
            p.Toggle();
        }
    }

    public static void InputValue(this AutomationElement e, string value)
    {
        var feedbackText = new StringBuilder();
        try
        {
            // Validate arguments / initial setup
            if (value == null)
                throw new ArgumentNullException(
                    "String parameter must not be null.");

            if (e == null)
                throw new ArgumentNullException(
                    "AutomationElement parameter must not be null");

            // Check #1: Is control enabled?
            if (!e.Current.IsEnabled)
            {
                throw new InvalidOperationException(
                    "The control with an AutomationID of "
                    + e.Current.AutomationId.ToString()
                    + " is not enabled.\n\n");
            }

            // Check #2: Are there styles that prohibit us from sending text to this control?
            if (!e.Current.IsKeyboardFocusable)
            {
                throw new InvalidOperationException(
                    "The control with an AutomationID of "
                    + e.Current.AutomationId.ToString()
                    + "is read-only.\n\n");
            }

            // Once you have an instance of an AutomationElement,
            // check if it supports the ValuePattern pattern.
            object valuePattern = null;

            // Control does not support the ValuePattern pattern
            // so use keyboard input to insert content.
            //
            // NOTE: Elements that support TextPattern
            //       do not support ValuePattern and TextPattern
            //       does not support setting the text of
            //       multi-line edit or document controls.
            //       For this reason, text input must be simulated
            //       using one of the following methods.
            //
            if (!e.TryGetCurrentPattern(
                ValuePattern.Pattern, out valuePattern))
            {
                feedbackText.Append("The control with an AutomationID of ")
                    .Append(e.Current.AutomationId.ToString())
                    .Append(" does not support ValuePattern.")
                    .AppendLine(" Using keyboard input.\n");

                // Set focus for input functionality and begin.
                e.SetFocus();

                // Pause before sending keyboard input.
                Thread.Sleep(100);

                // Delete existing content in the control and insert new content.
                SendKeys.SendWait("^{HOME}");   // Move to start of control
                SendKeys.SendWait("^+{END}");   // Select everything
                SendKeys.SendWait("{DEL}");     // Delete selection
                SendKeys.SendWait(value);
            }
            // Control supports the ValuePattern pattern so we can
            // use the SetValue method to insert content.
            else
            {
                feedbackText.Append("The control with an AutomationID of ")
                    .Append(e.Current.AutomationId.ToString())
                    .Append((" supports ValuePattern."))
                    .AppendLine(" Using ValuePattern.SetValue().\n");

                // Set focus for input functionality and begin.
                e.SetFocus();

                ((ValuePattern)valuePattern).SetValue(value);
            }
        }
        catch (Exception ex)
        {
            feedbackText.Append(ex.Message);
            throw new NotSupportedException(feedbackText.ToString());
        }
    }
}