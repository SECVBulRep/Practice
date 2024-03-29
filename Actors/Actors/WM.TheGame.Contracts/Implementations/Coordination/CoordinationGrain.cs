﻿using Orleans.EventSourcing;
using Orleans.Providers;
using WM.TheGame.Contracts.Contracts.Coordination;

namespace WM.TheGame.Contracts.Implementations.Coordination;

[LogConsistencyProvider(ProviderName = "LogStorage")]
public class CoordinationGrain : JournaledGrain<PlayerCoordinateState>,ICoordinationGrain
{
    
    protected override void OnStateChanged()
    {
        // read state and/or event log and take appropriate action
    }
    
    public async Task<(int,int)> GetInfo()
    {
        
        var temp = await RetrieveConfirmedEvents(0, Version);
        
        (int, int) ret = new (State.X,State.Y);
        return ret;
    }

    public Task MoveNorth(int steps)
    {
        RaiseEvent(new MovedToNorth()
        {
            Steps = steps
        });
        return ConfirmEvents();
    }

    public Task MoveWest(int steps)
    {
        RaiseEvent(new MovedToWest()
        {
            Steps = steps
        });
        return ConfirmEvents();
    }

    public Task MoveSouth(int steps)
    {
        RaiseEvent(new MovedToSouth()
        {
            Steps = steps
        });
        return ConfirmEvents();
    }

    public Task MoveEast(int steps)
    {
        RaiseEvent(new MovedToEast()
        {
            Steps = steps
        });
        return ConfirmEvents();
    }
}

public abstract class MovedEvent
{
    public int Steps { get; set; }
}

public class MovedToSouth : MovedEvent { }

public class MovedToNorth : MovedEvent { }

public class MovedToWest : MovedEvent { }

public class MovedToEast : MovedEvent { }


[GenerateSerializer]
public class PlayerCoordinateState
{

    [Id(0)]
    public int X { get; set; }
    
    [Id(1)]
    public int Y { get; set; }

    public PlayerCoordinateState Apply(MovedToSouth evnt)
    {
        Y -= evnt.Steps;
        return this;
    }
    
    public PlayerCoordinateState Apply(MovedToNorth evnt)
    {
        Y += evnt.Steps;
        return this;
    }
    
    public PlayerCoordinateState Apply(MovedToWest evnt)
    {
        X -= evnt.Steps;
        return this;
    }
    
    public PlayerCoordinateState Apply(MovedToEast evnt)
    {
        Y += evnt.Steps;
        return this;
    }

   
}
