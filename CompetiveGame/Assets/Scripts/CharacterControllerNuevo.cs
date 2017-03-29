using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure; // Required in C#

public class CharacterControllerNuevo : MonoBehaviour {
    public int CharacterID = -1;
    public bool Disabled = false;
    //Input Controller
    bool playerIndexSet = false;
    PlayerIndex playerIndex;
    GamePadState state;
    GamePadState prevState;
    //
    public RocketLauncher myGun;
    public GameObject RotationTarget;
    public MeshRenderer Model;
    public Camera myCamera;
    CharacterController myCharacterController;
    float rotationSpeed = 120;
    float MoveSpeed = 14;
    float Gravity = -0.9f;
    float Drag = 0.15f;
    float TerminalVel = -0.25f;
    float AirMovementDampening = 0.6f;
    float MovementMultiplier = 1;
    float JumpHeight = 0.3f;
    float VerticalMovement = 0;
    float triggerThreshold = 0.9f;
    bool isJumping = false;
    bool firePressed = false;


    public AnimationCurve LookRamp = AnimationCurve.EaseInOut(-1, 0, 1, 1);
    public AnimationCurve MoveRamp = AnimationCurve.EaseInOut(-1, 0, 1, 1);


    public void OnCollide()
    {

    }
	// Use this for initialization
	void Start () {
        myCharacterController = GetComponent<CharacterController>();
	}
	
   public  void FindController()
    {

        if (!playerIndexSet || !prevState.IsConnected)
        {
            //   for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)CharacterID;
                //print(testPlayerIndex + " " + CharacterID);
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (testState.IsConnected)
                {
                     Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                    playerIndex = testPlayerIndex;
                    playerIndexSet = true;
                  
                }
            }
        }
    }
    

    public void SetCamera(Rect input)
    {
        myCamera.rect = input;
    }

	// Update is called once per frame
	void FixedUpdate () {
        if(!playerIndexSet || Disabled)
        {
            return;
        }
        
        // Will find the first controller that is connected ans use it
       // FindController();

        prevState = state;
        state = GamePad.GetState(playerIndex);
        if(Disabled)
        {
            return;
        }
        
        //Rotation
        Vector2 LookVector = new Vector2(-state.ThumbSticks.Right.Y, state.ThumbSticks.Right.X);
        LookVector = LookRamp.Evaluate(LookVector.magnitude) * LookVector.normalized;

        Vector3 newAngle = RotationTarget.transform.rotation.eulerAngles + new Vector3(LookVector.x * rotationSpeed * Time.fixedDeltaTime, LookVector.y * rotationSpeed * Time.fixedDeltaTime, 0);//LookVector.y * rotationSpeed * Time.fixedDeltaTime);
        RotationTarget.transform.rotation = Quaternion.Euler(newAngle);
        //Ground Check
        if (myCharacterController.isGrounded)
        {
            VerticalMovement = 0;
            MovementMultiplier = 1;
        }
        else
        {
            MovementMultiplier = AirMovementDampening;
        }
        //Horizontal Movement
        Vector2 MoveVector = new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);
        MoveVector = MovementMultiplier * MoveRamp.Evaluate(MoveVector.magnitude) * MoveVector.normalized;

        Vector3 DeltaMove = Quaternion.Euler(new Vector3(0, newAngle.y, 0)) * new Vector3(MoveVector.x * MoveSpeed * Time.fixedDeltaTime, 0, MoveVector.y * MoveSpeed * Time.fixedDeltaTime);
        //Vertical Movement
        //Jump states

        if (!isJumping && state.Buttons.A == ButtonState.Pressed && myCharacterController.isGrounded)
        {
            VerticalMovement +=  JumpHeight;
            isJumping = true;
        }
        else if(isJumping && state.Buttons.A == ButtonState.Released)
        {
            isJumping = false;
        }
        VerticalMovement += Gravity * Time.fixedDeltaTime;
        VerticalMovement -= (VerticalMovement * Drag * Time.fixedDeltaTime);
        VerticalMovement = Mathf.Clamp(VerticalMovement, TerminalVel, float.MaxValue);
        DeltaMove += new Vector3(0, VerticalMovement, 0);

        myCharacterController.Move(DeltaMove);

        //Fire
        //firePressed = ;
        if(firePressed)
        {
            if(state.Triggers.Right < triggerThreshold)
            {
                firePressed = false;
            }
        }
        else
        {
            firePressed = state.Triggers.Right > triggerThreshold;
            if(firePressed)
            {
                myGun.onShoot();
            }
        }
            

    }
}
