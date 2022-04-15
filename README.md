# UI Culling for Unity

This package provides a Component that checks whether an uGUI widget is visible and triggers [OnBecameVisible](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnBecameVisible.html) and [OnBecameInvisible](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnBecameInvisible.html) events, as well as any ```On Visbility Changed``` Unity event that's wired-up through the Inspector.

By default, uGUI doesn't trigger the OnBecameVisible and OnBecameInvisible events, which are often useful to toggle expensive logic when an uGUI widget is outside a ScrollView viewport for example. This missing functionality in uGUI is for whatever reason "By Design", see [here](https://forum.unity.com/threads/onbecamevisible-does-not-fire-for-canvasrenderers.290641/#post-1918763) for details.



# Installation

In Unity's Package Manager, choose "Add package from git URL" and insert one of the Package URL's you can find below.


## Package URL's

| Version  |     Link      |
|----------|---------------|
| 1.0.0 | https://github.com/pschraut/UnityUICulling.git#1.0.0 |


# Credits

If you find this package useful, please mention my name in your credits screen.
Something like "UI Culling by Peter Schraut" or "Thanks to Peter Schraut" would be very much appreciated.

# How it works

The ```UICullingBehaviour``` implements [LateUpdate](https://docs.unity3d.com/ScriptReference/MonoBehaviour.LateUpdate.html) and transforms both RectTransform's, ```m_Rect``` and ```m_Viewport```, to world-space rectangles and checks whether they overlap.

This check is performed always (per frame), even when the uGUI widget is outside the screen.

I'm unable to get rid of this check, because Unity doesn't provide a callback when a Transform position changed.
