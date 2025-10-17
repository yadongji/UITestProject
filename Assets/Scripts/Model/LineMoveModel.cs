using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    left = 1,
    right = 2,
    foward = 3,
    back = 4,
    up = 5,
    down = 6
}

public enum MoveState 
{
    //匀速运动
    FixSpeed = 1,
    //匀变速
    FixAcceleration = 2,
    //静止
    Still = 3,
}

public class MoveStage 
{
    public int startTime { get; private set; }
    public int endTime { get; private set; }
    public int moveSpeed { get; private set; }
    public int acceleration { get; private set; }

    public MoveStage(int startTime, int endTime, int moveSpeed, int acceleration)
    {
        this.startTime = startTime;
        this.endTime = endTime;
        this.moveSpeed = moveSpeed;
        this.acceleration = acceleration;
    }
}

public class LineMoveModel : IModel
{
    public int modelID { get; private set; }
    //运动方向
    public Direction moveDirection { get; private set; }
    //初始位置
    public Vector3 initialPosition { get; private set; }
    //是否可以当做质点
    public bool isParticle { get; private set; }
    //运动阶段
    public List<MoveStage> moveStages { get; private set; }

    public void Clear()
    {
        
    }

    public void Initialize(int modelID, Direction moveDirection, Vector3 initialPosition, bool isParticle, List<MoveStage> moveStages)
    {
        this.modelID = modelID;
        this.moveDirection = moveDirection;
        this.initialPosition = initialPosition;
        this.isParticle = isParticle;
        this.moveStages = moveStages;
    }

    //自由落体的初始化
    public void Initialize(int modelID, Vector3 initialPosition, bool isParticle, List<MoveStage> moveStages)
    {
        this.modelID = modelID;
        moveDirection = Direction.down;
        this.initialPosition = initialPosition;
        this.isParticle = isParticle;
        this.moveStages = moveStages;
    }
}
