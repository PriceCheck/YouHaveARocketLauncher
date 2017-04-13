using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure; // Required in C#
using UnityStandardAssets.ImageEffects;
using com.ootii.Messages;

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
    float rotationSpeed = 270;
    float MoveSpeed = 10;
    float Gravity = -0.7f;
    float Drag = 0.15f;
    float TerminalVel = -0.2f;
    float AirMovementDampening = 0.6f;
    float MovementMultiplier = 1;
    float JumpHeight = 0.3f;
    float VerticalMovement = 0;
    float triggerThreshold = 0.9f;
    //Explosion Stuff
    Vector3 ExplosionKnockback = Vector3.zero;
    float ExplosionAmount = 20.0f;
    //
    bool isJumping = false;
    bool firePressed = false;
    float blurCounter = 0;
    float maxBlurAmount = 5f;
    TiltShift PlayerTiltShift;

    public AnimationCurve TiltShiftRamp = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve LookRamp = AnimationCurve.EaseInOut(-1, 0, 1, 1);
    public AnimationCurve MoveRamp = AnimationCurve.EaseInOut(-1, 0, 1, 1);


    public void OnCollide()
    {

    }
    // Use this for initialization
    void Start() {
        myCharacterController = GetComponent<CharacterController>();
        PlayerTiltShift = myCamera.GetComponent<TiltShift>();

    }

    public void OnExplosionHit(Vector3 normal)
    {
        //        print("Exploded");
        ExplosionKnockback = normal * ExplosionAmount;
    }

    public void SetPlayerIndex(int Index)
    {
        CharacterID = Index;
        if (Index == 0)
        {
            Model.gameObject.layer = LayerMask.NameToLayer("Player1");
            myCamera.cullingMask = ~(1 << LayerMask.NameToLayer("Player1"));
        }
        else
        {
            Model.gameObject.layer = LayerMask.NameToLayer("Player2");
            myCamera.cullingMask = ~(1 << LayerMask.NameToLayer("Player2"));
        }
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
        Vector2 LookVector = new Vector2(state.ThumbSticks.Right.Y, state.ThumbSticks.Right.X);
        LookVector = LookRamp.Evaluate((LookVector.magnitude)) * LookVector.normalized;
        

        float VerticalLook = (-LookVector.x * rotationSpeed * Time.fixedDeltaTime) + RotationTarget.transform.rotation.eulerAngles.x;

        if(VerticalLook < 100)
        {
            VerticalLook = Mathf.Clamp(VerticalLook, -50, 70);
        }
        else
        {
            VerticalLook = Mathf.Clamp(VerticalLook, 360 - 50, 360 + 70);
        }
        VerticalLook -= RotationTarget.transform.rotation.eulerAngles.x;
        Vector3 newAngle = RotationTarget.transform.rotation.eulerAngles + new Vector3(VerticalLook, LookVector.y * rotationSpeed * Time.fixedDeltaTime, 0);//LookVector.y * rotationSpeed * Time.fixedDeltaTime);
        
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

        if (!isJumping && ExplosionKnockback.magnitude < 0.1 && state.Buttons.A == ButtonState.Pressed && myCharacterController.isGrounded)
        {
            VerticalMovement +=  JumpHeight;
            isJumping = true;
        }
        else if(isJumping && state.Buttons.A == ButtonState.Released)
        {
            isJumping = false;
        }
        //Cap vertical Movement
        VerticalMovement += Gravity * Time.fixedDeltaTime;
        VerticalMovement -= (VerticalMovement * Drag * Time.fixedDeltaTime);
        VerticalMovement = Mathf.Clamp(VerticalMovement, TerminalVel, float.MaxValue);
        DeltaMove += new Vector3(0, VerticalMovement, 0);
        //Cap Explosion Knockback

    
        if(ExplosionKnockback.magnitude > 0)
        {
            ExplosionKnockback -= 4 * (ExplosionKnockback * Drag * Time.fixedDeltaTime);
            ExplosionKnockback.y += 6 * Gravity * Time.fixedDeltaTime;
            ExplosionKnockback += DeltaMove * AirMovementDampening;
            myCharacterController.Move(ExplosionKnockback * Time.fixedDeltaTime);
        }
        else
        {
            myCharacterController.Move(DeltaMove);
        }

        //   print(ExplosionKnockback + " mag: " + ExplosionKnockback.magnitude);
        if ((myCharacterController.isGrounded && ExplosionKnockback.magnitude < 1.5))
        {
            ExplosionKnockback = Vector3.zero;
        }
        if (myCharacterController.isGrounded && ExplosionKnockback.magnitude < (3 * ExplosionAmount) / 4)
        {
            ExplosionKnockback = Vector3.zero;
        }
 
 
        //Tilt shift
        if (DeltaMove.magnitude > 0.4f && myCharacterController.isGrounded)
        {
            
            blurCounter += Time.deltaTime;

        }
        else
        {
            blurCounter -= Time.deltaTime;
        }
        blurCounter = Mathf.Clamp01(blurCounter);
        PlayerTiltShift.blurArea = maxBlurAmount * TiltShiftRamp.Evaluate(blurCounter);
        //End tilt shift
        //Fire
        //firePressed = ;
        if (firePressed)
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

    void onCollisionEnter(Collision col)
    {
        //  print(col.gameObject.name);
        if (col.gameObject.tag == "Boom" && col.gameObject.GetComponent<OuterExplosion>() != null && col.contacts.Length > 0)
        {
            //Bounce
            OnExplosionHit(col.contacts[0].normal);

            //   col.gameObject.BroadcastMessage("Bounce", direction, SendMessageOptions.DontRequireReceiver);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit col)
    {
        if(col.gameObject.GetComponent<DeathPlane>())
        {
            col.gameObject.GetComponent<DeathPlane>().CollisionResolution(this.gameObject);
        }
        if (col.gameObject.tag == "Boom" && col.gameObject.GetComponent<OuterExplosion>() != null)
        {
            //Bounce
            OnExplosionHit(col.normal);

            //   col.gameObject.BroadcastMessage("Bounce", direction, SendMessageOptions.DontRequireReceiver);
        }
    }
}
