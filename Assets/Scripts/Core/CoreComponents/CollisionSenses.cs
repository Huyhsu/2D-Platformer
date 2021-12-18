using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CollisionSenses : CoreComponent
{
    #region Check Transforms

    public  Transform GroundCheck
    {
        get
        {
            if (groundCheck)
            {
                return groundCheck;
            }
            
            Debug.LogError("No Ground Check on " + core.transform.parent.name);

            return null;
        }
        private set => groundCheck = value;
    }
    
    public  Transform WallCheck
    {
        get
        {
            if (wallCheck)
            {
                return wallCheck;
            }
            
            Debug.LogError("No Wall Check on " + core.transform.parent.name);

            return null;
        }
        private set => wallCheck = value;
    }
    
    public  Transform LedgeHorizontalCheck
    {
        get
        {
            if (ledgeHorizontalCheck)
            {
                return ledgeHorizontalCheck;
            }
            
            Debug.LogError("No Ledge Horizontal Check on " + core.transform.parent.name);

            return null;
        }
        private set => ledgeHorizontalCheck = value;
    }

    public Transform LedgeVerticalCheck
    {
        get
        {
            if (ledgeVerticalCheck)
            {
                return ledgeVerticalCheck;
            }
            
            Debug.LogError("No Ledge Vertical Check on " + core.transform.parent.name);

            return null;
        }
        private set => ledgeVerticalCheck = value;
    }
    
    public  Transform CeilingCheck
    {
        get
        {
            if (ceilingCheck)
            {
                return ceilingCheck;
            }
            
            Debug.LogError("No Ceiling Check on " + core.transform.parent.name);

            return null;
        }        private set => ceilingCheck = value;
    }
    
    public float GroundCheckRadius
    {
        get => groundCheckRadius;
        set => groundCheckRadius = value;
    }

    public float WallCheckDistance
    {
        get => wallCheckDistance;
        set => wallCheckDistance = value;
    }

    public LayerMask WhatIsGround
    {
        get => whatIsGround;
        set => whatIsGround = value;
    }
    
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform ledgeHorizontalCheck;
    [SerializeField] private Transform ledgeVerticalCheck;
    [SerializeField] private Transform ceilingCheck;

    [SerializeField] private float groundCheckRadius;
    [SerializeField] private float wallCheckDistance;
    
    [SerializeField] private LayerMask whatIsGround;
    
    #endregion

    public bool Ceiling
    {
        get => Physics2D.OverlapCircle(CeilingCheck.position, groundCheckRadius, whatIsGround);
    }

    public bool Ground
    {
        get => Physics2D.OverlapCircle(GroundCheck.position, groundCheckRadius, whatIsGround);
    }

    public bool WallFront
    {
        get => Physics2D.Raycast(WallCheck.position, Vector2.right * core.Movement.FacingDirection,
            wallCheckDistance,
            whatIsGround);
    }

    public bool LedgeHorizontal
    {
        get => Physics2D.Raycast(LedgeHorizontalCheck.position, Vector2.right * core.Movement.FacingDirection,
            wallCheckDistance,
            whatIsGround);
    }

    public bool LedgeVertical
    {
        get => Physics2D.Raycast(LedgeVerticalCheck.position, Vector2.down, wallCheckDistance, whatIsGround);
    }
    
    public bool WallBack
    {
        get => Physics2D.Raycast(WallCheck.position, Vector2.right * -core.Movement.FacingDirection, wallCheckDistance,
            whatIsGround);
    }
}
