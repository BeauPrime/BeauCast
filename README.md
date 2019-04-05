
# BeauCast

**Current Version: 0.9.0**  
Updated 5 April 2019 | Changelog (Not yet available)

## About
BeauCast is a messaging and event management framework for Unity3D.

### Table of Contents
1. [Basic Usage](#basic-usage)
2. [Advanced Features](#advanced-features)
3. [Tips and Tricks](#tips-and-tricks)
4. [Reference](#reference)
----------------

## Basic Usage

### Installing BeauCast

**Note:** BeauCast requires Unity version 5.2 or newer.

1. Download the package from the repository: [BeauCast.unitypackage](https://github.com/FilamentGames/BeauCast/raw/master/BeauCast.unitypackage)
2. Unpack into your project.

BeauCast uses the ``BeauCast`` namespace. You'll need to add the statement ``using BeauCast;`` to the top of any scripts using it.

### Creating a ``MsgType``

Messages in BeauCast are identified by a ``MsgType`` object.

```csharp
public class ExampleObject : MonoBehaviour
{
	static public class Msg
    {
		static public readonly MsgType SomeRandomEvent = MsgType.Declare("ExampleObject::SomeRandomEvent");
    	static public readonly MsgType<int> ValueChanged = MsgType.Declare<int>("ExampleObject::ValueChanged");
    }
    
    ...
}
```

_Documentation in progress_

### Creating a ``Messenger``

```csharp
public class ExampleObject : MonoBehaviour
{
	static public class Msg
    {
    	...
    }
    
    private Messenger m_Messenger;
    
    private void Awake()
    {
		m_Messenger = Messenger.Require(this);
	}
}
```

_Documentation in progress_

### Registering handlers

```csharp
public class ExampleObject : MonoBehaviour
{
	static public class Msg
    {
    	static public readonly MsgType SomeRandomEvent = MsgType.Declare("ExampleObject::SomeRandomEvent");
    	static public readonly MsgType<int> ValueChanged = MsgType.Declare<int>("ExampleObject::ValueChanged");
    }
    
	private Messenger m_Messenger;
    
    private void Awake()
    {
    	m_Messenger = Messenger.Require( this )
        	.Register( Msg.SomeRandomEvent, SomeRandomHandler )
            .Register( Msg.ValueChanged, ValueChangedHandler )
            ;
    }
    
    private void RegisterAdditionalListeners()
    {
    	m_Messenger.Register( Msg.SomeRandomEvent, SomeRandomEventHandlerNoArgs )
        	.Register( Msg.ValueChanged, ValueChangedHandlerWithArg )
            .Register( Msg.ValueChanged, ValueChangedHandlerNoArgs )
    }
    
    // Handlers can accept a Message type as a parameter
    private void SomeRandomHandler( Message message )
    {
    	Debug.Log( message );
    }
    
    private void SomeRandomEventHandlerNoArgs() { }
    
    private void ValueChangedHandler( Message message ) { }
    
    private void ValueChangedHandlerWithArg( int value ) { }
    
    private void ValueChangedHandlerNoArgs() { }
}
```

_Documentation in progress_

### Dispatching messages

```csharp
public class ExampleObject : MonoBehaviour
{
	static public class Msg
    {
		static public readonly MsgType Clicked = MsgType.Declare("ExampleObject::Clicked");
    }
    
	...
    
    private void OnMouseDown()
    {
    	m_Messenger.Broadcast(Msg.Clicked);
    }
}
```

Handlers will not normally be executed if either the GameObject or the Messenger are inactive. If they are unable to be executed, they will be queued up to execute the next frame, or the next if inactive that frame, and so on. The behavior determining when a handler can execute, along with when it is executed, can be configured for each message type. See [Configuring Message Behavior](#configuring-message-behavior) for more details.

### Deregistering handlers

Handlers for any given Messenger will be automatically deregistered once its associated GameObject is destroyed. Handlers can persist across scene loads as long as the associated objects are still valid. In most cases, you don't need to implement any ``OnDisable`` or ``OnDestroy`` logic to clean up handlers registered during the object's lifetime.

You can also manually deregister a handler by calling ``Deregister``. Using this in conjunction with ``Register``, you can selectively enable, disable, and reroute messages.

```csharp
public class NoNeedToCleanUp : MonoBehaviour
{
	static public readonly MsgType MsgA = MsgType.Declare(...);
	static public readonly MsgType MsgB = MsgType.Declare(...);
	
	private Messenger messenger;
	
	private void Awake()
	{
		// These will automatically deregister once this GameObject
		// is destroyed.
		messenger = Messenger.Require(this)
			.Register(MsgA, OnMsgA)
			.Register(MsgB, OnMsgB);
	}
	
	public void OnMsgA()
	{
		// You can also manually deregister a handler.
		messenger.Deregister(MsgA, OnMsgA);
	}
	
	public void OnMsgB()
	{
		messenger.Deregister(MsgB, OnMsgB);
	}
}
```

### Configuring Message behavior

_Documentation in progress_

## Advanced Features

_Documentation in progress_

## Tips and Tricks

_Documentation in progress_

## Reference

### Configuring Message Types

#### Timing

| **Function** | **Effect** |
| ------------ | ---------- |
| ``Queue`` | Handlers will be executed at the end of the frame. Default behavior. |
| ``Immediate`` | Handlers will be executed immediately. |

#### Inactive Objects

| **Function** | **Effect** |
| ------------ | ---------- |
| ``Force`` | Handlers will always execute on both active and inactive objects. |
| ``Discard`` | Handlers will not execute on inactive objects. |
| ``Queue`` | Handlers will queue to execute next frame on inactive objects, with an optional limit on requeues. Default behavior. |

**Note:** These are listed in order of evaluation priority. A message with a type marked ``Discard`` and ``Queue`` will disregard handlers on inactive objects and execute at the end of the frame.

#### Miscellaneous

| **Function** | **Effect** |
| ------------ | ---------- |
| ``Editor`` | Allows the message type to be displayed in the inspector. |
| ``OptionalArg`` | Arguments for the message are not required. |
| ``RequireHandler`` | Messages will throw an exception if no handlers are registered for the type. |
| ``Priority`` | If queued, messages will be executed according to priority. |
| ``Log`` | Generates debug logs in certain circumstances. |

_Documentation in progress_