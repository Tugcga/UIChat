# UI Chat component

![Screen with the program window](screen.png?raw=true)

## What is this

This is UI component for simple MMO-type chat window. For Unity of course. Features:

* Scrollable area with chat messages
* Background of the scroll area change the color when chat is activated and deactivated
* Automatically deactivate input string after sending message to the server
* Control limit of the entire messages in the chat
* Control limit of the one message length

## Setup

Drag chat prefab as child of the Canvas object. `ChatController` component has two public functions, which should be called from the main application: 

```csharp
void SendMessageString(string message)
{
	//...
}
``` 

and 

```csharp
void ComeMessageString(string message)
{
	//...
}
```

Call the first function `SendMessageString` to send message from input string to the server (you should manually implement sending to the server). Call the second function `ComeMessageString` when the new message come from the server (then it appears on the chat list automatically). Also you can add some filters inside the function `SendMessageString` before sending string to the server: cutout some symbols, trim spaces and so on.

Addon contains simple script `ChatInputController`. This script should be added to any object on the scene (for example to the camera). When the user press `Enter`, then this script activate the chat input line. Press `Enter` again, and the input line will be deactivated.
