//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component that encapsulates the <see cref="EyeXActivatable"/> behavior.
/// </summary>
[AddComponentMenu("Tobii EyeX/Activatable")]
public class ActivatableComponent : EyeXGameObjectInteractorBase
{
    // Should tentative activation focus be enabled
    public bool enableTentativeActivationFocus;

    /// <summary>
    /// Gets a value indicating whether the game object has been activated.
    /// </summary>
    public bool IsActivated { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the game object has activation focus.
    /// </summary>
    public bool HasActivationFocus { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the game object has tentative activation focus.
    /// </summary>
    public bool HasTentativeActivationFocus { get; private set; }

    /// <summary>
    /// Triggers an activation on the game object the user is looking at (if it is activatable).
    /// </summary>
    public void Activate()
    {
        Host.TriggerActivation();
    }

    /// <summary>
    /// Trigger activation mode on.
    /// </summary>
    public void ActivationModeOn()
    {
        Host.TriggerActivationModeOn();
    }

    /// <summary>
    /// Trigger activation mode off.
    /// </summary>
    public void ActivationModeOff()
    {
        Host.TriggerActivationModeOff();
    }
        

    protected override void Update()
    {
        base.Update();

        IsActivated = GameObjectInteractor.IsActivated();

        var activationFocusState = GameObjectInteractor.GetActivationFocusState();
        HasActivationFocus = (activationFocusState == ActivationFocusState.HasActivationFocus);
        HasTentativeActivationFocus = (activationFocusState == ActivationFocusState.HasTentativeActivationFocus);
    }

    protected override IList<IEyeXBehavior> GetEyeXBehaviorsForGameObjectInteractor()
    {
        return new List<IEyeXBehavior>(new[] { new EyeXActivatable(EyeXHost.GetInstance().ActivationHub)
        {
            IsTentativeFocusEnabled = enableTentativeActivationFocus
        }});
    }
}
