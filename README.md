# Simpletons

A basic state machine for programmers to prototype Unity AI nav agents in C#.

## Implementation

Goal of this repo is to make fsm-based ai creation processs relatively easy and quick so it can be used to prototype games.

Samples to show you how to use Simpletons:
> Zombies and survivors following/avoiding each other
> 
> https://github.com/andrew-raphael-lukasik/Simpletons/tree/main/Samples/Zombies

> pedestrians moving across the street and reacting to traffic lights
> 
> https://github.com/andrew-raphael-lukasik/Simpletons/tree/main/Samples/TrafficLights
>
> ![GIF 11 02 2024 13-00-52](https://github.com/andrew-raphael-lukasik/Simpletons/assets/3066539/94946da3-957f-48c8-b6a7-7bc529acbfa0)

Key implementation points:

- `MonoBehaviour` updates it's state machine ai brain by calling `Tick(time)` method

```cs
public class WalkerController : MonoBehaviour
{
    [SerializeField] WalkerAI _brain = new WalkerAI( owner:this );
    void FixedUpdate ()
    {
        _brain.Tick( time:Time.time );
    }
}
```

- custom `SimpletonStateMachine` is a place where you design your fsm ai

```cs
public WalkerAI ( WalkerController owner )
{
    _owner = owner;

    // create states:
    var start   = new WalkerIdleState( _owner );
    var idle    = new WalkerIdleState( _owner );
    var wander  = new WalkerWanderState( _owner );
    var agro    = new WalkerAgroState( _owner );
    var attack  = new WalkerAttackState( _owner );
    var dead    = new WalkerDeadState( _owner );

    // create transitions:
    SimpletonStateTransition start_to_idle, idle_to_wander, wander_to_idle, any_to_agro, agro_to_idle, agro_to_attack, attack_to_agro, any_to_dead;
    start_to_idle = new SimpletonStateTransition(
        predicate:      (state,time) => true ,
        destination:    idle ,
        label:          nameof(start_to_idle)
    );
    idle_to_wander = new SimpletonStateTransition(
        predicate:      (state,time) => Random.Range(1,6)==1 ,// 20% chance
        destination:    wander ,
        label:          nameof(idle_to_wander)
    );
    wander_to_idle = new SimpletonStateTransition(
        predicate:      (state,time) => state.completed || time>state.timeExpectedEnd || _owner.navMeshAgent.remainingDistance<1f ,
        destination:    idle ,
        label:          nameof(wander_to_idle)
    );
    any_to_agro = new SimpletonStateTransition(
        predicate:      (state,time) => _owner.survivorsAround.Count!=0 ,
        destination:    agro ,
        label:          nameof(any_to_agro)
    );
    any_to_dead = new SimpletonStateTransition(
        predicate:      (state,time) => _owner.Health<=0 ,
        destination:    idle ,
        label:          nameof(any_to_dead)
    );
    /* more transition definitions */

    // assign transitions to states:
    start.transitions = new SimpletonStateTransition[]{
        start_to_idle
    };
    idle.transitions = new SimpletonStateTransition[]{
        any_to_dead ,
        idle_to_wander ,
        any_to_agro ,
    };
    /* more transitions assigned to states */

    // assign initial state
    _initial = start;
    Reset();
    ConstructorAssertions();
}
```

To help you understand and debug it, Simpletons come with an inspector window:

<p float="center">
    <img src="https://github.com/andrew-raphael-lukasik/Simpletons/assets/3066539/f2c739d3-03d7-4764-9c4c-13270aa9799c" height="300px">
</p>

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
