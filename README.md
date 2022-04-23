# UI Culling for Unity

This package provides a Component that checks whether an uGUI widget is inside a specific RectTransform and allows to trigger OnBecameVisible and OnBecameInvisible magic-methods and uGUI events.

These events are often useful to toggle expensive logic. For example to disable rendering to a RenderTexture that's shown in the UI (ie animated 3D character), but was moved outisde the ScrollView.

uGUI doesn't trigger the OnBecameVisible and OnBecameInvisible events on widgets. Unity Technologies was asked to implement it, but it seems they chose not to do it, see [here](https://forum.unity.com/threads/onbecamevisible-does-not-fire-for-canvasrenderers.290641/#post-1918763).

# Video
Below you can find a YouTube video where I explain what the ```UICullingBehaviour``` can be used for.

[![](http://img.youtube.com/vi/qvcg46J6wA8/0.jpg)](https://youtu.be/qvcg46J6wA8 "")

# UICullingBehaviour Component

The ```UICullingBehaviour``` Component is used to trigger these OnBecameVisible and OnBecameInvisible events for uGUI widgets.

![alt text](Documentation~/images/inspector.png "UI Culling Behaviour Inspector")

| Property  | Description|
|----------|---------------|
| ```Rect``` | A reference to a RectTransform whose rectangle is used to test whether it's inside the ```Viewport```, in which case the Rect is considered visible. Only spacial checks are performed, the alpha/transparency isn't considered. |
| ```Viewport``` | A reference to a RectTransform that represents the viewport, for example the visible area of a ScrollView. See above ```Rect``` description. |
| ```Message Mode``` | Unity features the [OnBecameVisible](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnBecameVisible.html) and [OnBecameInvisible](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnBecameInvisible.html) magic-methods. This property allows you to call them.<br><br>```Send``` will perform a [GameObject.SendMessage](https://docs.unity3d.com/ScriptReference/GameObject.SendMessage.html) on the gameObject where the UICullingBehaviour is added to.<br>```Broadcast``` will perform a [GameObject.BroadcastMessage](https://docs.unity3d.com/ScriptReference/GameObject.BroadcastMessage.html) on the gameObject where the UICullingBehaviour is added to.<br>```None``` will not Send/Broadcast any message. |
| ```On Visible Changed``` | An Unity Event that can be used to set up method callbacks through the Inspector. It's called when the ```Rect``` becomes visible or invisible in the ```Viewport```.  This event is raised even when ```Message Mode```is set to ```None```. |
| ```On Became Visible``` | An Unity Event that can be used to set up method callbacks through the Inspector. It's called when the ```Rect``` entered the ```Viewport```.  This event is raised even when ```Message Mode```is set to ```None```. |
| ```On Became Invisible``` | An Unity Event that can be used to set up method callbacks through the Inspector. It's called when the ```Rect``` exited ```Viewport```. This event is raised even when ```Message Mode```is set to ```None```. |

# Installation

In Unity's Package Manager, choose "Add package from git URL" and insert one of the Package URL's you can find below.


## Package URL's

Please see the ```CHANGELOG.md``` file to see what's changed in each version.

| Version  |     Link      |
|----------|---------------|
| 1.0.0-pre.1 | https://github.com/pschraut/UnityUICulling.git#1.0.0-pre.1 |


# Credits

If you find this package useful, please mention my name in your credits screen.
Something like "UI Culling for Unity by Peter Schraut" or "Thanks to Peter Schraut" would be very much appreciated.

# How it works

The ```UICullingBehaviour``` implements [LateUpdate](https://docs.unity3d.com/ScriptReference/MonoBehaviour.LateUpdate.html) and transforms both RectTransform's, ```m_Rect``` and ```m_Viewport```, to world-space rectangles and checks whether they overlap.

This check is performed always (per frame), even when the uGUI widget is outside the screen.

It seems I'm unable to get rid of the per-frame check, because Unity doesn't provide a callback when a Transform position changed. If you know how to get rid of it, please open an issue item with the solution.

# Samples

In ```Samples~\Sample1.unitypackage``` you can find the sample that I also show in the video.
