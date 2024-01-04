# Simpletons

A basic state machine for programmers to prototype Unity AI nav agents in C#.

## Implementation

```cs
using UnityEngine;
using Simpleton;

public abstract partial class MyAiState : SimpletonState
{
    public MonoBehaviour owner;
}
```

It comes with an easy to implement inspector window:

<p float="center">
    <img src="https://github.com/andrew-raphael-lukasik/Simpletons/assets/3066539/f2c739d3-03d7-4764-9c4c-13270aa9799c" height="300px">
</p>

```cs
using Simpletons;

SimpletonStateMachine _stateMachine = new CivilianAI( this );

void InspectorThisAI ()
{
    SimpletonInspectorWindow.InspectAI( _stateMachine );
}
```

## Installation
Open `Package Manager`->`Add package from git URL`:
```
https://github.com/andrew-raphael-lukasik/Simpletons.git#upm
```

Add this line to the dependencies in `manifest.json`:
```
"dependencies": {
    "com.andrewraphaellukasik.simpletons": "https://github.com/andrew-raphael-lukasik/Simpletons.git#upm",
    ...
}
```

### Games that use this package:
- https://andrew-raphael-lukasik.itch.io/non-human-society
